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
    public class NameMap
    {
        int version = 1;
        List<ulong> instance = new List<ulong>();
        List<string> nameStr = new List<string>();

        public int numberRecords
        {
            get {return this.instance.Count;}
        }

        public string[] nameList
        {
            get { return this.nameStr.ToArray(); }
        }

        public ulong[] idList
        {
            get { return this.instance.ToArray(); }
        }

        public NameMap() { }

        public NameMap(BinaryReader br)
        {
            version = br.ReadInt32();
            int numRecords = br.ReadInt32();
            for (int i = 0; i < numRecords; i++)
            {
                instance.Add(br.ReadUInt64());
                int nameLen = br.ReadInt32();
                char[] tmp = new char[nameLen];
                tmp = br.ReadChars(nameLen);
                nameStr.Add(new string(tmp));
            }
        }

        public void Write(BinaryWriter bw)
        {
            bw.Write(version);
            bw.Write(this.instance.Count);
            for (int i = 0; i < this.instance.Count; i++)
            {
                bw.Write(instance[i]);
                bw.Write(this.nameStr[i].Length);
                bw.Write(this.nameStr[i].ToCharArray());
            }
        }

        public string getName(ulong InstanceID)
        {
            int ind = this.instance.IndexOf(InstanceID);
            if (ind >= 0)
            {
                string theName = this.nameStr[ind];
                return theName;
            }
            else
            {
                return null;
            }
        }

        public bool setName(ulong InstanceID, string newName)
        {
            int ind = this.instance.IndexOf(InstanceID);
            if (ind >= 0)
            {
                this.nameStr[ind] = newName;
                return true;
            }
            else
            {
                return false;
            }
        }

        public void addName(ulong InstanceID, string newName, bool replaceDups)
        {
            int tmp = this.instance.IndexOf(InstanceID);
            if (tmp < 0)
            {
                this.instance.Add(InstanceID);
                this.nameStr.Add(newName);
            }
            else if (replaceDups)
            {
                this.deleteName(InstanceID);
                this.instance.Add(InstanceID);
                this.nameStr.Add(newName);
            }
        }

        public void addName(ulong InstanceID, string newName)
        {
            this.addName(InstanceID, newName, true);
        }

        public bool deleteName(ulong InstanceID)
        {
            int ind = this.instance.IndexOf(InstanceID);
            if (ind >= 0)
            {
                this.instance.RemoveAt(ind);
                this.nameStr.RemoveAt(ind);
                return true;
            }
            else
            {
                return false;
            }
        }

        public void addNmap(NameMap nmap)
        {
            ulong[] tmpInst = nmap.idList;
            string[] tmpStr = nmap.nameList;
            for (int i = 0; i < nmap.numberRecords; i++)
            {
                this.addName(tmpInst[i], tmpStr[i], false);
            }
        }

        public override string ToString()
        {
            string tmp = "";
            for (int i = 0; i < this.numberRecords; i++)
            {
                tmp += this.instance[i].ToString("X16") + ":" + this.nameStr[i] + System.Environment.NewLine;
            }
            return tmp;
        }
    }
}
