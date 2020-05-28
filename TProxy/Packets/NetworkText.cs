using System;
using System.IO;
using System.Text;

namespace TProxy.Packets
{
    public class NetworkText
    {
        public static readonly NetworkText Empty = NetworkText.FromLiteral("");
        private NetworkText[] _substitutions;
        private string _text;
        private NetworkText.Mode _mode;

        private NetworkText(string text, NetworkText.Mode mode)
        {
            this._text = text;
            this._mode = mode;
        }

        public static NetworkText FromLiteral(string text)
        {
            return new NetworkText(text, NetworkText.Mode.Literal);
        }

        public int GetMaxSerializedSize()
        {
            int num = 0 + 1 + (4 + Encoding.UTF8.GetByteCount(this._text));
            if (this._mode != NetworkText.Mode.Literal)
            {
                ++num;
                for (int index = 0; index < this._substitutions.Length; ++index)
                    num += this._substitutions[index].GetMaxSerializedSize();
            }
            return num;
        }

        private void SerializeSubstitutionList(BinaryWriter writer)
        {
            if (this._mode == NetworkText.Mode.Literal)
                return;
            writer.Write((byte)this._substitutions.Length);
            for (int index = 0; index < (this._substitutions.Length & (int)byte.MaxValue); ++index)
                this._substitutions[index].Serialize(writer);
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.Write((byte)this._mode);
            writer.Write(this._text);
            this.SerializeSubstitutionList(writer);
        }

        private void DeserializeSubstitutionList(BinaryReader reader)
        {
            if (this._mode == NetworkText.Mode.Literal)
                return;
            this._substitutions = new NetworkText[(int)reader.ReadByte()];
            for (int index = 0; index < this._substitutions.Length; ++index)
                this._substitutions[index] = NetworkText.Deserialize(reader);
        }

        public static NetworkText DeserializeLiteral(BinaryReader reader)
        {
            NetworkText.Mode mode = (NetworkText.Mode)reader.ReadByte();
            NetworkText networkText = new NetworkText(reader.ReadString(), mode);
            networkText.DeserializeSubstitutionList(reader);
            if (mode != NetworkText.Mode.Literal)
                networkText.SetToEmptyLiteral();
            return networkText;
        }

        private void SetToEmptyLiteral()
        {
            this._mode = NetworkText.Mode.Literal;
            this._text = string.Empty;
            this._substitutions = (NetworkText[])null;
        }

        public static NetworkText Deserialize(BinaryReader reader)
        {
            NetworkText.Mode mode = (NetworkText.Mode)reader.ReadByte();
            NetworkText networkText = new NetworkText(reader.ReadString(), mode);
            BinaryReader reader1 = reader;
            networkText.DeserializeSubstitutionList(reader1);
            return networkText;
        }


        public override string ToString()
        {
            try
            {
                switch (this._mode)
                {
                    case NetworkText.Mode.Literal:
                        return this._text;
                    case NetworkText.Mode.Formattable:
                        return string.Format(this._text, (object[])this._substitutions);
                    case Mode.LocalizationKey:
                        return null;
                    default:
                        return this._text;
                }
            }
            catch (Exception ex)
            {
                string str = "NetworkText.ToString() threw an exception.\n" + this.ToDebugInfoString("") + "\n" + "Exception: " + ex.ToString();
                this.SetToEmptyLiteral();
            }
            return this._text;
        }

        private string ToDebugInfoString(string linePrefix = "")
        {
            string str = string.Format("{0}Mode: {1}\n{0}Text: {2}\n", (object)linePrefix, (object)this._mode, (object)this._text);
            if (this._mode == NetworkText.Mode.LocalizationKey)
                str += string.Format("{0}Localized Text: {1}\n", (object)linePrefix, (object)null);
            if (this._mode != NetworkText.Mode.Literal)
            {
                for (int index = 0; index < this._substitutions.Length; ++index)
                    str = str + string.Format("{0}Substitution {1}:\n", (object)linePrefix, (object)index) + this._substitutions[index].ToDebugInfoString(linePrefix + "\t");
            }
            return str;
        }

        private enum Mode : byte
        {
            Literal,
            Formattable,
            LocalizationKey,
        }
    }
}