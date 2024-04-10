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
        private void CloneGo_button_Click(object sender, EventArgs e)
        {
            List<DataGridViewRow> swatchList =
                SwatchList_dataGridView.Rows.Cast<DataGridViewRow>().Where(k => Convert.ToBoolean(k.Cells[CloneColor.Name].Value) == true).ToList();
            if (swatchList.Count == 0)
            {
                MessageBox.Show("You must select at least one color!");
                return;
            }
            TGI dummy = new TGI(0U, 0U, 0UL);
            Package newPack = (Package)Package.NewPackage(0);

            if (Default_radioButton.Checked)
            {
                foreach (DataGridViewRow row in swatchList)
                {
                    myPack = CASitems[itemIndex].CASPpackages[(int)row.Tag];
                    Predicate<IResourceIndexEntry> getCASP = r => r.ResourceType == (uint)XmodsEnums.ResourceTypes.CASP &
                                                                  r.Instance == CASitems[itemIndex].CASPids[(int)row.Tag];
                    IResourceIndexEntry rcasp = myPack.Find(getCASP);
                    Stream s = myPack.GetResource(rcasp);
                    s.Position = 0;
                    BinaryReader br = new BinaryReader(s);
                    CASP casp = null;
                    casp = new CASP(br);
                    casp.PartName = NewMeshName.Text + "_" + row.Cells["SwatchName"].Value;
                    MemoryStream m = new MemoryStream();
                    BinaryWriter bw = new BinaryWriter(m);
                    casp.Write(bw);
                    IResourceIndexEntry irieCasp = newPack.AddResource(rcasp, m, true);
                    irieCasp.Compressed = (ushort)0x5A42;
                }
            }
         
            else if (New_radioButton.Checked)
            {
                ulong meshInstance = (FNVhash.FNV64(NewMeshName.Text) + (uint)ran.Next()) | 0x8000000000000000;
                uint outfitID = FNVhash.FNV16(NewMeshName.Text) | 0x80000000;
                List<CASP> CASPlist = new List<CASP>();
                List<IResourceIndexEntry> irCASPlist = new List<IResourceIndexEntry>();

                foreach (DataGridViewRow row in swatchList)
                {
                    // process CASPs in the swatch list
                    myPack = CASitems[itemIndex].CASPpackages[(int)row.Tag];
                    Predicate<IResourceIndexEntry> getCASP = r => r.ResourceType == (uint)XmodsEnums.ResourceTypes.CASP &
                                                      r.Instance == CASitems[itemIndex].CASPids[(int)row.Tag];
                    IResourceIndexEntry rcasp = myPack.Find(getCASP);
                    Stream s = myPack.GetResource(rcasp);
                    s.Position = 0;
                    BinaryReader br = new BinaryReader(s);
                    CASP casp = new CASP(br);
                    CASPlist.Add(casp);
                    if (casp.BodyType == XmodsEnums.BodyType.Eyecolor || casp.BodyType == XmodsEnums.BodyType.SecondaryEyeColor)
                    {
                        for (int i = 0; i < casp.ColorList.Length; i++)
                        {
                            uint tmp = casp.ColorList[i] + 0x00050505U;
                            casp.ColorList = new uint[] { tmp };
                        }
                    }
                    else
                    {
                        uint packMask = casp.OutfitID & 0x7FFF0000;
                        casp.OutfitID = outfitID | packMask;
                    }
                    string orig_partname = casp.PartName;
                    casp.PartName = NewMeshName.Text + "_" + row.Cells["SwatchName"].Value;

                    TGIBlock rcaspNew;
                    rcaspNew = new TGIBlock(0, null, rcasp.ResourceType,
                                            rcasp.ResourceGroup | 0x80000000, (FNVhash.FNV32(casp.PartName)  + (uint)ran.Next()) | 0x8000000000000000);
                    MemoryStream m = new MemoryStream();
                    BinaryWriter bw = new BinaryWriter(m);
                    casp.Write(bw);
                    IResourceIndexEntry irieCasp = newPack.AddResource(rcaspNew, m, true);
                    irieCasp.Compressed = (ushort)0x5A42;
                    irCASPlist.Add(irieCasp);
                    s.Dispose();

                    if (SelectPack_radioButton.Checked && CloneThumbs_checkBox.Checked)
                    {
                        Predicate<IResourceIndexEntry> getRes = r => r.ResourceType == (uint)XmodsEnums.ResourceTypes.THUM &
                                                    r.Instance == rcasp.Instance;
                        List<IResourceIndexEntry> rRes = myPack.FindAll(getRes);
                        foreach (IResourceIndexEntry tRes in rRes)
                        {
                            Stream thumb = myPack.GetResource(tRes);
                            IResourceIndexEntry itmp = newPack.AddResource(new TGIBlock(0, null, tRes.ResourceType, tRes.ResourceGroup, rcaspNew.Instance), thumb, true);
                            itmp.Compressed = (ushort)0x5A42;
                        }
                    }
                }

                if (SelectPack_radioButton.Checked && CASPlist.Count > 0)
                {
                    int unknownCounter = 1;
                    Dictionary<TGI, TGI> linkLookup = new Dictionary<TGI, TGI>();

                    TGI rMapTGI = CASPlist[0].LinkList[CASPlist[0].RegionMapIndex];
                    Predicate<IResourceIndexEntry> getrmap = r => r.ResourceType == rMapTGI.Type & r.ResourceGroup == rMapTGI.Group &
                                                                    r.Instance == rMapTGI.Instance;
                    IResourceIndexEntry rMesh = myPack.Find(getrmap);
                    RegionMap rMap = null;
                    if (rMesh != null)
                    {
                        Stream sm = myPack.GetResource(rMesh);
                        sm.Position = 0;
                        BinaryReader brm = new BinaryReader(sm);
                        rMap = new RegionMap(brm);
                    }
                    for (int i = 0; i < CASPlist.Count; i++)
                    {
                        foreach (TGI tgi in CASPlist[i].LinkList)
                        {
                            TGI tmp;
                            if (linkLookup.TryGetValue(tgi, out tmp))
                            {
                                CASPlist[i].replaceLink(tgi, tmp);
                                continue;
                            }
              
                            string newMeshNameRandom = NewMeshName.Text + ran.Next().ToString();
                            string newPartNameRandom = CASPlist[i].PartName + ran.Next().ToString();

                            if (tgi.Equals(dummy)) continue;
                            Predicate<IResourceIndexEntry> getRes = r => r.ResourceType == tgi.Type & r.ResourceGroup == tgi.Group &
                                                        r.Instance == tgi.Instance;
                            IResourceIndexEntry rRes = myPack.Find(getRes);
                            if (rRes == null)
                            {
                                continue;
                            }
                            Stream sr = myPack.GetResource(rRes);
                            sr.Position = 0;
                            BinaryReader brr = new BinaryReader(sr);
                            MemoryStream mr = new MemoryStream();
                            TGIBlock rResNew = new TGIBlock(0, null, rRes.ResourceType, rRes.ResourceGroup, rRes.Instance);
                            if (tgi.Type == (uint)XmodsEnums.ResourceTypes.GEOM)
                            {
                                GEOM g = new GEOM(brr);
                                TGI oldtgi = new TGI(rRes.ResourceType, rRes.ResourceGroup, rRes.Instance);
                                int[] lodpart = CASPlist[i].getLODandPart(oldtgi);
                                TGI newtgi = new TGI(rRes.ResourceType,
                                    FNVhash.FNV24(NewMeshName.Text + "_lod" + lodpart[0].ToString() + (lodpart[1] > 0 ? "_" + lodpart[1].ToString() : "")) | 0x80000000,
                                    meshInstance);
                                CASPlist[i].replaceLink(oldtgi, newtgi);
                                if (rMap != null) rMap.replaceLink(oldtgi, newtgi);
                                rResNew.ResourceGroup = newtgi.Group;
                                rResNew.Instance = newtgi.Instance;

                                if (g.ShaderHash != 0 && g.Shader.normalIndex >= 0)
                                {
                                    TGI oldBump = g.TGIList[g.Shader.normalIndex];
                                    TGI tmp2;
                                    if (linkLookup.TryGetValue(oldBump, out tmp2))
                                    {
                                        g.setTGI(g.Shader.normalIndex, tmp2);
                                    }
                                    else
                                    {
                                        TGI newBump = new TGI(oldBump.Type, oldBump.Group | 0x80000000,
                                            FNVhash.FNV64(newMeshNameRandom + "_bumpmap") | 0x8000000000000000);
                                        if (CopyMeshTexture(oldBump, newBump, newPack))
                                        {
                                            g.setTGI(g.Shader.normalIndex, newBump);
                                            linkLookup.Add(oldBump, newBump);
                                        }
                                    }
                                }
                                if (g.ShaderHash != 0 && g.Shader.emissionIndex >= 0)
                                {
                                    TGI oldGlow = g.TGIList[g.Shader.emissionIndex];
                                    TGI tmp3;
                                    if (linkLookup.TryGetValue(oldGlow, out tmp3))
                                    {
                                        g.setTGI(g.Shader.emissionIndex, tmp3);
                                    }
                                    else
                                    {
                                        TGI newGlow = new TGI(oldGlow.Type, oldGlow.Group | 0x80000000,
                                            FNVhash.FNV64(newMeshNameRandom + "_emission") | 0x8000000000000000);
                                        if (CopyMeshTexture(oldGlow, newGlow, newPack))
                                        {
                                            g.setTGI(g.Shader.emissionIndex, newGlow);
                                            linkLookup.Add(oldGlow, newGlow);
                                        }
                                    }
                                }
                                BinaryWriter bwg = new BinaryWriter(mr);
                                g.WriteFile(bwg);

                            }
                            else if (tgi == CASPlist[i].LinkList[CASPlist[i].RegionMapIndex])
                            {
                                CASPlist[i].setLink(CASPlist[i].RegionMapIndex, new TGI(rMesh.ResourceType, rMesh.ResourceGroup | 0x80000000, meshInstance));
                                continue;
                            }
                            else
                            {
                                int len = (int)sr.Length;
                                byte[] buff = new byte[len];
                                sr.Read(buff, 0, len);
                                mr.Write(buff, 0, len);
                                if (tgi == CASPlist[i].LinkList[CASPlist[i].SwatchIndex])
                                {
                                    rResNew.Instance = FNVhash.FNV32(newPartNameRandom) | 0x8000000000000000;
                                    CASPlist[i].setLink(CASPlist[i].SwatchIndex, new TGI(rResNew.ResourceType, rResNew.ResourceGroup, rResNew.Instance));
                                }
                                else if (tgi == CASPlist[i].LinkList[CASPlist[i].TextureIndex])
                                {
                                    rResNew.ResourceGroup = rRes.ResourceGroup | 0x80000000;
                                    rResNew.Instance = FNVhash.FNV64(newPartNameRandom) | 0x8000000000000000;
                                    CASPlist[i].setLink(CASPlist[i].TextureIndex, new TGI(rResNew.ResourceType, rResNew.ResourceGroup, rResNew.Instance));
                                }
                                else if (tgi == CASPlist[i].LinkList[CASPlist[i].ShadowIndex])
                                {
                                    rResNew.ResourceGroup = rRes.ResourceGroup | 0x80000000;
                                    rResNew.Instance = FNVhash.FNV64(newMeshNameRandom + "_shadow") | 0x8000000000000000;
                                    CASPlist[i].setLink(CASPlist[i].ShadowIndex, new TGI(rResNew.ResourceType, rResNew.ResourceGroup, rResNew.Instance));
                                }
                                else if (tgi == CASPlist[i].LinkList[CASPlist[i].NormalMapIndex])
                                {
                                    rResNew.ResourceGroup = rRes.ResourceGroup | 0x80000000;
                                    rResNew.Instance = FNVhash.FNV64(newMeshNameRandom + "_normal") | 0x8000000000000000;
                                    CASPlist[i].setLink(CASPlist[i].NormalMapIndex, new TGI(rResNew.ResourceType, rResNew.ResourceGroup, rResNew.Instance));
                                }
                                else if (tgi == CASPlist[i].LinkList[CASPlist[i].SpecularIndex])
                                {
                                    rResNew.ResourceGroup = rRes.ResourceGroup | 0x80000000;
                                    rResNew.Instance = FNVhash.FNV64(newMeshNameRandom + "_specular") | 0x8000000000000000;
                                    CASPlist[i].setLink(CASPlist[i].SpecularIndex, new TGI(rResNew.ResourceType, rResNew.ResourceGroup, rResNew.Instance));
                                }
                                else if (tgi == CASPlist[i].LinkList[CASPlist[i].EmissionIndex])
                                {
                                    rResNew.ResourceGroup = rRes.ResourceGroup | 0x80000000;
                                    rResNew.Instance = FNVhash.FNV64(newMeshNameRandom + "_emission") | 0x8000000000000000;
                                    CASPlist[i].setLink(CASPlist[i].EmissionIndex, new TGI(rResNew.ResourceType, rResNew.ResourceGroup, rResNew.Instance));
                                }
                                else if (tgi == CASPlist[i].LinkList[CASPlist[i].ColorShiftMaskIndex])
                                {
                                    rResNew.ResourceGroup = rRes.ResourceGroup | 0x80000000;
                                    rResNew.Instance = FNVhash.FNV64(newMeshNameRandom + "_colorshiftmask") | 0x8000000000000000;
                                    CASPlist[i].setLink(CASPlist[i].ColorShiftMaskIndex, new TGI(rResNew.ResourceType, rResNew.ResourceGroup, rResNew.Instance));
                                }
                                else
                                {
                                    TGI oldtgi = new TGI(rRes.ResourceType, rRes.ResourceGroup, rRes.Instance);
                                    TGI newtgi = new TGI(rRes.ResourceType, rRes.ResourceGroup | 0x80000000,
                                        FNVhash.FNV64(newPartNameRandom + "_unknown" + unknownCounter.ToString()) | 0x8000000000000000);
                                    CASPlist[i].replaceLink(oldtgi, newtgi);
                                    rResNew.ResourceGroup = newtgi.Group;
                                    rResNew.Instance = newtgi.Instance;
                                    unknownCounter++;
                                }
                            }
                            IResourceIndexEntry irieRes = newPack.AddResource(rResNew, mr, true);
                            if (irieRes != null) irieRes.Compressed = (ushort)0x5A42;
                            linkLookup.Add(tgi, new TGI(rResNew.ResourceType, rResNew.ResourceGroup, rResNew.Instance));
                        }
                        DeleteResource(newPack, irCASPlist[i]);
                        MemoryStream m = new MemoryStream();
                        BinaryWriter bw = new BinaryWriter(m);
                        CASPlist[i].Write(bw);
                        IResourceIndexEntry irieCasp = newPack.AddResource(irCASPlist[i], m, true);
                        irieCasp.Compressed = (ushort)0x5A42;
                        if (rMap != null)
                        {
                            TGIBlock rMeshNew = new TGIBlock(0, null, rMesh.ResourceType, rMesh.ResourceGroup | 0x80000000, meshInstance);
                            MemoryStream mm = new MemoryStream();
                            BinaryWriter bwm = new BinaryWriter(mm);
                            rMap.setInternalLink(new TGI(rMeshNew.ResourceType, rMeshNew.ResourceGroup, rMeshNew.Instance));
                            rMap.Write(bwm);
                            IResourceIndexEntry irieMesh = newPack.AddResource(rMeshNew, mm, true);
                            if (irieMesh != null) irieMesh.Compressed = (ushort)0x5A42;
                        }
                    }
                }
            }

            if (WritePackage("Save new package", newPack, NewMeshName.Text))
            {
                newPack.Dispose();
            }
            else
            {
                MessageBox.Show("Could not save cloned package!");
                newPack.Dispose();
            }
        }

        internal bool CopyMeshTexture(TGI oldTGI, TGI newTGI, Package newPack)
        {
            Predicate<IResourceIndexEntry> getTexture = r => r.ResourceType == oldTGI.Type & r.ResourceGroup == oldTGI.Group &
                                r.Instance == oldTGI.Instance;
            IResourceIndexEntry irTexture = myPack.Find(getTexture);
            if (irTexture == null) return false;
            Stream s = myPack.GetResource(irTexture);
            s.Position = 0;
            TGIBlock rResNew = new TGIBlock(1, null, newTGI.Type, newTGI.Group, newTGI.Instance);
            IResourceIndexEntry irieRes = newPack.AddResource(rResNew, s, true);
            if (irieRes != null) irieRes.Compressed = (ushort)0x5A42;
            return true;
        }

        internal IResourceIndexEntry FindResource(Predicate<IResourceIndexEntry> getRes, out Package foundInPackage)
        {
            IResourceIndexEntry rRes = null;
            if (myPack != null)
            {
                rRes = myPack.Find(getRes);
            }
            if (rRes == null)
            {
                if (GamePack_radioButton.Checked)
                {
                    foreach (Package p in resourcePacks)
                    {
                        rRes = p.Find(getRes);
                        if (rRes != null)
                        {
                            foundInPackage = p;
                            return rRes;
                        }
                    }
                    if (rRes == null)
                    {
                        foundInPackage = null;
                        return null;
                    }
                }
                else
                {
                    foundInPackage = null;
                    return null;
                }
            }
            foundInPackage = myPack;
            return rRes;
        }

        private bool CloneResource(TGI oldTGI, Package targetPack)
        {
            return CloneResource(oldTGI, oldTGI, targetPack);
        }

        private bool CloneResource(TGI oldTGI, TGI newTGI, Package targetPack)
        {
            Predicate<IResourceIndexEntry> getBump = r => r.ResourceType == oldTGI.Type & r.ResourceGroup == oldTGI.Group &
                                r.Instance == oldTGI.Instance;
            Package tmpPack = null;
            IResourceIndexEntry rRes = FindResource(getBump, out tmpPack);
            if (rRes == null)
            {
                return false;
            }
            Stream s = tmpPack.GetResource(rRes);
            s.Position = 0;
            int len = (int)s.Length;
            byte[] buff = new byte[len];
            s.Read(buff, 0, len);
            MemoryStream m = new MemoryStream();
            m.Write(buff, 0, len);
            TGIBlock rResNew = new TGIBlock(0, null, newTGI.Type, newTGI.Group, newTGI.Instance);
            IResourceIndexEntry irieRes = targetPack.AddResource(rResNew, m, true);
            if (irieRes != null) irieRes.Compressed = (ushort)0x5A42;
            return true;
        }
    }
}
