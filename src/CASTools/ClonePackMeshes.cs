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
using s4pi.ImageResource;
//using s4pi.Settings;

namespace XMODS
{
    public partial class Form1 : Form
    {
        List<GEOM> clonePackMeshes = new List<GEOM>();
        List<TGI> tgiMeshes = new List<TGI>();
        List<bool> clonedMesh = new List<bool>();
        List<bool> newMeshImport = new List<bool>();
        bool changesRegionMap = false;
        List<GEOM>[] LODmeshes = new List<GEOM>[4];
        List<TGI>[] LODtgis = new List<TGI>[4];
        string meshName;
        RegionMap meshRegionMap, backupRegionMap;
        IResourceIndexEntry iresRMap;
        DSTResource meshBumpMap, meshGlowMap;
        DdsFile ddsBumpmap, ddsGlowMap;
        IResourceIndexEntry iresBumpMap, iresGlow;
        bool clonedRMap, clonedBumpMap, importedBumpMap, clonedGlow, importedGlow, changedCASPs;
        int currentRowMesh = 0, currentRowRegion = 0;

        string[] shaderNames = new string[] { "SimSkin", "SimGlass", "Phong", "SimGhost" };
        static GEOM.MTNF SimShader = new GEOM.MTNF(new uint[] { 1179538509, 0, 168, 17, 1942590014, 1, 3, 288, 4280935609, 1, 2, 300, 3123518137, 1, 2, 308, 3287985231, 4, 4, 316, 1011213108, 1, 1, 332, 97660883, 1, 1, 336, 3668529965, 1, 1, 340, 700243231, 1, 1, 344, 1851151498, 65540, 4, 348, 2189874156, 1, 1, 364, 1003766176, 1, 3, 368, 1824587141, 4, 4, 380, 77978275, 1, 3, 396, 4149606399, 1, 1, 408, 752949314, 1, 3, 412, 2907867744, 65540, 4, 424, 1669179909, 1, 4, 440, 
            0, 0, 0, 1065353216, 1065353216, 1065353216, 1065353216, 0, 0, 0, 0, 1065353216, 1065353216, 1065353216, 1056964608, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1101004800, 1065353216, 1065353216, 1065353216, 2, 0, 0, 0, 1065353216, 1065353216, 1065353216, 1065353216 });
        
        private void GetClonePackMeshes()
        {
            PackageMesh_dataGridView.Rows.Clear();
            for (int i = 0; i < 4; i++)
            {
                LODmeshes[i] = new List<GEOM>();
                LODtgis[i] = new List<TGI>();
            }
            clonePackMeshes.Clear();
            tgiMeshes.Clear();
            clonedMesh.Clear();
            newMeshImport.Clear();
            meshRegionMap = null;
            backupRegionMap = null;
            changesRegionMap = false;
            iresRMap = null;
            clonedRMap = false;
            iresBumpMap = null;
            clonedBumpMap = false;
            importedBumpMap = false;
            changedCASPs = false;
            iresGlow = null;
            clonedGlow = false;
            importedGlow = false;
            int ind = myCASP.PartName.LastIndexOf("_");
            if (ind >= 0)
            {
                meshName = myCASP.PartName.Substring(0, ind);
            }
            else
            {
                meshName = myCASP.PartName;
            }
            foreach (TGI tgi in myCASP.LinkList)
            {
                if (tgi.Type == (uint)XmodsEnums.ResourceTypes.GEOM)
                {
                    bool foundMesh = false;
                    Predicate<IResourceIndexEntry> getGEOM = r => r.ResourceType == (uint)XmodsEnums.ResourceTypes.GEOM &
                                                                  r.ResourceGroup == tgi.Group & r.Instance == tgi.Instance;
                    IResourceIndexEntry ires = clonePack.Find(getGEOM);
                    if (ires != null)
                    {
                        GEOM tmp;
                        Stream s = clonePack.GetResource(ires);
                        s.Position = 0;
                        tmp = new GEOM(new BinaryReader(s));
                        if (tmp.UpdateToLatestVersion(SelectRig(myCASP.Species, myCASP.Age)))
                        {
                            tmp.AutoSeamStitches(myCASP.Species, myCASP.Age, myCASP.Gender, myCASP.getLOD(new TGI(ires.ResourceType, ires.ResourceGroup, ires.Instance)));
                            //if (myCASP.BodyType == XmodsEnums.BodyType.Top || myCASP.BodyType == XmodsEnums.BodyType.Body || myCASP.BodyType == XmodsEnums.BodyType.Bottom)
                            //{
                            //    tmp.AutoSlotray(SelectSlotRayData(myCASP.Species, myCASP.Age));
                            //}
                            MemoryStream sw = new MemoryStream();
                            BinaryWriter bw = new BinaryWriter(sw);
                            tmp.WriteFile(bw);
                            sw.Position = 0;
                            ReplaceResource(clonePack, ires, sw);
                        }
                        clonePackMeshes.Add(tmp);
                        tgiMeshes.Add(new TGI(ires.ResourceType, ires.ResourceGroup, ires.Instance));
                        clonedMesh.Add(true);
                        newMeshImport.Add(false);
                        foundMesh = true;
                    }
                    else
                    {
                        foreach (Package p in gamePacks0)
                        {
                            ires = p.Find(getGEOM);
                            if (ires != null)
                            {
                                GEOM tmp;
                                using (Stream s = p.GetResource(ires))
                                {
                                    s.Position = 0;
                                    tmp = new GEOM(new BinaryReader(s));
                                }
                                if (tmp.UpdateToLatestVersion(SelectRig(myCASP.Species, myCASP.Age)))
                                {
                                    tmp.AutoSeamStitches(myCASP.Species, myCASP.Age, myCASP.Gender, myCASP.getLOD(new TGI(ires.ResourceType, ires.ResourceGroup, ires.Instance)));
                                }
                                clonePackMeshes.Add(tmp);
                                tgiMeshes.Add(new TGI(ires.ResourceType, ires.ResourceGroup, ires.Instance));
                                clonedMesh.Add(false);
                                newMeshImport.Add(false);
                                foundMesh = true;
                                break;
                            }
                        }
                    }
                    if (!foundMesh)
                    {
                        MessageBox.Show("Can't find mesh: " + tgi.ToString());
                    }
                }
                else if (tgi.Type == (uint)XmodsEnums.ResourceTypes.RMAP)
                {
                    bool foundMAP = false;
                    Predicate<IResourceIndexEntry> getRMAP = r => r.ResourceType == (uint)XmodsEnums.ResourceTypes.RMAP &
                                              r.ResourceGroup == tgi.Group & r.Instance == tgi.Instance;
                    IResourceIndexEntry ires = clonePack.Find(getRMAP);
                    if (ires != null)
                    {
                        Stream s = clonePack.GetResource(ires);
                        meshRegionMap = new RegionMap(new BinaryReader(s));
                       // using (Stream s = clonePack.GetResource(ires))
                       // {
                       //     s.Position = 0;
                       //     meshRegionMap = new RegionMap(new BinaryReader(s));
                       // }
                        iresRMap = ires;
                        clonedRMap = true;
                        foundMAP = true;
                    }
                    else
                    {
                        foreach (Package p in gamePacks0)
                        {
                            ires = p.Find(getRMAP);
                            if (ires != null)
                            {
                                using (Stream s = p.GetResource(ires))
                                {
                                    s.Position = 0;
                                    meshRegionMap = new RegionMap(new BinaryReader(s));
                                }
                                iresRMap = ires;
                                clonedRMap = false;
                                foundMAP = true;
                                break;
                            }
                        }
                    }
                    if (foundMAP)
                    {
                        backupRegionMap = new RegionMap(meshRegionMap);
                    }
                    else
                    {
                        MessageBox.Show("Can't find RegionMap: " + tgi.ToString());
                    }
                }
            }

            if (clonePackMeshes.Count > 0)
            {
                for (int i = 0; i < clonePackMeshes.Count; i++)
                {
                    meshBumpMap = FindCloneTextureDST(clonePackMeshes[i].TGIList, clonePackMeshes[i].BumpMapIndex, out iresBumpMap, out clonedBumpMap);
                    if (meshBumpMap != null) break;
                }
                if (meshBumpMap != null)
                {
                    try
                    {
                        ddsBumpmap = new DdsFile();
                        ddsBumpmap.Load(meshBumpMap.ToDDS(), true);
                        bumpmapImage = ddsBumpmap.Image;
                    }
                    catch (Exception e)
                    {
                        ddsBumpmap = null;
                        bumpmapImage = null;
                    }
                }
                else
                {
                    ddsBumpmap = null;
                    bumpmapImage = null;
                }
                for (int i = 0; i < clonePackMeshes.Count; i++)
                {
                    meshGlowMap = FindCloneTextureDST(clonePackMeshes[i].TGIList, clonePackMeshes[i].EmissionMapIndex, out iresGlow, out clonedGlow);
                    if (meshGlowMap != null) break;
                }
                if (meshGlowMap != null)
                {
                    try
                    {
                        ddsGlowMap = new DdsFile();
                        ddsGlowMap.Load(meshGlowMap.ToDDS(), false);
                        glowImage = ddsGlowMap.Image;
                    }
                    catch (Exception e)
                    {
                        ddsGlowMap = null;
                        glowImage = null;
                    }
                }
                else
                {
                    ddsGlowMap = null;
                    glowImage = null;
                }

                for (int i = 0; i < clonePackMeshes.Count; i++)
                {
                    TGI tgi = new TGI(tgiMeshes[i]);
                    int lod = myCASP.getLOD(tgi);
                    LODmeshes[lod].Add(clonePackMeshes[i]);
                    LODtgis[lod].Add(tgi);
                }
            }
            else
            {
                ddsBumpmap = null;
                bumpmapImage = null;
                ddsGlowMap = null;
                glowImage = null;
            }
        }

        private void ListClonePackMeshes()
        {
            if (tgiMeshes.Count == 0)
            {
                clonedBumpMap = false;
                importedBumpMap = false;
                return;
            }
            PackageMesh_dataGridView.CellValueChanged -= PackageMesh_dataGridView_CellValueChanged;

            if (MeshViewPart_radioButton.Checked)
            {
                for (int i = 0; i < tgiMeshes.Count; i++)
                {
                    string[] tmp = new string[5];
                    TGI meshTGI = new TGI(tgiMeshes[i]);
                    int[] lodpart = myCASP.getLODandPart(meshTGI);
                    tmp[1] = clonePackMeshes[i].numberFaces.ToString();
                    if (lodpart != null)
                    {
                        tmp[0] = meshName + "_lod" + lodpart[0].ToString() + "_" + lodpart[1].ToString();
                        tmp[2] = lodpart[0].ToString();
                        tmp[3] = lodpart[1].ToString();
                    }
                    else
                    {
                        tmp[0] = meshName;
                        tmp[2] = "?";
                        tmp[3] = "?";
                    }
                    uint[] meshRegions = new uint[0];
                    if (meshRegionMap != null)
                    {
                        meshRegions = meshRegionMap.GetMeshRegions(meshTGI);
                    }
                    string s = "";
                    foreach (uint r in meshRegions)
                    {
                        s += (Enum.IsDefined(typeof(XmodsEnums.CASPartRegionTS4), r) ?
                            Enum.GetName(typeof(XmodsEnums.CASPartRegionTS4), r) : r.ToString()) + ", ";               
                    }
                    tmp[4] = s.TrimEnd(new char[] { ',', ' ' });
                    int ind = PackageMesh_dataGridView.Rows.Add(tmp);
                    DataGridViewComboBoxCell shader = (DataGridViewComboBoxCell)PackageMesh_dataGridView.Rows[ind].Cells["PackageMeshShader"];
                    shader.Items.Clear();
                    shader.Items.AddRange(Enum.GetNames(typeof(XmodsEnums.SimShader)));
                    shader.Value = clonePackMeshes[i].ShaderName;
                    shader.Tag = clonePackMeshes[i].ShaderName;
                    PackageMesh_dataGridView.Rows[ind].Tag = new int[] { i };
                }
                PackageMesh_dataGridView.Sort(PackageMesh_dataGridView.Columns["PackageMeshName"], ListSortDirection.Ascending);
                PackageMesh_dataGridView.ClearSelection();
                PackageMesh_dataGridView.Rows[0].Selected = true;
                PackageMesh_dataGridView.Columns["MeshPart"].HeaderText = "Mesh Part";
                PackageMesh_dataGridView.Columns["DeleteMesh"].Visible = true;
                MeshExportTS4_radioButton.Enabled = true;
            }

            else if (MeshViewLOD_radioButton.Checked)
            {
                string[] meshname = new string[4];
                int[] LODsize = new int[4];
                List<int>[] indexes = new List<int>[4];
                for (int i = 0; i < 4; i++)
                {
                    indexes[i] = new List<int>();
                }
                string[] shaders = new string[] { "", "", "", "" };
                List<uint>[] regions = new List<uint>[4] { new List<uint>(), new List<uint>(), new List<uint>(), new List<uint>() };
                for (int i = 0; i < tgiMeshes.Count; i++)
                {
                    TGI meshTGI = new TGI(tgiMeshes[i]);
                    int[] lodpart = myCASP.getLODandPart(meshTGI);
                    
                    if (lodpart != null)
                    {
                        LODsize[lodpart[0]] += clonePackMeshes[i].numberFaces;
                        indexes[lodpart[0]].Add(i);
                        if (string.Compare(shaders[lodpart[0]], "") == 0)
                        {
                            shaders[lodpart[0]] = clonePackMeshes[i].ShaderName;
                        }
                        else if (string.Compare(shaders[lodpart[0]], clonePackMeshes[i].ShaderName) != 0)
                        {
                            shaders[lodpart[0]] = "Mixed";
                        }
                        uint[] meshRegions = new uint[0];
                        if (meshRegionMap != null)
                        {
                            meshRegions = meshRegionMap.GetMeshRegions(meshTGI);
                        }
                        regions[lodpart[0]].AddRange(meshRegions);
                    }
                    else
                    {
                        PackageMesh_dataGridView.Rows.Add("Can't determine LODs!");
                    }
                }
                for (int i = 0; i < 4; i++)
                {
                    string[] tmp = new string[5];
                    if (LODsize[i] > 0)
                    {
                        tmp[0] = meshName + "_lod" + i.ToString();
                        tmp[1] = LODsize[i].ToString();
                        tmp[2] = i.ToString();
                        tmp[3] = indexes[i].Count.ToString();
                        string s = "";
                        foreach (uint r in regions[i].Distinct())
                        {
                            s += (Enum.IsDefined(typeof(XmodsEnums.CASPartRegionTS4), r) ?
                                Enum.GetName(typeof(XmodsEnums.CASPartRegionTS4), r) : r.ToString()) + ", "; 
                        }
                        tmp[4] = s.TrimEnd(new char[] { ',', ' ' });
                        int ind = PackageMesh_dataGridView.Rows.Add(tmp);
                        DataGridViewComboBoxCell shader = (DataGridViewComboBoxCell)PackageMesh_dataGridView.Rows[ind].Cells["PackageMeshShader"];
                        shader.Items.Clear();
                        shader.Items.Add("Mixed");
                        shader.Items.AddRange(Enum.GetNames(typeof(XmodsEnums.SimShader)));
                        shader.Value = shaders[i];
                        shader.Tag = shaders[i];
                        PackageMesh_dataGridView.Rows[ind].Tag = indexes[i].ToArray();
                    }
                }
                PackageMesh_dataGridView.Columns["MeshPart"].HeaderText = "Mesh Parts";
                PackageMesh_dataGridView.Columns["DeleteMesh"].Visible = false;
                MeshExportTS4_radioButton.Enabled = !myCASP.MultipleMeshParts;
            }

            PackageMesh_dataGridView.Rows[0].Selected = true;
            PackageMesh_dataGridView.CellValueChanged += PackageMesh_dataGridView_CellValueChanged;
        }

        private void MeshViewLOD_CheckedChanged(object sender, EventArgs e)
        {
            if (MeshViewLOD_radioButton.Checked && myCASP.MultipleMeshParts)
            {
                MeshExportTS4_radioButton.Checked = false;
                MeshExportTS4_radioButton.Enabled = false;
            }
            else
            {
                MeshExportTS4_radioButton.Enabled = true;
            }
            if (!MeshExportMS3D_radioButton.Checked & !MeshExportOBJ_radioButton.Checked &
                !MeshExportDAE_radioButton.Checked & !MeshExportTS4_radioButton.Checked) MeshExportMS3D_radioButton.Checked = true;
            PackageMesh_dataGridView.Rows.Clear();
            ListClonePackMeshes();
        }

        private void PackageMesh_dataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            if (currentRowMesh != e.RowIndex && changesMesh)
            {
                DialogResult res = MessageBox.Show("You have unsaved mesh changes which may be lost if you switch to another mesh!", "Unsaved changes", MessageBoxButtons.OKCancel);
                if (res == DialogResult.Cancel)
                {
                    PackageMesh_dataGridView.Rows[e.RowIndex].Selected = false;
                    PackageMesh_dataGridView.Rows[currentRowMesh].Selected = true;
                    return;
                }
            }

            currentRowMesh = e.RowIndex;
            int[] meshIndexes = (int[])PackageMesh_dataGridView.Rows[e.RowIndex].Tag;
            GEOM[] geomArray = new GEOM[meshIndexes.Length];
            uint[][] regionArray = new uint[meshIndexes.Length][];
            for (int i = 0; i < geomArray.Length; i++)
            {
                geomArray[i] = clonePackMeshes[meshIndexes[i]];
                if (meshRegionMap != null)
                {
                    regionArray[i] = meshRegionMap.GetMeshRegions(tgiMeshes[meshIndexes[i]]);
                }
                else
                {
                    regionArray[i] = new uint[0];
                }
            }

            if (PackageMesh_dataGridView.Columns["ExportMesh"].Index == e.ColumnIndex)
            {
                int uvSet;
                if (MeshExportUV0_radioButton.Checked)
                {
                    uvSet = 0;
                }
                else
                {
                    foreach (GEOM g in geomArray)
                    {
                        if (!g.hasUVset(1))
                        {
                            MessageBox.Show("At least one mesh does not have a second UV set! Please select first UV set for export or export separate mesh parts.");
                            return;
                        }
                    }
                    uvSet = 1;
                }
                if (MeshExportMS3D_radioButton.Checked)
                {
                    RIG rig = SelectRig(myCASP.Species, myCASP.Age);
                    if (rig == null)
                    {
                        MessageBox.Show("No rig is available for " + myCASP.Species.ToString() + ", " + myCASP.Age.ToString());
                        return;
                    }
                    try
                    {
                        MS3D ms3d = new MS3D(geomArray, null, rig, uvSet);
                        string tmp = WriteMS3DFile("Save MS3D file", ms3d, "MS3D_" +
                            PackageMesh_dataGridView.Rows[e.RowIndex].Cells["PackageMeshName"].Value.ToString() + ((uvSet > 0) ? "_uv" + uvSet.ToString() : ""));
                    }
                    catch (MS3D.MeshException em)
                    {
                        MessageBox.Show(em.Message);
                        return;
                    }
                }

                else if (MeshExportTS4_radioButton.Checked)
                {
                    GEOM geom = new GEOM(geomArray[0]);
                    RIG rig = SelectRig(myCASP.Species, myCASP.Age);
                    for (int i = 1; i < geomArray.Length; i++)
                    {
                        geom.AppendMesh(geomArray[i], rig, true);
                    }
                    string tmp = WriteGEOMFile("Save mesh file", geom,
                        "S4_" + PackageMesh_dataGridView.Rows[e.RowIndex].Cells["PackageMeshName"].Value.ToString());
                }

                else if (MeshExportDAE_radioButton.Checked)
                {
                    RIG rig = SelectRig(myCASP.Species, myCASP.Age);
                    if (rig == null)
                    {
                        MessageBox.Show("No rig is available for " + myCASP.Species.ToString() + ", " + myCASP.Age.ToString());
                        return;
                    }
                    try
                    {
                        DAE dae = new DAE(geomArray, null, rig, MeshExportYup_radioButton.Checked);
                        string tmp = WriteDAEFile("Save Collada DAE file", dae, false, "DAE_" +
                            PackageMesh_dataGridView.Rows[e.RowIndex].Cells["PackageMeshName"].Value.ToString() + ((uvSet > 0) ? "_uv" + uvSet.ToString() : ""));
                    }
                    catch (ApplicationException ea)
                    {
                        MessageBox.Show(ea.Message);
                        return;
                    }
                }

                    //GEOM S3geom = ConvertS4toS3(geomArray[0], uvSet);
                    //string tmp = WriteGEOMFile("Save converted mesh", S3geom,
                    //    "S3_" + PackageMesh_dataGridView.Rows[e.RowIndex].Cells["PackageMeshName"].Value.ToString() + ((uvSet > 0) ? "_uv" + uvSet.ToString() : ""));

                else if (MeshExportOBJ_radioButton.Checked)
                {
                    OBJ obj = new OBJ(geomArray, null, uvSet);
                    string tmp = WriteOBJFile("Save converted mesh", obj,
                        PackageMesh_dataGridView.Rows[e.RowIndex].Cells["PackageMeshName"].Value.ToString());
                }
            }

            if (PackageMesh_dataGridView.Columns["ImportMesh"].Index == e.ColumnIndex)
            {
                string meshfilename = "";
                if (MeshViewLOD_radioButton.Checked)
                {
                    meshfilename = GetFilename("Select an MS3D or DAE mesh to import", MultipartMeshImportFilter);
                }
                else if (MeshViewPart_radioButton.Checked)
                {
                    meshfilename = GetFilename("Select a GEOM, SIMGEOM, MS3D, or DAE mesh to import", AllMeshImportFilter);
                }
                
                if (String.Compare(meshfilename, " ") <= 0)
                {
                    MessageBox.Show("You must select a mesh to import!");
                    return;
                }

                if (String.Compare(Path.GetExtension(meshfilename), ".ms3d", true) == 0)
                {
                    MS3D ms3d = null;
                    if (GetMS3DData(meshfilename, out ms3d, true))
                    {
                        GEOM refMesh = GetBodyMesh(myCASP.Species, myCASP.Age, myCASP.Gender,
                            myCASP.getLOD(tgiMeshes[meshIndexes[0]]), XmodsEnums.BodyType.All, myCASP.BodySubType, false);
                        GEOM[] geoms = GEOM.GEOMsFromMS3D(ms3d, refMesh, null);
                        if (geoms.Length != geomArray.Length)
                        {
                            MessageBox.Show("You must import the same number of mesh groups as the number of meshparts in the original TS4 mesh(es)!");
                            return;
                        }
                        for (int m = 0; m < geoms.Length; m++)
                        {
                            if (MeshImportUV1_checkBox.Checked)
                            {
                                clonePackMeshes[meshIndexes[m]] = GEOM.SetMorphUV(geomArray[m], geoms[m]);
                                newMeshImport[meshIndexes[m]] = true;
                                changesMesh = true;
                            }
                            else
                            {
                                GEOM tmp1 = ApplyOriginalMesh(geoms[m], geomArray[m], false, myCASP.Species, 
                                    myCASP.Age, myCASP.Gender, myCASP.getLOD(tgiMeshes[meshIndexes[m]]), myCASP.BodyType);
                                if (tmp1 != null)
                                {
                                    clonePackMeshes[meshIndexes[m]] = tmp1;
                                    newMeshImport[meshIndexes[m]] = true;
                                    changesMesh = true;
                                }
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Error importing mesh!");
                    }
                }

                else if (String.Compare(Path.GetExtension(meshfilename), ".dae", true) == 0)
                {
                    DAE dae = null;
                    if (GetDAEData(meshfilename, false, out dae, true))
                    {
                        GEOM refMesh = GetBodyMesh(myCASP.Species, myCASP.Age, myCASP.Gender,
                            myCASP.getLOD(tgiMeshes[meshIndexes[0]]), XmodsEnums.BodyType.All, myCASP.BodySubType, false);
                        GEOM[] geoms = GEOM.GEOMsFromDAE(dae, refMesh, null);
                        if (geoms.Length != geomArray.Length)
                        {
                            MessageBox.Show("You must import the same number of mesh groups as the number of meshparts in the original TS4 mesh(es)!");
                            return;
                        }
                        for (int m = 0; m < geoms.Length; m++)
                        {
                            if (geoms[m].hasUVset(1)) geoms[m].AutoUV1Stitches();
                            int myLOD = myCASP.getLOD(tgiMeshes[meshIndexes[m]]);
                            refMesh = GetBodyMesh(myCASP.Species, myCASP.Age, myCASP.Gender, myLOD, XmodsEnums.BodyType.All, myCASP.BodySubType, false);
                            geoms[m].AutoSeamStitches(myCASP.Species, myCASP.Age, myCASP.Gender, myLOD);
                            geoms[m].AutoSlotray(SelectSlotRayData(myCASP.Species, myCASP.Age));
                            geoms[m].AutoSeamNormals(refMesh, myCASP.Species, myCASP.Age, myCASP.Gender, myLOD);
                            if (geoms[m].hasTags)
                            {
                                uint newalpha = (uint)myCASP.BodyType << 24;
                                for (int j = 0; j < geoms[m].numberVertices; j++)
                                {
                                    geoms[m].setTagval(j, (geoms[m].getTagval(j) & 0x00FFFFFF) + newalpha);
                                }
                            }
                            for (int j = 0; j < m; j++)
                            {
                                GEOM.MatchSeamVerts(geoms[m], geoms[j]);
                            }
                            clonePackMeshes[meshIndexes[m]] = geoms[m];
                            newMeshImport[meshIndexes[m]] = true;
                            changesMesh = true;
                        }
                    }
                    else
                    {
                        MessageBox.Show("Error importing mesh!");
                    }
                }

                else if (String.Compare(Path.GetExtension(meshfilename), ".geom", true) == 0 |
                         String.Compare(Path.GetExtension(meshfilename), ".simgeom", true) == 0)
                {
                    if (geomArray.Length > 1)
                    {
                        MessageBox.Show("You cannot import a GEOM into a LOD with more than one meshpart!");
                        return;
                    }
                    GEOM geom = new GEOM();
                    if (!GetGEOMData(meshfilename, out geom))
                    {
                        MessageBox.Show("Can't read GEOM file!");
                        return;
                    }
                    if (!((geom.meshVersion == 5 & geom.isBase) | geom.isTS4))
                    {
                        MessageBox.Show("The mesh file is not a valid TS4 or TS3 base mesh format!");
                        return;
                    }
                    if (geom.meshVersion == 5)
                    {
                        GEOM S4geom = geomArray[0];
                        if (!S4geom.isTS4)
                        {
                            MessageBox.Show("The original mesh is not an S4 mesh!");
                            return;
                        }
                        geom = ConvertS3toS4(geom);
                        if (MeshImportUV1_checkBox.Checked)
                        {
                            clonePackMeshes[meshIndexes[0]] = GEOM.SetMorphUV(S4geom, geom);
                            newMeshImport[meshIndexes[0]] = true;
                            changesMesh = true;
                        }
                        else
                        {
                            GEOM tmp = ApplyOriginalMesh(geom, S4geom, true, myCASP.Species,
                                myCASP.Age, myCASP.Gender, myCASP.getLOD(tgiMeshes[meshIndexes[0]]), myCASP.BodyType);
                            if (tmp != null)
                            {
                                geom = tmp;
                                geom.AutoSeamStitches(myCASP.Species, myCASP.Age, myCASP.Gender, myCASP.getLOD(tgiMeshes[meshIndexes[0]]));
                                clonePackMeshes[meshIndexes[0]] = geom;
                                newMeshImport[meshIndexes[0]] = true;
                                changesMesh = true;
                            }
                        }
                    }
                    else
                    {
                        clonePackMeshes[meshIndexes[0]] = geom;
                        newMeshImport[meshIndexes[0]] = true;
                        changesMesh = true;
                    }
                }
            }

            else if (PackageMesh_dataGridView.Columns["DeleteMesh"].Index == e.ColumnIndex)
            {
                DialogResult res = MessageBox.Show("Are you sure you want to delete mesh " + PackageMesh_dataGridView.Rows[e.RowIndex].Cells["PackageMeshName"].Value.ToString() + "?"
                    + Environment.NewLine + "This cannot be undone!", "Delete Mesh", MessageBoxButtons.OKCancel);
                if (res == DialogResult.Cancel) return;

                TGI tgi = new TGI(tgiMeshes[meshIndexes[0]]);

                meshRegionMap.RemoveLink(tgi);
                IResourceKey ik;
                if (clonedRMap)
                {
                    DeleteResource(clonePack, iresRMap);
                    ik = new TGIBlock(0, null, iresRMap.ResourceType, iresRMap.ResourceGroup, iresRMap.Instance);
                }
                else
                {
                    TGI oldtgi = new TGI(iresRMap.ResourceType, iresRMap.ResourceGroup, iresRMap.Instance);
                    TGI newtgi = new TGI(iresRMap.ResourceType,
                        iresRMap.ResourceGroup | 0x80000000, FNVhash.FNV64(meshName) | 0x8000000000000000);
                    for (int j = 0; j < clonePackCASPs.Count; j++)
                    {
                        clonePackCASPs[j].Casp.replaceLink(oldtgi, newtgi);
                    }
                    ik = new TGIBlock(0, null, newtgi.Type, newtgi.Group, newtgi.Instance);
                    clonedRMap = true;
                    meshRegionMap.setInternalLink(newtgi);
                }
                MemoryStream mg = new MemoryStream();
                BinaryWriter bwg = new BinaryWriter(mg);
                meshRegionMap.Write(bwg);
                iresRMap = clonePack.AddResource(ik, mg, false);
                iresRMap.Compressed = (ushort)0x5A42;

                for (int i = 0; i < clonePackCASPs.Count; i++)
                {
                    clonePackCASPs[i].Casp.RemoveMeshLink(tgi);
                    clonePackCASPs[i].Casp.RebuildLinkList();
                    DeleteResource(clonePack, iresCASPs[i]);
                    Stream s = new MemoryStream();
                    BinaryWriter bw = new BinaryWriter(s);
                    clonePackCASPs[i].Casp.Write(bw);
                    clonePack.AddResource(iresCASPs[i], s, true);
                }

                DeleteResource(clonePack, tgi);

                GetClonePackMeshes();
                ListClonePackMeshes();
                ListClonePackRegions();
                StartPreview(myCASP.PartName);
            }
        }

        private void PackageMesh_ExportMS3D_button_Click(object sender, EventArgs e)
        {
            int uvSet;
            if (MeshExportUV0_radioButton.Checked)
            {
                uvSet = 0;
            }
            else
            {
                foreach (GEOM g in clonePackMeshes)
                {
                    if (!g.hasUVset(1))
                    {
                        MessageBox.Show("At least one mesh does not have a second UV set! Please select first UV set for export or export separate mesh parts.");
                        return;
                    }
                }
                uvSet = 1;
            }
            RIG rig = SelectRig(myCASP.Species, myCASP.Age);
            if (rig == null)
            {
                MessageBox.Show("No rig is available for " + myCASP.Species.ToString() + ", " + myCASP.Age.ToString());
                return;
            }
            string[] groupNames = new string[clonePackMeshes.Count];
            for (int i = 0; i < clonePackMeshes.Count; i++)
            {
                TGI meshTGI = new TGI(tgiMeshes[i]);
                int[] lodpart = myCASP.getLODandPart(meshTGI);
                groupNames[i] = "LOD" + lodpart[0].ToString() + "Part" + lodpart[1].ToString();
            }
            try
            {
                MS3D ms3d = new MS3D(clonePackMeshes.ToArray(), groupNames, rig, uvSet);
                string tmp = WriteMS3DFile("Save MS3D file", ms3d, "MS3D_" +
                    myCASP.PartName + ((uvSet > 0) ? "_uv" + uvSet.ToString() : ""));
            }
            catch (MS3D.MeshException em)
            {
                MessageBox.Show(em.Message);
                return;
            }
        }

        private void PackageMesh_ExportOBJ_button_Click(object sender, EventArgs e)
        {
            int uvSet;
            if (MeshExportUV0_radioButton.Checked)
            {
                uvSet = 0;
            }
            else
            {
                foreach (GEOM g in clonePackMeshes)
                {
                    if (!g.hasUVset(1))
                    {
                        MessageBox.Show("At least one mesh does not have a second UV set! Please select first UV set for export or export separate mesh parts.");
                        return;
                    }
                }
                uvSet = 1;
            }
            string[] groupNames = new string[clonePackMeshes.Count];
            for (int i = 0; i < clonePackMeshes.Count; i++)
            {
                TGI meshTGI = new TGI(tgiMeshes[i]);
                int[] lodpart = myCASP.getLODandPart(meshTGI);
                groupNames[i] = "LOD" + lodpart[0].ToString() + "Part" + lodpart[1].ToString();
            }
            OBJ obj = new OBJ(clonePackMeshes.ToArray(), groupNames, uvSet);
            string tmp = WriteOBJFile("Save converted mesh", obj, myCASP.PartName);
        }


        private void PackageMesh_ExportDAE_button_Click(object sender, EventArgs e)
        {
            RIG rig = SelectRig(myCASP.Species, myCASP.Age);
            if (rig == null)
            {
                MessageBox.Show("No rig is available for " + myCASP.Species.ToString() + ", " + myCASP.Age.ToString());
                return;
            }
            string[] groupNames = new string[clonePackMeshes.Count];
            for (int i = 0; i < clonePackMeshes.Count; i++)
            {
                TGI meshTGI = new TGI(tgiMeshes[i]);
                int[] lodpart = myCASP.getLODandPart(meshTGI);
                groupNames[i] = "LOD" + lodpart[0].ToString() + "Part" + lodpart[1].ToString();
            }
            try
            {
                DAE dae = new DAE(clonePackMeshes.ToArray(), groupNames, rig, MeshExportYup_radioButton.Checked);
                string tmp = WriteDAEFile("Save DAE file", dae, false, "DAE_" + myCASP.PartName);
            }
            catch (ApplicationException em)
            {
                MessageBox.Show(em.Message);
                return;
            }
        }

        private void PackageMeshAdd_button_Click(object sender, EventArgs e)
        {
            MeshLodRegionAssignmentForm f = new MeshLodRegionAssignmentForm();
            DialogResult res = f.ShowDialog();
            if (res == DialogResult.Cancel) return;

            GEOM geom = new GEOM();
            if (!GetGEOMData(f.importFile, out geom))
            {
                MessageBox.Show("Can't read GEOM file!");
                return;
            }
            if (!geom.isTS4)
            {
                MessageBox.Show("The mesh file is not a valid TS4 mesh format!");
                return;
            }
            clonePackMeshes.Add(geom);
            newMeshImport.Add(true);
            clonedMesh.Add(true);
            changesMesh = true;

            TGI newtgi = new TGI((uint)XmodsEnums.ResourceTypes.GEOM,
                (FNVhash.FNV24(meshName) + (uint)Environment.TickCount) | 0x80000000,
                FNVhash.FNV64(meshName) | 0x8000000000000000);
            tgiMeshes.Add(newtgi);

            meshRegionMap.SetMeshRegions(newtgi, f.RegionsSet, f.LayerSet);
            changesRegionMap = true;

            for (int i = 0; i < clonePackCASPs.Count; i++)
            {
                clonePackCASPs[i].Casp.addLink(newtgi);
                clonePackCASPs[i].Casp.AddMeshLink(newtgi, f.LodSet);
                clonePackCASPs[i].Casp.RebuildLinkList();
            }

            meshRegionMap.SortLODs(myCASP);

            DialogResult res2 = MessageBox.Show("Commit changes now?", "New mesh added", MessageBoxButtons.OKCancel);
            if (res2 == DialogResult.OK)
            {
                MeshEditCommit();
            }
            else
            {
                ListClonePackMeshes();
            }
        }

        private void MeshEditCommit_button_Click(object sender, EventArgs e)
        {
            MeshEditCommit();
        }

        private void MeshEditCommit()
        {
            if (clonePackMeshes == null) return;
            MeshWriteToPackage();
            if (changesRegionMap) RegionMapWriteToPackage();

            for (int i = 0; i < clonePackCASPs.Count; i++)
            {
                DeleteResource(clonePack, iresCASPs[i]);
                Stream s = new MemoryStream();
                BinaryWriter bw = new BinaryWriter(s);
                clonePackCASPs[i].Casp.Write(bw);
                clonePack.AddResource(iresCASPs[i], s, true);
            }
            GetClonePackMeshes();
            ListClonePackMeshes();
            ListClonePackRegions();
            changesMesh = false;
            StartPreview(myCASP.PartName);
        }

        private void MeshWriteToPackage()
        {
            for (int i = 0; i < clonePackMeshes.Count; i++)
            {
                if (newMeshImport[i])
                {
                    IResourceKey ik = null;
                    if (clonedMesh[i])
                    {
                        DeleteResource(clonePack, tgiMeshes[i]);
                        ik = new TGIBlock(0, null, tgiMeshes[i].Type, tgiMeshes[i].Group, tgiMeshes[i].Instance);
                    }
                    else
                    {
                        TGI oldtgi = new TGI(tgiMeshes[i]);
                        int[] lodpart = myCASP.getLODandPart(tgiMeshes[i]);
                        TGI newtgi = new TGI(tgiMeshes[i].Type,
                            FNVhash.FNV24(meshName + "_lod" + lodpart[0].ToString() + (lodpart[1] > 0 ? "_" + lodpart[1].ToString() : "")) | 0x80000000,
                            FNVhash.FNV64(meshName) | 0x8000000000000000);
                        for (int j = 0; j < clonePackCASPs.Count; j++)
                        {
                            clonePackCASPs[j].Casp.replaceLink(oldtgi, newtgi);
                        }
                        meshRegionMap.replaceLink(oldtgi, newtgi);
                        ik = new TGIBlock(0, null, newtgi.Type, newtgi.Group, newtgi.Instance);
                        clonedMesh[i] = true;
                        changesRegionMap = true;
                    }
                    if (iresBumpMap != null)
                        clonePackMeshes[i].SetNormalMap(new TGI(iresBumpMap.ResourceType, iresBumpMap.ResourceGroup, iresBumpMap.Instance), myCASP.Species == XmodsEnums.Species.Human);
                    MemoryStream mg = new MemoryStream();
                    BinaryWriter bwg = new BinaryWriter(mg);
                    clonePackMeshes[i].WriteFile(bwg);
                    IResourceIndexEntry ires = clonePack.AddResource(ik, mg, true);
                    ires.Compressed = (ushort)0x5A42;
                    tgiMeshes[i] = new TGI(ires.ResourceType, ires.ResourceGroup, ires.Instance);
                    newMeshImport[i] = false;
                }
            }
            changesMesh = false;
        }

        private void RegionMapWriteToPackage()
        {
            IResourceKey ik = null;
            if (clonedRMap)
            {
                DeleteResource(clonePack, iresRMap);
                ik = new TGIBlock(0, null, iresRMap.ResourceType, iresRMap.ResourceGroup, iresRMap.Instance);
            }
            else
            {
                TGI oldtgi = new TGI(iresRMap.ResourceType, iresRMap.ResourceGroup, iresRMap.Instance);
                TGI newtgi = new TGI(iresRMap.ResourceType,
                    iresRMap.ResourceGroup | 0x80000000, FNVhash.FNV64(meshName) | 0x8000000000000000);
                for (int j = 0; j < clonePackCASPs.Count; j++)
                {
                    clonePackCASPs[j].Casp.replaceLink(oldtgi, newtgi);
                }
                ik = new TGIBlock(0, null, newtgi.Type, newtgi.Group, newtgi.Instance);
                clonedRMap = true;
                meshRegionMap.setInternalLink(newtgi);
            }
            MemoryStream mg = new MemoryStream();
            BinaryWriter bwg = new BinaryWriter(mg);
            meshRegionMap.Write(bwg);
            iresRMap = clonePack.AddResource(ik, mg, false);
            iresRMap.Compressed = (ushort)0x5A42;
            changesRegionMap = false;
        }

        private void MeshEditDiscard_button_Click(object sender, EventArgs e)
        {
            DialogResult res = MessageBox.Show("Are you sure you want to discard changes?", "Discard Changes", MessageBoxButtons.OKCancel);
            if (res == DialogResult.Cancel) return;
            GetClonePackMeshes();
            ListClonePackMeshes();
            ListClonePackRegions();
            StartPreview(myCASP.PartName);
            changesMesh = false;
        }

        private void PackageMesh_dataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewComboBoxCell shader = (DataGridViewComboBoxCell)PackageMesh_dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex];
            int[] meshIndexes = (int[])PackageMesh_dataGridView.Rows[e.RowIndex].Tag;
            if (String.Compare(shader.Value.ToString(), "Mixed") == 0)
            {
                MessageBox.Show("You haven't selected a valid shader!");
                shader.Value = shader.Tag;
                return;
            }
            string selectedShader = (string)shader.Value;
            int ind = selectedShader.IndexOf("_");
            if (ind > 0 ) selectedShader = selectedShader.Remove(ind);
            uint shaderHash = Xmods.DataLib.FNVhash.FNV32(selectedShader);
            for (int i = 0; i < meshIndexes.Length; i++)
            {
                if (MeshShaderData_checkBox.Checked)
                {
                    GEOM.MTNF oldShader = clonePackMeshes[meshIndexes[i]].Shader;
                    GEOM.MTNF newShader = SimShader;
                    if (newShader.normalIndex >= 0 & oldShader.normalIndex >= 0)
                    {
                        newShader.normalIndex = oldShader.normalIndex;
                    }
                    clonePackMeshes[meshIndexes[i]].setShader(shaderHash, newShader);
                    if (newShader.normalIndex < 0)
                    {
                        clonePackMeshes[meshIndexes[i]].AddNormalMap(new TGI(iresBumpMap.ResourceType, iresBumpMap.ResourceGroup, iresBumpMap.Instance));
                    }
                }
                else
                {
                    clonePackMeshes[meshIndexes[i]].setShader(shaderHash);
                }
                newMeshImport[meshIndexes[i]] = true;
            }
            shader.Tag = shader.Value;
            changesMesh = true;
        }

        private void MeshExportMS3D_radioButton_CheckedChanged(object sender, EventArgs e)
        {
            MeshExportFormatChanged();
        }
        private void MeshExportOBJ_radioButton_CheckedChanged(object sender, EventArgs e)
        {
            MeshExportFormatChanged();
        }
        private void MeshExportTS4_radioButton_CheckedChanged(object sender, EventArgs e)
        {
            MeshExportFormatChanged();
        }
        private void MeshExportDAE_radioButton_CheckedChanged(object sender, EventArgs e)
        {
            MeshExportFormatChanged();
        }

        private void MeshExportFormatChanged()
        {
            if (MeshExportMS3D_radioButton.Checked)
            {
                ExportUV_panel.Enabled = true;
                ExportAxis_panel.Enabled = false;
            }
            else if (MeshExportDAE_radioButton.Checked)
            {
                ExportUV_panel.Enabled = false;
                ExportAxis_panel.Enabled = true;
            }
            else if (MeshExportTS4_radioButton.Checked)
            {
                ExportUV_panel.Enabled = false;
                ExportAxis_panel.Enabled = false;
            }
            else
            {
                ExportUV_panel.Enabled = true;
                ExportAxis_panel.Enabled = false;
            }
        }

        private void MeshVertexAlphaGo_button_Click(object sender, EventArgs e)
        {
            UpdateMeshesVertexColor(myCASP.BodyType);
            changesMesh = true;
        }

        private void MeshVertexColorGo_button_Click(object sender, EventArgs e)
        {
            if (string.Compare(MeshVertexColorValue.Text, " ") > 0)
            {
                uint vertColor;
                if (uint.TryParse(MeshVertexColorValue.Text, System.Globalization.NumberStyles.HexNumber,
                    System.Globalization.CultureInfo.CurrentCulture, out vertColor))
                {
                    UpdateMeshesVertexColor(vertColor);
                    changesMesh = true;
                }
                else
                {
                    MessageBox.Show("You have not entered a correctly formatted vertex color!");
                    return;
                }
            }
            else
            {
                MessageBox.Show("You have not entered a vertex color!");
            }
        }

        private void MeshVertexColorStandard_comboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            XmodsEnums.VertexColorStandard v;
            Enum.TryParse((string)MeshVertexColorStandard_comboBox.SelectedItem, out v);
            MeshVertexColorValue.Text = ((uint)v).ToString("X8");
        }

        private void UpdateMeshesVertexColor(XmodsEnums.BodyType bodyType)
        {
            uint newalpha = (uint)bodyType << 24;
            UpdateMeshesVertexColor(true, newalpha);
        }

        private void UpdateMeshesVertexColor(uint newColorValue)
        {
            UpdateMeshesVertexColor(false, newColorValue);
        }
        
        private void UpdateMeshesVertexColor(bool alphaOnly, uint colorValue)
        {
            for (int i = 0; i < clonePackMeshes.Count; i++)
            {
                bool localAlphaOnly = alphaOnly;
                if (!clonePackMeshes[i].hasTags)
                {
                    clonePackMeshes[i].AddTagValtoFormat();
                    localAlphaOnly = false;
                }

                for (int j = 0; j < clonePackMeshes[i].numberVertices; j++)
                {
                    if (localAlphaOnly)
                    {
                        uint tmp = clonePackMeshes[i].getTagval(j) & 0x00FFFFFFU;
                        clonePackMeshes[i].setTagval(j, tmp + colorValue);
                    }
                    else
                    {
                        clonePackMeshes[i].setTagval(j, colorValue);
                    }
                }
                newMeshImport[i] = true;
            }
            changesMesh = true;
        }

        private void MeshRobeColorFix_button_Click(object sender, EventArgs e)
        {
            bool first = true;
            for (int i = 0; i < clonePackMeshes.Count; i++)
            {
                if (!clonePackMeshes[i].hasTags)
                {
                    if (first)
                    {
                        MessageBox.Show("At least one mesh has no vertex colors. Please use the" + Environment.NewLine +
                            "Mesh Properties and Fixers / Correct Vertex Color" + Environment.NewLine + "tool to add them.");
                        first = false;
                    }
                }
                else
                {
                    MeshRobeColorFix(clonePackMeshes[i]);
                    newMeshImport[i] = true;
                }
            }
            changesMesh = true;
        }
        private void MeshRobeColorFix(GEOM geom)
        {
            for (int j = 0; j < geom.numberVertices; j++)
            {
                uint tmp = geom.getTagval(j) & 0xFFFFFF00U;
                uint robeVal = (byte)(geom.getTagval(j) & 0x000000FFU);
                robeVal = robeVal >> 2;
                // if (robeVal > 0) MessageBox.Show(tmp.ToString("X8") + ", " + robeVal.ToString());
                geom.setTagval(j, tmp + robeVal);
            }
        }

        private void MeshHairBumpFix_button_Click(object sender, EventArgs e)
        {
            meshBumpMap = new DSTResource(1, new MemoryStream(Properties.Resources.blankBump));
            importedBumpMap = true;
            ddsBumpmap = new DdsFile();
            ddsBumpmap.Load(meshBumpMap.ToDDS(), false);
            bumpmapImage = ddsBumpmap.Image;
            changesMesh = true;
        }

        private void MeshUVStitchFix_button_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < clonePackMeshes.Count; i++)
            {
                clonePackMeshes[i].AutoUV1Stitches();
                newMeshImport[i] = true;
            }
            changesMesh = true;
        }

        private void MeshSeamStitch_button_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < clonePackMeshes.Count; i++)
            {
                int lod = myCASP.getLOD(tgiMeshes[i]);
                //if (myCASP.Species == XmodsEnums.Species.Human && myCASP.Age > XmodsEnums.Age.Child)
                //{
                    clonePackMeshes[i].AutoSeamStitches(myCASP.Species, myCASP.Age, myCASP.Gender, lod);
                    GEOM refMesh = GetBodyMesh(myCASP.Species, myCASP.Age, myCASP.Gender, lod, XmodsEnums.BodyType.Body, XmodsEnums.BodySubType.None, false);
                    clonePackMeshes[i].AutoSeamBones(refMesh, myCASP.Species, myCASP.Age, myCASP.Gender, lod);
                    clonePackMeshes[i].AutoSeamNormals(refMesh, myCASP.Species, myCASP.Age, myCASP.Gender, lod);
                    newMeshImport[i] = true;
                //}
            }
            changesMesh = true;
        }

        private void MeshGap_button_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < clonePackMeshes.Count; i++)
            {
                for (int j = i + 1; j < clonePackMeshes.Count; j++)
                {
                    if (myCASP.getLOD(tgiMeshes[i]) == myCASP.getLOD(tgiMeshes[j]))
                    {
                        GEOM.MatchSeamVerts(clonePackMeshes[i], clonePackMeshes[j]);
                        newMeshImport[i] = true;
                        newMeshImport[j] = true;
                    }
                }
            }
            changesMesh = true;
        }

        private void MeshColorFix_button_Click(object sender, EventArgs e)
        {
            SetWaitMessageOnButton(sender);
            uint newalpha = (uint)myCASP.BodyType << 24;
            for (int i = 0; i < clonePackMeshes.Count; i++)
            {
                GEOM refMesh = GetBodyMesh(myCASP.Species, myCASP.Age, myCASP.Gender, myCASP.getLOD(tgiMeshes[i]), myCASP.BodyType, myCASP.BodySubType, false);
                GEOM refSkirt = MeshAutoSkirt_checkBox.Checked ? GetBodyMesh(myCASP.Species, myCASP.Age, myCASP.Gender, myCASP.getLOD(tgiMeshes[i]), XmodsEnums.BodyType.Body, XmodsEnums.BodySubType.None, true) : null;

                Vector3[] refVerts = new Vector3[refMesh.numberVertices];
                Vector3[] refVertsSkirt = (refSkirt != null) ? new Vector3[refSkirt.numberVertices] : null;
                for (int j = 0; j < refMesh.numberVertices; j++)
                {
                    refVerts[j] = new Vector3(refMesh.getPosition(j));
                }
                if (refSkirt != null)
                {
                    for (int j = 0; j < refSkirt.numberVertices; j++)
                    {
                        refVertsSkirt[j] = new Vector3(refSkirt.getPosition(j));
                    }
                }

                    //use correct alpha for CASP type, use standard nude mesh and skirt mesh for the rest
                for (int j = 0; j < clonePackMeshes[i].numberVertices; j++)
                {
                    int nearestRefVert = (new Vector3(clonePackMeshes[i].getPosition(j))).NearestPointIndexSimple(refVerts);
                    uint tagVal = refMesh.getTagval(nearestRefVert);
                    if (refSkirt != null && (!(new Vector3(clonePackMeshes[i].getPosition(j)).CloseTo(refVerts[nearestRefVert], 0.001f))))
                    {
                        nearestRefVert = (new Vector3(clonePackMeshes[i].getPosition(j))).NearestPointIndexSimple(refVertsSkirt);
                        tagVal = refSkirt.getTagval(nearestRefVert);
                    }
                    if (HardColorAlpha_radioButton.Checked)
                    {
                        clonePackMeshes[i].setTagval(j, (tagVal & 0x00FFFFFF) + (newalpha));
                    }
                }
                newMeshImport[i] = true;
            }
            changesMesh = true;
            cloneWait_label.Visible = false;
            cloneWait_label.Refresh();
        }

        private void MeshSlotrayDefault_button_Click(object sender, EventArgs e)
        {
            SetWaitMessageOnButton(sender);
            XmodsEnums.BodyType bodyType;
            if (MeshSlotrayRef_comboBox.SelectedIndex == 0) bodyType = XmodsEnums.BodyType.Top;
            else if (MeshSlotrayRef_comboBox.SelectedIndex == 1) bodyType = XmodsEnums.BodyType.Bottom;
            else if (MeshSlotrayRef_comboBox.SelectedIndex == 2) bodyType = XmodsEnums.BodyType.Body;
            else bodyType = XmodsEnums.BodyType.All;

            for (int i = 0; i < clonePackMeshes.Count; i++)
            {
                GEOM refMesh = GetBodyMesh(myCASP.Species, myCASP.Age, myCASP.Gender, myCASP.getLOD(tgiMeshes[i]), bodyType, myCASP.BodySubType, false);
                clonePackMeshes[i].AutoSlotray(refMesh);
                newMeshImport[i] = true;
            }
            cloneWait_label.Visible = false;
            cloneWait_label.Refresh();
        }

        private void MeshSlotrayAdjust_button_Click(object sender, EventArgs e)
        {
            SetWaitMessageOnButton(sender);
            for (int i = 0; i < clonePackMeshes.Count; i++)
            {
                clonePackMeshes[i].AdjustSlotrays((float)MeshArmsOffsetX_numericUpDown.Value, (float)MeshArmsOffsetY_numericUpDown.Value, 
                    (float)MeshArmsOffsetZ_numericUpDown.Value);
                newMeshImport[i] = true;
            }
            cloneWait_label.Visible = false;
            cloneWait_label.Refresh();
        }
        
        private void MeshSlotrayFix_button_Click(object sender, EventArgs e)
        {
            SetWaitMessageOnButton(sender);
            for (int i = 0; i < clonePackMeshes.Count; i++)
            {
                clonePackMeshes[i].AutoSlotray(SelectSlotRayData(myCASP.Species, myCASP.Age), SelectRig(myCASP.Species, myCASP.Age));
                newMeshImport[i] = true;
            }
            changesMesh = true;
            cloneWait_label.Visible = false;
            cloneWait_label.Refresh();
        }

        private void MeshAutoAll_button_Click(object sender, EventArgs e)
        {
            MeshBoneWeightFix_button_Click(sender, e);
            MeshUV1Fix_button_Click(sender, e);
            MeshColorFix_button_Click(sender, e);
            MeshSlotrayFix_button_Click(sender, e);
        }

        private void MeshTangentsFix_button_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < clonePackMeshes.Count; i++)
            {
                clonePackMeshes[i].CalculateTangents();
                newMeshImport[i] = true;
            }
            changesMesh = true;
        }

        private void MeshBonesFix_button_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < clonePackMeshes.Count; i++)
            {
                clonePackMeshes[i].FixUnusedBones();
                newMeshImport[i] = true;
            }
            changesMesh = true;
        }

        private void MeshBoneWeightFix_button_Click(object sender, EventArgs e)
        {
            SetWaitMessageOnButton(sender);
            for (int i = 0; i < clonePackMeshes.Count; i++)
            {
                GEOM refMesh = GetBodyMesh(myCASP.Species, myCASP.Age, myCASP.Gender, myCASP.getLOD(tgiMeshes[i]), myCASP.BodyType, myCASP.BodySubType, false);
                GEOM refSkirt = MeshAutoSkirt_checkBox.Checked ? GetBodyMesh(myCASP.Species, myCASP.Age, myCASP.Gender, myCASP.getLOD(tgiMeshes[i]), XmodsEnums.BodyType.Body, XmodsEnums.BodySubType.None, true) : null;
                clonePackMeshes[i].AutoBone(refMesh, refSkirt, false, true, 3, 2f, null);
                clonePackMeshes[i].FixUnusedBones();
                newMeshImport[i] = true;
            }
            changesMesh = true;
            cloneWait_label.Visible = false;
            cloneWait_label.Refresh();
        }

        private void MeshUV1Fix_button_Click(object sender, EventArgs e)
        {
            SetWaitMessageOnButton(sender);
            for (int i = 0; i < clonePackMeshes.Count; i++)
            {
                GEOM refMesh = GetBodyMesh(myCASP.Species, myCASP.Age, myCASP.Gender, myCASP.getLOD(tgiMeshes[i]), myCASP.BodyType, myCASP.BodySubType, false);
                clonePackMeshes[i].MatchUVsets(refMesh.vertexFormat);
                for (int uv = 1; uv < clonePackMeshes[i].numberUVsets; uv++)
                {
                    clonePackMeshes[i].AutoUV(refMesh, uv);
                }
                clonePackMeshes[i].AutoUV1Stitches();
                newMeshImport[i] = true;
            }
            changesMesh = true;
            cloneWait_label.Visible = false;
            cloneWait_label.Refresh();
        }

        private void SetWaitMessageOnButton(object obj)
        {
            if (!(obj is Button)) return;
            Button button = obj as Button;
            cloneWait_label.Visible = true;
            Point waitPoint = button.FindForm().PointToClient(button.Parent.PointToScreen(button.Location));
            waitPoint.X += 25;
            waitPoint.Y += 25;
            cloneWait_label.Location = waitPoint;
            cloneWait_label.BringToFront();
            cloneWait_label.Refresh();
        }

        private void ResetGlassShader_button_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < clonePackMeshes.Count; i++)
            {
                if (clonePackMeshes[i].ShaderType == XmodsEnums.Shader.SimGlass)
                {
                    GEOM.MTNF shader = new GEOM.MTNF(new uint[] { 1179538509, 0, 152, 16, 1824587141, 4, 4, 272, 3123518137, 1, 2, 288, 97660883, 1, 1, 296, 
                    3287985231, 4, 4, 300, 3668529965, 1, 1, 316, 1011213108, 1, 1, 320, 1942590014, 1, 3, 324, 2907867744, 65540, 4, 336, 700243231, 
                    1, 1, 352, 1003766176, 1, 3, 356, 4149606399, 1, 1, 368, 77978275, 1, 3, 372, 2189874156, 1, 1, 384, 752949314, 1, 3, 388, 4280935609, 
                    1, 2, 400, 1669179909, 1, 4, 408, 0, 0, 0, 0, 1065353216, 1065353216, 1065353216, 0, 0, 0, 0, 1065353216, 1065353216, 
                    0, 0, 0, 1, 0, 0, 0, 1056964608, 0, 0, 0, 1101004800, 0, 0, 0, 0, 1065353216, 1065353216, 1065353216, 1065353216, 1065353216, 
                    1065353216, 1065353216, 1065353216, 1065353216 });

                    shader.normalIndex = clonePackMeshes[i].Shader.normalIndex;
                    clonePackMeshes[i].setShader((uint)XmodsEnums.Shader.SimGlass, shader);
                }
                newMeshImport[i] = true;
            }
            changesMesh = true;
        }
        
        /*     private void UpdateMeshes()
        {
            bool importedMesh = false;
            for (int i = 0; i < clonePackMeshes.Count; i++)
            {
                IResourceKey ik = null;
                if (clonedMesh[i])
                {
                    DeleteResource(clonePack, iresMeshes[i]);
                    ik = new TGIBlock(0, null, iresMeshes[i].ResourceType, iresMeshes[i].ResourceGroup, iresMeshes[i].Instance);
                }
                else
                {
                    TGI oldtgi = new TGI(iresMeshes[i].ResourceType, iresMeshes[i].ResourceGroup, iresMeshes[i].Instance);
                    int[] lodpart = myCASP.getLODandPart(new TGI(iresMeshes[i].ResourceType, iresMeshes[i].ResourceGroup, iresMeshes[i].Instance));
                    TGI newtgi = new TGI(iresMeshes[i].ResourceType,
                        FNVhash.FNV24(meshName + "_lod" + lodpart[0].ToString() + (lodpart[1] > 0 ? "_" + lodpart[1].ToString() : "")) | 0x80000000,
                        FNVhash.FNV64(meshName) | 0x8000000000000000);
                    for (int j = 0; j < clonePackCASPs.Count; j++)
                    {
                        clonePackCASPs[j].Casp.replaceLink(oldtgi, newtgi);
                    }
                    meshRegionMap.replaceLink(oldtgi, newtgi);
                    ik = new TGIBlock(0, null, newtgi.Type, newtgi.Group, newtgi.Instance);
                    clonedMesh[i] = true;
                    importedMesh = true;
                }
                MemoryStream mg = new MemoryStream();
                BinaryWriter bwg = new BinaryWriter(mg);
                clonePackMeshes[i].WriteFile(bwg);
                iresMeshes[i] = clonePack.AddResource(ik, mg, false);
                iresMeshes[i].Compressed = (ushort)0x5A42;
                newMeshImport[i] = false;
            }
            if (importedMesh)
            {
                MemoryStream mg = new MemoryStream();
                BinaryWriter bwg = new BinaryWriter(mg);
                meshRegionMap.Write(bwg);
                IResourceKey ik = null;
                if (clonedRMap)
                {
                    DeleteResource(clonePack, iresRMap);
                    ik = new TGIBlock(0, null, iresRMap.ResourceType, iresRMap.ResourceGroup, iresRMap.Instance);
                }
                else
                {
                    TGI oldtgi = new TGI(iresRMap.ResourceType, iresRMap.ResourceGroup, iresRMap.Instance);
                    TGI newtgi = new TGI(iresRMap.ResourceType,
                        iresRMap.ResourceGroup | 0x80000000, FNVhash.FNV64(meshName) | 0x8000000000000000);
                    for (int j = 0; j < clonePackCASPs.Count; j++)
                    {
                        clonePackCASPs[j].Casp.replaceLink(oldtgi, newtgi);
                    }
                    ik = new TGIBlock(0, null, newtgi.Type, newtgi.Group, newtgi.Instance);
                    clonedRMap = true;
                }
                iresRMap = clonePack.AddResource(ik, mg, false);
                iresRMap.Compressed = (ushort)0x5A42;
            }
        } */


        private void ListClonePackRegions()
        {
            cloneRegion_dataGridView.Rows.Clear();
            cloneRegionBlock_dataGridView.Rows.Clear();
            if (meshRegionMap == null) return;
            string[] tmp = new string[2];
            for (int i = 0; i < meshRegionMap.NumberRegions; i++)
            {
                tmp[0] = Enum.IsDefined(typeof(XmodsEnums.CASPartRegionTS4), meshRegionMap.GetRegion(i)) ?
                    Enum.GetName(typeof(XmodsEnums.CASPartRegionTS4), meshRegionMap.GetRegion(i)) : meshRegionMap.GetRegion(i).ToString();
                tmp[1] = meshRegionMap.GetLayer(i).ToString();
                cloneRegion_dataGridView.Rows.Add(tmp);
            }
            cloneRegion_comboBox.SelectedIndexChanged -= cloneRegion_comboBox_SelectedIndexChanged;
            cloneRegionLayer.TextChanged -= cloneRegionLayer_TextChanged;
            cloneRegionReplacement_checkBox.CheckedChanged -= cloneRegionReplacement_checkBox_CheckedChanged;
            cloneRegion_comboBox.SelectedItem = "Base";
            cloneRegionLayer.Text = "";
            cloneRegionReplacement_checkBox.Checked = false;
            cloneRegionBlock_dataGridView.Rows.Clear();
            DisplayRegionInfo(0);
        }

        private void cloneRegion_dataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (currentRowRegion != e.RowIndex && changesRegion)
            {
                DialogResult res = MessageBox.Show("You have unsaved region changes which may be lost if you switch to another region!", "Unsaved changes", MessageBoxButtons.OKCancel);
                if (res == DialogResult.Cancel)
                {
                    cloneRegion_dataGridView.Rows[e.RowIndex].Selected = false;
                    cloneRegion_dataGridView.Rows[Math.Max(currentRowRegion, 0)].Selected = true;
                    return;
                }
                ListClonePackRegions();
                changesRegion = false;
            }

            if (e.ColumnIndex == cloneRegion_dataGridView.Columns["regionRemove"].Index)
            {
                if (meshRegionMap.GetRegion(e.RowIndex) == 0)
                {
                    MessageBox.Show("You can't remove the Base region!");
                    return;
                }
                DialogResult res = MessageBox.Show("Are you sure? Removing a region cannot be undone!", "Remove Region", MessageBoxButtons.OKCancel);
                if (res == DialogResult.Cancel) return;
                TGI[] meshTGIlist = meshRegionMap.GetTGIlist(e.RowIndex);
                foreach (TGI tgi in meshTGIlist)
                {
                    bool multiRegion = false;
                    for (int i = 0; i < meshRegionMap.NumberRegions; i++)
                    {
                        if (i != e.RowIndex)
                        {
                            TGI[] tmpTGIlist = meshRegionMap.GetTGIlist(i);
                            foreach (TGI tmpTGI in tmpTGIlist)
                            {
                                if (tgi.Equals(tmpTGI))
                                {
                                    multiRegion = true;
                                    break;
                                }
                            }
                            if (multiRegion) break;
                        }
                    }
                    if (multiRegion) continue;
                    for (int i = 0; i < clonePackCASPs.Count; i++)
                    {
                        clonePackCASPs[i].Casp.RemoveMeshLink(tgi);
                    }
                    DeleteResource(clonePack, tgi);
                }
                for (int i = 0; i < clonePackCASPs.Count; i++)
                {
                    clonePackCASPs[i].Casp.RebuildLinkList();
                }
                meshRegionMap.RemoveRegion(e.RowIndex);
                currentRowRegion = 0;
                changesRegion = true;
                changedCASPs = true;
                RegionMapIO();
                GetClonePackMeshes();
                ListClonePackMeshes();
                ListClonePackRegions();
                StartPreview(myCASP.PartName);
                cloneRegionCommit();
            }

            else
            {
                DisplayRegionInfo(e.RowIndex);
            }
        }

        private void DisplayRegionInfo(int row)
        {
            cloneRegion_comboBox.SelectedIndexChanged -= cloneRegion_comboBox_SelectedIndexChanged;
            cloneRegionLayer.TextChanged -= cloneRegionLayer_TextChanged;
            cloneRegionReplacement_checkBox.CheckedChanged -= cloneRegionReplacement_checkBox_CheckedChanged;
            cloneRegion_comboBox.SelectedItem = Enum.GetName(typeof(XmodsEnums.CASPartRegionTS4), meshRegionMap.GetRegion(row));
            cloneRegionLayer.Text = meshRegionMap.GetLayer(row).ToString();
            cloneRegionReplacement_checkBox.Checked = meshRegionMap.GetIsReplacement(row) != 0;
            cloneRegion_comboBox.SelectedIndexChanged += cloneRegion_comboBox_SelectedIndexChanged;
            cloneRegionLayer.TextChanged += cloneRegionLayer_TextChanged;
            cloneRegionReplacement_checkBox.CheckedChanged += cloneRegionReplacement_checkBox_CheckedChanged;
            TGI[] meshList = meshRegionMap.GetTGIlist(row);
            string[] tmp = new string[3];
            cloneRegionBlock_dataGridView.Rows.Clear();
            foreach (TGI tgi in meshList)
            {
                tmp[0] = myCASP.getLOD(tgi).ToString();
                tmp[1] = tgi.ToString();
                for (int i = 0; i < tgiMeshes.Count; i++)
                {
                    if (tgi.Equals(tgiMeshes[i]))
                    {
                        tmp[2] = clonePackMeshes[i].numberFaces.ToString();
                        break;
                    }
                }
                cloneRegionBlock_dataGridView.Rows.Add(tmp);
            }
            currentRowRegion = row;
        }

        private void cloneRegion_comboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            changesRegion = true;
        }

        private void cloneRegionLayer_TextChanged(object sender, EventArgs e)
        {
            changesRegion = true;
        }

        private void cloneRegionReplacement_checkBox_CheckedChanged(object sender, EventArgs e)
        {
            changesRegion = true;
        }

        private void cloneRegionCommit_button_Click(object sender, EventArgs e)
        {
            cloneRegionCommit();
        }

        private void cloneRegionCommit()
        {
            float newLayer;
            try
            {
                newLayer = float.Parse(cloneRegionLayer.Text);
            }
            catch
            {
                MessageBox.Show("You have entered an invalid number for the layer!");
                return;
            }
            meshRegionMap.SetLayer(currentRowRegion, newLayer);
            uint newRegion = ((uint[])Enum.GetValues(typeof(XmodsEnums.CASPartRegionTS4)))[cloneRegion_comboBox.SelectedIndex];
            meshRegionMap.SetRegion(currentRowRegion, newRegion);
            meshRegionMap.SetIsReplacement(currentRowRegion, cloneRegionReplacement_checkBox.Checked ? (byte)1 : (byte)0);
            RegionMapIO();
            backupRegionMap = new RegionMap(meshRegionMap);
            SetupRegionList();
            changesRegion = false;
            GetClonePackMeshes();
            ListClonePackMeshes();
            ListClonePackRegions();
            StartPreview(myCASP.PartName);
        }

        private void RegionMapIO()
        {
            IResourceKey ik = null;
            if (clonedRMap)
            {
                DeleteResource(clonePack, iresRMap);
                ik = new TGIBlock(0, null, iresRMap.ResourceType, iresRMap.ResourceGroup, iresRMap.Instance);
            }
            else
            {
                TGI oldtgi = new TGI(iresRMap.ResourceType, iresRMap.ResourceGroup, iresRMap.Instance);
                TGI newtgi = new TGI(iresRMap.ResourceType,
                    iresRMap.ResourceGroup | 0x80000000, FNVhash.FNV64(meshName) | 0x8000000000000000);
                for (int j = 0; j < clonePackCASPs.Count; j++)
                {
                    clonePackCASPs[j].Casp.replaceLink(oldtgi, newtgi);
                }
                ik = new TGIBlock(0, null, newtgi.Type, newtgi.Group, newtgi.Instance);
                clonedRMap = true;
                meshRegionMap.setInternalLink(newtgi);
                changedCASPs = true;
            }
            MemoryStream mg = new MemoryStream();
            BinaryWriter bwg = new BinaryWriter(mg);
            meshRegionMap.Write(bwg);
            iresRMap = clonePack.AddResource(ik, mg, false);
            iresRMap.Compressed = (ushort)0x5A42;

            if (changedCASPs)
            {
                for (int i = 0; i < clonePackCASPs.Count; i++)
                {
                    DeleteResource(clonePack, iresCASPs[i]);
                    Stream s = new MemoryStream();
                    BinaryWriter bw = new BinaryWriter(s);
                    clonePackCASPs[i].Casp.Write(bw);
                    s.Position = 0;
                    iresCASPs[i] = clonePack.AddResource(iresCASPs[i], s, true);
                    iresCASPs[i].Compressed = (ushort)0x5A42;
                }
                changedCASPs = false;
            }
        }

        private void cloneRegionDiscard_button_Click(object sender, EventArgs e)
        {
            DialogResult res = MessageBox.Show("Are you sure you want to discard changes?", "Discard Changes", MessageBoxButtons.OKCancel);
            if (res == DialogResult.Cancel) return;
            meshRegionMap = new RegionMap(backupRegionMap);
            ListClonePackRegions();
            changesRegion = false;
        }

        private void CloneMeshWipe()
        {
            MeshVertexColorValue.Text = "";
            meshBumpMap = null;
            meshGlowMap = null;
            clonePackMeshes = new List<GEOM>();
            PackageMesh_dataGridView.Rows.Clear();

            cloneRegion_dataGridView.Rows.Clear();
            cloneRegion_comboBox.SelectedIndex = -1;
            cloneRegionBlock_dataGridView.Rows.Clear();
            cloneRegionLayer.Text = "";
            cloneRegionReplacement_checkBox.Checked = false;
        }

        private void cloneRegionBlock_dataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == cloneRegionBlock_dataGridView.Columns["regionMeshChange"].Index)
            {
                TGI tgi = new TGI((string)cloneRegionBlock_dataGridView.Rows[e.RowIndex].Cells[1].Value);
                int[] lodpart = myCASP.getLODandPart(tgi);
                string tmpName = meshName + "_lod" + lodpart[0].ToString() + "_" + lodpart[1].ToString();
                MeshLodRegionAssignmentForm f = new MeshLodRegionAssignmentForm(tmpName, lodpart[0],
                    meshRegionMap.GetMeshLayer(tgi), meshRegionMap.GetMeshRegions(tgi));
                DialogResult res = f.ShowDialog();
                if (res == DialogResult.Cancel) return;

                meshRegionMap.SetMeshRegions(tgi, f.RegionsSet, f.LayerSet);
                meshRegionMap.SortLODs(myCASP);

                currentRowRegion = 0;
                changesRegion = true;
             //   RegionMapIO();
             //   GetClonePackMeshes();
             //   ListClonePackMeshes();
                ListClonePackRegions();
             //   StartPreview(myCASP.PartName);
             //   cloneRegionCommit();
            }
        }

        private void MeshRegionLayerHelp_button_Click(object sender, EventArgs e)
        {
            LayerHelp();
        }
    }
}
