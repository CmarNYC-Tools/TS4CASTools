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
using System.Windows.Forms;
using System.Globalization;

namespace Xmods.DataLib
{
    public class OBJ
    {
        List<Vertex> vertexList;
        List<UV> uvList;
        List<Normal> normalList;
        List<Group> groupList;

        public OBJ.Vertex[] vertexArray
        {
            get { return this.vertexList.ToArray(); }
            set { this.vertexList = new List<Vertex>(value); }
        }

        public OBJ.UV[] uvArray
        {
            get { return this.uvList.ToArray(); }
            set { this.uvList = new List<UV>(value); }
        }

        public OBJ.Normal[] normalArray
        {
            get { return this.normalList.ToArray(); }
            set { this.normalList = new List<Normal>(value); }
        }
        
        public int numberGroups
        {
            get { return this.groupList.Count; }
        }

        public OBJ.Group[] groupArray
        {
            get { return this.groupList.ToArray(); }
        }

        public string getGroupName(int groupIndex)
        {
            return this.groupList[groupIndex].groupName;
        }

        public string[] getGroupNames()
        {
            string[] tmp = new string[this.numberGroups];
            for (int i = 0; i < this.numberGroups; i++)
            {
                tmp[i] = this.groupList[i].groupName;
            }
            return tmp;
        }

        public int totalFaces
        {
            get
            {
                int tot = 0;
                foreach (OBJ.Group g in this.groupArray)
                {
                    tot += g.numberFaces;
                }
                return tot;
            }
        }

        public OBJ() { }

        public OBJ(StreamReader sr)
        {
            sr.BaseStream.Position = 0;
            vertexList = new List<Vertex>();
            uvList = new List<UV>();
            normalList = new List<Normal>();
            groupList = new List<Group>();
            string str;
            string[] sep = new string[] { " " };
            string[] slash = new string[] { "/" };
            int grpInd = 0;
            while ((str = sr.ReadLine()) != null)
            {
                if (str.StartsWith("v "))
                {
                    string[] v = str.Split(sep, System.StringSplitOptions.RemoveEmptyEntries);
                    try 
                    {
                        vertexList.Add(new Vertex(float.Parse(v[1], CultureInfo.InvariantCulture),
                            float.Parse(v[2], CultureInfo.InvariantCulture), float.Parse(v[3], CultureInfo.InvariantCulture)));
                    }
                    catch
                    {
                        DialogResult res = MessageBox.Show("Error in .obj text: " + str + System.Environment.NewLine +
                            "Skip and continue anyway?", "OBJ Error", MessageBoxButtons.OKCancel);
                        if (res == DialogResult.Cancel) return;
                        vertexList.Add(new Vertex(0f, 0f, 0f));
                    }
                }
                else if (str.StartsWith("vt "))
                {
                    string[] t = str.Split(sep, System.StringSplitOptions.RemoveEmptyEntries);
                    try
                    {
                        uvList.Add(new UV(float.Parse(t[1], CultureInfo.InvariantCulture), float.Parse(t[2], CultureInfo.InvariantCulture)));
                    }
                    catch
                    {
                        DialogResult res = MessageBox.Show("Error in .obj text: " + str + System.Environment.NewLine +
                            "Skip and continue anyway?", "OBJ Error", MessageBoxButtons.OKCancel);
                        if (res == DialogResult.Cancel) return;
                        uvList.Add(new UV(0f, 0f));
                    }
                }
                else if (str.StartsWith("vn "))
                {
                    string[] n = str.Split(sep, System.StringSplitOptions.RemoveEmptyEntries);
                    try
                    {
                        normalList.Add(new Normal(float.Parse(n[1], CultureInfo.InvariantCulture),
                            float.Parse(n[2], CultureInfo.InvariantCulture), float.Parse(n[3], CultureInfo.InvariantCulture)));
                    }
                    catch
                    {
                        DialogResult res = MessageBox.Show("Error in .obj text: " + str + System.Environment.NewLine +
                            "Skip and continue anyway?", "OBJ Error", MessageBoxButtons.OKCancel);
                        if (res == DialogResult.Cancel) return;
                        normalList.Add(new Normal(0f, 0f, 0f));
                    }
                }
                else if (str.StartsWith("f "))
                {
                    if (groupList.Count == 0)
                    {
                        groupList.Add(new Group());
                        grpInd = 0;
                    }
                    try
                    {
                        string[] f = str.Split(sep, System.StringSplitOptions.RemoveEmptyEntries);
                        int[][] points = new int[4][];
                        for (int i = 1; i < f.Length; i++)
                        {
                            string[] ptmp = f[i].Split(slash, System.StringSplitOptions.None);
                            int[] itmp = new int[ptmp.Length];
                            for (int j = 0; j < itmp.Length; j++)
                            {
                                if (ptmp[j] == String.Empty)
                                {
                                    itmp[j] = 0;
                                }
                                else
                                {
                                    itmp[j] = Int32.Parse(ptmp[j], CultureInfo.InvariantCulture);
                                }
                                if (itmp[j] < 0)
                                {
                                    if (j == 0)
                                    {
                                        itmp[j] = vertexList.Count + itmp[j];
                                    }
                                    else if (j == 1)
                                    {
                                        itmp[j] = uvList.Count + itmp[j];
                                    }
                                    else if (j == 2)
                                    {
                                        itmp[j] = normalList.Count + itmp[j];
                                    }
                                }
                            }
                            points[i-1] = itmp;
                        }
                        groupList[grpInd].addFace(new Face(points[0], points[1], points[2]));
                        if (f.Length > 4)
                        {
                            groupList[grpInd].addFace(new Face(points[2], points[3], points[0]));
                        }
                    }
                    catch
                    {
                        DialogResult res = MessageBox.Show("Error in .obj text: " + str + System.Environment.NewLine +
                            "Skip and continue anyway?", "OBJ Error", MessageBoxButtons.OKCancel);
                        if (res == DialogResult.Cancel) return;
                    }
                }
                else if (str.StartsWith("g "))
                {
                    string[] g = str.Split(sep, System.StringSplitOptions.RemoveEmptyEntries);
                    if (g.Length > 1)
                    {
                        bool found = false;
                        for (int i = 0; i < groupList.Count; i++)
                        {
                            if (String.CompareOrdinal(groupList[i].groupName, g[1]) == 0)
                            {
                                grpInd = i;
                                found = true;
                                break;
                            }
                        }
                        if (!found)
                        {
                            groupList.Add(new Group(g[1]));
                            grpInd = groupList.Count - 1;
                        }
                    }
                }
            }
        }

        public OBJ(GEOM[] geom, string[] groupNames, int uvSet)
        {
            vertexList = new List<Vertex>();
            normalList = new List<Normal>();
            uvList = new List<UV>();
            groupList = new List<Group>();

            int vertOffset = 1;
            int groupNum = 0;
            for (int m = 0; m < geom.Length; m++)
            {
                if (!geom[m].hasUVset(uvSet))
                {
                    DialogResult res = MessageBox.Show("Input GEOM does not have UV set " + uvSet.ToString() + "! Continue using UV0?", "No UV" + uvSet.ToString(), MessageBoxButtons.OKCancel);
                    if (res == DialogResult.Cancel) return;
                }
                for (int i = 0; i < geom[m].numberVertices; i++)
                {
                    vertexList.Add(new Vertex(geom[m].getPosition(i)));
                    normalList.Add(new Normal(geom[m].getNormal(i)));
                    uvList.Add(new UV(geom[m].getUV(i, uvSet), true));
                }
                this.groupList.Add(new Group(groupNames != null ? groupNames[m] : "Group" + m.ToString()));
                for (int i = 0; i < geom[m].numberFaces; i++)
                {
                    this.groupList[groupNum].addFace(new Face(geom[m].getFaceIndices(i), vertOffset)); 
                }
                vertOffset += geom[m].numberVertices;
                groupNum++;
            }
        }

        public OBJ(GEOM geom, List<GEOM.Face[]> layers, List<string> layerNames, int uvSet)
        {
            vertexList = new List<Vertex>();
            normalList = new List<Normal>();
            uvList = new List<UV>();
            groupList = new List<Group>();

            int vertOffset = 1;
            int groupNum = 0;
            if (!geom.hasUVset(uvSet))
            {
                DialogResult res = MessageBox.Show("Input GEOM does not have UV set " + uvSet.ToString() + "! Continue using UV0?", "No UV" + uvSet.ToString(), MessageBoxButtons.OKCancel);
                if (res == DialogResult.Cancel) return;
            }
            for (int i = 0; i < geom.numberVertices; i++)
            {
                vertexList.Add(new Vertex(geom.getPosition(i)));
                normalList.Add(new Normal(geom.getNormal(i)));
                uvList.Add(new UV(geom.getUV(i, uvSet), true));
            }

            for (int l = 0; l < layers.Count; l++)
            {
                string grpName;
                if (layerNames != null)
                {
                    grpName = layerNames[l] + l.ToString();
                }
                else
                {
                    grpName = "Layer" + l.ToString();
                }
                this.groupList.Add(new Group(grpName));
                for (int i = 0; i < layers[l].Length; i++)
                {
                    this.groupList[groupNum].addFace(new Face(new int[] { layers[l][i].facePoint0, 
                        layers[l][i].facePoint1, layers[l][i].facePoint2 }, vertOffset));
                }
                groupNum++;
            }
        }

     /*   public OBJ(GEOM baseMesh, GEOM[] morphs)
        {
            string[] morphGroups = { "group_fat", "group_fit", "group_thin", "group_special" };

            vertexList = new List<Vertex>();
            normalList = new List<Normal>();
            uvList = new List<UV>();
            groupList = new List<Group>();

            for (int i = 0; i < baseMesh.numberVertices; i++)
            {
                vertexList.Add(new Vertex(baseMesh.getPosition(i)));
                normalList.Add(new Normal(baseMesh.getNormal(i)));
                uvList.Add(new UV(baseMesh.getUV(i, 0), true));
            }
            this.groupList.Add(new Group("group_base"));
            for (int i = 0; i < baseMesh.numberFaces; i++)
            {
                this.groupList[0].addFace(new Face(baseMesh.getFaceIndices(i), 1, meshType.baseMesh));
            }

            int vIndex = baseMesh.numberVertices + 1;
            int gIndex = 0;

            if (morphs != null)
            {
                for (int i = 0; i < morphs.Length; i++)
                {
                    if (morphs[i] != null)
                    {
                        for (int j = 0; j < morphs[i].numberVertices; j++)
                        {
                            vertexList.Add(new Vertex(addArrays(baseMesh.getPosition(j), morphs[i].getPosition(j))));
                            normalList.Add(new Normal(addArrays(baseMesh.getNormal(j), morphs[i].getNormal(j))));
                        }
                        this.groupList.Add(new Group(morphGroups[i]));
                        gIndex++;
                        for (int j = 0; j < morphs[i].numberFaces; j++)
                        {
                            this.groupList[gIndex].addFace(new Face(morphs[i].getFaceIndices(j), vIndex, meshType.morphMesh));
                        }
                        vIndex += morphs[i].numberVertices;
                    }
                }
            }
        }    */

        public void Write(StreamWriter sw)
        {
            sw.WriteLine("# Generated by XMODS on " + System.DateTime.Now.ToString("MM/dd/yyyy"));
            sw.WriteLine();
            foreach (Vertex v in vertexList)
            {
                sw.WriteLine("v " + v.ToString());
            }
            sw.WriteLine();
            foreach (UV vt in uvList)
            {
                sw.WriteLine("vt " + vt.ToString());
            }
            sw.WriteLine();
            foreach (Normal vn in normalList)
            {
                sw.WriteLine("vn " + vn.ToString());
            }
            sw.WriteLine();
            foreach (Group g in groupList)
            {
                sw.WriteLine("g " + g.groupName);
                foreach (Face f in g.facesList)
                {
                    sw.WriteLine("f " + f.ToString());
                }
                sw.WriteLine();
                sw.WriteLine("# Group " + g.groupName + " Total Faces: " + g.numberFaces);
                sw.WriteLine();
            }
            sw.WriteLine("# Total groups: " + groupList.Count.ToString() + ", Total vertices: " + vertexList.Count.ToString());
            sw.Flush();
        }

        public void CalculateNormals(bool IgnoreSeams)
        {
            Vector3[][] refFaces = new Vector3[this.totalFaces][];
            int[][] refIndexes = new int[this.totalFaces][];
            int index = 0;
            foreach (Group g in this.groupList)
            {
                for (int i = 0; i < g.facesList.Count; i++)
                {
                    int[][] facePnts = g.facesList[i].facePoints;
                    refIndexes[index] = new int[3];
                    refIndexes[index][0] = facePnts[0][0];
                    refIndexes[index][1] = facePnts[1][0];
                    refIndexes[index][2] = facePnts[2][0];
                    refFaces[index] = new Vector3[3];
                    refFaces[index][0] = new Vector3(this.vertexList[facePnts[0][0] - 1].Coordinates);
                    refFaces[index][1] = new Vector3(this.vertexList[facePnts[1][0] - 1].Coordinates);
                    refFaces[index][2] = new Vector3(this.vertexList[facePnts[2][0] - 1].Coordinates);
                    g.facesList[i].p1[2] = g.facesList[i].p1[0];
                    g.facesList[i].p2[2] = g.facesList[i].p2[0];
                    g.facesList[i].p3[2] = g.facesList[i].p3[0];
                    index++;
                }
            }
            List<int>[] refFaceRefs = new List<int>[this.vertexList.Count];
            for (int i = 0; i < this.vertexList.Count; i++)
            {
                refFaceRefs[i] = new List<int>();
            }
            for (int i = 0; i < this.vertexList.Count; i++)
            {
                for (int j = 0; j < refIndexes.Length; j++)
                {
                    for (int k = 0; k < 3; k++)
                    {
                        if (i + 1 == refIndexes[j][k]) refFaceRefs[i].Add(j);
                    }
                }
                if (IgnoreSeams)
                {
                    for (int j = 0; j < this.vertexList.Count; j++)
                    {
                        if (i != j &&
                            (new Vector3(this.vertexList[i].Coordinates) == new Vector3(this.vertexList[j].Coordinates)))
                        {
                            refFaceRefs[j].AddRange(refFaceRefs[i]);
                        }
                    }
                }
            }

            this.normalList = new List<Normal>();
            for (int i = 0; i < refFaceRefs.Length; i++)
            {
                Vector3 norm = new Vector3(0f, 0f, 0f);
                foreach (int j in refFaceRefs[i])
                {
                    norm += Triangle.Normal(refFaces[j]);
                }
                norm.Normalize();
                this.normalList.Add(new Normal(norm.Coordinates));
            }
        }

        public void AddEmptyUV()
        {
            this.uvList.Add(new UV(0f, 0f));
            for (int i = 0; i < this.numberGroups; i++)
            {
                for (int j = 0; j < this.groupList[i].numberFaces; j++)
                {
                    this.groupList[i].facesList[j].p1[1] = 1;
                    this.groupList[i].facesList[j].p2[1] = 1;
                    this.groupList[i].facesList[j].p3[1] = 1;
                }
            }
        }

        public void FlipUV(bool verticalFlip, bool horizontalFlip)
        {
            OBJ.UV[] tmp = new OBJ.UV[this.uvArray.Length];
            for (int i = 0; i < this.uvArray.Length; i++)
            {
                tmp[i] = new OBJ.UV(this.uvArray[i].Coordinates, verticalFlip, horizontalFlip);
            }
            this.uvArray = tmp;
        }

        public class Group
        {
            string grpName;
            List<Face> faceList;
            public string groupName
            {
                get { return grpName; }
                set { this.grpName = value; }
            }
            public List<Face> facesList
            {
                get { return this.faceList; }
            }
            public int numberFaces
            {
                get { return this.faceList.Count; }
            }
            internal Group()
            {
                grpName = "default";
                faceList = new List<Face>();
            }
            internal Group(string groupName)
            {
                grpName = groupName;
                faceList = new List<Face>();
            }
            internal void addFace(Face f)
            {
                faceList.Add(f);
            }
        }

        public class Vertex
        {
            float x, y, z;
            public float[] Coordinates
            {
                get { return new float[] { x, y, z }; }
            }
            public float X
            {
                get { return this.x; }
            }
            public float Y
            {
                get { return this.y; }
            }
            public float Z
            {
                get { return this.z; }
            }
            internal Vertex() { }
            internal Vertex(float x, float y, float z)
            {
                this.x = x;
                this.y = y;
                this.z = z;
            }
            internal Vertex(float[] coordinates)
            {
                this.x = coordinates[0];
                this.y = coordinates[1];
                this.z = coordinates[2];
            }
            public override string ToString()
            {
                return this.x.ToString("N7", CultureInfo.InvariantCulture) + " " + 
                    this.y.ToString("N7", CultureInfo.InvariantCulture) + " " +
                    this.z.ToString("N7", CultureInfo.InvariantCulture);
            }
        }

        public class UV
        {
            float x, y;
            public float[] Coordinates
            {
                get { return new float[] { x, y }; }
            }
            public float U
            {
                get { return this.x; }
            }
            public float V
            {
                get { return this.y; }
            }
            internal UV() { }
            internal UV(float x, float y)
            {
                this.x = x;
                this.y = y;
            }
            internal UV(float[] coordinates, bool verticalFlip)
            {
                this.x = coordinates[0];
                if (verticalFlip)
                {
                    this.y = 1f - coordinates[1];
                }
                else
                {
                    this.y = coordinates[1];
                }
            }
            internal UV(float[] coordinates, bool verticalFlip, bool horizontalFlip)
            {
                if (horizontalFlip)
                {
                    this.x = 1f - coordinates[0];
                }
                else
                {
                    this.x = coordinates[0];
                }
                if (verticalFlip)
                {
                    this.y = 1f - coordinates[1];
                }
                else
                {
                    this.y = coordinates[1];
                }
            }
            public override string ToString()
            {
                return this.x.ToString("N7", CultureInfo.InvariantCulture) + " " +
                    this.y.ToString("N7", CultureInfo.InvariantCulture);
            }
        }

        public class Normal
        {
            float x, y, z;
            public float[] Coordinates
            {
                get { return new float[] { x, y, z }; }
            }
            public float X
            {
                get { return this.x; }
            }
            public float Y
            {
                get { return this.y; }
            }
            public float Z
            {
                get { return this.z; }
            }
            internal Normal() { }
            internal Normal(float x, float y, float z)
            {
                this.x = x;
                this.y = y;
                this.z = z;
            }
            internal Normal(float[] coordinates)
            {
                this.x = coordinates[0];
                this.y = coordinates[1];
                this.z = coordinates[2];
            }
            public override string ToString()
            {
                return this.x.ToString("N7", CultureInfo.InvariantCulture) + " " +
                    this.y.ToString("N7", CultureInfo.InvariantCulture) + " " +
                    this.z.ToString("N7", CultureInfo.InvariantCulture);
            }
        }

        public class Face
        {
            internal int[] p1, p2, p3;

            internal int[][] facePoints
            {
                get
                {
                    int[][] tmp = new int[3][];
                    tmp[0] = new int[3];
                    tmp[1] = new int[3];
                    tmp[2] = new int[3];
                    for (int i = 0; i < 3; i++)
                    {
                        tmp[0][i] = p1[i];
                        tmp[1][i] = p2[i];
                        tmp[2][i] = p3[i];
                    }
                    return tmp;
                }
            }
            
            internal Face() { }
            internal Face(int[] point1, int[] point2, int[] point3)
            {
                p1 = new int[3] { 0, 0, 0 };
                p2 = new int[3] { 0, 0, 0 };
                p3 = new int[3] { 0, 0, 0 };
                for (int i = 0; i < 3; i++)
                {
                    if (i < point1.Length) p1[i] = point1[i];
                    if (i < point2.Length) p2[i] = point2[i];
                    if (i < point3.Length) p3[i] = point3[i];
                }
            }
            internal Face(int[] points, int offset)
            {
                p1 = new int[3] { 0, 0, 0 };
                p2 = new int[3] { 0, 0, 0 };
                p3 = new int[3] { 0, 0, 0 };
                for (int i = 0; i < 3; i++)
                {
                    p1[i] = points[0] + offset;
                    p2[i] = points[1] + offset;
                    p3[i] = points[2] + offset;
                }
            }
            public override string ToString()
            {
                return pointString(p1) + " " + pointString(p2) + " " + pointString(p3);
            }
            internal string pointString(int[] point)
            {
                string str = point[0].ToString();
                for (int i = 1; i < point.Length; i++)
                {
                    str += "/";
                    if (point[i] > 0) str += point[i].ToString();
                }
                return str;
            }
        }

        internal float[] addArrays(float[] v1, float[] v2)
        {
            float[] res = new float[v1.Length];
            for (int i = 0; i < v1.Length; i++)
            {
                res[i] = v1[i] + v2[i];
            }
            return res;
        }

        internal bool foundVert(int[] p, List<int[]> verts, out int vertInd, bool cleanModel)
        {
            for (int i = 0; i < verts.Count; i++)       //for each vertex
            {
                bool found = true;
                if (cleanModel)
                {
                    for (int k = 0; k < 3; k++)         //for position X, Y, Z
                    {
                        if (this.vertexArray[p[0] - 1].Coordinates[k] != this.vertexArray[verts[i][0] - 1].Coordinates[k])
                        {
                            found = false;
                            break;
                        }
                    }
                    if (found)
                    {
                        for (int k = 0; k < 2; k++)         //for UV X, Y
                        {
                            if (this.uvArray[p[1] - 1].Coordinates[k] != this.uvArray[verts[i][1] - 1].Coordinates[k])
                            {
                                found = false;
                                break;
                            }
                        }
                    }
                    if (found)
                    {
                        for (int k = 0; k < 3; k++)         //for normals X, Y, Z
                        {
                            if (this.normalArray[p[2] - 1].Coordinates[k] != this.normalArray[verts[i][2] - 1].Coordinates[k])
                            {
                                found = false;
                                break;
                            }
                        }
                    }
                }
                else
                {
                    for (int j = 0; j < verts[i].Length; j++)
                    {
                        if (p[j] != verts[i][j]) found = false;
                    }
                }

                if (found)
                {
                    vertInd = i;
                    return true;
                }
            }
            vertInd = -1;
            return false;
        }
    }
}
