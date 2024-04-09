using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace Xmods.DataLib
{
    public class WSO
    {
        int version;
        int numMeshes;
        MeshGroup[] meshes;
        int numBones;
        Bone[] bones;
        char[] softwareName;
        int unknown;

        public int numberMeshes
        {
            get
            {
                return this.numMeshes;
            }
        }
        public int numberBones
        {
            get
            {
                return numBones;
            }
        }

        public int meshIndex(string name)
        {
            return Array.IndexOf(this.meshNames, name);
        }

        public MeshGroup mesh(int Index)
        {
            if (Index < this.meshes.Length)
            {
                return meshes[Index];
            }
            else
            {
                return null;
            }
        }

        public MeshGroup mesh(string name)
        {
            int ind = Array.IndexOf(this.meshNames, name);
            if (ind > -1)
            {
                return this.meshes[ind];
            }
            else
            {
                return null;
            }
        }

        public string[] meshNames
        {
            get
            {
                string[] tmp = new string[this.numberMeshes];
                for (int i = 0; i < tmp.Length; i++)
                {
                    tmp[i] = this.meshes[i].meshName;
                }
                return tmp;
            }
        }

        public bool gotBase
        {
            get { return (Array.IndexOf(this.meshNames, "group_base") >= 0 || Array.IndexOf(this.meshNames, "group_0") >= 0); }
        }
        public bool gotMorphs
        {
            get { return (this.meshNames.Length > 1); }
        }
        public bool gotFat
        {
            get { return (Array.IndexOf(this.meshNames, "group_fat") >= 0); }
        }
        public bool gotFit
        {
            get { return (Array.IndexOf(this.meshNames, "group_fit") >= 0); }
        }
        public bool gotThin
        {
            get { return (Array.IndexOf(this.meshNames, "group_thin") >= 0); }
        }
        public bool gotSpecial
        {
            get { return (Array.IndexOf(this.meshNames, "group_special") >= 0); }
        }

        public int BaseIndex
        {
            get
            {
                int ind = 0;
                if (this.meshNames.Length > 1)
                {
                    ind = Array.IndexOf(this.meshNames, "group_base");
                }
                if (ind == -1)
                {
                    ind = Array.IndexOf(this.meshNames, "group_0");
                }
                if (ind > -1)
                {
                    return ind;
                }
                else
                {
                    return 0;
                }
            }
        }

        public MeshGroup Base
        {
            get 
            { 
                int ind = 0;
                if (this.meshNames.Length > 1)
                {
                    ind = Array.IndexOf(this.meshNames, "group_base");
                }
                if (ind == -1)
                {
                    ind = Array.IndexOf(this.meshNames, "group_0");
                }
                if (ind > -1)
                {
                    return this.meshes[ind];
                }
                else
                {
                    return null;
                }
            }
            set
            {
                int ind = 0;
                if (this.meshNames.Length > 1)
                {
                    ind = Array.IndexOf(this.meshNames, "group_base");
                }
                if (ind == -1)
                {
                    ind = Array.IndexOf(this.meshNames, "group_0");
                }
                if (ind > -1)
                {
                    this.meshes[ind] = value;
                }
                else
                {
                    MeshGroup[] tmp = new MeshGroup[this.meshes.Length + 1];
                    tmp[0] = value;
                    for (int i = 0; i < this.meshes.Length; i++)
                    {
                        tmp[i + 1] = this.meshes[i];
                    }
                    this.meshes = tmp;
                }
            }
        }
        public MeshGroup Fat
        {
            get
            {
                int ind = Array.IndexOf(this.meshNames, "group_fat");
                if (ind > -1)
                {
                    return this.meshes[ind];
                }
                else
                {
                    return null;
                }
            }
            set
            {
                int ind = Array.IndexOf(this.meshNames, "group_fat");
                if (ind > -1)
                {
                    this.meshes[ind] = value;
                }
                else
                {
                    MeshGroup[] tmp = new MeshGroup[this.meshes.Length + 1];
                    for (int i = 0; i < this.meshes.Length; i++)
                    {
                        tmp[i] = this.meshes[i];
                    }
                    tmp[this.meshes.Length] = value;
                    this.meshes = tmp;
                    this.numMeshes = this.meshes.Length;
                }
            }
        }
        public MeshGroup Fit
        {
            get
            {
                int ind = Array.IndexOf(this.meshNames, "group_fit");
                if (ind > -1)
                {
                    return this.meshes[ind];
                }
                else
                {
                    return null;
                }
            }
            set
            {
                int ind = Array.IndexOf(this.meshNames, "group_fit");
                if (ind > -1)
                {
                    this.meshes[ind] = value;
                }
                else
                {
                    MeshGroup[] tmp = new MeshGroup[this.meshes.Length + 1];
                    for (int i = 0; i < this.meshes.Length; i++)
                    {
                        tmp[i] = this.meshes[i];
                    }
                    tmp[this.meshes.Length] = value;
                    this.meshes = tmp;
                    this.numMeshes = this.meshes.Length;
                }
            }
        }
        public MeshGroup Thin
        {
            get
            {
                int ind = Array.IndexOf(this.meshNames, "group_thin");
                if (ind > -1)
                {
                    return this.meshes[ind];
                }
                else
                {
                    return null;
                }
            }
            set
            {
                int ind = Array.IndexOf(this.meshNames, "group_thin");
                if (ind > -1)
                {
                    this.meshes[ind] = value;
                }
                else
                {
                    MeshGroup[] tmp = new MeshGroup[this.meshes.Length + 1];
                    for (int i = 0; i < this.meshes.Length; i++)
                    {
                        tmp[i] = this.meshes[i];
                    }
                    tmp[this.meshes.Length] = value;
                    this.meshes = tmp;
                    this.numMeshes = this.meshes.Length;
                }
            }
        }
        public MeshGroup Special
        {
            get
            {
                int ind = Array.IndexOf(this.meshNames, "group_special");
                if (ind > -1)
                {
                    return this.meshes[ind];
                }
                else
                {
                    return null;
                }
            }
            set
            {
                int ind = Array.IndexOf(this.meshNames, "group_special");
                if (ind > -1)
                {
                    this.meshes[ind] = value;
                }
                else
                {
                    MeshGroup[] tmp = new MeshGroup[this.meshes.Length + 1];
                    for (int i = 0; i < this.meshes.Length; i++)
                    {
                        tmp[i] = this.meshes[i];
                    }
                    tmp[this.meshes.Length] = value;
                    this.meshes = tmp;
                    this.numMeshes = this.meshes.Length;
                }
            }
        }

        public string BoneName(int Index)
        {
            return bones[Index].ToString();
        }

        public string[] BoneNameList
        {
            get
            {
                string[] tmp = new string[this.numberBones];
                for (int i = 0; i < this.numberBones; i++)
                {
                    tmp[i] = this.bones[i].ToString();
                }
                return tmp;
            }
        }

        public Bone[] boneList
        {
            get
            {
                return this.bones;
            }
            set
            {
                Bone[] tmp = new Bone[value.Length];
                for (int i = 0; i < value.Length; i++)
                {
                    tmp[i] = new Bone(value[i]);
                }
                this.bones = tmp;
                this.numBones = this.bones.Length;
            }
        }
        public bool validBones(int meshGroupIndex, int vertexSequenceNumber)
        {
            int[] abones = this.meshes[meshGroupIndex].vertices[vertexSequenceNumber].boneAss;
            float[] wbones = this.meshes[meshGroupIndex].vertices[vertexSequenceNumber].weights;
            float totWeights = 0;
            for (int i = 0; i < 4; i++)
            {
                if (wbones[i] > 0f & (abones[i] < 0 | abones[i] >= this.numberBones)) return false;
                totWeights += wbones[i];
            }
            if (totWeights != 100.0) return false;
            return true;
        }

        public Bone getBone(int index)
        {
            return this.bones[index];
        }

        public void setBone(int index, Bone newBone)
        {
            this.bones[index] = new Bone(newBone);
        }

        internal int emptyBoneIndex
        {
            get
            {
                if (this.meshes.Length < 1) return -1;
                foreach (WSO.Vertex v in this.meshes[0].vertices)
                {
                    for (int i = 0; i < v.boneAss.Length; i++)
                    {
                        if (v.boneWeights[i] == 0) return v.boneAss[i];
                    }
                }
                return Array.IndexOf(this.BoneNameList, "b__ROOT_bind__");
            }
        }


        public WSO() { }

        public void ReadFile(BinaryReader br)
        {
            version = br.ReadInt32();
            if (version == 5)
            {
                int count = br.ReadInt32();
                softwareName = br.ReadChars(count);
                unknown = br.ReadInt32();
            }
            numMeshes = br.ReadInt32();
            meshes = new MeshGroup[numMeshes];
            for (int i = 0; i < numMeshes; i++)
            {
                meshes[i] = new MeshGroup(br);
            }
            numBones = br.ReadInt32();
            bones = new Bone[numBones];
            for (int i = 0; i < numBones; i++)
            {
                bones[i] = new Bone(br);
            }
        }

        public WSO(GEOM baseMesh, GEOM fatMorph, GEOM fitMorph, GEOM thinMorph, GEOM specialMorph, bool group0)
        {
            this.version = 4;
            int mcount = 1;
            if (fatMorph != null) mcount += 1;
            if (thinMorph != null) mcount += 1;
            if (fitMorph != null) mcount += 1;
            if (specialMorph != null) mcount += 1;
            this.numMeshes = mcount;
            this.meshes = new MeshGroup[mcount];
            if (group0)
            {
                meshes[0] = new MeshGroup(baseMesh, "group_0");
            }
            else
            {
                meshes[0] = new MeshGroup(baseMesh, "group_base");
            }
            mcount = 1;
            if (fatMorph != null)
            {
                meshes[mcount] = new MeshGroup(baseMesh, fatMorph, "group_fat");
                mcount += 1;
            }
            if (fitMorph != null) 
            {
                meshes[mcount] = new MeshGroup(baseMesh, thinMorph, "group_thin");
                mcount += 1;
            }
            if (thinMorph != null) 
            {
                meshes[mcount] = new MeshGroup(baseMesh, fitMorph, "group_fit");
                mcount += 1;
            }
            if (specialMorph != null) 
            {
                meshes[mcount] = new MeshGroup(baseMesh, specialMorph, "group_special");
                mcount += 1;
            }
            this.numBones = baseMesh.BoneHashList.Length;
            if (Array.IndexOf(baseMesh.BoneHashList, XmodsEnums.BoneHash.b__ROOT_bind__) < 0) this.numBones += 1;
            this.bones = new Bone[this.numBones];
            for (int i = 0; i < baseMesh.BoneHashList.Length; i++)
            {
                bones[i] = new Bone(baseMesh.BoneHashList[i]);
            }
            if (Array.IndexOf(baseMesh.BoneHashList, XmodsEnums.BoneHash.b__ROOT_bind__) < 0)
                bones[baseMesh.BoneHashList.Length] = new Bone((uint) XmodsEnums.BoneHash.b__ROOT_bind__);
        }

        public static WSO[] WSOfromOBJ(OBJ obj, WSO refMesh, bool smoothModel, bool cleanModel, bool flipUV, ProgressBar progressBar)
        {
            if (obj.uvArray.Length == 0)
            {
                DialogResult dr = MessageBox.Show("This OBJ mesh has no UV mapping. Continue with a blank UV map?",
                    "No UV mapping found", MessageBoxButtons.OKCancel);
                if (dr == DialogResult.Cancel) return null;
                obj.AddEmptyUV();
            }
            else if (flipUV)
            {
                OBJ.UV[] tmpUV = new OBJ.UV[obj.uvArray.Length];
                for (int i = 0; i < obj.uvArray.Length; i++)
                {
                    tmpUV[i] = new OBJ.UV(obj.uvArray[i].Coordinates, true);
                }
                obj.uvArray = tmpUV;
            }
            if (obj.normalArray.Length == 0 & !smoothModel)
            {
                DialogResult dr = MessageBox.Show("This OBJ mesh has no normals. Continue and calculate normals?",
                    "No normals found", MessageBoxButtons.OKCancel);
                if (dr == DialogResult.Cancel) return null;
                smoothModel = true;
            }
            if (smoothModel) obj.CalculateNormals(true);

            List<WSO> wso = new List<WSO>();
            int index = 0;
            wso.Add(new WSO());
            wso[index].version = 4;
            List<MeshGroup> meshList = new List<MeshGroup>();

            for (int i = 0; i < obj.numberGroups; i++)
            {
                if ((i > 0) &
                    (String.Compare(obj.groupArray[i].groupName, "group_base", true) == 0 ||
                     String.Compare(obj.groupArray[i].groupName, "group_0", true) == 0))
                {
                    wso[index].meshes = meshList.ToArray();
                    wso[index].numMeshes = meshList.Count;
                    wso[index].AutoBone(refMesh, false, true, 3, 2f, false, null);
                    index += 1;
                    wso.Add(new WSO());
                    wso[index].version = 4;
                    meshList.Clear();
                }

                List<int[]> verts = new List<int[]>();
                List<int[]> faces = new List<int[]>();
                foreach (OBJ.Face f in obj.groupArray[i].facesList)
                {
                    if (progressBar != null)
                    {
                        progressBar.PerformStep();
                    }
                    int j = 0;
                    int[] tmp = new int[3];
                    int vertInd = 0;
                    foreach (int[] p in f.facePoints)
                    {
                        if (!obj.foundVert(p, verts, out vertInd, cleanModel))
                        {
                            tmp[j] = verts.Count;
                            verts.Add(p);
                        }
                        else
                        {
                            tmp[j] = vertInd;
                        }
                        j++;
                    }
                    faces.Add(tmp);
                }
                meshList.Add(new MeshGroup(obj, obj.groupArray[i].groupName, verts, faces));

            }
            wso[index].meshes = meshList.ToArray();
            wso[index].numMeshes = meshList.Count;
            wso[index].AutoBone(refMesh, false, true, 3, 2f, false, null);
            if (progressBar != null)
            {
                progressBar.Visible = false;
            }
            return wso.ToArray();
        }   

        public void WriteFile(BinaryWriter bw)
        {
            bw.Write(version);
            if (version == 5)
            {
                bw.Write(softwareName.Length);
                bw.Write(softwareName);
                bw.Write(unknown);
            }
            bw.Write(numMeshes);
            for (int i = 0; i < numMeshes; i++)
            {
                meshes[i].Write(bw);
            }
            bw.Write(numBones);
            for (int i = 0; i < numBones; i++)
            {
                bones[i].Write(bw);
            }
        }

        public void ReplacePositions(WSO g)
        {
            if (this.Base.numberVertices != g.Base.numberVertices) throw new WSOException("Source number of vertices does not equal target number of vertices!");
            foreach (MeshGroup m in this.meshes)
            {
                for (int i = 0; i < m.numberVertices; i++)
                {
                    m.vertices[i].position = g.Base.vertices[i].position;
                }
            }
        }
        public void ReplaceNormals(WSO g)
        {
            if (this.Base.numberFacePoints != g.Base.numberFacePoints) throw new WSOException("Source number of faces does not equal target number of faces!");
            foreach (MeshGroup m in this.meshes)
            {
                for (int i = 0; i < m.numberFacePoints; i++)
                {
                    m.facePoints[i].normals = g.Base.facePoints[i].normals;
                }
            }
        }
        public void ReplaceUV(WSO g)
        {
            if (this.Base.numberFacePoints != g.Base.numberFacePoints) throw new WSOException("Source number of faces does not equal target number of faces!");
            foreach (MeshGroup m in this.meshes)
            {
                for (int i = 0; i < m.numberFacePoints; i++)
                {
                    m.facePoints[i].UVs = g.Base.facePoints[i].UVs;
                }
            }
        }
        public void ReplaceBones(WSO g)
        {
            if (this.Base.numberVertices != g.Base.numberVertices) throw new WSOException("Source number of vertices does not equal target number of vertices!");
            foreach (MeshGroup m in this.meshes)
            {
                for (int i = 0; i < m.numberVertices; i++)
                {
                    m.vertices[i].bones = g.Base.vertices[i].bones;
                    m.vertices[i].boneWeights = g.Base.vertices[i].boneWeights;
                }
            }
            this.boneList = g.boneList;
        }
        public void ReplaceTagvals(WSO g)
        {
            if (this.Base.numberVertices != g.Base.numberVertices) throw new WSOException("Source number of vertices does not equal target number of vertices!");
            foreach (MeshGroup m in this.meshes)
            {
                for (int i = 0; i < m.numberVertices; i++)
                {
                    m.vertices[i].tagVal = g.Base.vertices[i].tagVal;
                }
            }
        }
        public void ReplaceVertexIDs(WSO g)
        {
            if (this.Base.numberVertices != g.Base.numberVertices) throw new WSOException("Source number of vertices does not equal target number of vertices!");
            foreach (MeshGroup m in this.meshes)
            {
                for (int i = 0; i < m.numberVertices; i++)
                {
                    m.vertices[i].vertexID = g.Base.vertices[i].vertexID;
                }
            }
        }

        public void ReplacePositionsByID(WSO g)
        {
            int[] IDarray = new int[g.Base.numberVertices];
            int minID = 0, maxID = 0;
            for (int i = 0; i < g.Base.numberVertices; i++)
            {
                minID = Math.Max(minID, g.Base.vertices[i].vertexID);
                maxID = Math.Max(maxID, g.Base.vertices[i].vertexID);
                IDarray[i] = g.Base.vertices[i].vertexID;
            }
            if (maxID == 0 & minID == 0) throw new WSOException("No vertex IDs in source mesh!");

            int ind = 0, ind2 = 0;
            minID = 0; maxID = 0;
            for (int i = 0; i < this.Base.numberVertices; i++)
            {
                minID = Math.Max(minID, this.Base.vertices[i].vertexID);
                maxID = Math.Max(maxID, this.Base.vertices[i].vertexID);
                if (vertexIDsearch(this.Base.vertices[i].vertexID, IDarray, ind, out ind2))
                {
                    this.Base.vertices[i].position = g.Base.vertices[ind2].position;
                    ind = ind2;
                }
            }
            if (maxID == 0 & minID == 0) throw new WSOException("No vertex IDs in target mesh!");
        }
        public void ReplaceNormalsByID(WSO g)
        {
            throw new WSOException("Not yet implemented!");
        }
        public void ReplaceUVByID(WSO g)
        {
            throw new WSOException("Not yet implemented!");
        }
        public void ReplaceBonesByID(WSO g)
        {
            List<Bone> thisBones = new List<Bone>(this.boneList);
            int[] IDarray = new int[g.Base.numberVertices];
            int minID = 0, maxID = 0;
            for (int i = 0; i < g.Base.numberVertices; i++)
            {
                minID = Math.Max(minID, g.Base.vertices[i].vertexID);
                maxID = Math.Max(maxID, g.Base.vertices[i].vertexID);
                IDarray[i] = g.Base.vertices[i].vertexID;
            }
            if (maxID == 0 & minID == 0) throw new WSOException("No vertex IDs in source mesh!");

            int ind = 0, ind2 = 0;
            minID = 0; maxID = 0;
            for (int i = 0; i < this.Base.numberVertices; i++)
            {
                minID = Math.Max(minID, this.Base.vertices[i].vertexID);
                maxID = Math.Max(maxID, this.Base.vertices[i].vertexID);
                if (vertexIDsearch(this.Base.vertices[i].vertexID, IDarray, ind, out ind2))
                {
                    int[] tmp = g.Base.vertices[ind2].bones;
                    int[] tmp2 = new int[tmp.Length];
                    float[] tw = g.Base.vertices[ind2].boneWeights;
                    for (int j = 0; j < tmp.Length; j++)
                    {
                        if (tmp[j] < 0)
                        {
                            tmp2[j] = tmp[j];
                            continue;
                        }
                        tmp2[j] = thisBones.IndexOf(g.boneList[tmp[j]]);
                        if (tmp2[j] < 0)
                        {
                            if (tw[j] > 0f)
                            {
                                tmp2[j] = (byte)thisBones.Count;
                                thisBones.Add(g.boneList[tmp[j]]);
                            }
                            else
                            {
                                tmp2[j] = -1;
                            }
                        }
                    }
                    this.Base.vertices[i].bones = tmp2;
                    this.Base.vertices[i].boneWeights = g.Base.vertices[ind2].boneWeights;
                    ind = ind2;
                }
            }
            if (maxID == 0 & minID == 0) throw new WSOException("No vertex IDs in target mesh!");
            this.boneList = thisBones.ToArray();
        }
        public void ReplaceTagvalsByID(WSO g)
        {
            int[] IDarray = new int[g.Base.numberVertices];
            int minID = 0, maxID = 0;
            for (int i = 0; i < g.Base.numberVertices; i++)
            {
                minID = Math.Max(minID, g.Base.vertices[i].vertexID);
                maxID = Math.Max(maxID, g.Base.vertices[i].vertexID);
                IDarray[i] = g.Base.vertices[i].vertexID;
            }
            if (maxID == 0 & minID == 0) throw new WSOException("No vertex IDs in source mesh!");

            int ind = 0, ind2 = 0;
            minID = 0; maxID = 0;
            for (int i = 0; i < this.Base.numberVertices; i++)
            {
                minID = Math.Max(minID, this.Base.vertices[i].vertexID);
                maxID = Math.Max(maxID, this.Base.vertices[i].vertexID);
                if (vertexIDsearch(this.Base.vertices[i].vertexID, IDarray, ind, out ind2))
                {
                    this.Base.vertices[i].tagVal = g.Base.vertices[ind2].tagVal;
                    ind = ind2;
                }
            }
            if (maxID == 0 & minID == 0) throw new WSOException("No vertex IDs in target mesh!");
        }

        internal bool vertexIDsearch(int vertexID, int[] vertexIDarray, int startIndex, out int foundIndex)
        {
            int ind;

            ind = Array.IndexOf(vertexIDarray, vertexID, startIndex);
            if (ind > -1)
            {
                foundIndex = ind;
                return true;
            }
            else
            {
                ind = Array.IndexOf(vertexIDarray, vertexID);
                if (ind > -1)
                {
                    foundIndex = ind;
                    return true;
                }
                else
                {
                    foundIndex = startIndex;
                    return false;
                }
            }
        }

        public bool MorphMatch()
        {
            if (!this.Base.gotValidIDs) throw new WSOException("Mesh does not have valid ID numbering and morphs cannot be matched!");
            MeshGroup baseMesh = this.Base;
            MeshGroup[] newMorphs = new MeshGroup[this.numMeshes-1];
            for (int m = 0; m < this.numMeshes-1; m++)
            {
                MeshGroup mg = this.mesh(m + 1);
                vertexExtended[] OldMorphVerts = mg.getExtendedVertices();
                vertexExtended[] NewMorphVerts = new vertexExtended[baseMesh.numberVertices];
                int[] OldMorphVertIDs = new int[mg.numberVertices];
                for (int i = 0; i < mg.numberVertices; i++)
                {
                    OldMorphVertIDs[i] = mg.getVertex(i).vertexID;
                }
                int vertInd = 0, ind = 0;
                for (int i = 0; i < baseMesh.numberVertices; i++)          //search base mesh vertices for matching vert in morph
                {
                    ind = Array.IndexOf(OldMorphVertIDs, baseMesh.getVertex(i).vertexID, vertInd);
                    if (ind > -1)
                    {
                        NewMorphVerts[i] = OldMorphVerts[ind];
                        vertInd = ind;
                    }
                    else
                    {
                        ind = Array.IndexOf(OldMorphVertIDs, baseMesh.getVertex(i).vertexID);
                        if (ind > -1)
                        {
                            NewMorphVerts[i] = OldMorphVerts[ind];
                        }
                        else
                        {
                            NewMorphVerts[i] = new vertexExtended(baseMesh.getVertex(i));
                            for (int j = 0; j < baseMesh.numberFaces; j++)
                            {
                                if (baseMesh.getFacePoint(j).vertexIndex == i)
                                {
                                    NewMorphVerts[i].setNormals(baseMesh.getFacePoint(j).normals);
                                    NewMorphVerts[i].setUVs(baseMesh.getFacePoint(j).UVs);
                                }
                            }
                        }
                    }
                }
                newMorphs[m] = new MeshGroup(baseMesh, this.mesh(m + 1).meshName);
                bool success = newMorphs[m].applyExtendedVertices(NewMorphVerts, true, false);
                if (!success) return false;
            }

            for (int m = 1; m < this.numberMeshes; m++)
            {
                this.meshes[m] = newMorphs[m - 1];
            }
            return true;
        }

        public void AppendMesh(WSO meshToAppend)
        {
            if (!(meshToAppend.numberMeshes == this.numberMeshes)) throw new WSOException("Meshes do not have the same number of groups/morphs!");
            bool matched = true;
            for (int i = 0; i < this.numberMeshes; i++)
            {
                if (Array.IndexOf(meshToAppend.meshNames, this.meshes[i].meshName) < 0) matched = false;
            }
            if (!matched) throw new WSOException("Meshes do not have the same mesh groups");

            List<Bone> tmpBones = new List<Bone>();
            for (int b = 0; b < this.bones.Length; b++)
            {
                tmpBones.Add(this.bones[b]);                 //add bones from this mesh
            }
            for (int b = 0; b < meshToAppend.bones.Length; b++)
            {
                if (Array.IndexOf(this.BoneNameList, meshToAppend.bones[b].Name) < 0)
                {
                    tmpBones.Add(meshToAppend.bones[b]);         //add bones from second mesh
                }
            }
            Bone[] newbonearray = tmpBones.ToArray();
            this.bones = newbonearray;

            for (int i = 0; i < this.numberMeshes; i++)
            {
                int ind = Array.IndexOf(meshToAppend.meshNames, this.meshes[i].meshName);
                this.meshes[i].AppendMeshGroup(meshToAppend.meshes[ind], meshToAppend.BoneNameList, this.BoneNameList);
            }
        }

        public void AutoMorph(WSO refMesh, ProgressBar progressBar, bool interpolate, int interpolationPoints, 
                              bool restrictToFace, bool doFat, bool doThin, bool doFit, bool doSpecial, float weightingFactor)
        {
            int[][] refPoints = new int[this.Base.numberVertices][];
            float[][] valueWeights = new float[this.Base.numberVertices][];
            Vector3[] refVerts = new Vector3[refMesh.Base.numberVertices];
            for (int i = 0; i < refVerts.Length; i++)
            {
                refVerts[i] = new Vector3(refMesh.Base.getVertex(i).position);
            }
            int[][] refFaces = new int[refMesh.Base.numberFaces][];
            for (int i = 0; i < refMesh.Base.numberFaces; i++)
            {
                refFaces[i] = new int[] { (int)refMesh.Base.getFacePoint(i * 3).vertexIndex,
                    (int)refMesh.Base.getFacePoint((i * 3) + 1).vertexIndex,
                    (int)refMesh.Base.getFacePoint((i * 3) + 2).vertexIndex };
            }

            int stepit = 0;
            for (int i = 0; i < this.Base.numberVertices; i++)
            {
                stepit++;
                if (stepit >= 100)
                {
                    if (progressBar != null) progressBar.PerformStep();
                    stepit = 0;
                }
                Vector3 pos = new Vector3(this.Base.getVertex(i).position);
                refPoints[i] = pos.GetReferenceMeshPoints(refVerts, refFaces, interpolate, restrictToFace, interpolationPoints);
                Vector3[] refArray = new Vector3[refPoints[i].Length];
                for (int j = 0; j < refPoints[i].Length; j++)
                {
                    refArray[j] = new Vector3(refMesh.Base.getVertex(refPoints[i][j]).position);
                }
                valueWeights[i] = pos.GetInterpolationWeights(refArray, weightingFactor);
            }
            if (doFat)
            {
                this.Fat = MakeWSOautoMorph(this.Base, refMesh.Base, refMesh.Fat, refPoints, valueWeights, WSO.MeshName.group_fat);
                if (progressBar != null) progressBar.PerformStep();
            }
            if (doThin)
            {
                this.Thin = MakeWSOautoMorph(this.Base, refMesh.Base, refMesh.Thin, refPoints, valueWeights, WSO.MeshName.group_thin);
                if (progressBar != null) progressBar.PerformStep();
            }
            if (doFit)
            {
                this.Fit = MakeWSOautoMorph(this.Base, refMesh.Base, refMesh.Fit, refPoints, valueWeights, WSO.MeshName.group_fit);
                if (progressBar != null) progressBar.PerformStep();
            }
            if (doSpecial)
            {
                this.Special = MakeWSOautoMorph(this.Base, refMesh.Base, refMesh.Special, refPoints, valueWeights, WSO.MeshName.group_special);
                if (progressBar != null) progressBar.PerformStep();
            }
        }

        internal WSO.MeshGroup MakeWSOautoMorph(WSO.MeshGroup baseMesh, WSO.MeshGroup refBase, WSO.MeshGroup refMorph,
                                                int[][] refPoints, float[][] valueWeights, WSO.MeshName name)
        {
            WSO.vertexExtended[] refBaseVerts = refBase.getExtendedVertices();
            WSO.vertexExtended[] refMorphVerts = refMorph.getExtendedVertices();

            WSO.MeshGroup newMorph = new WSO.MeshGroup(baseMesh, name);
            Vector3[] deltaNorms = new Vector3[newMorph.numberVertices];
            //WSO.vertexExtended[] newVerts = newMorph.getExtendedVertices();

            for (int i = 0; i < newMorph.numberVertices; i++)
            {
                Vector3 newpos = new Vector3(newMorph.getVertex(i).position);
                //Vector3 newnorm = new Vector3(newVerts[i].getNormals());
                for (int j = 0; j < refPoints[i].Length; j++)
                {
                    newpos += valueWeights[i][j] * (new Vector3(refMorph.getVertex(refPoints[i][j]).position) - new Vector3(refBase.getVertex(refPoints[i][j]).position));
                    //newnorm += valueWeights[i][j] * (new Vector3(refMorphVerts[refPoints[i][j]].getNormals()) - new Vector3(refBaseVerts[refPoints[i][j]].getNormals()));
                    deltaNorms[i] += valueWeights[i][j] * (new Vector3(refMorphVerts[refPoints[i][j]].getNormals()) - new Vector3(refBaseVerts[refPoints[i][j]].getNormals()));
                }
                newMorph.setVertexPosition(i, newpos.Coordinates);
                //newVerts[i].setPosition(newpos.Array);
                //newVerts[i].setNormals(newnorm.Array);
            }
            for (int i = 0; i < newMorph.numberFacePoints; i++)
            {
                Vector3 newnorm = new Vector3(newMorph.getFacePoint(i).normals) + deltaNorms[newMorph.getFacePoint(i).vertexIndex];
                newMorph.setFacePointNormal(i, newnorm.Coordinates);
                //newVerts[i].setNormals(newnorm.Array);
            }

            //newMorph.applyExtendedVertices(newVerts, true, false);
            return newMorph;
        }

        public void AutoBone(WSO refMesh, bool unassignedVerticesOnly, bool interpolate, int interpolationPoints, float weightingFactor, bool restrictToFace, ProgressBar progress)
        {
            //Merge lists of bones
            string[] refBoneNameList = refMesh.BoneNameList;
            WSO.Bone[] newBonesList;
            string[] newBoneNameList;
            int emptyBone;
            if (unassignedVerticesOnly)
            {
                List<WSO.Bone> tmpBones = new List<WSO.Bone>(this.boneList);
                List<string> tmpBoneNames = new List<string>(this.BoneNameList);
                for (int b = 0; b < refMesh.BoneNameList.Length; b++)
                {
                    if (Array.IndexOf(this.BoneNameList, refBoneNameList[b]) < 0)
                    {
                        tmpBones.Add(refMesh.boneList[b]);         //add bones from reference mesh
                        tmpBoneNames.Add(refMesh.BoneName(b));
                    }
                }
                newBoneNameList = tmpBoneNames.ToArray();
                newBonesList = tmpBones.ToArray();
                emptyBone = this.emptyBoneIndex;
            }
            else
            {
                newBoneNameList = refMesh.BoneNameList;
                newBonesList = refMesh.boneList;
                emptyBone = refMesh.emptyBoneIndex;
            }
            this.boneList = newBonesList;

            Vector3[] refVerts = new Vector3[refMesh.Base.numberVertices];
            for (int i = 0; i < refVerts.Length; i++)
            {
                refVerts[i] = new Vector3(refMesh.Base.getVertex(i).position);
            }
            int[][] refFaces = new int[refMesh.Base.numberFaces][];
            for (int i = 0; i < refMesh.Base.numberFaces; i++)
            {
                refFaces[i] = new int[] { (int)refMesh.Base.getFacePoint(i * 3).vertexIndex,
                    (int)refMesh.Base.getFacePoint((i * 3) + 1).vertexIndex,
                    (int)refMesh.Base.getFacePoint((i * 3) + 2).vertexIndex };
            }

            int stepit = 0;
            int baseIndex = this.BaseIndex;
            for (int i = 0; i < this.Base.numberVertices; i++)
            {
                stepit++;
                if (stepit >= 100)
                {
                    if (progress != null) progress.PerformStep();
                    stepit = 0;
                }

                if (unassignedVerticesOnly & this.validBones(baseIndex, i)) continue; //skip assigned verts

                Vector3 pos = new Vector3(this.Base.getVertex(i).position);
                int[] refPoints;
                refPoints = pos.GetReferenceMeshPoints(refVerts, refFaces, interpolate, restrictToFace, interpolationPoints);
                Vector3[] refArray = new Vector3[refPoints.Length];
                for (int j = 0; j < refPoints.Length; j++)
                {
                    refArray[j] = new Vector3(refMesh.Base.getVertex(refPoints[j]).position);
                }
                float[] valueWeights = pos.GetInterpolationWeights(refArray, weightingFactor);

                List<int> newBones = new List<int>();
                List<float> newWeights = new List<float>();

                for (int j = 0; j < refPoints.Length; j++)
                {
                    int[] refBones = refMesh.Base.getVertex(refPoints[j]).bones;
                    float[] refWeights = refMesh.Base.getVertex(refPoints[j]).boneWeights;
                    for (int k = 0; k < refBones.Length; k++)
                    {
                        if (refWeights[k] > 0.0f & refBones[k] < refBoneNameList.Length & refBones[k] >= 0)
                        {
                            int ind = newBones.IndexOf(refBones[k]);
                            if (ind >= 0)
                            {
                                newWeights[ind] += valueWeights[j] * refWeights[k];
                            }
                            else
                            {
                                newBones.Add(refBones[k]);
                                newWeights.Add(valueWeights[j] * refWeights[k]);
                            }
                        }
                    }
                }
                SortBones(ref newBones, ref newWeights);
                for (int j = newBones.Count; j < 4; j++)
                {
                    newBones.Add(emptyBone);
                    newWeights.Add(0f);
                }
                for (int j = 0; j < 4; j++)
                {
                    if (newBones[j] < refBoneNameList.Length & newBones[j] >= 0 & newWeights[j] > 0f)
                    {
                        newBones[j] = Array.IndexOf(newBoneNameList, refBoneNameList[newBones[j]]);
                    }
                    else
                    {
                        newBones[j] = emptyBone;
                        newWeights[j] = 0f;
                    }
                }
                for (int j = 0; j < this.numberMeshes; j++)
                {
                    this.mesh(j).getVertex(i).bones = newBones.GetRange(0, 4).ToArray();
                    this.mesh(j).getVertex(i).boneWeights = newWeights.GetRange(0, 4).ToArray();
                }
            }
        }

        private void SortBones(ref List<int> newBones, ref List<float> newWeights)
        {
            for (int i = newBones.Count - 1; i > 0; i--)
            {
                for (int j = 0; j < i; j++)
                {
                    if (newWeights[j] < newWeights[j + 1])
                    {
                        int tb = newBones[j];
                        newBones[j] = newBones[j + 1];
                        newBones[j + 1] = tb;
                        float tw = newWeights[j];
                        newWeights[j] = newWeights[j + 1];
                        newWeights[j + 1] = tw;
                    }
                }
            }
        }

        public int BoneScan()
        {
            int maxbone = 0;
            bool badbone = false;
            bool toomanybones = false;
            for (int m = 0; m < this.numMeshes; m++)
            {
                for (int i = 0; i < this.mesh(m).numberVertices; i++)
                {
                    int[] abones = this.mesh(m).getVertex(i).bones;
                    float[] wbones = this.mesh(m).getVertex(i).boneWeights;
                    for (int j = 0; j < 4; j++)
                    {
                        if (wbones[j] > 0f & abones[j] > maxbone) maxbone = abones[j];
                    }
                    if (! validBones(m, i)) badbone = true;
                }
                if (this.mesh(m).numberBonesUsed > 60) toomanybones = true;
            }
            int res = 0;
            if (maxbone > this.numBones) res += (int)XmodsEnums.BoneScanResults.MissingBones;
            if (badbone) res += (int)XmodsEnums.BoneScanResults.BadBoneWeights;
            if (toomanybones) res += (int)XmodsEnums.BoneScanResults.TooManyBones;
            return res;
        }

        public void FixBoneWeights()
        {
            for (int m = 0; m < this.numMeshes; m++)
            {
                for (int i = 0; i < this.mesh(m).numberVertices; i++)
                {
                    float[] wbones = this.mesh(m).getVertex(i).boneWeights;
                    int totweight = 0;
                    for (int j = 0; j < 4; j++)
                    {
                        wbones[j] = (float)Math.Round(wbones[j]);
                        totweight += (int)wbones[j];
                    }
                    for (int j = 0; j < 4; j++)
                    {
                        if (wbones[j] >= totweight - 100f)
                        {
                            wbones[0] += 100f - totweight;
                            break;
                        }
                    }
                    this.mesh(m).getVertex(i).boneWeights = wbones;
                }
            }
        }

        public bool SeamFixer(string ageGender, bool fixBones, bool fixNormals)
        {
            List<string> thisBones = new List<string>(this.BoneNameList);
            byte emptyIndex = (byte)thisBones.IndexOf("b__ROOT_bind__");
            if (emptyIndex < 0) emptyIndex = (byte)this.emptyBoneIndex;
            Seams refSeams = new Seams(ageGender);
            if (refSeams.position == null)
            {
                MessageBox.Show("Invalid age/gender selected: " + ageGender);
                return false;
            }

            for (int i = 0; i < this.Base.numberVertices; i++)
            {
                for (int j = 0; j < refSeams.position.Length; j++)
                {
                    if (new Vector3(this.Base.vertices[i].position) == refSeams.position[j])
                    {
                        if (fixBones)
                        {
                            string[] boneNames = refSeams.boneName[j];
                            int[] index = new int[boneNames.Length];
                            float[] bw = refSeams.boneWeight[j];
                            for (int k = 0; k < boneNames.Length; k++)
                            {
                                index[k] = thisBones.IndexOf(boneNames[k]);
                                if (index[k] < 0)
                                {
                                    if (bw[k] > 0f)
                                    {
                                        index[k] = thisBones.Count;
                                        thisBones.Add(boneNames[k]);
                                    }
                                    else
                                    {
                                        index[k] = emptyIndex;
                                    }
                                }
                            }
                            float[] bwTSRW = new float[4];
                            for (int k = 0; k < 4; k++)
                            {
                                bwTSRW[k] = bw[k] * 100f;
                            }
                            foreach (MeshGroup m in this.meshes)
                            {
                                m.vertices[i].boneAss = index;
                                m.vertices[i].weights = bwTSRW;
                            }
                        }
                        if (fixNormals)
                        {
                            for (int k = 0; k < this.Base.numberFacePoints; k++)
                            {
                                if (this.Base.facePoints[k].vertexIndex == i)
                                {
                                    this.Base.facePoints[k].normals = refSeams.normal[j].Coordinates;
                                }
                            }
                        }
                        break;
                    }
                }
            }
            if (fixBones)
            {
                this.bones = new Bone[thisBones.Count];
                this.numBones = thisBones.Count;
                for (int i = 0; i < thisBones.Count; i++)
                {
                    this.bones[i] = new Bone(thisBones[i]);
                }
            }
            return true;
        }

        public void AutoUV(WSO refMesh, bool unassignedOnly, float weightingFactor, ProgressBar progress)
        {
            Vector3[] refVerts = new Vector3[refMesh.Base.numberVertices];
            for (int i = 0; i < refVerts.Length; i++)
            {
                refVerts[i] = new Vector3(refMesh.Base.getVertex(i).position);
            }
            vertexExtended[] refVertsEx = refMesh.Base.getExtendedVertices();
            int[][] refFaces = new int[refMesh.Base.numberFaces][];
            for (int i = 0; i < refMesh.Base.numberFaces; i++)
            {
                refFaces[i] = new int[] { (int)refMesh.Base.getFacePoint(i * 3).vertexIndex,
                    (int)refMesh.Base.getFacePoint((i * 3) + 1).vertexIndex,
                    (int)refMesh.Base.getFacePoint((i * 3) + 2).vertexIndex };
            }
            int[][] refFaceRefs = new int[refMesh.Base.numberVertices][];
            for (int i = 0; i < refVerts.Length; i++)
            {
                List<int> tmp = new List<int>();
                for (int j = 0; j < refFaces.Length; j++)
                {
                    for (int k = 0; k < 3; k++)
                    {
                        if (i == refFaces[j][k]) tmp.Add(j);
                    }
                }
                refFaceRefs[i] = tmp.ToArray();
            }

            int stepit = 0;
            int baseIndex = this.BaseIndex;
            for (int i = 0; i < this.Base.numberFacePoints; i++)
            {
                stepit++;
                if (stepit >= 100)
                {
                    if (progress != null) progress.PerformStep();
                    stepit = 0;
                }

                if (unassignedOnly & (this.Base.facePoints[i].UVs[0] > 0f | this.Base.facePoints[i].UVs[1] > 0f)) continue; //skip mapped facepoints

                Vector3 pos = new Vector3(this.Base.vertices[this.Base.facePoints[i].vertexIndex].position);
                List<Triangle> currentVertFaces = new List<Triangle>();
                for (int j = 0; j < this.Base.numberFaces; j++)
                {
                    if (this.Base.getFacePoint(j * 3).vertexIndex == this.Base.facePoints[i].vertexIndex |
                        this.Base.getFacePoint((j * 3) + 1).vertexIndex == this.Base.facePoints[i].vertexIndex |
                        this.Base.getFacePoint((j * 3) + 2).vertexIndex == this.Base.facePoints[i].vertexIndex)
                    {
                        currentVertFaces.Add( new Triangle(this.Base.vertices[this.Base.facePoints[j * 3].vertexIndex].position,
                            this.Base.vertices[this.Base.facePoints[(j * 3) + 1].vertexIndex].position, 
                            this.Base.vertices[this.Base.facePoints[(j * 3) + 2].vertexIndex].position) );
                    }
                }

                int[] refPoints = pos.GetFaceReferenceMeshPoints(refVerts, refFaces, refFaceRefs, currentVertFaces.ToArray(), 5);
                Vector3[] refArray = new Vector3[refPoints.Length];
                for (int j = 0; j < refPoints.Length; j++)
                {
                    refArray[j] = new Vector3(refMesh.Base.getVertex(refPoints[j]).position);
                }
                float[] valueWeights = pos.GetInterpolationWeights(refArray, weightingFactor);

                float[] newUV = new float[2];

                for (int j = 0; j < refPoints.Length; j++)
                {
                    float[] refUV = refVertsEx[refPoints[j]].getUVs();
                    for (int k = 0; k < refUV.Length; k++)
                    {
                        newUV[k] += valueWeights[j] * refUV[k];
                    }
                }
                for (int j = 0; j < this.numberMeshes; j++)
                {
                    this.mesh(j).facePoints[i].UVs = newUV;
                }
            }
        }


        public class MeshGroup
        {
            int numVertices;
            internal Vertex[] vertices;
            int numFaces;
            internal FacePoint[] facePoints;
            int numGeostates;
            byte nameLen;
            char[] meshNameStr;

            public int numberVertices
            {
                get
                {
                    return this.numVertices;
                }
                set
                {
                    this.numVertices = value;
                }
            }
            public int numberFaces
            {
                get
                {
                    return numFaces;
                }
                set
                {
                    this.numFaces = value;
                }
            }
            public int numberFacePoints
            {
                get
                {
                    return this.numFaces * 3;
                }
            }
            public int numberBonesUsed
            {
                get
                {
                    return this.bonesUsedIndexList.Length;
                }
            }
            public int[] bonesUsedIndexList
            {
                get
                {
                    List<int> b = new List<int>();
                    for (int i = 0; i < this.numVertices; i++)
                    {
                        int[] tmpb = this.vertices[i].boneAss;
                        float[] tmpw = this.vertices[i].weights;
                        for (int j = 0; j < 4; j++)
                        {
                            if (tmpw[j] > 0f & b.IndexOf(tmpb[j]) < 0)
                            {
                                b.Add(tmpb[j]);
                            }
                        }
                    }
                    return b.ToArray();
                }
            }
            public int[] vertexIDrange
            {
                get
                {
                    int low = Int32.MaxValue, high = 0;
                    foreach (Vertex v in this.vertices)
                    {
                        low = Math.Min(low, v.vertID);
                        high = Math.Max(high, v.vertID);
                    }
                    return new int[] { low, high };
                }
            }
            public string meshName
            {
                get
                {
                    return new string(this.meshNameStr);
                }
                set
                {
                    this.meshNameStr = value.ToCharArray();
                    this.nameLen = (byte)this.meshNameStr.Length;
                }
            }
            public int numberGEOstates
            {
                get
                {
                    return this.numGeostates;
                }
                set
                {
                    this.numGeostates = value;
                }
            }

            public bool gotValidIDs
            {
                get
                {
                    for (int i = 0; i < this.numberVertices; i++)
                    {
                        if (this.vertices[i].vertID < 0) return false;
                    }
                    int[] range = this.vertexIDrange;
                    return ((range[1] - range[0]) > 0);
                }
            }

            public Vertex getVertex(int Index)
            {
                return vertices[Index];
            }
            public void setVertexPosition(int Index, float[] position)
            {
                vertices[Index].position = new float[] { position[0], position[1], position[2] };
            }
            public FacePoint getFacePoint(int Index)
            {
                return facePoints[Index];
            }
            public void setFacePointNormal(int Index, float[] normal)
            {
                facePoints[Index].normals = new float[] { normal[0], normal[1], normal[2] };
            }

            internal MeshGroup() { }

            internal MeshGroup(int numVerts, Vertex[] verts, int numFace, FacePoint[] facePnts, int numGeos, string meshName)
            {
                this.numVertices = numVerts;
                this.vertices = verts;
                this.numFaces = numFace;
                this.facePoints = facePnts;
                this.numGeostates = numGeos;
                this.meshNameStr = meshName.ToCharArray();
                this.nameLen = (byte) this.meshNameStr.Length;
            }

            internal MeshGroup(BinaryReader br)
            {
                numVertices = br.ReadInt32();
                vertices = new Vertex[numVertices];
                for (int i = 0; i < numVertices; i++)
                {
                    vertices[i] = new Vertex(br);
                }
                numFaces = br.ReadInt32();
                facePoints = new FacePoint[numFaces * 3];
                for (int i = 0; i < numFaces * 3; i++)
                {
                    facePoints[i] = new FacePoint(br);
                }
                numGeostates = br.ReadInt32();
                nameLen = br.ReadByte();
                meshNameStr = new char[nameLen];
                meshNameStr = br.ReadChars(nameLen);
            }

            public MeshGroup(MeshGroup mesh, WSO.MeshName name) : this(mesh, Enum.GetName(typeof(WSO.MeshName), name))
            {
            }

            public MeshGroup(MeshGroup mesh, string meshName)
            {
                this.numVertices = mesh.numberVertices;
                this.vertices = new Vertex[this.numVertices];
                for (int i = 0; i < this.numVertices; i++)
                {
                    this.vertices[i] = new Vertex(mesh.vertices[i]);
                }
                this.numFaces = mesh.numFaces;
                this.facePoints = new FacePoint[this.numberFacePoints];
                for (int i = 0; i < this.numberFacePoints; i++)
                {
                    this.facePoints[i] = new FacePoint(mesh.facePoints[i]);
                }
                this.numGeostates = mesh.numGeostates;
                this.meshNameStr = meshName.ToCharArray();
                this.nameLen = (byte)this.meshNameStr.Length;
            }

            public MeshGroup(GEOM baseMesh, string meshName)
            {
                if (!baseMesh.isValid | !baseMesh.isBase) throw new WSOException("Input base mesh is not valid!");
                this.numVertices = baseMesh.numberVertices;
                vertices = new Vertex[this.numVertices];
                int rootbone = Array.IndexOf(baseMesh.BoneHashList, (uint)XmodsEnums.BoneHash.b__ROOT_bind__);
                if (rootbone < 0) rootbone = baseMesh.BoneHashList.Length;
                for (int i = 0; i < this.numVertices; i++)
                {
                    uint tmpTags = 0;
                    if (baseMesh.hasTags) tmpTags = baseMesh.getTagval(i);
                    byte[] Bones = baseMesh.getBones(i);
                    int[] tmpBones = new int[Bones.Length];
                    float[] boneWeights = baseMesh.getBoneWeights(i);
                    float[] tmpWeights = new float[boneWeights.Length];
                    
                    for (int j = 0; j < Bones.Length; j++)
                    {
                        tmpBones[j] = (int)Bones[j];
                       // tmpWeights[j] = (float)Math.Round(boneWeights[j] * 100f, 0);
                        tmpWeights[j] = boneWeights[j] * 100f;
                       // if (Math.Truncate(tmpWeights[j]) == 0)
                        if (tmpWeights[j] < .5f)
                        {
                            tmpBones[j] = rootbone;
                            tmpWeights[j] = 0f;
                        }
                    }
                    float totweights = 0;
                    for (int j = 0; j < Bones.Length; j++)
                    {
                        totweights += tmpWeights[j];
                    }
                    if (! IsEqual(totweights, 100f))
                    {
                        tmpWeights[0] += 100f - totweights;
                    }
                    int tmpID = 0;
                    if (baseMesh.hasVertexIDs) tmpID = baseMesh.getVertexID(i);
                    vertices[i] = new Vertex(baseMesh.getPosition(i), tmpID, tmpTags, tmpBones, tmpWeights);
                }
                this.numFaces = baseMesh.numberFaces;
                this.facePoints = new FacePoint[this.numFaces * 3];
                int f = 0;
                for (int i = 0; i < numFaces; i++)
                {
                    int[] tmpFace = baseMesh.getFace(i);
                    for (int j = 0; j < 3; j++)
                    {
                        facePoints[f + j] = new FacePoint(tmpFace[j], baseMesh.getNormal(tmpFace[j]), baseMesh.getUV(tmpFace[j], 0));
                    }
                    f += 3;
                }
                this.numGeostates = 0;
                this.meshNameStr = meshName.ToCharArray();
                this.nameLen = (byte)this.meshNameStr.Length;
            }

            public MeshGroup(GEOM baseMesh, GEOM morphMesh, string meshName)
            {
                if (!baseMesh.isValid | !baseMesh.isBase) throw new WSOException("Input base mesh is not valid!");
                if (!morphMesh.isValid | !morphMesh.isMorph) throw new WSOException("Input morph mesh " + meshName + " is not valid!");
                this.numVertices = baseMesh.numberVertices;
                vertices = new Vertex[this.numVertices];
                int rootbone = Array.IndexOf(baseMesh.BoneHashList, (uint)XmodsEnums.BoneHash.b__ROOT_bind__);
                if (rootbone < 0) rootbone = baseMesh.BoneHashList.Length;
                for (int i = 0; i < this.numVertices; i++)
                {
                    uint tmpTags = 0;
                    if (baseMesh.hasTags) tmpTags = baseMesh.getTagval(i);
                    byte[] Bones = baseMesh.getBones(i);
                    int[] tmpBones = new int[Bones.Length];
                    float[] boneWeights = baseMesh.getBoneWeights(i);
                    float[] tmpWeights = new float[boneWeights.Length];
                    for (int j = 0; j < Bones.Length; j++)
                    {
                        tmpBones[j] = (int)Bones[j];
                        tmpWeights[j] = boneWeights[j] * 100f;
                        if (tmpWeights[j] < .5f)
                        {
                            tmpBones[j] = rootbone;
                            tmpWeights[j] = 0f;
                        }
                    }
                    float totweights = 0;
                    for (int j = 0; j < Bones.Length; j++)
                    {
                        totweights += tmpWeights[j];
                    }
                    if (! IsEqual(totweights, 100f))
                    {
                        tmpWeights[0] += 100 - totweights;
                    }
                    int tmpID = 0;
                    if (baseMesh.hasVertexIDs) tmpID = baseMesh.getVertexID(i);
                    float [] basePos = baseMesh.getPosition(i);
                    float [] morphDelta = morphMesh.getPosition(i);
                    float [] morphPos = new float[basePos.Length];
                    for (int j = 0; j < basePos.Length; j++)
                    {
                        morphPos[j] = basePos[j] + morphDelta[j];
                    }
                    vertices[i] = new Vertex(morphPos, tmpID, tmpTags, tmpBones, tmpWeights);
                }
                this.numFaces = baseMesh.numberFaces;
                this.facePoints = new FacePoint[this.numFaces * 3];
                int f = 0;
                for (int i = 0; i < numFaces; i++)
                {
                    int[] tmpFace = baseMesh.getFace(i);
                    for (int j = 0; j < 3; j++)
                    {
                        float[] baseNorm = baseMesh.getNormal(tmpFace[j]);
                        float[] morphDelta = morphMesh.getNormal(tmpFace[j]);
                        float[] morphNorm = new float[baseNorm.Length];
                        for (int k = 0; k < baseNorm.Length; k++)
                        {
                            morphNorm[k] = baseNorm[k] + morphDelta[k];
                        }
                        facePoints[f + j] = new FacePoint(tmpFace[j], morphNorm, baseMesh.getUV(tmpFace[j], 0));
                    }
                    f += 3;
                }
                this.numGeostates = 0;
                this.meshNameStr = meshName.ToCharArray();
                this.nameLen = (byte)this.meshNameStr.Length;
            }

            public MeshGroup(OBJ obj, string meshName, List<int[]> verts, List<int[]> faces)
            {
                this.numVertices = verts.Count;
                vertices = new Vertex[this.numVertices];
                for (int i = 0; i < verts.Count; i++)
                {
                    uint tmpTags = 0;
                    int[] tmpBones = new int[4];
                    float[] tmpWeights = new float[4];
                    int tmpID = 0;
                    vertices[i] = new Vertex(obj.vertexArray[verts[i][0] - 1].Coordinates, tmpID, tmpTags, tmpBones, tmpWeights);
                }
                this.numFaces = faces.Count;
                this.facePoints = new FacePoint[this.numFaces * 3];
                int f = 0;
                for (int i = 0; i < faces.Count; i++)
                {
                    int[] tmpFace = faces[i];
                    for (int j = 0; j < 3; j++)
                    {
                        facePoints[f + j] = new FacePoint(tmpFace[j], obj.normalArray[verts[tmpFace[j]][2] - 1].Coordinates,
                            obj.uvArray[verts[tmpFace[j]][1] - 1].Coordinates, true);
                    }
                    f += 3;
                }
                this.numGeostates = 0;
                this.meshNameStr = meshName.ToCharArray();
                this.nameLen = (byte)this.meshNameStr.Length;
            }

            public void AppendMeshGroup(MeshGroup meshGroupToAppend, string[] oldBoneArray, string[] newBoneArray)
            {
                Vertex[] newVerts = new Vertex[this.numVertices + meshGroupToAppend.numVertices];
                int rootbone = Array.IndexOf(newBoneArray, Enum.GetName(typeof(XmodsEnums.BoneHash), XmodsEnums.BoneHash.b__ROOT_bind__));
                for (int i = 0; i < this.numVertices; i++)
                {
                    newVerts[i] = this.vertices[i];
                }
                for (int i = 0; i < meshGroupToAppend.numVertices; i++)
                {
                    int[] oldBones = meshGroupToAppend.vertices[i].bones;
                    int[] tmpBones = new int[oldBones.Length];
                    for (int b = 0; b < oldBones.Length; b++)
                    {
                        if (oldBones[b] >= 0 & oldBones[b] < oldBoneArray.Length)
                        {
                            tmpBones[b] = Array.IndexOf(newBoneArray, oldBoneArray[oldBones[b]]);
                        }
                        else
                        {
                            tmpBones[b] = rootbone;
                        }
                    }
                    newVerts[i + this.numVertices] = new Vertex(meshGroupToAppend.vertices[i].position,
                        meshGroupToAppend.vertices[i].vertID, meshGroupToAppend.vertices[i].tagV,
                        tmpBones, meshGroupToAppend.vertices[i].weights);
                }
                this.vertices = newVerts;

                FacePoint[] newFacepnts = new FacePoint[this.numberFacePoints + meshGroupToAppend.numberFacePoints];
                for (int i = 0; i < this.numberFacePoints; i++)
                {
                    newFacepnts[i] = this.facePoints[i];
                }
                for (int i = 0; i < meshGroupToAppend.numberFacePoints; i++)
                {
                    newFacepnts[i + this.numberFacePoints] = new FacePoint(meshGroupToAppend.facePoints[i].vertexIndex + this.numVertices,
                        meshGroupToAppend.facePoints[i].normals, meshGroupToAppend.facePoints[i].UVs);
                }
                this.facePoints = newFacepnts;

                this.numVertices += meshGroupToAppend.numVertices;
                this.numFaces += meshGroupToAppend.numFaces;
            }

            internal void Write(BinaryWriter bw)
            {
                bw.Write(numVertices);
                for (int i = 0; i < numVertices; i++)
                {
                    vertices[i].Write(bw);
                }
                bw.Write(numFaces);
                for (int i = 0; i < numFaces * 3; i++)
                {
                    facePoints[i].Write(bw);
                }
                bw.Write(numGeostates);
                bw.Write(nameLen);
                bw.Write(meshNameStr);
            }

            public vertexExtended[] getExtendedVertices()
            {
                vertexExtended[] vertExt = new vertexExtended[this.numberVertices];
                for (int v = 0; v < this.numberVertices; v++)
                {
                    vertExt[v] = new vertexExtended(this.getVertex(v));    //copy vertices to 
                }                                                           //extended vertex with normals and uv
                for (int f = 0; f < this.numFaces * 3; f++)
                {
                    FacePoint fp = this.getFacePoint(f);
                    vertExt[fp.vertexIndex].setNormals(fp.normals);
                    vertExt[fp.vertexIndex].setUVs(fp.UVs);
                }
                return vertExt;
            }
            public bool applyExtendedVertices(vertexExtended[] extendedVerts, bool applyNormals, bool applyUVs)
            {
                if (this.numberVertices != extendedVerts.Length) return false;
                for (int v = 0; v < this.numberVertices; v++)
                {
                    this.vertices[v] = new Vertex(extendedVerts[v].position, extendedVerts[v].vertexID, extendedVerts[v].tagVal,
                        extendedVerts[v].bones, extendedVerts[v].boneWeights);
                } 
                for (int f = 0; f < this.numFaces * 3; f++)
                {
                    int vertInd = this.facePoints[f].vertexIndex;
                    if (vertInd >= this.numberVertices) return false;
                    if (applyNormals) { this.facePoints[f].normals = extendedVerts[vertInd].getNormals(); }
                    if (applyUVs) { this.facePoints[f].UVs = extendedVerts[vertInd].getUVs(); }
                }
                return true;
            }

        }

        public class Vertex
        {
            internal float x, y, z;
            internal int vertID;
            internal uint tagV;
            internal int[] boneAss = new int[4];
            internal float[] weights = new float[4];

            public float[] position
            {
                get
                {
                    return new float[3] { x, y, z };
                }
                set
                {
                    this.x = value[0];
                    this.y = value[1];
                    this.z = value[2];
                }
            }
            public int vertexID
            {
                get
                {
                    return this.vertID;
                }
                set
                {
                    this.vertID = value;
                }
            }
            public uint tagVal
            {
                get
                {
                    return this.tagV;
                }
                set
                {
                    this.tagV = value;
                }
            }
            public int[] bones
            {
                get
                {
                    return boneAss;
                }
                set
                {
                    for (int i = 0; i < 4; i++)
                    {
                        this.boneAss[i] = value[i];
                    }
                }
            }
            public float[] boneWeights
            {
                get
                {
                    return this.weights;
                }
                set
                {
                    for (int i = 0; i < 4; i++)
                    {
                        this.weights[i] = value[i];
                    }
                }
            }

            public Vertex() { }

            public Vertex(Vertex v)
            {
                this.x = v.x;
                this.y = v.y;
                this.z = v.z;
                this.vertID = v.vertID;
                this.tagV = v.tagV;
                this.boneAss = new int[v.boneAss.Length];
                this.weights = new float[v.weights.Length];
                for (int i = 0; i < v.boneAss.Length; i++)
                {
                    this.boneAss[i] = v.boneAss[i];
                    this.weights[i] = v.weights[i];
                }
            }

            public Vertex(float[] position, int vertexID, uint tagval, int[] boneAssignments, float[] boneWeights)
            {
                this.x = position[0];
                this.y = position[1];
                this.z = position[2];
                this.vertID = vertexID;
                this.tagV = tagval;
                this.boneAss = boneAssignments;
                this.weights = boneWeights;
            }

            internal Vertex(BinaryReader br)
            {
                x = br.ReadSingle();
                y = br.ReadSingle();
                z = br.ReadSingle();
                vertID = br.ReadInt32();
                tagV = br.ReadUInt32();
                for (int i = 0; i < 4; i++)
                {
                    boneAss[i] = br.ReadInt32();
                }
                for (int i = 0; i < 4; i++)
                {
                    weights[i] = br.ReadSingle();
                }
            }

            internal void Write(BinaryWriter bw)
            {
                bw.Write(x);
                bw.Write(y);
                bw.Write(z);
                bw.Write(vertID);
                bw.Write(tagV);
                for (int i = 0; i < 4; i++)
                {
                    bw.Write(boneAss[i]);
                }
                for (int i = 0; i < 4; i++)
                {
                    bw.Write(weights[i]);
                }
            }

            public override string ToString()
            {
                return this.vertID.ToString() + ", X: " + this.x.ToString() + ", Y: " + this.y.ToString() + ", Z: " + this.z.ToString() +
                    ", tag: " + this.tagV.ToString() + ", Bones: " + this.boneAss[0].ToString() + ":" + this.weights[0].ToString() + ", " +
                    this.boneAss[1].ToString() + ":" + this.weights[1].ToString() + ", " + this.boneAss[2].ToString() + ":" + this.weights[2].ToString() + ", " +
                    this.boneAss[3].ToString() + ":" + this.weights[3].ToString();
            }
        }
 
        public class vertexExtended : WSO.Vertex
        { 
            internal float normX, normY, normZ;
            internal float UVx, UVy;

            public vertexExtended() : base() {}

            public vertexExtended(Vertex v)
            {
                float[] pos = v.position;
                this.x = pos[0];
                this.y = pos[1];
                this.z = pos[2];
                this.vertID = v.vertexID;
                this.tagV = v.tagVal;
                this.boneAss = v.bones;
                this.weights = v.boneWeights;
            }

            public float[] getPosition()
            {
                return new float[] { this.x, this.y, this.z };
            }
            public void setPosition(float[] pos)
            {
                this.x = pos[0];
                this.y = pos[1];
                this.z = pos[2];
            }
            public float[] getNormals()
            {
                return new float[] { normX, normY, normZ };
            }
            public void setNormals(float[] norms)
            {
                this.normX = norms[0];
                this.normY = norms[1];
                this.normZ = norms[2];
            }
            public float[] getUVs()
            {
                return new float[] { UVx, UVy };
            }
            public void setUVs(float[] UVs)
            {
                this.UVx = UVs[0];
                this.UVy = UVs[1];
            }
        }

        public class FacePoint
        {
            short vertexInd;
            float normX, normY, normZ;
            float UVx, UVy;

            public short vertexIndex
            {
                get
                {
                    return this.vertexInd;
                }
            }
            public float[] normals
            {
                get
                {
                    return new float[3] { normX, normY, normZ };
                }
                set
                {
                    this.normX = value[0];
                    this.normY = value[1];
                    this.normZ = value[2];
                }
            }
            public float[] UVs
            {
                get
                {
                    return new float[2] { UVx, UVy };
                }
                set
                {
                    this.UVx = value[0];
                    this.UVy = value[1];
                }
            }

            public FacePoint() { }

            public FacePoint(FacePoint f)
            {
                this.vertexInd = f.vertexInd;
                this.normX = f.normX;
                this.normY = f.normY;
                this.normZ = f.normZ;
                this.UVx = f.UVx;
                this.UVy = f.UVy;
            }

            public FacePoint(int vertIndex, float[] normals, float[] UVs)
            {
                this.vertexInd = (short)vertIndex;
                this.normX = normals[0];
                this.normY = normals[1];
                this.normZ = normals[2];
                this.UVx = UVs[0];
                this.UVy = UVs[1];
            }

            public FacePoint(int vertIndex, float[] normals, float[] UVs, bool verticalFlip)
            {
                this.vertexInd = (short) vertIndex;
                this.normX = normals[0];
                this.normY = normals[1];
                this.normZ = normals[2];
                this.UVx = UVs[0];
                if (verticalFlip)
                {
                    this.UVy = 1f - UVs[1];
                }
                else
                {
                    this.UVy = UVs[1];
                }
            }

            internal FacePoint(BinaryReader br)
            {
                vertexInd = br.ReadInt16();
                normX = br.ReadSingle();
                normY = br.ReadSingle();
                normZ = br.ReadSingle();
                UVx = br.ReadSingle();
                UVy = br.ReadSingle();
            }

            internal void Write(BinaryWriter bw)
            {
                bw.Write(vertexInd);
                bw.Write(normX);
                bw.Write(normY);
                bw.Write(normZ);
                bw.Write(UVx);
                bw.Write(UVy);
            }

            public override string ToString()
            {
                return "Index: " + this.vertexIndex.ToString() + ", Normals X: " + this.normX.ToString() + ", Y: " + this.normY.ToString() +
                    ", Z: " + this.normZ.ToString() + ", UV X: " + this.UVx.ToString() + ", Y: " + this.UVy.ToString(); 
            }
        }

        public class Bone
        {
            byte nameLen;
            char[] boneName;
            float[] bonePosition = new float[3];
            float[] boneRotation = new float[3];

            public string Name
            {
                get
                {
                    return new string(this.boneName);
                }
                set
                {
                    this.boneName = value.ToCharArray();
                    this.nameLen = (byte) this.boneName.Length;
                }
            }

            internal Bone() { }

            internal Bone(BinaryReader br)
            {
                nameLen = br.ReadByte();
                boneName = new char[nameLen];
                boneName = br.ReadChars(nameLen);
                for (int i = 0; i < 3; i++)
                {
                    bonePosition[i] = br.ReadSingle();
                }
                for (int i = 0; i < 3; i++)
                {
                    boneRotation[i] = br.ReadSingle();
                }
            }

            public Bone(Bone source)
            {
                this.nameLen = source.nameLen;
                this.boneName = new char[source.nameLen];
                for (int i = 0; i < source.nameLen; i++)
                {
                    this.boneName[i] = source.boneName[i];
                }
                for (int i = 0; i < 3; i++)
                {
                    this.bonePosition[i] = source.bonePosition[i];
                    this.boneRotation[i] = source.boneRotation[i];
                }
            }

            public Bone(uint boneHash)
            {
                string str = Enum.GetName(typeof(XmodsEnums.BoneHash), boneHash);
                this.nameLen = (byte) str.Length;
                this.boneName = str.ToCharArray();
                this.bonePosition = new float[] { 0f, 0f, 0f };
                this.boneRotation = new float[] { 0f, 0f, 0f };
            }

            public Bone(string boneName)
            {
                this.nameLen = (byte)boneName.Length;
                this.boneName = boneName.ToCharArray();
                this.bonePosition = new float[] { 0f, 0f, 0f };
                this.boneRotation = new float[] { 0f, 0f, 0f };
            }

            internal void Write(BinaryWriter bw)
            {
                bw.Write(nameLen);
                bw.Write(boneName);
                for (int i = 0; i < 3; i++)
                {
                    bw.Write(bonePosition[i]);
                }
                for (int i = 0; i < 3; i++)
                {
                    bw.Write(boneRotation[i]);
                }
            }

            public bool Equals(Bone bone)
            {
                if (this.nameLen != bone.nameLen) return false;
                for (int i = 0; i < this.nameLen; i++)
                {
                    if (!this.boneName[i].Equals(bone.boneName[i])) return false;
                }
                return true;
            }

            public override bool Equals(object obj)
            {
                return this.Equals(obj as Bone);
            }

            public override int GetHashCode()
            {
                return this.Name.GetHashCode();
            }

            public override string ToString()
            {
                return new string(this.boneName);
            }
        }

        internal static bool IsEqual(float x, float y)
        {
            return (Math.Abs(x - y) <= (Math.Abs(x) + Math.Abs(y)) / 2000000f);
        }

        public enum MeshName
        {
            group_0,
            group_base,
            group_fat,
            group_fit,
            group_thin,
            group_special
        }
    }

    [global::System.Serializable]
    public class WSOException : ApplicationException
    {
        public WSOException() { }
        public WSOException(string message) : base(message) { }
        public WSOException(string message, Exception inner) : base(message, inner) { }
        protected WSOException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }

}
