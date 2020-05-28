using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Threading;

namespace TProxy.Packets
{
    internal static class PacketManager
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


                            foreach (Server server in TProxy.Config.Servers)
                            {
                                if (!who.InTransfer && command.StartsWith(server.Name.ToLower()) && command.EndsWith(server.Name.ToLower()))
                                {
                                    
                                    if (who.Connection.Port == server.Port)
                                    {
                                        who.SendMessage("[c/595959:<] [c/52e092:TProxy] [c/595959:>]  Znajdujesz sie juz na tym serwerze.", 255, 0, 0);
                                    }
                                    else if ((DateTime.Now - who.LastChange).TotalMilliseconds < 5000)
                                    {
                                        who.SendMessage($"[c/595959:<] [c/52e092:TProxy] [c/595959:>]  Ochlon z przelaczaniem, sprobuj za {Math.Round((5000 - (DateTime.Now - who.LastChange).TotalMilliseconds) / 1000, 1)}s.", 255, 0, 0);
                                    }
                                    else
                                    {
                                        who.SendMessage($"[c/595959:<] [c/52e092:TProxy] [c/595959:>]  Przenosze do [c/66ff66:{server.Name}].", 255, 255, 255);
                                        who.ConnectTo(server.Port);
                                    }

                                    return true;
                                }
                            }
                        }


                        return false;
                    }
                case PacketTypes.PlayerInfo:
                    {

                        if (who.InTransfer)
                        {
                            return true;
                        }


                        if (who.Player.empty)
                        {
                            who.Player.playerid = reader.ReadByte();
                            who.Player.skinVariant = reader.ReadByte();
                            who.Player.hair = reader.ReadByte();

                            who.Player.name = reader.ReadString();

                            who.Player.hairDye = reader.ReadByte();

                            who.Player.hideVisual = reader.ReadByte();
                            who.Player.hideVisual2 = reader.ReadByte();
                            who.Player.hideMisc = reader.ReadByte();

                            who.Player.hairColor = new Color(reader.ReadByte(), reader.ReadByte(), reader.ReadByte());
                            who.Player.skinColor = new Color(reader.ReadByte(), reader.ReadByte(), reader.ReadByte());
                            who.Player.eyeColor = new Color(reader.ReadByte(), reader.ReadByte(), reader.ReadByte());
                            who.Player.shirtColor = new Color(reader.ReadByte(), reader.ReadByte(), reader.ReadByte());
                            who.Player.underShirtColor = new Color(reader.ReadByte(), reader.ReadByte(), reader.ReadByte());
                            who.Player.pantsColor = new Color(reader.ReadByte(), reader.ReadByte(), reader.ReadByte());
                            who.Player.shoeColor = new Color(reader.ReadByte(), reader.ReadByte(), reader.ReadByte());

                            who.Player.extra = reader.ReadByte();
                            who.Player.empty = false;
                        }


                        return false;
                    }
                case PacketTypes.PlayerSlot:
                    {
                        if (who.InTransfer)
                            return true;

                        return false;
                    }
                case PacketTypes.ContinueConnecting2:
                    {
                        if (who.InTransfer)
                            return true;

                        return false;
                    }
                case PacketTypes.ItemOwner:
                    {
                        if (who.InTransfer)
                            return true;

                        return false;
                    }
                case PacketTypes.ClientUUID:
                    {
                        who.Player.UUID = reader.ReadString();

                        return false;
                    }
                case PacketTypes.ConnectRequest:
                    {
                        Try_02:
                        try
                        {
                            who.Connection.SendData(PacketTypes.ConnectRequest);
                            who.Connection.SendProxyData(ProxyMessage.UpdateIP, who.Ip);
                        }
                        catch (NullReferenceException) { Thread.Sleep(10); goto Try_02; }
                        return true;
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
                        who.Connection.WorldSpawnX = reader.ReadInt16();
                        who.Connection.WorldSpawnY = reader.ReadInt16();

                       
                        who.Connection.SendData(PacketTypes.TileGetSection, null, -1, -1);

                        who.Connection.SendData(PacketTypes.ClientUUID, who.Player.UUID);

                        who.Connection.SendData(PacketTypes.ItemOwner, null, 400, 255);
                        return 0;
                    }
                case PacketTypes.PlayerSpawnSelf:
                    {
                        who.State = PlayerState.onWorld;


                        MemoryStream spawnSelf = Serialize(PacketTypes.PlayerSpawn, null, who.Player.playerid, number2: who.Connection.WorldSpawnX, number3: who.Connection.WorldSpawnY, 0 , 1);
                        who.Connection.Tcp.Send(spawnSelf.ToArray(), spawnSelf.ToArray().Length, System.Net.Sockets.SocketFlags.None);

                        who.LastChange = DateTime.Now;

                        who.SendData(PacketTypes.Teleport, null, 0, who.Player.playerid, who.Connection.WorldSpawnX * 16, who.Connection.WorldSpawnY * 16 - 3 * 16, 1);

                        return 2;
                    }
                case PacketTypes.ContinueConnecting:
                    {
                        Utils.ClearPlayers(who);
                        Utils.ClearNPCs(who);
                        Utils.ClearItems(who);

                        who.Player.playerid = reader.ReadByte();

                        MemoryStream apperance = SerializeApperance(who.Player);
                        who.Connection.Tcp.Send(apperance.ToArray(), apperance.ToArray().Length, System.Net.Sockets.SocketFlags.None);

                        who.Connection.SendData(PacketTypes.ContinueConnecting2);

                        return 0;
                    }
                case PacketTypes.Disconnect:
                    {
                        string text = NetworkText.Deserialize(reader).ToString();
                        who.SendMessage($"[c/52e092:{who.Connection.Name}] - Wyrzucono: \n{text}", 255, 255, 255);
                        who.State = PlayerState.inVoid;
                        who.Connection.Tcp.Close();
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
                        if (who.State == PlayerState.onWorld)
                        {
                            string text = NetworkText.Deserialize(reader).ToString();
                            who.SendMessage($"[c/52e092:{who.Connection.Port}] - [c/989898:Wyrzucono] [c/595959:»]\n{text}", 128, 128, 128);
                            who.State = PlayerState.inVoid;
                            who.Connection.Tcp.Close();
                            return true;
                        }
                        return false;
                    }
                case PacketTypes.Status:
                    {
                        int percentage = reader.ReadInt32();
                        string text = NetworkText.Deserialize(reader).ToString();
                        byte flags = reader.ReadByte();

                        if (who.State == PlayerState.Connecting)
                        {
                            who.SendData(PacketTypes.Status, "Witamy na serwerze!               \n\n> Ladowanie", percentage, 2);

                            return true;
                        }
                        else if (text == null)
                            return true;
                        else 
                        {
                            who.SendData(PacketTypes.Status, text, percentage, 2);
                        }


                        return true;
                    }
                case PacketTypes.PlayerSpawnSelf:
                    {
                        who.State = PlayerState.onWorld;
                        return false;
                    }
                case PacketTypes.ContinueConnecting:
                    {
                        who.Player.playerid = reader.ReadByte();


                        return false;
                    }
            }
            return false;
        }


       
    }
}
