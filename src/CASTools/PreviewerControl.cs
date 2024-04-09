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
        int[] customColors = new int[] {0xF0FFFF, 0xCDF0FF, 0xD1E4FF, 0xD0E5F5, 
                0xC4E4FF, 0x8CB4D2, 0x7A98B8, 0x5880B0, 0x406890, 0x3F3F65, 0x303040 };
        int currentLOD = 0;

        public void StartPreview(string recolorName)
        {
            previewHead_checkBox.CheckedChanged -= previewHead_checkBox_CheckedChanged;
            previewTop_checkBox.CheckedChanged -= previewTop_checkBox_CheckedChanged;
            previewBottom_checkBox.CheckedChanged -= previewBottom_checkBox_CheckedChanged;
            previewFeet_checkBox.CheckedChanged -= previewFeet_checkBox_CheckedChanged;
            previewBody_checkBox.CheckedChanged -= previewBody_checkBox_CheckedChanged;
            previewEars_checkBox.CheckedChanged -= previewEarsChange;
            previewEarsUp_radioButton.CheckedChanged -= previewEarsChange;
            previewEarsDown_radioButton.CheckedChanged -= previewEarsChange;
            previewTail_checkBox.CheckedChanged -= previewTailChange;
            previewTailLong_radioButton.CheckedChanged -= previewTailChange;
            previewTailRing_radioButton.CheckedChanged -= previewTailChange;
            previewTailScrew_radioButton.CheckedChanged -= previewTailChange;
            previewTailStub_radioButton.CheckedChanged -= previewTailChange;

            previewHead_checkBox.Checked = true;
            previewTop_checkBox.Checked = true;
            previewBottom_checkBox.Checked = true;
            previewFeet_checkBox.Checked = true;
            previewBody_checkBox.Checked = true;
            previewEars_checkBox.Checked = true;
            previewTail_checkBox.Checked = true;

            if (myCASP.BodyType == XmodsEnums.BodyType.Body)
            {
                previewTop_checkBox.Checked = false;
                previewBottom_checkBox.Checked = false;
                previewBody_checkBox.Checked = false;
            }
            else if (myCASP.BodyType == XmodsEnums.BodyType.Shoes)
            {
                previewFeet_checkBox.Checked = false;
            }
            else if (myCASP.BodyType == XmodsEnums.BodyType.Head)
            {
                previewHead_checkBox.Checked = false;
            }
            
            System.Windows.Media.Color skinColor;
            if (myCASP.Species == XmodsEnums.Species.Human)
            {
                skinColor = System.Windows.Media.Colors.Tan;
                SetCheckBoxState(previewEars_checkBox, false, previewEars_panel);
                SetCheckBoxState(previewTail_checkBox, false, previewTail_panel);
                SetCheckBoxState(previewUndies_checkBox, true);
                previewBody_checkBox.Checked = false;
                previewBody_checkBox.Enabled = false;
                if (myCASP.BodyType == XmodsEnums.BodyType.Top)
                {
                    previewTop_checkBox.Checked = false;
                }
                else if (myCASP.BodyType == XmodsEnums.BodyType.Bottom)
                {
                    previewBottom_checkBox.Checked = false;
                }
                else if (myCASP.BodyType == XmodsEnums.BodyType.Body)
                {
                    previewTop_checkBox.Checked = false;
                    previewBottom_checkBox.Checked = false;
                }
                else if (myCASP.BodyType == XmodsEnums.BodyType.Tail)
                {
                    previewBottom_checkBox.Checked = false;
                    previewFeet_checkBox.Checked = false;
                }
            }
            else
            {
                skinColor = System.Windows.Media.Colors.Gray;
                previewSkinColor_pictureBox.BackColor = Color.Gray;
                SetCheckBoxState(previewTop_checkBox, false);
                SetCheckBoxState(previewBottom_checkBox, false);
                SetCheckBoxState(previewUndies_checkBox, false);
                SetCheckBoxState(previewEars_checkBox, true, previewEars_panel);
                SetCheckBoxState(previewTail_checkBox, true, previewTail_panel);
                previewEars_checkBox.Enabled = true;
                previewTail_checkBox.Enabled = true;
                previewBody_checkBox.Enabled = true;
                if (myCASP.Species == XmodsEnums.Species.Cat)
                {
                    previewTailLong_radioButton.Checked = true;
                    SetRadioButtonState(previewTailRing_radioButton, false);
                    SetRadioButtonState(previewTailScrew_radioButton, false);
                }
                else
                {
                    previewTailRing_radioButton.Enabled = true;
                    previewTailScrew_radioButton.Enabled = true;
                }
                if (myCASP.BodyType == XmodsEnums.BodyType.AnimalEars)
                {
                    previewEars_checkBox.Checked = false;
                }
                else if (myCASP.BodyType == XmodsEnums.BodyType.Tail)
                {
                    previewTail_checkBox.Checked = false;
                }
            }

            if ((myCASP.ExcludePartFlags & ExcludePartValues[3]) > 0) previewHead_checkBox.Checked = false;
            if ((myCASP.ExcludePartFlags & ExcludePartValues[5]) > 0) previewBody_checkBox.Checked = false;
            if ((myCASP.ExcludePartFlags & ExcludePartValues[6]) > 0) previewTop_checkBox.Checked = false;
            if ((myCASP.ExcludePartFlags & ExcludePartValues[7]) > 0) previewBottom_checkBox.Checked = false;
            if ((myCASP.ExcludePartFlags & ExcludePartValues[8]) > 0) previewFeet_checkBox.Checked = false;
            if ((myCASP.ExcludePartFlags & ExcludePartValues[60]) > 0) previewEars_checkBox.Checked = false;
            if ((myCASP.ExcludePartFlags & ExcludePartValues[61]) > 0) previewTail_checkBox.Checked = false;

            previewHead_checkBox.CheckedChanged += previewHead_checkBox_CheckedChanged;
            previewTop_checkBox.CheckedChanged += previewTop_checkBox_CheckedChanged;
            previewBottom_checkBox.CheckedChanged += previewBottom_checkBox_CheckedChanged;
            previewFeet_checkBox.CheckedChanged += previewFeet_checkBox_CheckedChanged;
            previewBody_checkBox.CheckedChanged += previewBody_checkBox_CheckedChanged;
            previewEars_checkBox.CheckedChanged += previewEarsChange;
            previewEarsUp_radioButton.CheckedChanged += previewEarsChange;
            previewEarsDown_radioButton.CheckedChanged += previewEarsChange;
            previewTail_checkBox.CheckedChanged += previewTailChange;
            previewTailLong_radioButton.CheckedChanged += previewTailChange;
            previewTailRing_radioButton.CheckedChanged += previewTailChange;
            previewTailScrew_radioButton.CheckedChanged += previewTailChange;
            previewTailStub_radioButton.CheckedChanged += previewTailChange;

            bool glassOnly = true;
            if (LODmeshes[currentLOD] != null)
            {
                foreach (GEOM g in LODmeshes[currentLOD])
                {
                    if (g.ShaderType != XmodsEnums.Shader.SimGlass) glassOnly = false;
                }
            }

            previewDiffuse_checkBox.Checked = textureImage != null;
            previewSpecular_checkBox.Checked = specularImage != null;
            previewShadow_checkBox.Checked = shadowImage != null && !glassOnly;
            previewBumpmap_checkBox.Checked = false;
            previewGlowmap_checkBox.Checked = glowImage != null;
            previewDiffuse_checkBox.Checked = textureImage != null;
            previewSpecular_checkBox.Enabled = specularImage != null;
            previewShadow_checkBox.Enabled = shadowImage != null && !glassOnly;
            previewBumpmap_checkBox.Enabled = bumpmapImage != null;
            previewGlowmap_checkBox.Enabled = glowImage != null;
            previewUnisex_button.Text = "Change to " + (myCASP.Gender == XmodsEnums.Gender.Female ? "Male" : "Female") + " Model";
            previewUnisex_button.Visible = myCASP.Species == XmodsEnums.Species.Human && myCASP.Age > XmodsEnums.Age.Child && myCASP.Age <= XmodsEnums.Age.Elder;

            SetupPreviewMorphs(myCASP.Gender);

            Image bumpPreview = ExpandPartialImage(DisplayableBumpMap((Bitmap)bumpmapImage), myCASP.BodyType, myCASP.Species);
            Image shadowPreview = DisplayableShadow((Bitmap)shadowImage);
            Image specularPreview = DisplayableSpecular((Bitmap)specularImage);
            Image glowPreview = ExpandPartialImage(DisplayableGlow((Bitmap)glowImage), myCASP.BodyType, myCASP.Species);

            SetupRegionList();
         /*   string[] regions = new string[LODmeshes[currentLOD].Count];
            for (int i = 0; i < LODmeshes[currentLOD].Count; i++)
            {
                uint[] rs = meshRegionMap.GetMeshRegions(LODtgis[currentLOD][i]);
                regions[i] = Enum.GetName(typeof(XmodsEnums.CASPartRegionTS4), rs[0]);
                for (int j = 1; j < rs.Length; j++)
                {
                    regions[i] += "/" + Enum.GetName(typeof(XmodsEnums.CASPartRegionTS4), rs[j]);
                }
                regions[i] += ", Layer " + meshRegionMap.GetMeshLayer(LODtgis[currentLOD][i]).ToString();
                if (regions[i].IndexOf("HairHatB/HairHatD") >= 0) { regions[i] += " (low hat)"; }
                else if (regions[i].IndexOf("HairHatD/HairHatC") >= 0) { regions[i] += " (high hat)"; }
                else if (regions[i].IndexOf("HairHatA/HairHatB/HairHatC/HairHatD") >= 0) { regions[i] += " (no hat)"; }
            }
            previewRegions_checkedListBox.ItemCheck -= previewRegions_checkedListBox_ItemCheck;
            previewRegions_checkedListBox.Items.Clear();
            previewRegions_checkedListBox.Items.AddRange(regions);
            for (int i = 0; i < previewRegions_checkedListBox.Items.Count; i++)
            {
                if (myCASP.BodyType == XmodsEnums.BodyType.Hair)
                {
                    if (i == previewRegions_checkedListBox.Items.Count - 1)
                        previewRegions_checkedListBox.SetItemChecked(i, true);
                }
                else
                {
                    previewRegions_checkedListBox.SetItemChecked(i, true);
                }
            }
            previewRegions_checkedListBox.ItemCheck += previewRegions_checkedListBox_ItemCheck; */

            casPartPreviewer1.Start_Mesh(LODmeshes[currentLOD].ToArray(), currentLOD, 
                textureImage, specularPreview, shadowPreview, bumpPreview, glowPreview,
                myCASP.Species, myCASP.Age, myCASP.Gender, skinColor, previewSkin_checkBox.Checked, 
                previewLight1_checkBox.Checked, previewLight2_checkBox.Checked,
                previewHead_checkBox.Checked, previewTop_checkBox.Checked, previewBottom_checkBox.Checked, previewFeet_checkBox.Checked,
                previewUndies_checkBox.Checked, previewBody_checkBox.Checked, previewEars_checkBox.Checked, previewTail_checkBox.Checked);

            previewRecolorName_textBox.Text = recolorName;

            if (myCASP.BodyType == XmodsEnums.BodyType.Hair)
            {
                for (int i = 0; i < previewRegions_checkedListBox.Items.Count; i++)
                {
                    if (!previewRegions_checkedListBox.GetItemChecked(i))
                        casPartPreviewer1.SetRegionMesh(i, false);
                }
            }
        }

        public void SetupPreviewMorphs(XmodsEnums.Gender gender)
        {
            if (myCASP.Species == XmodsEnums.Species.Human)
            {
                previewMorph_comboBox.SelectedIndexChanged -= previewMorph_comboBox_SelectedIndexChanged;
                previewMorph_comboBox.Items.Clear();
                previewMorph_comboBox.Enabled = true;
                if (myCASP.Age >= XmodsEnums.Age.Teen && myCASP.Age <= XmodsEnums.Age.Elder)
                {
                    if (gender == XmodsEnums.Gender.Female)
                    {
                        previewMorph_comboBox.Items.AddRange(MorphNamesAdultFemale);
                    }
                    else
                    {
                        previewMorph_comboBox.Items.AddRange(MorphNamesAdultMale);
                    }
                }
                else if (myCASP.Age == XmodsEnums.Age.Child)
                {
                    previewMorph_comboBox.Items.AddRange(MorphNamesChild);
                }
                else if (myCASP.Age == XmodsEnums.Age.Toddler)
                {
                    previewMorph_comboBox.Items.AddRange(MorphNamesToddler);
                }
                if (previewMorph_comboBox.Items.Count > 0)
                {
                    previewMorph_comboBox.SelectedIndex = 0;
                    previewMorph_comboBox.SelectedIndexChanged += previewMorph_comboBox_SelectedIndexChanged;
                }
                else
                {
                    previewMorph_comboBox.Enabled = false;
                }
            }
            else
            {
                previewMorph_comboBox.Enabled = false;
            }
        }

        private void SetCheckBoxState(CheckBox box, bool state)
        {
            box.Checked = state;
            box.Enabled = state;
        }
        private void SetCheckBoxState(CheckBox box, bool state, Panel associatedPanel)
        {
            box.Checked = state;
            box.Enabled = state;
            associatedPanel.Enabled = state;
        }
        private void SetRadioButtonState(RadioButton radio, bool state)
        {
            radio.Checked = state;
            radio.Enabled = state;
        }

        internal void SetupRegionList()
        {
            if (meshRegionMap == null)
            {
                previewRegions_checkedListBox.ItemCheck -= previewRegions_checkedListBox_ItemCheck;
                previewRegions_checkedListBox.Items.Clear();
                previewRegions_checkedListBox.ItemCheck += previewRegions_checkedListBox_ItemCheck;
                return;
            }
            string[] regions = new string[LODmeshes[currentLOD].Count];
            for (int i = 0; i < LODmeshes[currentLOD].Count; i++)
            {
                uint[] rs = meshRegionMap.GetMeshRegions(LODtgis[currentLOD][i]);
                regions[i] = (Enum.IsDefined(typeof(XmodsEnums.CASPartRegionTS4), rs[0]) ?
                                Enum.GetName(typeof(XmodsEnums.CASPartRegionTS4), rs[0]) : rs[0].ToString());
                for (int j = 1; j < rs.Length; j++)
                {
                    regions[i] += "/" + (Enum.IsDefined(typeof(XmodsEnums.CASPartRegionTS4), rs[j]) ?
                                Enum.GetName(typeof(XmodsEnums.CASPartRegionTS4), rs[j]) : rs[j].ToString()); ;
                }
                regions[i] += ", Layer " + meshRegionMap.GetMeshLayer(LODtgis[currentLOD][i]).ToString();
                if (regions[i].IndexOf("HairHatB/HairHatD") >= 0) { regions[i] += " (hat cut straight)"; }
                else if (regions[i].IndexOf("HairHatD/HairHatC") >= 0) { regions[i] += " (hat cut tilted)"; }
                else if (regions[i].IndexOf("HairHatA/HairHatB/HairHatC/HairHatD") >= 0) { regions[i] += " (no hat)"; }
            }
            previewRegions_checkedListBox.ItemCheck -= previewRegions_checkedListBox_ItemCheck;
            previewRegions_checkedListBox.Items.Clear();
            previewRegions_checkedListBox.Items.AddRange(regions);
            for (int i = 0; i < previewRegions_checkedListBox.Items.Count; i++)
            {
                if (myCASP.BodyType == XmodsEnums.BodyType.Hair)
                {
                    if (regions[i].IndexOf("(no hat)") >= 0)
                        previewRegions_checkedListBox.SetItemChecked(i, true);
                }
                else
                {
                    previewRegions_checkedListBox.SetItemChecked(i, true);
                }
            }
            previewRegions_checkedListBox.ItemCheck += previewRegions_checkedListBox_ItemCheck;
        }

        private Image ExpandPartialImage(Image myImage, XmodsEnums.BodyType partType, XmodsEnums.Species species)
        {
            if (myImage == null) return null;
            if (species == XmodsEnums.Species.Human)
            {
                Bitmap final = new Bitmap(1024, 2048);
                Graphics g = Graphics.FromImage(final);
                g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;
                int x, y;
                if (partType == XmodsEnums.BodyType.Body | partType == XmodsEnums.BodyType.Top | partType == XmodsEnums.BodyType.Bottom)
                {
                    x = 0;
                    y = 1025;
                }
                else if (partType == XmodsEnums.BodyType.Shoes)
                {
                    x = 0;
                    y = 1537;
                }
                else if (partType == XmodsEnums.BodyType.Hair)
                {
                    x = 0;
                    y = 0;
                }
                else if (partType == XmodsEnums.BodyType.Hat)
                {
                    x = 513;
                    y = 257;
                }
                else
                {
                    if (myImage.Width <= 512)
                    {
                        x = 513;
                    }
                    else
                    {
                        x = 0;
                    }
                    y = 0;
                }
                g.DrawImage(myImage, x, y, myImage.Width, myImage.Height);
                Image tmp = (Image)final;
                return tmp;
            }
            else
            {
                Bitmap final = new Bitmap(2048, 1024);
                Graphics g = Graphics.FromImage(final);
                g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;
                int x = 0, y = 0;
                if (partType == XmodsEnums.BodyType.Body || partType == XmodsEnums.BodyType.Hat || partType == XmodsEnums.BodyType.Necklace)
                {
                    x = 1537;
                    y = 0;
                }
                g.DrawImage(myImage, x, y, myImage.Width, myImage.Height);
                Image tmp = (Image)final;
                return tmp;
            }
        }

        private Image DisplayableBumpMap(Bitmap bumpmapImage)
        {
            if (bumpmapImage == null) return null;
            Bitmap bumpmap = new Bitmap(bumpmapImage);
            Rectangle rect = new Rectangle(0, 0, bumpmap.Width, bumpmap.Height);
            System.Drawing.Imaging.BitmapData bmpData = bumpmap.LockBits(rect, ImageLockMode.ReadWrite,
                bumpmap.PixelFormat);

            IntPtr ptr;
            if (bmpData.Stride > 0)
            {
                ptr = bmpData.Scan0;
            }
            else
            {
                ptr = bmpData.Scan0 + bmpData.Stride * (bumpmap.Height - 1);
            }

            int bytes  = Math.Abs(bmpData.Stride) * bumpmap.Height;
            byte[] argbValues = new byte[bytes];

            System.Runtime.InteropServices.Marshal.Copy(ptr, argbValues, 0, bytes);

            for (int i = 0; i < argbValues.Length; i += 4)
            {
                int a = argbValues[i + 3] - 132;                        //get alpha value
                argbValues[i + 3] = 150;                                //alpha
                argbValues[i + 2] = colorAdjust(a, argbValues[i + 2]);  //red
                argbValues[i + 1] = colorAdjust(a, argbValues[i + 1]);  //green
                argbValues[i] = colorAdjust(a, argbValues[i]);          //blue
            }

            System.Runtime.InteropServices.Marshal.Copy(argbValues, 0, ptr, bytes);
            bumpmap.UnlockBits(bmpData);

            return bumpmap;
        }

        private byte colorAdjust(int a, int c)
        {
            int tmp = ((a + (c - 132)) * 2) + 132;
            tmp = Math.Min(tmp, 255);
            tmp = Math.Max(tmp, 0);
            return (byte)tmp;
        }

        private Image DisplayableShadow(Bitmap shadowImage)
        {
            if (shadowImage == null) return null;
            Bitmap shadow = new Bitmap(shadowImage);
            Rectangle rect = new Rectangle(0, 0, shadow.Width, shadow.Height);
            System.Drawing.Imaging.BitmapData bmpData = shadow.LockBits(rect, ImageLockMode.ReadWrite,
                shadow.PixelFormat);

            IntPtr ptr;
            if (bmpData.Stride > 0)
            {
                ptr = bmpData.Scan0;
            }
            else
            {
                ptr = bmpData.Scan0 + bmpData.Stride * (shadow.Height - 1);
            }

            int bytes = Math.Abs(bmpData.Stride) * shadow.Height;
            byte[] argbValues = new byte[bytes];

            System.Runtime.InteropServices.Marshal.Copy(ptr, argbValues, 0, bytes);

            for (int i = 0; i < argbValues.Length; i += 4)
            {
                if (argbValues[i + 3] > 0)
                {
                    int tmp = ((((int)argbValues[i + 2] + (int)argbValues[i]) / 2) - 150) * 3;
                    tmp = Math.Min(tmp, 255);
                    tmp = Math.Max(tmp, 0);
                    int a = 255 - tmp;
                    argbValues[i + 3] = (byte)a;        //alpha
                    argbValues[i + 2] = 0;              //red
                    argbValues[i + 1] = 0;              //green
                    argbValues[i] = 0;                  //blue
                }
            }
            System.Runtime.InteropServices.Marshal.Copy(argbValues, 0, ptr, bytes);
            shadow.UnlockBits(bmpData);

            return shadow;
        }

        private Image DisplayableSpecular(Bitmap specularImage)
        {
            if (specularImage == null) return null;
            Bitmap specular = new Bitmap(specularImage);

            Rectangle rect = new Rectangle(0, 0, specular.Width, specular.Height);
            System.Drawing.Imaging.BitmapData bmpData = specular.LockBits(rect, ImageLockMode.ReadWrite,
                specular.PixelFormat);
            IntPtr ptr;
            if (bmpData.Stride > 0)
            {
                ptr = bmpData.Scan0;
            }
            else
            {
                ptr = bmpData.Scan0 + bmpData.Stride * (specular.Height - 1);
            }
            int bytes = Math.Abs(bmpData.Stride) * specular.Height;
            byte[] argbValues = new byte[bytes];
            System.Runtime.InteropServices.Marshal.Copy(ptr, argbValues, 0, bytes);

            for (int i = 0; i < argbValues.Length; i += 4)
            {
               // int v = argbValues[i + 3] + argbValues[i + 1] + argbValues[i];
               //// byte v = Math.Max(argbValues[i + 3], argbValues[i + 1]);
               //// v = Math.Max(v, argbValues[i]);
               // if (v > 255) v = 255;
               //// if (v < 0) v = 0;
               // argbValues[i + 3] = (byte)v;      //alpha

               // argbValues[i + 3] = (byte)Math.Max(argbValues[i + 3], argbValues[i + 1]);      //alpha
                argbValues[i + 2] = 255;          //red
                argbValues[i + 1] = 255;          //green
                argbValues[i] = 255;              //blue
            }
            System.Runtime.InteropServices.Marshal.Copy(argbValues, 0, ptr, bytes);
            specular.UnlockBits(bmpData);

            return specular ;
        }

        private Image DisplayableGlow(Bitmap glowImage)
        {
            if (glowImage == null) return null;
            Bitmap glow = new Bitmap(glowImage);
            Rectangle rect = new Rectangle(0, 0, glow.Width, glow.Height);
            System.Drawing.Imaging.BitmapData bmpData = glow.LockBits(rect, ImageLockMode.ReadWrite,
                glow.PixelFormat);

            IntPtr ptr;
            if (bmpData.Stride > 0)
            {
                ptr = bmpData.Scan0;
            }
            else
            {
                ptr = bmpData.Scan0 + bmpData.Stride * (glow.Height - 1);
            }

            int bytes = Math.Abs(bmpData.Stride) * glow.Height;
            byte[] argbValues = new byte[bytes];

            System.Runtime.InteropServices.Marshal.Copy(ptr, argbValues, 0, bytes);

            for (int i = 0; i < argbValues.Length; i += 4)
            {
                if (argbValues[i] == 0 && argbValues[i + 1] == 0 && argbValues[i + 2] == 0)
                {
                    argbValues[i + 3] = 0;              //alpha
                }
                else
                {
                    byte tmp = Math.Max(argbValues[i], argbValues[i + 1]);
                    tmp = Math.Max(tmp, argbValues[i + 2]);
                    argbValues[i + 3] = tmp;           //alpha
                }
            }
            System.Runtime.InteropServices.Marshal.Copy(argbValues, 0, ptr, bytes);
            glow.UnlockBits(bmpData);

            return glow;
        }

        private void previewRegions_checkedListBox_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            casPartPreviewer1.SetRegionMesh(e.Index, e.NewValue == CheckState.Checked);
        }

        private void previewHead_checkBox_CheckedChanged(object sender, EventArgs e)
        {
            casPartPreviewer1.SetHeadDisplay(previewHead_checkBox.Checked);
        }

        private void previewTop_checkBox_CheckedChanged(object sender, EventArgs e)
        {
            casPartPreviewer1.SetTopDisplay(previewTop_checkBox.Checked);
        }

        private void previewBottom_checkBox_CheckedChanged(object sender, EventArgs e)
        {
            casPartPreviewer1.SetBottomDisplay(previewBottom_checkBox.Checked);
        }

        private void previewFeet_checkBox_CheckedChanged(object sender, EventArgs e)
        {
            casPartPreviewer1.SetFeetDisplay(previewFeet_checkBox.Checked);
        }

        private void previewUndies_checkBox_CheckedChanged(object sender, EventArgs e)
        {
            casPartPreviewer1.SetUndies(previewUndies_checkBox.Checked);
        }

        private void previewBody_checkBox_CheckedChanged(object sender, EventArgs e)
        {
            casPartPreviewer1.SetBodyDisplay(previewBody_checkBox.Checked);
        }

        private void previewEarsChange(object sender, EventArgs e)
        {
            previewEars_panel.Enabled = previewEars_checkBox.Checked;
            casPartPreviewer1.SetEarsDisplay(previewEars_checkBox.Checked,
                previewEarsDown_radioButton.Checked ? XmodsEnums.BodySubType.EarsDown : XmodsEnums.BodySubType.EarsUp);
        }

        private void previewTailChange(object sender, EventArgs e)
        {
            XmodsEnums.BodySubType subType;
            if (myCASP.Species == XmodsEnums.Species.Human)
            {
                previewTail_panel.Enabled = false;
                subType = XmodsEnums.BodySubType.None;
            }
            else
            {
                previewTail_panel.Enabled = previewTail_checkBox.Checked;          
                if (previewTailRing_radioButton.Checked) subType = XmodsEnums.BodySubType.TailRing;
                else if (previewTailScrew_radioButton.Checked) subType = XmodsEnums.BodySubType.TailScrew;
                else if (previewTailStub_radioButton.Checked) subType = XmodsEnums.BodySubType.TailStub;
                else subType = XmodsEnums.BodySubType.TailLong;
            }
            casPartPreviewer1.SetTailDisplay(previewTail_checkBox.Checked, subType);
        }
        
        private void previewSkinColor_pictureBox_Click(object sender, EventArgs e)
        {
            ColorDialog color = new ColorDialog();
            color.FullOpen = true;
            color.CustomColors = customColors;
            DialogResult res = color.ShowDialog();
            if (res == DialogResult.OK)
            {
                previewSkinColor_pictureBox.BackColor = color.Color;
                casPartPreviewer1.SetSkinColor(color.Color);
            }
        }

        private void previewSkin_checkBox_CheckedChanged(object sender, EventArgs e)
        {
            casPartPreviewer1.SetSkinOverlay(previewSkin_checkBox.Checked);
        }

        private void previewDiffuse_checkBox_CheckedChanged(object sender, EventArgs e)
        {
            if (previewDiffuse_checkBox.Checked) previewBumpmap_checkBox.Checked = false;
            casPartPreviewer1.SetDiffuse(previewDiffuse_checkBox.Checked);
        }

        private void previewSpecular_checkBox_CheckedChanged(object sender, EventArgs e)
        {
            casPartPreviewer1.SetSpecular(previewSpecular_checkBox.Checked);
        }

        private void previewShadow_checkBox_CheckedChanged(object sender, EventArgs e)
        {
            if (previewShadow_checkBox.Checked) previewBumpmap_checkBox.Checked = false;
            casPartPreviewer1.SetShadow(previewShadow_checkBox.Checked);
        }

        private void previewBumpMap_checkBox_CheckedChanged(object sender, EventArgs e)
        {
            casPartPreviewer1.SetBumpmap(previewBumpmap_checkBox.Checked);
        }

        private void previewGlowmap_checkBox_CheckedChanged(object sender, EventArgs e)
        {
            if (previewGlowmap_checkBox.Checked) previewBumpmap_checkBox.Checked = false;
            casPartPreviewer1.SetGlowmap(previewGlowmap_checkBox.Checked);
        }

        private void previewLight1_checkBox_CheckedChanged(object sender, EventArgs e)
        {
            casPartPreviewer1.SetLights(previewLight1_checkBox.Checked, previewLight2_checkBox.Checked);
        }

        private void previewLight2_checkBox_CheckedChanged(object sender, EventArgs e)
        {
            casPartPreviewer1.SetLights(previewLight1_checkBox.Checked, previewLight2_checkBox.Checked);
        }

        private void previewLOD0_radioButton_CheckedChanged(object sender, EventArgs e)
        {
            setLOD();
        }

        private void previewLOD1_radioButton_CheckedChanged(object sender, EventArgs e)
        {
            setLOD();
        }

        private void previewLOD2_radioButton_CheckedChanged(object sender, EventArgs e)
        {
            setLOD();
        }

        private void previewLOD3_radioButton_CheckedChanged(object sender, EventArgs e)
        {
            setLOD();
        }

        private void setLOD()
        {
            if (previewLOD0_radioButton.Checked)
            {
                currentLOD = 0;
            }
            else if (previewLOD1_radioButton.Checked)
            {
                currentLOD = 1;
            }
            else if (previewLOD2_radioButton.Checked)
            {
                currentLOD = 2;
            }
            else if (previewLOD3_radioButton.Checked)
            {
                currentLOD = 3;
            }
            casPartPreviewer1.SetLOD(LODmeshes[currentLOD].ToArray(), currentLOD);
            SetupRegionList();
            if (myCASP.BodyType == XmodsEnums.BodyType.Hair)
            {
                for (int i = 0; i < previewRegions_checkedListBox.Items.Count - 1; i++)
                {
                    casPartPreviewer1.SetRegionMesh(i, previewRegions_checkedListBox.GetItemChecked(i));
                }
            }
        }

        private void previewUnisex_button_Click(object sender, EventArgs e)
        {
            if (previewUnisex_button.Text.IndexOf(" Female ") > -1)
            {
                casPartPreviewer1.SetGender(XmodsEnums.Gender.Female);
                previewUnisex_button.Text = "Change to Male Model";
                SetupPreviewMorphs(XmodsEnums.Gender.Female);
            }
            else if (previewUnisex_button.Text.IndexOf(" Male ") > -1)
            {
                casPartPreviewer1.SetGender(XmodsEnums.Gender.Male);
                previewUnisex_button.Text = "Change to Female Model";
                SetupPreviewMorphs(XmodsEnums.Gender.Male);
            }
        }

        private void previewMorph_comboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (previewMorph_comboBox.SelectedIndex == 0)
            {
                casPartPreviewer1.SetMorph(LODmeshes[currentLOD].ToArray(), null, null);
                return;
            }
            Predicate<IResourceIndexEntry> morphShape = r => r.ResourceType == (uint)XmodsEnums.ResourceTypes.DMAP &
                r.Instance == Xmods.DataLib.FNVhash.FNV64(previewMorph_comboBox.Items[previewMorph_comboBox.SelectedIndex].ToString() + "_Shape");
            MorphMap shapeMap = null;
            foreach (Package p in gamePacks0)
            {
                IResourceIndexEntry ires = p.Find(morphShape);
                if (ires != null)
                {
                    Stream s = p.GetResource(ires);
                    s.Position = 0;
                    if (s.Length < 16)
                    {
                        MessageBox.Show("The Deformer Shape Map for this morph is empty!");
                        return;
                    }
                    DMap tmp = new DMap(new BinaryReader(s));
                    if (!tmp.HasData)
                    {
                        MessageBox.Show("There is no data in the Deformer Shape Map for this morph!");
                        return;
                    }
                    shapeMap = tmp.ToMorphMap(true);
                    break;
                }
            }
            if (shapeMap == null)
            {
                MessageBox.Show("No Deformer Shape Map exists for this morph!" + Xmods.DataLib.FNVhash.FNV64(previewMorph_comboBox.Items[previewMorph_comboBox.SelectedIndex].ToString() + "_Shape").ToString("X8"));
                return;
            }

            Predicate<IResourceIndexEntry> morphNormals = r => r.ResourceType == (uint)XmodsEnums.ResourceTypes.DMAP &
                r.Instance == Xmods.DataLib.FNVhash.FNV64(previewMorph_comboBox.Items[previewMorph_comboBox.SelectedIndex].ToString() + "_Normals");
            MorphMap normalMap = null;
            foreach (Package p in gamePacks0)
            {
                IResourceIndexEntry ires = p.Find(morphNormals);
                if (ires != null)
                {
                    Stream s = p.GetResource(ires);
                    s.Position = 0;
                    if (s.Length < 16)
                    {
                        MessageBox.Show("The Deformer Normals Map for this morph is empty!");
                        return;
                    }
                    DMap tmp = new DMap(new BinaryReader(s));
                    if (!tmp.HasData)
                    {
                        MessageBox.Show("There is no data in the Deformer Normals Map for this morph!");
                        return;
                    }
                    normalMap = tmp.ToMorphMap(previewMorph_comboBox.Items[previewMorph_comboBox.SelectedIndex].ToString().IndexOf("Belly_Big") < 0 &&
                                               previewMorph_comboBox.Items[previewMorph_comboBox.SelectedIndex].ToString().IndexOf("Butt_Big") < 0);
                    break;
                }
            }
            if (normalMap == null)
            {
                MessageBox.Show("No Deformer Normals Map exists for this morph!" + Xmods.DataLib.FNVhash.FNV64(previewMorph_comboBox.Items[previewMorph_comboBox.SelectedIndex].ToString() + "_Shape").ToString("X8"));
                return;
            }

            casPartPreviewer1.SetMorph(LODmeshes[currentLOD].ToArray(), shapeMap, normalMap);
        }
    }
}
