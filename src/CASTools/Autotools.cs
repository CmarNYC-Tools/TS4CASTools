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
using Xmods.DataLib;
using s4pi.Interfaces;
using s4pi.Package;

namespace XMODS
{
    public partial class Form1 : Form
    {
        bool isTS4mesh = false;
        bool isDAEmesh = false;

        private void AutoMeshFile_button_Click(object sender, EventArgs e)
        {
            AutoMeshFile.Text = GetFilename("Select mesh to be converted", Meshfilter);
            if (String.Compare(AutoMeshFile.Text, " ") <= 0) return;

            if (String.Compare(Path.GetExtension(AutoMeshFile.Text).ToLower(), ".obj") == 0)
            {
                AutoShaderOrig_radioButton.Enabled = false;
                AutoShaderOrig_radioButton.Checked = false;
                AutoShaderRef_radioButton.Checked = true;
                AutoBones_checkBox.Checked = true;
                AutoColor_checkBox.Checked = true;
                AutoColor_radioButton.Checked = true;
                AutoMorph_checkBox.Checked = true;
                AutoSeamStitch_checkBox.Checked = true;
                AutoSlotRay_checkBox.Checked = true;
                AutoFlipYZ_checkBox.Enabled = false;
            }
            else if (String.Compare(Path.GetExtension(AutoMeshFile.Text).ToLower(), ".ms3d") == 0)
            {
                AutoShaderOrig_radioButton.Enabled = false;
                AutoShaderOrig_radioButton.Checked = false;
                AutoShaderRef_radioButton.Checked = true;
                AutoBones_checkBox.Checked = false;
                AutoColor_checkBox.Checked = true;
                HardColorAlpha_radioButton.Checked = true;
                AutoMorph_checkBox.Checked = true;
                AutoSeamStitch_checkBox.Checked = true;
                AutoSlotRay_checkBox.Checked = true;
                AutoFlipYZ_checkBox.Enabled = false;
            }
            else if (String.Compare(Path.GetExtension(AutoMeshFile.Text).ToLower(), ".dae") == 0)
            {
                AutoShaderOrig_radioButton.Enabled = false;
                AutoShaderOrig_radioButton.Checked = false;
                AutoShaderRef_radioButton.Checked = true;
                DAE dae = null;
                if (GetDAEData(AutoMeshFile.Text, false, out dae, false))
                {
                    if (dae.AllMeshesHaveBones) AutoBones_checkBox.Checked = false;
                    else AutoBones_checkBox.Checked = true;
                    AutoColor_checkBox.Checked = true;
                    if (dae.AllMeshesHaveColors) HardColorAlpha_radioButton.Checked = true;
                    else AutoColor_radioButton.Checked = true;
                    if (dae.AllMeshesHaveUV1) AutoMorph_checkBox.Checked = false;
                    else AutoMorph_checkBox.Checked = true;
                    AutoSeamStitch_checkBox.Checked = true;
                    AutoSlotRay_checkBox.Checked = true;
                }
                AutoFlipYZ_checkBox.Enabled = true;
                isDAEmesh = true;
            }
            else
            {
                AutoShaderOrig_radioButton.Enabled = true;
                AutoBones_checkBox.Checked = false;
                AutoColor_checkBox.Checked = true;
                HardColorAlpha_radioButton.Checked = true;
                AutoMorph_checkBox.Checked = true;
                AutoSeamStitch_checkBox.Checked = true;
                AutoSlotRay_checkBox.Checked = true;
                GEOM tmp;
                if (GetGEOMData(AutoMeshFile.Text, out tmp))
                {
                    if (tmp.meshVersion >= 12)
                    {
                        isTS4mesh = true;
                        AutoColor_checkBox.Checked = false;
                        AutoMorph_checkBox.Checked = !tmp.hasUVset(1);
                        AutoSeamStitch_checkBox.Checked = (tmp.UVStitches == null || tmp.UVStitches.Length == 0 ||
                            tmp.SeamStitches == null || tmp.SeamStitches.Length == 0);
                        AutoSlotRay_checkBox.Checked = (tmp.SlotrayAdjustments == null || tmp.SlotrayAdjustments.Length == 0);
                    }
                }
                AutoFlipYZ_checkBox.Enabled = false;
            }
        }

        private void AutoReferenceFile_button_Click(object sender, EventArgs e)
        {
            AutoReferenceFile.Text = GetFilename("Select TS4 GEOM reference mesh", GEOMfilter);
        }

        private void AutoBones_checkBox_CheckedChanged(object sender, EventArgs e)
        {
            Autobone_groupBox.Enabled = AutoBones_checkBox.Checked;
        }

        private void AutoColor_checkBox_CheckedChanged(object sender, EventArgs e)
        {
            if (AutoColor_checkBox.Checked)
            {
                AutoColor_radioButton.Enabled = true;
                HardColorAlpha_radioButton.Enabled = true;
                HardColorAll_radioButton.Enabled = true;
                HardColor_comboBox.Enabled = true;
            }
            else
            {
                AutoColor_radioButton.Enabled = false;
                HardColorAlpha_radioButton.Enabled = false;
                HardColorAll_radioButton.Enabled = false;
                HardColor_comboBox.Enabled = false;
            }
        }

        private void HardColorAlpha_checkBox_CheckedChanged(object sender, EventArgs e)
        {
            if (HardColorAlpha_radioButton.Checked)
            {
                HardColorAlphaValue.Enabled = true;
            }
            else
            {
                HardColorAlphaValue.Enabled = false;
            }
        }
        private void HardColorAll_checkBox_CheckedChanged(object sender, EventArgs e)
        {
            if (HardColorAll_radioButton.Checked)
            {
                HardColorAllValue.Enabled = true;
            }
            else
            {
                HardColorAllValue.Enabled = false;
            }
        }

        private void AutoKeepVertexID_checkBox_CheckedChanged(object sender, EventArgs e)
        {
            if (AutoKeepVertexID_checkBox.Checked) AutoOverrideVertexID_checkBox.Checked = false;
        }

        private void AutoOverrideVertexID_checkBox_CheckedChanged(object sender, EventArgs e)
        {
            if (AutoOverrideVertexID_checkBox.Checked) AutoKeepVertexID_checkBox.Checked = false;
        }
        
        private void HardColor_comboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            XmodsEnums.VertexColorStandard v;
            Enum.TryParse((string)HardColor_comboBox.SelectedItem, out v);
            HardColorAllValue.Text = ((uint)v).ToString("X8");
            HardColorAlphaValue.Text = (((uint)v) >> 24).ToString("X2");
        }

        private void AutoPartType_comboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            string part = AutoPartType_comboBox.Items[AutoPartType_comboBox.SelectedIndex].ToString();
            if (AutoPartType_comboBox.SelectedIndex == 0)   //hat
            {
                if (!isTS4mesh && !isDAEmesh) AutoMorph_checkBox.Checked = true;
                AutoSeamStitch_checkBox.Checked = false;
                AutoSlotRay_checkBox.Checked = false;
                HardColor_comboBox.SelectedIndex = 0;
            }
            else if (AutoPartType_comboBox.SelectedIndex == 1)   //hair
            {
                if (!isTS4mesh && !isDAEmesh) AutoMorph_checkBox.Checked = true;
                AutoSeamStitch_checkBox.Checked = false;
                AutoSlotRay_checkBox.Checked = false;
                HardColor_comboBox.SelectedIndex = 1;
            }
            else if (AutoPartType_comboBox.SelectedIndex == 2)   //head
            {
                if (!isTS4mesh && !isDAEmesh) AutoMorph_checkBox.Checked = true;
                AutoSeamStitch_checkBox.Checked = true;
                AutoSlotRay_checkBox.Checked = false;
                HardColor_comboBox.SelectedIndex = 2;
            }
            else if (AutoPartType_comboBox.SelectedIndex == 3)   //body
            {
                if (isDAEmesh)
                {
                    AutoSeamStitch_checkBox.Checked = true;
                    AutoSlotRay_checkBox.Checked = true;
                }
                else if (!isTS4mesh)
                {
                    AutoMorph_checkBox.Checked = true;
                    AutoSeamStitch_checkBox.Checked = true;
                    AutoSlotRay_checkBox.Checked = true;
                }
                HardColor_comboBox.SelectedIndex = 3;
            }
            else if (AutoPartType_comboBox.SelectedIndex == 4)   //top
            {
                if (isDAEmesh)
                {
                    AutoSeamStitch_checkBox.Checked = true;
                    AutoSlotRay_checkBox.Checked = true;
                }
                else if (!isTS4mesh)
                {
                    AutoMorph_checkBox.Checked = true;
                    AutoSeamStitch_checkBox.Checked = true;
                    AutoSlotRay_checkBox.Checked = true;
                }
                HardColor_comboBox.SelectedIndex = 4;
            }
            else if (AutoPartType_comboBox.SelectedIndex == 5)   //bottom
            {
                if (isDAEmesh)
                {
                    AutoSeamStitch_checkBox.Checked = true;
                    AutoSlotRay_checkBox.Checked = true;
                }
                else if (!isTS4mesh)
                {
                    AutoMorph_checkBox.Checked = true;
                    AutoSeamStitch_checkBox.Checked = true;
                    AutoSlotRay_checkBox.Checked = true;
                }
                HardColor_comboBox.SelectedIndex = 5;
            }
            else if (AutoPartType_comboBox.SelectedIndex == 6)   //shoes
            {
                if (isDAEmesh)
                {
                    AutoSeamStitch_checkBox.Checked = true;
                    AutoSlotRay_checkBox.Checked = true;
                }
                else if (!isTS4mesh)
                {
                    AutoMorph_checkBox.Checked = true;
                    AutoSeamStitch_checkBox.Checked = true;
                }
                AutoSlotRay_checkBox.Checked = false;
                HardColor_comboBox.SelectedIndex = 6;
            }
            else if (AutoPartType_comboBox.SelectedIndex == 7)   //other - morphing
            {
                if (!isTS4mesh && !isDAEmesh) AutoMorph_checkBox.Checked = true;
                AutoSeamStitch_checkBox.Checked = false;
                AutoSlotRay_checkBox.Checked = false;
                HardColor_comboBox.SelectedIndex = -1;
                HardColorAllValue.Text = "";
                HardColorAlphaValue.Text = "";
            }
            else if (AutoPartType_comboBox.SelectedIndex == 8)   //other - non-morphing
            {
                AutoMorph_checkBox.Checked = false;
                AutoSeamStitch_checkBox.Checked = false;
                AutoSlotRay_checkBox.Checked = false;
                HardColor_comboBox.SelectedIndex = -1;
                HardColorAllValue.Text = "";
                HardColorAlphaValue.Text = "";
            }
            else if (AutoPartType_comboBox.SelectedIndex == 9 || AutoPartType_comboBox.SelectedIndex == 10)   //Animal ears
            {
                if (!isTS4mesh && !isDAEmesh) AutoMorph_checkBox.Checked = true;
                AutoSeamStitch_checkBox.Checked = true;
                AutoSlotRay_checkBox.Checked = false;
                HardColor_comboBox.SelectedIndex = 24;
            }
            else if (AutoPartType_comboBox.SelectedIndex >= 11 && AutoPartType_comboBox.SelectedIndex <= 14)   //Animal tail
            {
                if (!isTS4mesh && !isDAEmesh) AutoMorph_checkBox.Checked = true;
                AutoSeamStitch_checkBox.Checked = true;
                AutoSlotRay_checkBox.Checked = true;
                HardColor_comboBox.SelectedIndex = 25;
            }
            else if (AutoPartType_comboBox.SelectedIndex == 15)   //Mermaid tail
            {
                if (isDAEmesh)
                {
                    AutoSeamStitch_checkBox.Checked = true;
                    AutoSlotRay_checkBox.Checked = true;
                }
                else if (!isTS4mesh)
                {
                    AutoMorph_checkBox.Checked = true;
                    AutoSeamStitch_checkBox.Checked = true;
                    AutoSlotRay_checkBox.Checked = true;
                }
                HardColor_comboBox.SelectedIndex = 25;
            }
        }

        private void AutoRefStandard_radioButton_CheckedChanged(object sender, EventArgs e)
        {
            AutoReferenceOptions();
        }

        private void AutoRefCustom_radioButton_CheckedChanged(object sender, EventArgs e)
        {
            AutoReferenceOptions();
        }

        private void AutoRefNone_radioButton_CheckedChanged(object sender, EventArgs e)
        {
            AutoReferenceOptions();
        }
        
        private void AutoReferenceOptions()
        {
            if (AutoRefCustom_radioButton.Checked)
            {
                AutoReferenceFile_button.Enabled = true;
                AutoReferenceFile.Enabled = true;
                AutoColor_radioButton.Checked = true;
            }
            else
            {
                AutoReferenceFile_button.Enabled = false;
                AutoReferenceFile.Enabled = false;
                HardColorAlpha_radioButton.Checked = true;
            }
            if (AutoRefNone_radioButton.Checked)
            {
                AutoBones_checkBox.Enabled = false;
                AutoColor_checkBox.Enabled = false;
                AutoTexture_checkBox.Enabled = false;
                AutoMorph_checkBox.Enabled = false;
                AutoSeamStitch_checkBox.Enabled = false;
                AutoSlotRay_checkBox.Enabled = false;
                AutoShaderOrig_radioButton.Checked = true;
                HardColor_comboBox.SelectedIndex = -1;
                HardColorAllValue.Text = "";
                HardColorAlphaValue.Text = "";
                Autobone_groupBox.Enabled = false;
                VertexColor_groupBox.Enabled = false;
                AutoPartType_comboBox.Enabled = false;
                AutoAge_comboBox.Enabled = false;
                AutoGender_comboBox.Enabled = false;
                AutoLod_comboBox.Enabled = false;
            }
            else
            {
                AutoBones_checkBox.Enabled = true;
                AutoColor_checkBox.Enabled = true;
                AutoTexture_checkBox.Enabled = true;
                AutoMorph_checkBox.Enabled = true;
                AutoSeamStitch_checkBox.Enabled = true;
                AutoSlotRay_checkBox.Enabled = true;
                AutoShaderRef_radioButton.Checked = true;
                Autobone_groupBox.Enabled = true;
                VertexColor_groupBox.Enabled = true;
                AutoPartType_comboBox.Enabled = true;
                AutoAge_comboBox.Enabled = true;
                AutoGender_comboBox.Enabled = true;
                AutoLod_comboBox.Enabled = true;
            }
        }

        private void AutoGo_button_Click(object sender, EventArgs e)
        {
            if (!File.Exists(AutoMeshFile.Text))
            {
                MessageBox.Show("You must select a mesh to convert!");
                return;
            }

            GEOM refMesh = new GEOM();
            GEOM standardMesh = new GEOM();
            GEOM skirtMesh = null;
            XmodsEnums.Species species;
            XmodsEnums.Age age;
            XmodsEnums.Gender gender;
            species = (XmodsEnums.Species)(AutoSpecies_comboBox.SelectedIndex + 1);
            if (AutoAge_comboBox.SelectedIndex == 0) age = XmodsEnums.Age.Infant;
            else if (AutoAge_comboBox.SelectedIndex == 1) age = XmodsEnums.Age.Toddler;
            else if (AutoAge_comboBox.SelectedIndex == 2) age = XmodsEnums.Age.Child;
            else age = XmodsEnums.Age.TeenToElder;
            if (AutoGender_comboBox.SelectedIndex == 0) gender = XmodsEnums.Gender.Male;
            else if (AutoGender_comboBox.SelectedIndex == 1) gender = XmodsEnums.Gender.Female;
            else gender = XmodsEnums.Gender.Unisex;
            RIG rig = SelectRig(species, age);
            if (AutoPartType_comboBox.SelectedIndex == 1)
            {
                standardMesh = new GEOM(new BinaryReader(new MemoryStream(Properties.Resources.yfHairReference)));
            }
            else if (AutoPartType_comboBox.SelectedIndex == 2)
            {
                standardMesh = GetBodyMesh(species, age, gender, AutoLod_comboBox.SelectedIndex, XmodsEnums.BodyType.Head, XmodsEnums.BodySubType.None, false);
            }
            else if (AutoPartType_comboBox.SelectedIndex == 9 && species != XmodsEnums.Species.Human)
            {
                standardMesh = GetBodyMesh(species, age, gender, AutoLod_comboBox.SelectedIndex, XmodsEnums.BodyType.AnimalEars, XmodsEnums.BodySubType.EarsUp, false);
            }
            else if (AutoPartType_comboBox.SelectedIndex == 10 && species != XmodsEnums.Species.Human)
            {
                standardMesh = GetBodyMesh(species, age, gender, AutoLod_comboBox.SelectedIndex, XmodsEnums.BodyType.AnimalEars, XmodsEnums.BodySubType.EarsDown, false);
            }
            else if (AutoPartType_comboBox.SelectedIndex == 11 && species != XmodsEnums.Species.Human)
            {
                standardMesh = GetBodyMesh(species, age, gender, AutoLod_comboBox.SelectedIndex, XmodsEnums.BodyType.Tail, XmodsEnums.BodySubType.TailLong, false);
            }
            else if (AutoPartType_comboBox.SelectedIndex == 12 && species != XmodsEnums.Species.Human)
            {
                standardMesh = GetBodyMesh(species, age, gender, AutoLod_comboBox.SelectedIndex, XmodsEnums.BodyType.Tail, XmodsEnums.BodySubType.TailRing, false);
            }
            else if (AutoPartType_comboBox.SelectedIndex == 13 && species != XmodsEnums.Species.Human)
            {
                standardMesh = GetBodyMesh(species, age, gender, AutoLod_comboBox.SelectedIndex, XmodsEnums.BodyType.Tail, XmodsEnums.BodySubType.TailScrew, false);
            }
            else if (AutoPartType_comboBox.SelectedIndex == 14 && species != XmodsEnums.Species.Human)
            {
                standardMesh = GetBodyMesh(species, age, gender, AutoLod_comboBox.SelectedIndex, XmodsEnums.BodyType.Tail, XmodsEnums.BodySubType.TailStub, false);
            }
            else if (AutoPartType_comboBox.SelectedIndex == 15 && species == XmodsEnums.Species.Human)
            {
                standardMesh = GetBodyMesh(species, age, gender, AutoLod_comboBox.SelectedIndex, XmodsEnums.BodyType.Tail, XmodsEnums.BodySubType.None, false);
            }
            else if (AutoPartType_comboBox.SelectedIndex == 3)
            {
                standardMesh = GetBodyMesh(species, age, gender, AutoLod_comboBox.SelectedIndex, XmodsEnums.BodyType.Body, XmodsEnums.BodySubType.None, false);
            }
            else
            {
                standardMesh = GetBodyMesh(species, age, gender, AutoLod_comboBox.SelectedIndex, XmodsEnums.BodyType.All, XmodsEnums.BodySubType.None, false);
            }
            if (AutoRefStandard_radioButton.Checked)
            {
                refMesh = standardMesh;
                if (AutoSkirt_checkBox.Checked && species == XmodsEnums.Species.Human && age > XmodsEnums.Age.Toddler)
                    skirtMesh = GetBodyMesh(species, age, gender, AutoLod_comboBox.SelectedIndex, XmodsEnums.BodyType.All, XmodsEnums.BodySubType.None, true);
            }
            else if (AutoRefNone_radioButton.Checked)
            {
                refMesh = null;
                skirtMesh = null;
            }
            else
            {
                if (String.CompareOrdinal(AutoReferenceFile.Text, "") > 0)
                {
                    if (!File.Exists(AutoReferenceFile.Text))
                    {
                        MessageBox.Show("You must select a valid reference mesh!");
                        return;
                    }
                    if (!GetGEOMData(AutoReferenceFile.Text, out refMesh))
                    {
                        return;
                    }
                    if (!refMesh.isValid | !refMesh.isTS4)
                    {
                        MessageBox.Show("The reference mesh is not a valid TS4 mesh!");
                        return;
                    }
                    refMesh.UpdateToLatestVersion(rig);
                }
                else
                {
                    MessageBox.Show("You must select a reference mesh!");
                    return;
                }
            }

            uint hardVertColor = 0u;
            if (AutoColor_checkBox.Checked && !AutoColor_radioButton.Checked && refMesh != null)
            {
                try
                {
                    if (HardColorAlpha_radioButton.Checked)
                    {
                        hardVertColor = uint.Parse(HardColorAlphaValue.Text, System.Globalization.NumberStyles.HexNumber);
                    }
                    else
                    {
                        hardVertColor = uint.Parse(HardColorAllValue.Text, System.Globalization.NumberStyles.HexNumber);
                    }
                }
                catch
                {
                    MessageBox.Show("The override vertex color is not a valid hex number!");
                    return;
                }
            }

            GEOM[] fixMesh;
            string[] meshNames = null;
            if (String.Compare(Path.GetExtension(AutoMeshFile.Text).ToLower(), ".ms3d") == 0)
            {
                if (AutoRefNone_radioButton.Checked)
                {
                    MessageBox.Show("You must use a reference mesh to convert an ms3d file!");
                    return;
                }
                MS3D ms3d = null;
                if (!GetMS3DData(AutoMeshFile.Text, out ms3d, true))
                {
                    MessageBox.Show("Can't read ms3d mesh!");
                    return;
                }
                fixMesh = GEOM.GEOMsFromMS3D(ms3d, refMesh, AutoStuff_progressBar);
                meshNames = ms3d.getGroupNames();
            }

            else if (String.Compare(Path.GetExtension(AutoMeshFile.Text).ToLower(), ".dae") == 0)
            {
                DAE dae = null;
                if (!GetDAEData(AutoMeshFile.Text, AutoFlipYZ_checkBox.Checked, out dae, true))
                {
                    MessageBox.Show("Can't read Collada .dae mesh!");
                    return;
                }
                fixMesh = GEOM.GEOMsFromDAE(dae, refMesh, AutoStuff_progressBar);
                meshNames = dae.getMeshNames;
            }

            else if (String.Compare(Path.GetExtension(AutoMeshFile.Text).ToLower(), ".obj") == 0)
            {
                if (AutoRefNone_radioButton.Checked)
                {
                    MessageBox.Show("You must use a reference mesh to convert an obj file!");
                    return;
                }
                OBJ obj = null;
                if (!GetOBJData(AutoMeshFile.Text, out obj))
                {
                    MessageBox.Show("Can't read obj mesh!");
                    return;
                }
                fixMesh = GEOM.GEOMsFromOBJ(obj, refMesh, skirtMesh, AutoStuff_progressBar, false);
                meshNames = obj.getGroupNames();
            }

            else if (String.Compare(Path.GetExtension(AutoMeshFile.Text).ToLower(), ".geom") == 0 ||
                        String.Compare(Path.GetExtension(AutoMeshFile.Text).ToLower(), ".simgeom") == 0)
            {
                GEOM tmpMesh = new GEOM();
                if (!GetGEOMData(AutoMeshFile.Text, out tmpMesh))
                {
                    MessageBox.Show("Can't read GEOM mesh!");
                    return;
                }
                if (!tmpMesh.isValid | !tmpMesh.isBase)
                {
                    MessageBox.Show("This is not a valid TS4 mesh or TS3 base mesh!");
                    return;
                }

                if (AutoShaderRef_radioButton.Checked)
                {
                    tmpMesh.setShader(refMesh.ShaderHash, refMesh.Shader);
                    tmpMesh.TGIList = refMesh.TGIList;
                }
                fixMesh = new GEOM[1];
                if (tmpMesh.meshVersion < 12)
                    fixMesh[0] = ConvertS3toS4(tmpMesh);
                else
                {
                    tmpMesh.UpdateToLatestVersion(rig);
                    if (tmpMesh.UVStitches == null) tmpMesh.UVStitches = new GEOM.UVStitch[0];
                    if (tmpMesh.SeamStitches == null) tmpMesh.SeamStitches = new GEOM.SeamStitch[0];
                    if (tmpMesh.SlotrayAdjustments == null) tmpMesh.SlotrayAdjustments = new GEOM.SlotrayIntersection[0];
                    fixMesh[0] = tmpMesh;
                }
            }
            else
            {
                MessageBox.Show("Unrecognized mesh file type!");
                return;
            }

            if (fixMesh.Length > 1)
            {
                MeshMultipartConversionForm f = new MeshMultipartConversionForm(AutoPartType_comboBox.SelectedIndex == 1);
                DialogResult res = f.ShowDialog();
                if (res == DialogResult.Cancel) return;
                AutoStuff_progressBar.Visible = true;
                AutoStuff_progressBar.Minimum = 0;
                AutoStuff_progressBar.Maximum = fixMesh.Length;
                AutoStuff_progressBar.Value = 0;
                AutoStuff_progressBar.Step = 1;
                if (f.ConvertAllToOne)
                {
                    
                    GEOM tmp = new GEOM(fixMesh[0]);
                    for (int i = 1; i < fixMesh.Length; i++)
                    {
                        AutoStuff_progressBar.PerformStep();
                        tmp.AppendMesh(fixMesh[i], rig, false);
                    }
                    fixMesh = new GEOM[] { tmp };
                    meshNames = null;
                }
            }

            for (int i = 0; i < fixMesh.Length; i++)
            {
                AutoStuff_progressBar.Visible = true;
                AutoStuff_progressBar.Minimum = 0;
                AutoStuff_progressBar.Maximum = (AutoBones_checkBox.Checked ? 1 : 0) +
                    (AutoTexture_checkBox.Checked ? 1: 0) + (AutoMorph_checkBox.Checked ? 2 : 0) +
                    (AutoColor_checkBox.Checked ? 1 : 0) + (AutoSeamStitch_checkBox.Checked ? 1 : 0) +
                    (AutoSlotRay_checkBox.Checked ? 1 : 0) + (AutoCleanMesh_checkBox.Checked ? 1 : 0);
                AutoStuff_progressBar.Value = 1;
                AutoStuff_progressBar.Step = 1;

                if (AutoVertexID_checkBox.Checked && refMesh != null && refMesh.hasVertexIDs)
                {
                    fixMesh[i].AutoVertexID(refMesh);
                }
                else if (AutoKeepVertexID_checkBox.Checked)
                {
                    fixMesh[i].AddVertexIDtoFormat();
                }
                else if (AutoOverrideVertexID_checkBox.Checked)
                {
                    int vertID;
                    if (!int.TryParse(AutoManualID.Text, out vertID))
                    {
                        MessageBox.Show("Please enter a valid Vertex ID for manual override!");
                        return;
                    }
                    fixMesh[i].AddVertexIDtoFormat();
                    for (int j = 0; j < fixMesh[i].numberVertices; j++)
                    {
                        fixMesh[i].setVertexID(j, vertID);
                    }
                }

                if (AutoBones_checkBox.Checked && refMesh != null)
                {
                    if (!fixMesh[i].hasBones)
                    {
                        MessageBox.Show("The mesh format does not include bone assignments!");
                        return;
                    }
                    if (!refMesh.hasBones)
                    {
                        MessageBox.Show("The reference mesh does not have bone assignment data!");
                        return;
                    }

                    float weightingFactor;
                    if (!float.TryParse(AutoBoneWeighting.Text, out weightingFactor) || weightingFactor < 0f)
                    {
                        MessageBox.Show("The Weighting Power must be a valid positive number!");
                        return;
                    }

                    try
                    {
                        fixMesh[i].AutoBone(refMesh, skirtMesh, AutoBoneZero_checkBox.Checked, AutoBoneInterpolate_checkBox.Checked, (int)AutoBoneInterpolationPoints.Value, weightingFactor, AutoStuff_progressBar);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace.ToString());
                        //AutoBone_progressBar.Visible = false;
                        return;
                    }

                    fixMesh[i].BoneFixer(3);

                    if (fixMesh[i].numberBones > 60)
                    {
                        DialogResult mb = MessageBox.Show("The bone assigned mesh contains too many bones, probably causing distortion in-game." +
                            System.Environment.NewLine + "Save the mesh anyway?", "Too many bones", MessageBoxButtons.YesNo);
                        if (mb == DialogResult.No)
                        {
                            return;
                        }
                    }

                    AutoStuff_progressBar.PerformStep();
                }

                if (AutoTexture_checkBox.Checked && refMesh != null)
                {
                    fixMesh[i].AutoUV(refMesh, 0);
                    AutoStuff_progressBar.PerformStep();
                }

                if (AutoMorph_checkBox.Checked && refMesh != null)
                {
                    fixMesh[i].MatchUVsets(refMesh.vertexFormat);
                    for (int uv = 1; uv < fixMesh[i].numberUVsets; uv++)
                    {
                        fixMesh[i].AutoUV(refMesh, uv);
                        AutoStuff_progressBar.PerformStep();
                    }
                    fixMesh[i].AutoUV1Stitches();
                    AutoStuff_progressBar.PerformStep();
                }
                else if (AutoManualUV1_checkBox.Enabled && AutoManualUV1_checkBox.Checked)
                {
                    fixMesh[i].AddMorphUV();
                    float overrideX, overrideY;
                    if (!float.TryParse(AutoManualUV1X.Text, out overrideX) || !float.TryParse(AutoManualUV1Y.Text, out overrideY))
                    {
                        MessageBox.Show("Please enter valid manual override UV coordinates!");
                        return;
                    }
                    for (int j = 0; j < fixMesh[i].numberVertices; j++)
                    {
                        fixMesh[i].setUV(j, 1, overrideX, overrideY);
                    }
                }

                if (AutoColor_checkBox.Checked && refMesh != null)
                {
                    if (HardColorAll_radioButton.Checked)
                    {
                        for (int j = 0; j < fixMesh[i].numberVertices; j++)
                        {
                            fixMesh[i].setTagval(j, hardVertColor);
                        }
                    }
                    else if (HardColorAlpha_radioButton.Checked)
                    {
                        for (int j = 0; j < fixMesh[i].numberVertices; j++)
                        {
                            fixMesh[i].setTagval(j, (fixMesh[i].getTagval(j) & 0x00FFFFFF) + (hardVertColor << 24));
                        }
                    }
                    else if (refMesh != null)
                    {
                        Vector3[] refVerts = new Vector3[refMesh.numberVertices];
                        Vector3[] refVertsSkirt = (skirtMesh != null) ? new Vector3[skirtMesh.numberVertices] : null;
                        for (int j = 0; j < refMesh.numberVertices; j++)
                        {
                            refVerts[j] = new Vector3(refMesh.getPosition(j));
                        }
                        if (skirtMesh != null)
                        {
                            for (int j = 0; j < skirtMesh.numberVertices; j++)
                            {
                                refVertsSkirt[j] = new Vector3(skirtMesh.getPosition(j));
                            }
                        }

                        //use correct alpha for CASP type, use standard nude mesh and skirt mesh for the rest
                        for (int j = 0; j < fixMesh[i].numberVertices; j++)
                        {
                            int nearestRefVert = (new Vector3(fixMesh[i].getPosition(j))).NearestPointIndexSimple(refVerts);
                            uint tagVal = refMesh.getTagval(nearestRefVert);
                            if (skirtMesh != null && (!(new Vector3(fixMesh[i].getPosition(j)).CloseTo(refVerts[nearestRefVert], 0.001f))))
                            {
                                nearestRefVert = (new Vector3(fixMesh[i].getPosition(j))).NearestPointIndexSimple(refVertsSkirt);
                                tagVal = skirtMesh.getTagval(nearestRefVert);
                            }                           
                            fixMesh[i].setTagval(j, tagVal);
                        }
                    }
                    AutoStuff_progressBar.PerformStep();
                }

                if (AutoSeamStitch_checkBox.Checked && refMesh != null)
                {
                    fixMesh[i].AutoSeamStitches(species, age, gender, AutoLod_comboBox.SelectedIndex);
                    AutoStuff_progressBar.PerformStep();
                }

                if (AutoSlotRay_checkBox.Checked && refMesh != null)
                {
                    //XmodsEnums.BodyType type;
                    //XmodsEnums.BodySubType subType = XmodsEnums.BodySubType.None;
                    //if (AutoPartType_comboBox.SelectedIndex == 3)
                    //{
                    //    type = XmodsEnums.BodyType.Body;
                    //}
                    //else if (AutoPartType_comboBox.SelectedIndex == 4)
                    //{
                    //    type = XmodsEnums.BodyType.Top;
                    //}
                    //else if (AutoPartType_comboBox.SelectedIndex == 5)
                    //{
                    //    type = XmodsEnums.BodyType.Bottom;
                    //}
                    //else if (AutoPartType_comboBox.SelectedIndex == 11)
                    //{
                    //    type = XmodsEnums.BodyType.Tail;
                    //    subType = XmodsEnums.BodySubType.TailLong;
                    //}
                    //else if (AutoPartType_comboBox.SelectedIndex == 12)
                    //{
                    //    type = XmodsEnums.BodyType.Tail;
                    //    subType = XmodsEnums.BodySubType.TailRing;
                    //}
                    //else if (AutoPartType_comboBox.SelectedIndex == 13)
                    //{
                    //    type = XmodsEnums.BodyType.Tail;
                    //    subType = XmodsEnums.BodySubType.TailScrew;
                    //}
                    //else if (AutoPartType_comboBox.SelectedIndex == 14)
                    //{
                    //    type = XmodsEnums.BodyType.Tail;
                    //    subType = XmodsEnums.BodySubType.TailStub;
                    //}
                    //else
                    //{
                    //    type = XmodsEnums.BodyType.All;
                    //}
                    //GEOM slotMesh = GetBodyMesh(species, age, gender, AutoLod_comboBox.SelectedIndex, type, subType);
                    fixMesh[i].AutoSlotray(SelectSlotRayData(species, age));
                    AutoStuff_progressBar.PerformStep();
                }

                if (AutoRemoveBump_checkBox.Checked) fixMesh[i].RemoveNormalMap();

                if (AutoCleanMesh_checkBox.Checked)
                {
                    fixMesh[i].Clean();
                    AutoStuff_progressBar.PerformStep();
                }

               // fixMesh[i].NeatenFaceIndices();
                AutoStuff_progressBar.Visible = false;
                string str = WriteGEOMFile("Save new mesh " + (meshNames == null ? i.ToString() : meshNames[i]), fixMesh[i], "");
            }
        }

        internal GEOM GetBodyMesh(XmodsEnums.Species species, XmodsEnums.Age age, XmodsEnums.Gender gender, int lod, 
            XmodsEnums.BodyType partType, XmodsEnums.BodySubType subType, bool hasSkirt)
        {
            if (partType == XmodsEnums.BodyType.Hair) return new GEOM(new BinaryReader(new MemoryStream(Properties.Resources.yfHairReference)));

            GEOM myHead = new GEOM();
            GEOM myTop = new GEOM();
            GEOM myBottom = new GEOM();
            GEOM myFeet = new GEOM();
            GEOM myBody = new GEOM();
            GEOM myEars = new GEOM();
            GEOM myTail = new GEOM();
            RIG rig = SelectRig(species, age);

        //    System.Resources.ResourceManager rm = new System.Resources.ResourceManager(typeof(Form1));
            System.Resources.ResourceManager rm = Properties.Resources.ResourceManager;
            string specifier = "";
            if ((age & XmodsEnums.Age.Infant) > 0) specifier = (species == XmodsEnums.Species.Human ? "i" : "c");
            else if (age == XmodsEnums.Age.Toddler) specifier = (species == XmodsEnums.Species.Human ? "p" : "c");
            else if (age == XmodsEnums.Age.Child) specifier = "c";
            else specifier = (species == XmodsEnums.Species.Human ? "y" : "a");
            if (species != XmodsEnums.Species.Human) specifier +=
                (age == XmodsEnums.Age.Child && species == XmodsEnums.Species.LittleDog) ? "d" :
                species.ToString().Substring(0, 1).ToLower();
            else if (age <= XmodsEnums.Age.Child || (age & XmodsEnums.Age.Infant) > 0) specifier += "u";
            else if (age > XmodsEnums.Age.Child && age <= XmodsEnums.Age.Elder && gender == XmodsEnums.Gender.Unisex) specifier += "f";
            else specifier += gender.ToString().Substring(0, 1).ToLower();

            myHead = new GEOM(new BinaryReader(new MemoryStream((byte[])rm.GetObject(specifier + "Head_lod" + lod.ToString()))));
            myBody = new GEOM(new BinaryReader(new MemoryStream((byte[])rm.GetObject(specifier + "Body_lod" + lod.ToString()))));
            myFeet = new GEOM(new BinaryReader(new MemoryStream((byte[])rm.GetObject(specifier + "Shoes_lod" + lod.ToString()))));
            if (species == XmodsEnums.Species.Human)
            {
                myTop = new GEOM(new BinaryReader(new MemoryStream((byte[])rm.GetObject(specifier + "Top_lod" + lod.ToString()))));
                if (hasSkirt && age > XmodsEnums.Age.Toddler) myBody = new GEOM(new BinaryReader(new MemoryStream((byte[])rm.GetObject(specifier + "Skirt_lod" + lod.ToString()))));
                myBottom = new GEOM(new BinaryReader(new MemoryStream((byte[])rm.GetObject(specifier + "Bottom_lod" + lod.ToString()))));
                myTail = age > XmodsEnums.Age.Child && (age & XmodsEnums.Age.Infant) == 0 ? new GEOM(new BinaryReader(new MemoryStream((byte[])rm.GetObject(specifier + "Tail_lod" + lod.ToString())))) : null;
            }
            else
            {
                if (subType == XmodsEnums.BodySubType.EarsDown)
                {
                    myEars = new GEOM(new BinaryReader(new MemoryStream((byte[])rm.GetObject(specifier + "EarsDown_lod" + lod.ToString()))));
                }
                else
                {
                    myEars = new GEOM(new BinaryReader(new MemoryStream((byte[])rm.GetObject(specifier + "EarsUp_lod" + lod.ToString()))));
                }
                if (subType == XmodsEnums.BodySubType.TailRing)
                {
                    myTail = new GEOM(new BinaryReader(new MemoryStream((byte[])rm.GetObject(specifier + "TailRing_lod" + lod.ToString()))));
                }
                else if (subType == XmodsEnums.BodySubType.TailScrew)
                {
                    myTail = new GEOM(new BinaryReader(new MemoryStream((byte[])rm.GetObject(specifier + "TailScrew_lod" + lod.ToString()))));
                }
                else if (subType == XmodsEnums.BodySubType.TailStub)
                {
                    myTail = new GEOM(new BinaryReader(new MemoryStream((byte[])rm.GetObject(specifier + "TailStub_lod" + lod.ToString()))));
                }
                else
                {
                    myTail = new GEOM(new BinaryReader(new MemoryStream((byte[])rm.GetObject(specifier + "Tail_lod" + lod.ToString()))));
                }
            }

            if (partType == XmodsEnums.BodyType.Head)
            {
                return myHead;
            }
            //if (partType == XmodsEnums.BodyType.Top)
            //{
            //    return myTop;
            //}
            else if (partType == XmodsEnums.BodyType.Bottom)
            {
                return myBottom;
            }
            else if (partType == XmodsEnums.BodyType.Body || partType == XmodsEnums.BodyType.Top)
            {
                GEOM justHead = myHead.RemoveChildBones(0x0F97B21B, rig);
                if (species == XmodsEnums.Species.Human)
                {
                    myBody.AppendMesh(justHead, rig, false);
                    myBody.AppendMesh(myFeet, rig, false);
                }
                else
                {
                    myBody.AppendMesh(justHead, rig, false);
                    myBody.AppendMesh(myFeet, rig, false);
                    myBody.AppendMesh(myEars, rig, false);
                    myBody.AppendMesh(myTail, rig, false);
                }
                return myBody;
            }
            else if (partType == XmodsEnums.BodyType.AnimalEars)
            {
                return myEars;
            }
            else if (partType == XmodsEnums.BodyType.Tail)
            {
                return myTail;
            }
            GEOM myBod;
            if (species == XmodsEnums.Species.Human)
            {
                myBod = myTop;
                myBod.AppendMesh(myHead, rig, false);
                myBod.AppendMesh(myBottom, rig, false);
                myBod.AppendMesh(myFeet, rig, false);
            }
            else
            {
                myBod = myBody;
                myBod.AppendMesh(myHead, rig, false);
                myBod.AppendMesh(myFeet, rig, false);
                myBod.AppendMesh(myEars, rig, false);
                myBod.AppendMesh(myTail, rig, false);
            }
            return myBod;
        }

        private void AutoSkirtHelp_button_Click(object sender, EventArgs e)
        {
            MessageBox.Show("EXPERIMENTAL:" + Environment.NewLine +
                            "If you check the option to auto assign a mesh with a skirt," + Environment.NewLine +
                            "CAS Tools will try to assign bone weights and vertex colors" + Environment.NewLine +
                            "based on whether each vertex is most likely to be part of a" + Environment.NewLine +
                            "skirt or part of the legs. For tight skirts or if the leg" + Environment.NewLine +
                            "vertices aren't in the standard positions you probably will" + Environment.NewLine +
                            "have to correct the vertex colors manually, and possibly" + Environment.NewLine +
                            "the bone assignments too.");
        }

        private void GEOMlayersFile_button_Click(object sender, EventArgs e)
        {
            GEOMlayersFile.Text = GetFilename("Select geom mesh", GEOMfilter);
            if (GEOMlayersCenterStart_radioButton.Checked)
            {
                try
                {
                    GEOM layerGEOM;
                    if (GetGEOMData(GEOMlayersFile.Text, out layerGEOM))
                    {
                        layerStartPoint = layerGEOM.MeshCenter();
                        GEOMlayersCoordinates.Text = layerStartPoint.ToString();
                    }
                }
                catch
                {
                    GEOMlayersCoordinates.Text = "";
                }
            }
        }

        private void GEOMlayersStandardStart_radioButton_CheckedChanged(object sender, EventArgs e)
        {
            layerStartPoint = new Vector3(0f, 1.7f, -0.05f);
            GEOMlayersCoordinates.Text = layerStartPoint.ToString();
        }

        private void GEOMlayersCenterStart_radioButton_CheckedChanged(object sender, EventArgs e)
        {
            GEOM layerGEOM;
            if (!GEOMlayersCenterStart_radioButton.Checked) return;
            if (string.Compare(GEOMlayersFile.Text, " ") <= 0) return;
            try
            {
                if (GetGEOMData(GEOMlayersFile.Text, out layerGEOM))
                {
                    layerStartPoint = layerGEOM.MeshCenter();
                    GEOMlayersCoordinates.Text = layerStartPoint.ToString();
                }
            }
            catch
            {
                GEOMlayersCoordinates.Text = "";
            }
        }

        private void GEOMlayersToMesh_Click(object sender, EventArgs e)
        {
            if (GEOMlayersManualStart_radioButton.Checked)
            {
                try
                {
                    layerStartPoint = Vector3.Parse(GEOMlayersCoordinates.Text);
                }
                catch
                {
                    MessageBox.Show("The start point is not specified correctly!");
                    return;
                }
            }
            GEOM layerGEOM;
            if (String.CompareOrdinal(GEOMlayersFile.Text, "") > 0)
            {
                if (!File.Exists(GEOMlayersFile.Text))
                {
                    MessageBox.Show(GEOMlayersFile.Text + " does not exist!");
                    return;
                }
                if (GetGEOMData(GEOMlayersFile.Text, out layerGEOM))
                {
                    List<GEOM.Face[]>[] layers = layerGEOM.GetLayers(layerStartPoint, GEOMlayersPosition_radioButton.Checked,
                                                 GEOMlayersUV_checkBox.Checked, GEOMlayersNormals_checkBox.Checked,
                                                 GEOMlayersBackface_checkBox.Checked);
                    List<GEOM.Face[]> finalLayers = new List<GEOM.Face[]>();
                    List<string> layerNames = new List<string>();
                    if (layers.Length > 1)
                    {
                        if (GEOMlayersBackCombine_checkBox.Checked)
                        {
                            List<GEOM.Face> temp = new List<GEOM.Face>();
                            for (int i = 0; i < layers[1].Count; i++)
                            {
                                temp.AddRange(layers[1][i]);
                            }
                            finalLayers.Add(temp.ToArray());
                            layerNames.Add("Backfaces");
                        }
                        else
                        {
                            finalLayers.AddRange(layers[1]);
                            for (int i = 0; i < layers[1].Count; i++)
                            {
                                layerNames.Add("BackfaceLayer");
                            }
                        }
                    }
                    finalLayers.AddRange(layers[0]);
                    for (int i = 0; i < layers[0].Count; i++)
                    {
                        layerNames.Add("TopLayer");
                    }
                    Button meshType = sender as Button;
                    if ((meshType) == GEOMlayersToMS3D_button)
                    {
                        XmodsEnums.Age age;
                        if (GEOMlayersInfant_radioButton.Checked) age = XmodsEnums.Age.Infant;
                        else if (GEOMlayersToddler_radioButton.Checked) age = XmodsEnums.Age.Toddler;
                        else if (GEOMlayersChild_radioButton.Checked) age = XmodsEnums.Age.Child;
                        else age = XmodsEnums.Age.Adult;
                        if (SelectRig(XmodsEnums.Species.Human, age) == null)
                        {
                            MessageBox.Show("Rig file not found!");
                            return;
                        }
                        MS3D ms3d = new MS3D(layerGEOM, finalLayers, layerNames, SelectRig(XmodsEnums.Species.Human, age), 0);
                        WriteMS3DFile("Save MS3D file", ms3d, "");
                    }
                    else if (meshType == GEOMlayersToOBJbutton)
                    {
                        OBJ obj = new OBJ(layerGEOM, finalLayers, layerNames, 0);
                        WriteOBJFile("Save OBJ file", obj, "");
                    }
                    else if (meshType == GEOMlayersToDAE_button)
                    {
                        XmodsEnums.Age age;
                        if (GEOMlayersInfant_radioButton.Checked) age = XmodsEnums.Age.Infant;
                        else if (GEOMlayersToddler_radioButton.Checked) age = XmodsEnums.Age.Toddler;
                        else if (GEOMlayersChild_radioButton.Checked) age = XmodsEnums.Age.Child;
                        else age = XmodsEnums.Age.Adult;
                        if (SelectRig(XmodsEnums.Species.Human, age) == null)
                        {
                            MessageBox.Show("Rig file not found!");
                            return;
                        }
                        DAE dae = new DAE(layerGEOM, finalLayers, layerNames, SelectRig(XmodsEnums.Species.Human, age), false);
                        WriteDAEFile("Save DAE file", dae, false, "");
                    }
                }
                else
                {
                    MessageBox.Show("Can't read GEOM data!");
                    return;
                }
            }
            else { return; }
        }
    }
}
