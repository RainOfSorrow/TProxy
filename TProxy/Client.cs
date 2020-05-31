using TProxy.Packets;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Linq;

namespace TProxy
{
    internal class Client
    {
        public string Ip;
        public Player Player;

        //public Dictionary<byte[], int> ClientqueuedPackages = new Dictionary<byte[], int>();

        public Socket Tcp;

        public Connection Connection;
        public Connection TransferConnection;

        public PlayerState State = 0;

        public bool InTransfer;

        private readonly byte[] _buffer = new byte[131070];

        public volatile bool IsConnected;
        public volatile bool OnConnect;

        public DateTime LastChange = DateTime.UtcNow;

        public int slot;

        public byte index;

        private Thread _listenPlayer;
        private Thread _listenServer;
        public Thread _listenTransfer;

        public Client(Socket tcp, byte index, int port = 7778)
        {
            this.index = index;
            this.Tcp = tcp;
            this.Tcp.NoDelay = true;
            Ip = ((IPEndPoint)this.Tcp.RemoteEndPoint).Address.ToString();
            IsConnected = true;

            slot = 0;

            Player = new Player();


            try
            {
                _listenPlayer = new Thread(Listener)
                {
                    IsBackground = true
                };
                _listenPlayer.Start(port);
                Console.WriteLine($"Connection from ({Ip}). [{TProxy.Config.Servers[0].Port}]");
            }
            catch (Exception e)
            {
                IsConnected = false;
                TProxy.AvalibleClient[index] = true;
                Console.WriteLine($"Client Create: {e.Message}\n{e.StackTrace}");
                IsConnected = false;
                TProxy.AvalibleClient[index] = true;
                return;
            }


            Connection = new Connection();
            TransferConnection = new Connection();

            Player.empty = true;

            try
            {
                Connection.Tcp = new Socket(SocketType.Stream, ProtocolType.Tcp);
                Connection.Tcp.Connect("127.0.0.1", TProxy.Config.Servers[0].Port);
                Connection.Tcp.NoDelay = true;
                Connection.Name = TProxy.Config.Servers[0].Name;
                Connection.Port = TProxy.Config.Servers[0].Port;

                _listenServer = new Thread(FromServer)
                {
                    IsBackground = true
                };
                _listenServer.Start(port);
            }
            catch (Exception e)
            {

                IsConnected = false;
                TProxy.AvalibleClient[index] = true;
                Console.WriteLine("Client Server Create: " + e.Message + '\n' + e.StackTrace);
                
            }

        }

        private void Listener(object port)
        {
            try
            {
                var lengthBuffer = new byte[2];
                var packetTypeBuffer = new byte[1];
                while (Tcp.Receive(lengthBuffer, 2, SocketFlags.None) != 0)
                {
                    ushort dataLength = (ushort)(BitConverter.ToUInt16(lengthBuffer, 0) - 3);
                    Tcp.Receive(packetTypeBuffer, 1, SocketFlags.None);
                    PacketTypes packetType = (PacketTypes)packetTypeBuffer[0];
                    var readed = 0;
                    while (dataLength - readed != 0)
                    {
                        if (!Tcp.Connected)
                            throw new Exception("Disconnected");
                        
                        if (Tcp.Available < 1)
                            Thread.Sleep(10);
                        
                        readed += Tcp.Receive(_buffer, readed, dataLength - readed, SocketFlags.None);
                    }


                    Console.WriteLine("CLIENT -> " + packetType);

                    if (PacketManager.DeserializeFromPlayer(packetType, _buffer.Take(dataLength).ToArray(), this) ||
                        InTransfer) continue;
                    while (Connection?.Tcp == null)
                    {
                        Connection?.SendData(PacketTypes.ConnectRequest);
                        Thread.Sleep(15);
                    }
                    Try_01:
                    try
                    {
                        if (Connection.Tcp.Connected)
                        {
                            Connection.Tcp.Send(lengthBuffer, lengthBuffer.Length, SocketFlags.None);
                            Connection.Tcp.Send(packetTypeBuffer, packetTypeBuffer.Length, SocketFlags.None);
                            Connection.Tcp.Send(_buffer, dataLength, SocketFlags.None);
                        }
                    } catch (SocketException)
                    {
                        goto Try_01;
                    }

                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Client Listen: " + e.Message + '\n' + e.StackTrace);
            }


            Console.WriteLine($"({Ip}) disconnected. [Exit]");

            IsConnected = false;
            TProxy.AvalibleClient[index] = true;
            try
            {
                Connection?.Tcp?.Close();

                Tcp?.Close();
            }
            catch (Exception)
            {
                // ignored
            }
        }




        private void FromServer(object port)
        {
            try
            {
                OnConnect = true;
                Connection.Buffer = new byte[131070];
                var lengthBuffer = new byte[2];
                var packetTypeBuffer = new byte[1];
                while (true)
                {
                    Connection.Tcp.Receive(lengthBuffer, 2, SocketFlags.None);
                    ushort dataLength = (ushort) (BitConverter.ToUInt16(lengthBuffer, 0) - 3);
                    Connection.Tcp.Receive(packetTypeBuffer, 1, SocketFlags.None);
                    var packetType = (PacketTypes) packetTypeBuffer[0];
                    var readed = 0;
                    while (dataLength - readed != 0)
                    {
                        if (!Connection.Tcp.Connected)
                            throw new Exception("Disconnected"); 
                        
                        if (Connection.Tcp.Available < 1)
                            Thread.Sleep(10);
                        

                        readed += Connection.Tcp.Receive(Connection.Buffer, readed, dataLength - readed,
                            SocketFlags.None);

                    }

                    //Console.WriteLine("SERVER -> " + packetType);

                    //if (packetType == PacketTypes.WorldInfo)
                    //{
                    //    connection.buffer[6] = 208;
                    //    connection.buffer[7] = 32;
                    //    connection.buffer[8] = 96;
                    //    connection.buffer[9] = 9;
                    //}

                    if (!PacketManager.DeserializeFromServer(packetType, Connection.Buffer.Take(dataLength).ToArray(), this))
                    {
                        Tcp.Send(lengthBuffer, lengthBuffer.Length, SocketFlags.None);
                        Tcp.Send(packetTypeBuffer, packetTypeBuffer.Length, SocketFlags.None);
                        Tcp.Send(Connection.Buffer, dataLength, SocketFlags.None);
                    }
                }
            }
            catch (SocketException e)
            {
                if (e.ErrorCode != 10053 || e.ErrorCode != 43 || e.ErrorCode != 104 || e.ErrorCode != 32)
                {
                    Console.WriteLine("Server Listen: " + e.Message + '\n' + e.StackTrace + "\n\n" + e.ErrorCode);
                }
            }
            catch (ObjectDisposedException)
            {
            }
            catch (Exception)
            {
                //Ignore
            }


            try
            {

                OnConnect = false;

                if (Connection.Tcp.Connected)
                    Connection.Tcp.Close();

                if (Tcp.Connected)
                    SendMessage($"[c/595959:<] [c/52e092:TProxy] [c/595959:>]  Odlaczono od [c/66ff66:{Connection.Name}].");
            }
            catch (Exception)
            {
                // ignored
            }
        }

        private void Transfer(object port)
        {

            
            
            try
            {
                TransferConnection.Tcp = new Socket(SocketType.Stream, ProtocolType.Tcp);
                TransferConnection.Tcp.Connect("127.0.0.1", (int)port);
                TransferConnection.Tcp.NoDelay = true;
            }
            catch (SocketException)
            {
                SendMessage($"[c/595959:<] [c/52e092:TProxy] [c/595959:>]  Serwer jest wylaczony.", 128, 128, 128);
                return;
            }
            catch (Exception e)
            {
                Console.WriteLine("Server Transfer Create Listen: " + e.Message + '\n' + e.StackTrace);
                SendMessage($"[c/595959:<] [c/52e092:TProxy] [c/595959:>]  Cos poszlo nie tak, zglos to dla administratora.", 128, 128, 128);
                return;
            }
            InTransfer = true;
            Connection.Tcp?.Close();

            Console.WriteLine($"({Ip}) -> [{(int)port}]");
            
            
            SendData(PacketTypes.Teleport, null, 0, Player.playerid, 64 * 16, 64 * 16, 3);

            TransferConnection.Port = (int)port;

            SendData(PacketTypes.PlayerAddBuff, null, Player.playerid, 149, 330);
            
            TransferConnection.Name = TProxy.Config.Servers.First(x => x.Port == (int)port).Name;


            TransferConnection.SendData(PacketTypes.ConnectRequest);
            TransferConnection.SendProxyData(ProxyMessage.UpdateIP, Ip);

            var isConnectedFully = 0;

            TransferConnection.Buffer = new byte[131070];
            var lengthBuffer = new byte[2];
            var packetTypeBuffer = new byte[1];
            try
            {
                while (isConnectedFully != 2)
                {

                    TransferConnection.Tcp.Receive(lengthBuffer, 2, SocketFlags.None);
                    ushort dataLength = (ushort)(BitConverter.ToUInt16(lengthBuffer, 0) - 3);
                    TransferConnection.Tcp.Receive(packetTypeBuffer, 1, SocketFlags.None);
                    PacketTypes packetType = (PacketTypes)packetTypeBuffer[0];
                    var readed = 0;
                    while (dataLength - readed != 0)
                    {
                        if (TransferConnection.Tcp.Available < 1)
                            Thread.Sleep(10);

                        if (!TransferConnection.Tcp.Connected)
                            throw new Exception("Disconnected");

                        readed += TransferConnection.Tcp.Receive(TransferConnection.Buffer, readed, dataLength - readed, SocketFlags.None);
                    }

                    Console.WriteLine(packetType);

                    //2 -> Send packet to client and then go to NormalLoop
                    //1 -> Continue loop but don't send packet to Client
                    //0 -> Send packet to client and continue loop

                    isConnectedFully = PacketManager.DeserializeFromTransfer(packetType, TransferConnection.Buffer.Take(dataLength).ToArray(), this);

                    if (isConnectedFully != 1)
                    {
                        Tcp.Send(lengthBuffer, lengthBuffer.Length, SocketFlags.None);
                        Tcp.Send(packetTypeBuffer, packetTypeBuffer.Length, SocketFlags.None);
                        Tcp.Send(TransferConnection.Buffer, dataLength, SocketFlags.None);
                    }
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine("Server Transfer Socket: " + e.Message + '\n' + e.StackTrace);
            }
            catch (ObjectDisposedException e) 
            { 
                Console.WriteLine("Server Transfer Disposed: " + e.Message + '\n' + e.StackTrace);
            }
            catch (Exception e)
            {
                Console.WriteLine("Server Transfer: " + e.Message + '\n' + e.StackTrace);
                SendMessage($"[c/595959:<] [c/52e092:TProxy] [c/595959:>]  Cos poszlo nie tak, zglos to dla administratora. Przenosze do [c/66ff66:{TProxy.Config.Servers[0].Name}].", 128, 128, 128);
                ConnectTo(TProxy.Config.Servers[0].Port);
                return;
            }

            Connection = TransferConnection;
            TransferConnection = new Connection();

            Thread listenServer = new Thread(FromServer)
            {
                IsBackground = true
            };
            listenServer.Start(port);

            InTransfer = false;
        }


        public void Disconnect(string message, string code = "none")
        {
            SendData(PacketTypes.Disconnect, message);
            Console.WriteLine($"({Ip}) disconnected. [{code}]");
            IsConnected = false;
            Tcp.Close();
        }

        public void ConnectTo(int port)
        {

            _listenTransfer = new Thread(Transfer)
            {
                IsBackground = true
            };
            _listenTransfer.Start(port);
        }

        public void SendMessage(string message, int r = 255, int g = 255, int b = 255)
        {
                SendData(PacketTypes.SmartTextMessage, message, 255, r, g, b, -1);
        }

        public void SendData(PacketTypes msgType, string text = null, int number = 0, float number2 = 0.0f, float number3 = 0.0f, float number4 = 0.0f, int number5 = 0, int number6 = 0, int number7 = 0)
        {
            if (Tcp.Connected)
            {
                MemoryStream message = PacketManager.Serialize(msgType, NetworkText.FromLiteral(text), number, number2, number3, number4, number5, number6, number7);

                try
                {
                    Tcp.Send(message.ToArray(), message.ToArray().Length, SocketFlags.None);
                }
                catch (IOException)
                {
                }
            }
        }
    
        
        public void ClearNPCs()
        {
            for (int i = 0; i < 201; i++)
                SendData(PacketTypes.NpcUpdate, number: i);
        }

        public void ClearItems()
        {
            for (int i = 0; i < 401; i++)
                SendData(PacketTypes.UpdateItemDrop, number: i);
        }

        public void ClearPlayers()
        {
            for (int i = 0; i < 256; i++)
            {
                if (i == Player.playerid)
                    continue;

                SendData(PacketTypes.PlayerActive, number: i);
            }
        }

        public void ClearPylons()
        {
             try
             {
                 for (int i = 0; i < 9; i++)
                 {
                     byte[] packet = PacketManager.SerializeModule(NetModuleType.TeleportPylonModule, null, 1, 0, 0, i)
                         .ToArray();
                     Tcp.Send(packet, packet.Length, SocketFlags.None);
                 }
             }
             catch (Exception)
             {
                 // ignored
             }
        }
    }
    class Connection
    {
        public Socket Tcp;
        public string Name;
        public int Port; 
        public byte[] Buffer;

        public int WorldSpawnX;
        public int WorldSpawnY;

        public void SendData(PacketTypes msgType, string text = null, int number = 0, float number2 = 0.0f, float number3 = 0.0f, float number4 = 0.0f, int number5 = 0, int number6 = 0, int number7 = 0)
        {
            try
            {
                MemoryStream message = PacketManager.Serialize(msgType, NetworkText.FromLiteral(text), number, number2, number3, number4, number5, number6, number7);
                Tcp.Send(message.ToArray(), message.ToArray().Length, SocketFlags.None);
            }
            catch (Exception)
            {
                // ignored
            }
        }

        public void SendProxyData(ProxyMessage type, string text = null, int number = 0, float number2 = 0.0f, float number3 = 0.0f, int number4 = 0)
        {
            try
            {
                MemoryStream message = PacketManager.SerializeProxyMessage(type, NetworkText.FromLiteral(text), number, number2, number3, number4);
                Tcp.Send(message.ToArray(), message.ToArray().Length, SocketFlags.None);
            }
            catch (Exception)
            {
                // ignored
            }
        }
    }

}


