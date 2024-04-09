using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Collada141;
using System.Xml;
using System.Xml.Serialization;

namespace Xmods.DataLib
{
    public class DAE
    {
        private List<ColladaMesh> meshes;
        private bool y_up;
        Skeleton skeleton;

        public ColladaMesh[] Meshes
        {
            get { return this.meshes.ToArray(); }
            set { this.meshes = new List<ColladaMesh>(value); }
        }

        public string[] getMeshNames
        {
            get
            {
                string[] tmp = new string[this.meshes.Count];
                for (int i = 0; i < meshes.Count; i++) tmp[i] = meshes[i].meshName.Length > 0 ? meshes[i].meshName : meshes[i].meshID;
                return tmp;
            }
        }

        public int TotalFaces
        {
            get
            {
                int tmp = 0;
                foreach (ColladaMesh mesh in this.meshes)
                {
                    tmp += mesh.TotalFaces;
                }
                return tmp;
            }
        }

        public bool AllMeshesHaveUV1
        {
            get
            {
                foreach (ColladaMesh mesh in meshes) if (!mesh.HasUV1) return false;
                return true;
            }
        }

        public bool AllMeshesHaveBones
        {
            get
            {
                foreach (ColladaMesh mesh in meshes) if (!mesh.HasBones) return false;
                return true;
            }
        }

        public bool AllMeshesHaveColors
        {
            get
            {
                foreach (ColladaMesh mesh in meshes) if (!mesh.HasColors) return false;
                return true;
            }
        }

        public bool Y_UP
        {
            get { return this.y_up; }
        }

        public DAE(string path) : this(path, false) {}
        
        public DAE(string path, bool flipYZ)
        {
            COLLADA dae = COLLADA.Load(path);
            this.meshes = new List<ColladaMesh>();

            this.y_up = dae.asset.up_axis == UpAxisType.Y_UP;

            // Iterate on libraries
            foreach (var item in dae.Items)
            {
                if (item is library_geometries)
                {
                    library_geometries geometries = item as library_geometries;
                    if (geometries == null) continue;

                    foreach (geometry geom in geometries.geometry)
                    {
                        if (geom == null) continue;
                        ColladaMesh daeMesh = new ColladaMesh();
                        daeMesh.meshID = geom.id;
                        daeMesh.meshName = geom.name != null ? geom.name.Replace("-mesh", "") : geom.id.Replace("-mesh", "");

                        mesh geomMesh = geom.Item as mesh;
                        if (geomMesh == null) continue;

                        int positionOffset = -1, normalOffset = -1, colorOffset = -1, vertexOffset = -1;
                        SortedDictionary<int, int> uvOffset = new SortedDictionary<int, int>();
                        double[] positions = new double[0];
                        double[] normals = new double[0];
                        SortedDictionary<ulong, double[]> uvs = new SortedDictionary<ulong, double[]>();
                        double[] colors = new double[0];
                        int[] vcounts = null;
                        uint[] triangleIndices = null;

                        if (geomMesh.Items == null) continue;
                        InputLocalOffset[] inputs = new InputLocalOffset[0];
                        foreach (var meshItem in geomMesh.Items)
                        {
                            if (meshItem is polygons)
                            {
                                throw new ApplicationException("Collada meshes using polygons structures cannot be converted!");
                            }
                            if (meshItem is polylist)
                            {
                                polylist polys = meshItem as polylist;
                                inputs = polys.input;
                                if (polys.vcount != null && polys.p != null)
                                {
                                    string[] vc = polys.vcount.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                                    string[] p = polys.p.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                                    vcounts = new int[vc.Length];
                                    for (int i = 0; i < vc.Length; i++)
                                    {
                                        int tmp;
                                        if (!int.TryParse(vc[i], out tmp)) throw new ApplicationException("Can't read polylist vcount: " + vc[i]);
                                        if (tmp > 4) throw new ApplicationException("Can't convert polygon with more than 4 sides!");
                                        vcounts[i] = tmp;
                                    }
                                    triangleIndices = new uint[p.Length];
                                    for (int i = 0; i < p.Length; i++)
                                    {
                                        uint tmp;
                                        if (!uint.TryParse(p[i], out tmp)) throw new ApplicationException("Can't read polylist p: " + p[i]);
                                        triangleIndices[i] = tmp;
                                    }
                                }
                            }
                            else if (meshItem is triangles)
                            {
                                triangles tri = meshItem as triangles;
                                inputs = tri.input;
                                string[] p = tri.p.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                                triangleIndices = new uint[p.Length];
                                for (int i = 0; i < p.Length; i++)
                                {
                                    uint tmp;
                                    if (!uint.TryParse(p[i], out tmp)) throw new ApplicationException("Can't read triangles p: " + p);
                                    triangleIndices[i] = tmp;
                                }
                            }
                        }

                        foreach (InputLocalOffset input in inputs)
                        {
                            if (String.Compare(input.semantic, "VERTEX") == 0)
                            {
                                vertexOffset = (int)input.offset;
                            }
                            foreach (source s in geomMesh.source)
                            {
                                if (String.Compare(s.id, input.source.Replace("#", "")) == 0)
                                {   
                                    if (String.Compare(input.semantic, "POSITION") == 0)
                                    {
                                        positions = (s.Item as float_array).Values;
                                        positionOffset = (int)input.offset;
                                    }
                                    else if (String.Compare(input.semantic, "NORMAL") == 0)
                                    {
                                        normals = (s.Item as float_array).Values;
                                        normalOffset = (int)input.offset;
                                    }
                                    else if (String.Compare(input.semantic, "TEXCOORD") == 0)
                                    {
                                        uvs.Add(input.set, (s.Item as float_array).Values);
                                        uvOffset.Add((int)input.set, (int)input.offset);
                                    }
                                    else if (String.Compare(input.semantic, "COLOR") == 0)
                                    {
                                        colors = (s.Item as float_array).Values;
                                        colorOffset = (int)input.offset;
                                    }
                                }
                            }
                        }

                        var meshVerts = geomMesh.vertices;
                        if (meshVerts != null)
                        {
                            var vertInputs = meshVerts.input;
                            foreach (InputLocal vertInput in vertInputs)
                            {
                                foreach (source s in geomMesh.source)
                                {
                                    if (String.Compare(s.id, vertInput.source.Replace("#", "")) == 0)
                                    {
                                        if (String.Compare(vertInput.semantic, "POSITION") == 0)
                                        {
                                            positions = (s.Item as float_array).Values;
                                            positionOffset = vertexOffset;
                                        }
                                        else if (String.Compare(vertInput.semantic, "NORMAL") == 0)
                                        {
                                            normals = (s.Item as float_array).Values;
                                            normalOffset = vertexOffset;
                                        }
                                        else if (String.Compare(vertInput.semantic, "TEXCOORD") == 0)
                                        {
                                            uvs.Add(0, (s.Item as float_array).Values);
                                            uvOffset.Add(0, vertexOffset);
                                        }
                                        else if (String.Compare(vertInput.semantic, "COLOR") == 0)
                                        {
                                            colors = (s.Item as float_array).Values;
                                            colorOffset = vertexOffset;
                                        }
                                    }
                                }
                            }
                        }

                        List<Vector3> posVectors = new List<Vector3>();
                        for (int i = 0; i < positions.Length; i += 3)
                        {
                            Vector3 pos;
                            if (flipYZ) pos = new Vector3((float)positions[i], (float)positions[i + 2], (float)-positions[i + 1]);
                            else pos = new Vector3((float)positions[i], (float)positions[i + 1], (float)positions[i + 2]);
                            posVectors.Add(pos);
                        }
                        daeMesh.positions = posVectors.ToArray();

                        List<Vector3> normVectors = new List<Vector3>();
                        for (int i = 0; i < normals.Length; i += 3)
                        {
                            Vector3 norm;
                            if (flipYZ) norm = new Vector3((float)normals[i], (float)normals[i + 2], (float)-normals[i + 1]);
                            else norm = new Vector3((float)normals[i], (float)normals[i + 1], (float)normals[i + 2]);
                            normVectors.Add(norm);
                        }
                        daeMesh.normals = normVectors.ToArray();

                        if (uvs.Count > 0)
                        {
                            List<Vector2[]> uvSets = new List<Vector2[]>();
                            foreach (KeyValuePair<ulong, double[]> pair in uvs)
                            {
                                List<Vector2> map = new List<Vector2>();
                                for (int i = 0; i < pair.Value.Length; i += 2)
                                {
                                    Vector2 uv = new Vector2((float)pair.Value[i], (float)(1d - pair.Value[i + 1]));
                                    map.Add(uv);
                                }
                                uvSets.Add(map.ToArray());
                            }
                            daeMesh.uvs = uvSets.ToArray();
                        }

                        List<uint> colorList = new List<uint>();
                        for (int i = 0; i < colors.Length; i += 3)
                        {
                            byte r = (byte)(colors[i] * 255);
                            byte g = (byte)(colors[i + 1] * 255);
                            byte b = (byte)(colors[i + 2] * 255);
                            uint color = b + ((uint)g << 8) + ((uint)r << 16) + ((uint)255 << 24);
                            colorList.Add(color);
                        }
                        daeMesh.colors = colorList.ToArray();

                        if (uvOffset.Count > 0)
                        {
                            List<int> uvOffs = new List<int>();
                            foreach (KeyValuePair<int, int> pair in uvOffset) { uvOffs.Add(pair.Value); }
                            Offsets offsets = new Offsets(positionOffset, normalOffset, uvOffs.ToArray(), colorOffset);
                            daeMesh.offsets = offsets;
                        }
                        else
                        {
                            daeMesh.offsets = new Offsets(positionOffset, normalOffset, new int[0], colorOffset);
                        }

                        int facePointStride = daeMesh.Stride;
                        if (triangleIndices == null) throw new ApplicationException("No polygons found in mesh!");
                        if (vcounts != null)
                        {
                            List<uint> newIndices = new List<uint>();
                            uint ind = 0;
                            for (int i = 0; i < vcounts.Length; i++)
                            {
                                if (vcounts[i] == 4)
                                {
                                    uint[] tmp = new uint[facePointStride * 3];
                                    Array.Copy(triangleIndices, ind, tmp, 0, facePointStride * 3);  //copy first 3 points of quad
                                    newIndices.AddRange(tmp);
                                    tmp = new uint[facePointStride * 2];
                                    Array.Copy(triangleIndices, ind + (facePointStride * 2), tmp, 0, facePointStride * 2);  //copy third & fourth points of quad
                                    newIndices.AddRange(tmp);
                                    tmp = new uint[facePointStride];
                                    Array.Copy(triangleIndices, ind, tmp, 0, facePointStride);      //copy first point, splitting quad into triangles
                                    newIndices.AddRange(tmp);
                                    ind += (uint)(facePointStride * 4);
                                }
                                else if (vcounts[i] == 3)
                                {
                                    uint[] tmp = new uint[facePointStride * 3];
                                    Array.Copy(triangleIndices, ind, tmp, 0, facePointStride * 3);  //copy triangle
                                    newIndices.AddRange(tmp);
                                    ind += (uint)(facePointStride * 3);
                                }
                                else
                                {
                                    throw new ApplicationException("Invalid vcount: " + vcounts[i].ToString());
                                }
                            }
                            triangleIndices = newIndices.ToArray();
                        }
                        daeMesh.facePoints = triangleIndices;

                        this.meshes.Add(daeMesh);
                    }
                }

                else if (item is library_controllers)
                {
                    library_controllers rigs = item as library_controllers;
                    if (rigs == null || rigs.controller == null) continue;
                    foreach (controller rig in rigs.controller)
                    {
                        if (rig == null) continue;
                        if (rig.Item is skin && rig.Item != null)
                        {
                            var rigSkin = rig.Item as skin;
                            int meshIndex = 0;
                            bool foundMesh = false;
                            for (int i = 0; i < this.meshes.Count; i++)
                            {
                                if (String.Compare(rigSkin.source1.Replace("#", ""), this.meshes[i].meshID) == 0)
                                {
                                    meshIndex = i;
                                    foundMesh = true;
                                    break;
                                }
                            }
                            if (!foundMesh) break;
                            this.meshes[meshIndex].controllerID = rig.id;
                            if (rigSkin.bind_shape_matrix != null)
                            {
                                string bind = rigSkin.bind_shape_matrix;
                                string[] bindStrings = bind.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                                double[] bindArray = new double[16];
                                for (int i = 0; i < 16; i++)
                                {
                                    double tmp;
                                    if (!double.TryParse(bindStrings[i], out tmp)) throw new ApplicationException("Can't read bind shape matrix: " + bindStrings[i]);
                                    bindArray[i] = tmp;
                                }
                                this.meshes[meshIndex].bindShapeMatrix = new Matrix4D(bindArray);
                            }
                            double[] weights = new double[0];
                            foreach (source s in rigSkin.source)
                            {
                                var access = s.technique_common.accessor;
                                string param = access.param[0].name;
                                if (String.Compare(param, "JOINT") == 0)
                                {
                                    Name_array joints = s.Item as Name_array;
                                    this.meshes[meshIndex].jointNames = joints.Values;
                                }
                                else if (String.Compare(param, "TRANSFORM") == 0)
                                {
                                    float_array inverseBind = s.Item as float_array;
                                    double[] inverseBindMatrix = inverseBind.Values;
                                    Matrix4D[] inverseBindPose = new Matrix4D[inverseBindMatrix.Length / 16];
                                    for (int i = 0; i < inverseBindMatrix.Length; i += 16)
                                    {
                                        double[] tmp = new double[16];
                                        Array.Copy(inverseBindMatrix, i, tmp, 0, 16);
                                        inverseBindPose[i / 16] = new Matrix4D(tmp);
                                    }
                                    this.meshes[meshIndex].inverseBindMatrix = inverseBindPose;
                                }
                                else if (String.Compare(param, "WEIGHT") == 0)
                                {
                                    float_array jointWeights = s.Item as float_array;
                                    weights = jointWeights.Values;
                                }
                            }
                            int[] vcount = new int[0];
                            int[] v = new int[0];
                            if (rigSkin.vertex_weights.vcount != null && rigSkin.vertex_weights.v != null)
                            {
                                string[] vc = rigSkin.vertex_weights.vcount.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                                string[] vb = rigSkin.vertex_weights.v.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                                vcount = new int[vc.Length];
                                for (int i = 0; i < vc.Length; i++)
                                {
                                    int tmp;
                                    if (!int.TryParse(vc[i], out tmp)) throw new ApplicationException("Can't read joints vcount: " + vc[i]);
                                    if (tmp > 4) throw new ApplicationException("More than 4 bone assignments found in mesh: " + rigSkin.source1);
                                    vcount[i] = tmp;
                                }
                                v = new int[vb.Length];
                                for (int i = 0; i < vb.Length; i++)
                                {
                                    int tmp;
                                    if (!int.TryParse(vb[i], out tmp)) throw new ApplicationException("Can't read joints v: " + vb[i]);
                                    v[i] = tmp;
                                }
                            }
                            int index = 0;
                            List<BoneAssignment> bones = new List<BoneAssignment>();
                            for (int i = 0; i < vcount.Length; i++)
                            {
                                int[] assignment = new int[4];
                                for (int j = 0; j < 4; j++) assignment[j] = -1;
                                float[] weight = new float[4];
                                for (int j = 0; j < vcount[i]; j++)
                                {
                                    assignment[j] = v[index];
                                    weight[j] = (float)weights[v[index + 1]];
                                    index += 2;
                                }
                                BoneAssignment b = new BoneAssignment(assignment, weight);
                                bones.Add(b);
                            }
                            this.meshes[meshIndex].bones = bones.ToArray();
                        }
                    }
                }

                else if (item is library_visual_scenes)
                {
                    library_visual_scenes scenes = item as library_visual_scenes;
                    if (scenes == null || scenes.visual_scene == null) continue;

                    foreach (visual_scene scene in scenes.visual_scene)
                    {
                        List<SkeletonJoint> joints = new List<SkeletonJoint>();
                        Matrix4D transform = Matrix4D.Identity;
                        foreach (node n in scene.node)
                        {
                            if (n.instance_geometry != null)
                            {
                                Matrix4D trans = ReadTransform(n);
                                foreach (instance_geometry g in n.instance_geometry)
                                {
                                    for (int i = 0; i < this.meshes.Count; i++)
                                    {
                                        if (String.Compare(g.url.Replace("#", ""), this.meshes[i].meshID) == 0)
                                        {
                                            this.meshes[i].sceneGeomMatrix = trans;
                                        }
                                    }
                                }
                            }
                            else if (n.instance_controller != null)
                            {
                                Matrix4D trans = ReadTransform(n);
                                foreach (instance_controller c in n.instance_controller)
                                {
                                    for (int i = 0; i < this.meshes.Count; i++)
                                    {
                                        if (String.Compare(c.url.Replace("#", ""), this.meshes[i].controllerID) == 0)
                                        {
                                            this.meshes[i].sceneControllerMatrix = trans;
                                        }
                                    }
                                }
                            }
                            else if (n.type == NodeType.NODE & n.node1 != null)
                            {
                                transform = ReadTransform(n);
                                foreach (var b in n.node1)
                                {
                                    if (b is node && b.type == NodeType.JOINT)
                                    {
                                        ReadSkeletonJoint(b, null, joints);
                                    }
                                }
                            }
                            else if (n.type == NodeType.JOINT)
                            {
                                ReadSkeletonJoint(n, null, joints);
                            }

                        }
                        this.skeleton = new Skeleton(transform, joints.ToArray());
                    }
                }
            }                
        }

        internal void ReadSkeletonJoint(node joint, string parentName, List<SkeletonJoint> skeleton)
        {
            if (joint.type != NodeType.JOINT) return;
            skeleton.Add(new SkeletonJoint(joint.sid, parentName, ReadTransform(joint)));            
            if (joint.node1 == null) return;
            foreach (var jointNode in joint.node1)
            {
                if (jointNode is node && ((node)jointNode).type == NodeType.JOINT)
                {
                    ReadSkeletonJoint(jointNode as node, joint.sid, skeleton);
                }
            }
        }

        internal Matrix4D ReadTransform(node n)
        {
            Matrix4D transform = Matrix4D.Identity;
            if (n.Items == null) return transform;
            for (int i = 0; i < n.Items.Length; i++)
            {
                if (n.ItemsElementName[i] == ItemsChoiceType2.matrix)
                {
                    matrix mj = n.Items[i] as matrix;
                    transform = transform * new Matrix4D(mj.Values);
                }
                else if (n.ItemsElementName[i] == ItemsChoiceType2.translate)
                {
                    TargetableFloat3 mj = n.Items[i] as TargetableFloat3;
                    transform = transform * Matrix4D.FromOffset(mj.Values);
                }
                else if (n.ItemsElementName[i] == ItemsChoiceType2.rotate)
                {
                    rotate mj = n.Items[i] as rotate;
                    AxisAngle aa = new AxisAngle(mj.Values);
                    transform = transform * Matrix4D.FromAxisAngle(aa);
                }
                else if (n.ItemsElementName[i] == ItemsChoiceType2.scale)
                {
                    TargetableFloat3 mj = n.Items[i] as TargetableFloat3;
                    transform = transform * Matrix4D.FromScale(mj.Values);
                }
            }
            return transform;
        }

        public DAE(GEOM[] geomArray, string[] meshNames, RIG rig, bool Y_Up)
        {
            this.y_up = Y_Up;
            Matrix4D axisTransform = Y_Up ? Matrix4D.Identity : Matrix4D.RotateYupToZup;
            this.meshes = new List<ColladaMesh>();
            for (int m = 0; m < geomArray.Length; m++)
            {
                GEOM geom = new GEOM(geomArray[m]);
                geom.FixUnusedBones();
                ColladaMesh mesh = new ColladaMesh();
                mesh.meshName = meshNames != null ? meshNames[m].Replace(" ", "") : "CASMesh" + m.ToString();
                mesh.meshID = mesh.meshName;
                mesh.bindShapeMatrix = Matrix4D.Identity;
                mesh.sceneControllerMatrix = Matrix4D.Identity;
                mesh.sceneGeomMatrix = Matrix4D.Identity;
                List<Vector3> positions = new List<Vector3>();
                List<Vector3> normals = new List<Vector3>();
                List<Vector2>[] uvs = new List<Vector2>[geom.numberUVsets];
                for (int i = 0; i < geom.numberUVsets; i++) uvs[i] = new List<Vector2>();
                List<uint> colors = new List<uint>();
                List<BoneAssignment> bones = new List<BoneAssignment>();
                bool hasNormals = geom.hasNormals;
                bool hasUVs = geom.hasUVs;
                bool hasColors = geom.hasTags;
                bool hasBones = geom.hasBones;
                for (int v = 0; v < geom.numberVertices; v++)
                {
                    positions.Add(axisTransform * new Vector3(geom.getPosition(v)));
                    if (hasNormals) normals.Add(axisTransform * new Vector3(geom.getNormal(v)));
                    if (hasUVs) for (int i = 0; i < geom.numberUVsets; i++) uvs[i].Add(new Vector2(geom.getUV(v, i)));
                    if (hasColors) colors.Add(geom.getTagval(v));
                    if (hasBones) bones.Add(new BoneAssignment(geom.getBones(v), geom.getBoneWeights(v)));
                }
                mesh.positions = positions.ToArray();
                mesh.normals = normals.ToArray();
                mesh.uvs = new Vector2[geom.numberUVsets][];
                for (int i = 0; i < geom.numberUVsets; i++) mesh.uvs[i] = uvs[i].ToArray();
                mesh.colors = colors.ToArray();
                mesh.bones = bones.ToArray();

                int positionOffset = 0, normalsOffset = -1, colorsOffset = -1;
                int[] uvOffsets = new int[geom.numberUVsets];
                int index = 1;
                if (hasNormals)
                {
                    normalsOffset = index;
                    index++;
                }
                if (hasUVs)
                {
                    for (int i = 0; i < geom.numberUVsets; i++) 
                    {
                        uvOffsets[i] = index;
                        index++;
                    }
                }
                if (hasColors)
                {
                    colorsOffset = index;
                }
                mesh.offsets = new Offsets(positionOffset, normalsOffset, uvOffsets, colorsOffset);

                List<uint> facepoints = new List<uint>();
                for (int f = 0; f < geom.numberFaces; f++)
                {
                    uint[] face = geom.getFaceIndicesUint(f);
                    for (int i = 0; i < 3; i++)
                    {
                        for (int j = 0; j < mesh.Stride; j++)
                        facepoints.Add(face[i]);
                    }
                }
                mesh.facePoints = facepoints.ToArray();

                mesh.Clean();

                List<string> boneNames = new List<string>();
                List<Matrix4D> inverseBinds = new List<Matrix4D>();
                for (int b = 0; b < geom.BoneHashList.Length; b++)
                {
                    RIG.Bone bone = rig.GetBone(geom.BoneHashList[b]);
                    boneNames.Add(bone.BoneName);
                    inverseBinds.Add((axisTransform * bone.GlobalTransform).Inverse());
                }
                mesh.jointNames = boneNames.ToArray();
                mesh.inverseBindMatrix = inverseBinds.ToArray();

                this.meshes.Add(mesh);
            }

            DAE.SkeletonJoint[] joints = new SkeletonJoint[rig.NumberBones];
            joints[0] = new SkeletonJoint(rig.Bones[0].BoneName, "", axisTransform * rig.Bones[0].LocalTransform);
            for (int i = 1; i < rig.NumberBones; i++)
            {
                joints[i] = new SkeletonJoint(rig.Bones[i].BoneName, rig.Bones[i].ParentName, rig.Bones[i].LocalTransform);
            }
            this.skeleton = new Skeleton(Matrix4D.Identity, joints);
        }

        public DAE(GEOM geom, List<GEOM.Face[]> layers, List<string> layerNames, RIG rig, bool Y_Up)
        {
            this.y_up = Y_Up;
            Matrix4D axisTransform = Y_Up ? Matrix4D.Identity : Matrix4D.RotateYupToZup;
            this.meshes = new List<ColladaMesh>();
            geom.FixUnusedBones();
            for (int m = 0; m < layers.Count; m++)
            {
                ColladaMesh mesh = new ColladaMesh();
                mesh.meshName = layerNames != null ? layerNames[m].Replace(" ", "") : "CASMesh" + m.ToString();
                mesh.meshID = mesh.meshName;
                mesh.bindShapeMatrix = Matrix4D.Identity;
                mesh.sceneControllerMatrix = Matrix4D.Identity;
                mesh.sceneGeomMatrix = Matrix4D.Identity;
                List<Vector3> positions = new List<Vector3>();
                List<Vector3> normals = new List<Vector3>();
                List<Vector2>[] uvs = new List<Vector2>[geom.numberUVsets];
                for (int i = 0; i < geom.numberUVsets; i++) uvs[i] = new List<Vector2>();
                List<uint> colors = new List<uint>();
                List<BoneAssignment> bones = new List<BoneAssignment>();
                bool hasNormals = geom.hasNormals;
                bool hasUVs = geom.hasUVs;
                bool hasColors = geom.hasTags;
                bool hasBones = geom.hasBones;
                GEOM.Face[] faces = layers[m];
                for (int f = 0; f < faces.Length; f++)
                {
                    for (int p = 0; p < faces[f].meshface.Length; p++)
                    {
                        int v = (int)faces[f].meshface[p];
                        positions.Add(axisTransform * new Vector3(geom.getPosition(v)));
                        if (hasNormals) normals.Add(axisTransform * new Vector3(geom.getNormal(v)));
                        if (hasUVs) for (int i = 0; i < geom.numberUVsets; i++) uvs[i].Add(new Vector2(geom.getUV(v, i)));
                        if (hasColors) colors.Add(geom.getTagval(v));
                        if (hasBones) bones.Add(new BoneAssignment(geom.getBones(v), geom.getBoneWeights(v)));
                    }
                }
                mesh.positions = positions.ToArray();
                mesh.normals = normals.ToArray();
                mesh.uvs = new Vector2[geom.numberUVsets][];
                for (int i = 0; i < geom.numberUVsets; i++) mesh.uvs[i] = uvs[i].ToArray();
                mesh.colors = colors.ToArray();
                mesh.bones = bones.ToArray();

                int positionOffset = 0, normalsOffset = -1, colorsOffset = -1;
                int[] uvOffsets = new int[geom.numberUVsets];
                int index = 1;
                if (hasNormals)
                {
                    normalsOffset = index;
                    index++;
                }
                if (hasUVs)
                {
                    for (int i = 0; i < geom.numberUVsets; i++)
                    {
                        uvOffsets[i] = index;
                        index++;
                    }
                }
                if (hasColors)
                {
                    colorsOffset = index;
                }
                mesh.offsets = new Offsets(positionOffset, normalsOffset, uvOffsets, colorsOffset);

                List<uint> facepoints = new List<uint>();
                for (int f = 0; f < faces.Length; f++)
                {
                    uint[] face = geom.getFaceIndicesUint(f);
                    for (int i = 0; i < 3; i++)
                    {
                        for (int j = 0; j < mesh.Stride; j++)
                            facepoints.Add(face[i]);
                    }
                }
                mesh.facePoints = facepoints.ToArray();

                mesh.Clean();

                List<string> boneNames = new List<string>();
                List<Matrix4D> inverseBinds = new List<Matrix4D>();
                for (int b = 0; b < geom.BoneHashList.Length; b++)
                {
                    RIG.Bone bone = rig.GetBone(geom.BoneHashList[b]);
                    boneNames.Add(bone.BoneName);
                    inverseBinds.Add((axisTransform * bone.GlobalTransform).Inverse());
                }
                mesh.jointNames = boneNames.ToArray();
                mesh.inverseBindMatrix = inverseBinds.ToArray();

                this.meshes.Add(mesh);
            }

            DAE.SkeletonJoint[] joints = new SkeletonJoint[rig.NumberBones];
            joints[0] = new SkeletonJoint(rig.Bones[0].BoneName, "", rig.Bones[0].LocalTransform, rig.Bones[0].GlobalRotation);
            for (int i = 1; i < rig.NumberBones; i++)
            {
                joints[i] = new SkeletonJoint(rig.Bones[i].BoneName, rig.Bones[i].ParentName, rig.Bones[i].LocalTransform, rig.Bones[i].GlobalRotation);
            }
            this.skeleton = new Skeleton(Matrix4D.Identity, joints);
        }

        public void Write(string path, bool flipYZ)
        {
            COLLADA dae = new COLLADA();
            dae.asset = new asset() { up_axis = (this.y_up ? UpAxisType.Y_UP : UpAxisType.Z_UP), created = DateTime.Now, modified = DateTime.Now, unit = new assetUnit() { meter = 1d, name = "meter" } };
            dae.asset.contributor = new assetContributor[] { new assetContributor() { authoring_tool = "CAS Tools" } };
            library_images images = new library_images();
            library_materials materials = new library_materials();
            library_effects effects = new library_effects();
            library_geometries geometries = new library_geometries();
            List<geometry> geometry = new List<geometry>();
            library_controllers controllers = new library_controllers();
            List<controller> controls = new List<controller>();
            List<node> sceneNodes = new List<node>();

            foreach (ColladaMesh cmesh in this.meshes)
            {
                geometry geom = new geometry();
                geom.id = cmesh.meshID + "-mesh";
                geom.name = cmesh.meshName;
                mesh daemesh = new mesh();
                daemesh.source = new source[1 + (cmesh.normals.Length > 0 ? 1 : 0) + cmesh.uvs.GetLength(0) + (cmesh.colors.Length > 0 ? 1 : 0)];

                double[] positions = new double[cmesh.positions.Length * 3];
                for (int i = 0; i < positions.Length; i += 3)
                {
                    positions[i] = cmesh.positions[i / 3].X;
                    if (flipYZ)
                    {
                        positions[i + 1] = -cmesh.positions[i / 3].Z;
                        positions[i + 2] = cmesh.positions[i / 3].Y;
                    }
                    else
                    {
                        positions[i + 1] = cmesh.positions[i / 3].Y;
                        positions[i + 2] = cmesh.positions[i / 3].Z;
                    }
                }
                double[] normals = new double[cmesh.normals.Length * 3];
                for (int i = 0; i < normals.Length; i += 3)
                {
                    normals[i] = cmesh.normals[i / 3].X;
                    if (flipYZ)
                    {
                        normals[i + 1] = -cmesh.normals[i / 3].Z;
                        normals[i + 2] = cmesh.normals[i / 3].Y;
                    }
                    else
                    {
                        normals[i + 1] = cmesh.normals[i / 3].Y;
                        normals[i + 2] = cmesh.normals[i / 3].Z;
                    }
                }
                double[][] uvs = new double[cmesh.uvs.GetLength(0)][];
                for (int j = 0; j < cmesh.uvs.GetLength(0); j++)
                {
                    double[] uv = new double[cmesh.uvs[j].Length * 2];
                    for (int i = 0; i < uv.Length; i += 2)
                    {
                        uv[i] = cmesh.uvs[j][i / 2].X;
                        uv[i + 1] = 1d - cmesh.uvs[j][i / 2].Y;
                    }
                    uvs[j] = uv;
                }
                double[] colors = new double[cmesh.colors.Length * 3];
                for (int i = 0; i < colors.Length; i += 3)
                {
                    colors[i] = ((cmesh.colors[i / 3] & 0x00FF0000) >> 16) / 255d;
                    colors[i + 1] = ((cmesh.colors[i / 3] & 0x0000FF00) >> 8) / 255d;
                    colors[i + 2] = (cmesh.colors[i / 3] & 0x000000FF) / 255d;
                }

                int index = 0;

                daemesh.source[index] = new source() { id = geom.id + "-positions" };
                daemesh.source[index].Item = new float_array() { id = geom.id + "-positions-array", count = (ulong)positions.Length, Values = positions };
                daemesh.source[index].technique_common = new sourceTechnique_common();
                daemesh.source[index].technique_common.accessor = new accessor() { source = "#" + geom.id + "-positions-array", count = (ulong)(positions.Length / 3), stride = 3 };
                param x = new param() { name = "X", type = "float" };
                param y = new param() { name = "Y", type = "float" };
                param z = new param() { name = "Z", type = "float" };
                daemesh.source[index].technique_common.accessor.param = new param[] { x, y, z };

                if (normals.Length > 0)
                {
                    index++;
                    daemesh.source[index] = new source() { id = geom.id + "-normals" };
                    daemesh.source[index].Item = new float_array() { id = geom.id + "-normals-array", count = (ulong)normals.Length, Values = normals };
                    daemesh.source[index].technique_common = new sourceTechnique_common();
                    daemesh.source[index].technique_common.accessor = new accessor() { source = "#" + geom.id + "-normals-array", count = (ulong)(normals.Length / 3), stride = 3 };
                    daemesh.source[index].technique_common.accessor.param = new param[] { x, y, z };
                }

                for (int j = 0; j < uvs.GetLength(0); j++)
                {
                    if (uvs[j].Length > 0)
                    {
                        index++;
                        daemesh.source[index] = new source() { id = geom.id + "-map-" + j.ToString() };
                        daemesh.source[index].Item = new float_array() { id = geom.id + "-map-" + j.ToString() + "-array", count = (ulong)uvs[j].Length, Values = uvs[j] };
                        daemesh.source[index].technique_common = new sourceTechnique_common();
                        daemesh.source[index].technique_common.accessor = new accessor() { source = "#" + geom.id + "-map-" + j.ToString() + "-array", count = (ulong)(uvs[j].Length / 2), stride = 2 };
                        param u = new param() { name = "S", type = "float" };
                        param v = new param() { name = "T", type = "float" };
                        daemesh.source[index].technique_common.accessor.param = new param[] { u, v };
                    }
                }

                if (colors.Length > 0)
                {
                    index++;
                    param r = new param() { name = "R", type = "float" };
                    param g = new param() { name = "G", type = "float" };
                    param b = new param() { name = "B", type = "float" };
                    daemesh.source[index] = new source() { id = geom.id + "-colors" };
                    daemesh.source[index].Item = new float_array() { id = geom.id + "-colors-array", count = (ulong)colors.Length, Values = colors };
                    daemesh.source[index].technique_common = new sourceTechnique_common();
                    daemesh.source[index].technique_common.accessor = new accessor() { source = "#" + geom.id + "-colors-array", count = (ulong)(colors.Length / 3), stride = 3 };
                    daemesh.source[index].technique_common.accessor.param = new param[] { r, g, b };
                }

                daemesh.vertices = new vertices() { id = geom.id + "_vertices", input = new InputLocal[] { new InputLocal() { semantic = "POSITION", source = "#" + geom.id + "-positions" } } };

                polylist poly = new polylist() { count = (ulong)cmesh.TotalFaces };
                InputLocalOffset[] polyInputs = new InputLocalOffset[1 + (normals.Length > 0 ? 1 : 0) + uvs.GetLength(0) + (colors.Length > 0 ? 1 : 0)];
                index = 0;
                polyInputs[index] = new InputLocalOffset() { semantic = "VERTEX", source = "#" + geom.id + "_vertices", offset = (ulong)cmesh.offsets.positionOffset };
                if (normals.Length > 0)
                {
                    index++;
                    polyInputs[index] = new InputLocalOffset() { semantic = "NORMAL", source = "#" + geom.id + "-normals", offset = (ulong)cmesh.offsets.normalsOffset };
                }
                for (int j = 0; j < uvs.GetLength(0); j++)
                {
                    index++;
                    polyInputs[index] = new InputLocalOffset() { semantic = "TEXCOORD", source = "#" + geom.id + "-map-" + j.ToString(), offset = (ulong)cmesh.offsets.uvOffset[j], set = (ulong)j, setSpecified = true };
                }
                if (colors.Length > 0)
                {
                    index++;
                    polyInputs[index] = new InputLocalOffset() { semantic = "COLOR", source = "#" + geom.id + "-colors", offset = (ulong)cmesh.offsets.colorOffset };
                }

                poly.input = polyInputs;
                string vcount = "";
                for (int i = 0; i < cmesh.TotalFaces; i++) vcount += "3 ";
                poly.vcount = vcount;
                string[] facePoints = new string[cmesh.facePoints.Length];
                for (int i = 0; i < cmesh.facePoints.Length; i++)
                {
                    facePoints[i] = cmesh.facePoints[i].ToString();
                }
                string p = String.Join(" ", facePoints);
                poly.p = p;

                daemesh.Items = new object[] { poly };

                geom.Item = daemesh;
                geometry.Add(geom);

                if (cmesh.HasBones)
                {
                    Name_array jointNames = new Name_array() { id = geom.id + "-joints-array", count = (ulong)cmesh.jointNames.Length, Values = cmesh.jointNames };
                    accessor access1 = new accessor()
                    {
                        count = (ulong)cmesh.jointNames.Length,
                        stride = 1ul,
                        source = "#" + geom.id + "-joints-array",
                        param = new param[] { new param() { name = "JOINT", type = "name" } }
                    };
                    source jointsSource = new source() { id = geom.id + "-joints", Item = jointNames, 
                        technique_common = new sourceTechnique_common() { accessor = access1 } };

                    List<double> bindArray = new List<double>();
                    for (int i = 0; i < cmesh.inverseBindMatrix.Length; i++) bindArray.AddRange(cmesh.inverseBindMatrix[i].Values);
                    float_array jointBindPoses = new float_array() { id = geom.id + "-bind_poses-array", count = (ulong)bindArray.Count, Values = bindArray.ToArray() };
                    accessor access2 = new accessor()
                    {
                        count = (ulong)cmesh.inverseBindMatrix.Length,
                        stride = 16ul,
                        source = "#" + geom.id + "-bind_poses-array",
                        param = new param[] { new param() { name = "TRANSFORM", type = "float4x4" } }
                    };
                    source bindSource = new source()
                    {
                        id = geom.id + "-bind_poses",
                        Item = jointBindPoses,
                        technique_common = new sourceTechnique_common() { accessor = access2 }
                    };

                    List<ulong> vcountList = new List<ulong>();
                    List<ulong> assignmentsList = new List<ulong>();
                    List<double> weightsList = new List<double>();
                    for (int i = 0; i < cmesh.bones.Length; i++)
                    {
                        ulong[] assignments = cmesh.bones[i].AssignmentsForCollada;
                        vcountList.Add((ulong)assignments.Length);
                        assignmentsList.AddRange(assignments);
                        weightsList.AddRange(cmesh.bones[i].WeightsForCollada);
                    }
                    float_array weights = new float_array() { id = geom.id + "-skin-weights-array", count = (ulong)weightsList.Count, Values = weightsList.ToArray() };
                    accessor access3 = new accessor()
                    {
                        count = (ulong)weightsList.Count,
                        stride = 1ul,
                        source = "#" + geom.id + "-skin-weights-array",
                        param = new param[] { new param() { name = "WEIGHT", type = "float" } }
                    };
                    source weightSource = new source()
                    {
                        id = geom.id + "-skin-weights",
                        Item = weights,
                        technique_common = new sourceTechnique_common() { accessor = access3 }
                    };

                    skinJoints skinjoints = new skinJoints()
                    {
                        input = new InputLocal[] { new InputLocal() { source = "#" + geom.id + "-joints", semantic = "JOINT" }, 
                                                   new InputLocal() { source = "#" + geom.id + "-bind_poses", semantic = "INV_BIND_MATRIX" } }
                    };

                    string vbcount = "";
                    for (int i = 0; i < vcountList.Count; i++) vbcount += vcountList[i].ToString() + " ";
                    string vb = "";
                    for (int i = 0; i < assignmentsList.Count; i++) vb += assignmentsList[i].ToString() + " " + i.ToString() + " ";
                    skinVertex_weights skinWeights = new skinVertex_weights()
                    {
                        count = (ulong)vcountList.Count,
                        input = new InputLocalOffset[] { new InputLocalOffset() { source = "#" + geom.id + "-joints", semantic = "JOINT", offset = 0 }, 
                                                         new InputLocalOffset() { source = "#" + geom.id + "-skin-weights", semantic = "WEIGHT", offset = 1 } },
                        vcount = vbcount,
                        v = vb
                    };

                    skin rigSkin = new skin()
                    {
                        source1 = "#" + geom.id,
                        bind_shape_matrix = cmesh.bindShapeMatrix.ToUnpunctuatedString(),
                        source = new source[] { jointsSource, bindSource, weightSource},
                        joints = skinjoints,
                        vertex_weights = skinWeights
                    };

                    controller control = new controller() { id = geom.id + "-skin", name = geom.id + "-rig", Item = rigSkin };
                    controls.Add(control);
                }

                if (cmesh.HasBones && this.skeleton.joints.Length > 0)
                {
                    node sceneNode = new node()
                    {
                        id = cmesh.meshID,
                        name = cmesh.meshID,
                        type = NodeType.NODE,
                        instance_controller = new instance_controller[] { new instance_controller() { url = "#" + geom.id + "-skin", 
                            skeleton = new string[] { "#" + this.skeleton.joints[0].jointName } } }
                    };
                    sceneNodes.Add(sceneNode);
                }
                else
                {
                    node sceneNode = new node()
                    {
                        id = cmesh.meshID,
                        name = cmesh.meshID,
                        type = NodeType.NODE,
                        instance_geometry = new instance_geometry[] { new instance_geometry() { url = "#" + cmesh.meshID + "-mesh" } }
                    };
                    sceneNodes.Add(sceneNode);
                }
            }

            geometries.geometry = geometry.ToArray();
            controllers.controller = controls.ToArray();
            if (this.skeleton != null && this.skeleton.joints.Length > 0)
            {
                node rigNode = new node()
                {
                    id = "rig",
                    name = "rig",
                    type = NodeType.NODE,
                    Items = new object[] { new matrix() { Values = this.skeleton.transform.Values } },
                    ItemsElementName = new ItemsChoiceType2[] { ItemsChoiceType2.matrix }
                };
                technique blender = new technique() { profile = "blender" };
                rigNode.node1 = new node[] { WriteSkeletonJoint(this.skeleton.joints[0], this.skeleton.joints) };
                sceneNodes.Add(rigNode);
            }
            visual_scene visualScene = new visual_scene() { id = "RootScene", name = "RootScene", node = sceneNodes.ToArray() };
            library_visual_scenes scenes = new library_visual_scenes() { visual_scene = new visual_scene[] { visualScene } };
            dae.Items = new object[] { images, materials, effects, geometries, controllers, scenes };
            dae.scene = new COLLADAScene() { instance_visual_scene = new InstanceWithExtra() { url = "#RootScene" } };
            dae.Save(path);
        }

        internal node WriteSkeletonJoint(SkeletonJoint joint, SkeletonJoint[] skeleton)
        {
            node jointNode = new node()
            {
                id = joint.jointName,
                name = joint.jointName,
                sid = joint.jointName,
                type = NodeType.JOINT,
                Items = new object[] { new matrix() { Values = joint.localTransform.Values } },
                ItemsElementName = new ItemsChoiceType2[] { ItemsChoiceType2.matrix },
            };
            if (joint.globalRotation != null)
            {
                AxisAngle aa = mat3_to_vec_roll(joint.globalRotation.toMatrix3D());
                string roll = aa.Angle.ToString(System.Globalization.CultureInfo.InvariantCulture);
                string x = (aa.X / 200.0).ToString(System.Globalization.CultureInfo.InvariantCulture);
                string y = (aa.Y / 200.0).ToString(System.Globalization.CultureInfo.InvariantCulture);
                string z = (aa.Z / 200.0).ToString(System.Globalization.CultureInfo.InvariantCulture);
                technique[] tech = new technique[] { new technique() { profile = "blender", layer = "0", roll = roll, tip_x = x, tip_y = y, tip_z = z } };
                extra[] ex = new extra[] { new extra() { technique = tech } };
                jointNode.extra = ex;
            }            
            List<node> children = new List<node>();
            foreach (SkeletonJoint j in skeleton)
            {
                if (String.Compare(j.parentName, joint.jointName) == 0 )
                {
                    children.Add(WriteSkeletonJoint(j, skeleton));
                }
            }
            jointNode.node1 = children.Count > 0 ? children.ToArray() : null;
            return jointNode;
        }

        //port of the updated C function from armature.c
        //https://developer.blender.org/T39470
        //note that C accesses columns first, so all matrix indices are swapped compared to the C version
        public static Matrix3D vec_roll_to_mat3(Vector3 vec, float roll)
        {
            vec.Normalize();
            float[] nor = vec.Coordinates;
            double THETA_THRESHOLD_NEGY = 1.0e-9;
            double THETA_THRESHOLD_NEGY_CLOSE = 1.0e-5;

            // create a 3x3 matrix
            float[,] bMatrix = new float[3, 3];

            float theta = 1.0f + nor[1];
            float test = nor[0] > 0f ? nor[0] : nor[2];
            float test2 = test > 0f ? theta : test;
            if ((theta > THETA_THRESHOLD_NEGY_CLOSE) || (test2 > THETA_THRESHOLD_NEGY))
            {
                bMatrix[1, 0] = -nor[0];
                bMatrix[0, 1] = nor[0];
                bMatrix[1, 1] = nor[1];
                bMatrix[2, 1] = nor[2];
                bMatrix[1, 2] = -nor[2];
                if (theta > THETA_THRESHOLD_NEGY_CLOSE)
                {
                    //If nor is far enough from -Y, apply the general case.
                    bMatrix[0, 0] = 1 - nor[0] * nor[0] / theta;
                    bMatrix[2, 2] = 1 - nor[2] * nor[2] / theta;
                    bMatrix[0, 2] = bMatrix[2, 0] = -nor[0] * nor[2] / theta;
                }
                else
                {
                    // If nor is too close to -Y, apply the special case.
                    theta = nor[0] * nor[0] + nor[2] * nor[2];
                    bMatrix[0, 0] = (nor[0] + nor[2]) * (nor[0] - nor[2]) / -theta;
                    bMatrix[2, 2] = -bMatrix[0, 0];
                    bMatrix[0, 2] = bMatrix[2, 0] = 2.0f * nor[0] * nor[2] / theta;
                }
            }
            else
            {
                // If nor is -Y, simple symmetry by Z axis.
                bMatrix[0, 0] = bMatrix[1, 1] = -1.0f;
            }

            // Make Roll matrix
            AxisAngle aa = new AxisAngle(roll, nor);
            Matrix3D rMatrix = aa.ToMatrix();

            // Combine and output result
            Matrix3D mat = rMatrix * new Matrix3D(bMatrix);
            return mat;
        }
        
        public static AxisAngle mat3_to_vec_roll(Matrix3D mat)
        {
            // this hasn't changed
            float[,] matrix = mat.Matrix;
            Vector3 vec = new Vector3(matrix[0, 1], matrix[1, 1], matrix[2, 1]);
            Matrix3D vecmat = vec_roll_to_mat3(vec, 0f);
            Matrix3D vecmatinv = vecmat.Inverse();
            Matrix3D rollmatrix = vecmatinv * mat;
            float[,] rollmat = rollmatrix.Matrix;
            float roll = (float)Math.Atan2(rollmat[0, 2], rollmat[2, 2]);
            if (roll == float.NaN) roll = 0;
            return new AxisAngle(roll, vec);
        }
        
        public class ColladaMesh
        {
            public string meshID;
            public string meshName;
            public string controllerID;
            public Vector3[] positions;
            public Vector3[] normals;
            public Vector2[][] uvs;
            public uint[] colors;

            public Offsets offsets;

            public uint[] facePoints;

            public Matrix4D sceneGeomMatrix = Matrix4D.Identity;
            public Matrix4D sceneControllerMatrix = Matrix4D.Identity;
            public Matrix4D bindShapeMatrix = Matrix4D.Identity;
            public string[] jointNames;
            public Matrix4D[] inverseBindMatrix;
            public BoneAssignment[] bones;

            public int MaxOffset
            {
                get { return offsets.MaxOffset; }
            }

            public int Stride
            {
                get { return this.MaxOffset + 1; }
            }

            public int TotalFaces
            {
                get { return this.facePoints.Length / this.Stride / 3; }
            }

            public bool HasUV1
            {
                get { return this.uvs != null && this.uvs.GetLength(0) > 1; }
            }

            public bool HasBones
            {
                get { return this.bones != null && this.bones.Length > 0; }
            }

            public bool HasColors
            {
                get { return this.colors != null && this.colors.Length > 0; }
            }

            /// <summary>
            /// Prevents normals edges caused by uv seams. Should be used only when converting TS4 meshes.
            /// </summary>
            public void Clean()
            {
                if (this.normals == null || this.normals.Length == 0) return;
                uint[] trans = new uint[this.positions.Length];
                List<Vector3> newPositions = new List<Vector3>();
                List<Vector3> newNormals = new List<Vector3>();
                List<BoneAssignment> newBones = new List<BoneAssignment>();
                for (uint i = 0; i < this.positions.Length; i++)
                {
                    int posIndex = newPositions.IndexOf(this.positions[i]);
                    int normIndex = newNormals.IndexOf(this.normals[i]);
                    if ((posIndex < 0 || normIndex < 0) || (posIndex != normIndex))
                    {
                        trans[i] = (uint)newPositions.Count;
                        newPositions.Add(this.positions[i]);
                        newNormals.Add(this.normals[i]);
                        if (this.HasBones) newBones.Add(this.bones[i]);
                    }
                    else
                    {
                        trans[i] = (uint)posIndex;
                    }
                    //for (uint j = 0; j < i; j++)
                    //{
                    //    if (this.positions[i].Equals(this.positions[j]) && this.normals[i].Equals(this.normals[j]))
                    //    {
                    //        trans[i] = j;
                    //        break;
                    //    }
                    //    trans[i] = i;
                    //}
                }
                this.positions = newPositions.ToArray();
                this.normals = newNormals.ToArray();
                if (this.HasBones) this.bones = newBones.ToArray();
                for (int i = 0; i < this.facePoints.Length; i += this.Stride)
                {
                    int posIndex = i + this.offsets.positionOffset;
                    int normIndex = i + this.offsets.normalsOffset;
                    this.facePoints[posIndex] = trans[this.facePoints[posIndex]];
                    this.facePoints[normIndex] = trans[this.facePoints[normIndex]];
                }
            }
        }

        public class Skeleton
        {
            public Matrix4D transform;
            public SkeletonJoint[] joints;
            public Skeleton(Matrix4D transform, SkeletonJoint[] skeletonJoints)
            {
                this.transform = transform;
                this.joints = skeletonJoints;
            }
        }

        public class SkeletonJoint
        {
            public string jointName;
            public string parentName;
            public Matrix4D localTransform;
            public Quaternion globalRotation;
            public SkeletonJoint(string jointName, string parentName, Matrix4D localTransform)
            {
                this.jointName = jointName;
                this.parentName = parentName;
                this.localTransform = localTransform;
                this.globalRotation = null;
            }
            public SkeletonJoint(string jointName, string parentName, Matrix4D localTransform, Quaternion globalRotation)
            {
                this.jointName = jointName;
                this.parentName = parentName;
                this.localTransform = localTransform;
                this.globalRotation = globalRotation;
            }
        }

        public class BoneAssignment
        {
            public int[] assignments;
            public float[] weights;

            public BoneAssignment()
            {
                assignments = new int[4];
                weights = new float[4];
            }

            public BoneAssignment(int[] assignments, float[] weights)
            {
                this.assignments = assignments;
                this.weights = weights;
            }

            public BoneAssignment(byte[] assignments, byte[] weights)
            {
                this.assignments = new int[4];
                this.weights = new float[4];
                for (int i = 0; i < 4; i++)
                {
                    if (weights[i] > 0)
                    {
                        this.assignments[i] = assignments[i];
                        this.weights[i] = weights[i] / 255f;
                    }
                    else
                    {
                        this.assignments[i] = -1;
                        this.weights[i] = 0;
                    }
                }
            }

            public ulong[] AssignmentsForCollada
            {
                get
                {
                    List<ulong> w = new List<ulong>();
                    for (int i = 0; i < 4; i++) if (this.assignments[i] >= 0) w.Add((ulong)this.assignments[i]);
                    return w.ToArray();
                }
            }

            public double[] WeightsForCollada
            {
                get
                {
                    List<double> w = new List<double>();
                    for (int i = 0; i < 4; i++) if (this.assignments[i] >= 0) w.Add(this.weights[i]);
                    return w.ToArray();
                }
            }
        }

        public class Offsets
        {
            public int positionOffset;
            public int normalsOffset;
            public int[] uvOffset;
            public int colorOffset;

            public Offsets()
            {
                uvOffset = new int[] { 0 };
            }

            public Offsets(int positionOffset, int normalsOffset, int[] uvOffset, int colorOffset)
            {
                this.positionOffset = positionOffset;
                this.normalsOffset = normalsOffset;
                this.uvOffset = uvOffset;
                this.colorOffset = colorOffset;
            }

            internal int MaxOffset
            {
                get 
                { 
                    int tmp1 = Math.Max(positionOffset, normalsOffset);
                    int tmp2 = Math.Max(uvOffset.Max(), colorOffset);
                    return Math.Max(tmp1, tmp2);
                }
            }
        }
    }
}
