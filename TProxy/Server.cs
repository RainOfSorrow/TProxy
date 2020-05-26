using System;
using System.Collections.Generic;
using System.Text;

namespace TProxy
{
    class Server
    {
        public string name;
        public int port;
        public byte slots;

        public Server(string name, int port, byte slots)
        {
            this.port = port;
            this.name = name;
            this.slots = slots;
        }
    }

    class Instance
    {
        public string name;
        public Server[] servers;

        public Instance(string name, params Server[] servers)
        {
            this.name = name;
            this.servers = servers;
        }
    }
}
