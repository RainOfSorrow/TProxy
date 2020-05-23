using TProxy.Packets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace TProxy
{
    class Utils
    {
        public static void ClearNPCs(Client who)
        {
            for (int i = 0; i < 201; i++)
                who.SendData(PacketTypes.NpcUpdate, number: i);
        }

        public static void ClearItems(Client who)
        {
            for (int i = 0; i < 401; i++)
                who.SendData(PacketTypes.ItemDrop, number: i);
        }

        public static void ClearPlayers(Client who)
        {
            for (int i = 0; i < 256; i++)
            {
                if (i == who.player.playerid)
                    continue;

                who.SendData(PacketTypes.PlayerActive, number: i);
            }
        }

    }

}