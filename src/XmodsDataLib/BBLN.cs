using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Text;

namespace Xmods.DataLib
{
    public class BBLN
    {
        int version;
        int TGI_offset, TGI_size;
        string partName;
        int unknown;
        uint bgType, bgGroup;
        ulong bgInstance;
        Entry[] entries;
        TGI[] tgiList;

        public BBLN(BinaryReader br)
        {
            version = br.ReadInt32();
            TGI_offset = br.ReadInt32();
            TGI_size = br.ReadInt32();
            int tmp = br.ReadByte();
            byte[] tmpb = br.ReadBytes(tmp);
            partName = Encoding.BigEndianUnicode.GetString(tmpb);
            unknown = br.ReadInt32();
            if (version == 8)
            {
                bgType = br.ReadUInt32();
                bgGroup = br.ReadUInt32();
                bgInstance = br.ReadUInt64();
            }
            int entryCount = br.ReadInt32();
            entries = new Entry[entryCount];
            for (int i = 0; i < entryCount; i++)
            {
                entries[i] = new Entry(br);
            }
            int tgiCount = br.ReadInt32();
            tgiList = new TGI[tgiCount];
            for (int i = 0; i < tgiCount; i++)
            {
                tgiList[i] = new TGI(br);
            }
        }

        public BBLN(int version, string partName, Xmods.DataLib.TGI linkedResourceTGI)
        {
            this.version = version;
            this.partName = partName;
            this.unknown = 2;
            if (version == 8)
            {
                this.bgType = linkedResourceTGI.Type;
                this.bgGroup = linkedResourceTGI.Group;
                this.bgInstance = linkedResourceTGI.Instance;
            }
            this.entries = new Entry[1];
            this.entries[0] = new Entry(XmodsEnums.CASregions.Body,
                                        new MorphEntry[] { new MorphEntry(77951, 1f, 0) },
                                        new MorphEntry[0]);
            if (version == 8)
            {
                this.tgiList = new TGI[] { new TGI(0, 0, 0) };
            }
            else
            {
                this.tgiList = new TGI[] { new TGI(linkedResourceTGI.Type, linkedResourceTGI.Group, linkedResourceTGI.Instance) };
            }
        }

        public void Write(BinaryWriter bw)
        {
            bw.Write(version);
            byte[] tmpb = Encoding.BigEndianUnicode.GetBytes(partName);
            this.TGI_offset = 13 + tmpb.Length;
            if (version == 8) this.TGI_offset += 16;
            for (int i = 0; i < entries.Length; i++)
            {
                this.TGI_offset += entries[i].Length;
            }
            bw.Write(TGI_offset);
            this.TGI_size = 4 + (tgiList.Length * 16);
            if (version == 8) this.TGI_size += 8;
            bw.Write(TGI_size);
            bw.Write((byte)tmpb.Length);
            bw.Write(tmpb);
            bw.Write(unknown);
            if (version == 8)
            {
                bw.Write(bgType);
                bw.Write(bgGroup);
                bw.Write(bgInstance);
            }
            bw.Write(entries.Length);
            for (int i = 0; i < entries.Length; i++)
            {
                entries[i].Write(bw);
            }
            bw.Write(tgiList.Length);
            for (int i = 0; i < tgiList.Length; i++)
            {
                tgiList[i].Write(bw);
            }
        }

        public class Entry
        {
            XmodsEnums.CASregions region;
            MorphEntry[] geomMorphs;
            MorphEntry[] boneMorphs;

            internal int Length
            {
                get { return 12 + (geomMorphs.Length * 12) + (boneMorphs.Length * 12); }
            }

            internal Entry(BinaryReader br)
            {
                region = (XmodsEnums.CASregions)br.ReadUInt32();
                int geomCount = br.ReadInt32();
                this.geomMorphs = new MorphEntry[geomCount];
                for (int i = 0; i < geomCount; i++)
                {
                    this.geomMorphs[i] = new MorphEntry(br);
                }
                int boneCount = br.ReadInt32();
                this.boneMorphs = new MorphEntry[boneCount];
                for (int i = 0; i < boneCount; i++)
                {
                    this.boneMorphs[i] = new MorphEntry(br);
                }
            }

            internal Entry(XmodsEnums.CASregions region, MorphEntry[] geomMorphs, MorphEntry[] boneMorphs)
            {
                this.region = region;
                this.geomMorphs = geomMorphs;
                this.boneMorphs = boneMorphs;
            }

            internal void Write(BinaryWriter bw)
            {
                bw.Write((uint)region);
                bw.Write(this.geomMorphs.Length);
                for (int i = 0; i < this.geomMorphs.Length; i++)
                {
                    this.geomMorphs[i].Write(bw);
                }
                bw.Write(this.boneMorphs.Length);
                for (int i = 0; i < this.boneMorphs.Length; i++)
                {
                    this.boneMorphs[i].Write(bw);
                }
            }
        }

        public class MorphEntry
        {
            uint ageGenderFlags;
            float amount;
            int tgiIndex;

            internal MorphEntry(BinaryReader br)
            {
                ageGenderFlags = br.ReadUInt32();
                amount = br.ReadSingle();
                tgiIndex = br.ReadInt32();
            }

            internal MorphEntry(uint ageGenderFlags, float amount, int tgiIndex)
            {
                this.ageGenderFlags = ageGenderFlags;
                this.amount = amount;
                this.tgiIndex = tgiIndex;
            }

            internal void Write(BinaryWriter bw)
            {
                bw.Write(ageGenderFlags);
                bw.Write(amount);
                bw.Write(tgiIndex);
            }
        }
    }
}
