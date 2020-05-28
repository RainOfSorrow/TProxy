using System;
using System.Collections.Generic;
using System.Text;

namespace TProxy
{
    internal class Server
    {
        public string Name;
        public int Port;
        public byte Slots;

        public Server(string name, int port, byte slots)
        {
            this.Port = port;
            this.Name = name;
            this.Slots = slots;
        }
    }

    internal class Instance
    {
        public string Name;
        public Server[] Servers;

        public Instance(string name, params Server[] servers)
        {
            Name = name;
            Servers = servers;
        }
    }
}
