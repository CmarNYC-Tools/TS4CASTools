using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace Xmods.DataLib
{
    public class VPXY
    {
        int rcolVersion, rcolCount, index3, extCount, intCount;
        ITG[] extITG, intITG;
        int chunkPos, chunkSize;
        char[] magic;
        int version;
        int tgiOffset;
        int tgiSize;
        byte count;
        Entry[] entries;
        byte boxType;
        float[] boundingBox;
        uint unknown;
        byte flag;
        int ftptIndex;
        int tgiCount;
        TGI[] tgiList;

        public Entry[] Type0Entries
        {
            get
            {
                if (entries == null) return null;
                List<Entry> tmp = new List<Entry>();
                foreach (Entry e in this.entries)
                {
                    if (e.type == 0) tmp.Add(e);
                }
                return tmp.ToArray();
            }
        }

        public Entry[] Type1Entries
        {
            get
            {
                if (entries == null) return null;
                List<Entry> tmp = new List<Entry>();
                foreach (Entry e in this.entries)
                {
                    if (e.type == 1) tmp.Add(e);
                }
                return tmp.ToArray();
            }
        }

        public TGI[] tgiArray
        {
            get
            {
                return this.tgiList;
            }
            set
            {
                this.tgiList = new TGI[value.Length];
                for (int i = 0; i < value.Length; i++)
                {
                    this.tgiList[i] = new TGI(value[i].Type, value[i].Group, value[i].Instance);
                }
            }
        }

        public VPXY(BinaryReader br)
        {
            rcolVersion = br.ReadInt32();
            rcolCount = br.ReadInt32();
            index3 = br.ReadInt32();
            extCount = br.ReadInt32();
            intCount = br.ReadInt32();
            if (intCount > 0) intITG = new ITG[intCount];
            for (int i = 0; i < intCount; i++)
            {
                intITG[i] = new ITG(br);
            }
            if (extCount > 0) extITG = new ITG[extCount];
            for (int i = 0; i < extCount; i++)
            {
                extITG[i] = new ITG(br);
            }
            chunkPos = br.ReadInt32();
            chunkSize = br.ReadInt32();
            magic = new char[4];
            magic = br.ReadChars(4);
            version = br.ReadInt32();
            tgiOffset = br.ReadInt32();
            tgiSize = br.ReadInt32();
            count = br.ReadByte();
            entries = new Entry[count];
            for (int i = 0; i < count; i++)
            {
                entries[i] = new Entry(br);
            }
            boxType = br.ReadByte();
            boundingBox = new float[6];
            for (int i = 0; i < 6; i++)
            {
                boundingBox[i] = br.ReadSingle();
            }
            unknown = br.ReadUInt32();
            flag = br.ReadByte();
            if (flag == 1)
            {
                ftptIndex = br.ReadInt32();
            }
            if (tgiSize > 0)
            {
                tgiCount = br.ReadInt32();
                tgiList = new TGI[tgiCount];
                for (int i = 0; i < tgiCount; i++)
                {
                    tgiList[i] = new TGI(br);
                }
            }
            else
            {
                tgiCount = 0;
                tgiList = new TGI[0];
            }
        }

        public VPXY(TGI tgi, TGI[][] geomTGIs) : this(tgi, new TGI[] { }, geomTGIs) { }

        public VPXY(TGI tgi, TGI[] boneTGIs, TGI[][] geomTGIs)
        {
            if (geomTGIs.GetLength(0) != 4) throw new ApplicationException("First dimension of LOD TGIs must be 4!");
            rcolVersion = 3;
            rcolCount = 1;
            index3 = 0;
            extCount = 0;
            intCount = 1;
            intITG = new ITG[1];
            intITG[0] = new ITG(tgi.Instance, tgi.Type, tgi.Group);
            chunkPos = 44;
            chunkSize = 141;
            magic = new char[4] { 'V', 'P', 'X', 'Y' };
            version = 4;
            count = 0;
            tgiCount = 0;
            for (int i = 0; i < boneTGIs.Length; i++)
            {
                count++;
                tgiCount++;
            }
            for (int i = 0; i < geomTGIs.GetLength(0); i++)
            {
                for (int j = 0; j < geomTGIs[i].Length; j++)
                {
                    tgiCount++;
                }
                if (geomTGIs[i].Length > 0) count++;
            }
            boxType = 2;
            boundingBox = new float[6] { 0f, 0f, 0f, 0f, 0f, 0f };
            unknown = 0;
            flag = 0;
            entries = new Entry[count];
            tgiList = new TGI[tgiCount];
            if (count > 0)
            {
                int counterTGI = 0;
                int counterEnt = 0;
                for (int i = 0; i < boneTGIs.Length; i++)
                {
                    entries[counterEnt] = new Entry(counterTGI);
                    tgiList[counterTGI] = new TGI(boneTGIs[i].Type, boneTGIs[i].Group, boneTGIs[i].Instance);
                    counterEnt++;
                    counterTGI++;
                }
                for (int i = 0; i < 4; i++)
                {
                    List<int> tmp = new List<int>();
                    for (int j = 0; j < geomTGIs[i].Length; j++)
                    {
                        tmp.Add(counterTGI);
                        tgiList[counterTGI] = new TGI(geomTGIs[i][j].Type, geomTGIs[i][j].Group, geomTGIs[i][j].Instance);
                        counterTGI++;
                    }
                    if (tmp.Count > 0)
                    {
                        entries[counterEnt] = new Entry(i, tmp.ToArray());
                        counterEnt++;
                    }
                }
               // for (int i = 3; i >= 0; i--)
               // {
               //     for (int j = 0; j < geomTGIs[i].Length; j++)
               //     {
               //         entries[counter] = new Entry(i, counter);
               //         tgiList[counter] = new TGI(geomTGIs[i][j].Type, geomTGIs[i][j].Group, geomTGIs[i][j].Instance);
               //         counter++;
               //     }
               // }
            }
            tgiOffset = 35;
            foreach (Entry e in this.entries)
            {
                tgiOffset += e.Size;
            }
            tgiSize = (tgiCount * 16) + 4;
        }

        public void Write(BinaryWriter bw)
        {
            bw.Write(rcolVersion);
            bw.Write(rcolCount);
            bw.Write(index3);
            bw.Write(extCount);
            bw.Write(intCount);
            for (int i = 0; i < intCount; i++)
            {
                intITG[i].Write(bw);
            }
            for (int i = 0; i < extCount; i++)
            {
                extITG[i].Write(bw);
            }
            bw.Write(chunkPos);
            bw.Write(chunkSize);
            bw.Write(magic);
            bw.Write(version);
            bw.Write(tgiOffset);
            bw.Write(tgiSize);
            bw.Write(count);
            for (int i = 0; i < count; i++)
            {
                entries[i].Write(bw);
            }
            bw.Write(boxType);
            for (int i = 0; i < 6; i++)
            {
                bw.Write(boundingBox[i]);
            }
            bw.Write(unknown);
            bw.Write(flag);
            if (flag == 1)
            {
                bw.Write(ftptIndex);
            }
            if (tgiSize > 0)
            {
                bw.Write(tgiCount);
                for (int i = 0; i < tgiCount; i++)
                {
                    tgiList[i].Write(bw);
                }
            }
        }

        public class Entry
        {
            internal byte type;
            internal byte id;
            internal byte refCount;
            internal int[] tgiRefs;

            public int Type
            {
                get { return (int)type; }
                set { this.type = (byte)value; }
            }

            public int ID
            {
                get 
                {
                    if (this.type == 0 & tgiRefs.Length > 0) return (int)id;
                    else return -1;
                }
                set 
                {
                    if (this.type == 0 & tgiRefs.Length > 0) this.id = (byte)value;
                }
            }

            public int Size
            {
                get
                {
                    if (this.type == 0)
                    {
                        return 3 + (this.refCount * 4);
                    }
                    else
                    {
                        return 5;
                    }
                }
            }

            public int[] IndexArray
            {
                get { return tgiRefs; }
            }

            internal Entry(BinaryReader br)
            {
                type = br.ReadByte();
                if (type == 0)
                {
                    id = br.ReadByte();
                    refCount = br.ReadByte();
                    tgiRefs = new int[refCount];
                    for (int i = 0; i < refCount; i++)
                    {
                        tgiRefs[i] = br.ReadInt32();
                    }
                }
                else if (type == 1)
                {
                    tgiRefs = new int[1];
                    tgiRefs[0] = br.ReadInt32();
                }
            }

            public Entry(int TGIindex)
            {
                this.type = 1;
                this.tgiRefs = new int[] { TGIindex };
            }

            public Entry(byte ID, int[] TGIindexArray)
            {
                this.type = 0;
                this.id = ID;
                this.tgiRefs = new int[TGIindexArray.Length];
                this.refCount = (byte)TGIindexArray.Length;
                for (int i = 0; i < TGIindexArray.Length; i++)
                {
                    this.tgiRefs[i] = TGIindexArray[i];
                }
            }

            public Entry(int ID, int[] TGIindexArray) : this((byte)ID, TGIindexArray) { }

            public Entry(int ID, int TGIindex) : this((byte)ID, new int[] { TGIindex }) { }

            internal void Write(BinaryWriter bw)
            {
                bw.Write(type);
                if (type == 0)
                {
                    bw.Write(id);
                    bw.Write(refCount);
                    for (int i = 0; i < refCount; i++)
                    {
                        bw.Write(tgiRefs[i]);
                    }
                }
                else if (type == 1)
                {
                    bw.Write(tgiRefs[0]);
                }
            }
        }

        internal class ITG : TGI
        {
            internal ITG(BinaryReader br)
            {
                this.Instance = br.ReadUInt64();
                this.Type = br.ReadUInt32();
                this.Group = br.ReadUInt32();
            }

            internal ITG(ulong instance, uint type, uint group)
            {
                this.Instance = instance;
                this.Type = type;
                this.Group = group;
            }

            internal new void Write(BinaryWriter bw)
            {
                bw.Write(this.Instance);
                bw.Write(this.Type);
                bw.Write(this.Group);
            }

        }
    }
}
