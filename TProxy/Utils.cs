using TProxy.Packets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace TProxy
{
    internal static class Utils
    {

        public static void WriteMessage(string msg, ConsoleColor color = ConsoleColor.Gray)
        {
            Console.ForegroundColor = color;

            Console.WriteLine(msg);

            Console.ForegroundColor = ConsoleColor.Gray;
        }

        public static void WriteException(string where, Exception e)
        {
            WriteMessage($"{where}: {e.Message}\n");
        }

    }

}