/* Xmods Data Library, a library to support tools for The Sims 4,
   Copyright (C) 2014  C. Marinetti

   This program is free software: you can redistribute it and/or modify
   it under the terms of the GNU General Public License as published by
   the Free Software Foundation, either version 3 of the License, or
   (at your option) any later version.

   This program is distributed in the hope that it will be useful,
   but WITHOUT ANY WARRANTY; without even the implied warranty of
   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
   GNU General Public License for more details.

   You should have received a copy of the GNU General Public License
   along with this program.  If not, see <http://www.gnu.org/licenses/>. 
   The author may be contacted at modthesims.info, username cmarNYC. */

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Xmods.DataLib
{
    public class STBL
    {
        char[] magic = new char[4];
        byte version;
        byte[] unk = new byte[2];
        int count;
        byte[] unk2 = new byte[6];
        int v5size;
        List<keyValuePair> keyTextPair;

        public string[] TextKeys
        {
            get
            {
                string[] tmp = new string[this.count];
                for (int i = 0; i < this.count; i++)
                {
                    tmp[i] = keyTextPair[i].Key.ToString("X16");
                }
                return tmp;
            }
        }

        public ulong[] Keys
        {
            get
            {
                ulong[] tmp = new ulong[this.count];
                for (int i = 0; i < this.count; i++)
                {
                    tmp[i] = keyTextPair[i].Key;
                }
                return tmp;
            }
        }

        public string[] TextStrings
        {
            get 
            {
                string[] tmp = new string[this.count];
                for (int i = 0; i < this.count; i++)
                {
                    tmp[i] = keyTextPair[i].Text;
                }
                return tmp;
            }
        }

        public int NumberStrings
        {
            get { return this.count; }
        }

        public string getText(ulong key)
        {
            foreach (keyValuePair kt in this.keyTextPair)
            {
                if (kt.Key == key)
                {
                    return kt.Text;
                }
            }
            return "";
        }

        public STBL(int Version) 
        {
            this.magic = new char[4] { 'S', 'T', 'B', 'L' };
            this.version = (byte)Version;
            this.unk = new byte[2] { 0, 0 };
            this.count = 0;
            this.unk2 = new byte[6] { 0, 0, 0, 0, 0, 0 };
            this.v5size = 0;
            this.keyTextPair = new List<keyValuePair>();
        }

        public STBL(BinaryReader br)
        {
            magic = br.ReadChars(4);
            version = br.ReadByte();
            unk = br.ReadBytes(2);
            count = br.ReadInt32();
            unk2 = br.ReadBytes(6);
            if (this.version == 5) v5size = br.ReadInt32();
            this.keyTextPair = new List<keyValuePair>();
            for (int i = 0; i < count; i++)
            {
                if (version == 2)
                {
                    ulong k = br.ReadUInt64();
                    int textLen = br.ReadInt32();
                    byte[] tmp = br.ReadBytes(textLen * 2);
                    this.keyTextPair.Add(new keyValuePair(k, System.Text.Encoding.Unicode.GetString(tmp)));
                }
                else if (version == 5)
                {
                    uint k = br.ReadUInt32();
                    byte tmp = br.ReadByte();
                    int textLen = br.ReadInt16();
                    char[] tmp2 = br.ReadChars(textLen);
                    this.keyTextPair.Add(new keyValuePair(k, new string(tmp2)));
                }
            }
        }

        public STBL(STBL s)
        {
            this.magic = new char[4] { s.magic[0], s.magic[1], s.magic[2], s.magic[3] };
            this.version = s.version;
            this.unk = new byte[2] { s.unk[0], s.unk[1] };
            this.count = s.count;
            this.unk2 = new byte[6] { s.unk2[0], s.unk2[1], s.unk2[2], s.unk2[3], s.unk2[4], s.unk2[5] };
            this.v5size = s.v5size;
            this.keyTextPair = new List<keyValuePair>();
            for (int i = 0; i < this.count; i++)
            {
                keyValuePair tmp = new keyValuePair(s.keyTextPair[i]);
                this.keyTextPair.Add(tmp);
            }
        }

        public void WriteSTBL(BinaryWriter bw)
        {
            bw.Write(magic);
            bw.Write(version);
            bw.Write(unk);
            bw.Write(count);
            bw.Write(unk2);
            if (this.version == 5)
            {
                int totCount = 0;
                foreach (STBL.keyValuePair p in keyTextPair)
                {
                    totCount += p.Text.Length + 1;
                }
                bw.Write(totCount);
            }
            for (int i = 0; i < count; i++)
            {
                if (version == 2)
                {
                    bw.Write(keyTextPair[i].Key);
                    bw.Write(keyTextPair[i].Text.Length);
                    byte[] tmp = System.Text.Encoding.Unicode.GetBytes(keyTextPair[i].Text);
                    bw.Write(tmp);
                }
                else if (version == 5)
                {
                    bw.Write((uint)keyTextPair[i].Key);
                    bw.Write((byte)0);
                    bw.Write((ushort)keyTextPair[i].Text.Length);
                    char[] tmp = keyTextPair[i].Text.ToCharArray();
                    bw.Write(tmp);
                }
            }
        }

        public void Add(ulong key, string text)
        {
            this.keyTextPair.Add(new keyValuePair(key, text));
            this.count++;
        }

        public void Update(ulong key, string newText)
        {
            foreach (keyValuePair kt in this.keyTextPair)
            {
                if (kt.Key == key)
                {
                    kt.Text = newText;
                    break;
                }
            }
        }

        public void Remove(ulong key)
        {
            for (int i = 0; i < this.keyTextPair.Count; i++)
            {
                if (this.keyTextPair[i].Key == key)
                {
                    this.keyTextPair.RemoveAt(i);
                }
            }
        }

        internal class keyValuePair
        {
            ulong key;
            string text;
            internal ulong Key
            {
                get { return key; }
                set { key = value; }
            }
            internal string Text
            {
                get { return text; }
                set { text = value; }
            }
            internal keyValuePair()
            {
                key = 0;
                text = "";
            }
            internal keyValuePair(ulong key, string text)
            {
                this.key = key;
                this.text = text;
            }
            internal keyValuePair(keyValuePair p)
            {
                this.key = p.key;
                this.text = String.Copy(p.text);
            }
        }
    }
}
