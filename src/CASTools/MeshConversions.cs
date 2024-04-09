/* TS4 CAS Mesh Tools, a tool for creating custom content for The Sims 4,
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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using Xmods.DataLib;
using s4pi.Interfaces;
using s4pi.Package;

namespace XMODS
{
    public partial class Form1 : Form
    {
        private void S4fileToConvert_button_Click(object sender, EventArgs e)
        {
            S4fileToConvert.Text = GetFilename("Select TS4 GEOM mesh to convert", GEOMfilter);
        }

        private void S4toS3go_button_Click(object sender, EventArgs e)
        {
            GEOM myGeom = ValidTS4Mesh(S4fileToConvert.Text, "You must select a TS4 GEOM to convert!");
            if (myGeom == null) return;
            GEOM S3geom = ConvertS4toS3(myGeom, 0);
            string tmp = WriteGEOMFile("Save converted mesh", S3geom, "");
            S4fileOriginal.Text = S4fileToConvert.Text;
        }

        private void S4toMS3Dgo_button_Click(object sender, EventArgs e)
        {
            GEOM[] myGeom = new GEOM[] { ValidTS4Mesh(S4fileToConvert.Text, "Only a TS4 GEOM can be converted to MS3D!") };
            if (myGeom[0] == null) return;
            XmodsEnums.Age age;
            if (S4toMS3Dinfant_radioButton.Checked) age = XmodsEnums.Age.Infant;
            else if (S4toMS3Dtoddler_radioButton.Checked) age = XmodsEnums.Age.Toddler;
            else if (S4toMS3Dchild_radioButton.Checked) age = XmodsEnums.Age.Child;
            else age = XmodsEnums.Age.Adult;
            XmodsEnums.Species species;
            if (S4toMS3Dcat_radioButton.Checked) species = XmodsEnums.Species.Cat;
            else if (S4toMS3Ddog_radioButton.Checked) species = XmodsEnums.Species.Dog;
            else if (S4toMS3DlittleDog_radioButton.Checked) species = XmodsEnums.Species.LittleDog;
            else species = XmodsEnums.Species.Human;
            RIG rig = SelectRig(species, age);
            if (rig == null) return;
            try
            {
                MS3D ms3d = new MS3D(myGeom, null, rig, 0);
                string tmp = WriteMS3DFile("Save converted mesh", ms3d, "");
            }
            catch (MS3D.MeshException em)
            {
                MessageBox.Show(em.Message);
                return;
            }
        }

        private void S4toDAEgo_button_Click(object sender, EventArgs e)
        {
            GEOM[] myGeom = new GEOM[] { ValidTS4Mesh(S4fileToConvert.Text, "Only a TS4 GEOM can be converted to MS3D!") };
            if (myGeom[0] == null) return;
            XmodsEnums.Age age;
            if (S4toMS3Dinfant_radioButton.Checked) age = XmodsEnums.Age.Infant;
            else if (S4toMS3Dtoddler_radioButton.Checked) age = XmodsEnums.Age.Toddler;
            else if (S4toMS3Dchild_radioButton.Checked) age = XmodsEnums.Age.Child;
            else age = XmodsEnums.Age.Adult;
            XmodsEnums.Species species;
            if (S4toMS3Dcat_radioButton.Checked) species = XmodsEnums.Species.Cat;
            else if (S4toMS3Ddog_radioButton.Checked) species = XmodsEnums.Species.Dog;
            else if (S4toMS3DlittleDog_radioButton.Checked) species = XmodsEnums.Species.LittleDog;
            else species = XmodsEnums.Species.Human;
            RIG rig = SelectRig(species, age);
            if (rig == null) return;
            try
            {
                DAE dae = new DAE(myGeom, null, rig, false);
                string tmp = WriteDAEFile("Save converted mesh", dae, false, "");
            }
            catch (Exception ed)
            {
                MessageBox.Show(ed.Message + Environment.NewLine + ed.StackTrace);
                return;
            }
        }

        private void S4toOBJgo_button_Click(object sender, EventArgs e)
        {
            GEOM[] myGeom = new GEOM[] { ValidTSMesh(S4fileToConvert.Text, "You must select a TS3 or TS4 GEOM to convert!") };
            if (myGeom[0] == null) return;
            OBJ obj = new OBJ(myGeom, null, 0);
            string tmp = WriteOBJFile("Save converted mesh", obj, "");
        }

        private GEOM ValidTS4Mesh(string filename, string noFileMessage)
        {
            if (String.Compare(filename, " ") <= 0)
            {
                MessageBox.Show(noFileMessage);
                return null;
            }
            GEOM myGeom = new GEOM();
            if (!GetGEOMData(filename, out myGeom))
            {
                MessageBox.Show("Can't read GEOM file!");
                return null;
            }
            if (!myGeom.isTS4)
            {
                MessageBox.Show("This is not a TS4 mesh!");
                return null;
            }
            return myGeom;
        }

        private GEOM ValidTSMesh(string filename, string noFileMessage)
        {
            if (String.Compare(filename, " ") <= 0)
            {
                MessageBox.Show(noFileMessage);
                return null;
            }
            GEOM myGeom = new GEOM();
            if (!GetGEOMData(filename, out myGeom))
            {
                MessageBox.Show("Can't read GEOM file!");
                return null;
            }
            if (!((myGeom.meshVersion == 5) | myGeom.isTS4))
            {
                MessageBox.Show("This is not a Sims mesh!");
                return null;
            }
            return myGeom;
        }

        private void ModMeshToConvert_button_Click(object sender, EventArgs e)
        {
            ModMeshToConvert.Text = GetFilename("Select a modified mesh to convert back to TS4", ConvertMeshImportFilter);
        }

        private void S4original_button_Click(object sender, EventArgs e)
        {
            S4fileOriginal.Text = GetFilename("Select original TS4 GEOM mesh", GEOMfilter);
        }

        private void ModMeshtoS4go_button_Click(object sender, EventArgs e)
        {
            if (String.Compare(ModMeshToConvert.Text, " ") <= 0)
            {
                MessageBox.Show("You must select a modified mesh to convert back to TS4!");
                return;
            }
            GEOM origGeom = ValidTS4Mesh(S4fileOriginal.Text, "You must select the original TS4 GEOM as reference!");
            if (origGeom == null) return;

            GEOM modMesh = new GEOM();
            XmodsEnums.Species species;
            if (ConvertRefDog_radioButton.Checked) species = XmodsEnums.Species.Dog;
            else if (ConvertRefCat_radioButton.Checked) species = XmodsEnums.Species.Cat;
            else if (ConvertRefLittleDog_radioButton.Checked) species = XmodsEnums.Species.LittleDog;
            else species = XmodsEnums.Species.Human;
            XmodsEnums.Age age;
            if (ConvertRefAdult_radioButton.Checked) age = XmodsEnums.Age.Adult;
            else if (ConvertRefChild_radioButton.Checked) age = XmodsEnums.Age.Child;
            else if (ConvertRefToddler_radioButton.Checked) age = XmodsEnums.Age.Toddler;
            else age = XmodsEnums.Age.Infant;
            XmodsEnums.Gender gender;
            if (ConvertRefMale_radioButton.Checked) gender = XmodsEnums.Gender.Male;
            else if (ConvertRefFemale_radioButton.Checked) gender = XmodsEnums.Gender.Female;
            else gender = XmodsEnums.Gender.Unisex;
            int lod = ConvertRefLOD_comboBox.SelectedIndex;
            XmodsEnums.BodyType partType = XmodsEnums.BodyType.All;
            if (ConvertRefPartType_comboBox.SelectedIndex == 0)
            {
                partType = XmodsEnums.BodyType.Top;
            }
            else if (ConvertRefPartType_comboBox.SelectedIndex == 1)
            {
                partType = XmodsEnums.BodyType.Bottom;
            }
            else
            {
                partType = XmodsEnums.BodyType.Body;
            }

            if (String.Compare(Path.GetExtension(ModMeshToConvert.Text), ".ms3d", true) == 0)
            {
                GEOM[] geoms = new GEOM[0];
                MS3D ms3d = null;
                if (GetMS3DData(ModMeshToConvert.Text, out ms3d, true))
                {
                    GEOM refMesh = GetBodyMesh(species, age, gender, 0, partType, XmodsEnums.BodySubType.None, false);
                    geoms = GEOM.GEOMsFromMS3D(ms3d, refMesh, null);
                    if (geoms.Length > 1)
                    {
                        MessageBox.Show("There is more than one mesh group in this MS3D mesh; they will be converted to separate meshes!");
                    }
                    for (int i = 0; i < geoms.Length; i++)
                    {
                        geoms[i] = ApplyOriginalMesh(geoms[i], origGeom, false, species, age, gender, lod, partType);
                        if (geoms[i] != null)
                        {
                            string grp = ms3d.getGroupName(i);
                            string tmp = WriteGEOMFile("Save converted mesh: " + grp, geoms[i], "");
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Can't read MS3D mesh!");
                }
            }

            else if (String.Compare(Path.GetExtension(ModMeshToConvert.Text), ".geom", true) == 0 |
                         String.Compare(Path.GetExtension(ModMeshToConvert.Text), ".simgeom", true) == 0)
            {
                if (!GetGEOMData(ModMeshToConvert.Text, out modMesh))
                {
                    MessageBox.Show("Can't read TS3 GEOM file!");
                    return;
                }
                if (!(modMesh.meshVersion == 5) & modMesh.isBase)
                {
                    MessageBox.Show("The TS3 file is not a TS3 base mesh!");
                    return;
                }

                modMesh = ConvertS3toS4(modMesh);
                GEOM geom = ApplyOriginalMesh(modMesh, origGeom, true, species, age, gender, lod, partType);
                if (geom != null) WriteGEOMFile("Save converted mesh", modMesh, "");
            }
        }

     /*   private void S3FiletoConvertNoRef_button_Click(object sender, EventArgs e)
        {
            S3FiletoConvertNoRef.Text = GetFilename("Select TS3 mesh to be converted", GEOMfilter);
        }

        private void S3toS4NoRefgo_button_Click(object sender, EventArgs e)
        {
            if (String.Compare(S3FiletoConvertNoRef.Text, " ") <= 0)
            {
                MessageBox.Show("You must select an S3 GEOM to convert!");
                return;
            }
            GEOM S3 = new GEOM();
            if (!GetGEOMData(S3FiletoConvertNoRef.Text, out S3))
            {
                MessageBox.Show("Can't read S3 GEOM file!");
                return;
            }
            if (!(S3.meshVersion == 5) & S3.isBase)
            {
                MessageBox.Show("The S3 file is not an S3 base mesh!");
                return;
            }
            GEOM g = new GEOM();
            g = ConvertS3toS4(S3);
            string tmp = WriteGEOMFile("Save converted mesh", g, "");
        } */

        private void S4AppendFileOne_button_Click(object sender, EventArgs e)
        {
            S4AppendFileOne.Text = GetFilename("Select TS4 mesh one", GEOMfilter);
        }

        private void S4AppendFileTwo_button_Click(object sender, EventArgs e)
        {
            S4AppendFileTwo.Text = GetFilename("Select TS4 mesh two", GEOMfilter);
        }

        private void S4AppendGo_button_Click(object sender, EventArgs e)
        {
            GEOM g1 = ValidTS4Mesh(S4AppendFileOne.Text, "You must select a file to append to!");
            if (g1 == null) return;
            GEOM g2 = ValidTS4Mesh(S4AppendFileTwo.Text, "You must select a file to be appended!");
            if (g2 == null) return;
            if (!(g1.isTS4 & g2.isTS4))
            {
                MessageBox.Show("Both meshes must be TS4 format!");
                return;
            }
            g1.AppendMesh(g2, adultRig);
            g1.BoneFixer(3);
            if (g1.numberBones > 60)
            {
                DialogResult res = MessageBox.Show("The combined mesh has two many bones and will be distorted in-game." +
                    Environment.NewLine + "Save anyway?", "Too many bones", MessageBoxButtons.OKCancel);
                if (res == DialogResult.Cancel) return;
            }
            string tmp = WriteGEOMFile("Save converted mesh", g1, "");
        }

        internal GEOM ConvertS4toS3(GEOM S4geom, int uvSet)
        {
            GEOM newgeom = new GEOM(S4geom);
            newgeom.SetVersion(5, adultRig);
            if (uvSet > 0)
            {
                if (newgeom.numberUVsets <= uvSet)
                {
                    MessageBox.Show("This mesh does not have a UV1. Exporting with UV0.");
                }
                else
                {
                    newgeom.MakeUVset0(uvSet);
                }
            }
            newgeom.RemoveMorphUV();
            newgeom.NumberSequential(0);
            return newgeom;
        }

     /*   internal GEOM ConvertS3toS4(GEOM S3geom, GEOM S4refgeom)
        {
            GEOM newgeom = new GEOM(S3geom);
            newgeom.SetVersion(S4refgeom.meshVersion);
            newgeom.MatchFormats(S4refgeom.vertexFormat);
            newgeom.setBoneHashList(S4refgeom.BoneHashList);
            List<int> vertList = new List<int>();
            for (int i = 0; i < newgeom.numberVertices; i++)
            {
                for (int j = 1; j < S4refgeom.numberUVsets; j++)
                {
                    newgeom.setUV(i, j, S4refgeom.getUV(newgeom.getVertexID(i), j));
                }
                newgeom.setBones(i, S4refgeom.getBones(newgeom.getVertexID(i)));
                newgeom.setBoneWeights(i, S4refgeom.getBoneWeights(newgeom.getVertexID(i)));
                vertList.Add(newgeom.getVertexID(i));             // vertex IDs are the original vertex indexes
            }
            List<GEOM.UVStitch> newAdjusts = new List<GEOM.UVStitch>();
            for (int i = 0; i < S4refgeom.UVStitches.Length; i++)
            {
                for (int j = 0; j < vertList.Count; j++)
                {
                    if (vertList[j] == S4refgeom.UVStitches[i].Index)
                    {
                        GEOM.UVStitch adj = new GEOM.UVStitch(S4refgeom.UVStitches[i]);
                        adj.Index = j;
                        newAdjusts.Add(adj);
                        break;
                    }
                }
            }
            newgeom.UVStitches = newAdjusts.ToArray();

            if (S4refgeom.meshVersion >= 13)
            {
                List<GEOM.SeamStitch> newSeams = new List<GEOM.SeamStitch>();
                for (int i = 0; i < S4refgeom.SeamStitches.Length; i++)
                {
                    for (int j = 0; j < vertList.Count; j++)
                    {
                        if (vertList[j] == S4refgeom.SeamStitches[i].Index)
                        {
                            GEOM.SeamStitch adj = new GEOM.SeamStitch(S4refgeom.SeamStitches[i]);
                            adj.Index = (uint)j;
                            newSeams.Add(adj);
                            break;
                        }
                    }
                }
                newgeom.SeamStitches = newSeams.ToArray();
            } 

            List<GEOM.SlotrayIntersection> newAdjusts2 = new List<GEOM.SlotrayIntersection>();
            for (int i = 0; i < S4refgeom.slotrayAdjustments.Length; i++)
            {
                int[] fa = S4refgeom.slotrayAdjustments[i].TrianglePointIndices;
                for (int j = 0; j < newgeom.numberFaces; j++)
                {
                    int[] f = newgeom.getFaceIndices(j);
                    int[] fo = new int[] { vertList[f[0]], vertList[f[1]], vertList[f[2]] };
                    if ((fa[0] == fo[0]) & (fa[1] == fo[1]) & (fa[2] == fo[2]))
                    {
                        GEOM.SlotrayIntersection adj = new GEOM.SlotrayIntersection(S4refgeom.slotrayAdjustments[i]);
                        adj.TrianglePointIndices = f;
                        newAdjusts2.Add(adj);
                        break;
                    }
                }
            }
            newgeom.slotrayAdjustments = newAdjusts2.ToArray();
            newgeom.BoneFixer(3);
            newgeom.CalculateTangents();
            return newgeom;
        } */

        internal GEOM ConvertS3toS4(GEOM S3geom)
        {
            GEOM newgeom = new GEOM(S3geom);
            GEOM.vertexForm[] V12format = new GEOM.vertexForm[] { new GEOM.vertexForm(1, 1, (byte)12), new GEOM.vertexForm(2, 1, (byte)12), 
                new GEOM.vertexForm(3, 1, (byte)8), new GEOM.vertexForm(7, 3, (byte)4), 
                new GEOM.vertexForm(4, 2, (byte)4), new GEOM.vertexForm(5, 2, (byte)4), new GEOM.vertexForm(6, 1, (byte)12) };
            newgeom.MatchFormats(V12format);
            if (newgeom.Shader == null)
            {
                newgeom.setShader(XmodsEnums.Shader.SimSkin);
                newgeom.TGIList = new TGI[] { new TGI("0x00B2D882-0x00000000-0x904A5BDBF4047CF7"), 
                    new TGI("0x00B2D882-0x00000000-0xEE762E0E70DD44E3") };
            }
            newgeom.UpdateToLatestVersion(adultRig);
            newgeom.BoneFixer(3);
            newgeom.CalculateTangents(true);
            newgeom.UVStitches = new GEOM.UVStitch[0];
            newgeom.SeamStitches = new GEOM.SeamStitch[0];
            newgeom.SlotrayAdjustments = new GEOM.SlotrayIntersection[0];
            return newgeom;
        }

        private GEOM ApplyOriginalMesh(GEOM sourceMesh, GEOM referenceMesh, bool oldBones, 
            XmodsEnums.Species species, XmodsEnums.Age age, XmodsEnums.Gender gender, int lod, XmodsEnums.BodyType partType)
        {
            if (lod < 0 || lod > 3)
            {
                MessageBox.Show("You must select a valid lod number!");
                return null;
            }
            if (!Enum.IsDefined(typeof(XmodsEnums.BodyType), partType))
            {
                MessageBox.Show("You must select a valid part type!");
                return null;
            }
            GEOM newgeom = sourceMesh;
            GEOM S4refgeom = referenceMesh;
            newgeom.MatchFormats(S4refgeom.vertexFormat);
            List<int> vertList = new List<int>();
            for (int i = 0; i < newgeom.numberVertices; i++)
            {
                int refIndex = 0;
                if (S4refgeom.hasVertexIDs)
                {
                    for (int j = 0; j < S4refgeom.numberVertices; j++)      // find matching reference vertex ID
                    {
                        if (S4refgeom.getVertexID(j) == newgeom.getVertexID(i)) 
                        {
                            refIndex = j;
                            break;
                        }
                    }
                }
                else
                {
                    refIndex = newgeom.getVertexID(i);   // for original meshes with no vertex ID, the ms3d was assigned IDs sequentially
                }
                for (int j = 1; j < S4refgeom.numberUVsets; j++)
                {
                    newgeom.setUV(i, j, S4refgeom.getUV(refIndex, j));
                }
                if (oldBones) 
                {
                    newgeom.setBones(i, S4refgeom.getBones(refIndex));
                    newgeom.setBoneWeights(i, S4refgeom.getBoneWeights(refIndex));
                }
                vertList.Add(refIndex); 
            }
            if (oldBones) newgeom.setBoneHashList(S4refgeom.BoneHashList);

            if (newgeom.hasUVset(1)) newgeom.AutoUV1Stitches();

            //if (!S4refgeom.hasVertexIDs)        // if Vert ID is sequential, use that to find ref vertices and copy seamstitch and slotray data
            //{
            //    List<GEOM.SeamStitch> newSeams = new List<GEOM.SeamStitch>();
            //    for (int i = 0; i < S4refgeom.SeamStitches.Length; i++)
            //    {
            //        for (int j = 0; j < vertList.Count; j++)
            //        {
            //            if (vertList[j] == S4refgeom.SeamStitches[i].Index)
            //            {
            //                GEOM.SeamStitch adj = new GEOM.SeamStitch(S4refgeom.SeamStitches[i]);
            //                adj.Index = (uint)j;
            //                newSeams.Add(adj);
            //                break;
            //            }
            //        }
            //    }
            //    newgeom.SeamStitches = newSeams.ToArray();

            //    List<GEOM.SlotrayIntersection> newAdjusts2 = new List<GEOM.SlotrayIntersection>();
            //    for (int i = 0; i < S4refgeom.SlotrayAdjustments.Length; i++)
            //    {
            //        int[] fa = S4refgeom.SlotrayAdjustments[i].TrianglePointIndices;
            //        for (int j = 0; j < newgeom.numberFaces; j++)
            //        {
            //            int[] f = newgeom.getFaceIndices(j);
            //            int[] fo = new int[] { vertList[f[0]], vertList[f[1]], vertList[f[2]] };
            //            if ((fa[0] == fo[0]) & (fa[1] == fo[1]) & (fa[2] == fo[2]))
            //            {
            //                GEOM.SlotrayIntersection adj = new GEOM.SlotrayIntersection(S4refgeom.SlotrayAdjustments[i]);
            //                adj.TrianglePointIndices = f;
            //                newAdjusts2.Add(adj);
            //                break;
            //            }
            //        }
            //    }
            //    newgeom.SlotrayAdjustments = newAdjusts2.ToArray();
            //}
            //else                    //otherwise do autoassignments
            //{
                newgeom.AutoSeamStitches(species, age, gender, lod);
                newgeom.AutoSlotray(SelectSlotRayData(species, age));
            //}

            newgeom.BoneFixer(3);
            newgeom.CalculateTangents();
            newgeom.TGIList = S4refgeom.TGIList;
            newgeom.setShader(S4refgeom.ShaderHash, S4refgeom.Shader);
            return newgeom;
        }
    }
}
