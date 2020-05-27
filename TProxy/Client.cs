using TProxy.Packets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;

namespace TProxy
{
    enum PlayerState
    {
        Connecting = 0,
        onWorld = 1,
        inVoid = 2
    }

    class Client
    {
        public string IP;
        public Player player;

        //public Dictionary<byte[], int> ClientqueuedPackages = new Dictionary<byte[], int>();

        public Socket client;

        public Connection connection;

        public PlayerState state = 0;

        public bool inTransfer = false;

        private byte[] buffer = new byte[131070];

        public volatile bool isConnected;
        public volatile bool onConnect;

        public DateTime LastChange = DateTime.UtcNow;

        public int slot;

        public byte index;

        Thread ListenPlayer;
        Thread ListenServer;
        Thread ListenTransfer;

        public Client(Socket tcp, int port, byte index)
        {
            this.index = index;
            client = tcp;
            client.NoDelay = true;
            IP = ((IPEndPoint)client.RemoteEndPoint).Address.ToString();
            isConnected = true;

            byte[] buffer = new byte[4096];

            slot = 0;

            player = new Player();


            try
            {
                ListenPlayer = new Thread(Listener)
                {
                    IsBackground = true
                };
                ListenPlayer.Start(port);
                Console.WriteLine($"Connection from ({IP}). [{TProxy.config.Servers[0].port}]");
            }
            catch (Exception e)
            {
                isConnected = false;
                TProxy.avalibleClient[index] = true;
                Console.WriteLine("Client Create: " + e.Message + '\n' + e.StackTrace);
            }


            connection = new Connection();

            player.empty = true;

            try
            {
                connection.tcp = new Socket(SocketType.Stream, ProtocolType.Tcp);
                connection.tcp.Connect("127.0.0.1", TProxy.config.Servers[0].port);
                connection.tcp.NoDelay = true;
                connection.name = TProxy.config.Servers[0].name;
                connection.port = TProxy.config.Servers[0].port;

                ListenServer = new Thread(new ParameterizedThreadStart(FromServer))
                {
                    IsBackground = true
                };
                ListenServer.Start(port);
            }
            catch (Exception e)
            {

                isConnected = false;
                TProxy.avalibleClient[index] = true;
                Console.WriteLine("Client Server Create: " + e.Message + '\n' + e.StackTrace);
            }

        }

        private void Listener(object port)
        {
            try
            {
                var lengthBuffer = new byte[2];
                var packetTypeBuffer = new byte[1];
                while (true)
                {
                    client.Receive(lengthBuffer, 2, SocketFlags.None);
                    ushort dataLength = (ushort)(BitConverter.ToUInt16(lengthBuffer, 0) - 3);
                    client.Receive(packetTypeBuffer, 1, SocketFlags.None);
                    PacketTypes packetType = (PacketTypes)packetTypeBuffer[0];
                    var readed = 0;
                    while (dataLength - readed != 0)
                    {
                        if (client.Available < 1)
                            Thread.Sleep(10);

                        readed += client.Receive(buffer, readed, dataLength - readed, SocketFlags.None);
                    }


                    //Console.WriteLine("CLIENT -> " + packetType);

                    if (!PacketManager.DeserializeFromPlayer(packetType, buffer.Take(dataLength).ToArray(), this) && !inTransfer)
                    {

                        while (connection == null || connection.tcp == null)
                        {
                             connection.SendData(PacketTypes.ConnectRequest);
                            Thread.Sleep(15);
                        }
                        Try_01:
                        try
                        {
                            if (connection.tcp.Connected)
                            {
                                connection.tcp.Send(lengthBuffer, lengthBuffer.Length, SocketFlags.None);
                                connection.tcp.Send(packetTypeBuffer, packetTypeBuffer.Length, SocketFlags.None);
                                connection.tcp.Send(buffer, dataLength, SocketFlags.None);
                            }
                        } catch (SocketException)
                        {
                            goto Try_01;
                        }


                    }

                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Client Listen: " + e.Message + '\n' + e.StackTrace);
            }


            Console.WriteLine($"({IP}) disconnected. [Exit]");

            isConnected = false;
            TProxy.avalibleClient[index] = true;
            try
            {
                if (connection.tcp != null)
                {
                    connection.tcp.Close();
                }

                if (client != null)
                    client.Close();
            }
            catch (Exception) { }
        }




        private void FromServer(object port)
        {
            try
            {
                onConnect = true;
                connection.buffer = new byte[131070];
                var lengthBuffer = new byte[2];
                var packetTypeBuffer = new byte[1];
                while (connection.tcp.Receive(lengthBuffer, 2, SocketFlags.None) != 0)
                {
                    ushort dataLength = (ushort)(BitConverter.ToUInt16(lengthBuffer, 0) - 3);
                    connection.tcp.Receive(packetTypeBuffer, 1, SocketFlags.None);
                    PacketTypes packetType = (PacketTypes)packetTypeBuffer[0];
                    var readed = 0;
                    while (dataLength - readed != 0)
                    {
                        if (connection.tcp.Available < 1)
                            Thread.Sleep(10);

                        readed += connection.tcp.Receive(connection.buffer, readed, dataLength - readed, SocketFlags.None);

                    }

                    //Console.WriteLine("SERVER -> " + packetType);

                    //if (packetType == PacketTypes.WorldInfo)
                    //{
                    //    connection.buffer[6] = 208;
                    //    connection.buffer[7] = 32;
                    //    connection.buffer[8] = 96;
                    //    connection.buffer[9] = 9;
                    //}

                    if (!PacketManager.DeserializeFromServer(packetType, connection.buffer.Take(dataLength).ToArray(), this))
                    {
                        client.Send(lengthBuffer, lengthBuffer.Length, SocketFlags.None);
                        client.Send(packetTypeBuffer, packetTypeBuffer.Length, SocketFlags.None);
                        client.Send(connection.buffer, dataLength, SocketFlags.None);
                    }
                }
            }
            catch (SocketException e)
            {
                if (e.ErrorCode != 10053)
                {
                    Console.WriteLine("Server Listen: " + e.Message + '\n' + e.StackTrace + "\n\n" + e.ErrorCode);
                }
            }
            catch (ObjectDisposedException) { }


            try
            {

                onConnect = false;

                if (connection.tcp.Connected)
                    connection.tcp.Close();

                if (client.Connected)
                    SendMessage($"[c/595959:<] [c/52e092:TProxy] [c/595959:>]  Odlaczono od [c/66ff66:{connection.name}].", 255, 255, 255);
            }
            catch (Exception) { }
        }

        private void Transfer(object port)
        {
            inTransfer = true;

            if (connection.tcp != null)
            {
                connection.tcp.Close();
            }

            Console.WriteLine($"({IP}) -> [{(int)port}]");
            try
            {
                connection.tcp = new Socket(SocketType.Stream, ProtocolType.Tcp);
                connection.tcp.Connect("127.0.0.1", (int)port);
                connection.tcp.NoDelay = true;
            }
            catch (SocketException)
            {
                SendMessage("[c/595959:<] [c/52e092:TProxy] [c/595959:>]  Serwer jest wylaczony. Przenosze do [c/66ff66:Lobby].", 128, 128, 128);
                ConnectTo(TProxy.config.Servers[0].port);
                return;
            }
            catch (Exception e)
            {
                Console.WriteLine("Server Transfer Create Listen: " + e.Message + '\n' + e.StackTrace);
                return;
            }
            connection.name = TProxy.config.Servers.First(x => x.port == (int)port).name;


            connection.SendData(PacketTypes.ConnectRequest);
            connection.SendProxyData(ProxyMessage.UpdateIP, IP);

            int isConnectedFully = 0;

            connection.buffer = new byte[131070];
            var lengthBuffer = new byte[2];
            var packetTypeBuffer = new byte[1];
            try
            {
                while (isConnectedFully != 2)
                {

                    connection.tcp.Receive(lengthBuffer, 2, SocketFlags.None);
                    ushort dataLength = (ushort)(BitConverter.ToUInt16(lengthBuffer, 0) - 3);
                    connection.tcp.Receive(packetTypeBuffer, 1, SocketFlags.None);
                    PacketTypes packetType = (PacketTypes)packetTypeBuffer[0];
                    var readed = 0;
                    while (dataLength - readed != 0)
                    {
                        readed += connection.tcp.Receive(connection.buffer, readed, dataLength - readed, SocketFlags.None);
                    }

                    //Console.WriteLine(packetType);

                    //2 -> Send packet to client and then go to NormalLoop
                    //1 -> Continue loop but don't send packet to Client
                    //0 -> Send packet to client and continue loop

                    isConnectedFully = PacketManager.DeserializeFromTransfer(packetType, connection.buffer.Take(dataLength).ToArray(), this);

                    if (isConnectedFully != 1)
                    {
                        client.Send(lengthBuffer, lengthBuffer.Length, SocketFlags.None);
                        client.Send(packetTypeBuffer, packetTypeBuffer.Length, SocketFlags.None);
                        client.Send(connection.buffer, dataLength, SocketFlags.None);
                    }
                }
            }
            catch (SocketException)
            {

            }
            catch (ObjectDisposedException) 
            { 
            
            }
            catch (Exception e)
            {
                Console.WriteLine("Server Transfer: " + e.Message + '\n' + e.StackTrace);
            }



            Thread ListenServer = new Thread(new ParameterizedThreadStart(FromServer))
            {
                IsBackground = true
            };
            ListenServer.Start(port);

            inTransfer = false;

        }


        public void Disconnect(string Message, string code = "none")
        {
            SendData(PacketTypes.Disconnect, Message);
            Console.WriteLine($"({IP}) disconnected. [{code}]");
            isConnected = false;
            client.Close();
        }

        public void ConnectTo(int port)
        {
            if (connection.tcp != null)
            {
                connection.tcp.Close();
            }

            SendData(PacketTypes.Teleport, null, 0, player.playerid, 64 * 16, 64 * 16, 3);

            connection.port = port;

            SendData(PacketTypes.PlayerAddBuff, null, player.playerid, 149, 330);

            ListenTransfer = new Thread(new ParameterizedThreadStart(Transfer))
            {
                IsBackground = true
            };
            ListenTransfer.Start(port);
        }

        public void SendMessage(string Message, int R = 255, int G = 255, int B = 255)
        {
                SendData(PacketTypes.SmartTextMessage, Message, 255, R, G, B, -1);
        }

        public void SendData(PacketTypes msgType, string text = null, int number = 0, float number2 = 0.0f, float number3 = 0.0f, float number4 = 0.0f, int number5 = 0, int number6 = 0, int number7 = 0)
        {
            if (client.Connected)
            {
                MemoryStream message = PacketManager.Serialize(msgType, NetworkText.FromLiteral(text), number, number2, number3, number4, number5, number6, number7);

                try
                {
                    client.Send(message.ToArray(), message.ToArray().Length, SocketFlags.None);
                }
                catch (IOException)
                {
                    return;
                }
            }
        }
    }
    class Connection
    {
        public Socket tcp;
        public string name;
        public int port; 
        public byte[] buffer;

        public int worldSpawnX;
        public int worldSpawnY;

        public void SendData(PacketTypes msgType, string text = null, int number = 0, float number2 = 0.0f, float number3 = 0.0f, float number4 = 0.0f, int number5 = 0, int number6 = 0, int number7 = 0)
        {
            try
            {
                MemoryStream message = PacketManager.Serialize(msgType, NetworkText.FromLiteral(text), number, number2, number3, number4, number5, number6, number7);
                tcp.Send(message.ToArray(), message.ToArray().Length, SocketFlags.None);
            }
            catch (Exception) { }
        }

        public void SendProxyData(ProxyMessage type, string text = null, int number = 0, float number2 = 0.0f, float number3 = 0.0f, int number4 = 0)
        {
            try
            {
                MemoryStream message = PacketManager.SerializeProxyMessage(type, NetworkText.FromLiteral(text), number, number2, number3, number4);
                tcp.Send(message.ToArray(), message.ToArray().Length, SocketFlags.None);
            }
            catch (Exception) { }
        }
    }

}


