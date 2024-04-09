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
using s4pi.ImageResource;
using s4pi.Interfaces;
using s4pi.Package;

namespace XMODS
{
    public partial class Form1 : Form
    {
        int thumbManagerIndex = 0;
        Image thumbOverlay = null;

        private void CloneThumbsList()
        {
            CloneThumbs_dataGridView.Rows.Clear();
            foreach (CASPinfo c in clonePackCASPs)
            {
                object[] obj = new object[4] { c.Casp.PartName, c.maleThumb != null ? c.maleThumb.Image : Properties.Resources.NullImage, 
                                        c.femaleThumb != null ? c.femaleThumb.Image : Properties.Resources.NullImage, c.Casp.SwatchOrder.ToString().PadLeft(3) };
                int ind = CloneThumbs_dataGridView.Rows.Add(obj);
                CloneThumbs_dataGridView.Rows[ind].Tag = ind;
                CloneThumbs_dataGridView.Rows[ind].Cells["CASPthumbMale"].Tag = c.maleThumb;
                CloneThumbs_dataGridView.Rows[ind].Cells["CASPthumbFemale"].Tag = c.femaleThumb;
            }
            CloneThumbs_dataGridView.Sort(CloneThumbs_dataGridView.Columns["SortOrder"], ListSortDirection.Ascending);
            for (int i = 0; i < CloneThumbs_dataGridView.Rows.Count; i++)
            {
                CloneThumbs_dataGridView.Rows[i].Cells[0].Selected = false;
            }
            CloneThumbs_dataGridView.Rows[myRow].Cells[0].Selected = true;
            thumbManagerIndex = 0;
            CloneThumbsMale_pictureBox.Image = (Image)CloneThumbs_dataGridView.Rows[0].Cells["CASPthumbMale"].Value;
            CloneThumbsFemale_pictureBox.Image = (Image)CloneThumbs_dataGridView.Rows[0].Cells["CASPthumbFemale"].Value;
        }

        private void CloneThumbs_dataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            thumbManagerIndex = e.RowIndex;
            CloneThumbsMale_pictureBox.Image = (Image)CloneThumbs_dataGridView.Rows[e.RowIndex].Cells["CASPthumbMale"].Value;
            CloneThumbsFemale_pictureBox.Image = (Image)CloneThumbs_dataGridView.Rows[e.RowIndex].Cells["CASPthumbFemale"].Value;
        }

        private void CloneThumbsMale_pictureBox_Click(object sender, EventArgs e)
        {
            if (thumbManagerIndex > CloneThumbs_dataGridView.Rows.Count - 1) return;
            ThumbnailResource t = null;
            if (ImportExportThumb((ThumbnailResource)CloneThumbs_dataGridView.Rows[thumbManagerIndex].Cells["CASPthumbMale"].Tag, CloneThumbsMale_pictureBox, out t))
            {
                CloneThumbs_dataGridView.Rows[thumbManagerIndex].Cells["CASPthumbMale"].Tag = t;
                Image img = t != null ? t.Image : Properties.Resources.NullImage;
                CloneThumbs_dataGridView.Rows[thumbManagerIndex].Cells["CASPthumbMale"].Value = img;
                CloneThumbsMale_pictureBox.Image = img;
                changesThumbs = true;
            }
        }

        private void CloneThumbsFemale_pictureBox_Click(object sender, EventArgs e)
        {
            if (thumbManagerIndex > CloneThumbs_dataGridView.Rows.Count - 1) return;
            ThumbnailResource t = null;
            if (ImportExportThumb((ThumbnailResource)CloneThumbs_dataGridView.Rows[thumbManagerIndex].Cells["CASPthumbFemale"].Tag, CloneThumbsFemale_pictureBox, out t))
            {
                CloneThumbs_dataGridView.Rows[thumbManagerIndex].Cells["CASPthumbFemale"].Tag = t;
                Image img = t != null ? t.Image : Properties.Resources.NullImage;
                CloneThumbs_dataGridView.Rows[thumbManagerIndex].Cells["CASPthumbFemale"].Value = img;
                CloneThumbsFemale_pictureBox.Image = img;
                changesThumbs = true;
            }
        }

        private bool ImportExportThumb(ThumbnailResource thumb, PictureBox picturebox, out ThumbnailResource newThumb)
        {
            ImageDisplayImportExport imgDisplay = new ImageDisplayImportExport(thumb);
            DialogResult res = imgDisplay.ShowDialog();
            if (res == DialogResult.OK)
            {
                cloneWait_label.Visible = true;
                cloneWait_label.Refresh();
                Image retImage = imgDisplay.ReturnIMG;
                picturebox.Image = retImage;
                picturebox.Refresh();
                ThumbnailResource t = null;
                if (retImage != null)
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        Bitmap newImage = new Bitmap(retImage);
                        newImage.Save(ms, ImageFormat.Png);
                        ms.Position = 0;
                        t = new ThumbnailResource(1, ms);
                    }
                }
                cloneWait_label.Visible = false;
                cloneWait_label.Refresh();
                newThumb = t;
                return true;
            }
            else
            {
                newThumb = thumb;
                return false;
            }
        }

        private ThumbnailResource ApplyOverlay(ThumbnailResource baseThumb, Image overlay)
        {
            if (overlay == null) return baseThumb;
            Bitmap final;
            if (baseThumb != null)
            {
                final = new Bitmap(baseThumb.Image);
            }
            else
            {
                final = new Bitmap(104, 148);
            }
            Graphics g = Graphics.FromImage(final);
            g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;
            g.DrawImage(overlay, new Rectangle(0, 0, overlay.Width, overlay.Height));
            ThumbnailResource tmp;
            using (MemoryStream ms = new MemoryStream())
            {
                final.Save(ms, ImageFormat.Png);
                tmp = new ThumbnailResource(0, ms);
            }
            return tmp;
        }

        private ThumbnailResource ReplaceWithOverlay(Image overlay)
        {
            if (overlay == null) return null;
            ThumbnailResource tmp;
            using (MemoryStream ms = new MemoryStream())
            {
                overlay.Save(ms, ImageFormat.Png);
                tmp = new ThumbnailResource(0, ms);
            }
            return tmp;
        }


        private void CloneThumbsMaleApply_button_Click(object sender, EventArgs e)
        {
            ThumbnailResource t = ApplyOverlay((ThumbnailResource)CloneThumbs_dataGridView.Rows[thumbManagerIndex].Cells["CASPthumbMale"].Tag, thumbOverlay);
            CloneThumbs_dataGridView.Rows[thumbManagerIndex].Cells["CASPthumbMale"].Tag = t;
            CloneThumbs_dataGridView.Rows[thumbManagerIndex].Cells["CASPthumbMale"].Value = t.Image;
            CloneThumbsMale_pictureBox.Image = t.Image;
            changesThumbs = true;
        }

        private void CloneThumbsFemaleApply_button_Click(object sender, EventArgs e)
        {
            ThumbnailResource t = ApplyOverlay((ThumbnailResource)CloneThumbs_dataGridView.Rows[thumbManagerIndex].Cells["CASPthumbFemale"].Tag, thumbOverlay);
            CloneThumbs_dataGridView.Rows[thumbManagerIndex].Cells["CASPthumbFemale"].Tag = t;
            CloneThumbs_dataGridView.Rows[thumbManagerIndex].Cells["CASPthumbFemale"].Value = t.Image;
            CloneThumbsFemale_pictureBox.Image = t.Image;
            changesThumbs = true;
        }

        private void CloneThumbsOverlay_pictureBox_Click(object sender, EventArgs e)
        {
            ImageDisplayImportExport imgDisplay = new ImageDisplayImportExport(thumbOverlay);
            DialogResult res = imgDisplay.ShowDialog();
            if (res == DialogResult.OK)
            {
                thumbOverlay = imgDisplay.ReturnIMG;
                CloneThumbsOverlay_pictureBox.Image = thumbOverlay;
            }
        }

        private void CloneThumbsOverlayAll_button_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < CloneThumbs_dataGridView.RowCount; i++)
            {
                ThumbnailResource t = ApplyOverlay((ThumbnailResource)CloneThumbs_dataGridView.Rows[i].Cells["CASPthumbMale"].Tag, thumbOverlay);
                CloneThumbs_dataGridView.Rows[i].Cells["CASPthumbMale"].Tag = t;
                CloneThumbs_dataGridView.Rows[i].Cells["CASPthumbMale"].Value = t.Image;
                t = ApplyOverlay((ThumbnailResource)CloneThumbs_dataGridView.Rows[i].Cells["CASPthumbFemale"].Tag, thumbOverlay);
                CloneThumbs_dataGridView.Rows[i].Cells["CASPthumbFemale"].Tag = t;
                CloneThumbs_dataGridView.Rows[i].Cells["CASPthumbFemale"].Value = t.Image;
            }
            CloneThumbsMale_pictureBox.Image = (Image)CloneThumbs_dataGridView.Rows[thumbManagerIndex].Cells["CASPthumbMale"].Value;
            CloneThumbsFemale_pictureBox.Image = (Image)CloneThumbs_dataGridView.Rows[thumbManagerIndex].Cells["CASPthumbFemale"].Value;
            changesThumbs = true;
        }

        private void CloneThumbsOverlayMale_button_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < CloneThumbs_dataGridView.RowCount; i++)
            {
                ThumbnailResource t = ApplyOverlay((ThumbnailResource)CloneThumbs_dataGridView.Rows[i].Cells["CASPthumbMale"].Tag, thumbOverlay);
                CloneThumbs_dataGridView.Rows[i].Cells["CASPthumbMale"].Tag = t;
                CloneThumbs_dataGridView.Rows[i].Cells["CASPthumbMale"].Value = t.Image;
            }
            CloneThumbsMale_pictureBox.Image = (Image)CloneThumbs_dataGridView.Rows[thumbManagerIndex].Cells["CASPthumbMale"].Value;
            changesThumbs = true;
        }

        private void CloneThumbsOverlayFemale_button_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < CloneThumbs_dataGridView.RowCount; i++)
            {
                ThumbnailResource t = ApplyOverlay((ThumbnailResource)CloneThumbs_dataGridView.Rows[i].Cells["CASPthumbFemale"].Tag, thumbOverlay);
                CloneThumbs_dataGridView.Rows[i].Cells["CASPthumbFemale"].Tag = t;
                CloneThumbs_dataGridView.Rows[i].Cells["CASPthumbFemale"].Value = t.Image;
            }
            CloneThumbsFemale_pictureBox.Image = (Image)CloneThumbs_dataGridView.Rows[thumbManagerIndex].Cells["CASPthumbFemale"].Value;
            changesThumbs = true;
        }

        private void CloneThumbsReplaceAll_button_Click(object sender, EventArgs e)
        {
            if (thumbOverlay == null) return;
            ThumbnailResource t = ReplaceWithOverlay(thumbOverlay);
            for (int i = 0; i < CloneThumbs_dataGridView.RowCount; i++)
            {
                CloneThumbs_dataGridView.Rows[i].Cells["CASPthumbMale"].Tag = t;
                CloneThumbs_dataGridView.Rows[i].Cells["CASPthumbMale"].Value = t.Image;
                CloneThumbs_dataGridView.Rows[i].Cells["CASPthumbFemale"].Tag = t;
                CloneThumbs_dataGridView.Rows[i].Cells["CASPthumbFemale"].Value = t.Image;
            }
            CloneThumbsMale_pictureBox.Image = (Image)CloneThumbs_dataGridView.Rows[thumbManagerIndex].Cells["CASPthumbMale"].Value;
            CloneThumbsFemale_pictureBox.Image = (Image)CloneThumbs_dataGridView.Rows[thumbManagerIndex].Cells["CASPthumbFemale"].Value;
            changesThumbs = true;
        }

        private void CloneThumbsReplaceMale_button_Click(object sender, EventArgs e)
        {
            if (thumbOverlay == null) return;
            ThumbnailResource t = ReplaceWithOverlay(thumbOverlay);
            for (int i = 0; i < CloneThumbs_dataGridView.RowCount; i++)
            {
                CloneThumbs_dataGridView.Rows[i].Cells["CASPthumbMale"].Tag = t;
                CloneThumbs_dataGridView.Rows[i].Cells["CASPthumbMale"].Value = t.Image;
            }
            CloneThumbsMale_pictureBox.Image = (Image)CloneThumbs_dataGridView.Rows[thumbManagerIndex].Cells["CASPthumbMale"].Value;
            changesThumbs = true;
        }

        private void CloneThumbsReplaceFemale_button_Click(object sender, EventArgs e)
        {
            if (thumbOverlay == null) return;
            ThumbnailResource t = ReplaceWithOverlay(thumbOverlay);
            for (int i = 0; i < CloneThumbs_dataGridView.RowCount; i++)
            {
                CloneThumbs_dataGridView.Rows[i].Cells["CASPthumbFemale"].Tag = t;
                CloneThumbs_dataGridView.Rows[i].Cells["CASPthumbFemale"].Value = t.Image;
            }
            CloneThumbsFemale_pictureBox.Image = (Image)CloneThumbs_dataGridView.Rows[thumbManagerIndex].Cells["CASPthumbFemale"].Value;
            changesThumbs = true;
        }

        private void CloneThumbsCommit_button_Click(object sender, EventArgs e)
        {
            CloneThumbsCommit();
        }

        private void CloneThumbsCommit()
        {
            Predicate<IResourceIndexEntry> idel = r => r.ResourceType == (uint)XmodsEnums.ResourceTypes.THUM;
            List<IResourceIndexEntry> id = clonePack.FindAll(idel);
            foreach (IResourceIndexEntry ir in id)
            {
                DeleteResource(clonePack, ir);
            }

            for (int i = 0; i < CloneThumbs_dataGridView.RowCount; i++)
            {
                int index = (int)CloneThumbs_dataGridView.Rows[i].Tag;
                if (index < 0 | index > clonePackCASPs.Count - 1) continue;
                clonePackCASPs[index].maleThumb = (ThumbnailResource)CloneThumbs_dataGridView.Rows[i].Cells["CASPthumbMale"].Tag;
                clonePackCASPs[index].femaleThumb = (ThumbnailResource)CloneThumbs_dataGridView.Rows[i].Cells["CASPthumbFemale"].Tag;
                IResourceKey irk = new TGIBlock(0, null, (uint)XmodsEnums.ResourceTypes.THUM, 0x00000102U, iresCASPs[index].Instance);
                if (clonePackCASPs[index].maleThumb != null)
                {
                    clonePack.AddResource(irk, clonePackCASPs[index].maleThumb.ToJFIF(), false);
                }
                irk = new TGIBlock(0, null, (uint)XmodsEnums.ResourceTypes.THUM, 0x00000002U, iresCASPs[index].Instance);
                if (clonePackCASPs[index].femaleThumb != null)
                {
                    clonePack.AddResource(irk, clonePackCASPs[index].femaleThumb.ToJFIF(), false);
                }
                if (index == myCASPindex)
                {
                    CloneColorThumb_pictureBox.Image = clonePackCASPs[myCASPindex].maleThumb != null ? clonePackCASPs[myCASPindex].maleThumb.Image :
                                                (clonePackCASPs[myCASPindex].femaleThumb != null ? clonePackCASPs[myCASPindex].femaleThumb.Image : Properties.Resources.NullImage);
                }
            }
            changesThumbs = false;
        }

        private void CloneThumbsDiscard_button_Click(object sender, EventArgs e)
        {
            DialogResult res = MessageBox.Show("Are you sure you want to discard changes?", "Discard Changes", MessageBoxButtons.OKCancel);
            if (res == DialogResult.Cancel) return;
            CloneThumbsList();
            CloneThumbsMale_pictureBox.Image = null;
            CloneThumbsFemale_pictureBox.Image = null;
            changesThumbs = false;
        }

        private void DeleteFromThumbsList(int CASPindex)
        {
            int selectIndex = thumbManagerIndex;
            for (int i = 0; i < CloneThumbs_dataGridView.RowCount; i++)
            {
                int index = (int)CloneThumbs_dataGridView.Rows[i].Tag;
                CloneThumbs_dataGridView.Rows[i].Selected = false;
                if (index > CASPindex)
                {
                    CloneThumbs_dataGridView.Rows[i].Tag = index - 1;
                }
                else if (index == CASPindex)
                {
                    CloneThumbs_dataGridView.Rows.RemoveAt(i);
                    selectIndex = i;
                }
            }
            CloneThumbsMale_pictureBox.Image = null;
            CloneThumbsFemale_pictureBox.Image = null;
            thumbManagerIndex = Math.Min(selectIndex, CloneThumbs_dataGridView.RowCount-1);
            CloneThumbs_dataGridView.Rows[thumbManagerIndex].Selected = true;
            changesThumbs = true;
        }

        private void CloneThumbsWipe()
        {
            CloneThumbs_dataGridView.Rows.Clear();
            CloneThumbsMale_pictureBox.Image = null;
            CloneThumbsFemale_pictureBox.Image = null;
            CloneThumbsOverlay_pictureBox.Image = null;
        }
    }
}
