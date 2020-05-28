using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using static System.Threading.Thread;

namespace TProxy
{
    class TProxy
    {
        public static TcpListener Server;
        static Client[] clients = new Client[256];
        public static bool[] AvalibleClient = new bool[256];
        public static Config Config;

        public static bool IsRunning = true;

        static void Main(string[] args)
        {
            Config = Config.Read("config.json");
            if (!File.Exists("config.json"))
            {
                Config.Write("config.json");
            }

            for(int i = 0; i < 256; i++)
            {
                AvalibleClient[i] = true;
            }

            Server = new TcpListener(IPAddress.Any, Config.MainPort);

            Server.Start();

            var listener = new Thread(PlayersListener)
            {
                IsBackground = true
            }; listener.Start();

            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine($"[TProxy] Listening on port {Config.MainPort}...");
            Console.ResetColor();


            while (IsRunning)
            {
                Sleep(1000);
            }
        }

        static void PlayersListener()
        {
            while (true)
            {
                Sleep(200);
                var tcpclient = Server.AcceptSocket();

                byte indxr = 0;
                for (int indx = 0; indx < 256; indx++)
                {
                    if (AvalibleClient[indx])
                    {
                        indxr = (byte)indx;
                        AvalibleClient[indx] = false;
                        break;
                    }
                }

                clients[indxr] = new Client(tcpclient, 7778, indxr);
            }
        }
    }

    public struct Color
    {
        public byte R;
        public byte G;
        public byte B;

        public Color(byte r, byte g, byte b)
        {
            R = r;
            G = g;
            B = b;
        }
    }
}
