using TProxy.Packets;
using System;
using System.Collections.Generic;
using System.Text;

namespace TProxy
{
    class Player
    {
        public byte playerid = 255;

        public string UUID;

        //Apperance
        public int skinVariant;
        public byte hair;
        public string name;
        public byte hairDye;

        public BitsByte hideVisual;
        public BitsByte hideVisual2;
        public BitsByte hideMisc;

        public Color hairColor;
        public Color skinColor;
        public Color eyeColor;
        public Color shirtColor;
        public Color underShirtColor;
        public Color pantsColor;
        public Color shoeColor;

        public BitsByte extra;

        //Stats
        public int maxHP;
        public int HP;
        public int maxMP;
        public int MP;

        //Proxy
        public bool empty = true;
    }
}
