using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading;

namespace TProxy.Packets
{
    class PacketManager
    {
        public static MemoryStream Serialize(PacketTypes msgType, NetworkText text = null, int number = 0, float number2 = 0.0f, float number3 = 0.0f, float number4 = 0.0f, int number5 = 0, int number6 = 0, int number7 = 0)
        {
            using (MemoryStream mem = new MemoryStream())
            {
                using (BinaryWriter binary = new BinaryWriter(mem))
                {
                    binary.BaseStream.Position = 0L;
                    long pointer = binary.BaseStream.Position;
                    binary.BaseStream.Position += 2L;
                    binary.Write((byte)msgType);
                    switch (msgType)
                    {
                        case PacketTypes.ConnectRequest:
                            {
                                binary.Write("Terraria" + (object)228);
                                break;
                            }
                        case PacketTypes.Disconnect:
                            {
                                text.Serialize(binary);
                                break;
                            }
                        case PacketTypes.ContinueConnecting2:
                            {
                                break;
                            }
                        case PacketTypes.TileGetSection:
                            {
                                binary.Write(number);
                                binary.Write((int)number2);
                                break;
                            }
                        case PacketTypes.Status:
                            {
                                binary.Write(number);
                                text.Serialize(binary);
                                binary.Write((byte)number2);
                                break;
                            }
                        case PacketTypes.PlayerSpawn:
                            {
                                binary.Write((byte)number);
                                binary.Write((short)number2);
                                binary.Write((short)number3);
                                binary.Write((int)number4);
                                binary.Write((byte)number5);
                                break;
                            }
                        case PacketTypes.PlayerSpawnSelf:
                            {
                                break;
                            }
                        case PacketTypes.PlayerAddBuff:
                            {
                                binary.Write((byte)number);
                                binary.Write((byte)number2);
                                binary.Write((int)number3);
                                break;
                            }
                        case PacketTypes.PlayHarp:
                            {
                                binary.Write((byte)number);
                                binary.Write((float)number2);
                                break;
                            }
                        case PacketTypes.Teleport:
                            {
                                binary.Write((byte)number);
                                binary.Write((short)number2);
                                binary.Write(number3);
                                binary.Write(number4);
                                binary.Write(number5);
                                break;
                            }
                        case PacketTypes.SmartTextMessage:
                            {
                                binary.Write((byte)number2);
                                binary.Write((byte)number3);
                                binary.Write((byte)number4);
                                text.Serialize(binary);
                                binary.Write((short)number5);
                                break;
                            }
                        case PacketTypes.NpcUpdate:
                            {
                                binary.Write((short)number);
                                binary.Write((float)0);
                                binary.Write((float)0);
                                binary.Write((float)0);
                                binary.Write((float)0);
                                binary.Write((ushort)0);
                                binary.Write((byte)0);
                                binary.Write((byte)0);
                                binary.Write((short)0);
                                binary.Write((sbyte)0);
                                break;
                            }
                        case PacketTypes.NpcStrike:
                            {
                                binary.Write((short)number);
                                binary.Write((short)-1);
                                binary.Write((float)0);
                                binary.Write((byte)0);
                                binary.Write((byte)0);
                                break;
                            }
                        case PacketTypes.NotifyPlayerNpcKilled:
                            {
                                binary.Write((short)number);
                                break;
                            }
                        case PacketTypes.PlayerActive:
                            {
                                binary.Write((byte)number);
                                binary.Write((byte)0);
                                break;
                            }
                        case PacketTypes.ClientUUID:
                            {
                                binary.Write(text.ToString());
                                break;
                            }
                        case PacketTypes.UpdateItemDrop:
                            {
                                binary.Write((short)number);
                                binary.Write((float)0);
                                binary.Write((float)0);
                                binary.Write((float)0);
                                binary.Write((float)0);
                                binary.Write((short)0);
                                binary.Write((byte)0);
                                binary.Write((byte)4);
                                binary.Write((short)0);
                                break;
                            }
                        case PacketTypes.PlayerHealOther:
                            {
                                binary.Write((byte)number);
                                binary.Write((short)number2);
                                break;
                            }
                        case PacketTypes.LoadNetModule:
                            {
                                binary.Write((ushort)number);
                                binary.Write((byte)0);
                                break;
                            }
                        case PacketTypes.ItemOwner:
                            {
                                binary.Write((short)number);
                                binary.Write((byte)number2);
                                break;
                            }
                        default:
                            {
                                break;
                            }
                    }
                    int pointHolder = (int)binary.BaseStream.Position;
                    binary.BaseStream.Position = pointer;
                    binary.Write((short)pointHolder);
                    binary.BaseStream.Position = (long)pointHolder;
                }
                return mem;
            }
        }
        public static MemoryStream SerializeApperance(Player plr)
        {
            using (MemoryStream mem = new MemoryStream())
            {
                using (BinaryWriter binary = new BinaryWriter(mem))
                {
                    binary.BaseStream.Position = 0L;
                    long pointer = binary.BaseStream.Position;
                    binary.BaseStream.Position += 2L;
                    binary.Write((byte)4);

                    binary.Write(plr.playerid);
                    binary.Write((byte)plr.skinVariant);
                    binary.Write(plr.hair);
                    binary.Write(plr.name);
                    binary.Write(plr.hairDye);

                   BitsByte hideVisual = 0;
                    for (int local_9 = 0; local_9 < 8; ++local_9)
                        hideVisual[local_9] = plr.hideVisual[local_9];
                    binary.Write(hideVisual);

                   BitsByte hideVisual2 = (byte)0;
                    for (int local_10 = 0; local_10 < 2; ++local_10)
                        hideVisual2[local_10] = plr.hideVisual[local_10 + 8];
                    binary.Write(hideVisual2);

                    binary.Write(plr.hideMisc);

                    binary.Write(plr.hairColor.R);
                    binary.Write(plr.hairColor.G);
                    binary.Write(plr.hairColor.B);

                    binary.Write(plr.skinColor.R);
                    binary.Write(plr.skinColor.G);
                    binary.Write(plr.skinColor.B);

                    binary.Write(plr.eyeColor.R);
                    binary.Write(plr.eyeColor.G);
                    binary.Write(plr.eyeColor.B);

                    binary.Write(plr.shirtColor.R);
                    binary.Write(plr.shirtColor.G);
                    binary.Write(plr.shirtColor.B);

                    binary.Write(plr.underShirtColor.R);
                    binary.Write(plr.underShirtColor.G);
                    binary.Write(plr.underShirtColor.B);

                    binary.Write(plr.pantsColor.R);
                    binary.Write(plr.pantsColor.G);
                    binary.Write(plr.pantsColor.B);

                    binary.Write(plr.shoeColor.R);
                    binary.Write(plr.shoeColor.G);
                    binary.Write(plr.shoeColor.B);

                    binary.Write(plr.extra);


                    int positionHolder = (int)binary.BaseStream.Position;
                    binary.BaseStream.Position = pointer;
                    binary.Write((short)positionHolder);
                    binary.BaseStream.Position = pointer;

                }
                return mem;
            }
        }
        public static MemoryStream SerializeProxyMessage(ProxyMessage type, NetworkText text = null, int number = 0, float number2 = 0.0f, float number3 = 0.0f, int number4 = 0)
        {
            using (MemoryStream mem = new MemoryStream())
            {
                using (BinaryWriter binary = new BinaryWriter(mem))
                {
                    binary.BaseStream.Position = 0L;
                    long local_4 = binary.BaseStream.Position;
                    binary.BaseStream.Position += 2L;
                    binary.Write((byte)PacketTypes.Placeholder);
                    binary.Write("P`2uCbcbB:vfDv}NYP`Q$$m<]:;w8W=&D+/.[Q99");
                    binary.Write((byte)type);
                    switch (type)
                    {
                        case ProxyMessage.UpdateIP:
                            {
                                binary.Write(number);
                                text.Serialize(binary);
                                break;
                            }
                        default:
                            {
                                break;
                            }
                    }
                    int local_5 = (int)binary.BaseStream.Position;
                    binary.BaseStream.Position = local_4;
                    binary.Write((short)local_5);
                    binary.BaseStream.Position = (long)local_5;
                }
                return mem;
            }
        }


        public static bool DeserializeFromPlayer(PacketTypes type, byte[] data, Client who)
        {
            BinaryReader reader = new BinaryReader(new MemoryStream(data, 0, data.Length));


            switch (type)
            {

                case PacketTypes.LoadNetModule:
                    {
                        ushort module = reader.ReadUInt16();

                        if (module != 1)
                            return false;


                        string commandID = reader.ReadString();
                        string Message = reader.ReadString();


                        if (Message.StartsWith("/"))
                        {
                            string command = Message.Remove(0, 1).ToLower();


                            foreach (Server server in TProxy.config.Servers)
                            {
                                if (command.StartsWith(server.name.ToLower()) && !who.inTransfer)
                                {
                                    if (who.connection.port == server.port)
                                    {
                                        who.SendMessage("[c/20b262:TProxy] [c/595959:]  Znajdujesz sie juz na tym serwerze.", 128, 128, 128);
                                    }
                                    else
                                    {
                                        who.SendMessage($"[c/20b262:TProxy] [c/595959:»]  Przenosze do [c/66ff66:{server.name}].", 128, 128, 128);
                                        who.ConnectTo(server.port);
                                    }

                                    return true;
                                }
                            }
                        }


                        return false;
                    }
                case PacketTypes.PlayerInfo:
                    {

                        if (who.inTransfer)
                        {
                            return true;
                        }


                        if (who.player.empty)
                        {
                            who.player.playerid = reader.ReadByte();
                            who.player.skinVariant = reader.ReadByte();
                            who.player.hair = reader.ReadByte();

                            who.player.name = reader.ReadString();

                            who.player.hairDye = reader.ReadByte();

                            who.player.hideVisual = reader.ReadByte();
                            who.player.hideVisual2 = reader.ReadByte();
                            who.player.hideMisc = reader.ReadByte();

                            who.player.hairColor = new Color(reader.ReadByte(), reader.ReadByte(), reader.ReadByte());
                            who.player.skinColor = new Color(reader.ReadByte(), reader.ReadByte(), reader.ReadByte());
                            who.player.eyeColor = new Color(reader.ReadByte(), reader.ReadByte(), reader.ReadByte());
                            who.player.shirtColor = new Color(reader.ReadByte(), reader.ReadByte(), reader.ReadByte());
                            who.player.underShirtColor = new Color(reader.ReadByte(), reader.ReadByte(), reader.ReadByte());
                            who.player.pantsColor = new Color(reader.ReadByte(), reader.ReadByte(), reader.ReadByte());
                            who.player.shoeColor = new Color(reader.ReadByte(), reader.ReadByte(), reader.ReadByte());

                            who.player.extra = reader.ReadByte();
                            who.player.empty = false;
                        }

                        return false;
                    }
                case PacketTypes.PlayerSlot:
                    {
                        if (who.inTransfer)
                            return true;

                        return false;
                    }
                case PacketTypes.ContinueConnecting2:
                    {
                        if (who.inTransfer)
                            return true;

                        return false;
                    }
                case PacketTypes.ItemOwner:
                    {
                        if (who.inTransfer)
                            return true;

                        return false;
                    }
                case PacketTypes.ClientUUID:
                    {
                        who.player.UUID = reader.ReadString();

                        return false;
                    }
                default:
                    {
                        return false;
                    }
            }
        }



        public static byte DeserializeFromTransfer(PacketTypes type, byte[] data, Client who)
        {
            BinaryReader reader = new BinaryReader(new MemoryStream(data, 0, data.Length));

            switch (type)
            {
                case PacketTypes.WorldInfo:
                    {
                        reader.ReadInt32();
                        reader.ReadByte();
                        reader.ReadByte();
                        reader.ReadInt16();
                        reader.ReadInt16();
                        who.connection.worldSpawnX = reader.ReadInt16();
                        who.connection.worldSpawnY = reader.ReadInt16();

                        MemoryStream apperance = SerializeApperance(who.player);
                        who.connection.tcp.Send(apperance.ToArray(), apperance.ToArray().Length, System.Net.Sockets.SocketFlags.None);

                        who.connection.SendData(PacketTypes.TileGetSection, null, -1, -1);

                        who.connection.SendData(PacketTypes.ClientUUID, who.player.UUID);

                        who.connection.SendData(PacketTypes.ItemOwner, null, 400, 255);
                        return 0;
                    }
                case PacketTypes.PlayerSpawnSelf:
                    {
                        who.state = PlayerState.onWorld;


                        MemoryStream spawnSelf = Serialize(PacketTypes.PlayerSpawn, null, who.player.playerid, number2: who.connection.worldSpawnX, number3: who.connection.worldSpawnY, 0 , 1);
                        who.connection.tcp.Send(spawnSelf.ToArray(), spawnSelf.ToArray().Length, System.Net.Sockets.SocketFlags.None);


                        who.SendData(PacketTypes.Teleport, null, 0, who.player.playerid, who.connection.worldSpawnX * 16, who.connection.worldSpawnY * 16 - 3 * 16, 1);

                        return 2;
                    }
                case PacketTypes.ContinueConnecting:
                    {
                        Utils.ClearPlayers(who);
                        Utils.ClearNPCs(who);
                        Utils.ClearItems(who);

                        who.player.playerid = reader.ReadByte();

                        MemoryStream apperance = SerializeApperance(who.player);
                        who.connection.tcp.Send(apperance.ToArray(), apperance.ToArray().Length, System.Net.Sockets.SocketFlags.None);

                        who.connection.SendData(PacketTypes.ContinueConnecting2);

                        return 0;
                    }
                case PacketTypes.Disconnect:
                    {
                        string text = NetworkText.Deserialize(reader).ToString();
                        who.SendMessage($"[c/20b262:{who.connection.port}] - [c/989898:Wyrzucono] [c/595959:»]\n{text}", 128, 128, 128);
                        who.state = PlayerState.inVoid;
                        who.connection.tcp.Close();
                        return 1;
                    }
                default:
                    {

                        return 0;
                    }


            }

        }
        public static bool DeserializeFromServer(PacketTypes type, byte[] data, Client who)
        {
            BinaryReader reader = new BinaryReader(new MemoryStream(data, 0, data.Length));

            switch (type)
            {
                case PacketTypes.Disconnect:
                    {
                        if (who.state == PlayerState.onWorld)
                        {
                            string text = NetworkText.Deserialize(reader).ToString();
                            who.SendMessage($"[c/20b262:{who.connection.port}] - [c/989898:Wyrzucono] [c/595959:»]\n{text}", 128, 128, 128);
                            who.state = PlayerState.inVoid;
                            who.connection.tcp.Close();
                            return true;
                        }
                        return false;
                    }
                case PacketTypes.Status:
                    {
                        int percentage = reader.ReadInt32();
                        string text = NetworkText.Deserialize(reader).ToString();
                        byte flags = reader.ReadByte();

                        if (who.state == PlayerState.Connecting)
                        {
                            who.SendData(PacketTypes.Status, "Witamy na serwerze Powelder!\n\n> Ladowanie", percentage, 2);

                            return true;
                        }
                        else if (text == null)
                            return true;
                        else 
                        {
                            who.SendData(PacketTypes.Status, "[a:NO_HOBO]\n[g:23]\n[c/66ff6:Kolor]\n[i:1]" + text, percentage, 2);
                        }


                        return true;
                    }
                case PacketTypes.PlayerSpawnSelf:
                    {
                        who.state = PlayerState.onWorld;
                        return false;
                    }
                case PacketTypes.ContinueConnecting:
                    {
                        who.player.playerid = reader.ReadByte();


                        return false;
                    }
            }
            return false;
        }


       
    }
}
