﻿using TProxy.Packets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace TProxy
{
    internal static class Utils
    {
        public static void ClearNPCs(Client who)
        {
            for (int i = 0; i < 201; i++)
                who.SendData(PacketTypes.NpcUpdate, number: i);
        }

        public static void ClearItems(Client who)
        {
            for (int i = 0; i < 401; i++)
                who.SendData(PacketTypes.UpdateItemDrop, number: i);
        }

        public static void ClearPlayers(Client who)
        {
            for (int i = 0; i < 256; i++)
            {
                if (i == who.Player.playerid)
                    continue;

                who.SendData(PacketTypes.PlayerActive, number: i);
            }
        }

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