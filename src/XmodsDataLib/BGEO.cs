using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace Xmods.DataLib
{
    public class BGEO
    {
        private char[] magic;
        private int version;
        private int sect1count, sect1lodcount, sect2count, sect3count;
        private int sect1headerSize, sect1lodSize;
        private int sect1offset, sect2offset, sect3offset;
        private Section1[] sect1;
        private Section2[] sect2;
        private Section3[] sect3;

        public int section1count
        {
            get
            {
                return sect1count;
            }
        }
        public int section1LODcount
        {
            get
            {
                return sect1lodcount;
            }
        }

        public int section2count()
        {
            return sect2count;
        }
        public int section2count(int section1entryNumber)
        {
            Section1 sect1 = getSection1(section1entryNumber);
            int tmp = 0;
            for (int i = 0; i < this.sect1lodcount; i++)
            {
                int[] LODdata = sect1.LODdata(i);
                tmp += LODdata[1];
            }
            return tmp;
        }
        public int section2count(int section1entryNumber, int lod)
        {
            Section1 sect1 = getSection1(section1entryNumber);
            int[] LODdata = sect1.LODdata(lod);
            return LODdata[1];
        }
        public int section3count()
        {
            return sect3count;
        }
        public int section3count(int section1entryNumber)
        {
            Section1 sect1 = getSection1(section1entryNumber);
            int tmp = 0;
            for (int i = 0; i < this.sect1lodcount; i++)
            {
                int[] LODdata = sect1.LODdata(i);
                tmp += LODdata[2];
            }
            return tmp;
        }
        public int section3count(int section1entryNumber, int lod)
        {
            Section1 sect1 = getSection1(section1entryNumber);
            int[] LODdata = sect1.LODdata(lod);
            return LODdata[2];
        }
        public Section1 getSection1(int section1entryNumber)
        {
            return sect1[section1entryNumber];
        }
        public Section2 getSection2(int section2index)
        {
            return sect2[section2index];
        }
        public float[] getSection3(int section3index)
        {
            return sect3[section3index].DeltaValues;
        }

        public int getSection2StartIndex(int section1entryNumber, int lod)
        {
            int tmp = 0;
            for (int i = 0; i < section1entryNumber; i++)
            {
                for (int j = 0; j < this.section1LODcount; j++)
                {
                    int[] lodDatatmp = sect1[i].LODdata(j);
                    tmp += lodDatatmp[1];
                }
            }
            for (int j = 0; j < lod; j++)
            {
                int[] lodDatatmp = sect1[section1entryNumber].LODdata(j);
                tmp += lodDatatmp[1];
            }
            return tmp;
        }
        public int getSection3StartIndex(int section1entryNumber, int lod)
        {
            int tmp = 0;
            for (int i = 0; i < section1entryNumber; i++)
            {
                for (int j = 0; j < this.section1LODcount; j++)
                {
                    int[] lodDatatmp = sect1[i].LODdata(j);
                    tmp += lodDatatmp[2];
                }
            }
            for (int j = 0; j < lod; j++)
            {
                int[] lodDatatmp = sect1[section1entryNumber].LODdata(j);
                tmp += lodDatatmp[2];
            }
            return tmp;
        }
        public int getLODstartVertexID(int section1entryNumber, int lod)
        {
            int[] lodDatatmp = sect1[section1entryNumber].LODdata(lod);
            return lodDatatmp[0];
        }
        public int getLODinitialOffset(int section1entryNumber, int lod)
        {
            //Offsets are calculated for all lods in the morphs as one set of numbers, with the total number of vertices
            //for each lod added at the end of the lod. Get the initial offset relative to the beginning of the current
            //lod, before the first section 2 offset. 
            if (section1entryNumber == 0 & lod == 0) return 0;
            int offset = 0;
            for (int e = 0; e < section1entryNumber; e++)
            {                                   //add offsets for all vertices in previous entries
                int eStart = this.getSection2StartIndex(e, 0);
                int eCount = this.section2count(e);
                for (int j = eStart; j < eStart + eCount; j++)
                {
                    Section2 tmp = this.getSection2(j);
                    offset += tmp.dataOffset;
                }
            }
            for (int i = 0; i < lod; i++)      //add offsets for all vertices in previous lods in current entry
            {
                int lodStart = this.getSection2StartIndex(section1entryNumber, i);
                int lodCount = this.section2count(section1entryNumber, i);
                for (int j = lodStart; j < lodStart + lodCount; j++)
                {
                    Section2 tmp = this.getSection2(j);
                    offset += tmp.dataOffset;
                }
            }
            return offset;
        }

        public BGEO() { }

        public BGEO(GEOM[][][] geomArray, int[] ageArray, int[] genderArray, int[] speciesArray, int[] regionArray)     //dimensions: age/gender group, lod, mesh - for slider morphs
        {
            if (geomArray.GetLength(0) != ageArray.Length | geomArray.GetLength(0) != genderArray.Length |
                geomArray.GetLength(0) != speciesArray.Length | geomArray.GetLength(0) != regionArray.Length) 
                throw new BlendException("Age/gender/species/region array lengths do not match GEOM array length!");
            this.magic = new char[4] { 'B', 'G', 'E', 'O' };
            this.version = 768;
            this.sect1count = geomArray.GetLength(0);
            this.sect1lodcount = 4;
            this.sect1 = new Section1[this.sect1count];
            List<Section2> lstSect2 = new List<Section2>();
            List<Section3> lstSect3 = new List<Section3>();
            int indexIn = 0, indexOut = 0;
            for (int s = 0; s < this.sect1count; s++)
            {
                int[][] sect1info = new int[3][];
                for (int i = 0; i < 3; i++) sect1info[i] = new int[4];
                for (int i = 0; i < geomArray[s].Length; i++)
                {
                    if (geomArray[s][i] == null || geomArray[s][i].Length == 0) continue;
                    Section2[] tmpSect2 = null;
                    Section3[] tmpSect3 = null;
                    int firstVert = 0;
                    BGEOLODConstructor(geomArray[s][i], indexIn, out firstVert, out tmpSect2, out tmpSect3, out indexOut);
                    sect1info[0][i] = firstVert;
                    sect1info[1][i] = tmpSect2.Length;
                    sect1info[2][i] = tmpSect3.Length;
                    lstSect2.AddRange(tmpSect2);
                    lstSect3.AddRange(tmpSect3);
                    indexIn = indexOut;
                }
                this.sect1[s] = new Section1(ageArray[s], genderArray[s], speciesArray[s], regionArray[s], sect1info[0], sect1info[1], sect1info[2]);
            }
            this.sect1headerSize = 8;
            this.sect1lodSize = 12;
            this.sect1offset = 44;
            this.sect2 = lstSect2.ToArray();
            this.sect2offset = 44 + (this.sect1count * 56);
            this.sect2count = this.sect2.Length;
            this.sect3 = lstSect3.ToArray();
            this.sect3offset = this.sect2offset + (this.sect2count * 2);
            this.sect3count = this.sect3.Length;
        }

        public BGEO(GEOM[][] geomArray)     //dimensions: lod, mesh - for clothing fat/fit/thin/special morphs
        {
            this.magic = new char[4] {'B', 'G', 'E', 'O'};
            this.version = 768;
            this.sect1count = 1;
            this.sect1lodcount = 4;
            this.sect1 = new Section1[1];
            List<Section2> lstSect2 = new List<Section2>();
            List<Section3> lstSect3 = new List<Section3>();
            int indexIn = 0, indexOut = 0;
            int [][] sect1info = new int[3][];
            for (int i = 0; i < 3; i++) sect1info[i] = new int[4];
            for (int i = 0; i < geomArray.Length; i++)
            {
                if (geomArray[i] == null || geomArray[i].Length == 0) continue;
                Section2[] tmpSect2 = null;
                Section3[] tmpSect3 = null;
                int firstVert = 0;
                BGEOLODConstructor(geomArray[i], indexIn, out firstVert, out tmpSect2, out tmpSect3, out indexOut);
                sect1info[0][i] = firstVert;
                sect1info[1][i] = tmpSect2.Length;
                sect1info[2][i] = tmpSect3.Length;
                lstSect2.AddRange(tmpSect2);
                lstSect3.AddRange(tmpSect3);
                indexIn = indexOut;
            }
            int age = (int)XmodsEnums.Age.AllAges;
            int gender = (int)XmodsEnums.Gender.Unisex;
            int species = (int)XmodsEnums.Species.Human;
            int region = (int)XmodsEnums.CASregions.Body;
            this.sect1[0] = new Section1(age, gender, species, region, sect1info[0], sect1info[1], sect1info[2]);
            this.sect1headerSize = 8;
            this.sect1lodSize = 12;
            this.sect1offset = 44;
            this.sect2offset = 100;
            this.sect2 = lstSect2.ToArray();
            this.sect2count = this.sect2.Length;
            this.sect3offset = 100 + (this.sect2count * 2);
            this.sect3 = lstSect3.ToArray();
            this.sect3count = this.sect3.Length;
        }

        internal void BGEOLODConstructor(GEOM[] lodMorphMeshes, int indexIn, out int firstVertID, out Section2[] outSection2, out Section3[] outSection3, out int indexOut)
        {
            float posLimit = 0f;
            float normLimit = 0f;
            int lowVertID = Int32.MaxValue;
            int hiVertID = 0;
            for (int seq = 0; seq < lodMorphMeshes.Length; seq++)
            {
                if (!lodMorphMeshes[seq].isMorph) throw new BlendException("Not a valid morph mesh!");
                lowVertID = Math.Min(lodMorphMeshes[seq].minVertexID, lowVertID);
                hiVertID = Math.Max(lodMorphMeshes[seq].maxVertexID, hiVertID);
            }
            bool overlap = false;
            if ((lodMorphMeshes.Length > 1) &&
                ((lodMorphMeshes[1].minVertexID > lodMorphMeshes[0].minVertexID & lodMorphMeshes[1].minVertexID < lodMorphMeshes[0].maxVertexID)
                | (lodMorphMeshes[1].maxVertexID > lodMorphMeshes[0].minVertexID & lodMorphMeshes[1].maxVertexID < lodMorphMeshes[0].maxVertexID)))
                overlap = true;
            if ((lodMorphMeshes.Length > 2) &&
                ((lodMorphMeshes[2].minVertexID > lodMorphMeshes[0].minVertexID & lodMorphMeshes[2].minVertexID < lodMorphMeshes[0].maxVertexID)
                | (lodMorphMeshes[2].maxVertexID > lodMorphMeshes[0].minVertexID & lodMorphMeshes[2].maxVertexID < lodMorphMeshes[0].maxVertexID)))
                overlap = true;
            if ((lodMorphMeshes.Length > 2) &&
                ((lodMorphMeshes[2].minVertexID > lodMorphMeshes[1].minVertexID & lodMorphMeshes[2].minVertexID < lodMorphMeshes[1].maxVertexID)
                | (lodMorphMeshes[2].maxVertexID > lodMorphMeshes[1].minVertexID & lodMorphMeshes[2].maxVertexID < lodMorphMeshes[1].maxVertexID)))
                overlap = true;
            if (overlap)
            {
                if (MessageBox.Show("Your meshes have overlapping vertex IDs within\na LOD and the morph may not work correctly.\nDo you want to continue anyway?",
                    "Vertex numbering alert", MessageBoxButtons.YesNo) == DialogResult.No)
                    throw new BlendException("Vertex numbering error");
            }
            int numVertIDs = hiVertID - lowVertID + 1;
            vertexData[] vertList = new vertexData[numVertIDs];
            for (int seq = 0; seq < lodMorphMeshes.Length; seq++)
            {
                for (int i = 0; i < lodMorphMeshes[seq].numberVertices; i++)
                {
                    int ID = lodMorphMeshes[seq].getVertexID(i);
                    vertList[ID - lowVertID] = new vertexData(ID, lodMorphMeshes[seq].getPosition(i), lodMorphMeshes[seq].getNormal(i));
                }
            }
            bool gap = false;
            float[] nothing = new float[3];
            for (int i = 0; i < vertList.Length; i++)
            {
                if (vertList[i] == null)
                {
                    gap = true;
                    vertList[i] = new vertexData(i + lowVertID, nothing, nothing);
                }
            }
            if (gap)
            {
                if (MessageBox.Show("Your meshes have a gap in vertex IDs within\na LOD but the morph will probably work.\nDo you want to continue anyway?",
                    "Vertex numbering alert", MessageBoxButtons.YesNo) == DialogResult.No)
                    throw new BlendException("Vertex numbering error");
            }
            Section2[] newSect2 = new Section2[numVertIDs];
            List<Section3> listSect3 = new List<Section3>();
            int offset = 0 - indexIn;
            for (int i = 0; i < vertList.Length; i++)
            {
                bool hasPos = false;
                bool hasNorm = false;
                if (Math.Abs(vertList[i].posX) > posLimit | Math.Abs(vertList[i].posY) > posLimit | Math.Abs(vertList[i].posZ) > posLimit)
                {
                    listSect3.Add(new Section3(vertList[i].posX, vertList[i].posY, vertList[i].posZ));
                    hasPos = true;
                 }
                if (Math.Abs(vertList[i].normX) > normLimit | Math.Abs(vertList[i].normY) > normLimit | Math.Abs(vertList[i].normZ) > normLimit)
                {
                    listSect3.Add(new Section3(vertList[i].normX, vertList[i].normY, vertList[i].normZ));
                    hasNorm = true;
                }
                if (hasPos | hasNorm)
                {
                    newSect2[i] = new Section2(hasPos, hasNorm, offset);
                    offset = 0;
                    if (hasPos) offset++;
                    if (hasNorm) offset++;
                }
                else
                {
                    newSect2[i] = new Section2(false, false, 0);
                }
            }

            firstVertID = lowVertID;
            outSection2 = newSect2;
            outSection3 = listSect3.ToArray();
            indexOut = outSection3.Length - offset;
        }

        public void ReadFile(BinaryReader br)
        {
            this.magic = br.ReadChars(4);
            if (new string(this.magic) != "BGEO")
            {
                throw new BlendException("Not a valid BGEO file.");
            }
            this.version = br.ReadInt32();
            if (version != 768)
            {
                throw new BlendException("Not a recognized BGEO version.");
            }
            this.sect1count = br.ReadInt32();
            this.sect1lodcount = br.ReadInt32();
            this.sect2count = br.ReadInt32();
            this.sect3count = br.ReadInt32();
            this.sect1headerSize = br.ReadInt32();
            this.sect1lodSize = br.ReadInt32();
            this.sect1offset = br.ReadInt32();
            this.sect2offset = br.ReadInt32();
            this.sect3offset = br.ReadInt32();
            this.sect1 = new Section1[sect1count];
            for (int i = 0; i < sect1count; i++)
            {
                this.sect1[i] = new Section1(br, sect1lodcount);
            }
            this.sect2 = new Section2[sect2count];
            for (int i = 0; i < sect2count; i++)
            {
                this.sect2[i] = new Section2(br);
            }
            this.sect3 = new Section3[sect3count];
            for (int i = 0; i < sect3count; i++)
            {
                this.sect3[i] = new Section3(br);
            }
            int sect2index = 0;
            for (int i = 0; i < sect1count; i++)
            {
                sect2index = this.sect1[i].FixSection3Count(this.sect2, sect1lodcount, sect2index);
            }
        }

        public void WriteFile(BinaryWriter bw)
        {
            bw.Write(this.magic);
            bw.Write(this.version);
            bw.Write(this.sect1count);
            bw.Write(this.sect1lodcount);
            bw.Write(this.sect2count);
            bw.Write(this.sect3count);
            bw.Write(this.sect1headerSize);
            bw.Write(this.sect1lodSize);
            bw.Write(this.sect1offset);
            bw.Write(this.sect2offset);
            bw.Write(this.sect3offset);
            for (int i = 0; i < this.sect1count; i++)
            {
                this.sect1[i].Write(bw, this.sect1lodcount);
            }
            for (int i = 0; i < this.sect2count; i++)
            {
                this.sect2[i].Write(bw);
            }
            for (int i = 0; i < this.sect3count; i++)
            {
                this.sect3[i].Write(bw);
            }
        }

        public class Section1
        {
            uint agegenderFlags, region;
            int[] firstVertID, numVertIDs, entryCount, original_entryCount;

            internal Section1() { }

            internal Section1(BinaryReader br, int sect1lodcount)
            {
                this.agegenderFlags = br.ReadUInt32();
                this.region = br.ReadUInt32();
                this.firstVertID = new int[sect1lodcount];
                this.numVertIDs = new int[sect1lodcount];
                this.entryCount = new int[sect1lodcount];
                this.original_entryCount = new int[sect1lodcount];
                for (int i = 0; i < sect1lodcount; i++)
                {
                    this.firstVertID[i] = br.ReadInt32();
                    this.numVertIDs[i] = br.ReadInt32();
                    this.entryCount[i] = br.ReadInt32();
                    this.original_entryCount[i] = this.entryCount[i];
                }
            }
            public Section1(int age, int gender, int species, int region, int[] firstVertID, int[] numVertIDs, int[] entryCount)
            {
                this.agegenderFlags = (uint)(age + (gender << 12) + (species << 8)) + (1 << 16);
                this.region = (uint)region;
                this.firstVertID = firstVertID;
                this.numVertIDs = numVertIDs;
                this.entryCount = entryCount;
                this.original_entryCount = entryCount;
                if (firstVertID.Length != 4 | numVertIDs.Length != 4 | entryCount.Length != 4)
                    throw new BlendException("Section 1 constructor: LOD information arrays must have four elements.");
            }
            internal void Write(BinaryWriter bw, int sect1lodcount)
            {
                bw.Write(this.agegenderFlags);
                bw.Write(this.region);
                for (int i = 0; i < sect1lodcount; i++)
                {
                    bw.Write(this.firstVertID[i]);
                    bw.Write(this.numVertIDs[i]);
                    bw.Write(this.entryCount[i]);
                }
            }
            internal int FixSection3Count(Section2[] sect2, int sect1lodcount, int sect2index)
            {
                int ind = sect2index;
                for (int i = 0; i < sect1lodcount; i++)
                {
                    int fixedCount = 0;
                    for (int j = ind; j < ind + this.numVertIDs[i]; j++)
                    {
                        if (sect2[j].hasPositionData) fixedCount++;
                        if (sect2[j].hasNormalsData) fixedCount++;
                    }
                    this.entryCount[i] = Math.Max(fixedCount, this.original_entryCount[i]);
                    ind += this.numVertIDs[i];
                }
                return ind;
            }
            public int age
            {
                get
                {
                    uint tmp = (this.agegenderFlags & 0xFF);
                    return (int)tmp;
                }
            }
            public int gender
            {
                get
                {
                    uint tmp = (this.agegenderFlags & 0xF000) >> 12;
                    return (int)tmp;
                }
            }
            public int species
            {
                get
                {
                    uint tmp = (this.agegenderFlags & 0xF00) >> 8;
                    return (int)tmp;
                }
            }
            public int CASregion
            {
                get
                {
                    return (int)this.region;
                }
            }
            public override string ToString()
            {
                return Enum.GetName(typeof(XmodsEnums.Age), this.age) + ", " + Enum.GetName(typeof(XmodsEnums.Gender), this.gender) + Environment.NewLine +
                    "LOD 0: First vertex " + this.firstVertID[0].ToString() + ", Number vertices " + this.numVertIDs[0].ToString() + ", Number entries " + this.entryCount[0] + Environment.NewLine +
                    "LOD 1: First vertex " + this.firstVertID[1].ToString() + ", Number vertices " + this.numVertIDs[1].ToString() + ", Number entries " + this.entryCount[1] + Environment.NewLine +
                    "LOD 2: First vertex " + this.firstVertID[2].ToString() + ", Number vertices " + this.numVertIDs[2].ToString() + ", Number entries " + this.entryCount[2] + Environment.NewLine +
                    "LOD 3: First vertex " + this.firstVertID[3].ToString() + ", Number vertices " + this.numVertIDs[3].ToString() + ", Number entries " + this.entryCount[3];
            }
            public string LODdatatoString(int LOD)
            {
                return this.firstVertID[LOD].ToString() + ", " + this.numVertIDs[LOD].ToString() + ", " + this.entryCount[LOD].ToString();
            }
            public int[] LODdata(int LOD)
            {
                return new int[] { this.firstVertID[LOD], this.numVertIDs[LOD], this.entryCount[LOD], this.original_entryCount[LOD] };
            }
        }
        public class Section2
        {
            bool hasPosition, hasNormals;
            int offset;
            internal Section2(BinaryReader br)
            {
                short tmp = br.ReadInt16();
                this.hasPosition = (tmp & 1) == 1;
                this.hasNormals = (tmp & 2) == 2;
                this.offset = tmp >> 2;
            }
            public Section2()
            {
                this.hasPosition = false;
                this.hasNormals = false;
                this.offset = 0;
            }
            public Section2(bool PositionFlag, bool NormalsFlag, int ListOffset)
            {
                this.hasPosition = PositionFlag;
                this.hasNormals = NormalsFlag;
                this.offset = ListOffset;
            }
            internal void Write(BinaryWriter bw)
            {
                short tmp = (short)(this.offset << 2);                
                if (this.hasPosition) tmp += 1;
                if (this.hasNormals) tmp += 2;
                bw.Write(tmp);
            }
            public override string ToString()
            {
                return "Position: " + hasPosition.ToString() + ", Normals: " + hasNormals.ToString() + ", Offset: " + offset.ToString();
            }
            public bool hasPositionData
            {
                get
                {
                    return this.hasPosition;
                }
            }
            public bool hasNormalsData
            {
                get
                {
                    return this.hasNormals;
                }
            }
            public int dataOffset
            {
                get
                {
                    return offset;
                }
            }
        }

        internal class Section3
        {
            float x, y, z;
            internal Section3(BinaryReader br)
            {
                UInt16 xtmp = br.ReadUInt16();
                UInt16 ytmp = br.ReadUInt16();
                UInt16 ztmp = br.ReadUInt16();
                this.x = convertIn(xtmp);
                this.y = convertIn(ytmp);
                this.z = convertIn(ztmp);
            }
            public Section3(float Xvalue, float Yvalue, float Zvalue)
            {
                this.x = Xvalue;
                this.y = Yvalue;
                this.z = Zvalue;
            }
            internal void Write(BinaryWriter bw)
            {
                UInt16 xtmp = convertOut(this.x);
                UInt16 ytmp = convertOut(this.y);
                UInt16 ztmp = convertOut(this.z);
                bw.Write(xtmp);
                bw.Write(ytmp);
                bw.Write(ztmp);
            }
            internal float convertIn(UInt16 encode)
            {
                int tmp = encode;
                int tmp2 = tmp ^ 0x8000;            //flip sign bit
                int tmp3 = tmp2 << 16;
                int tmp4 = tmp3 >> 16;
                float tmpf = tmp4 / 2000f;
                return tmpf;
            }
            internal UInt16 convertOut(float decode)
            {
                int tmp = Convert.ToInt32(decode * 2000f);
                int tmp2 = tmp ^ 0x8000;            //flip sign bit
                byte[] tmp3 = BitConverter.GetBytes(tmp2);
                UInt16 tmp4 = BitConverter.ToUInt16(tmp3, 0);
                return tmp4;
            }
            public override string ToString()
            {
                return "X: " + x.ToString() + ", Y: " + y.ToString() + ", Z: " + z.ToString();
            }
            public float[] DeltaValues
            {
                get
                {
                    return new float[] { this.x, this.y, this.z };
                }
            }
        }

        internal class vertexData
        {
            internal int vertexID;
            internal float posX, posY, posZ, normX, normY, normZ;
            internal vertexData(int ID, float[] position, float[] normals)
            {
                this.vertexID = ID;
                this.posX = position[0];
                this.posY = position[1];
                this.posZ = position[2];
                this.normX = normals[0];
                this.normY = normals[1];
                this.normZ = normals[2];
            }
        }

        [global::System.Serializable]
        public class BlendException : ApplicationException
        {
            public BlendException() { }
            public BlendException(string message) : base(message) { }
            public BlendException(string message, Exception inner) : base(message, inner) { }
            protected BlendException(
              System.Runtime.Serialization.SerializationInfo info,
              System.Runtime.Serialization.StreamingContext context)
                : base(info, context) { }
        }
    }
}
