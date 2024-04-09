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
using System.Windows.Media.Media3D;

namespace XMODS
{
    public partial class Form1 : Form
    {
        string RigFilter = "RIG files (*.grannyrig, *.rig)|*.grannyrig; *.rig|All files (*.*)|*.*";

        private void rigFilename_button_Click(object sender, EventArgs e)
        {
            rigFilename.Text = GetFilename("Select RIG file", RigFilter);
            BinaryReader br = null;
            RIG rig = null;
            if ((br = new BinaryReader(File.OpenRead(rigFilename.Text))) != null)
            {
                using (br)
                {
                    rig = new RIG(br);
                }
                br.Dispose();
            }
            else
            {
                MessageBox.Show("Can't open RIG file!");
                return;
            }
            rigBones_dataGridView.SortCompare += new DataGridViewSortCompareEventHandler(this.rigBones_dataGridView_SortCompare);
            string[] s = new string[9];
            for (int i = 0; i < rig.NumberBones; i++)
            {
                RIG.Bone b = rig.Bones[i];
                s[0] = b.BoneName;
                s[1] = "0x" + b.BoneHash.ToString("X8");
                s[2] = b.ParentName;
                s[3] = b.OpposingBoneName;
                s[4] = b.PositionVector.ToString();
                s[5] = b.LocalRotation.ToString();
                s[6] = b.ScalingVector.ToString();
                s[7] = b.LocalTransform.ToString();
                s[8] = b.GlobalTransform.ToString();
                int ind = rigBones_dataGridView.Rows.Add(s);
                rigBones_dataGridView.Rows[ind].HeaderCell.Value = ind.ToString();
                rigBones_dataGridView.Rows[ind].Tag = b.BoneHash;
                rigBones_dataGridView.Rows[ind].Cells[7].Tag = b.LocalTransform;
                rigBones_dataGridView.Rows[ind].Cells[8].Tag = b.GlobalTransform;
            }
        }

        private void rigBones_dataGridView_SortCompare(object sender, DataGridViewSortCompareEventArgs e)
        {
            if (e.Column.Name == "BoneName") e.SortResult = String.Compare(e.CellValue1.ToString(), e.CellValue2.ToString());
            else if (e.Column.Name == "BoneHash") e.SortResult = ((uint)rigBones_dataGridView.Rows[e.RowIndex1].Tag < (uint)rigBones_dataGridView.Rows[e.RowIndex2].Tag) ? -1 : 1;
            e.Handled = true;
        }

        private void RigBonesToXML_button_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "XML file (*.xml)|*.xml|All files (*.*)|*.*";
            saveFileDialog1.FilterIndex = 1;
            saveFileDialog1.Title = "Save bonelist XML";
            saveFileDialog1.AddExtension = true;
            saveFileDialog1.CheckPathExists = true;
            saveFileDialog1.DefaultExt = "xml";
            saveFileDialog1.OverwritePrompt = true;
            if (saveFileDialog1.ShowDialog() != DialogResult.OK) return;

            using (System.IO.StreamWriter file = new System.IO.StreamWriter(saveFileDialog1.FileName, false))
            {
                file.Write("<?xml version=\"1.0\" encoding=\"utf-8\"?>" + Environment.NewLine +
                "\t<bones>" + Environment.NewLine +
                "\t\t<bone name=\"(None)\" hash=\"00000000\" />" + Environment.NewLine);
                foreach (DataGridViewRow r in rigBones_dataGridView.Rows)
                {
                    file.WriteLine("\t\t<bone name=\"" + r.Cells["BoneName"].Value.ToString() + 
                        "\" hash=\"" + r.Cells["BoneHash"].Value.ToString().Remove(0, 2) + "\" />");
                }
                file.Write("\t</bones>");
            }
        }

        private void rigBones_dataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 7) return;
            Matrix4D transform = (Matrix4D)rigBones_dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Tag;
            MessageBox.Show("Inverse" + Environment.NewLine + transform.Inverse().ToString() + Environment.NewLine + 
                "Transpose" + Environment.NewLine + transform.Transpose().ToString());
        }
    }
}
