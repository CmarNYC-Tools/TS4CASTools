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

        GEOM displayGEOM = new GEOM();
        GEOM viewUVGEOM = new GEOM();

        private void GEOMDisplayFile_button_Click(object sender, EventArgs e)
        {
            GEOMDisplayData();
        }

        private void GEOMDisplay_button_Click(object sender, EventArgs e)
        {
            GEOMDataDisplay meshData = new GEOMDataDisplay(displayGEOM, GEOMDisplayFilename.Text, GEOMDataID_checkbox.Checked,
                GEOMDataPosition_checkbox.Checked, GEOMDataNormals_checkbox.Checked, GEOMDataUV_checkbox.Checked,
                GEOMDataStitch_checkBox.Checked, GEOMDataSeamStitch_checkBox.Checked,
                GEOMDataBones_checkbox.Checked, GEOMDataTangents_checkbox.Checked, GEOMDataTags_checkbox.Checked);
            meshData.Show();
          //  StitchForm stitch = new StitchForm(displayGEOM);
          //  stitch.Show();
          //  UVStitchChart chart = new UVStitchChart(displayGEOM);
          //  chart.Show();
        }

        private void SlotRayDisplay_button_Click(object sender, EventArgs e)
        {
            SlotRayDataDisplay slotRays = new SlotRayDataDisplay(displayGEOM, GEOMDisplayFilename.Text);
            slotRays.Show();
        }

        private void GEOMDisplayFaceList_button_Click(object sender, EventArgs e)
        {
            GEOMFacesDisplay faces = new GEOMFacesDisplay(displayGEOM, GEOMDisplayFilename.Text);
            faces.Show();
        }

        private void GEOMDisplayData()
        {
            string myFile = GetFilename("Open sims 3 or 4 mesh file", GEOMfilter);
            if (String.CompareOrdinal(myFile, "") > 0)
            {
                if (!File.Exists(myFile))
                {
                    MessageBox.Show(myFile + " does not exist!");
                    return;
                }
                GetGEOMData(myFile, out displayGEOM);
                GEOMDisplayFilename.Text = myFile;
                GEOMdisplayVersion.Text = displayGEOM.meshVersion.ToString() + " ";
                if (displayGEOM.isTS4) GEOMdisplayVersion.Text += "(TS4)";
                else if (displayGEOM.meshVersion == 5) GEOMdisplayVersion.Text += "(TS3)";
                else GEOMdisplayVersion.Text += "(unknown)";
                string str = "";
                for (int i = 0; i < displayGEOM.vertexFormatList.Length; i++)
                    str = str + displayGEOM.vertexFormat[i].ToString() + Environment.NewLine;
                GEOMDisplayVertFormatList.Text = str;
                GEOMDisplayNumVerts.Text = displayGEOM.numberVertices.ToString();
                GEOMDisplayVertexIDmin.Text = displayGEOM.minVertexID.ToString();
                GEOMDisplayVertexIDmax.Text = displayGEOM.maxVertexID.ToString();
                GEOMDisplayNumFaces.Text = displayGEOM.numberFaces.ToString();
                string[] boneStr = new string[displayGEOM.BoneHashList.Length];
                for (int i = 0; i < displayGEOM.BoneHashList.Length; i++)
                {
                    string bone = adultRig.GetBoneName(displayGEOM.BoneHashList[i]);
                    if (bone.Length == 0) bone = adRig.GetBoneName(displayGEOM.BoneHashList[i]);
                    if (bone.Length == 0) bone = acRig.GetBoneName(displayGEOM.BoneHashList[i]);
                    // boneStr[i] = "0x" + Convert.ToString(displayGEOM.BoneHashList[i], 16).ToUpper().PadLeft(8, '0');
                    boneStr[i] = i.ToString() + ": 0x" + Convert.ToString(displayGEOM.BoneHashList[i], 16).ToUpper().PadLeft(8, '0') + " (" +
                        bone + ")";
                }
                GEOMDisplayBoneHashList.Lines = boneStr;
                string[] tgiStr = new string[displayGEOM.TGIList.Length];
                for (int i = 0; i < displayGEOM.TGIList.Length; i++)
                {
                    tgiStr[i] = displayGEOM.TGIList[i].ToString();
                    if (i == displayGEOM.Shader.diffuseIndex)
                    {
                        tgiStr[i] += " (Diffuse Texture)";
                    }
                    if (i == displayGEOM.Shader.normalIndex)
                    {
                        tgiStr[i] += " (Bumpmap/Normal Map)";
                    }
                    if (i == displayGEOM.Shader.specularIndex)
                    {
                        tgiStr[i] += " (Specular)";
                    }
                    if (displayGEOM.meshVersion == 5 && i == displayGEOM.skeletonIndex)
                    {
                        tgiStr[i] += " (Skeleton)";
                    }
                }

                if (displayGEOM.ShaderHash > 0)
                {
                    string tmp = Enum.GetName(typeof(XmodsEnums.Shader), displayGEOM.ShaderHash);
                    if (String.CompareOrdinal(tmp, "") <= 0) tmp = "Unknown";
                    GEOMDisplayShader.Text = tmp + " (" + Convert.ToString(displayGEOM.ShaderHash, 16).ToUpper().PadLeft(8, '0') + ")";
                    GEOMDisplayMTNFList.Lines = displayGEOM.Shader.ToStringArray();
                }
                else
                {
                    GEOMDisplayShader.Text = "";
                    GEOMDisplayMTNFList.Lines = new string[] { "" };
                }

                GEOMDisplayTGIlist.Lines = tgiStr;

                GEOMDisplayMergeGroup.Text = displayGEOM.mergeGroup.ToString();
                GEOMDisplaySortOrder.Text = displayGEOM.sortOrder.ToString();
                GEOMDisplayItemCount.Text = displayGEOM.numberSubMeshes.ToString();
                GEOMDisplayBytesPerFP.Text = displayGEOM.bytesPerFacePoint.ToString();
                GEOMDisplayVertData_panel.Enabled = true;
                GEOMDataID_checkbox.Enabled = displayGEOM.hasVertexIDs;
                GEOMDataPosition_checkbox.Enabled = displayGEOM.hasPositions;
                GEOMDataNormals_checkbox.Enabled = displayGEOM.hasNormals;
                GEOMDataUV_checkbox.Enabled = displayGEOM.hasUVs;
                GEOMDataStitch_checkBox.Enabled = (displayGEOM.UVStitches != null && displayGEOM.UVStitches.Length > 0);
                GEOMDataSeamStitch_checkBox.Enabled = (displayGEOM.SeamStitches != null && displayGEOM.SeamStitches.Length > 0);
                GEOMDataBones_checkbox.Enabled = displayGEOM.hasBones;
                GEOMDataTangents_checkbox.Enabled = displayGEOM.hasTangents;
                GEOMDataTags_checkbox.Enabled = displayGEOM.hasTags;
                SlotRayDisplay_button.Enabled = (displayGEOM.SlotrayAdjustments != null) && (displayGEOM.SlotrayAdjustments.Length > 0);
                GEOMDataID_checkbox.Checked = GEOMDataID_checkbox.Enabled;
                GEOMDataPosition_checkbox.Checked = GEOMDataPosition_checkbox.Enabled;
                GEOMDataNormals_checkbox.Checked = GEOMDataNormals_checkbox.Enabled;
                GEOMDataUV_checkbox.Checked = GEOMDataUV_checkbox.Enabled;
                GEOMDataStitch_checkBox.Checked = GEOMDataStitch_checkBox.Enabled;
                GEOMDataSeamStitch_checkBox.Checked = GEOMDataSeamStitch_checkBox.Enabled;
                GEOMDataBones_checkbox.Checked = GEOMDataBones_checkbox.Enabled;
                GEOMDataTangents_checkbox.Checked = GEOMDataTangents_checkbox.Enabled;
                GEOMDataTags_checkbox.Checked = GEOMDataTags_checkbox.Enabled;
            }
        }

        private void GEOMviewFile_button_Click(object sender, EventArgs e)
        {
            GEOMviewFilename.Text = GetFilename("Select GEOM mesh to view", GEOMfilter);
            GEOMview();
        }

        private void GEOMview()
        {
            GEOM viewGEOM;
            if (String.CompareOrdinal(GEOMviewFilename.Text, "") > 0)
            {
                if (!File.Exists(GEOMviewFilename.Text))
                {
                    MessageBox.Show(GEOMviewFilename.Text + " does not exist!");
                    return;
                }
                if (GetGEOMData(GEOMviewFilename.Text, out viewGEOM))
                {
                    GEOMviewer1.Start_Mesh(viewGEOM);
                }
                else
                {
                    MessageBox.Show("Can't read GEOM data!");
                    return;
                }
            }
            else { return; }
        }

        private void GEOMviewUVfile_button_Click(object sender, EventArgs e)
        {
            GEOMviewUVfile.Text = GetFilename("Select GEOM mesh to view", GEOMfilter);
            if (String.CompareOrdinal(GEOMviewUVfile.Text, "") > 0)
            {
                if (!File.Exists(GEOMviewUVfile.Text))
                {
                    MessageBox.Show(GEOMviewUVfile.Text + " does not exist!");
                    return;
                }
                if (GetGEOMData(GEOMviewUVfile.Text, out viewUVGEOM))
                {
                    if (viewUVGEOM.numberUVsets < 1)
                    {
                        MessageBox.Show("No uv sets found!");
                        return;
                    }
                    string[] maps = new string[viewUVGEOM.numberUVsets];
                    for (int i = 0; i < viewUVGEOM.numberUVsets; i++) { maps[i] = "UV_" + i.ToString(); }
                    GEOMviewUV_comboBox.Items.Clear();
                    GEOMviewUV_comboBox.Items.AddRange(maps);
                    GEOMviewUV_comboBox.SelectedIndex = 0;
                    GEOMviewUVmap();
                }
                else
                {
                    MessageBox.Show("Can't read GEOM data!");
                    return;
                }
            }
        }

        private void GEOMviewUVmap()
        {
            int selectedMap = GEOMviewUV_comboBox.SelectedIndex;
            Bitmap map = new Bitmap(GEOMUVmap_pictureBox.Width, GEOMUVmap_pictureBox.Height);
            using (Graphics g = Graphics.FromImage(map))
            {
                g.Clear(Color.White);
                Pen p = new Pen(Color.Black, 1);
                for (int i = 0; i < viewUVGEOM.numberFaces; i++)
                {
                    int[] faceIndices = viewUVGEOM.getFaceIndices(i);
                    Point[] facePoints = new Point[4];
                    for (int j = 0; j < 3; j++)
                    {
                        float[] uv = viewUVGEOM.getUV(faceIndices[j], selectedMap);
                        facePoints[j] = new Point((int)(((uv[0] + 1) / 2f) * GEOMUVmap_pictureBox.Width), (int)(uv[1] * GEOMUVmap_pictureBox.Height));
                    }
                    facePoints[3] = facePoints[0];
                    g.DrawLines(p, facePoints);
                }
                GEOMUVmap_pictureBox.Image = map;
            }
        }


        private void GEOMviewUV_comboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            GEOMviewUVmap();
        }

        private void GEOMcleanFile_button_Click(object sender, EventArgs e)
        {
            GEOMcleanFile.Text = GetFilename("Select GEOM to clean", GEOMfilter);
        }

        private void GEOMcleanGo_button_Click(object sender, EventArgs e)
        {
            GEOM cleanGEOM;
            if (String.CompareOrdinal(GEOMcleanFile.Text, "") > 0)
            {
                if (!File.Exists(GEOMcleanFile.Text))
                {
                    MessageBox.Show(GEOMcleanFile.Text + " does not exist!");
                    return;
                }
                if (GetGEOMData(GEOMcleanFile.Text, out cleanGEOM))
                {
                    cleanGEOM.Clean();
                    WriteGEOMFile("Save cleaned GEOM file", cleanGEOM, "");
                }
                else
                {
                    MessageBox.Show("Can't read GEOM data!");
                    return;
                }
            }
            else { return; }
        }

        private void GEOMcopyUVfrom_button_Click(object sender, EventArgs e)
        {
            GEOMcopyFrom.Text = GetFilename("Select GEOM mesh to copy UV from", GEOMfilter);
        }

        private void GEOMcopyUVto_button_Click(object sender, EventArgs e)
        {
            GEOMcopyTo.Text = GetFilename("Select GEOM mesh to copy UV to", GEOMfilter);
        }

        private void GEOMcopyGo_button_Click(object sender, EventArgs e)
        {
            if (String.CompareOrdinal(GEOMcopyFrom.Text, "") > 0 && String.CompareOrdinal(GEOMcopyTo.Text, "") > 0)
            {
                GEOM sourceGEOM;
                GEOM targetGEOM;
                if (!File.Exists(GEOMcopyFrom.Text))
                {
                    MessageBox.Show(GEOMcopyFrom.Text + " does not exist!");
                    return;
                }
                if (!File.Exists(GEOMcopyTo.Text))
                {
                    MessageBox.Show(GEOMcopyTo.Text + " does not exist!");
                    return;
                }
                if (GetGEOMData(GEOMcopyFrom.Text, out sourceGEOM) && GetGEOMData(GEOMcopyTo.Text, out targetGEOM))
                {
                    if (GEOMcopyByID_radioButton.Checked)
                    {
                        if (!sourceGEOM.hasVertexIDs || !targetGEOM.hasVertexIDs)
                        {
                            DialogResult res = MessageBox.Show("One or both meshes do not have vertex IDs. Continue using vertex sequence?", "No vertex IDs", MessageBoxButtons.OKCancel);
                            if (res == DialogResult.Cancel) return;
                            GEOMcopyBySeq_radioButton.Checked = true;
                        }
                    }
                    
                    if (GEOMcopyBySeq_radioButton.Checked)
                    {
                        if (GEOMcopyPosition_checkBox.Checked) targetGEOM.ReplacePositions(sourceGEOM);
                        if (GEOMcopyNormals_checkBox.Checked) targetGEOM.ReplaceNormals(sourceGEOM);
                        if (GEOMcopyUV0_checkBox.Checked) targetGEOM.ReplaceUV(sourceGEOM, 0);
                        if (GEOMcopyUV1_checkBox.Checked && sourceGEOM.hasUVset(1) && targetGEOM.hasUVset(1)) targetGEOM.ReplaceUV(sourceGEOM, 1);
                        if (GEOMcopyUV2_checkBox.Checked && sourceGEOM.hasUVset(2) && targetGEOM.hasUVset(2)) targetGEOM.ReplaceUV(sourceGEOM, 2);
                        if (GEOMcopyBones_checkBox.Checked) targetGEOM.ReplaceBones(sourceGEOM);
                        if (GEOMcopyColor_checkBox.Checked) targetGEOM.ReplaceTagvals(sourceGEOM);
                    }
                    else
                    {
                        if (GEOMcopyPosition_checkBox.Checked) targetGEOM.ReplacePositionsByID(sourceGEOM);
                        if (GEOMcopyNormals_checkBox.Checked) targetGEOM.ReplaceNormalsByID(sourceGEOM);
                        if (GEOMcopyUV0_checkBox.Checked) targetGEOM.ReplaceUVByID(sourceGEOM, 0);
                        if (GEOMcopyUV1_checkBox.Checked && sourceGEOM.hasUVset(1) && targetGEOM.hasUVset(1)) targetGEOM.ReplaceUVByID(sourceGEOM, 1);
                        if (GEOMcopyUV2_checkBox.Checked && sourceGEOM.hasUVset(2) && targetGEOM.hasUVset(2)) targetGEOM.ReplaceUVByID(sourceGEOM, 2);
                        if (GEOMcopyBones_checkBox.Checked) targetGEOM.ReplaceBonesByID(sourceGEOM);
                        if (GEOMcopyColor_checkBox.Checked) targetGEOM.ReplaceTagvalsByID(sourceGEOM);
                    }
                    WriteGEOMFile("Save result GEOM file", targetGEOM, "");
                }
                else
                {
                    MessageBox.Show("Can't read one or both GEOM files!");
                    return;
                }
            }
            else { MessageBox.Show("You must enter 'from' and 'to' GEOM meshes!"); return; }

        }

        GEOM removeGEOM;

        private void GeomRemoveFrom_button_Click(object sender, EventArgs e)
        {
            GeomRemoveFrom.Text = GetFilename("Select mesh to remove data from", GEOMfilter);
            if (String.CompareOrdinal(GeomRemoveFrom.Text, "") > 0)
            {
                if (!File.Exists(GeomRemoveFrom.Text))
                {
                    MessageBox.Show(GeomRemoveFrom.Text + " does not exist!");
                    return;
                }
                if (GetGEOMData(GeomRemoveFrom.Text, out removeGEOM))
                {
                    RemoveID_checkBox.Enabled = removeGEOM.hasVertexIDs;
                    RemovePosition_checkBox.Enabled = removeGEOM.hasPositions;
                    RemoveNormals_checkBox.Enabled = removeGEOM.hasNormals;
                    RemoveUV0_checkBox.Enabled = removeGEOM.hasUVset(0);
                    RemoveUV1_checkBox.Enabled = removeGEOM.hasUVset(1);
                    RemoveUV2_checkBox.Enabled = removeGEOM.hasUVset(2);
                    RemoveBones_checkBox.Enabled = removeGEOM.hasBones;
                    RemoveTangents_checkBox.Enabled = removeGEOM.hasTangents;
                    RemoveColor_checkBox.Enabled = removeGEOM.hasTags;
                 //   RemoveUVstitch_checkBox.Enabled = removeGEOM.has
                }
                else
                {
                    MessageBox.Show("Can't read GEOM file!");
                    return;
                }
            }
            else { MessageBox.Show("You must enter a GEOM mesh!"); return; }
        }

        private void RemoveData_button_Click(object sender, EventArgs e)
        {

        }

    }
}
