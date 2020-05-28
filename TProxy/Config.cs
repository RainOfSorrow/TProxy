using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Newtonsoft.Json;

namespace TProxy
{
    internal class Config
    {
        public int MainPort = 7777;

        public Server[] Servers = { new Server("Lobby", 7778, 8) };

        public void Write(string file)
        {
            File.WriteAllText(file, JsonConvert.SerializeObject(this, Formatting.Indented));
        }

        public static Config Read(string file)
        {
            return File.Exists(file) ? JsonConvert.DeserializeObject<Config>(File.ReadAllText(file)) : new Config();
        }
    }
}
