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
using System.Reflection;
using Xmods.DataLib;
using s4pi.Interfaces;
using s4pi.Package;
using s4pi.ImageResource;

namespace XMODS
{
    public partial class Form1 : Form
    {
        string PythonSourceFilter = "py files (*.py)|*.py|All files (*.*)|*.*";
        List<ProtoEnum> enumFolderList;
        List<ProtoClass> classFolderList;
        List<string> fieldsFolderList;
        Package caspPack;

        private void XMLtagFile_button_Click(object sender, EventArgs e)
        {
            string XMLfilter = "XML file with tags list (*.xml)|*.xml|All files (*.*)|*.*";
            XMLtagFile.Text = GetFilename("Select xml file", XMLfilter);
            FormatXMLtagsAsEnum();
        }

        private void FormatXMLtagsAsEnum()
        {
            XmlTextReader reader = new XmlTextReader(XMLtagFile.Text);
            string output = "";
            uint val = 0;
            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        string s = reader.GetAttribute("n");
                        if (s != null && s.StartsWith("Tag"))
                        {
                            output += "****  " + s + ":" + Environment.NewLine;
                        }
                        else if (String.Compare(reader.Name, "T") == 0)
                        {
                            val = UInt32.Parse(reader.GetAttribute("ev"));
                        }
                        break;
                    case XmlNodeType.Text:
                        output += reader.Value.Replace("'", "").Replace("-", "_") + " = 0x" + val.ToString("X4") + "," + Environment.NewLine;
                        break;
                }
            }
            XMLtagEnumOutput.Text = output;
        }

        private void XMLtagCopyOutput_button_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(XMLtagEnumOutput.Text);
        }

        private void XMLbuffTrait_button_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folder = new FolderBrowserDialog();
            folder.Description = "Select folder containing XML files to be searched or extracted from";
            folder.ShowNewFolderButton = false;
            DialogResult res = folder.ShowDialog();
            if (res == DialogResult.OK)
            {
                XMLbuffTraitFolder.Text = folder.SelectedPath;
                FormatXMLbuffTraitAsEnum();
            }
        }

        private void FormatXMLbuffTraitAsEnum()
        {
            string[] fileslist = Directory.GetFiles(XMLbuffTraitFolder.Text, "*.xml", SearchOption.TopDirectoryOnly);
            string output = "";
            bool headerDone = false;
            foreach (string f in fileslist)
            {
                XmlTextReader reader = new XmlTextReader(f);
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        string c = reader.GetAttribute("c");
                        if (c != null)
                        {
                            if (!headerDone)
                            {
                                output += "public enum TS4" + c + "s" + Environment.NewLine + "{" + Environment.NewLine;
                                headerDone = true;
                            }
                            output += reader.GetAttribute("n").Replace('-', '_').Replace('(', '_').Replace(')', '_').Replace("+", "") +
                                " = " + reader.GetAttribute("s") + "," + Environment.NewLine;
                        }
                    }
                }
            }
            output += "}";
            XMLbuffTraitOutput.Text = output;
        }

        private void XMLbuffTraitCopy_button_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(XMLbuffTraitOutput.Text);
        }

        private void ShaderSearch_button_Click(object sender, EventArgs e)
        {
            List<string> snameList = new List<string>();
            List<GEOM.MTNF> shaderList = new List<GEOM.MTNF>();
            List<int> shaderCount = new List<int>();
            Predicate<IResourceIndexEntry> pred = r => r.ResourceType == (uint)XmodsEnums.ResourceTypes.GEOM;
            foreach (Package p in gamePacks0)
            {
                List<IResourceIndexEntry> iries = p.FindAll(pred);
                foreach (IResourceIndexEntry irie in iries)
                {
                    using (BinaryReader br = new BinaryReader(p.GetResource(irie)))
                    {
                        GEOM g = new GEOM(br);
                        if (g.Shader == null) continue;
                        string tmp;
                        if (g.ShaderName != null)
                        {
                            tmp = g.ShaderName + ": " + g.Shader.getParamsNameValueString(false);
                        }
                        else
                        {
                            tmp = "0x" + g.ShaderHash.ToString("X8") + ": " + g.Shader.getParamsNameValueString(false);
                        }
                        int i = snameList.IndexOf(tmp);
                        if (i >= 0)
                        {
                            shaderCount[i]++;
                        }
                        else
                        {
                            snameList.Add(tmp);
                            shaderList.Add(g.Shader);
                            shaderCount.Add(1);
                        }
                    }
                }
            }
            for (int i = 0; i < snameList.Count; i++)
            {
                ShaderSearchOutput.Text += shaderCount[i].ToString() + " " + snameList[i] + Environment.NewLine;
                string tmp = "new uint[] { ";
                uint[] sdata = shaderList[i].toDataArray();
                foreach (uint u in sdata)
                {
                    tmp += u.ToString() + ", ";
                }
                ShaderSearchOutput.Text += tmp + " }" + Environment.NewLine + Environment.NewLine;
            }
        }

        private void ListVertexColors_button_Click(object sender, EventArgs e)
        {
            List<uint> vcolorList = new List<uint>();
            List<int> vcolorCount = new List<int>();
            Predicate<IResourceIndexEntry> pred = r => r.ResourceType == (uint)XmodsEnums.ResourceTypes.GEOM;
            foreach (Package p in gamePacks0)
            {
                List<IResourceIndexEntry> iries = p.FindAll(pred);
                foreach (IResourceIndexEntry irie in iries)
                {
                    using (BinaryReader br = new BinaryReader(p.GetResource(irie)))
                    {
                        GEOM g = new GEOM(br);
                        if (!g.hasTags) break;
                        uint tmp = g.getTagval(0);
                        int i = vcolorList.IndexOf(tmp);
                        if (i >= 0)
                        {
                            vcolorCount[i]++;
                        }
                        else
                        {
                            vcolorList.Add(tmp);
                            vcolorCount.Add(1);
                        }
                    }
                }
            }
            for (int i = 0; i < vcolorList.Count; i++)
            {
                string[] tmp = new string[2];
                tmp[0] = vcolorCount[i].ToString().PadLeft(4);
                tmp[1] = vcolorList[i].ToString("X8");
                ListVertexColors_dataGridView.Rows.Add(tmp);
            }
        }

        private void SpecialSortLayer_button_Click(object sender, EventArgs e)
        {
            List<uint> sortLayerType = new List<uint>();
            List<string> partList = new List<string>();
            List<int> sortLayerVal = new List<int>();
            List<List<string>> sortLayerPartNames = new List<List<string>>();
            Predicate<IResourceIndexEntry> pred = r => r.ResourceType == (uint)XmodsEnums.ResourceTypes.CASP;
            foreach (Package p in gamePacks0)
            {
                List<IResourceIndexEntry> iries = p.FindAll(pred);
                foreach (IResourceIndexEntry irie in iries)
                {
                    using (BinaryReader br = new BinaryReader(p.GetResource(irie)))
                    {
                        CASP c = null;
                        try
                        {
                            c = new CASP(br);
                        }
                        catch (CASP.CASPEmptyException)
                        {
                            continue;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Can't read this CASP: " + irie.ToString() + Environment.NewLine + ex.Message + Environment.NewLine + ex.StackTrace);
                            continue;
                        }

                        bool addNew = true;
                        int index = 0;
                        for (int i = 0; i < sortLayerVal.Count; i++)
                        {
                            if (c.SortLayer == sortLayerVal[i] && (uint)c.BodyType == sortLayerType[i])
                            {
                                addNew = false;
                                index = i;
                                break;
                            }
                        }
                        if (addNew)
                        {
                            sortLayerType.Add((uint)c.BodyType);
                            partList.Add(Enum.GetName(typeof(XmodsEnums.BodyType), (uint)c.BodyType));
                            sortLayerVal.Add(c.SortLayer);
                            sortLayerPartNames.Add(new List<string>(new string[] { c.PartName }));
                        }
                        else
                        {
                            sortLayerPartNames[index].Add(c.PartName);
                        }
                    }
                }
            }
            for (int i = 0; i < partList.Count; i++)
            {
                string[] tmp = new string[4];
                tmp[0] = sortLayerType[i].ToString().PadLeft(6);
                tmp[1] = partList[i];
                tmp[2] = sortLayerVal[i].ToString().PadLeft(6);
                tmp[3] = sortLayerPartNames[i].Count.ToString().PadLeft(6);
                int row = specialSortLayer_dataGridView.Rows.Add(tmp);
                specialSortLayer_dataGridView.Rows[row].Tag = sortLayerPartNames[i];
            }
            specialSortLayer_dataGridView.Sort(specialSortLayer_dataGridView.Columns[0], ListSortDirection.Ascending);
        }

        private void specialSortLayer_dataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            string partList = "";
            foreach (string s in (List<string>)specialSortLayer_dataGridView.Rows[e.RowIndex].Tag)
            {
                partList += s + ", ";
            }
            MessageBox.Show(partList);
        }

        private void searchForID_button_Click(object sender, EventArgs e)
        {
            UInt32 type = 0;
            if (string.Compare(searchForTypeID.Text, " ") > 0)
            {
                try
                {
                    type = UInt32.Parse(searchForTypeID.Text.Replace("0x", "").Replace("0X", ""), System.Globalization.NumberStyles.HexNumber);
                }
                catch
                {
                    MessageBox.Show("Please enter a valid hex type ID!");
                    return;
                }
            }
            UInt32 group = 0;
            if (string.Compare(searchForGroupID.Text, " ") > 0)
            {
                try
                {
                    group = UInt32.Parse(searchForGroupID.Text.Replace("0x", "").Replace("0X", ""), System.Globalization.NumberStyles.HexNumber);
                }
                catch
                {
                    MessageBox.Show("Please enter a valid hex group ID!");
                    return;
                }
            }
            UInt64 instance = 0;
            if (string.Compare(searchForInstanceID.Text, " ") > 0)
            {
                try
                {
                    instance = UInt64.Parse(searchForInstanceID.Text.Replace("0x", "").Replace("0X", ""), System.Globalization.NumberStyles.HexNumber);
                }
                catch
                {
                    MessageBox.Show("Please enter a valid hex instance ID!");
                    return;
                }
            }
            string TS4FilesPath = Properties.Settings.Default.TS4Path;
            string[] paths = Directory.GetFiles(TS4FilesPath, "*.package", SearchOption.AllDirectories);
            if (paths.Length == 0)
            {
                MessageBox.Show("Can't find game packages! Please go to File / Change Settings and correct the game packages path or make it blank to reset, then restart.");
                return;
            }
            string output = "";
            foreach (string s in paths)
            {
                Package p = OpenPackage(s, false);
                if (p == null)
                {
                    MessageBox.Show("Can't read package: " + s);
                    return;
                }
                Predicate<IResourceIndexEntry> getID = r => (String.Compare(searchForTypeID.Text, " ") > 0 ? r.ResourceType == type : true) &&
                    (String.Compare(searchForGroupID.Text, " ") > 0 ? r.ResourceGroup == group : true) &&
                    (String.Compare(searchForInstanceID.Text, " ") > 0 ? r.Instance == instance : true);

                List<IResourceIndexEntry> ires = p.FindAll(getID);
                foreach (IResourceIndexEntry i in ires)
                {
                    output += s + " : " + i.ResourceType.ToString("X8") + "_" + i.ResourceGroup.ToString("X8") + "_" + i.Instance.ToString("X16") + Environment.NewLine;
                }
            }
            searchForInstanceID_output.Text = output;
        }

        private void EmbedResourcesSelect_button_Click(object sender, EventArgs e)
        {
            string pack = GetFilename("Select Package File", Packagefilter);
            EmbedResourcesPackage.Text = pack;
            EmbedResourcesPackage.Refresh();
            Package p = OpenPackage(pack, false);
            Predicate<IResourceIndexEntry> pred = r => r.ResourceType == (uint)XmodsEnums.ResourceTypes.CASP;
            List<IResourceIndexEntry> iries = p.FindAll(pred);
            foreach (IResourceIndexEntry ires in iries)
            {
                CASP c = new CASP(new BinaryReader(p.GetResource(ires)));
                TGI[] tgiLinks = c.LinkList;
                CopyResource(tgiLinks[c.RegionMapIndex], p);
                CopyResource(tgiLinks[c.TextureIndex], p);
                CopyResource(tgiLinks[c.ShadowIndex], p);
                CopyResource(tgiLinks[c.SpecularIndex], p);
                CopyResource(tgiLinks[c.SwatchIndex], p);
                for (int lod = 0; lod < 4; lod++)
                {
                    TGI[] geomLinks = c.MeshParts(lod);
                    for (int i = 0; i < geomLinks.Length; i++)
                    {
                        CopyGeomResource(geomLinks[i], p);
                    }
                }
            }
            WritePackage("Save package with embedded resources", p, "");
        }
        private void CopyResource(TGI tgi, Package p)
        {
            if (tgi.Instance == 0) return;
            Package[] packList = (tgi.Type == (uint)XmodsEnums.ResourceTypes.RMAP) ? gamePacks0 : gamePacksOther;
            Predicate<IResourceIndexEntry> pred = r => r.ResourceType == tgi.Type && r.ResourceGroup == tgi.Group && r.Instance == tgi.Instance;
            for (int i = 0; i < packList.Length; i++)
            {
                IResourceIndexEntry irie = packList[i].Find(pred);
                if (irie != null)
                {
                    Stream s = packList[i].GetResource(irie);
                    s.Position = 0;
                    IResourceIndexEntry ir = p.AddResource(irie, s, true);
                    if (ir != null) ir.Compressed = (ushort)0x5A42;
                    break;
                }
            }
        }
        private void CopyGeomResource(TGI tgi, Package p)
        {
            if (tgi.Instance == 0) return;
            Predicate<IResourceIndexEntry> pred = r => r.ResourceType == tgi.Type && r.ResourceGroup == tgi.Group && r.Instance == tgi.Instance;
            for (int i = 0; i < gamePacks0.Length; i++)
            {
                IResourceIndexEntry irie = gamePacks0[i].Find(pred);
                if (irie != null)
                {
                    Stream s = gamePacks0[i].GetResource(irie);
                    MemoryStream ms = new MemoryStream();
                    s.Position = 0;
                    s.CopyTo(ms);
                    s.Position = 0;
                    IResourceIndexEntry ir = p.AddResource(irie, s, true);
                    if (ir != null) ir.Compressed = (ushort)0x5A42;
                    //try
                    //{
                        GEOM g = new GEOM(new BinaryReader(ms));
                        if (g.Shader.normalIndex > -1)
                        {
                            TGI normTGI = g.TGIList[g.Shader.normalIndex];
                            CopyResource(normTGI, p);
                        }
                        if (g.Shader.emissionIndex > -1)
                        {
                            TGI emitTGI = g.TGIList[g.Shader.emissionIndex];
                            CopyResource(emitTGI, p);
                        }
                    //}
                    //catch
                    //{
                    //    MessageBox.Show("Unreadable GEOM: " + tgi.ToString());
                    //}
                    break;
                }
            }
        }

        private void SearchOtherPackage_button_Click(object sender, EventArgs e)
        {
            string pack = GetFilename("Select Package File", Packagefilter);
            Package p = OpenPackage(pack, false);
            ListSelectedParts(new Package[] { p });
        }

        private void ListSelectedParts_button_Click(object sender, EventArgs e)
        {
            ListSelectedParts(gamePacks0);
        }

        private void ListSelectedParts(Package[] packages)
        {
            AltParts_dataGridView.Rows.Clear();
            bool ageAdult = AlternatePartsAdult_radioButton.Checked;
            bool ageChild = AlternatePartsChild_radioButton.Checked;
            bool genderMale = AlternatePartsMale_radioButton.Checked;
            bool genderFemale = AlternatePartsFemale_radioButton.Checked;
            bool genderUnisex = AlternatePartsUnisex_radioButton.Checked;
            bool flagDisableForHuman = AlternatePartsFilter_checkedListBox.GetItemChecked(0);
            bool flagDisableForAlien = AlternatePartsFilter_checkedListBox.GetItemChecked(1);
            bool flagAllowForCASRandom = AlternatePartsFilter_checkedListBox.GetItemChecked(2);
            bool flagAllowForLiveRandom = AlternatePartsFilter_checkedListBox.GetItemChecked(3);
            bool flagShowInUI = AlternatePartsFilter_checkedListBox.GetItemChecked(4);
            bool flagShowInInfoPanel = AlternatePartsFilter_checkedListBox.GetItemChecked(5);
            bool flagRestrictOppositeGender = AlternatePartsFilter_checkedListBox.GetItemChecked(6);
            bool flagRestrictOppositeFrame = AlternatePartsFilter_checkedListBox.GetItemChecked(7);
            bool flagDefaultNudeMale = AlternatePartsFilter_checkedListBox.GetItemChecked(8);
            bool flagDefaultNudeFemale = AlternatePartsFilter_checkedListBox.GetItemChecked(9);
            bool flagUnknown = AlternatePartsFilter_checkedListBox.GetItemChecked(10);
            bool flagMustBeKnitted = AlternatePartsFilter_checkedListBox.GetItemChecked(11);
            bool hasOppositeGenderPart = AlternateParts_checkedListBox.GetItemChecked(0);
            bool hasFallbackPart = AlternateParts_checkedListBox.GetItemChecked(1);
            bool hasSlotLinks = AlternateParts_checkedListBox.GetItemChecked(2);
            bool hasSliderData = AlternateParts_checkedListBox.GetItemChecked(3);
            bool hasNakedKey = AlternateParts_checkedListBox.GetItemChecked(4);
            bool hasParentKey = AlternateParts_checkedListBox.GetItemChecked(5);
            bool hasUnknown1 = AlternateParts_checkedListBox.GetItemChecked(6);
            bool hasCASPID = false;
            uint id = 0;
            if (String.Compare(SearchCASPid.Text, " ") > 0)
            {
                try
                {
                    id = UInt32.Parse(SearchCASPid.Text.Replace("0x", "").Replace("0X", ""), System.Globalization.NumberStyles.HexNumber);
                    hasCASPID = true;
                }
                catch
                {
                    MessageBox.Show("Please enter a valid CASP ID in hex format!");
                    return;
                }
            }
            bool hasBodyType = false;
            XmodsEnums.BodyType bodytype = XmodsEnums.BodyType.All;
            if (SearchCASPBodyType_comboBox.SelectedIndex >= 0)
            {
                if (Enum.TryParse((string)SearchCASPBodyType_comboBox.SelectedItem, out bodytype)) hasBodyType = true;
            }

            caspPack = (Package)Package.NewPackage(1);
            string packname = "BaseGame";
            List<ulong> instances = new List<ulong>();
            bool gotSome = false;
            Predicate<IResourceIndexEntry> pred = r => r.ResourceType == (uint)XmodsEnums.ResourceTypes.CASP;
            for (int i = 0; i < packages.Length; i++)
            //  foreach (Package p in gamePacks0)
            {
                Package p = packages[i];
                List<IResourceIndexEntry> iries = p.FindAll(pred);
                foreach (IResourceIndexEntry irie in iries)
                {
                    if (instances.IndexOf(irie.Instance) >= 0) continue;
                    instances.Add(irie.Instance);
                    using (BinaryReader br = new BinaryReader(p.GetResource(irie)))
                    {
                        CASP c = null;
                        try
                        {
                            c = new CASP(br);
                        }
                        catch (CASP.CASPEmptyException)
                        {
                            continue;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Can't read this CASP: " + irie.ToString() + Environment.NewLine + ex.Message + Environment.NewLine + ex.StackTrace);
                            continue;
                        }
                        if ((ageAdult ? c.Age > XmodsEnums.Age.Child : true) &&
                            (ageChild ? c.Age < XmodsEnums.Age.Teen : true) &&
                            (genderMale ? c.Gender == XmodsEnums.Gender.Male : true) &&
                            (genderFemale ? c.Gender == XmodsEnums.Gender.Female : true) &&
                            (genderUnisex ? c.Gender == XmodsEnums.Gender.Unisex : true) &&
                            (flagDisableForHuman ? c.OccultDisableForHuman : true) &&
                            (flagDisableForAlien ? c.OccultDisableForAlien : true) &&
                            (flagAllowForCASRandom ? c.FlagAllowForCASRandom : true) &&
                            (flagAllowForLiveRandom ? c.FlagAllowForLiveRandom : true) &&
                            (flagShowInInfoPanel ? c.FlagShowInSimInfoPanel : true) &&
                            (flagShowInUI ? c.FlagShowInUI : true) &&
                            (flagRestrictOppositeGender ? c.FlagRestrictOppositeGender : true) &&
                            (flagRestrictOppositeFrame ? c.FlagRestrictOppositeFrame : true) &&
                            (flagDefaultNudeMale ? c.FlagDefaultForBodyTypeMale : true) &&
                            (flagDefaultNudeFemale ? c.FlagDefaultForBodyTypeFemale : true) &&
                            (flagUnknown ? c.FlagUnknown : true) &&
                            (flagMustBeKnitted ? c.FlagCreateInGame : true) &&
                            (hasOppositeGenderPart ? c.OppositeGenderPart > 0 : true) &&
                            (hasFallbackPart ? c.FallbackPart > 0 : true) &&
                            (hasNakedKey ? c.NakedKey > 0 : true) &&
                            (hasParentKey ? c.ParentKey > 0 : true) &&
                            (hasUnknown1 ? c.Unknown1 > 0 : true ) &&
                            (hasSlotLinks ? CASPhasSlots(c) : true) &&
                            (hasSliderData ? (c.BrightnessSliderSettings != null && (c.BrightnessSliderSettings.Minimum != 0 || c.BrightnessSliderSettings.Maximum != 0) || 
                                             c.HueSliderSettings != null && (c.HueSliderSettings.Minimum != 0 || c.HueSliderSettings.Maximum != 0) || 
                                             c.OpacitySliderSettings != null && c.OpacitySliderSettings.Minimum != 0) : true) &&
                            (hasCASPID ? c.OutfitID == id : true) &&
                            (hasBodyType ? c.BodyType == bodytype : true) &&
                            (SearchCASPknit_checkBox.Checked ? c.CreateDescriptionKey != 0 : true))
                        {

                            //IResourceIndexEntry rleIres;
                            //bool inPack = false;
                            //RLEResource rle = FindCloneTextureRLE(c.LinkList[c.TextureIndex], out rleIres, out inPack);
                            //if (rle != null)
                            //{
                            //    TGIBlock tgi2 = new TGIBlock(1, null, rleIres.ResourceType, rleIres.ResourceGroup, rleIres.Instance);
                            //    IResourceIndexEntry ir2 = caspPack.AddResource(tgi2, rle.Stream, true);
                            //    if (ir2 != null)
                            //    {
                            //        ir2.Compressed = 0x5A42;

                            Stream s = new MemoryStream();
                            c.Write(new BinaryWriter(s));
                            s.Position = 0;
                            TGIBlock tgi = new TGIBlock(1, null, irie.ResourceType, irie.ResourceGroup, irie.Instance);
                            IResourceIndexEntry ir = caspPack.AddResource(tgi, s, true);
                            ir.Compressed = 0x5A42;

                            gotSome = true;
                            //    }
                            //}

                            AltParts_dataGridView.Rows.Add(new string[] { irie.ToString() + " v0x" + c.Version.ToString("X"), 
                                c.PartName, c.Age.ToString(), c.Gender.ToString(), 
                                c.OppositeGenderPart > 0 ? "0x" + c.OppositeGenderPart.ToString("X16") : "", 
                                c.FallbackPart > 0 ? "0x" + c.FallbackPart.ToString("X16") : "" });
                        }
                    }
                }
            }
            //  if (gotSome) WritePackage("Save CASP package", caspPack, bodytype + packname + ".package");
            int count = AltParts_dataGridView.Rows.Count;
            AltParts_dataGridView.Rows.Add(new string[] { "Total found: " + count.ToString() });
        }

        private void SearchCASPSave_button_Click(object sender, EventArgs e)
        {
            WritePackage("Save CASP package", caspPack, "");
        }

        private bool CASPhasSlots(CASP c)
        {
            if (c.SlotKeys == null || c.SlotKeys.Length == 0) return false;
            bool tmp = false;
            foreach (byte ind in c.SlotKeys)
            {
                if (c.LinkList[ind].Instance > 0) tmp = true;
            }
            return tmp;
        }

        //private void ListMapSizes(bool isBumpMap)
        //{
        //    TGI blank = new TGI(0, 0, 0);
        //    List<XmodsEnums.BodyType> types = new List<XmodsEnums.BodyType>();
        //    List<List<System.Drawing.Size>> sizes = new List<List<System.Drawing.Size>>();
        //    Predicate<IResourceIndexEntry> pred = r => r.ResourceType == (uint)XmodsEnums.ResourceTypes.CASP;
        //    foreach (Package p in gamePacks0)
        //    {
        //        List<IResourceIndexEntry> iries = p.FindAll(pred);
        //        foreach (IResourceIndexEntry irie in iries)
        //        {
        //            using (BinaryReader br = new BinaryReader(p.GetResource(irie)))
        //            {
        //                CASP c = null;
        //                try
        //                {
        //                    c = new CASP(br);
        //                }
        //                catch (CASP.CASPEmptyException)
        //                {
        //                    continue;
        //                }
        //                catch (Exception ex)
        //                {
        //                    MessageBox.Show("Can't read this CASP: " + irie.ToString() + Environment.NewLine + ex.Message + Environment.NewLine + ex.StackTrace);
        //                    continue;
        //                }
        //                if ((isBumpMap && !c.LinkList[c.NormalMapIndex].Equals(blank)) || (!isBumpMap && !c.LinkList[c.EmissionIndex].Equals(blank)))
        //                {
        //                    FindCloneTextureDST(c.LinkList, c.NormalMapIndex, 
        //                    if (types.Contains(c.BodyType))
        //                    {
        //                        int ind = types.IndexOf(c.BodyType);
        //                        if (sizes[ind].Contains(new Size
        //                    }

        //                    AltParts_dataGridView.Rows.Add(new string[] { irie.ToString(), c.PartName, c.Age.ToString(), c.Gender.ToString(), 
        //                    c.OppositeGenderPart > 0 ? "0x" + c.OppositeGenderPart.ToString("X16") : "", 
        //                    c.FallbackPart > 0 ? "0x" + c.FallbackPart.ToString("X16") : "" });
        //                }
        //            }
        //        }
        //    }
        //}

        private void ListSeamVerts_button_Click(object sender, EventArgs e)
        {
            if (ListSeamVertsIntersection_radioButton.Checked) ListSeamVerts_Intersection();
            else ListSeamVerts_Stitches();
        }

        private void ListSeamVerts_Intersection()
        {
            string output = "";
            int numSpecies = 4;     // 1 - 4
            int numLods = 4;        // 0 - 3
            int numSeams = 7;       // 0 - 6

            bool codeOutput = ListSeamVertsCode_checkBox.Checked;

            System.Resources.ResourceManager rm = Properties.Resources.ResourceManager;

            if (codeOutput)
            {
                output = "Vector3[][][][][] meshSeamVerts = new Vector3[" + numSpecies.ToString() + "][][][][];   //indices: species, age/gender, lod, seam, verts" + Environment.NewLine;
            }

            for (int sp = 1; sp <= numSpecies; sp++)
            {
                if ((XmodsEnums.Species)sp == XmodsEnums.Species.Human)
                {
                    int numAgeGenders = 5;      //male, female, child, toddler, infant
                    if (codeOutput)
                    {
                        output += "meshSeamVerts[" + (sp - 1).ToString() + "] = new Vector3[5][][][];        //ageGenders" + Environment.NewLine;
                    }
                    string[] specifier = new string[numAgeGenders];
                    specifier[0] = "ym";
                    specifier[1] = "yf";
                    specifier[2] = "cu";
                    specifier[3] = "pu";
                    specifier[4] = "iu";
                    string[] specifierLong = new string[numAgeGenders];
                    specifierLong[0] = "Adult Male";
                    specifierLong[1] = "Adult Female";
                    specifierLong[2] = "Child";
                    specifierLong[3] = "Toddler";
                    specifierLong[4] = "Infant";
                    for (int ag = 0; ag < numAgeGenders; ag++)
                    {
                        if (codeOutput)
                        {
                            output += "meshSeamVerts[" + (sp - 1).ToString() + "][" + ag.ToString() + "] = new Vector3[" + numLods.ToString() + "][][];         //" + specifierLong[ag] + Environment.NewLine;
                        }
                        for (int l = 0; l < numLods; l++)
                        {
                            if (codeOutput)
                            {
                                output += "meshSeamVerts[" + (sp - 1).ToString() + "][" + ag.ToString() + "][" + l.ToString() + "] = new Vector3[" + numSeams.ToString() + "][];         //" + specifierLong[ag] + " LOD" + l.ToString() + " seams" + Environment.NewLine;
                            }
                            else
                            {
                                output += ((XmodsEnums.Species)sp).ToString() + " " + specifierLong[ag] + " LOD" + l.ToString() + ":" + Environment.NewLine;
                            }
                            GEOM head = new GEOM(new BinaryReader(new MemoryStream((byte[])rm.GetObject(specifier[ag] + "Head_lod" + l.ToString()))));
                            GEOM top = new GEOM(new BinaryReader(new MemoryStream((byte[])rm.GetObject(specifier[ag] + "Top_lod" + l.ToString()))));
                            GEOM bottom = new GEOM(new BinaryReader(new MemoryStream((byte[])rm.GetObject(specifier[ag] + "Bottom_lod" + l.ToString()))));
                            GEOM feet = new GEOM(new BinaryReader(new MemoryStream((byte[])rm.GetObject(specifier[ag] + "Shoes_lod" + l.ToString()))));
                            for (int seam = 0; seam < numSeams; seam++)
                            {
                                GEOM.SeamType seamName = (GEOM.SeamType)seam;
                                string tmp;
                                if (seamName == GEOM.SeamType.Ankles) tmp = IntersectionString(bottom, feet);
                                else if (seamName == GEOM.SeamType.Neck) tmp = IntersectionString(head, top);
                                else if ((seamName == GEOM.SeamType.Waist && ag >= 2) ||
                                         (seamName == GEOM.SeamType.WaistAdultFemale && ag == 1) ||
                                         (seamName == GEOM.SeamType.WaistAdultMale && ag == 0))
                                    tmp = IntersectionString(top, bottom);
                                else tmp = "";
                                if (codeOutput)
                                {
                                    output += "meshSeamVerts[" + (sp - 1).ToString() + "][" + ag.ToString() + "][" + l.ToString() + "][" + seam.ToString() + "] = ";
                                    output += (tmp.Length > 0 ? tmp : "new Vector3[0];");
                                }
                                else { output += tmp.Length > 0 ? seamName.ToString() + ": " + tmp + Environment.NewLine : ""; }
                                if (codeOutput) output += "     //" + seamName.ToString() + Environment.NewLine;
                            }
                        }
                        output += Environment.NewLine;
                    }
                }
                else
                {
                    int numAgeGenders;
                    string[] specifier;
                    string[] specifierLong;
                    if ((XmodsEnums.Species)sp == XmodsEnums.Species.LittleDog)
                    {
                        numAgeGenders = 1;      //there are only adult little dogs
                        if (codeOutput)
                        {
                            output += "  meshSeamVerts[" + (sp - 1).ToString() + "] = new Vector3[4][][][];        //ageSpecies" + Environment.NewLine +
                            "  meshSeamVerts[" + (sp - 1).ToString() + "][0] = new Vector3[" + numLods.ToString() + "][][];          //adult" + Environment.NewLine;
                        }
                        specifier = new string[numAgeGenders];
                        specifier[0] = "al";
                        specifierLong = new string[numAgeGenders];
                        specifierLong[0] = "Adult LittleDog";
                    }
                    else
                    {
                        numAgeGenders = 2;      //adult, child
                        if (codeOutput)
                        {
                            output += "  meshSeamVerts[" + (sp - 1).ToString() + "] = new Vector3[4][][][];        //ageSpecies" + Environment.NewLine +
                            "  meshSeamVerts[" + (sp - 1).ToString() + "][0] = new Vector3[" + numLods.ToString() + "][][];          //adult" + Environment.NewLine +
                            "  meshSeamVerts[" + (sp - 1).ToString() + "][1] = new Vector3[" + numLods.ToString() + "][][];          //child" + Environment.NewLine;
                        }
                        specifier = new string[numAgeGenders];
                        specifier[0] = "a" + ((XmodsEnums.Species)sp).ToString().Substring(0, 1).ToLower();
                        specifier[1] = "c" + ((XmodsEnums.Species)sp).ToString().Substring(0, 1).ToLower();
                        specifierLong = new string[numAgeGenders];
                        specifierLong[0] = "Adult " + ((XmodsEnums.Species)sp).ToString();
                        specifierLong[1] = ((XmodsEnums.Species)sp) == XmodsEnums.Species.Dog ? "Puppy" : "Kitten";
                    }
                    for (int ag = 0; ag < numAgeGenders; ag++)
                    {
                        if (codeOutput)
                        {
                            output += "meshSeamVerts[" + (sp - 1).ToString() + "][" + ag.ToString() + "] = new Vector3[" + numLods.ToString() + "][][];         //" + specifierLong[ag] + Environment.NewLine;
                        }
                        for (int l = 0; l < numLods; l++)
                        {
                            if (codeOutput)
                            {
                                output += "meshSeamVerts[" + (sp - 1).ToString() + "][" + ag.ToString() + "][" + l.ToString() + "] = new Vector3[" + numSeams.ToString() + "][];         //" + specifierLong[ag] + " LOD" + l.ToString() + " seams" + Environment.NewLine;
                            }
                            else
                            {
                                output += specifierLong[ag] + " LOD" + l.ToString() + ":" + Environment.NewLine;
                            }
                            GEOM ears = new GEOM(new BinaryReader(new MemoryStream((byte[])rm.GetObject(specifier[ag] + "EarsUp_lod" + l.ToString()))));
                            GEOM head = new GEOM(new BinaryReader(new MemoryStream((byte[])rm.GetObject(specifier[ag] + "Head_lod" + l.ToString()))));
                            GEOM body = new GEOM(new BinaryReader(new MemoryStream((byte[])rm.GetObject(specifier[ag] + "Body_lod" + l.ToString()))));
                            GEOM tail = new GEOM(new BinaryReader(new MemoryStream((byte[])rm.GetObject(specifier[ag] + "Tail_lod" + l.ToString()))));
                            GEOM feet = new GEOM(new BinaryReader(new MemoryStream((byte[])rm.GetObject(specifier[ag] + "Shoes_lod" + l.ToString()))));
                            for (int seam = 0; seam < numSeams; seam++)
                            {
                                GEOM.SeamType seamName = (GEOM.SeamType)seam;
                                string tmp;
                                if (seamName == GEOM.SeamType.Ankles) tmp = IntersectionString(body, feet);
                                else if (seamName == GEOM.SeamType.Ears) tmp = IntersectionString(head, ears);
                                else if (seamName == GEOM.SeamType.Neck) tmp = IntersectionString(head, body);
                                else if (seamName == GEOM.SeamType.Tail) tmp = IntersectionString(body, tail);
                                else tmp = "";
                                if (codeOutput)
                                {
                                    output += "meshSeamVerts[" + (sp - 1).ToString() + "][" + ag.ToString() + "][" + l.ToString() + "][" + seam.ToString() + "] = ";
                                    output += (tmp.Length > 0 ? tmp : "new Vector3[0];");
                                }
                                else { output += tmp.Length > 0 ? seamName.ToString() + ": " + tmp + Environment.NewLine : ""; }
                                if (codeOutput) output += "     //" + seamName.ToString() + Environment.NewLine;
                            }
                        }
                        output += Environment.NewLine;
                    }
                }
            }

            SeamVertList.Text = output;
        }

        internal string IntersectionString(GEOM mesh1, GEOM mesh2)
        {
            string tmp = "";
            List<Vector3> seam = new List<Vector3>();
            for (int i = 0; i < mesh1.numberVertices; i++)
            {
                Vector3 pos = new Vector3(mesh1.getPosition(i));
                for (int j = 0; j < mesh2.numberVertices; j++)
                {
                    if (pos.Equals(new Vector3(mesh2.getPosition(j))))
                    {
                        seam.Add(pos);
                    }
                }
            }
            if (seam.Count == 0) return "";
            IEnumerable<Vector3> seamDistinct = seam.Distinct();
            if (ListSeamVertsCode_checkBox.Checked) tmp += "new Vector3[] { ";
            foreach (Vector3 p in seamDistinct)
            {
                if (ListSeamVertsCode_checkBox.Checked)
                {
                    tmp += "new Vector3(" + p.X.ToString() + "f, " + p.Y.ToString() + "f, " + p.Z.ToString() + "f), ";
                }
                else
                {
                    tmp += p.ToString() + " ; ";
                }
            }
            if (ListSeamVertsCode_checkBox.Checked)
            {
                tmp = tmp.TrimEnd(new char[] { ' ', ',' });
                tmp += " };";
            }
            return tmp;
        }

        private void ListSeamVerts_Stitches()
        {
            string output = "";
            int numSpecies = 4;     // 1 - 4
            int numLods = 4;        // 0 - 3
            int numSeams = 7;       // 0 - 6

            bool codeOutput = ListSeamVertsCode_checkBox.Checked;

            System.Resources.ResourceManager rm = Properties.Resources.ResourceManager;

            if (codeOutput)
            {
                output = "Vector3[][][][][] meshSeamVerts = new Vector3[" + numSpecies.ToString() + "][][][][];   //indices: species, age/gender, lod, seam, verts" + Environment.NewLine;
            }

            for (int sp = 1; sp <= numSpecies; sp++)
            {
                if ((XmodsEnums.Species)sp == XmodsEnums.Species.Human)
                {
                    int numAgeGenders = 5;      //male, female, child, toddlerk, infant
                    if (codeOutput)
                    {
                        output += "meshSeamVerts[" + (sp - 1).ToString() + "] = new Vector3[4][][][];        //ageGenders" + Environment.NewLine;
                    }
                    string[] specifier = new string[numAgeGenders];
                    specifier[0] = "ym";
                    specifier[1] = "yf";
                    specifier[2] = "cu";
                    specifier[3] = "pu";
                    specifier[4] = "iu";
                    string[] specifierLong = new string[numAgeGenders];
                    specifierLong[0] = "Adult Male";
                    specifierLong[1] = "Adult Female";
                    specifierLong[2] = "Child";
                    specifierLong[3] = "Toddler";
                    specifierLong[4] = "Infant";
                    for (int ag = 0; ag < numAgeGenders; ag++)
                    {
                        if (codeOutput)
                        {
                            output += "meshSeamVerts[" + (sp - 1).ToString() + "][" + ag.ToString() + "] = new Vector3[" + numLods.ToString() + "][][];         //" + specifierLong[ag] + Environment.NewLine;
                        }
                        for (int l = 0; l < numLods; l++)
                        {
                            if (codeOutput)
                            {
                                output += "meshSeamVerts[" + (sp - 1).ToString() + "][" + ag.ToString() + "][" + l.ToString() + "] = new Vector3[" + numSeams.ToString() + "][];         //" + specifierLong[ag] + " LOD" + l.ToString() + " seams" + Environment.NewLine;
                            }
                            else
                            {
                                output += ((XmodsEnums.Species)sp).ToString() + " " + specifierLong[ag] + " LOD" + l.ToString() + ":" + Environment.NewLine;
                            }
                            GEOM[] meshes = new GEOM[2];
                            meshes[0] = new GEOM(new BinaryReader(new MemoryStream((byte[])rm.GetObject(specifier[ag] + "Top_lod" + l.ToString()))));
                            meshes[1] = new GEOM(new BinaryReader(new MemoryStream((byte[])rm.GetObject(specifier[ag] + "Bottom_lod" + l.ToString()))));
                            for (int seam = 0; seam < numSeams; seam++)
                            {
                                string tmp = SeamString(meshes, seam);
                                if (codeOutput)
                                {
                                    output += "meshSeamVerts[" + (sp - 1).ToString() + "][" + ag.ToString() + "][" + l.ToString() + "][" + seam.ToString() + "] = ";
                                    output += (tmp.Length > 0 ? tmp : "new Vector3[0];");
                                }
                                else { output += tmp.Length > 0 ? ((GEOM.SeamType)seam).ToString() + ": " + tmp + Environment.NewLine : ""; }
                                if (codeOutput) output += "     //" + ((GEOM.SeamType)seam).ToString() + Environment.NewLine;
                            }
                        }
                        output += Environment.NewLine;
                    }
                }
                else
                {
                    int numAgeGenders;
                    string[] specifier;
                    string[] specifierLong;
                    if ((XmodsEnums.Species)sp == XmodsEnums.Species.LittleDog)
                    {
                        numAgeGenders = 1;      //only adult little dogs
                        if (codeOutput)
                        {
                            output += "  meshSeamVerts[" + (sp - 1).ToString() + "] = new Vector3[4][][][];        //ageSpecies" + Environment.NewLine +
                            "  meshSeamVerts[" + (sp - 1).ToString() + "][0] = new Vector3[" + numLods.ToString() + "][][];          //adult" + Environment.NewLine;
                        }
                        specifier = new string[numAgeGenders];
                        specifier[0] = "al";
                        specifierLong = new string[numAgeGenders];
                        specifierLong[0] = "Adult LittleDog";
                    }
                    else
                    {
                        numAgeGenders = 2;      //adult, child
                        if (codeOutput)
                        {
                            output += "  meshSeamVerts[" + (sp - 1).ToString() + "] = new Vector3[4][][][];        //ageSpecies" + Environment.NewLine +
                            "  meshSeamVerts[" + (sp - 1).ToString() + "][0] = new Vector3[" + numLods.ToString() + "][][];          //adult" + Environment.NewLine +
                            "  meshSeamVerts[" + (sp - 1).ToString() + "][1] = new Vector3[" + numLods.ToString() + "][][];          //child" + Environment.NewLine;
                        }
                        specifier = new string[numAgeGenders];
                        specifier[0] = "a" + ((XmodsEnums.Species)sp).ToString().Substring(0, 1).ToLower();
                        specifier[1] = "c" + ((XmodsEnums.Species)sp).ToString().Substring(0, 1).ToLower();
                        specifierLong = new string[numAgeGenders];
                        specifierLong[0] = "Adult " + ((XmodsEnums.Species)sp).ToString();
                        specifierLong[1] = ((XmodsEnums.Species)sp) == XmodsEnums.Species.Dog ? "Puppy" : "Kitten";
                    }
                    for (int ag = 0; ag < numAgeGenders; ag++)
                    {
                        if (codeOutput)
                        {
                            output += "meshSeamVerts[" + (sp - 1).ToString() + "][" + ag.ToString() + "] = new Vector3[" + numLods.ToString() + "][][];         //" + specifierLong[ag] + Environment.NewLine;
                        }
                        for (int l = 0; l < numLods; l++)
                        {
                            if (codeOutput)
                            {
                                output += "meshSeamVerts[" + (sp - 1).ToString() + "][" + ag.ToString() + "][" + l.ToString() + "] = new Vector3[" + numSeams.ToString() + "][];         //" + specifierLong[ag] + " LOD" + l.ToString() + " seams" + Environment.NewLine;
                            }
                            else
                            {
                                output += specifierLong[ag] + " LOD" + l.ToString() + ":" + Environment.NewLine;
                            }
                            GEOM[] meshes = new GEOM[2];
                            meshes[0] = new GEOM(new BinaryReader(new MemoryStream((byte[])rm.GetObject(specifier[ag] + "Body_lod" + l.ToString()))));
                            meshes[1] = new GEOM(new BinaryReader(new MemoryStream((byte[])rm.GetObject(specifier[ag] + "EarsUp_lod" + l.ToString()))));
                            for (int seam = 0; seam < numSeams; seam++)
                            {
                                string tmp = SeamString(meshes, seam);
                                if (codeOutput)
                                {
                                    output += "meshSeamVerts[" + (sp - 1).ToString() + "][" + ag.ToString() + "][" + l.ToString() + "][" + seam.ToString() + "] = ";
                                    output += (tmp.Length > 0 ? tmp : "new Vector3[0];");
                                }
                                else { output += tmp.Length > 0 ? ((GEOM.SeamType)seam).ToString() + ": " + tmp + Environment.NewLine : ""; }
                                if (codeOutput) output += "     //" + ((GEOM.SeamType)seam).ToString() + Environment.NewLine;
                            }
                        }
                        output += Environment.NewLine;
                    }
                }
            }

            SeamVertList.Text = output;
        }

        internal string SeamString(GEOM[] meshes, int type)
        {
            string tmp = "";
            SortedDictionary<int, Vector3> seam = new SortedDictionary<int, Vector3>();
            foreach (GEOM mesh in meshes)
            {
                foreach (GEOM.SeamStitch s in mesh.SeamStitches)
                {
                    if (s.SeamType == type)
                    {
                        try { seam.Add(s.SeamIndex, new Vector3(mesh.getPosition((int)s.Index))); }
                        catch { }
                    }
                }
            }
            if (seam.Count == 0) return "";
            if (ListSeamVertsCode_checkBox.Checked) tmp += "new Vector3[] { ";
            foreach (KeyValuePair<int, Vector3> p in seam)
            {
                if (ListSeamVertsCode_checkBox.Checked)
                {
                    tmp += "new Vector3(" + p.Value.X.ToString() + "f, " + p.Value.Y.ToString() + "f, " + p.Value.Z.ToString() + "f), ";
                }
                else
                {
                    tmp += p.ToString() + " ; ";
                }
            }
            if (ListSeamVertsCode_checkBox.Checked)
            {
                tmp = tmp.TrimEnd(new char[] { ' ', ',' });
                tmp += " };";
            }
            return tmp;
        }

        private void searchXMLfolder_button_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folder = new FolderBrowserDialog();
            folder.Description = "Select folder containing XML files to be searched";
            folder.ShowNewFolderButton = false;
            DialogResult res = folder.ShowDialog();
            if (res == DialogResult.OK) searchXMLfolder_textBox.Text = folder.SelectedPath;
        }

        private void searchXMLgo_button_Click(object sender, EventArgs e)
        {
            string output = "";
            string[] fileslist = Directory.GetFiles(searchXMLfolder_textBox.Text, "*.xml", SearchOption.AllDirectories);
            foreach (string f in fileslist)
            {
                string text = File.ReadAllText(f);
                if (text.IndexOf(searchXMLtext.Text, 0, text.Length, StringComparison.CurrentCultureIgnoreCase) >= 0)
                {
                    output += f + Environment.NewLine;
                }
            }
            searchXMLoutput.Text = output;
        }

        private void SMODGo_button_Click(object sender, EventArgs e)
        {
            List<string> modifierNames = new List<string>();
            List<TGI> modifierTGIs = new List<TGI>();
            List<string> dmapNames = new List<string>();
            List<TGI> dmapTGIs = new List<TGI>();
            List<ulong> modifierInstances = new List<ulong>();

            ParseTextResourcesList("ModifierList.txt", out modifierTGIs, out modifierNames);
            ParseTextResourcesList("DMapList.txt", out dmapTGIs, out dmapNames);

            CNTXsearch_dataGridView.Rows.Clear();
            Predicate<IResourceIndexEntry> pred = r => r.ResourceType == 0xC5F6763EU;
            TGI zero = new TGI();
            DataGridViewCellStyle wrapStyle = new DataGridViewCellStyle();
            wrapStyle.WrapMode = DataGridViewTriState.True;
            foreach (Package p in gamePacks0)
            {
                List<IResourceIndexEntry> iries = p.FindAll(pred);
                foreach (IResourceIndexEntry irie in iries)
                {
                    if (modifierInstances.IndexOf(irie.Instance) >= 0) continue;
                    using (BinaryReader br = new BinaryReader(p.GetResource(irie)))
                    {
                        SMOD smod = new SMOD(br);
                        string[] str = new string[4];
                        str[0] = irie.Instance.ToString("X16");
                        int tmp = modifierTGIs.IndexOf(new TGI(irie.ResourceType, irie.ResourceGroup, irie.Instance));
                        if (tmp >= 0) str[0] += " : " + modifierNames[tmp];
                        str[1] = "";
                        for (int i = 0; i < smod.delayLoadKey.Length; i++)
                        {
                            str[1] += smod.delayLoadKey[i].Instance.ToString("X16") + (i < smod.delayLoadKey.Length - 1 ? Environment.NewLine : "");
                        }
                        str[2] = smod.bonePoseKey.Instance.ToString("X16");
                        bool doubleLine = false;
                        if (!smod.deformerMapShapeKey.Equals(zero) || !smod.deformerMapNormalKey.Equals(zero))
                        {
                            str[3] = smod.deformerMapShapeKey.Instance.ToString("X16");
                            if (dmapTGIs.IndexOf(smod.deformerMapShapeKey) >= 0)
                                str[3] += " : " + dmapNames[dmapTGIs.IndexOf(smod.deformerMapShapeKey)];
                            str[3] += Environment.NewLine + smod.deformerMapNormalKey.Instance.ToString("X16");
                            if (dmapTGIs.IndexOf(smod.deformerMapNormalKey) >= 0)
                                str[3] += " : " + dmapNames[dmapTGIs.IndexOf(smod.deformerMapNormalKey)];
                            doubleLine = true;
                        }
                        int ind = CNTXsearch_dataGridView.Rows.Add(str);
                        if (doubleLine)
                        {
                            CNTXsearch_dataGridView.Rows[ind].Height = CNTXsearch_dataGridView.Rows[ind].Height * 2;
                            CNTXsearch_dataGridView.Rows[ind].Cells[3].Style = wrapStyle;
                        }
                        modifierInstances.Add(irie.Instance);
                    }
                }
            }
        }

        /// <summary>
        /// Reads text list of TGI and name, ex: c5f6763e:00000000:8088d1e4b2013f98   yfheadNose_TipWide
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="morphTGIs"></param>
        /// <param name="morphNames"></param>
        private void ParseTextResourcesList(string filename, out List<TGI> tgiList, out List<string> nameList)
        {
            string executingPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string resourcePath = Path.Combine(executingPath, filename);
            if (!File.Exists(resourcePath))
            {
                MessageBox.Show(string.Format("'{0}' not found in CAS Tools directory '{1}'; TGI and resource name list cannot be loaded.", filename, executingPath));
                tgiList = null;
                nameList = null;
                return;
            }
            string line = "";
            System.IO.StreamReader file = new System.IO.StreamReader(resourcePath);
            List<TGI> tgiTmp = new List<TGI>();
            List<string> nameTmp = new List<string>();
            while ((line = file.ReadLine()) != null)
            {
                string[] s = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                tgiTmp.Add(new TGI(s[0]));
                nameTmp.Add(s[1]);
            }
            file.Close();
            tgiList = tgiTmp;
            nameList = nameTmp;
            return;
        }

        //private void ClipEventGo_button_Click(object sender, EventArgs e)
        //{
        //    ClipEvent_dataGridView.Rows.Clear();
        //    Predicate<IResourceIndexEntry> pred = r => r.ResourceType == 0x6B20C4F3U;
        //    TGI zero = new TGI();
        //    foreach (Package p in gamePacks0)
        //    {
        //        List<IResourceIndexEntry> iries = p.FindAll(pred);
        //        foreach (IResourceIndexEntry irie in iries)
        //        {
        //            using (Stream ms = p.GetResource(irie))
        //            {
        //                s4pi.Animation.ClipResource clip = new s4pi.Animation.ClipResource(0, ms);
        //                foreach (s4pi.Animation.ClipEvent evt in clip.ClipEvents)
        //                {
        //                    if ((uint)evt.TypeId == ClipEventType_comboBox.SelectedIndex)
        //                    {
        //                        string[] str = new string[1];
        //                        str[0] = irie.Instance.ToString("X16");
        //                        int ind = ClipEvent_dataGridView.Rows.Add(str);
        //                    }
        //                    break;
        //                }
        //            }
        //        }
        //    }
        //}
        //private void tabPage8_Enter(object sender, EventArgs e)
        //{
        //    ClipEventType_comboBox.Items.AddRange(Enum.GetNames(typeof(ClipEventType)));
        //}
        //public enum ClipEventType : uint
        //{
        //    INVALID = 0,
        //    PARENT,
        //    UNPARENT,
        //    SOUND,
        //    SCRIPT,
        //    EFFECT,
        //    VISIBILITY,
        //    DEPRECATED_6,
        //    CREATE_PROP,
        //    DESTROY_PROP,
        //    STOP_EFFECT,
        //    BLOCK_TRANSITION,
        //    SNAP,
        //    REACTION,
        //    DOUBLE_MODIFIER_SOUND,
        //    DSP_INTERVAL,
        //    MATERIAL_STATE,
        //    FOCUS_COMPATIBILITY,
        //    SUPPRESS_LIP_SYNC,
        //    CENSOR,
        //    SIMULATION_SOUND_START,
        //    SIMULATION_SOUND_STOP,
        //    ENABLE_FACIAL_OVERLAY,
        //    FADE_OBJECT,
        //    DISABLE_OBJECT_HIGHLIGHT,
        //    THIGH_TARGET_OFFSET,
        //    UNKNOWN26,
        //    UNKNOWN27,
        //    UNKNOWN28,
        //    UNKNOWN29,
        //    UNKNOWN30,
        //    UNKNOWN31,
        //    UNKNOWN32,
        //    UNKNOWN33,
        //    UNKNOWN34,
        //    UNKNOWN35

        //}

        private void CalcSlotrayFile1_button_Click(object sender, EventArgs e)
        {
            CalcSlotrayFile1.Text = GetFilename("Select TS4 geom", GEOMfilter);
        }

        private void CalcSlotrayFile2_button_Click(object sender, EventArgs e)
        {
            CalcSlotrayFile2.Text = GetFilename("Select TS4 geom", GEOMfilter);
        }

        private void CalcSlotrayFile3_button_Click(object sender, EventArgs e)
        {
            CalcSlotrayFile3.Text = GetFilename("Select TS4 geom", GEOMfilter);
        }

        //private void CalcSlotrayGo_button_Click(object sender, EventArgs e)
        //{
        //    GEOM geom1, geom2, geom3;
        //    if (!GetGEOMData(CalcSlotrayFile1.Text, out geom1) || !GetGEOMData(CalcSlotrayFile2.Text, out geom2) || !GetGEOMData(CalcSlotrayFile3.Text, out geom3))
        //    {
        //        MessageBox.Show("Can't read at least one of the input files!");
        //        return;
        //    }
        //    string slotInfo = "";
        //    foreach (GEOM.SlotrayIntersection s in geom1.SlotrayAdjustments)
        //    {
        //        Vector3[] origins1 = GetRayOriginPoints(geom1, s);
        //        Vector3[] origins2 = null;
        //        Vector3[] origins3 = null;
        //        Vector3 origin = new Vector3();
        //        foreach (GEOM.SlotrayIntersection s2 in geom2.SlotrayAdjustments)
        //        {
        //            if (s.SlotBone == s2.SlotBone)
        //            {
        //                origins2 = GetRayOriginPoints(geom2, s2);
        //                continue;
        //            }
        //        }
        //        foreach (GEOM.SlotrayIntersection s3 in geom3.SlotrayAdjustments)
        //        {
        //            if (s.SlotBone == s3.SlotBone)
        //            {
        //                origins3 = GetRayOriginPoints(geom3, s3);
        //                continue;
        //            }
        //        }
        //        if (origins2 == null || origins3 == null)
        //        {
        //            slotInfo += "0x" + s.SlotBone.ToString("X8") + " *not determined*" + Environment.NewLine;
        //            continue;
        //        }
        //        float maxSep = 0.0001f;
        //        if ((origins1[0].CloseTo(origins2[0], maxSep) || origins1[0].CloseTo(origins2[1], maxSep)) &&
        //            (origins1[0].CloseTo(origins3[0], maxSep) || origins1[0].CloseTo(origins3[1], maxSep)))
        //        {
        //            origin = origins1[0];
        //        }
        //        else if ((origins1[1].CloseTo(origins2[0], maxSep) || origins1[1].CloseTo(origins2[1], maxSep)) &&
        //                 (origins1[1].CloseTo(origins3[0], maxSep) || origins1[1].CloseTo(origins3[1], maxSep)))
        //        {
        //            origin = origins1[1];
        //        }
        //        slotInfo += "0x" + s.SlotBone.ToString("X8") + '\t' + s.SlotAveragePosOS.ToString("G7") + '\t' +
        //            origin.ToString("G7") + '\t' + (s.PivotBoneHash > 0 ? "0x" + s.PivotBoneHash.ToString("X8") : "") + Environment.NewLine;
        //    }
        //    CalcSlotrayResults.Text = slotInfo;
        //}

        private void CalcSlotrayGo_button_Click(object sender, EventArgs e)
        {
            GEOM geom, geom2, geom3;
            if (!GetGEOMData(CalcSlotrayFile1.Text, out geom) || !GetGEOMData(CalcSlotrayFile2.Text, out geom2) || !GetGEOMData(CalcSlotrayFile3.Text, out geom3))
            {
                MessageBox.Show("Can't read at least one of the input files!");
                return;
            }
            string slotInfo = "";
            RIG rig = SelectRig(XmodsEnums.Species.Human, XmodsEnums.Age.Adult);
            foreach (GEOM.SlotrayIntersection s in geom.SlotrayAdjustments)
            {
                Vector3[] origins = new Vector3[] { new Vector3(), new Vector3(), new Vector3() };
                origins[0] = GetRayOrigin(geom, s);
                if (geom2 != null)
                {
                    foreach (GEOM.SlotrayIntersection s2 in geom2.SlotrayAdjustments)
                    {
                        if (s.SlotBone == s2.SlotBone)
                        {
                            origins[1] = GetRayOrigin(geom2, s2);
                            continue;
                        }
                    }
                }
                if (geom3 != null)
                {
                    foreach (GEOM.SlotrayIntersection s3 in geom3.SlotrayAdjustments)
                    {
                        if (s.SlotBone == s3.SlotBone)
                        {
                            origins[2] = GetRayOrigin(geom3, s3);
                            continue;
                        }
                    }
                }

                Vector3 origin = new Vector3();
                int ocount = 0;
                for (int i = 0; i < 3; i++)
                {
                    if (origins[i] != new Vector3())
                    {
                        origin += origins[i];
                        ocount += 1;
                    }
                }

                origin = origin / ocount;

                //RIG.Bone bone = rig.GetBone(s.SlotBone);
                //RIG.Bone parent = bone.ParentBone;
                //Vector3 parentPos = parent.WorldPosition;
                
                slotInfo += "0x" + s.SlotBone.ToString("X8") + '\t' + s.SlotAveragePosOS.ToString("G7") + '\t' +
                    origin.ToString("G7") + '\t' + (s.PivotBoneHash > 0 ? "0x" + s.PivotBoneHash.ToString("X8") : "") + Environment.NewLine;
            }
            CalcSlotrayResults.Text = slotInfo;
        }

        private void CalcSlotrayClipboard_button_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(CalcSlotrayResults.Text);
        }

        private Vector3 GetRayOrigin(GEOM geom, GEOM.SlotrayIntersection s)
        {
            Vector3 slotPosition = s.SlotAveragePosOS;
            Triangle tri = new Triangle(geom.getPosition(s.TrianglePointIndices[0]), geom.getPosition(s.TrianglePointIndices[1]), geom.getPosition(s.TrianglePointIndices[2]));
            Vector3 intersectPos = tri.WorldCoordinates(s.Coordinates);
            Vector3 ray = intersectPos - slotPosition;
            ray.Normalize();
            ray = ray * s.Distance;
            Vector3 origin;
            if (s.OffsetFromIntersectionOS == new Vector3())
            {
                origin = intersectPos - ray;
            }
            else
            {
                origin = intersectPos + ray;
            }
            return origin;
        }

        private Vector3[] GetRayOriginPoints(GEOM geom, GEOM.SlotrayIntersection slot)
        {
            Triangle tri = new Triangle(geom.getPosition(slot.TrianglePointIndices[0]), geom.getPosition(slot.TrianglePointIndices[1]), geom.getPosition(slot.TrianglePointIndices[2]));
            Vector3 intersection = tri.WorldCoordinates(slot.Coordinates);

            Vector3 rayDirection = intersection - slot.SlotAveragePosOS;
            rayDirection.Normalize();
            Vector3 origin_AvgPosInside = intersection - (slot.Distance * rayDirection);
            Vector3 origin_AvgPosOutside = intersection + (slot.Distance * rayDirection);
            return new Vector3[] { origin_AvgPosInside, origin_AvgPosOutside };
        }

        private void ProtobufDesc_button_Click(object sender, EventArgs e)
        {
            ProtobufDescFile.Text = GetFilename("Select python source .py file", PythonSourceFilter);
            if (!File.Exists(ProtobufDescFile.Text))
            {
                MessageBox.Show("You have not selected a valid file!");
                return;
            }
            else if (!(String.Compare(Path.GetExtension(ProtobufDescFile.Text).ToLower(), ".py") == 0))
            {
                MessageBox.Show("You have not selected a Python .py file!");
                return;
            }

            List<ProtoEnum> enumList = new List<ProtoEnum>();
            List<ProtoClass> classList = new List<ProtoClass>();
            List<string> fieldsList = new List<string>();

            ReadPythonFile(ProtobufDescFile.Text, enumList, classList, fieldsList);

            UpdateClasses(enumList, classList, fieldsList);

            string output = "";
            foreach (ProtoEnum n in enumList)
            {
                output += "public enum " + n.enumName + Environment.NewLine + "{" + Environment.NewLine;
                for (int i = 0; i < n.valueNames.Length; i++)
                {
                    output += "\t" + n.valueNames[i] + " = " + n.valueNumbers[i] + "," + Environment.NewLine;
                }
                output += "}" + Environment.NewLine + Environment.NewLine;
            }
            foreach (ProtoClass p in classList)
            {
                output += "[ProtoContract]" + Environment.NewLine;
                output += "public class " + p.className + Environment.NewLine + "{" + Environment.NewLine;
                for (int i = 0; i < p.fieldNames.Length; i++)
                {
                    output += "\t[ProtoMember(" + p.fieldNumbers[i] + p.fieldFormats[i] + ")]" + Environment.NewLine;
                    output += "\tpublic " + p.fieldTypes[i] + (p.fieldRepeat[i] ? "[] " : " ") + p.fieldNames[i] + ";" + Environment.NewLine;
                }
                output += "}" + Environment.NewLine + Environment.NewLine;
            }
            ProtobufDescOutput.Text = output;
        }

        private class ProtoClass
        {
            internal string classLabel;
            internal string className;
            internal string[] fieldNames;
            internal string[] fieldTypes;
            internal string[] fieldFormats;
            internal string[] fieldNumbers;
            internal bool[] fieldRepeat;
            internal bool[] fieldIsReference;
            internal ProtoClass(string label, string name, string[] fields, string[] types, string[] formats, string[] numbers, bool[] repeat, bool[] isReference)
            {
                this.classLabel = label;
                this.className = name;
                this.fieldNames = fields;
                this.fieldTypes = types;
                this.fieldFormats = formats;
                this.fieldNumbers = numbers;
                this.fieldRepeat = repeat;
                this.fieldIsReference = isReference;
            }
            internal void setFieldType(string field, string type)
            {
                for (int i = 0; i < this.fieldNames.Length; i++)
                {
                    if (String.Compare(field, this.fieldNames[i]) == 0)
                    {
                        this.fieldTypes[i] = type;
                        break;
                    }
                }
            }
        }

        private class ProtoEnum
        {
            internal string enumLabel;
            internal string enumName;
            internal string[] valueNames;
            internal string[] valueNumbers;
            internal ProtoEnum(string label, string name, string[] valNames, string[] valNumbers)
            {
                this.enumLabel = label;
                this.enumName = name;
                this.valueNames = valNames;
                this.valueNumbers = valNumbers;
            }
        }

        private void ReadPythonFile(string filename, List<ProtoEnum> enumList, List<ProtoClass> classList, List<string> fieldsList)
        {
            using (StreamReader file = new System.IO.StreamReader(filename))
            {
                string line;
                while ((line = file.ReadLine()) != null)
                {
                    int index;
                    if ((index = line.IndexOf("descriptor.EnumDescriptor")) >= 0)
                    {
                        string enumLabel = line.Substring(0, line.IndexOf(" = "));
                        int s1 = line.IndexOf("name='", index) + 6;
                        int s2 = line.IndexOf("'", s1);
                        string enumName = line.Substring(s1, s2 - s1);
                        index = s2 + 1;
                        List<string> valueNames = new List<string>();
                        List<string> valueNumbers = new List<string>();
                        while ((index = line.IndexOf("descriptor.EnumValueDescriptor", index)) >= 0)
                        {
                            s1 = line.IndexOf("name='", index) + 6;
                            s2 = line.IndexOf("'", s1);
                            index = s2 + 1;
                            string valName = line.Substring(s1, s2 - s1);
                            s1 = line.IndexOf("number=", index) + 7;
                            s2 = line.IndexOf(",", s1);
                            index = s2 + 1;
                            string valNumber = line.Substring(s1, s2 - s1);
                            valueNames.Add(valName);
                            valueNumbers.Add(valNumber);
                        }
                        enumList.Add(new ProtoEnum(enumLabel, enumName, valueNames.ToArray(), valueNumbers.ToArray()));
                    }
                    if ((index = line.IndexOf("descriptor.Descriptor")) >= 0)
                    {
                        string classLabel = line.Substring(0, line.IndexOf(" = "));
                        int s1 = line.IndexOf("name='", index) + 6;
                        int s2 = line.IndexOf("'", s1);
                        string className = line.Substring(s1, s2 - s1);
                        index = s2 + 1;
                        List<string> fieldNames = new List<string>();
                        List<string> fieldTypes = new List<string>();
                        List<string> fieldFormats = new List<string>();
                        List<string> fieldNumbers = new List<string>();
                        List<bool> fieldRepeats = new List<bool>();
                        List<bool> fieldRefs = new List<bool>();
                        while ((index = line.IndexOf("descriptor.FieldDescriptor", index)) >= 0)
                        {
                            s1 = line.IndexOf("name='", index) + 6;
                            s2 = line.IndexOf("'", s1);
                            index = s2 + 1;
                            string fieldName = line.Substring(s1, s2 - s1);
                            s1 = line.IndexOf("number=", index) + 7;
                            s2 = line.IndexOf(",", s1);
                            index = s2 + 1;
                            string number = line.Substring(s1, s2 - s1);
                            s1 = line.IndexOf("type=", index) + 5;
                            s2 = line.IndexOf(",", s1);
                            index = s2 + 1;
                            string type = line.Substring(s1, s2 - s1);
                            int itype = int.Parse(type);
                            s1 = line.IndexOf("label=", index) + 6;
                            s2 = line.IndexOf(",", s1);
                            index = s2 + 1;
                            bool repeat = String.Compare(line.Substring(s1, s2 - s1), "3") == 0 || itype == 12;
                            fieldNames.Add(fieldName);
                            fieldTypes.Add(Enum.GetName(typeof(Protobuf_Types), itype).Replace("TYPE_", "").ToLower());
                            fieldFormats.Add(GetProtoDataFormat((Protobuf_Types)itype));
                            fieldNumbers.Add(number);
                            fieldRepeats.Add(repeat);
                            fieldRefs.Add(itype == 11 || itype == 14);
                        }
                        classList.Add(new ProtoClass(classLabel, className, fieldNames.ToArray(), fieldTypes.ToArray(), fieldFormats.ToArray(),
                            fieldNumbers.ToArray(), fieldRepeats.ToArray(), fieldRefs.ToArray()));
                    }
                    if ((index = line.IndexOf(".fields_by_name")) >= 0)
                    {
                        fieldsList.Add(line);
                    }
                }
            }
        }

        private void UpdateClasses(List<ProtoEnum> enumList, List<ProtoClass> classList, List<string> fieldsList)
        {
            foreach (string s in fieldsList)
            {
                int index = s.IndexOf(".fields_by_name");
                string classLabel = s.Substring(0, index);
                int s1 = s.IndexOf("['", index) + 2;
                int s2 = s.IndexOf("'", s1);
                string fieldName = s.Substring(s1, s2 - s1);
                index = s2 + 1;
                s1 = s.IndexOf(" = ", index) + 3;
                s2 = s.IndexOf(".", s1);
                if (s2 > 0) s1 = s2 + 1;
                string fieldLabel = s.Substring(s1);
                string fieldType = fieldLabel;
                for (int i = 0; i < classList.Count; i++)
                {
                    if (String.Compare(classList[i].classLabel, fieldLabel) == 0)
                    {
                        fieldType = classList[i].className;
                        break;
                    }
                }
                for (int i = 0; i < enumList.Count; i++)
                {
                    if (String.Compare(enumList[i].enumLabel, fieldLabel) == 0)
                    {
                        fieldType = enumList[i].enumName;
                        break;
                    }
                }
                for (int i = 0; i < classList.Count; i++)
                {
                    if (String.Compare(classList[i].classLabel, classLabel) == 0)
                    {
                        classList[i].setFieldType(fieldName, fieldType);
                        break;
                    }
                }
            }
        }

        private void ProtobufSelectFolder_button_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folder = new FolderBrowserDialog();
            folder.Description = "Select folder containing Python source files to be searched or extracted from";
            folder.ShowNewFolderButton = false;
            DialogResult res = folder.ShowDialog();
            if (res == DialogResult.OK)
            {
                enumFolderList = new List<ProtoEnum>();
                classFolderList = new List<ProtoClass>();
                fieldsFolderList = new List<string>();
                ProtobufSearchFolder.Text = folder.SelectedPath;
                string[] fileslist = Directory.GetFiles(ProtobufSearchFolder.Text, "*.py", SearchOption.AllDirectories);
                foreach (string f in fileslist)
                {
                    ReadPythonFile(f, enumFolderList, classFolderList, fieldsFolderList);
                }
                UpdateClasses(enumFolderList, classFolderList, fieldsFolderList);
                ProtobufExtractClass_comboBox.Items.Clear();
                List<string> classNames = new List<string>();
                foreach (ProtoClass c in classFolderList)
                {
                    classNames.Add(c.className);
                }
                classNames.Sort();
                ProtobufExtractClass_comboBox.Items.AddRange(classNames.ToArray());
                if (ProtobufExtractClass_comboBox.Items.Count > 0) ProtobufExtractClass_comboBox.SelectedIndex = 0;
            }
        }

        private void ProtobufSearchGo_button_Click(object sender, EventArgs e)
        {
            string output = "";
            string[] fileslist = Directory.GetFiles(ProtobufSearchFolder.Text, "*.py", SearchOption.AllDirectories);
            foreach (string f in fileslist)
            {
                string text = File.ReadAllText(f);
                if (text.IndexOf(ProtobufSearchString.Text, 0, text.Length, StringComparison.CurrentCultureIgnoreCase) >= 0)
                {
                    output += f + Environment.NewLine;
                }
            }
            ProtobufDescOutput.Text = output;
        }


        private void ProtobufExtractClass_button_Click(object sender, EventArgs e)
        {
            List<string> done = new List<string>();
            string output = ProtobufClassExtractor((string)ProtobufExtractClass_comboBox.SelectedItem, enumFolderList, classFolderList, done);
            ProtobufDescOutput.Text = output;
        }

        private string ProtobufClassExtractor(string name, List<ProtoEnum> enumList, List<ProtoClass> classList, List<string> doneList)
        {
            foreach (string s in doneList)
            {
                if (String.Compare(name, s) == 0) return "";
            }
            string output = "";

            foreach (ProtoClass p in classList)
            {
                if (String.Compare(name, p.className) == 0)
                {
                    output += "[ProtoContract]" + Environment.NewLine;
                    output += "public class " + p.className + Environment.NewLine + "{" + Environment.NewLine;
                    for (int i = 0; i < p.fieldNames.Length; i++)
                    {
                        if (String.Compare("message", p.fieldTypes[i]) == 0) continue;
                        output += "\t[ProtoMember(" + p.fieldNumbers[i] + p.fieldFormats[i] + ")]" + Environment.NewLine;
                        output += "\tpublic " + p.fieldTypes[i] + (p.fieldRepeat[i] ? "[] " : " ") + p.fieldNames[i] + ";" + Environment.NewLine;
                    }
                    output += "}" + Environment.NewLine + Environment.NewLine;

                    doneList.Add(name);

                    for (int i = 0; i < p.fieldIsReference.Length; i++)
                    {
                        if (p.fieldIsReference[i]) output += ProtobufClassExtractor(p.fieldTypes[i], enumList, classList, doneList);
                    }
                    break;
                }
            }

            foreach (ProtoEnum n in enumList)
            {
                if (String.Compare(name, n.enumName) == 0)
                {
                    output += "public enum " + n.enumName + Environment.NewLine + "{" + Environment.NewLine;
                    for (int i = 0; i < n.valueNames.Length; i++)
                    {
                        output += "\t" + n.valueNames[i] + " = " + n.valueNumbers[i] + "," + Environment.NewLine;
                    }
                    output += "}" + Environment.NewLine + Environment.NewLine;
                    doneList.Add(name);
                    break;
                }
            }

            return output;
        }

        private void ProtobufDescCopy_button_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(ProtobufDescOutput.Text);
        }

        enum Protobuf_Types
        {
            TYPE_DOUBLE = 1,
            TYPE_FLOAT = 2,
            TYPE_LONG = 3,      //INT64
            TYPE_ULONG = 4,     //UINT64
            TYPE_INT = 5,       //INT32
            TYPE_ulong = 6,      //FIXED64
            TYPE_uint = 7,       //FIXED32
            TYPE_BOOL = 8,
            TYPE_STRING = 9,
            TYPE_GROUP = 10,
            TYPE_MESSAGE = 11,
            TYPE_BYTE = 12,     //BYTES
            TYPE_UINT = 13,     //UINT32
            TYPE_ENUM = 14,
            TYPE_int = 15,      //SFIXED32
            TYPE_long = 16,     //SFIXED64
            TYPE_Int = 17,      //SINT32
            TYPE_Long = 18      //SINT64
        }

        private string GetProtoDataFormat(Protobuf_Types protoType)
        {
            if (protoType == Protobuf_Types.TYPE_ulong || protoType == Protobuf_Types.TYPE_uint || protoType == Protobuf_Types.TYPE_int || protoType == Protobuf_Types.TYPE_long)
            {
                return ", DataFormat = DataFormat.FixedSize";
            }
            else if (protoType == Protobuf_Types.TYPE_Int || protoType == Protobuf_Types.TYPE_Long)
            {
                return ", DataFormat = DataFormat.ZigZag";
            }
            return "";
        }

        private void SculptTextures_button_Click(object sender, EventArgs e)
        {
            Package newpack = (Package)Package.NewPackage(1);
            Predicate<IResourceIndexEntry> pred = r => r.ResourceType == 0x9D1AB874;
            foreach (Package p in gamePacks0)
            {
                List<IResourceIndexEntry> iries = p.FindAll(pred);
                foreach (IResourceIndexEntry irie in iries)
                {
                    using (BinaryReader br = new BinaryReader(p.GetResource(irie)))
                    {
                        try
                        {
                            Sculpt sculpt = new Sculpt(br);
                            if (sculpt.region != XmodsEnums.SimRegion.Eyes) continue;
                            TGI link = sculpt.textureRef;
                            IResourceIndexEntry key;
                            bool dummy;
                            RLEResource rle = FindCloneTextureRLE(link, out key, out dummy);
                            if (rle != null && !dummy) newpack.AddResource(key, rle.Stream, true);
                        }
                        catch { }
                    }
                }
            }
            WritePackage("Save sculpt textures package", newpack, "SculptTextures.package");
        }

        public class Sculpt
        {
            public uint contextVersion;
            public TGI[] publicKey;
            public TGI[] externalKey;
            public TGI[] BGEOKey;
            public ObjectData[] objectKey;

            private uint version;
            public XmodsEnums.AgeGender ageGender;
            public XmodsEnums.SimRegion region;
            public SimSubRegion subRegion;
            public BgeoLinkTag linkTag;
            public TGI textureRef;
            public TGI specularRef;
            public TGI bumpmapRef;
            private byte unknown7;
            public TGI dmapShapeRef;
            public TGI dmapNormalRef;
            public TGI boneDeltaRef;
            private uint unknown8;

            public Sculpt(BinaryReader br)
            {
                this.contextVersion = br.ReadUInt32();
                uint publicKeyCount = br.ReadUInt32();
                uint externalKeyCount = br.ReadUInt32();
                uint delayLoadKeyCount = br.ReadUInt32();
                uint objectKeyCount = br.ReadUInt32();
                this.publicKey = new TGI[publicKeyCount];
                for (int i = 0; i < publicKeyCount; i++) publicKey[i] = new TGI(br, TGI.TGIsequence.ITG);
                this.externalKey = new TGI[externalKeyCount];
                for (int i = 0; i < externalKeyCount; i++) externalKey[i] = new TGI(br, TGI.TGIsequence.ITG);
                this.BGEOKey = new TGI[delayLoadKeyCount];
                for (int i = 0; i < delayLoadKeyCount; i++) BGEOKey[i] = new TGI(br, TGI.TGIsequence.ITG);
                this.objectKey = new ObjectData[objectKeyCount];
                for (int i = 0; i < objectKeyCount; i++) objectKey[i] = new ObjectData(br);
                this.version = br.ReadUInt32();
                this.ageGender = (XmodsEnums.AgeGender)br.ReadUInt32();
                this.region = (XmodsEnums.SimRegion)br.ReadUInt32();
                this.subRegion = (SimSubRegion)br.ReadUInt32();
                this.linkTag = (BgeoLinkTag)br.ReadUInt32();
                this.textureRef = new TGI(br, TGI.TGIsequence.ITG);
                this.specularRef = new TGI(br, TGI.TGIsequence.ITG);
                this.bumpmapRef = new TGI(br, TGI.TGIsequence.ITG);
                this.unknown7 = br.ReadByte();
                this.dmapShapeRef = new TGI(br, TGI.TGIsequence.ITG);
                this.dmapNormalRef = new TGI(br, TGI.TGIsequence.ITG);
                this.boneDeltaRef = new TGI(br, TGI.TGIsequence.ITG);
                this.unknown8 = br.ReadUInt32();
            }

            public class ObjectData
            {
                internal uint position;
                internal uint length;

                internal ObjectData(BinaryReader br)
                {
                    this.position = br.ReadUInt32();
                    this.length = br.ReadUInt32();
                }
            }

            public enum SimSubRegion
            {
                None = 0,
                EarsUp = 1,
                EarsDown = 2,
                TailLong = 3,
                TailRing = 4,
                TailScrew = 5,
                TailStub = 6
            }

            public enum BgeoLinkTag : uint
            {
                NoBGEO = 0,
                UseBGEO = 0x30000001U
            }
        }

        private void SkincolorTextures_button_Click(object sender, EventArgs e)
        {
            Package newpack = (Package)Package.NewPackage(1);
            Package newpack2 = (Package)Package.NewPackage(1);
            Package newpack3 = (Package)Package.NewPackage(1);
            Predicate<IResourceIndexEntry> pred = r => r.ResourceType == (uint)XmodsEnums.ResourceTypes.TONE;
            foreach (Package p in gamePacks0)
            {
                List<IResourceIndexEntry> iries = p.FindAll(pred);
                foreach (IResourceIndexEntry irie in iries)
                {
                    using (BinaryReader br = new BinaryReader(p.GetResource(irie)))
                    {
                        try
                        {
                            TONE tone = new TONE(br);
                            for (int i = 0; i < tone.SkinSets.Length; i++)
                            {
                                Predicate<IResourceIndexEntry> predd = r => r.ResourceType == (uint)XmodsEnums.ResourceTypes.DDSuncompressed &&
                                    r.ResourceGroup == 0 && r.Instance == tone.GetSkinSetTextureInstance(i);
                                IResourceIndexEntry ir = p.Find(predd);
                                IResourceIndexEntry ir2 = newpack.Find(predd);
                                if (ir != null && ir2 == null)
                                {
                                    TGIBlock tgi = new TGIBlock(1, null, ir.ResourceType, ir.ResourceGroup, ir.Instance);
                                    IResourceIndexEntry ire = newpack.AddResource(tgi, p.GetResource(ir), true);
                                    ire.Compressed = 0x5A42;
                                }
                                Predicate<IResourceIndexEntry> predl = r => r.ResourceType == (uint)XmodsEnums.ResourceTypes.LRLE &&
                                    r.ResourceGroup == 0 && r.Instance == tone.GetSkinSetTextureInstance(i);
                                IResourceIndexEntry irl = p.Find(predl);
                                IResourceIndexEntry irl2 = newpack2.Find(predl);
                                if (irl != null && irl2 == null)
                                {
                                    TGIBlock tgi = new TGIBlock(1, null, irl.ResourceType, irl.ResourceGroup, irl.Instance);
                                    IResourceIndexEntry irel = newpack2.AddResource(tgi, p.GetResource(irl), true);
                                    irel.Compressed = 0x5A42;
                                }
                                if (tone.GetSkinSetOverlayInstance(i) != 0)
                                {
                                    Predicate<IResourceIndexEntry> predmd = r => r.ResourceType == (uint)XmodsEnums.ResourceTypes.DDSuncompressed &&
                                        r.ResourceGroup == 0 && r.Instance == tone.GetSkinSetOverlayInstance(i);
                                    IResourceIndexEntry irmd = p.Find(predmd);
                                    IResourceIndexEntry irmd2 = newpack3.Find(predmd);
                                    if (irmd != null && irmd2 == null)
                                    {
                                        TGIBlock tgi = new TGIBlock(1, null, irmd.ResourceType, irmd.ResourceGroup, irmd.Instance);
                                        IResourceIndexEntry iremd = newpack3.AddResource(tgi, p.GetResource(irmd), true);
                                        iremd.Compressed = 0x5A42;
                                    }
                                    Predicate<IResourceIndexEntry> predm2 = r => r.ResourceType == (uint)XmodsEnums.ResourceTypes.DXT5RLE2 &&
                                        r.ResourceGroup == 0 && r.Instance == tone.GetSkinSetOverlayInstance(i);
                                    IResourceIndexEntry irm2 = p.Find(predm2);
                                    IResourceIndexEntry irm22 = newpack3.Find(predm2);
                                    if (irm2 != null && irm22 == null)
                                    {
                                        TGIBlock tgi = new TGIBlock(1, null, irm2.ResourceType, irm2.ResourceGroup, irm2.Instance);
                                        IResourceIndexEntry irem2 = newpack3.AddResource(tgi, p.GetResource(irm2), true);
                                        irem2.Compressed = 0x5A42;
                                    }
                                }

                                foreach (Package pt in gamePacksOther)
                                {
                                    IResourceIndexEntry irt = pt.Find(predd);
                                    IResourceIndexEntry irt2 = newpack.Find(predd);
                                    if (irt != null && irt2 == null)
                                    {
                                        TGIBlock tgi = new TGIBlock(1, null, irt.ResourceType, irt.ResourceGroup, irt.Instance);
                                        IResourceIndexEntry ire = newpack.AddResource(tgi, pt.GetResource(irt), true);
                                        ire.Compressed = 0x5A42;
                                    }
                                    IResourceIndexEntry irtl = pt.Find(predl);
                                    IResourceIndexEntry irtl2 = newpack2.Find(predl);
                                    if (irtl != null && irtl2 == null)
                                    {
                                        TGIBlock tgi = new TGIBlock(1, null, irtl.ResourceType, irtl.ResourceGroup, irtl.Instance);
                                        IResourceIndexEntry irel = newpack2.AddResource(tgi, pt.GetResource(irtl), true);
                                        irel.Compressed = 0x5A42;
                                    }
                                    if (tone.GetSkinSetOverlayInstance(i) != 0)
                                    {
                                        Predicate<IResourceIndexEntry> predmd = r => r.ResourceType == (uint)XmodsEnums.ResourceTypes.DDSuncompressed &&
                                            r.ResourceGroup == 0 && r.Instance == tone.GetSkinSetOverlayInstance(i);
                                        IResourceIndexEntry irtmd = pt.Find(predmd);
                                        IResourceIndexEntry irtmd2 = newpack3.Find(predmd);
                                        if (irtmd != null && irtmd2 == null)
                                        {
                                            TGIBlock tgi = new TGIBlock(1, null, irtmd.ResourceType, irtmd.ResourceGroup, irtmd.Instance);
                                            IResourceIndexEntry irtemd = newpack3.AddResource(tgi, pt.GetResource(irtmd), true);
                                            irtemd.Compressed = 0x5A42;
                                        }
                                        Predicate<IResourceIndexEntry> predm2 = r => r.ResourceType == (uint)XmodsEnums.ResourceTypes.DXT5RLE2 &&
                                            r.ResourceGroup == 0 && r.Instance == tone.GetSkinSetOverlayInstance(i);
                                        IResourceIndexEntry irtm2 = pt.Find(predm2);
                                        IResourceIndexEntry irtm22 = newpack3.Find(predm2);
                                        if (irtm2 != null && irtm22 == null)
                                        {
                                            TGIBlock tgi = new TGIBlock(1, null, irtm2.ResourceType, irtm2.ResourceGroup, irtm2.Instance);
                                            IResourceIndexEntry irtem2 = newpack3.AddResource(tgi, pt.GetResource(irtm2), true);
                                            irtem2.Compressed = 0x5A42;
                                        }
                                    }
                                }
                            }
                            for (int i = 0; i < tone.NumberOverlays; i++)
                            {
                                Predicate<IResourceIndexEntry> predo = r => r.ResourceType == (uint)XmodsEnums.ResourceTypes.DDSuncompressed &&
                                    r.ResourceGroup == 0 && r.Instance == tone.GetOverLayInstance(i);
                                IResourceIndexEntry iro = p.Find(predo);
                                IResourceIndexEntry iro2 = newpack3.Find(predo);
                                if (iro != null && iro2 == null)
                                {
                                    TGIBlock tgi = new TGIBlock(1, null, iro.ResourceType, iro.ResourceGroup, iro.Instance);
                                    IResourceIndexEntry ire = newpack3.AddResource(tgi, p.GetResource(iro), true);
                                    ire.Compressed = 0x5A42;
                                }
                                Predicate<IResourceIndexEntry> predo2 = r => r.ResourceType == (uint)XmodsEnums.ResourceTypes.DXT5RLE2 &&
                                    r.ResourceGroup == 0 && r.Instance == tone.GetOverLayInstance(i);
                                IResourceIndexEntry iroo = p.Find(predo2);
                                IResourceIndexEntry iroo2 = newpack2.Find(predo2);
                                if (iroo != null && iroo2 == null)
                                {
                                    TGIBlock tgi = new TGIBlock(1, null, iroo.ResourceType, iroo.ResourceGroup, iroo.Instance);
                                    IResourceIndexEntry ireo = newpack3.AddResource(tgi, p.GetResource(iroo), true);
                                    ireo.Compressed = 0x5A42;
                                }

                                foreach (Package pt in gamePacksOther)
                                {
                                    IResourceIndexEntry irto = pt.Find(predo);
                                    IResourceIndexEntry irto2 = newpack3.Find(predo);
                                    if (irto != null && irto2 == null)
                                    {
                                        TGIBlock tgi = new TGIBlock(1, null, irto.ResourceType, irto.ResourceGroup, irto.Instance);
                                        IResourceIndexEntry ireto = newpack3.AddResource(tgi, pt.GetResource(irto), true);
                                        ireto.Compressed = 0x5A42;
                                    }
                                    IResourceIndexEntry irtoo = pt.Find(predo2);
                                    IResourceIndexEntry irtoo2 = newpack2.Find(predo2);
                                    if (irtoo != null && irtoo2 == null)
                                    {
                                        TGIBlock tgi = new TGIBlock(1, null, irtoo.ResourceType, irtoo.ResourceGroup, irtoo.Instance);
                                        IResourceIndexEntry iretoo = newpack3.AddResource(tgi, pt.GetResource(irtoo), true);
                                        iretoo.Compressed = 0x5A42;
                                    }
                                }
                            }
                        }
                        catch { }
                    }
                }
            }
           // WritePackage("Save skin color DDS textures package", newpack, "SkincolorTexturesDDS.package");
           // WritePackage("Save skin color LRLE textures package", newpack2, "SkincolorTexturesLRLE.package");
            WritePackage("Save skin color burn mask textures package", newpack3, "SkincolorTexturesBurnmask.package");
        }

        class CASPDisplay : CASP
        {
            public IResourceIndexEntry ires;
            public CASPDisplay(BinaryReader br, IResourceIndexEntry ires) : base(br) { this.ires = ires; }
            public override string ToString()
            {
                return this.PartName;
            }
        }

        private void ExtractFromMerged_button_Click(object sender, EventArgs e)
        {
            string mergePack = GetFilename("Select Troubleshooting or other merged package", Packagefilter);
            if (mergePack == null) return;
            Package p = OpenPackage(mergePack, false);
            ExtractFromMerge_listBox.Items.Clear();
           // List<CASP> caspList = new List<CASP>();
            Predicate<IResourceIndexEntry> pred = r => r.ResourceType == (uint)XmodsEnums.ResourceTypes.CASP;
            List<IResourceIndexEntry> iries = p.FindAll(pred);
            foreach (IResourceIndexEntry ires in iries)
            {
                CASPDisplay c = new CASPDisplay(new BinaryReader(p.GetResource(ires)), ires);
                int ind = ExtractFromMerge_listBox.Items.Add(c);
              //  caspList.Add(c);
            }
            ExtractFromMerge_listBox.Tag = p;
        }

        private void ExtractFromMerge_listBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ExtractFromMerge_listBox.Tag == null) return;
            Package mergePack = (Package)ExtractFromMerge_listBox.Tag;
           // List<CASP> caspList = (List<CASP>)ExtractFromMerge_listBox.Tag;
           // CASP c = caspList[ExtractFromMerge_listBox.SelectedIndex];
            if (ExtractFromMerge_listBox.SelectedIndex < 0) return;
            CASPDisplay c = (CASPDisplay)ExtractFromMerge_listBox.SelectedItem;

            Package p = (Package)Package.NewPackage(0);
            MemoryStream ms = new MemoryStream();
            c.Write(new BinaryWriter(ms));
            ms.Position = 0;
            IResourceIndexEntry ir = p.AddResource(c.ires, ms, true);
            if (ir != null) ir.Compressed = (ushort)0x5A42;

            TGI[] tgiLinks = c.LinkList;
            Predicate<IResourceIndexEntry> pred = r => r.ResourceType == (uint)XmodsEnums.ResourceTypes.CASP;

            ExtractResource(tgiLinks[c.RegionMapIndex], mergePack, p);
            ExtractResource(tgiLinks[c.TextureIndex], mergePack, p);
            ExtractResource(tgiLinks[c.ShadowIndex], mergePack, p);
            ExtractResource(tgiLinks[c.SpecularIndex], mergePack, p);
            ExtractResource(tgiLinks[c.SwatchIndex], mergePack, p);
            for (int lod = 0; lod < 4; lod++)
            {
                TGI[] geomLinks = c.MeshParts(lod);
                for (int i = 0; i < geomLinks.Length; i++)
                {
                    ExtractGeomResource(geomLinks[i], mergePack, p);
                }
            }

            WritePackage("Save package with extracted CASP and resources", p, "");
        }

        private void ExtractResource(TGI tgi, Package sourcePack, Package targetPack)
        {
            if (tgi.Instance == 0) return;
            Predicate<IResourceIndexEntry> pred = r => r.ResourceType == tgi.Type && r.ResourceGroup == tgi.Group && r.Instance == tgi.Instance;
            IResourceIndexEntry irie = sourcePack.Find(pred);
            if (irie != null)
            {
                Stream s = sourcePack.GetResource(irie);
                s.Position = 0;
                IResourceIndexEntry ir = targetPack.AddResource(irie, s, true);
                if (ir != null) ir.Compressed = (ushort)0x5A42;
            }

        }
        private void ExtractGeomResource(TGI tgi, Package sourcePack, Package targetPack)
        {
            if (tgi.Instance == 0) return;
            Predicate<IResourceIndexEntry> pred = r => r.ResourceType == tgi.Type && r.ResourceGroup == tgi.Group && r.Instance == tgi.Instance;
            IResourceIndexEntry irie = sourcePack.Find(pred);
            if (irie != null)
            {
                Stream s = sourcePack.GetResource(irie);
                MemoryStream ms = new MemoryStream();
                s.Position = 0;
                s.CopyTo(ms);
                s.Position = 0;
                IResourceIndexEntry ir = targetPack.AddResource(irie, s, true);
                if (ir != null) ir.Compressed = (ushort)0x5A42;
                GEOM g = new GEOM(new BinaryReader(ms));
                if (g.Shader.normalIndex > -1)
                {
                    TGI normTGI = g.TGIList[g.Shader.normalIndex];
                    ExtractResource(normTGI, sourcePack, targetPack);
                }
                if (g.Shader.emissionIndex > -1)
                {
                    TGI emitTGI = g.TGIList[g.Shader.emissionIndex];
                    ExtractResource(emitTGI, sourcePack, targetPack);
                }
            }
        }

        private void PackageDiffOriginal_button_Click(object sender, EventArgs e)
        {
            PackageDiffOriginal.Text = GetFilename("Select original package", Packagefilter);
        }

        private void PackageDiffDelta_button_Click(object sender, EventArgs e)
        {
            PackageDiffDelta.Text = GetFilename("Select updated/modified package", Packagefilter);
        }

        private void PackageDiffGo_button_Click(object sender, EventArgs e)
        {

            if (String.Compare(PackageDiffOriginal.Text, " ") <= 0 || String.Compare(PackageDiffDelta.Text, " ") <= 0)
            {
                MessageBox.Show("Please select two packages to compare!");
                return;
            }
            Package p1 = OpenPackage(PackageDiffOriginal.Text, false);
            Package p2 = OpenPackage(PackageDiffDelta.Text, false);
            Package diff = (Package)Package.NewPackage(0);
            XmodsEnums.ResourceTypes t;
            Enum.TryParse((string)PackageDiffType_comboBox.SelectedItem, out t);
            Predicate<IResourceIndexEntry> pred = r => r.ResourceType == (uint)t;
            List<IResourceIndexEntry> iries = p2.FindAll(pred);
            foreach (IResourceIndexEntry ires in iries)
            {
                Predicate<IResourceIndexEntry> test = r => r.ResourceType == ires.ResourceType && r.ResourceGroup == ires.ResourceGroup && r.Instance == ires.Instance;
                IResourceIndexEntry find = p1.Find(test);
                if (find == null)
                {
                    Stream s = p2.GetResource(ires);
                    TGIBlock tgi = new TGIBlock(1, null, ires);
                    IResourceIndexEntry ir = diff.AddResource(tgi, s, true);
                    if (ir != null) ir.Compressed = (ushort)0x5A42;
                }
            }
            WritePackage("Save differences package", diff, "");
        }


        private void TextureLabel_button_Click(object sender, EventArgs e)
        {
            TextureLabelPackage.Text = GetFilename("Select skin definition textures package", Packagefilter);
            if (String.Compare(TextureLabelPackage.Text, " ") <= 0) return;
            Package textures = OpenPackage(TextureLabelPackage.Text, false);
            Package labels = (Package)Package.NewPackage(0);
            Font font = new Font("Arial", 36, FontStyle.Regular, GraphicsUnit.Pixel);
            Brush brush = new SolidBrush(Color.Black);
            Predicate<IResourceIndexEntry> pred = r => r.ResourceType == (uint)XmodsEnums.ResourceTypes.LRLE;
            List<IResourceIndexEntry> iries = textures.FindAll(pred);
            foreach (IResourceIndexEntry ires in iries)
            {
                Stream s = textures.GetResource(ires);
                LRLE lrle = new LRLE(new BinaryReader(s));
                Bitmap bitmap = lrle.image;
                string l = ires.Instance.ToString("X16").Substring(0, 4);
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    g.DrawString(l, font, brush, new Point(85, 1080));
                }
                LRLE lrle2 = new LRLE(bitmap);
                TGIBlock tgi = new TGIBlock(1, null, ires);
                IResourceIndexEntry ir = labels.AddResource(tgi, lrle2.Stream, true);
                if (ir != null) ir.Compressed = (ushort)0x5A42;
            }
            WritePackage("Save labelled package", labels, "");
        }
    }
}
