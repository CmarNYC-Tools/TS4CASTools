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
using System.Drawing.Imaging;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Xmods.DataLib;
using s4pi.ImageResource;
using s4pi.Interfaces;
using s4pi.Package;

namespace XMODS
{
    public partial class Form1 : Form
    {
        string clonePartName;
        List<CASPinfo> clonePackCASPs;
        List<IResourceIndexEntry> iresCASPs;
        CASP myCASP;
        IResourceIndexEntry myKey;
        int myRow, myCASPindex;
        bool changesRecolor = false, changesGeneral = false, changesThumbs = false, changesMesh = false, changesRegion = false, changesSliders = false, changesHairColor = false;
        DSTResource mySwatch;
        Object myTexture;
        AResource myShadow, mySpecular, myColorShiftMask;
        DdsFile ddsSwatch, ddsTexture, ddsShadow, ddsSpecular, ddsColorShiftMask;
        Image textureImage, specularImage, shadowImage, bumpmapImage, glowImage, colorShiftMaskImage;
        IResourceIndexEntry iresSwatch, iresTexture, iresShadow, iresSpecular, iresColorShiftMask;
        bool clonedSwatch, clonedTexture, clonedShadow, clonedSpecular, clonedColorShiftMask, 
             importedSwatch, importedTexture, importedShadow, importedSpecular, importedcolorShiftMask;
        string[] SpeciesNames = Enum.GetNames(typeof(XmodsEnums.Species));
        string[] BodyTypeNames = Enum.GetNames(typeof(XmodsEnums.BodyType));
        string[] BodySubTypeNames = Enum.GetNames(typeof(XmodsEnums.BodySubType));
        string[] ExcludeParts = Enum.GetNames(typeof(XmodsEnums.ExcludePartFlag));
        string[] ExcludeParts2 = Enum.GetNames(typeof(XmodsEnums.ExcludePartFlag2));
        ulong[] ExcludePartValues = (ulong[])Enum.GetValues(typeof(XmodsEnums.ExcludePartFlag));
        ulong[] ExcludePartValues2 = (ulong[])Enum.GetValues(typeof(XmodsEnums.ExcludePartFlag2));
        string[] ExcludeModifiers = Enum.GetNames(typeof(XmodsEnums.ExcludeRegion));
        ulong[] ExcludeModifierValues = (ulong[])Enum.GetValues(typeof(XmodsEnums.ExcludeRegion));
        XmodsEnums.BodyType savedBodyType;
        bool createInGameCheckState;

        private class CASPinfo
        {
            internal CASP Casp;
            internal ThumbnailResource maleThumb, femaleThumb;

            internal CASPinfo(CASP casp, ThumbnailResource maleThumbnail, ThumbnailResource femaleThumbnail)
            {
                Casp = casp;
                maleThumb = maleThumbnail;
                femaleThumb = femaleThumbnail;
            }

            internal CASPinfo(CASP casp, ThumbnailResource[] maleAndFemaleThumbnails)
            {
                Casp = casp;
                maleThumb = maleAndFemaleThumbnails[0];
                femaleThumb = maleAndFemaleThumbnails[1];
            }
        }

        private void ListCASPflags(DataGridView flagsGrid, List<uint[]> flagsList, string[] categoryNames, uint[] categoryNumeric)
        {
            foreach (uint[] f in flagsList)
            {
                if (Array.IndexOf(categoryNumeric, f[0]) < 0) continue;
                flagsGrid.Rows.Add();
                int index = flagsGrid.Rows.Count - 1;
                DataGridViewComboBoxCell flag = (DataGridViewComboBoxCell)flagsGrid.Rows[index].Cells[0];
                flag.Items.AddRange(categoryNames);
                string selectedFlagType = "";
                bool found = false;
                for (int i = 0; i < categoryNumeric.Length; i++)
                {
                    if (f[0] == categoryNumeric[i])
                    {
                        flag.Value = categoryNames[i];
                        selectedFlagType = categoryNames[i];
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    MessageBox.Show("Unknown category found: " + f[0].ToString());
                    flag.Items.Add(f[0].ToString());
                    flag.Value = f[0].ToString();
                }
                DataGridViewComboBoxCell val = (DataGridViewComboBoxCell)flagsGrid.Rows[index].Cells[1];
                List<string> tmp = new List<string>();
                for (int i = 0; i < tagNames.Count; i++)
                {
                    int ind = tagNames[i].IndexOf("_");
                    if (ind > 0 && String.Compare(selectedFlagType, tagNames[i].Substring(0, ind)) == 0)
                    {
                        tmp.Add(tagNames[i].Substring(tagNames[i].IndexOf("_") + 1));
                    }
                }
                //if (errors.Length > 1) MessageBox.Show(errors);
                val.Items.AddRange(tmp.ToArray());
                found = false;
                for (int i = 0; i < tagNumbers.Count; i++)
                {
                    if (f[1] == tagNumbers[i])
                    {
                        if (tagNames[i].IndexOf("_") > 0) val.Value = tagNames[i].Substring(tagNames[i].IndexOf("_") + 1);
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    MessageBox.Show("Unknown category value found: " + f[1].ToString());
                    val.Items.Add(f[1].ToString());
                    val.Value = f[1].ToString();
                }
                if (!val.Items.Contains(val.Value))
                {
                    MessageBox.Show("Value " + val.Value.ToString() + " does not match category " + flag.Value.ToString() + ", please correct!");
                    string s = "* " + val.Value.ToString() + " *";
                    val.Items.Add(s);
                    val.Value = s;
                }
            }
        }

        private void CloneFlags_dataGridView_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            DataGridView grid = sender as DataGridView;
           // DataGridViewComboBoxCell flag = (DataGridViewComboBoxCell)grid.Rows[e.RowIndex].Cells[0];
            DataGridViewComboBoxCell val = (DataGridViewComboBoxCell)grid.Rows[e.RowIndex].Cells[1];
           // MessageBox.Show("Value " + val.Value.ToString() + " does not match category " + flag.Value.ToString());
           // val.Items.Add(val.Value.ToString());
            if (val.Items.Contains(val.Value))
            {
                MessageBox.Show("An unknown data error has occurred: " + e.Context.ToString());
            }
        }

        private void CloneAddFlag_button_Click(DataGridView grid, EventArgs e, bool forAll)
        {
            grid.Rows.Add();
            int index = grid.Rows.Count - 1;
            DataGridViewComboBoxCell flag = (DataGridViewComboBoxCell)grid.Rows[index].Cells[0];
            if (forAll)
            {
                flag.Items.AddRange(tagCategoryNames4All);
            }
            else
            {
                flag.Items.AddRange(tagCategoryNamesInd);
            }
        }

        private void CloneFlags_dataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            DataGridView grid = sender as DataGridView;
            if (e.ColumnIndex == 2 & e.RowIndex >= 0)
            {
                DialogResult res = MessageBox.Show("Okay to delete this CASP property?", "Delete Property", MessageBoxButtons.OKCancel);
                if (res == DialogResult.OK)
                {
                    grid.Rows.RemoveAt(e.RowIndex);
                    if (string.Compare(grid.Name, "ClonePropFlags_dataGridView") == 0)
                    {
                        changesGeneral = true;
                    }
                    else if (string.Compare(grid.Name, "CloneColorProp_dataGridView") == 0)
                    {
                        changesRecolor = true;
                    }
                }
            }
        }

        private void CloneFlags_dataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            DataGridView grid = sender as DataGridView;
            if (e.ColumnIndex == 0 & e.RowIndex >= 0)
            {
                string catName = grid.Rows[e.RowIndex].Cells[0].Value.ToString();
                DataGridViewComboBoxCell val = (DataGridViewComboBoxCell)grid.Rows[e.RowIndex].Cells[1];
                List<string> tmp = new List<string>();
                for (int i = 0; i < tagNames.Count; i++)
                {
                    int ind = tagNames[i].IndexOf("_");
                    if (ind > 0 && String.Compare(catName, tagNames[i].Substring(0, ind)) == 0)
                    {
                        tmp.Add(tagNames[i].Substring(tagNames[i].IndexOf("_") + 1));
                    }
                }
                val.Value = null;
                val.Items.Clear();
                val.Items.AddRange(tmp.ToArray());
            }
            if (string.Compare(grid.Name, "ClonePropFlags_dataGridView") == 0)
            {
                changesGeneral = true;
            }
            else if (string.Compare(grid.Name, "CloneColorProp_dataGridView") == 0)
            {
                changesRecolor = true;
            }
        }

        private List<uint[]> ReadPropertyFlags(DataGridView flagsGrid)
        {
            List<uint[]> newFlags = new List<uint[]>();
            for (int i = 0; i < flagsGrid.Rows.Count; i++)
            {
                DataGridViewComboBoxCell flag = (DataGridViewComboBoxCell)flagsGrid.Rows[i].Cells[0];
                string flagName = flag.Value.ToString();
                int ind = tagCategoryNames.IndexOf(flagName);
                uint flagNumeric = 0;
                if (ind >= 0) flagNumeric = tagCategoryNumbers[ind];
                else
                {
                    uint tmp;
                    if (uint.TryParse(flagName, out tmp)) flagNumeric = tmp;
                    else MessageBox.Show("Cannot read unknown flag category!");
                }
                DataGridViewComboBoxCell val = (DataGridViewComboBoxCell)flagsGrid.Rows[i].Cells[1];
                string flagVal = flagName + "_" + val.Value.ToString();
                ind = tagNames.IndexOf(flagVal);
                uint valNumeric = 0;
                if (ind >= 0) valNumeric = tagNumbers[ind];
                else
                {
                    uint tmp;
                    if (uint.TryParse(val.Value.ToString(), out tmp)) valNumeric = tmp;
                    else MessageBox.Show("Cannot read unknown flag value!");
                }
                newFlags.Add(new uint[] { flagNumeric, valNumeric });
            }
            return newFlags;
        }

        private List<uint[]> UpdatePropertyFlags(List<uint[]> caspFlags, List<uint[]> modifiedFlags, bool forAll)
        {
            List<uint[]> newFlags = new List<uint[]>();
            uint[] filterIn;
            if (forAll)
            {
                filterIn = tagCategoryNumbersInd;
            }
            else
            {
                filterIn = tagCategoryNumbers4All;
            }
            if (forAll) newFlags.AddRange(modifiedFlags);
            for (int i = 0; i < caspFlags.Count; i++)
            {
                for (int j = 0; j < filterIn.Length; j++)
                {
                    if (caspFlags[i][0] == filterIn[j]) newFlags.Add(caspFlags[i]);
                }
            }
            if (!forAll) newFlags.AddRange(modifiedFlags);
            return newFlags;
        }

        private void ShowClonePackProperties()
        {
            cloneWait_label.Visible = true;
            cloneWait_label.Refresh();
            ClonePropFlags_dataGridView.CellValueChanged -= ClonePropFlags_dataGridView_CellValueChanged;
            ClonePropID.TextChanged -= ClonePropID_TextChanged;
            ClonePropFlags_checkedListBox.ItemCheck -= ClonePropFlags_checkedListBox_ItemChecked;
            ClonePropType_comboBox.SelectedIndexChanged -= ClonePropType_comboBox_SelectedIndexChanged;
            ClonePropUVSpace_comboBox.SelectedIndexChanged -= ClonePropUVSpace_comboBox_SelectedIndexChanged;
            ClonePropSubType_comboBox.SelectedIndexChanged -= ClonePropSubType_comboBox_SelectedIndexChanged;
            ClonePropSpecies_comboBox.SelectedIndexChanged -= ClonePropSpecies_comboBox_SelectedIndexChanged;
            ClonePropAge_checkedListBox.ItemCheck -= ClonePropAge_checkedListBox_ItemChecked;
            ClonePropGender_checkedListBox.ItemCheck -= ClonePropGender_checkedListBox_ItemChecked;
            ClonePropSortOrder.TextChanged -= ClonePropSortOrder_TextChanged;
            ClonePropCompMethod.ValueChanged -= ClonePropCompMethod_ValueChanged;
            ClonePropSortLayer.TextChanged -= ClonePropSortLayer_TextChanged;
            ClonePropExclude_checkedListBox.SelectedValueChanged -= ClonePropExclude_checkedListBox_SelectedValueChanged;
            ClonePropExcludeModifiers_checkedListBox.SelectedValueChanged -= ClonePropExcludeModifiers_checkedListBox_SelectedValueChanged;
            ClonePropLegacy_checkBox.CheckedChanged -= ClonePropLegacy_checkBox_CheckedChanged;
            //ClonePropSliders_checkBox.CheckedChanged -= ClonePropSliders_checkBox_CheckedChanged;
            ClonePropReset();
            //myCASP = clonePackCASPs[0].Casp;
            //myKey = iresCASPs[0];
            //myRow = 0;
            //myCASPindex = 0;
            clonePackID.Text = myCASP.PackID.ToString();
            int tmp = myCASP.PartName.LastIndexOf("_");
            if (tmp >= 0)
            {
                clonePartName = myCASP.PartName.Substring(0, tmp);
            }
            else
            {
                clonePartName = myCASP.PartName;
            }

            ClonePropID.Text = myCASP.OutfitID.ToString("X8");

            List<uint[]> caspFlags = myCASP.CategoryTags;

            ListCASPflags(ClonePropFlags_dataGridView, caspFlags, tagCategoryNames4All, tagCategoryNumbers4All);

            ClonePropFlags_checkedListBox.SetItemChecked(0, myCASP.OccultDisableForHuman);
            ClonePropFlags_checkedListBox.SetItemChecked(1, myCASP.OccultDisableForAlien);
            ClonePropFlags_checkedListBox.SetItemChecked(2, myCASP.OccultDisableForVampire);
            ClonePropFlags_checkedListBox.SetItemChecked(3, myCASP.OccultDisableForMermaid);
            ClonePropFlags_checkedListBox.SetItemChecked(4, myCASP.OccultDisableForWitch);
            ClonePropFlags_checkedListBox.SetItemChecked(5, myCASP.OccultDisableForWerewolf);
            ClonePropFlags_checkedListBox.SetItemChecked(6, myCASP.FlagAllowForCASRandom);
            ClonePropFlags_checkedListBox.SetItemChecked(7, myCASP.FlagAllowForLiveRandom);
            ClonePropFlags_checkedListBox.SetItemChecked(8, myCASP.FlagShowInUI);
            ClonePropFlags_checkedListBox.SetItemChecked(9, myCASP.FlagShowInSimInfoPanel);
            ClonePropFlags_checkedListBox.SetItemChecked(10, myCASP.FlagRestrictOppositeGender);
            ClonePropFlags_checkedListBox.SetItemChecked(11, myCASP.FlagRestrictOppositeFrame);
            ClonePropFlags_checkedListBox.SetItemChecked(12, myCASP.FlagCreateInGame);
            createInGameCheckState = myCASP.FlagCreateInGame;

            ClonePropExclude_checkedListBox.Items.AddRange(ExcludeParts);
            for (int i = 0; i < ExcludeParts.Length; i++)
            {
                if ((ExcludePartValues[i] & myCASP.ExcludePartFlags) > 0)
                {
                    ClonePropExclude_checkedListBox.SetItemChecked(i, true);
                }
            }
            ClonePropExclude_checkedListBox.Items.AddRange(ExcludeParts2);
            for (int i = 0; i < ExcludeParts2.Length; i++)
            {
                if ((ExcludePartValues2[i] & myCASP.ExcludePartFlags2) > 0)
                {
                    ClonePropExclude_checkedListBox.SetItemChecked(i + ExcludeParts.Length, true);
                }
            }
            ClonePropExcludeModifiers_checkedListBox.Items.AddRange(ExcludeModifiers);
            for (int i = 0; i < ExcludeModifiers.Length; i++)
            {
                if ((ExcludeModifierValues[i] & myCASP.ExcludeModifierRegionFlags) > 0)
                {
                    ClonePropExcludeModifiers_checkedListBox.SetItemChecked(i, true);
                }
            }

            ClonePropType_comboBox.Items.AddRange(BodyTypeNames);
            ClonePropType_comboBox.SelectedIndex = 0;
            bool found = false;
            for (int i = 0; i < BodyTypeNames.Length; i++)
            {
                if (Enum.GetName(typeof(XmodsEnums.BodyType), myCASP.BodyType) == BodyTypeNames[i])
                {
                    ClonePropType_comboBox.SelectedIndex = i;
                    found = true;
                    break;
                }
            }
            if (!found)
            {
                int ind = ClonePropType_comboBox.Items.Add("0x" + myCASP.BodyTypeNumeric.ToString("X2"));
                ClonePropType_comboBox.SelectedIndex = ind;
            }

            ClonePropUVSpace_comboBox.Items.AddRange(BodyTypeNames);
            ClonePropUVSpace_comboBox.SelectedIndex = 0;
            found = false;
            for (int i = 0; i < BodyTypeNames.Length; i++)
            {
                if (Enum.GetName(typeof(XmodsEnums.BodyType), myCASP.SharedUVSpace) == BodyTypeNames[i])
                {
                    ClonePropUVSpace_comboBox.SelectedIndex = i;
                    found = true;
                    break;
                }
            }
            if (!found)
            {
                int ind = ClonePropUVSpace_comboBox.Items.Add("0x" + myCASP.SharedUVNumeric.ToString("X2"));
                ClonePropUVSpace_comboBox.SelectedIndex = ind;
            }

            ClonePropSubType_comboBox.Items.AddRange(BodySubTypeNames);
            ClonePropSubType_comboBox.SelectedIndex = 0;
            for (int i = 0; i < BodySubTypeNames.Length; i++)
            {
                if (Enum.GetName(typeof(XmodsEnums.BodySubType), myCASP.BodySubType) == BodySubTypeNames[i])
                {
                    ClonePropSubType_comboBox.SelectedIndex = i;
                    break;
                }
            }

            ClonePropSpecies_comboBox.Items.AddRange(SpeciesNames);
            ClonePropSpecies_comboBox.SelectedIndex = 0;
            for (int i = 0; i < SpeciesNames.Length; i++)
            {
                if (Enum.GetName(typeof(XmodsEnums.Species), myCASP.Species) == SpeciesNames[i])
                {
                    ClonePropSpecies_comboBox.SelectedIndex = i;
                    break;
                }
            }

            ClonePropAge_checkedListBox.Items.AddRange(AgeNames);
            for (int i = 0; i < ClonePropAge_checkedListBox.Items.Count; i++)
            {
                if (((uint)myCASP.Age & AgeValues[i]) > 0)
                {
                    ClonePropAge_checkedListBox.SetItemChecked(i, true);
                }
            }

            ClonePropGender_checkedListBox.Items.AddRange(GenderNames);
            for (int i = 0; i < ClonePropGender_checkedListBox.Items.Count; i++)
            {
                if (((uint)myCASP.Gender & GenderValues[i]) > 0)
                {
                    ClonePropGender_checkedListBox.SetItemChecked(i, true);
                }
            }

            ClonePropSortOrder.Text = myCASP.SortPriority.ToString();
            ClonePropCompMethod.Value = myCASP.CompositionMethod;
            ClonePropSortLayer.Text = myCASP.SortLayer.ToString();

            if (myCASP.Version >= 0x2B)
            {
                ClonePropLegacy_checkBox.Checked = false;
                ClonePropCreate_comboBox.Enabled = true;
                try { ClonePropCreate_comboBox.SelectedItem = new KeyValuePair<uint, string>(myCASP.CreateDescriptionKey, unlockLevels[myCASP.CreateDescriptionKey]); }
                catch { ClonePropCreate_comboBox.SelectedIndex = 0; }
            }
            else
            {
                ClonePropLegacy_checkBox.Checked = true;
                ClonePropCreate_comboBox.Enabled = false;
            }
            //ClonePropSliders_checkBox.Checked = (myCASP.Version >= 0x2C);
            SliderSettings_panel.Enabled = myCASP.Version >= 0x2C;
            HairColorSettings_panel.Enabled = myCASP.Version >= 0x2E;

            myShadow = FindCloneTextureRLE(myCASP.LinkList[myCASP.ShadowIndex], out iresShadow, out clonedShadow);
            if (myShadow != null)
            {
                ddsShadow = new DdsFile();
                ddsShadow.Load(myShadow.ToDDS(), false);
                shadowImage = new Bitmap(ddsShadow.Image);
            }
            else
            {
                ddsShadow = null;
                shadowImage = null;
            }
            mySpecular = FindCloneTextureRLE(myCASP.LinkList[myCASP.SpecularIndex], out iresSpecular, out clonedSpecular);
            if (mySpecular != null)
            {
                ddsSpecular = new DdsFile();
                ddsSpecular.Load(mySpecular.ToDDS(), false);
                specularImage = new Bitmap(ddsSpecular.Image);
            }
            else
            {
                ddsSpecular = null;
                specularImage = null;
            }

            myColorShiftMask = FindCloneTextureRLE(myCASP.LinkList[myCASP.ColorShiftMaskIndex], out iresColorShiftMask, out clonedColorShiftMask);
            if (myColorShiftMask != null)
            {
                ddsColorShiftMask = new DdsFile();
                ddsColorShiftMask.Load(myColorShiftMask.ToDDS(), false);
                colorShiftMaskImage = new Bitmap(ddsColorShiftMask.Image);
            }
            else
            {
                ddsColorShiftMask = null;
                colorShiftMaskImage = null;
            }

            ClonePropFlags_dataGridView.CellValueChanged += ClonePropFlags_dataGridView_CellValueChanged;
            ClonePropID.TextChanged += ClonePropID_TextChanged;
            ClonePropFlags_checkedListBox.ItemCheck += ClonePropFlags_checkedListBox_ItemChecked;
            ClonePropType_comboBox.SelectedIndexChanged += ClonePropType_comboBox_SelectedIndexChanged;
            ClonePropUVSpace_comboBox.SelectedIndexChanged += ClonePropUVSpace_comboBox_SelectedIndexChanged;
            ClonePropSubType_comboBox.SelectedIndexChanged += ClonePropSubType_comboBox_SelectedIndexChanged;
            ClonePropSpecies_comboBox.SelectedIndexChanged += ClonePropSpecies_comboBox_SelectedIndexChanged;
            ClonePropAge_checkedListBox.ItemCheck += ClonePropAge_checkedListBox_ItemChecked;
            ClonePropGender_checkedListBox.ItemCheck += ClonePropGender_checkedListBox_ItemChecked;
            ClonePropSortOrder.TextChanged += ClonePropSortOrder_TextChanged;
            ClonePropCompMethod.ValueChanged += ClonePropCompMethod_ValueChanged;
            ClonePropSortLayer.TextChanged += ClonePropSortLayer_TextChanged;
            ClonePropExclude_checkedListBox.SelectedValueChanged += ClonePropExclude_checkedListBox_SelectedValueChanged;
            ClonePropExcludeModifiers_checkedListBox.SelectedValueChanged += ClonePropExcludeModifiers_checkedListBox_SelectedValueChanged;
            ClonePropLegacy_checkBox.CheckedChanged += ClonePropLegacy_checkBox_CheckedChanged;
            //ClonePropSliders_checkBox.CheckedChanged += ClonePropSliders_checkBox_CheckedChanged;
            savedBodyType = myCASP.BodyType;
            if (myCASP.BodyType == XmodsEnums.BodyType.Top)
            {
                MeshSlotrayRef_comboBox.SelectedIndex = 0;
            }
            else if (myCASP.BodyType == XmodsEnums.BodyType.Bottom)
            {
                MeshSlotrayRef_comboBox.SelectedIndex = 1;
            }
            else
            {      
                MeshSlotrayRef_comboBox.SelectedIndex = 2;
            }

            ShowSliderSetup(myCASP.OpacitySliderSettings, CloneSliderOpacityMinimum, null, CloneSliderOpacityIncrement);
            ShowSliderSetup(myCASP.HueSliderSettings, CloneSliderHueMinimum, CloneSliderHueMaximum, CloneSliderHueIncrement);
            ShowSliderSetup(myCASP.SaturationSliderSettings, CloneSliderSaturationMinimum, CloneSliderSaturationMaximum, CloneSliderSaturationIncrement);
            ShowSliderSetup(myCASP.BrightnessSliderSettings, CloneSliderBrightnessMinimum, CloneSliderBrightnessMaximum, CloneSliderBrightnessIncrement);
            changesSliders = false;

            ShowHairColorSetup(myCASP);

            cloneWait_label.Visible = false;
            cloneWait_label.Refresh();
        }

        private void ShowSliderSetup(CASP.OpacitySettings settings, TextBox minimum, TextBox maximum, TextBox increment)
        {
            if (settings == null) return;
            minimum.Text = settings.Minimum.ToString();
            increment.Text = settings.Increment.ToString();
            if (settings is CASP.SliderSettings)
            {
                maximum.Text = (settings as CASP.SliderSettings).Maximum.ToString();
            }
        }

        private CASP.OpacitySettings ReadSliderSetup(TextBox minimum, TextBox increment)
        {
            try
            {
                CASP.OpacitySettings opacity = new CASP.OpacitySettings(float.Parse(minimum.Text), float.Parse(increment.Text));
                return opacity;
            }
            catch
            {
                MessageBox.Show("Unable to read slider opacity settings!");
            }
            return null;
        }
        private CASP.SliderSettings ReadSliderSetup(TextBox minimum, TextBox maximum, TextBox increment)
        {
            try
            {
                CASP.SliderSettings slider = new CASP.SliderSettings(float.Parse(minimum.Text), float.Parse(maximum.Text), float.Parse(increment.Text));
                return slider;
            }
            catch
            {
                MessageBox.Show("Unable to read color slider settings!");
            }
            return null;
        }

        private void WipeSliderSettings()
        {
            CloneSliderOpacityMinimum.Text = "";
            CloneSliderOpacityIncrement.Text = "";
            CloneSliderHueMinimum.Text = "";
            CloneSliderHueMaximum.Text = "";
            CloneSliderHueIncrement.Text = "";
            CloneSliderSaturationMinimum.Text = "";
            CloneSliderSaturationMaximum.Text = "";
            CloneSliderSaturationIncrement.Text = "";
            CloneSliderBrightnessMinimum.Text = "";
            CloneSliderBrightnessMaximum.Text = "";
            CloneSliderBrightnessIncrement.Text = "";
            changesSliders = false;
        }

        private void ClonePropTexture_comboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetupClonePropTextureImage();
        }

        private void SetupClonePropTextureImage()
        {
            Image tmp;
            switch (ClonePropTexture_comboBox.SelectedIndex)
            {
                case 0:     //shadow
                    tmp = shadowImage;
                    break;
                case 1:     //specular
                    tmp = specularImage;
                    break;
                case 2:     //bumpmap
                    tmp = bumpmapImage;
                    break;
                case 3:     //emissionmap
                    tmp = glowImage;
                    break;
                case 4:     //colorshiftmask
                    tmp = colorShiftMaskImage;
                    break;
                default:
                    tmp = null;
                    break;
            }
            if (tmp == null) tmp = Properties.Resources.NullImage;
            ClonePropTexture_pictureBox.Image = tmp;
        }

        private void ClonePropAddFlag_button_Click(object sender, EventArgs e)
        {
            CloneAddFlag_button_Click(ClonePropFlags_dataGridView, e, true);
            changesGeneral = true;
        }

        private void ClonePropFlags_dataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            CloneFlags_dataGridView_CellContentClick(sender, e);
        }

        private void ClonePropFlags_dataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            CloneFlags_dataGridView_CellValueChanged(sender, e);
        }

        private void ClonePropCompMethod_ValueChanged(object sender, EventArgs e)
        {
            changesGeneral = true;
        }

        private void ClonePropFlags_checkedListBox_ItemChecked(object sender, ItemCheckEventArgs e)
        {
            if (ClonePropLegacy_checkBox.Checked && e.Index == 11 && e.NewValue == CheckState.Checked && createInGameCheckState == false)
                MessageBox.Show("'Create in Game' cannot be used in Legacy compatibility." + Environment.NewLine +
                                "Please uncheck 'Legacy Compatible' if you want this option.");
            createInGameCheckState = (e.NewValue == CheckState.Checked);
            changesGeneral = true;
        }

        private void ClonePropType_comboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            changesGeneral = true;
            if (ClonePropType_comboBox.SelectedIndex < ClonePropUVSpace_comboBox.Items.Count)
                ClonePropUVSpace_comboBox.SelectedIndex = ClonePropType_comboBox.SelectedIndex;
        }

        private void ClonePropUVSpace_comboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            changesGeneral = true;
        }

        private void ClonePropSubType_comboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            changesGeneral = true;
        }

        private void ClonePropSpecies_comboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            changesGeneral = true;
        }

        private void ClonePropAge_checkedListBox_ItemChecked(object sender, EventArgs e)
        {
            changesGeneral = true;
        }

        private void ClonePropGender_checkedListBox_ItemChecked(object sender, EventArgs e)
        {
            changesGeneral = true;
        }

        private void ClonePropSortOrder_TextChanged(object sender, EventArgs e)
        {
            changesGeneral = true;
        }

        private void ClonePropSortLayer_TextChanged(object sender, EventArgs e)
        {
            changesGeneral = true;
        }

        private void ClonePropID_TextChanged(object sender, EventArgs e)
        {
            changesGeneral = true;
        }

        private void ClonePropExcludeToggle_button_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < ClonePropExclude_checkedListBox.Items.Count; i++)
            {
                ClonePropExclude_checkedListBox.SetItemChecked(i, ClonePropExclude_checkedListBox.GetItemChecked(i) ^ true); 
            }
            changesGeneral = true;
        }

        private void ClonePropExclude_checkedListBox_SelectedValueChanged(object sender, EventArgs e)
        {
            changesGeneral = true;
        }

        private void ClonePropExcludeModifiers_button_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < ClonePropExcludeModifiers_checkedListBox.Items.Count; i++)
            {
                ClonePropExcludeModifiers_checkedListBox.SetItemChecked(i, ClonePropExcludeModifiers_checkedListBox.GetItemChecked(i) ^ true);
            }
            changesGeneral = true;
        }

        private void ClonePropExcludeModifiers_checkedListBox_SelectedValueChanged(object sender, EventArgs e)
        {
            changesGeneral = true;
        }

        private void ClonePropReset()
        {
            ClonePropFlags_dataGridView.Rows.Clear();
            for (int i = 0; i < ClonePropFlags_checkedListBox.Items.Count; i++)
            {
                ClonePropFlags_checkedListBox.SetItemChecked(i, false);
            }
            ClonePropID.Text = "";
            clonePackID.Text = "";
            ClonePropType_comboBox.Items.Clear();
            ClonePropUVSpace_comboBox.Items.Clear();
            ClonePropSubType_comboBox.Items.Clear();
            ClonePropSpecies_comboBox.Items.Clear();
            ClonePropAge_checkedListBox.Items.Clear();
            ClonePropGender_checkedListBox.Items.Clear();
            ClonePropExclude_checkedListBox.Items.Clear();
            ClonePropExcludeModifiers_checkedListBox.Items.Clear();
            ClonePropSortOrder.Text = "";
            ClonePropCompMethod.Value = 1;
            ClonePropCreate_comboBox.SelectedIndex = 0;
            ClonePropSortLayer.Text = "";
        }

        private void ClonePropCommit_button_Click(object sender, EventArgs e)
        {
            ClonePropCommit();
        }

        private void ClonePropCommit()
        {
            if (clonePackCASPs == null) return;
            uint propertyID;
            try
            {
                propertyID = uint.Parse(ClonePropID.Text, System.Globalization.NumberStyles.HexNumber);
            }
            catch
            {
                MessageBox.Show("Please enter a valid hexidecimal number in the Property ID box!");
                return;
            }
            float sortOrder;
            try
            {
                sortOrder = float.Parse(ClonePropSortOrder.Text);
            }
            catch
            {
                MessageBox.Show("Please enter a valid number in the Sort Order box!");
                return;
            }
            int sortLayer;
            try
            {
                sortLayer = Int32.Parse(ClonePropSortLayer.Text);
            }
            catch
            {
                MessageBox.Show("Please enter a valid number in the Sort Layer box!");
                return;
            }
            List<uint[]> modifiedFlags = ReadPropertyFlags(ClonePropFlags_dataGridView);

            bool flagDisableForHuman = ClonePropFlags_checkedListBox.GetItemChecked(0);
            bool flagDisableForAlien = ClonePropFlags_checkedListBox.GetItemChecked(1);
            bool flagDisableForVampire = ClonePropFlags_checkedListBox.GetItemChecked(2);
            bool flagDisableForMermaid = ClonePropFlags_checkedListBox.GetItemChecked(3);
            bool flagDisableForSpellcaster = ClonePropFlags_checkedListBox.GetItemChecked(4);
            bool flagDisableForWerewolf = ClonePropFlags_checkedListBox.GetItemChecked(5);
            bool flagAllowForCASRandom = ClonePropFlags_checkedListBox.GetItemChecked(6);
            bool flagAllowForLiveRandom = ClonePropFlags_checkedListBox.GetItemChecked(7);
            bool flagShowInUI = ClonePropFlags_checkedListBox.GetItemChecked(8);
            bool flagShowInInfoPanel = ClonePropFlags_checkedListBox.GetItemChecked(9);
            bool flagRestrictOppositeGender = ClonePropFlags_checkedListBox.GetItemChecked(10);
            bool flagRestrictOppositeFrame = ClonePropFlags_checkedListBox.GetItemChecked(11);
            bool flagCreateInGame = ClonePropLegacy_checkBox.Checked ? false : ClonePropFlags_checkedListBox.GetItemChecked(12);

            ulong excludeFlags = 0UL;
            for (int i = 0; i < ExcludePartValues.Length; i++)
            {
                if (ClonePropExclude_checkedListBox.GetItemChecked(i))
                {
                    excludeFlags += ExcludePartValues[i] ;
                }
            }
            ulong excludeFlags2 = 0UL;
            for (int i = 0; i < ExcludePartValues2.Length; i++)
            {
                if (ClonePropExclude_checkedListBox.GetItemChecked(i + ExcludePartValues.Length))
                {
                    excludeFlags2 += ExcludePartValues2[i];
                }
            }
            ulong excludeModifierFlags = 0UL;
            for (int i = 0; i < ExcludeModifierValues.Length; i++)
            {
                if (ClonePropExcludeModifiers_checkedListBox.GetItemChecked(i))
                {
                    excludeModifierFlags += ExcludeModifierValues[i];
                }
            }

            XmodsEnums.BodyType bodyType = XmodsEnums.BodyType.All;
            try
            {
                bodyType = (XmodsEnums.BodyType)Enum.Parse(typeof(XmodsEnums.BodyType), BodyTypeNames[ClonePropType_comboBox.SelectedIndex]);
            }
            catch
            {
                uint body = 0;
                if (UInt32.TryParse((string)ClonePropType_comboBox.SelectedValue, NumberStyles.HexNumber, NumberFormatInfo.InvariantInfo, out body))
                {
                    bodyType = (XmodsEnums.BodyType)body;
                }
                else
                {
                    bodyType = savedBodyType;
                }
            }
            if (savedBodyType != bodyType) 
            {
                UpdateMeshesVertexColor(bodyType);
                MeshEditCommit();
                savedBodyType = bodyType;
            }
            XmodsEnums.BodyType UVSpace = (XmodsEnums.BodyType)Enum.Parse(typeof(XmodsEnums.BodyType), BodyTypeNames[ClonePropUVSpace_comboBox.SelectedIndex]);

            XmodsEnums.BodySubType bodySubType = (XmodsEnums.BodySubType)Enum.Parse(typeof(XmodsEnums.BodySubType), BodySubTypeNames[ClonePropSubType_comboBox.SelectedIndex]);

            XmodsEnums.Species species = (XmodsEnums.Species)Enum.Parse(typeof(XmodsEnums.Species), SpeciesNames[ClonePropSpecies_comboBox.SelectedIndex]);

            uint CreateDesc = ((KeyValuePair<uint, string>)ClonePropCreate_comboBox.SelectedItem).Key;

            uint tmp = 0;
            for (int i = 0; i < AgeValues.Length; i++)
            {
                if (ClonePropAge_checkedListBox.GetItemChecked(i)) tmp += AgeValues[i];
            }
            XmodsEnums.Age age = (XmodsEnums.Age)tmp;
            tmp = 0;
            for (int i = 0; i < GenderValues.Length; i++)
            {
                if (ClonePropGender_checkedListBox.GetItemChecked(i)) tmp += GenderValues[i];
            }
            XmodsEnums.Gender gender = (XmodsEnums.Gender)tmp;

            if (importedcolorShiftMask)
            {
                IResourceKey ik = null;
                if (clonedColorShiftMask)
                {
                    DeleteResource(clonePack, iresColorShiftMask);
                    ik = new TGIBlock(0, null, iresColorShiftMask.ResourceType, iresColorShiftMask.ResourceGroup, iresColorShiftMask.Instance);
                }
                else
                {
                    if (myColorShiftMask != null)
                    {
                        TGI newtgi = new TGI((uint)XmodsEnums.ResourceTypes.DXT5RLE2, 0x80000000U, Xmods.DataLib.FNVhash.FNV64(meshName + "_colorShiftMask") | 0x8000000000000000);
                        for (int j = 0; j < clonePackCASPs.Count; j++)
                        {
                            if (clonePackCASPs[j].Casp.ColorShiftMaskIndex == clonePackCASPs[j].Casp.EmptyLink)
                            {
                                clonePackCASPs[j].Casp.ColorShiftMaskIndex = clonePackCASPs[j].Casp.addLink(newtgi);
                            }
                            else
                            {
                                clonePackCASPs[j].Casp.setLink(clonePackCASPs[j].Casp.ColorShiftMaskIndex, newtgi);
                            }
                        }
                        ik = new TGIBlock(0, null, newtgi.Type, newtgi.Group, newtgi.Instance);
                        clonedColorShiftMask = true;
                    }
                }
                if (myColorShiftMask == null)
                {
                    for (int j = 0; j < clonePackCASPs.Count; j++)
                    {
                        clonePackCASPs[j].Casp.RemoveColorShiftMask();
                    }
                    clonedColorShiftMask = false;
                }
                else //if (myColorShiftMask != null)
                {
                    iresColorShiftMask = clonePack.AddResource(ik, myColorShiftMask.Stream, false);
                    iresColorShiftMask.Compressed = (ushort)0x5A42;
                }
                importedcolorShiftMask = false;
            }
            if (importedShadow)
            {
                if (myShadow != null)
                {
                    IResourceKey ik = null;
                    if (clonedShadow)
                    {
                        DeleteResource(clonePack, iresShadow);
                        ik = new TGIBlock(0, null, iresShadow.ResourceType, iresShadow.ResourceGroup, iresShadow.Instance);
                    }
                    else
                    {
                        TGI newtgi = new TGI((uint)XmodsEnums.ResourceTypes.DXT5RLE2,
                            0x80000000U, FNVhash.FNV64(clonePartName + "_shadow") | 0x8000000000000000);
                        ik = new TGIBlock(0, null, newtgi.Type, newtgi.Group, newtgi.Instance);
                        for (int i = 0; i < clonePackCASPs.Count; i++)
                        {
                            if (clonePackCASPs[i].Casp.ShadowIndex == clonePackCASPs[i].Casp.EmptyLink)
                            {
                                clonePackCASPs[i].Casp.ShadowIndex = clonePackCASPs[i].Casp.addLink(newtgi);
                            }
                            else
                            {
                                clonePackCASPs[i].Casp.LinkList[clonePackCASPs[i].Casp.ShadowIndex] = newtgi;
                            }
                        }
                        clonedShadow = true;
                    }
                    iresShadow = clonePack.AddResource(ik, myShadow.Stream, true);
                    iresShadow.Compressed = (ushort)0x5A42;
                    importedShadow = false;
                }
            }

            if (importedSpecular)
            {
                if (mySpecular != null)
                {
                    IResourceKey ik = null;
                    if (clonedSpecular)
                    {
                        DeleteResource(clonePack, iresSpecular);
                        ik = new TGIBlock(0, null, iresSpecular.ResourceType, iresSpecular.ResourceGroup, iresSpecular.Instance);
                    }
                    else
                    {
                        TGI newtgi = new TGI((uint)XmodsEnums.ResourceTypes.DXT5RLES,
                            0x80000000U, FNVhash.FNV64(clonePartName + "_specular") | 0x8000000000000000);
                        ik = new TGIBlock(0, null, newtgi.Type, newtgi.Group, newtgi.Instance);
                        for (int i = 0; i < clonePackCASPs.Count; i++)
                        {
                            if (clonePackCASPs[i].Casp.SpecularIndex == clonePackCASPs[i].Casp.EmptyLink)
                            {
                                clonePackCASPs[i].Casp.SpecularIndex = clonePackCASPs[i].Casp.addLink(newtgi);
                            }
                            else
                            {
                                clonePackCASPs[i].Casp.LinkList[clonePackCASPs[i].Casp.SpecularIndex] = newtgi;
                            }
                        }
                        clonedSpecular = true;
                    }
                    iresSpecular = clonePack.AddResource(ik, mySpecular.Stream, true);
                    iresSpecular.Compressed = (ushort)0x5A42;
                    importedSpecular = false;
                }
            }

            bool updateMeshes = importedBumpMap | importedGlow;
            if (importedBumpMap)
            {
                if (meshBumpMap == null) return;
                IResourceKey ik = null;
                if (clonedBumpMap)
                {
                    DeleteResource(clonePack, iresBumpMap);
                    ik = new TGIBlock(0, null, iresBumpMap.ResourceType, iresBumpMap.ResourceGroup, iresBumpMap.Instance);
                }
                else
                {
                    TGI newtgi = new TGI((uint)XmodsEnums.ResourceTypes.DDS, 0x80000000U, Xmods.DataLib.FNVhash.FNV64(meshName + "_bumpmap") | 0x8000000000000000);
                    for (int j = 0; j < clonePackMeshes.Count; j++)
                    {
                        clonePackMeshes[j].SetNormalMap(newtgi, myCASP.Species == XmodsEnums.Species.Human);
                        newMeshImport[j] = true;
                    }
                    ik = new TGIBlock(0, null, newtgi.Type, newtgi.Group, newtgi.Instance);
                    clonedBumpMap = true;
                }
                iresBumpMap = clonePack.AddResource(ik, meshBumpMap.Stream, false);
                iresBumpMap.Compressed = (ushort)0x5A42;
                importedBumpMap = false;
            }

            if (importedGlow)
            {
                IResourceKey ik = null;
                if (clonedGlow)
                {
                    DeleteResource(clonePack, iresGlow);
                    ik = new TGIBlock(0, null, iresGlow.ResourceType, iresGlow.ResourceGroup, iresGlow.Instance);
                }
                else
                {
                    if (meshGlowMap != null)
                    {
                        TGI newtgi = new TGI((uint)XmodsEnums.ResourceTypes.DDS, 0x80000000U, Xmods.DataLib.FNVhash.FNV64(meshName + "_emissionmap") | 0x8000000000000000);
                        for (int j = 0; j < clonePackMeshes.Count; j++)
                        {
                            clonePackMeshes[j].SetEmissionMap(newtgi);
                            newMeshImport[j] = true;
                        }
                        for (int j = 0; j < clonePackCASPs.Count; j++)
                        {
                            if (clonePackCASPs[j].Casp.EmissionIndex == clonePackCASPs[j].Casp.EmptyLink)
                            {
                                clonePackCASPs[j].Casp.EmissionIndex = clonePackCASPs[j].Casp.addLink(newtgi);
                            }
                            else
                            {
                                clonePackCASPs[j].Casp.setLink(clonePackCASPs[j].Casp.EmissionIndex, newtgi);
                            }
                        }
                        ik = new TGIBlock(0, null, newtgi.Type, newtgi.Group, newtgi.Instance);
                        clonedGlow = true;
                    }
                }
                if (meshGlowMap == null)
                {
                    for (int j = 0; j < clonePackMeshes.Count; j++)
                    {
                        clonePackMeshes[j].RemoveEmissionMap();
                        newMeshImport[j] = true;
                    }
                    for (int j = 0; j < clonePackCASPs.Count; j++)
                    {
                        clonePackCASPs[j].Casp.RemoveEmission();
                    }
                    clonedGlow = false;
                }
                else //if (meshGlowMap != null)
                {
                    iresGlow = clonePack.AddResource(ik, meshGlowMap.Stream, false);
                    iresGlow.Compressed = (ushort)0x5A42;
                }
                importedGlow = false;
            }

            if (updateMeshes) MeshWriteToPackage();
            if (changesRegionMap) RegionMapWriteToPackage();
            if (updateMeshes)
            {
                GetClonePackMeshes();
                ListClonePackMeshes();
                ListClonePackRegions();
            }

            for (int i = 0; i < clonePackCASPs.Count; i++)
            {
                clonePackCASPs[i].Casp.OutfitID = propertyID;
                List<uint[]> flags = UpdatePropertyFlags(clonePackCASPs[i].Casp.CategoryTags, modifiedFlags, true);
                clonePackCASPs[i].Casp.CategoryTags = flags;
                clonePackCASPs[i].Casp.SortPriority = sortOrder;
                clonePackCASPs[i].Casp.FlagAllowForCASRandom = flagAllowForCASRandom;
                clonePackCASPs[i].Casp.FlagAllowForLiveRandom = flagAllowForLiveRandom;
                clonePackCASPs[i].Casp.FlagShowInUI = flagShowInUI;
                clonePackCASPs[i].Casp.FlagShowInSimInfoPanel = flagShowInInfoPanel;
                clonePackCASPs[i].Casp.FlagRestrictOppositeGender = flagRestrictOppositeGender;
                clonePackCASPs[i].Casp.FlagRestrictOppositeFrame = flagRestrictOppositeFrame;
                clonePackCASPs[i].Casp.FlagCreateInGame = flagCreateInGame;
                clonePackCASPs[i].Casp.OccultDisableForHuman = flagDisableForHuman;
                clonePackCASPs[i].Casp.OccultDisableForAlien = flagDisableForAlien;
                clonePackCASPs[i].Casp.OccultDisableForVampire = flagDisableForVampire;
                clonePackCASPs[i].Casp.OccultDisableForMermaid = flagDisableForMermaid;
                clonePackCASPs[i].Casp.OccultDisableForWitch = flagDisableForSpellcaster;
                clonePackCASPs[i].Casp.OccultDisableForWerewolf = flagDisableForWerewolf;
                clonePackCASPs[i].Casp.BodyType = bodyType;
                clonePackCASPs[i].Casp.SharedUVSpace = UVSpace;
                clonePackCASPs[i].Casp.BodySubType = bodySubType;
                clonePackCASPs[i].Casp.Species = species;
                clonePackCASPs[i].Casp.Age = age;
                clonePackCASPs[i].Casp.Gender = gender;
                clonePackCASPs[i].Casp.CompositionMethod = (int)ClonePropCompMethod.Value;
                clonePackCASPs[i].Casp.CreateDescriptionKey = CreateDesc;
                clonePackCASPs[i].Casp.SortLayer = sortLayer;
                clonePackCASPs[i].Casp.ExcludePartFlags = excludeFlags;
                clonePackCASPs[i].Casp.ExcludePartFlags2 = excludeFlags2;
                clonePackCASPs[i].Casp.ExcludeModifierRegionFlags = excludeModifierFlags;
                DeleteResource(clonePack, iresCASPs[i]);
                Stream s = new MemoryStream();
                BinaryWriter bw = new BinaryWriter(s);
                clonePackCASPs[i].Casp.Write(bw);
                clonePack.AddResource(iresCASPs[i], s, true);
            }

            changesGeneral = false;
            StartPreview(myCASP.PartName);
        }

        private void ClonePropLegacy_checkBox_CheckedChanged(object sender, EventArgs e)
        {
            if (ClonePropLegacy_checkBox.Checked)
            {
                DialogResult res = MessageBox.Show("Are you sure?" + Environment.NewLine +
                    "Any slider settings will not be saved.",
                    "Confirm change to Legacy", MessageBoxButtons.OKCancel);
                if (res != DialogResult.OK) return;
                //ClonePropSliders_checkBox.Checked = false;
            }
            if (ClonePropLegacy_checkBox.Checked && ClonePropFlags_checkedListBox.GetItemChecked(11))
            {
                DialogResult res = MessageBox.Show("Are you sure?" + Environment.NewLine + 
                    "The flag making this item 'Create in Game'" + Environment.NewLine + "is currently checked and will not be saved.",
                    "Confirm change to Legacy", MessageBoxButtons.OKCancel);
                if (res != DialogResult.OK) return;
            }
            ClonePropCreate_comboBox.Enabled = !ClonePropLegacy_checkBox.Checked;
            if (clonePackCASPs != null)
            {
                for (int i = 0; i < clonePackCASPs.Count; i++)
                {
                    if (clonePackCASPs[i].Casp.UpdateToLatestVersion(ClonePropLegacy_checkBox.Checked))
                    {
                        MemoryStream sw = new MemoryStream();
                        BinaryWriter bw = new BinaryWriter(sw);
                        clonePackCASPs[i].Casp.Write(bw);
                        sw.Position = 0;
                        ReplaceResource(clonePack, iresCASPs[i], sw);
                    }
                    //ClonePropCommit();
                    //changesGeneral = true;
                }
            }
        }

        //private void ClonePropSliders_checkBox_CheckedChanged(object sender, EventArgs e)
        //{
        //    if (ClonePropSliders_checkBox.Checked)
        //    {
        //        ClonePropLegacy_checkBox.Checked = false;
        //        SliderSettings_panel.Enabled = true;
        //    }
        //    else
        //    {
        //        DialogResult res = MessageBox.Show("Are you sure?" + Environment.NewLine +
        //            "Any slider settings will not be saved.",
        //            "Confirm change to no slider support", MessageBoxButtons.OKCancel);
        //        if (res != DialogResult.OK) return;
        //        SliderSettings_panel.Enabled = false;
        //    }
        //    if (clonePackCASPs != null)
        //    {
        //        for (int i = 0; i < clonePackCASPs.Count; i++)
        //        {
        //            clonePackCASPs[i].Casp.UpdateToLatestVersion(ClonePropLegacy_checkBox.Checked);
        //            changesGeneral = true;
        //        }
        //    }
        //}

        private void CloneSlider_TextChanged(object sender, EventArgs e)
        {
            changesSliders = true;
        }

        private void SortLayerHelp_button_Click(object sender, EventArgs e)
        {
            LayerHelp();
        }

        public static void LayerHelp()
        {
            MessageBox.Show("Standard Layer Values:" + Environment.NewLine +
                            "    0 : Earrings/Glasses/Some gloves" + Environment.NewLine +
                            " 1000 : Head/Teeth" + Environment.NewLine +
                            " 1100 : Skin overlay" + Environment.NewLine +
                            " 2100 : Freckles/Moles/Alien and special makeup" + Environment.NewLine +
                            " 2200 : Skin crease" + Environment.NewLine +
                            " 3000 : Tattoo" + Environment.NewLine +
                            " 4000 : Eye color" + Environment.NewLine +
                            " 5200 : Eye shadow" + Environment.NewLine +
                            " 5300 : Eye Liner" + Environment.NewLine +
                            " 5500 : Lipstick" + Environment.NewLine +
                            " 5600 : Blush" + Environment.NewLine +
                            " 7000 : Eyebrows" + Environment.NewLine +
                            " 8300 : Facial hair" + Environment.NewLine +
                            " 7500 : Happy/Sad facepaint" + Environment.NewLine +
                            " 9000 : Facepaint" + Environment.NewLine +
                            "10100 : Underwear (child)" + Environment.NewLine +
                            "10300 : Skinny jeans/Jeggings" + Environment.NewLine +
                            "10400 : Underwear (adult)/Some gloves" + Environment.NewLine +
                            "10500 : Tights" + Environment.NewLine +
                            "10600 : Socks" + Environment.NewLine +
                            "10700 : Shoes" + Environment.NewLine +
                            "11000 : Hat/Spiked and pony tail hair" + Environment.NewLine +
                            "12000 : Hair" + Environment.NewLine +
                            "13000 : Tucked Top" + Environment.NewLine +
                            "14000 : Bottoms" + Environment.NewLine +
                            "16000 : Body/Untucked Top" + Environment.NewLine +
                            "17100 : Necklace/Rings" + Environment.NewLine +
                            "17200 : Bracelet"
                            );
        }

        private void ClonePropDiscard_button_Click(object sender, EventArgs e)
        {
            DialogResult res = MessageBox.Show("Are you sure you want to discard changes?", "Discard Changes", MessageBoxButtons.OKCancel);
            if (res == DialogResult.Cancel) return;
            ShowClonePackProperties();
            changesGeneral = false;
        }

        private void CloneCASPsList(bool selectFirst)
        {
            CloneCASPlist_dataGridView.Rows.Clear();
            int index = 0;
            foreach (CASPinfo c in clonePackCASPs)
            {
                CloneCASPlist_dataGridView.Rows.Add(new string[] { c.Casp.PartName, c.Casp.SwatchOrder.ToString().PadLeft(3) });
                CloneCASPlist_dataGridView.Rows[CloneCASPlist_dataGridView.Rows.Count - 1].Tag = index;
                index++;
            }
            CloneCASPlist_dataGridView.Sort(CloneCASPlist_dataGridView.Columns["CASPorder"], ListSortDirection.Ascending);
            for (int i = 0; i < CloneCASPlist_dataGridView.Rows.Count; i++)
            {
                CloneCASPlist_dataGridView.Rows[i].Cells[0].Selected = false;
            }
            if (selectFirst)
            {
                myRow = 0;
                myCASPindex = (int)CloneCASPlist_dataGridView.Rows[0].Tag;
                myCASP = clonePackCASPs[myCASPindex].Casp;
                myKey = iresCASPs[myCASPindex];
            }
            else
            {
                myCASPindex = (int)CloneCASPlist_dataGridView.Rows[myRow].Tag;
                myCASP = clonePackCASPs[myCASPindex].Casp;
                myKey = iresCASPs[myCASPindex];
            }
            CloneCASPlist_dataGridView.Rows[myRow].Cells[0].Selected = true;
            CloneCASPdisplay();
        }

        private void CloneCASPlist_dataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                if (myCASPindex != (int)CloneCASPlist_dataGridView.Rows[e.RowIndex].Tag)
                {
                    if (changesRecolor)
                    {
                        DialogResult res = MessageBox.Show("Any uncommitted changes will be lost if you move to another recolor!",
                                                            "Unsaved Changes", MessageBoxButtons.OKCancel);
                        if (res == DialogResult.Cancel)
                        {
                            CloneCASPlist_dataGridView.Rows[myRow].Cells[0].Selected = true;
                            CloneCASPlist_dataGridView.Rows[e.RowIndex].Cells[0].Selected = false;
                            return;
                        }
                    }
                    myCASPindex = (int)CloneCASPlist_dataGridView.Rows[e.RowIndex].Tag;
                    myCASP = clonePackCASPs[myCASPindex].Casp;
                    myKey = iresCASPs[myCASPindex];
                    myRow = e.RowIndex;
                    CloneCASPlist_dataGridView.Rows[myRow].Cells[0].Selected = true;
                    CloneCASPdisplay();
                    StartPreview(CloneColorName.Text);
                    changesRecolor = false;
                }

                if (e.ColumnIndex == CloneCASPlist_dataGridView.Columns["CASPdelete"].Index)
                {
                    if (clonePackCASPs.Count <= 1)
                    {
                        MessageBox.Show("You can't delete all the recolors in the package!");
                        return;
                    }
                    DialogResult res = MessageBox.Show("Okay to permanently delete this recolor? This cannot be undone!", "Delete Recolor", MessageBoxButtons.OKCancel);
                    if (res == DialogResult.OK)
                    {
                        TGI tex = myCASP.LinkList[myCASP.TextureIndex];
                        IResourceKey tmp = new TGIBlock(0, null, tex.Type, tex.Group, tex.Instance);
                        DeleteResource(clonePack, tmp);
                        DeleteResource(clonePack, myKey);
                        DeleteResource(clonePack, new TGIBlock(0, null, (uint)XmodsEnums.ResourceTypes.THUM, 0x00000102U, myKey.Instance));
                        DeleteResource(clonePack, new TGIBlock(0, null, (uint)XmodsEnums.ResourceTypes.THUM, 0x00000002U, myKey.Instance));
                        clonePackCASPs.RemoveAt(myCASPindex);
                        iresCASPs.RemoveAt(myCASPindex);
                        myRow = Math.Min(myRow, CloneCASPlist_dataGridView.Rows.Count - 2);
                        CloneCASPsList(false);
                        CloneThumbsList();
                    }
                    changesRecolor = false;
                }
            }
        }

        private void CloneCASPdisplay()
        {
            cloneWait_label.Visible = true;
            cloneWait_label.Refresh();
            CloneColorProp_dataGridView.CellValueChanged -= CloneFlags_dataGridView_CellValueChanged;
            CloneColorName.TextChanged -= CloneColorName_TextChanged;
            CloneColorSortOrder.TextChanged -= CloneColorSortOrder_TextChanged;
            CloneColorDefaultThumb_checkBox.CheckedChanged -= CloneColorDefaultThumb_checkBox_CheckedChanged;
            CloneColorDefaultBodyTypeMale_checkBox.CheckedChanged -= CloneColorDefaultBodyTypeMale_checkBox_CheckedChanged;
            CloneColorDefaultBodyTypeFemale_checkBox.CheckedChanged -= CloneColorDefaultBodyTypeFemale_checkBox_CheckedChanged;
            CloneColorOppositeGenderPart.TextChanged -= CloneColorOppositeGenderPart_TextChanged;
            CloneColorFallbackPart.TextChanged -= CloneColorFallbackPart_TextChanged;
            List<uint[]> catFlags = myCASP.CategoryTags;
            CloneColorProp_dataGridView.Rows.Clear();
            ListCASPflags(CloneColorProp_dataGridView, catFlags, tagCategoryNamesInd, tagCategoryNumbersInd);
            CloneColorVersion.Text = "V0x" + myCASP.Version.ToString("X2");
            CloneColorThumb_pictureBox.Image = clonePackCASPs[myCASPindex].maleThumb != null ? clonePackCASPs[myCASPindex].maleThumb.Image : 
                                                (clonePackCASPs[myCASPindex].femaleThumb != null ? clonePackCASPs[myCASPindex].femaleThumb.Image : Properties.Resources.NullImage);
            mySwatch = FindCloneTextureDST(myCASP.LinkList, myCASP.SwatchIndex, out iresSwatch, out clonedSwatch);
            if (mySwatch != null)
            {
                ddsSwatch = new DdsFile();
                ddsSwatch.Load(mySwatch.ToDDS(), true);
                CloneColorSwatch_pictureBox.Image = new Bitmap(ddsSwatch.Image);
                CloneColorSwatch_pictureBox.Refresh();
            }
            else
            {
                ddsSwatch = null;
                CloneColorSwatch_pictureBox.Image = Properties.Resources.NullImage;
            }
            myTexture = FindCloneTexture(myCASP.LinkList[myCASP.TextureIndex], out iresTexture, out clonedTexture);
            if (myTexture != null)
            {
                if (myTexture is AResource ar)
                {
                    ddsTexture = new DdsFile();
                    ddsTexture.Load(ar.ToDDS(), false);
                    textureImage = new Bitmap(ddsTexture.Image);
                    CloneColorTexture_pictureBox.Image = textureImage;
                }
                else if (myTexture is LRLE)
                {
                    ddsTexture = null;
                    textureImage = new Bitmap((myTexture as LRLE).image);
                    CloneColorTexture_pictureBox.Image = textureImage;
                }
            }
            else
            {
                ddsTexture = null;
                textureImage = null;
                CloneColorTexture_pictureBox.Image = Properties.Resources.NullImage;
            }
            CloneColorName.Text = myCASP.PartName;
            CloneColorSortOrder.Text = myCASP.SwatchOrder.ToString();
            CloneColorDefaultBodyTypeMale_checkBox.Checked = myCASP.FlagDefaultForBodyTypeMale;
            CloneColorDefaultBodyTypeFemale_checkBox.Checked = myCASP.FlagDefaultForBodyTypeFemale;
            CloneColorDefaultThumb_checkBox.Checked = myCASP.FlagDefaultForThumbnail;
            CloneColorOppositeGenderPart.Text = myCASP.OppositeGenderPart > 0 ? myCASP.OppositeGenderPart.ToString("X16") : "";
            CloneColorFallbackPart.Text = myCASP.FallbackPart > 0 ? myCASP.FallbackPart.ToString("X16") : "";
            CloneColor1_panel.Visible = myCASP.ColorList.Length > 0;
            CloneColor2_panel.Visible = myCASP.ColorList.Length > 1;
            CloneColor3_panel.Visible = myCASP.ColorList.Length > 2;
            CloneColorAdd_button.Visible = myCASP.ColorList.Length < 3;
            if (myCASP.ColorList.Length > 0) CloneColor1_pictureBox.BackColor = Color.FromArgb((int)myCASP.ColorList[0]);
            if (myCASP.ColorList.Length > 1) CloneColor2_pictureBox.BackColor = Color.FromArgb((int)myCASP.ColorList[1]);
            if (myCASP.ColorList.Length > 2) CloneColor3_pictureBox.BackColor = Color.FromArgb((int)myCASP.ColorList[2]);
            CloneColorProp_dataGridView.CellValueChanged += new DataGridViewCellEventHandler(CloneFlags_dataGridView_CellValueChanged);
            CloneColorName.TextChanged += CloneColorName_TextChanged;
            CloneColorSortOrder.TextChanged += CloneColorSortOrder_TextChanged;
            CloneColorDefaultThumb_checkBox.CheckedChanged += CloneColorDefaultThumb_checkBox_CheckedChanged;
            CloneColorDefaultBodyTypeMale_checkBox.CheckedChanged += CloneColorDefaultBodyTypeMale_checkBox_CheckedChanged;
            CloneColorDefaultBodyTypeFemale_checkBox.CheckedChanged += CloneColorDefaultBodyTypeFemale_checkBox_CheckedChanged;
            CloneColorOppositeGenderPart.TextChanged += CloneColorOppositeGenderPart_TextChanged;
            CloneColorFallbackPart.TextChanged += CloneColorFallbackPart_TextChanged;
            ShowSliderSetup(myCASP.OpacitySliderSettings, CloneSliderOpacityMinimum, null, CloneSliderOpacityIncrement);
            ShowSliderSetup(myCASP.HueSliderSettings, CloneSliderHueMinimum, CloneSliderHueMaximum, CloneSliderHueIncrement);
            ShowSliderSetup(myCASP.SaturationSliderSettings, CloneSliderSaturationMinimum, CloneSliderSaturationMaximum, CloneSliderSaturationIncrement);
            ShowSliderSetup(myCASP.BrightnessSliderSettings, CloneSliderBrightnessMinimum, CloneSliderBrightnessMaximum, CloneSliderBrightnessIncrement);
            changesSliders = false;
            changesRecolor = false;
            ShowHairColorSetup(myCASP);
            cloneWait_label.Visible = false;
            cloneWait_label.Refresh();

        }

        private void CloneColorAddFlag_button_Click(object sender, EventArgs e)                                                                                                             
        {
            CloneAddFlag_button_Click(CloneColorProp_dataGridView, e, false);
            changesRecolor = true;
        }

        private void CloneColorProp_dataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            CloneFlags_dataGridView_CellContentClick(sender, e);
        }

        private void CloneCASPFlags_dataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            CloneFlags_dataGridView_CellValueChanged(sender, e);
        }

        private void CloneCASPAddRecolor_button_Click(object sender, EventArgs e)
        {
            DialogResult res = MessageBox.Show("Copy properties and colors from currently selected recolor?", "Copy Properties Prompt", MessageBoxButtons.YesNoCancel);
            if (res == DialogResult.Cancel) return;
            Stream ms = new MemoryStream();
            BinaryWriter bw = new BinaryWriter(ms);
            myCASP.Write(bw);
            ms.Position = 0;
            BinaryReader br = new BinaryReader(ms);
            CASP newCASP = new CASP(br);
            ms.Dispose();
            int ind = newCASP.PartName.LastIndexOf("_");
            if (ind > 0)
            {
                string tmp = newCASP.PartName.Substring(0, ind);
                newCASP.PartName = tmp + "_Recolor" + ran.Next().ToString();
            }
            else
            {
                newCASP.PartName += "_Recolor" + ran.Next().ToString();
            }
            if (res == DialogResult.No)
            {
                newCASP.CategoryTags = UpdatePropertyFlags(newCASP.CategoryTags, new List<uint[]>(), false);
                newCASP.ColorList = new uint[] { 0 };
            }
            TGI newtgi = new TGI((uint)XmodsEnums.ResourceTypes.DXT5RLE2,
                            0x80000000U, FNVhash.FNV64(newCASP.PartName) | 0x8000000000000000);
            if (newCASP.TextureIndex == newCASP.EmptyLink)
            {
                newCASP.TextureIndex = newCASP.addLink(newtgi);
            }
            else
            {
                newCASP.LinkList[newCASP.TextureIndex] = newtgi;
            }
            clonedTexture = true;
            if (myTexture is RLEResource)
            {
                IResourceKey ik = new TGIBlock(0, null, newtgi.Type, newtgi.Group, newtgi.Instance);
                iresTexture = clonePack.AddResource(ik, (myTexture as RLEResource).Stream, true);
            }else if (myTexture is DSTResource)
            {
                IResourceKey ik = new TGIBlock(0, null, newtgi.Type, newtgi.Group, newtgi.Instance);
                iresTexture = clonePack.AddResource(ik, (myTexture as DSTResource).Stream, true);
            }
            else if (myTexture is LRLE)
            {
                IResourceKey ik = new TGIBlock(0, null, (uint)XmodsEnums.ResourceTypes.LRLE, newtgi.Group, newtgi.Instance);
                iresTexture = clonePack.AddResource(ik, (myTexture as LRLE).Stream, true);
            }
            iresTexture.Compressed = (ushort)0x5A42;
            importedTexture = false;
            newCASP.SwatchIndex = (byte)newCASP.EmptyLink;
            clonePackCASPs.Add(new CASPinfo(newCASP, null, null));
            ms = new MemoryStream();
            bw = new BinaryWriter(ms);
            newCASP.Write(bw);
            ms.Position = 0;
            IResourceKey rk = new TGIBlock(0, null, (uint)XmodsEnums.ResourceTypes.CASP, 0x80000000U, 
                Xmods.DataLib.FNVhash.FNV32(newCASP.PartName) | 0x8000000000000000U);
            myKey = clonePack.AddResource(rk, ms, true);
            iresCASPs.Add(myKey);
            myCASP = newCASP;
            int i = CloneCASPlist_dataGridView.Rows.Add(new object[] { newCASP.PartName, newCASP.SwatchOrder.ToString().PadLeft(3) });
            CloneCASPlist_dataGridView.Rows[i].Tag = clonePackCASPs.Count - 1;
            CloneCASPlist_dataGridView.Rows[myRow].Selected = false;
            CloneCASPlist_dataGridView.Rows[i].Selected = true;
            object[] obj = new object[4] { newCASP.PartName, Properties.Resources.NullImage, Properties.Resources.NullImage, newCASP.SwatchOrder.ToString().PadLeft(3) };
            int ind2 = CloneThumbs_dataGridView.Rows.Add(obj);
            CloneThumbs_dataGridView.Rows[ind2].Tag = ind2;
            CloneThumbs_dataGridView.Rows[ind2].Cells["CASPthumbMale"].Tag = null;
            CloneThumbs_dataGridView.Rows[ind2].Cells["CASPthumbFemale"].Tag = null;
            myRow = i;
            myCASPindex = clonePackCASPs.Count - 1;
            CloneCASPdisplay();
            StartPreview(CloneColorName.Text);
        }

        private void CloneCASPRenumber_button_Click(object sender, EventArgs e)
        {
            if (changesRecolor)
            {
                MessageBox.Show("You have unsaved changes - please commit or discard changes first!");
                return;
            }
            RenumberSortOrder renum = new RenumberSortOrder();
            DialogResult res = renum.ShowDialog();
            if (res == DialogResult.OK)
            {
                bool increments = renum.RenumberIncrement != 0;
                ushort sortVal = (ushort)renum.RenumberValue;
                for (int i = 0; i < CloneCASPlist_dataGridView.Rows.Count; i++)
                {
                    int index = (int)CloneCASPlist_dataGridView.Rows[i].Tag;
                    int tmp = sortVal + (increments ? renum.RenumberIncrement * i : clonePackCASPs[index].Casp.SwatchOrder);
                    if (tmp < 0) tmp = 0;
                    if (tmp > UInt16.MaxValue) tmp = UInt16.MaxValue;
                    ushort newSortVal = (ushort)tmp;
                    clonePackCASPs[index].Casp.SwatchOrder = newSortVal;
                    MemoryStream ms = new MemoryStream();
                    BinaryWriter bw = new BinaryWriter(ms);
                    clonePackCASPs[index].Casp.Write(bw);
                    ReplaceResource(clonePack, iresCASPs[index], ms);
                }

                CloneCASPsList(false);
            }
        }

        private void CloneColor_pictureBox_Click(object sender, EventArgs e)
        {
            PictureBox clonePbox = sender as PictureBox;
            ColorDialog cd = new ColorDialog();
            cd.FullOpen = true;
            cd.Color = clonePbox.BackColor;
            if (cd.ShowDialog() == DialogResult.OK)
            {
                clonePbox.BackColor = cd.Color;
                changesRecolor = true;
            }
        }

        private void CloneColorAdd_button_Click(object sender, EventArgs e)
        {
            if (!CloneColor1_panel.Visible)
            {
                CloneColor1_pictureBox.BackColor = Color.Transparent;
                CloneColor1_panel.Visible = true;
            }
            else if (!CloneColor2_panel.Visible)
            {
                CloneColor2_pictureBox.BackColor = Color.Transparent;
                CloneColor2_panel.Visible = true;
            }
            else if (!CloneColor3_panel.Visible)
            {
                CloneColor3_pictureBox.BackColor = Color.Transparent;
                CloneColor3_panel.Visible = true;
                CloneColorAdd_button.Visible = false;
            }
            changesRecolor = true;
        }

        private void CloneColor2Delete_button_Click(object sender, EventArgs e)
        {
            if (CloneColor3_panel.Visible)
            {
                CloneColor2_pictureBox.BackColor = CloneColor3_pictureBox.BackColor;
                CloneColor3_panel.Visible = false;
            }
            else
            {
                CloneColor2_panel.Visible = false;
            }
            CloneColorAdd_button.Visible = true;
            changesRecolor = true;
        }

        private void CloneColor3Delete_button_Click(object sender, EventArgs e)
        {
            CloneColor3_panel.Visible = false;
            CloneColorAdd_button.Visible = true;
            changesRecolor = true;
        }

        private void CloneColorName_TextChanged(object sender, EventArgs e)
        {
            changesRecolor = true;
        }

        private void CloneColorSortOrder_TextChanged(object sender, EventArgs e)
        {
            changesRecolor = true;
        }

        private void CloneColorDefaultThumb_checkBox_CheckedChanged(object sender, EventArgs e)
        {
            changesRecolor = true;
        }

        private void CloneColorDefaultBodyTypeMale_checkBox_CheckedChanged(object sender, EventArgs e)
        {
            changesRecolor = true;
        }

        private void CloneColorDefaultBodyTypeFemale_checkBox_CheckedChanged(object sender, EventArgs e)
        {
            changesRecolor = true;
        }

        private void CloneColorOppositeGenderPart_TextChanged(object sender, EventArgs e)
        {
            changesRecolor = true;
        }

        private void CloneColorFallbackPart_TextChanged(object sender, EventArgs e)
        {
            changesRecolor = true;
        }

        private void CloneCASPCommit_button_Click(object sender, EventArgs e)
        {
            CloneCASPCommit();
        }

        private void CloneCASPCommit()
        {
            if (myCASP == null) return;
            try
            {
                myCASP.SwatchOrder = ushort.Parse(CloneColorSortOrder.Text);
            }
            catch
            {
                MessageBox.Show("Please enter a valid number in the Sort Order box!");
                return;
            }
            if (string.Compare(CloneColorOppositeGenderPart.Text, " ") > 0)
            {
                try
                {
                    myCASP.OppositeGenderPart = UInt64.Parse(CloneColorOppositeGenderPart.Text.Replace("0x", "").Replace("0X", ""), System.Globalization.NumberStyles.HexNumber);
                }
                catch
                {
                    MessageBox.Show("Please enter a valid Instance ID in the Opposite Gender Part box!");
                    return;
                }
            }
            else
            {
                myCASP.OppositeGenderPart = 0;
            }
            if (string.Compare(CloneColorFallbackPart.Text, " ") > 0)
            {
                try
                {
                    myCASP.FallbackPart = UInt64.Parse(CloneColorFallbackPart.Text.Replace("0x", "").Replace("0X", ""), System.Globalization.NumberStyles.HexNumber);
                }
                catch
                {
                    MessageBox.Show("Please enter a valid Instance ID in the Fallback Part box!");
                    return;
                }
            }
            else
            {
                myCASP.FallbackPart = 0;
            }
            List<uint[]> flags = UpdatePropertyFlags(myCASP.CategoryTags, ReadPropertyFlags(CloneColorProp_dataGridView), false);
            myCASP.CategoryTags = flags;
            myCASP.PartName = CloneColorName.Text;
            myCASP.FlagDefaultForBodyTypeMale = CloneColorDefaultBodyTypeMale_checkBox.Checked;
            myCASP.FlagDefaultForBodyTypeFemale = CloneColorDefaultBodyTypeFemale_checkBox.Checked;
            myCASP.FlagDefaultForThumbnail = CloneColorDefaultThumb_checkBox.Checked;
            List<uint> colors = new List<uint>();
            if (CloneColor1_panel.Visible) colors.Add((uint)CloneColor1_pictureBox.BackColor.ToArgb());
            if (CloneColor2_panel.Visible) colors.Add((uint)CloneColor2_pictureBox.BackColor.ToArgb());
            if (CloneColor3_panel.Visible) colors.Add((uint)CloneColor3_pictureBox.BackColor.ToArgb());
            myCASP.ColorList = colors.ToArray();

            if (importedTexture)
            {
                if (myTexture != null)
                {
                    IResourceKey ik = null;
                    if (clonedTexture)
                    {
                        DeleteResource(clonePack, iresTexture);
                        ik = new TGIBlock(0, null, iresTexture.ResourceType, iresTexture.ResourceGroup, iresTexture.Instance);
                    }
                    else
                    {
                        TGI newtgi = new TGI((uint)XmodsEnums.ResourceTypes.DXT5RLE2,
                            0x80000000U, FNVhash.FNV64(myCASP.PartName) | 0x8000000000000000);
                        ik = new TGIBlock(0, null, newtgi.Type, newtgi.Group, newtgi.Instance);
                        if (myCASP.TextureIndex == myCASP.EmptyLink)
                        {
                            myCASP.TextureIndex = myCASP.addLink(newtgi);
                        }
                        else
                        {
                            myCASP.LinkList[myCASP.TextureIndex] = newtgi;
                        }
                        clonedTexture = true;
                    }
                    if (myTexture is RLEResource)
                    {
                        iresTexture = clonePack.AddResource(ik, (myTexture as RLEResource).Stream, true);
                    }else if (myTexture is DSTResource)
                    {
                        iresTexture = clonePack.AddResource(ik, (myTexture as DSTResource).Stream, true);
                    }
                    else if (myTexture is LRLE)
                    {
                        ik.ResourceType = (uint)XmodsEnums.ResourceTypes.LRLE;
                        iresTexture = clonePack.AddResource(ik, (myTexture as LRLE).Stream, true);
                    }                  
                    iresTexture.Compressed = (ushort)0x5A42;
                    importedTexture = false;
                    StartPreview(myCASP.PartName);
                }
            }

            if (importedSwatch)
            {
                if (mySwatch != null)
                {
                    IResourceKey ik = null;
                    if (clonedSwatch)
                    {
                        DeleteResource(clonePack, iresSwatch);
                        ik = new TGIBlock(0, null, iresSwatch.ResourceType, iresSwatch.ResourceGroup, iresSwatch.Instance);
                    }
                    else
                    {
                        TGI newtgi = new TGI((uint)XmodsEnums.ResourceTypes.DDS,
                            0x00000000U, FNVhash.FNV64(myCASP.PartName) | 0x8000000000000000);
                        ik = new TGIBlock(0, null, newtgi.Type, newtgi.Group, newtgi.Instance);
                        if (myCASP.SwatchIndex == myCASP.EmptyLink)
                        {
                            myCASP.SwatchIndex = myCASP.addLink(newtgi);
                        }
                        else
                        {
                            myCASP.LinkList[myCASP.SwatchIndex] = newtgi;
                        }
                        clonedSwatch = true;
                    }
                    iresSwatch = clonePack.AddResource(ik, mySwatch.Stream, true);
                    iresSwatch.Compressed = (ushort)0x5A42;
                    importedSwatch = false;
                }
                else
                {
                    myCASP.SwatchIndex = (byte)myCASP.EmptyLink;
                    if (iresSwatch != null) DeleteResource(clonePack, iresSwatch);
                    iresSwatch = null;
                    clonedSwatch = false;
                    importedSwatch = false;
                }
            }

            for (int i = 0; i < CloneThumbs_dataGridView.Rows.Count; i++)
            {
                if ((int)CloneThumbs_dataGridView.Rows[i].Tag == myCASPindex)
                {
                    CloneThumbs_dataGridView.Rows[i].Cells["CASPThumbName"].Value = myCASP.PartName;
                    break;
                }
            }

            if (changesSliders) SliderCommit(false);
            if (changesHairColor) HairColor_Commit(false);

            DeleteResource(clonePack, myKey);
            Stream s = new MemoryStream();
            BinaryWriter bw = new BinaryWriter(s);
            myCASP.Write(bw);
            s.Position = 0;
            myKey = clonePack.AddResource(myKey, s, true);
            myKey.Compressed = (ushort)0x5A42;

            CloneCASPsList(false);
            changesRecolor = false;
        }

        private void CloneCASPDiscard_button_Click(object sender, EventArgs e)
        {
            DialogResult res = MessageBox.Show("Are you sure you want to discard changes?", "Discard Changes", MessageBoxButtons.OKCancel);
            if (res == DialogResult.Cancel) return;
            myCASPindex = (int)CloneCASPlist_dataGridView.Rows[myRow].Tag;
            myCASP = clonePackCASPs[myCASPindex].Casp;
            myKey = iresCASPs[myCASPindex];
            CloneCASPdisplay();
            StartPreview(CloneColorName.Text);
            changesRecolor = false;
        }

        private void CloneSliderCommit_button_Click(object sender, EventArgs e)
        {
            if (changesGeneral)
            {
                MessageBox.Show("You have unsaved general property changes - please commit or discard changes first!");
                return;
            }
            if (changesRecolor)
            {
                MessageBox.Show("You have unsaved recolor changes - please commit or discard changes first!");
                return;
            }
            SliderCommit(CloneSliderAll_checkBox.Checked);
        }

        private void SliderCommit(bool applyToAll)
        {
            if (myCASP.Version >= 0x2C)
            {
                CASP.OpacitySettings opacity = ReadSliderSetup(CloneSliderOpacityMinimum, CloneSliderOpacityIncrement);
                CASP.SliderSettings hue = ReadSliderSetup(CloneSliderHueMinimum, CloneSliderHueMaximum, CloneSliderHueIncrement);
                CASP.SliderSettings saturation = ReadSliderSetup(CloneSliderSaturationMinimum, CloneSliderSaturationMaximum, CloneSliderSaturationIncrement);
                CASP.SliderSettings brightness = ReadSliderSetup(CloneSliderBrightnessMinimum, CloneSliderBrightnessMaximum, CloneSliderBrightnessIncrement);
                if (opacity == null || hue == null || saturation == null || brightness == null) return;

                if (applyToAll)
                {
                    for (int i = 0; i < clonePackCASPs.Count; i++)
                    {
                        clonePackCASPs[i].Casp.OpacitySliderSettings = opacity;
                        clonePackCASPs[i].Casp.HueSliderSettings = hue;
                        clonePackCASPs[i].Casp.SaturationSliderSettings = saturation;
                        clonePackCASPs[i].Casp.BrightnessSliderSettings = brightness;

                        DeleteResource(clonePack, iresCASPs[i]);
                        Stream s = new MemoryStream();
                        BinaryWriter bw = new BinaryWriter(s);
                        clonePackCASPs[i].Casp.Write(bw);
                        s.Position = 0;
                        iresCASPs[i] = clonePack.AddResource(iresCASPs[i], s, true);
                        iresCASPs[i].Compressed = (ushort)0x5A42;
                    }
                }
                else
                {
                    myCASP.OpacitySliderSettings = opacity;
                    myCASP.HueSliderSettings = hue;
                    myCASP.SaturationSliderSettings = saturation;
                    myCASP.BrightnessSliderSettings = brightness;

                    DeleteResource(clonePack, myKey);
                    Stream s = new MemoryStream();
                    BinaryWriter bw = new BinaryWriter(s);
                    myCASP.Write(bw);
                    s.Position = 0;
                    myKey = clonePack.AddResource(myKey, s, true);
                    myKey.Compressed = (ushort)0x5A42;
                }
            }
            changesSliders = false;
        }

        private void CloneSliderDiscard_button_Click(object sender, EventArgs e)
        {
            ShowSliderSetup(myCASP.OpacitySliderSettings, CloneSliderOpacityMinimum, null, CloneSliderOpacityIncrement);
            ShowSliderSetup(myCASP.HueSliderSettings, CloneSliderHueMinimum, CloneSliderHueMaximum, CloneSliderHueIncrement);
            ShowSliderSetup(myCASP.SaturationSliderSettings, CloneSliderSaturationMinimum, CloneSliderSaturationMaximum, CloneSliderSaturationIncrement);
            ShowSliderSetup(myCASP.BrightnessSliderSettings, CloneSliderBrightnessMinimum, CloneSliderBrightnessMaximum, CloneSliderBrightnessIncrement);
            changesSliders = false;
        }

        private void CloneSliderDefaults_button_Click(object sender, EventArgs e)
        {
            CloneSliderOpacityMinimum.Text = "0.2";
            CloneSliderOpacityIncrement.Text = "0.05";
            CloneSliderHueMinimum.Text = "-0.5";
            CloneSliderHueMaximum.Text = "0.5";
            CloneSliderHueIncrement.Text = "0.05";
            CloneSliderSaturationMinimum.Text = "-0.5";
            CloneSliderSaturationMaximum.Text = "0.5";
            CloneSliderSaturationIncrement.Text = "0.05";
            CloneSliderBrightnessMinimum.Text = "-0.5";
            CloneSliderBrightnessMaximum.Text = "0.5";
            CloneSliderBrightnessIncrement.Text = "0.05";
        }

        private void ShowHairColorSetup(CASP casp)
        {
            HairColor_dataGridView.Rows.Clear();
            if (casp.HairColorKeys != null)
            {
                for (int i = 0; i < casp.HairColorKeys.Length; i++)
                {
                    TGI tgi = new TGI(casp.LinkList[casp.HairColorKeys[i]]);
                    int ind = HairColor_dataGridView.Rows.Add(tgi.ToString());
                    HairColor_dataGridView.Rows[ind].Tag = tgi;
                }
            }
        }

        private void HairColorImport_button_Click(object sender, EventArgs e)
        {
            string file = GetFilename("Select HairColor package", Packagefilter);
            try
            {
                List<TGI> tgis = new List<TGI>();
                Package pack = (Package)Package.OpenPackage(1, file);
                Predicate<IResourceIndexEntry> getCASP = r => r.ResourceType == (uint)XmodsEnums.ResourceTypes.CASP;
                List<IResourceIndexEntry> rcasps = pack.FindAll(getCASP);
                foreach (IResourceIndexEntry ires in rcasps)
                {
                    CASP casp = new CASP(new BinaryReader(pack.GetResource(ires)));
                    if (casp.BodyType == XmodsEnums.BodyType.HairColor || !HairColorRestrict_checkBox.Checked)
                    {
                        tgis.Add(new TGI(ires.ResourceType, ires.ResourceGroup, ires.Instance));
                    }
                }
                if (tgis.Count > 0)
                {
                    HairColor_dataGridView.Rows.Clear();
                    HairColorTGI_type.Text = "";
                    HairColorTGI_group.Text = "";
                    HairColorTGI_instance.Text = "";
                    HairColorTGISave_button.Tag = null;
                    HairColorMod_groupBox.Enabled = false;
                    for (int i = 0; i < tgis.Count; i++)
                    {
                        int ind = HairColor_dataGridView.Rows.Add(tgis[i]);
                        HairColor_dataGridView.Rows[ind].Tag = tgis[i];
                    }
                    changesHairColor = true;
                    MessageBox.Show("Done! Changes will not take effect until you click the Commit button.");
                }
                else
                {
                    MessageBox.Show("The package does not contain any HairColor CASPs!");
                }
            }
            catch (Exception eh)
            {
                MessageBox.Show("Could not read CASPs from package!" + Environment.NewLine + eh.Message);
            }
        }

        private void HairColorTGIadd_button_Click(object sender, EventArgs e)
        {
            HairColorMod_groupBox.Enabled = true;
            HairColorTGI_type.Text = ((uint)XmodsEnums.ResourceTypes.CASP).ToString("X8");
            HairColorTGI_group.Text = "";
            HairColorTGI_instance.Text = "";
            HairColorTGISave_button.Tag = -1;
        }

        private void HairColor_dataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex > HairColor_dataGridView.RowCount - 1) return;
            if (e.ColumnIndex < 0 || e.ColumnIndex > 1) return;
            if (e.ColumnIndex == 0)     //modify
            {
                HairColorMod_groupBox.Enabled = true;
                TGI tgi = (TGI)HairColor_dataGridView.Rows[e.RowIndex].Tag;
                HairColorTGI_type.Text = tgi.Type.ToString("X8");
                HairColorTGI_group.Text = tgi.Group.ToString("X8");
                HairColorTGI_instance.Text = tgi.Instance.ToString("X16");
                HairColorTGISave_button.Tag = e.RowIndex;
            }
            else if (e.ColumnIndex == 1)        //delete
            {
                if (HairColorMod_groupBox.Enabled)
                {
                    MessageBox.Show("Please save or cancel the current modification or addition first.");
                    return;
                }
                DialogResult res = MessageBox.Show("Are you sure?", "Delete TGI from HairColors list", MessageBoxButtons.OKCancel);
                if (res == DialogResult.OK)
                {
                    HairColor_dataGridView.Rows.RemoveAt(e.RowIndex);
                }
            }
        }

        private void HairColorTGISave_button_Click(object sender, EventArgs e)
        {
            int row = HairColorTGISave_button.Tag != null ? (int)HairColorTGISave_button.Tag : -1;
            string str = HairColorTGI_type.Text + "-" + HairColorTGI_group.Text + "-" + HairColorTGI_instance.Text;
            TGI tgi = null;
            try
            {
                tgi = new TGI(str);
            }
            catch
            {
                MessageBox.Show("Please enter a valid TGI using hexidecimal numbers in the format xxxxxxxx-xxxxxxxx-xxxxxxxxxxxxxxxx");
                return;
            }
            if (tgi == null)
            {
                MessageBox.Show("TGI is null, something went wrong.");
                return;
            }
            if (row < 0)
            {
                int ind = HairColor_dataGridView.Rows.Add(tgi.ToString());
                HairColor_dataGridView.Rows[ind].Tag = tgi;
            }
            else
            {
                HairColor_dataGridView.Rows[row].Cells[0].Value = tgi.ToString();
                HairColor_dataGridView.Rows[row].Tag = tgi;
            }
            HairColorTGI_type.Text = "";
            HairColorTGI_group.Text = "";
            HairColorTGI_instance.Text = "";
            HairColorTGISave_button.Tag = null;
            HairColorMod_groupBox.Enabled = false;
            changesHairColor = true;
        }

        private void HairColorTGICancel_button_Click(object sender, EventArgs e)
        {
            HairColorTGI_type.Text = "";
            HairColorTGI_group.Text = "";
            HairColorTGI_instance.Text = "";
            HairColorTGISave_button.Tag = null;
            HairColorMod_groupBox.Enabled = false;
        }

        private void HairColorCommit_button_Click(object sender, EventArgs e)
        {
            if (changesGeneral)
            {
                MessageBox.Show("You have unsaved general property changes - please commit or discard changes first!");
                return;
            }
            if (changesRecolor)
            {
                MessageBox.Show("You have unsaved recolor changes - please commit or discard changes first!");
                return;
            }
            HairColor_Commit(HairColorApplyAll_checkBox.Checked);
            changesHairColor = false;
        }
        private void HairColor_Commit(bool applyToAll)
        {
            if (myCASP.Version >= 0x2E)
            {
                if (applyToAll)
                {
                    for (int i = 0; i < clonePackCASPs.Count; i++)
                    {
                        byte[] tgiKeys = new byte[HairColor_dataGridView.RowCount];
                        for (int j = 0; j < HairColor_dataGridView.RowCount; j++)
                        {
                            TGI tgi = (TGI)HairColor_dataGridView.Rows[j].Tag;
                            tgiKeys[j] = clonePackCASPs[i].Casp.addLink(tgi);
                        }
                        clonePackCASPs[i].Casp.HairColorKeys = tgiKeys;
                        clonePackCASPs[i].Casp.RebuildLinkList();

                        DeleteResource(clonePack, iresCASPs[i]);
                        Stream s = new MemoryStream();
                        BinaryWriter bw = new BinaryWriter(s);
                        clonePackCASPs[i].Casp.Write(bw);
                        s.Position = 0;
                        iresCASPs[i] = clonePack.AddResource(iresCASPs[i], s, true);
                        iresCASPs[i].Compressed = (ushort)0x5A42;
                    }
                }
                else
                {
                    byte[] tgiKeys = new byte[HairColor_dataGridView.RowCount];
                    for (int i = 0; i < HairColor_dataGridView.RowCount; i++)
                    {
                        TGI tgi = (TGI)HairColor_dataGridView.Rows[i].Tag;
                        tgiKeys[i] = myCASP.addLink(tgi);
                    }
                    myCASP.HairColorKeys = tgiKeys;
                    myCASP.RebuildLinkList();

                    DeleteResource(clonePack, myKey);
                    Stream s = new MemoryStream();
                    BinaryWriter bw = new BinaryWriter(s);
                    myCASP.Write(bw);
                    s.Position = 0;
                    myKey = clonePack.AddResource(myKey, s, true);
                    myKey.Compressed = (ushort)0x5A42;
                }
            }
        }

        private void HairColorDiscard_button_Click(object sender, EventArgs e)
        {
            ShowHairColorSetup(myCASP);
            changesHairColor = false;
        }

        private ThumbnailResource[] FindCloneColorThumb(IResourceIndexEntry caspIndexEntry)
        {
            ThumbnailResource[] tmpThumb = new ThumbnailResource[2] { null, null };
            bool foundMale = false, foundFemale = false;
            Predicate<IResourceIndexEntry> pred = r => r.ResourceType == (uint)XmodsEnums.ResourceTypes.THUM &
                                                r.Instance == caspIndexEntry.Instance;
            List<IResourceIndexEntry> cthumbs = clonePack.FindAll(pred);

            if (cthumbs.Count > 0)
            {
                foreach (IResourceIndexEntry ir in cthumbs)
                {
                    if (ir.ResourceGroup == 0x00000002U | ir.ResourceGroup == 0x00000102U)
                    {
                        ThumbnailResource thumb = new ThumbnailResource(0, clonePack.GetResource(ir));
                        if (ir.ResourceGroup == 0x00000102U)
                        {
                            tmpThumb[0] = thumb;
                            foundMale = true;
                        }
                        else
                        {
                            tmpThumb[1] = thumb;
                            foundFemale = true;
                        }
                    }
                }
            }
            if (foundMale & foundFemale) return tmpThumb;
            foreach (Package tp in gameThumbPacks)
            {
                List<IResourceIndexEntry> rthumbs = tp.FindAll(pred);
                if (rthumbs.Count > 0)
                {
                    foreach (IResourceIndexEntry ir in rthumbs)
                    {
                        if (ir.ResourceGroup == 0x00000002U | ir.ResourceGroup == 0x00000102U)
                        {
                            ThumbnailResource thumb = new ThumbnailResource(0, tp.GetResource(ir));
                            if (ir.ResourceGroup == 0x00000102U & !foundMale)
                            {
                                tmpThumb[0] = thumb;
                                foundMale = true;
                            }
                            else if (ir.ResourceGroup == 0x00000002U & !foundFemale)
                            {
                                tmpThumb[1] = thumb;
                                foundFemale = true;
                            }
                        }
                    }
                }
            }
            return tmpThumb;
        }

        private DSTResource FindCloneTextureDST(TGI[] tgiArray, int tgiIndex, out IResourceIndexEntry foundKey, out bool inClonePack)
        {
            if (tgiIndex < 0 || tgiArray.Length < tgiIndex + 1 || (tgiArray[tgiIndex].Type == 0 & tgiArray[tgiIndex].Group == 0 & tgiArray[tgiIndex].Instance == 0))
            {
                inClonePack = false;
                foundKey = null;
                return null;
            }
            TGI tgi = tgiArray[tgiIndex];
            Predicate<IResourceIndexEntry> itex = r => r.ResourceType == tgi.Type &
                                    r.ResourceGroup == tgi.Group & r.Instance == tgi.Instance;
            IResourceIndexEntry ctex = clonePack.Find(itex);
            if (ctex != null)
            {
                inClonePack = true;
                foundKey = ctex;
                return new DSTResource(1, clonePack.GetResource(ctex));
            }
            foreach (Package p in gamePacksOther)
            {
                IResourceIndexEntry ptex = p.Find(itex);
                if (ptex != null)
                {
                    inClonePack = false;
                    foundKey = ptex;
                    return new DSTResource(1, p.GetResource(ptex));
                }
            }
            foreach (Package p in gamePacks0)
            {
                IResourceIndexEntry ptex = p.Find(itex);
                if (ptex != null)
                {
                    inClonePack = false;
                    foundKey = ptex;
                    return new DSTResource(1, p.GetResource(ptex));
                }
            }
            inClonePack = false;
            foundKey = null;
            return null;
        }

        private AResource FindCloneTextureRLE(TGI tgi, out IResourceIndexEntry foundKey, out bool inClonePack)
        {
            if (tgi.Type == 0 & tgi.Group == 0 & tgi.Instance == 0)
            {
                inClonePack = false;
                foundKey = null;
                return null;
            }
            Predicate<IResourceIndexEntry> itex = r => r.ResourceType == tgi.Type &
                                    r.ResourceGroup == tgi.Group & r.Instance == tgi.Instance;
            if (clonePack != null)
            {
                IResourceIndexEntry ctex = clonePack.Find(itex);
                if (ctex != null)
                {
                    inClonePack = true;
                    foundKey = ctex;
                    return (XmodsEnums.ResourceTypes)ctex.ResourceType switch
                    {
                        XmodsEnums.ResourceTypes.DDS => new DSTResource(1, clonePack.GetResource(ctex)),
                        _ => new RLEResource(1, clonePack.GetResource(ctex))
                    };
                }
            }
            foreach (Package p in gamePacksOther.Union(gamePacks0))
            {
                IResourceIndexEntry ptex = p.Find(itex);
                if (ptex != null)
                {
                    inClonePack = false;
                    foundKey = ptex;
                    Stream s = p.GetResource(ptex);
                    if (s.Length > 0)
                        return (XmodsEnums.ResourceTypes)ptex.ResourceType switch
                        {
                            XmodsEnums.ResourceTypes.DDS => new DSTResource(1, s),
                            _ => new RLEResource(1, s)
                        };
                }
            }
            inClonePack = false;
            foundKey = null;
            return null;
        }

        private Object FindCloneTexture(TGI tgi, out IResourceIndexEntry foundKey, out bool inClonePack)
        {
            if (tgi.Type == 0 & tgi.Group == 0 & tgi.Instance == 0)
            {
                inClonePack = false;
                foundKey = null;
                return null;
            }
            Predicate<IResourceIndexEntry> itexL = r => r.ResourceType == (uint)XmodsEnums.ResourceTypes.LRLE &
                                    r.ResourceGroup == tgi.Group & r.Instance == tgi.Instance;
            Predicate<IResourceIndexEntry> itexR = r => r.ResourceType == (uint)XmodsEnums.ResourceTypes.DXT5RLE2 &
                                    r.ResourceGroup == tgi.Group & r.Instance == tgi.Instance;
            Predicate<IResourceIndexEntry> itexD = r => r.ResourceType == (uint)XmodsEnums.ResourceTypes.DDS &
                                    r.ResourceGroup == tgi.Group & r.Instance == tgi.Instance;
            if (clonePack != null)
            {
                IResourceIndexEntry ctex = clonePack.Find(itexL);
                if (ctex != null)
                {
                    inClonePack = true;
                    foundKey = ctex;
                    try
                    {
                        Stream s = clonePack.GetResource(ctex);
                        return new LRLE(new BinaryReader(s));
                    }
                    catch
                    {
                        try
                        {
                            return new RLEResource(1, clonePack.GetResource(ctex));
                        }
                        catch
                        {
                            return null;
                        }
                    }
                }
                else
                {
                    ctex = clonePack.Find(itexR);
                    if (ctex != null)
                    {
                        inClonePack = true;
                        foundKey = ctex;
                        try
                        {
                            return new RLEResource(1, clonePack.GetResource(ctex));
                        }
                        catch
                        {
                            return null;
                        }
                    }
                    else
                    {
                        IResourceIndexEntry dtex = clonePack.Find(itexD);
                        if (dtex != null)
                        {
                            inClonePack = true;
                            foundKey = dtex;
                            try
                            {
                                return new DSTResource(1, clonePack.GetResource(dtex));
                            }
                            catch
                            {
                                return null;

                            }
                        }
                    }
                }
            }

            foreach (Package p in gamePacksOther)           //search game packs for LRLE
            {
                IResourceIndexEntry ptex = p.Find(itexL);
                if (ptex != null)
                {
                    inClonePack = false;
                    foundKey = ptex;
                    return new LRLE(new BinaryReader(p.GetResource(ptex)));
                }
            }
            foreach (Package p in gamePacks0)
            {
                IResourceIndexEntry ptex = p.Find(itexL);
                if (ptex != null)
                {
                    inClonePack = false;
                    foundKey = ptex;
                    return new LRLE(new BinaryReader(p.GetResource(ptex)));
                }
            }

            foreach (Package p in gamePacksOther)           //search game packs for RLE
            {
                IResourceIndexEntry ptex = p.Find(itexR);
                if (ptex != null)
                {
                    inClonePack = false;
                    foundKey = ptex;
                    Stream s = p.GetResource(ptex);
                    if (s.Length > 0) return new RLEResource(1, s);
                }
            }
            foreach (Package p in gamePacks0)
            {
                IResourceIndexEntry ptex = p.Find(itexR);
                if (ptex != null)
                {
                    inClonePack = false;
                    foundKey = ptex;
                    Stream s = p.GetResource(ptex);
                    if (s.Length > 0) return new RLEResource(1, s);
                }
            }
            foreach (Package p in gamePacksOther)           //search game packs for DST
            {
                IResourceIndexEntry ptex = p.Find(itexD);
                if (ptex != null)
                {
                    inClonePack = false;
                    foundKey = ptex;
                    Stream s = p.GetResource(ptex);
                    if (s.Length > 0) return new DSTResource(1, s);
                }
            }
            foreach (Package p in gamePacks0)
            {
                IResourceIndexEntry ptex = p.Find(itexD);
                if (ptex != null)
                {
                    inClonePack = false;
                    foundKey = ptex;
                    Stream s = p.GetResource(ptex);
                    if (s.Length > 0) return new DSTResource(1, s);
                }
            }

            inClonePack = false;
            foundKey = null;
            return null;
        }

        private void ClonePropTexture_pictureBox_Click(object sender, EventArgs e)
        {
            switch (ClonePropTexture_comboBox.SelectedIndex)
            {
                case 0:     //shadow
                    clonePropShadowClicked();
                    break;
                case 1:     //specular
                    clonePropSpecularClicked();
                    break;
                case 2:     //bumpmap
                    MeshBumpmapClicked();
                    break;
                case 3:     //emissionmap
                    MeshGlowClicked();
                    break;
                case 4:     //specular
                    clonePropColorShiftMaskClicked();
                    break;
                default:
                    break;
            }
        }
        
        private void clonePropShadowClicked()
        {
            ImageDisplayImportExport imgDisplay = new ImageDisplayImportExport(myShadow, ImageType.Shadow, "Import/Export ShadowMap", myCASP.Species, myCASP.BodyType, false);
            DialogResult res = imgDisplay.ShowDialog();
            if (res == DialogResult.OK)
            {
                cloneWait_label.Visible = true;
                cloneWait_label.Refresh();
                myShadow = imgDisplay.ReturnRLE;
                importedShadow = true;
                ddsShadow = new DdsFile();
                ddsShadow.Load(myShadow.ToDDS(), false);
                shadowImage = new Bitmap(ddsShadow.Image);
                ClonePropTexture_pictureBox.Image = shadowImage;
                cloneWait_label.Visible = false;
                cloneWait_label.Refresh();
                changesGeneral = true;
            }
        }

        private void clonePropSpecularClicked()
        {
            ImageDisplayImportExport imgDisplay = new ImageDisplayImportExport(mySpecular, ImageType.Specular, "Import/Export Specular Map", myCASP.Species, myCASP.BodyType, false);
            DialogResult res = imgDisplay.ShowDialog();
            if (res == DialogResult.OK)
            {
                cloneWait_label.Visible = true;
                cloneWait_label.Refresh();
                mySpecular = imgDisplay.ReturnRLE;
                importedSpecular = true;
                ddsSpecular = new DdsFile();
                ddsSpecular.Load(mySpecular.ToDDS(), false);
                specularImage = new Bitmap(ddsSpecular.Image);
                ClonePropTexture_pictureBox.Image = specularImage;
                cloneWait_label.Visible = false;
                cloneWait_label.Refresh();
                changesGeneral = true;
            }
        }private void clonePropColorShiftMaskClicked()
        {
            ImageDisplayImportExport imgDisplay = new ImageDisplayImportExport(myColorShiftMask, ImageType.ColorShiftMask, "Import/Export Color Shift Mask", myCASP.Species, myCASP.BodyType, false);
            DialogResult res = imgDisplay.ShowDialog();
              if (res == DialogResult.OK)
            {
                cloneWait_label.Visible = true;
                cloneWait_label.Refresh();
                myColorShiftMask = imgDisplay.ReturnRLE;
                importedcolorShiftMask = true;
                ddsColorShiftMask = new DdsFile();
                ddsColorShiftMask.Load(myColorShiftMask.ToDDS(), false);
                colorShiftMaskImage = new Bitmap(ddsColorShiftMask.Image);
                ClonePropTexture_pictureBox.Image = colorShiftMaskImage;
                cloneWait_label.Visible = false;
                cloneWait_label.Refresh();
                changesGeneral = true;
            }
        }

        private void MeshBumpmapClicked()
        {
            ImageDisplayImportExport imgDisplay = new ImageDisplayImportExport(meshBumpMap, ImageType.BumpMap, "Import/Export BumpMap", myCASP.Species, myCASP.BodyType);
            DialogResult res = imgDisplay.ShowDialog();
            if (res == DialogResult.OK)
            {
                cloneWait_label.Visible = true;
                cloneWait_label.Refresh();
                meshBumpMap = imgDisplay.ReturnDST;
                importedBumpMap = true;
                changesGeneral = true;
                ddsBumpmap = new DdsFile();
                ddsBumpmap.Load(meshBumpMap.ToDDS(), false);
                bumpmapImage = ddsBumpmap.Image;
                ClonePropTexture_pictureBox.Image = bumpmapImage;
                cloneWait_label.Visible = false;
                cloneWait_label.Refresh();
            }
        }

        private void MeshGlowClicked()
        {
            ImageDisplayImportExport imgDisplay = new ImageDisplayImportExport(meshGlowMap, ImageType.GlowMap, "Import/Export Emission/Glow Map", myCASP.Species, myCASP.BodyType);
            DialogResult res = imgDisplay.ShowDialog();
            if (res == DialogResult.OK)
            {
                cloneWait_label.Visible = true;
                cloneWait_label.Refresh();
                meshGlowMap = imgDisplay.ReturnDST;
                if (meshGlowMap != null)
                {
                    ddsGlowMap = new DdsFile();
                    ddsGlowMap.Load(meshGlowMap.ToDDS(), false);
                    glowImage = ddsGlowMap.Image;
                    ClonePropTexture_pictureBox.Image = glowImage;
                }
                else
                {
                    ddsGlowMap = null;
                    glowImage = null;
                    ClonePropTexture_pictureBox.Image = Properties.Resources.NullImage;
                }
                importedGlow = true;
                cloneWait_label.Visible = false;
                cloneWait_label.Refresh();
                changesGeneral = true;
            }
        }

        private void CloneColorTexture_pictureBox_Click(object sender, EventArgs e)
        {
            ImageDisplayImportExport imgDisplay;
            if (myTexture is AResource)
            {
                imgDisplay = new ImageDisplayImportExport((AResource)myTexture, ImageType.Material, "Import/Export Diffuse/Material Texture", myCASP.Species, myCASP.BodyType, IsMakeup(myCASP));
            }
            else if (myTexture is LRLE)
            {
                imgDisplay = new ImageDisplayImportExport((LRLE)myTexture, ImageType.Material, "Import/Export Diffuse/Material Texture", myCASP.Species, myCASP.BodyType);
            }
            else
            {
                MessageBox.Show("Unrecognized texture type!");
                return;
            }
            DialogResult res = imgDisplay.ShowDialog();
            if (res == DialogResult.OK)
            {
                if (imgDisplay.UseLRLE)
                {
                    cloneWait_label.Visible = true;
                    cloneWait_label.Refresh();
                    myTexture = imgDisplay.ReturnLRLE;
                    textureImage = imgDisplay.ReturnLRLE.image;
                }
                else
                {
                    myTexture = imgDisplay.ReturnRLE;
                    ddsTexture = new DdsFile();
                    ddsTexture.Load(imgDisplay.ReturnRLE.ToDDS(), false);
                    textureImage = new Bitmap(ddsTexture.Image);
                }
                CloneColorTexture_pictureBox.Image = textureImage;
                CloneColorTexture_pictureBox.Refresh();
                importedTexture = true;
                cloneWait_label.Visible = false;
                cloneWait_label.Refresh();
                changesRecolor = true;
            }
        }

        private void CloneColorThumb_pictureBox_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Please use the Thumbnail Manager tab to modify thumbnails.");
        }

        private void CloneColorSwatch_pictureBox_Click(object sender, EventArgs e)
        {
            ImageDisplayImportExport imgDisplay = new ImageDisplayImportExport(mySwatch, ImageType.Swatch, "Import/Export Swatch", myCASP.Species, myCASP.BodyType);
            DialogResult res = imgDisplay.ShowDialog();
            if (res == DialogResult.OK)
            {
                cloneWait_label.Visible = true;
                cloneWait_label.Refresh();
                mySwatch = imgDisplay.ReturnDST;
                if (mySwatch != null)
                {
                    ddsSwatch = new DdsFile();
                    ddsSwatch.Load(mySwatch.ToDDS(), false);
                    CloneColorSwatch_pictureBox.Image = new Bitmap(ddsSwatch.Image);
                    CloneColorSwatch_pictureBox.Refresh();
                }
                else
                {
                    ddsSwatch = null;
                    CloneColorSwatch_pictureBox.Image = Properties.Resources.NullImage;
                    CloneColorSwatch_pictureBox.Refresh();
                }
                importedSwatch = true;
                cloneWait_label.Visible = false;
                cloneWait_label.Refresh();
                changesRecolor = true;
            }
        }

        private static void SortCategories(string[] catNameList, uint[] catValueList)
        {
            for (int i = catNameList.Length - 1; i >= 0; i--)
            {
                for (int j = 0; j < i; j++)
                {
                    if (string.Compare(catNameList[j], catNameList[j + 1]) > 0)
                    {
                        string tmp = catNameList[j];
                        catNameList[j] = catNameList[j + 1];
                        catNameList[j + 1] = tmp;
                        uint stmp = catValueList[j];
                        catValueList[j] = catValueList[j + 1];
                        catValueList[j + 1] = stmp;
                    }
                }
            }
        }

        private void ClonePackageWipe()
        {
            ClonePropReset();
            myCASP = null;
            myKey = null;
            myRow = 0;
            myCASPindex = 0;
            clonePartName = "";
            ClonePropID.Text = "";
            List<ushort[]> caspFlags = new List<ushort[]>();
            myShadow = null;
            mySpecular = null;
            ClonePropTexture_pictureBox.Image = null;

            CloneCASPlist_dataGridView.Rows.Clear();
            List<ushort[]> catFlags = new List<ushort[]>();
            CloneColorProp_dataGridView.Rows.Clear();
            CloneColorTexture_pictureBox.Image = null;
            CloneColorSwatch_pictureBox.Image = null;
            CloneColorThumb_pictureBox.Image = null;
            mySwatch = null;
            myTexture = null;
            CloneColorName.Text = "";
            CloneColorSortOrder.Text = "";
            CloneColorDefaultBodyTypeMale_checkBox.Checked = false;
            CloneColorDefaultThumb_checkBox.Checked = false;
            CloneColor1_panel.Visible = true;
            CloneColor2_panel.Visible = false;
            CloneColor3_panel.Visible = false;
            CloneColorAdd_button.Visible = true;
            CloneColor1_pictureBox.BackColor = Color.White;
            WipeSliderSettings();
        }
    }
}
