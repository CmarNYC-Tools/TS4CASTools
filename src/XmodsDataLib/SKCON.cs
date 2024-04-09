using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Xmods.DataLib
{
    public class SKCON
    {
        int version;
        int boneCnt;
        string[] boneNames;
        int transformCount;
        float[,] transforms;

        public int boneCount
        {
            get
            {
                return boneCnt;
            }
        }
        public string boneName(int index)
        {
            return boneNames[index];
        }
        public float[] transform(int index)
        {
            float[] tmp = new float[12];
            for (int i = 0; i < 12; i++)
            {
                tmp[i] = transforms[index, i];
            }
            return tmp;
        }

        public SKCON() {}

        public void ReadFile(BinaryReader br)
        {
            version = br.ReadInt32();
            boneCnt = br.ReadInt32();
            boneNames = new string[boneCnt];
            for (int i = 0; i < boneCnt; i++)
            {
                int tmp = br.ReadByte();
                byte[] tmpb = br.ReadBytes(tmp);
                boneNames[i] = Encoding.BigEndianUnicode.GetString(tmpb);
            }
            transformCount = br.ReadInt32();
            transforms = new float[transformCount, 12];
            for (int i = 0; i < transformCount; i++)
            {
                for (int j = 0; j < 12; j++)
                {
                    transforms[i, j] = br.ReadSingle();
                }
            }
        }

    }
}
