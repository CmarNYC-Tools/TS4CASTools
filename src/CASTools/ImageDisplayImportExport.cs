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
using XMODS;
using s4pi.Interfaces;
using s4pi.Package;
using s4pi.ImageResource;

namespace XMODS
{
    public partial class ImageDisplayImportExport : Form
    {
        RLEResource rle = null;
        DSTResource dst = null;
        DdsFile dds = null;
        Bitmap img = null;
        string PNGfilter = "PNG image files (*.png)|*.png|All files (*.*)|*.*";
        string DDSfilter = "DDS image files (*.png)|*.png|All files (*.*)|*.*";
        string ImageImportfilter = "PNG or DDS files (*.png, *.dds)|*.png; *.dds|All files (*.*)|*.*";
        string ImageExportfilter = "PNG files (*.png)|*.png|DDS files (*.dds)|*.dds|All files (*.*)|*.*";
        Form1.ImageType type;
        XmodsEnums.Species species;
        XmodsEnums.BodyType bodyType;

        public RLEResource ReturnRLE
        {
            get { return rle; }
        }
        public LRLE ReturnLRLE
        {
            get { if (img != null) return new LRLE((Bitmap)img); else return null; }
        }
        public DSTResource ReturnDST
        {
            get { return dst; }
        }
        public Image ReturnIMG
        {
            get { return img; }
        }
        public bool UseLRLE
        {
            get { return this.CloneTextureLRLE_checkBox.Checked; }
        }

        public ImageDisplayImportExport(AResource aImage, Form1.ImageType imageType, string windowTitle, XmodsEnums.Species species, XmodsEnums.BodyType type, bool useLRLE= false)
        {
            InitializeComponent();
            this.Text = windowTitle;
            this.species = species;
            bodyType = type;
            dds = new DdsFile();
            switch (aImage)
            {
                case DSTResource dstImage:
                    if (dstImage != null)
                    {
                        dst = new DSTResource(1, dstImage.Stream);
                        DSTResource tmp = new DSTResource(1, dstImage.Stream);
                        dds.Load(tmp.ToDDS(), false);
                        img = dds.Image;
                    }
                    else
                    {
                        dst = null;
                        dds = null;
                        img = null;
                    }
                    break;
                case RLEResource rleImage:
                    if (rleImage != null)
                    {
                        rle = new RLEResource(1, rleImage.Stream);
                        dds.Load(rleImage.ToDDS(), false);
                        img = dds.Image;
                    }
                    else
                    {
                        rle = null;
                        dds = null;
                        img = null;
                    }
                    if (imageType == Form1.ImageType.Material)
                    {
                        CloneTextureLRLE_checkBox.Enabled = true;
                        CloneTextureLRLE_checkBox.Checked = useLRLE;
                    }
                    break;
            }
            DoSetup(img, imageType);
        }
        public ImageDisplayImportExport(LRLE lrleImage, Form1.ImageType imageType, string windowTitle, XmodsEnums.Species species, XmodsEnums.BodyType type)
        {
            InitializeComponent();
            this.Text = windowTitle;
            this.species = species;
            bodyType = type;
            rle = null;
            dds = null;
            img = lrleImage != null ? lrleImage.image : null;
            CloneTextureLRLE_checkBox.Enabled = true;
            CloneTextureLRLE_checkBox.Checked = true;
            DoSetup(img, imageType);
        }

        public ImageDisplayImportExport(ThumbnailResource ThumbImage)
        {
            InitializeComponent();
            this.Text = "Import/Export Thumbnail";
            if (ThumbImage != null)
            {
                img = ThumbImage.Image;
                DisplayImage(img);
            }
            else
            {
                img = null;
                DisplayImage(null);
                cloneTextureExport_button.Enabled = false;
            }
            cloneTextureSave_button.Enabled = false;
            cloneTextureBlank_button.Text = "Remove";
            type = Form1.ImageType.Thumbnail;
        }

        public ImageDisplayImportExport(Image image)
        {
            InitializeComponent();
            this.Text = "Import Overlay for Thumbnails";
            if (image != null)
            {
                img = (Bitmap)image;
                DisplayImage(img);
            }
            else
            {
                img = null;
                DisplayImage(null);
                cloneTextureExport_button.Enabled = false;
            }
            cloneTextureSave_button.Enabled = false;
            cloneTextureBlank_button.Text = "Remove";
            type = Form1.ImageType.Thumbnail;
        }

        internal void DoSetup(Bitmap image, Form1.ImageType imageType)
        {
            if (image != null)
            {
                DisplayImage(image);
            }
            else
            {
                DisplayImage(null);
                cloneTextureExport_button.Enabled = false;
            }
            cloneTextureSave_button.Enabled = false;
            if (imageType == Form1.ImageType.Specular) cloneTextureMask_checkBox.Visible = true;
            if (imageType == Form1.ImageType.GlowMap || imageType == Form1.ImageType.Swatch || imageType == Form1.ImageType.Thumbnail|| imageType == Form1.ImageType.ColorShiftMask)
            {
                cloneTextureBlank_button.Text = "Remove";
            }
            else
            {
                cloneTextureBlank_button.Text = "Make Blank";
            }
            type = imageType;
            cloneTextureMask_checkBox.Checked = Properties.Settings.Default.SpecularMask;
        }

        private void DisplayImage(Image image)
        {
            if (image == null)
            {
                cloneTextureDimensions.Text = "No image";
                cloneTexture_pictureBox.Width = 197;
                cloneTexture_pictureBox.Height = 223;
                this.Width = 360;
                this.Height = 315;
                cloneTexture_pictureBox.Image = Properties.Resources.NullImage;
                cloneTextureExport_button.Enabled = false;
                return;
            }
            cloneTextureDimensions.Text = image.Width.ToString() + "x" + image.Height.ToString();
            int h = Math.Min(image.Height + 150, Screen.FromControl(this).WorkingArea.Height / 2);
            float zoom = (float)h / (float)image.Height;
            if (zoom > 1) zoom = 1;
            int pboxWidth = (int)(image.Width * zoom);
            int pboxHeight = (int)(image.Height * zoom);
            this.Width = Math.Max(pboxWidth + 163, 360);
            this.Height = Math.Max(pboxHeight + 92, 315);
            cloneTexture_pictureBox.Width = pboxWidth;
            cloneTexture_pictureBox.Height = pboxHeight;
            cloneTexture_pictureBox.Image = image;
            cloneTextureExport_button.Enabled = true;
        }

        private void cloneTextureExport_button_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            if (type == Form1.ImageType.Thumbnail)
            {
                saveFileDialog1.Filter = PNGfilter;
                saveFileDialog1.Title = "Save Thumbnail Image File";
                saveFileDialog1.DefaultExt = "png";
            }
            else
            {
                saveFileDialog1.Filter = ImageExportfilter;
                saveFileDialog1.Title = "Save Texture Image File";
                saveFileDialog1.DefaultExt = "png";
            }
            saveFileDialog1.FilterIndex = 1;
            saveFileDialog1.CheckFileExists = false;
            saveFileDialog1.CheckPathExists = true;
            saveFileDialog1.OverwritePrompt = true;
            saveFileDialog1.AddExtension = true;
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (type == Form1.ImageType.Thumbnail)
                {
                    using (FileStream myStream = new FileStream(saveFileDialog1.FileName, FileMode.Create, FileAccess.Write))
                    {
                        img.Save(myStream, System.Drawing.Imaging.ImageFormat.Png);
                        myStream.Close();
                    }
                }
                else if (type == Form1.ImageType.BumpMap || type == Form1.ImageType.GlowMap || type == Form1.ImageType.Swatch)
                {
                    using (FileStream myStream = new FileStream(saveFileDialog1.FileName, FileMode.Create, FileAccess.Write))
                    {
                        if (String.Compare(Path.GetExtension(saveFileDialog1.FileName).ToLower(), ".png") == 0)
                        {
                            dds.Image.Save(myStream, System.Drawing.Imaging.ImageFormat.Png);
                        }
                        else
                        {
                            DSTResource tmp = new DSTResource(1, dst.Stream);
                            tmp.ToDDS().CopyTo(myStream);
                        }
                    }
                }
                else 
                {
                    using (FileStream myStream = new FileStream(saveFileDialog1.FileName, FileMode.Create, FileAccess.Write))
                    {
                        if (String.Compare(Path.GetExtension(saveFileDialog1.FileName).ToLower(), ".png") == 0)
                        {
                            if (img != null)
                            {
                                img.Save(myStream, System.Drawing.Imaging.ImageFormat.Png);
                            }
                            else if (dds != null)
                            {
                                dds.Image.Save(myStream, System.Drawing.Imaging.ImageFormat.Png);
                            }
                        }
                        else
                        {
                            if (dds != null)
                            {
                                dds.Save(myStream);
                            }
                            else if (rle != null)
                            {
                                rle.ToDDS().CopyTo(myStream);
                            }
                            else if (img != null)
                            {
                                dds = new DdsFile();
                                dds.CreateImage(img, false);
                                dds.UseDXT = false;
                                dds.Save(myStream);
                            }
                        }
                    }
                    if (type == Form1.ImageType.Specular && cloneTextureMask_checkBox.Checked)
                    {
                        string maskname = Path.GetDirectoryName(saveFileDialog1.FileName) + Path.DirectorySeparatorChar +
                            Path.GetFileNameWithoutExtension(saveFileDialog1.FileName) + ".mask" + Path.GetExtension(saveFileDialog1.FileName);
                        using (FileStream myStream = File.Open(maskname, FileMode.Create, FileAccess.Write))
                        {
                            if (String.Compare(Path.GetExtension(saveFileDialog1.FileName).ToLower(), ".png") == 0)
                            {
                                rle.ToSpecularMaskImage().Save(myStream, System.Drawing.Imaging.ImageFormat.Png);
                                myStream.Close();
                            }
                            else
                            {
                                DdsFile tmp = new DdsFile();
                                tmp.Load(rle.ToSpecularMaskDDS(), false);
                                tmp.UseDXT = true;
                                tmp.AlphaDepth = 1;
                                tmp.Save(myStream);
                                myStream.Close();
                            }
                        }
                    }
                }
            }
        }

        private void cloneTextureImport_button_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            if (type == Form1.ImageType.Thumbnail)
            {
                openFileDialog1.Filter = PNGfilter;
                openFileDialog1.Title = "Select Thumbnail Image File";
            }
            else
            {
                openFileDialog1.Filter = ImageImportfilter;
                if (type == Form1.ImageType.BumpMap)
                {
                    openFileDialog1.Title = "Select Bumpmap Image File";
                }
                else if (type == Form1.ImageType.GlowMap)
                {
                    openFileDialog1.Title = "Select Emission Glow Map Image File";
                }
                else if (type == Form1.ImageType.Material)
                {
                    openFileDialog1.Title = "Select Material Texture Image File";
                }
                else if (type == Form1.ImageType.Shadow)
                {
                    openFileDialog1.Title = "Select Shadow Image File";
                }
                else if (type == Form1.ImageType.Specular)
                {
                    openFileDialog1.Title = "Select Specular Image File";
                }
                else if (type == Form1.ImageType.Swatch)
                {
                    openFileDialog1.Title = "Select Swatch Image File";
                }
                else if (type == Form1.ImageType.ColorShiftMask)
                {
                    openFileDialog1.Title = "Select Color Shift Mask Image File";
                }
            }
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.CheckFileExists = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (type == Form1.ImageType.Thumbnail)
                {
                    using (FileStream myStream = new FileStream(openFileDialog1.FileName, FileMode.Open, FileAccess.Read))
                    {
                        img = (Bitmap)Image.FromStream(myStream, true);
                        DisplayImage(img);
                    }
                }
                else if (type == Form1.ImageType.Material)
                {
                    using (FileStream myStream = new FileStream(openFileDialog1.FileName, FileMode.Open, FileAccess.Read))
                    {
                        rle = new RLEResource(1, null);
                        if (String.Compare(Path.GetExtension(openFileDialog1.FileName).ToLower(), ".png") == 0)
                        {
                            img = new Bitmap(myStream);
                            rle.ImportToRLE(GetConvertedPNG(img, true, true));      //sets dds
                        }
                        else
                        {
                            dds = new DdsFile();
                            dds.Load(myStream, false);
                            bool oldDXT = dds.UseDXT;
                            if (!dds.UseDXT) dds.UseDXT = true;
                            Stream m = new MemoryStream();
                            dds.Save(m);
                            m.Position = 0;
                            rle.ImportToRLE(m);
                            if (dds.UseDXT != oldDXT) dds.UseDXT = oldDXT;
                            img = dds.Image;
                        }
                    }
                    DisplayImage(img);
                }
                else if (type == Form1.ImageType.ColorShiftMask)
                {
                    using (FileStream myStream = new FileStream(openFileDialog1.FileName, FileMode.Open, FileAccess.Read))
                    {
                        rle = new RLEResource(1, null);
                        if (String.Compare(Path.GetExtension(openFileDialog1.FileName).ToLower(), ".png") == 0)
                        {
                            img = new Bitmap(myStream);
                            var ddsStream = img.ToColorShiftMask();
                            rle.ImportToRLE(ddsStream);      //sets dds
                        }
                        else
                        {
                            dds = new DdsFile();
                            dds.Load(myStream, false);
                            bool oldDXT = dds.UseDXT;
                            if (!dds.UseDXT) dds.UseDXT = true;
                            Stream m = new MemoryStream();
                            dds.Save(m);
                            m.Position = 0;
                            rle.ImportToRLE(m);
                            if (dds.UseDXT != oldDXT) dds.UseDXT = oldDXT;
                            img = dds.Image;
                        }
                    }
                    DisplayImage(img);
                }
                else if (type == Form1.ImageType.Shadow)
                {
                    rle = new RLEResource(1, null);
                    using (FileStream myStream = new FileStream(openFileDialog1.FileName, FileMode.Open, FileAccess.Read))
                    {
                        if (String.Compare(Path.GetExtension(openFileDialog1.FileName).ToLower(), ".png") == 0)
                        {
                            img = new Bitmap(myStream);
                            rle.ImportToRLE(GetConvertedPNG(img, true, true));  //sets dds
                        }
                        else
                        {
                            dds = new DdsFile();
                            dds.Load(myStream, false);
                            bool oldDXT = dds.UseDXT;
                            if (!dds.UseDXT) dds.UseDXT = true;
                            Stream m = new MemoryStream();
                            dds.Save(m);
                            m.Position = 0;
                            rle.ImportToRLE(m);
                            if (dds.UseDXT != oldDXT) dds.UseDXT = oldDXT;
                            img = dds.Image;
                        }
                    }
                    DisplayImage(img);
                }
                else if (type == Form1.ImageType.Specular)
                {
                    rle = new RLEResource(1, null);
                    using (FileStream myStream = new FileStream(openFileDialog1.FileName, FileMode.Open, FileAccess.Read))
                    {
                        if (cloneTextureMask_checkBox.Checked)
                        {
                            string maskname = Path.GetDirectoryName(openFileDialog1.FileName) + Path.DirectorySeparatorChar +
                            Path.GetFileNameWithoutExtension(openFileDialog1.FileName) + ".mask" + Path.GetExtension(openFileDialog1.FileName);
                            if (!File.Exists(maskname))
                            {
                                DialogResult res = MessageBox.Show("Mask image file not found! Import using only main texture?", "Mask not found",
                                    MessageBoxButtons.OKCancel);
                                if (res == DialogResult.Cancel) return;
                                if (String.Compare(Path.GetExtension(openFileDialog1.FileName).ToLower(), ".png") == 0)
                                {
                                    img = new Bitmap(myStream);
                                    rle.ImportToRLE(GetConvertedPNG(img, true, true), RLEResource.RLEVersion.RLES);
                                }
                                else
                                {
                                    dds = new DdsFile();
                                    dds.Load(myStream, false);
                                    bool oldDXT = dds.UseDXT;
                                    if (!dds.UseDXT) dds.UseDXT = true;
                                    Stream m = new MemoryStream();
                                    dds.Save(m);
                                    m.Position = 0;
                                    rle.ImportToRLE(m, RLEResource.RLEVersion.RLES);
                                    if (dds.UseDXT != oldDXT) dds.UseDXT = oldDXT;
                                    img = dds.Image;
                                }
                            }
                            else
                            {
                                using (FileStream maskStream = new FileStream(maskname, FileMode.Open, FileAccess.Read))
                                {
                                    Stream mainStream;
                                    DdsFile maskTmp = new DdsFile();
                                    if (String.Compare(Path.GetExtension(openFileDialog1.FileName).ToLower(), ".png") == 0)
                                    {
                                        img = new Bitmap(myStream);
                                        mainStream = GetConvertedPNG(img, true, false); //sets dds
                                    }
                                    else
                                    {
                                        mainStream = myStream;
                                    }
                                    if (String.Compare(Path.GetExtension(maskname).ToLower(), ".png") == 0)
                                    {
                                        Image m = new Bitmap(maskStream);
                                        maskTmp.Load(GetConvertedPNG(m, true, false), false);      //sets dds                                  
                                    }
                                    else
                                    {
                                        maskTmp.Load(maskStream, false);
                                    }
                                    maskTmp.UseDXT = false;
                                    MemoryStream mask = new MemoryStream();
                                    maskTmp.Save(mask);
                                    mask.Position = 0;
                                    rle.ImportToRLESwithMask(mainStream, mask);
                                    mask.Dispose();
                                }
                            }
                        }
                        else
                        {
                            if (String.Compare(Path.GetExtension(openFileDialog1.FileName).ToLower(), ".png") == 0)
                            {
                                img = new Bitmap(myStream);
                                rle.ImportToRLE(GetConvertedPNG(img, true, true), RLEResource.RLEVersion.RLES); //sets dds
                            }
                            else
                            {
                                dds = new DdsFile();
                                dds.Load(myStream, false);
                                bool oldDXT = dds.UseDXT;
                                if (!dds.UseDXT) dds.UseDXT = true;
                                Stream m = new MemoryStream();
                                dds.Save(m);
                                m.Position = 0;
                                rle.ImportToRLE(m, RLEResource.RLEVersion.RLES);
                                if (dds.UseDXT != oldDXT) dds.UseDXT = oldDXT;
                                img = dds.Image;
                            }
                        }
                    }
                    DisplayImage(img);
                }
                else if (type == Form1.ImageType.BumpMap || type == Form1.ImageType.GlowMap || type == Form1.ImageType.Swatch)
                {
                    dst = new DSTResource(1, null);
                    using (FileStream myStream = new FileStream(openFileDialog1.FileName, FileMode.Open, FileAccess.Read))
                    {
                        if (String.Compare(Path.GetExtension(openFileDialog1.FileName).ToLower(), ".png") == 0)
                        {
                            img = new Bitmap(myStream);
                            dst.ImportToDST(GetConvertedPNG(img, true, true));
                        }
                        else
                        {
                            dst.ImportToDST(myStream);
                        }
                    }
                    DSTResource tmp2 = new DSTResource(1, dst.Stream);
                    dds = new DdsFile();
                    dds.Load(tmp2.ToDDS(), false);
                    img = dds.Image;
                    DisplayImage(img);
                }
                cloneTextureSave_button.Enabled = true;
            }
        }

        private Stream GetConvertedPNG(Image image, bool generateMipMaps, bool forceDXT)
        {
            PleaseWait_label.Location = new Point(this.Width / 2 - PleaseWait_label.Width / 2, this.Height / 2 - PleaseWait_label.Height / 2);
            PleaseWait_label.Visible = true;
            PleaseWait_label.Refresh();
            dds = new DdsFile();
            dds.CreateImage(image, false);
            if (generateMipMaps) dds.GenerateMipMaps();
            bool oldDXT = dds.UseDXT;
            if (forceDXT && !dds.UseDXT) dds.UseDXT = true;
            MemoryStream m = new MemoryStream();
            dds.Save(m);
            m.Position = 0;
            if (dds.UseDXT != oldDXT) dds.UseDXT = oldDXT;
            PleaseWait_label.Visible = false;
            PleaseWait_label.Refresh();
            return m;
        }

        private void cloneTextureBlank_button_Click(object sender, EventArgs e)
        {
            if (type == Form1.ImageType.Thumbnail)
            {
                img = null;
                DisplayImage(null);
            }
            else if (type == Form1.ImageType.GlowMap || type == Form1.ImageType.Swatch)
            {
                dst = null;
                DisplayImage(null);
            }
            else
            {
                dds = new DdsFile();
                if (type == Form1.ImageType.BumpMap)
                {
                    if (species == XmodsEnums.Species.Human)
                    {
                        if (bodyType == XmodsEnums.BodyType.Body || bodyType == XmodsEnums.BodyType.Hair) dst = new DSTResource(1, new MemoryStream(Properties.Resources.blankBump));
                        else if (bodyType == XmodsEnums.BodyType.Hat) dst = new DSTResource(1, new MemoryStream(Properties.Resources.blankBump_hat));
                        else if (bodyType == XmodsEnums.BodyType.Shoes) dst = new DSTResource(1, new MemoryStream(Properties.Resources.blankBump_shoes));
                        else dst = null;
                    }
                    else
                    {
                        if (bodyType == XmodsEnums.BodyType.Body) dst = new DSTResource(1, new MemoryStream(Properties.Resources.blankBump_pet));
                        else if (bodyType == XmodsEnums.BodyType.Hat) dst = new DSTResource(1, new MemoryStream(Properties.Resources.blankBump_hat));
                        else if (bodyType == XmodsEnums.BodyType.Necklace) dst = new DSTResource(1, new MemoryStream(Properties.Resources.blankBump_necklace));
                        else dst = null;
                    }
                    if (dst != null) dds.Load(dst.ToDDS(), false);
                    img = dds.Image;
                }
                else if (type == Form1.ImageType.Shadow || type == Form1.ImageType.Material)
                {
                    if (species == XmodsEnums.Species.Human)
                    {
                        rle = new RLEResource(1, new MemoryStream(Properties.Resources.BlankShadow));
                    }
                    else
                    {
                        rle = new RLEResource(1, new MemoryStream(Properties.Resources.BlankShadow_pet));
                    }
                    dds.Load(rle.ToDDS(), false);
                    img = dds.Image;
                }
                else if (type == Form1.ImageType.Specular)
                {
                    if (species == XmodsEnums.Species.Human)
                    {
                        rle = new RLEResource(1, new MemoryStream(Properties.Resources.BlankSpecular));
                    }
                    else
                    {
                        rle = new RLEResource(1, new MemoryStream(Properties.Resources.BlankSpecular_pet));
                    }
                    dds.Load(rle.ToDDS(), false);
                    img = dds.Image;
                }
                DisplayImage(img);
            }
            cloneTextureSave_button.Enabled = true;
        }

        private void cloneTextureSave_button_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void cloneTextureCancel_button_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void cloneTextureMask_checkBox_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.SpecularMask = cloneTextureMask_checkBox.Checked;
        }
    }
}
