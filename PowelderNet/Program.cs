using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace TProxy
{
    class TProxy
    {
        public static TcpListener server;
        static Client[] clients = new Client[256];
        public static Config config;

        public static bool isRunning = true;

        static void Main(string[] args)
        {
            config = Config.Read("config.json");
            if (!File.Exists("config.json"))
            {
                config.Write("config.json");
            }

            server = new TcpListener(IPAddress.Any, config.MainPort);

            server.Start();

            Thread clistener = new Thread(PlayersListener)
            {
                IsBackground = true
            }; clistener.Start();

            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine($"[TProxy] Listening on port {config.MainPort}...");
            Console.ResetColor();


            while (isRunning)
            {
                Thread.Sleep(1000);
            }
        }

        static void PlayersListener()
        {
            while (true)
            {
                Thread.Sleep(200);
                Socket tcpclient = server.AcceptSocket();

                int indxr = 0;
                for (int indx = 0; indx < 256; indx++)
                {
                    if (clients[indx] == null || !clients[indx].isConnected)
                    {
                        Console.WriteLine("Index: " + indx);
                        indxr = indx;
                        break;
                    }
                }

                clients[indxr] = new Client(tcpclient, 7778);
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
