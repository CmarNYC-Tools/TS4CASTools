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
using Xmods.DataLib;

namespace XMODS
{
    public partial class MeshLodRegionAssignmentForm : Form
    {
        bool isNewImport;
        public string importFile { get { return MeshAssignFile.Text; } }
        public int LodSet { get { return (int)MeshAssignLOD_numericUpDown.Value; } }
        public float LayerSet { get { return Int32.Parse(MeshAssignLayer_textBox.Text); } }
        public int[] RegionsSet { get { return MeshAssignRegion_checkedListBox.CheckedIndices.Cast<int>().ToArray(); } }

        public MeshLodRegionAssignmentForm()        //new mesh
        {
            InitializeComponent();
            MeshAssignRegion_checkedListBox.Items.AddRange(Enum.GetNames(typeof(XmodsEnums.CASPartRegionTS4)));
            isNewImport = true;
        }

        public MeshLodRegionAssignmentForm(string meshName, int lod, float layer, uint[] regions)  //change mesh region(s)
        {
            InitializeComponent();
            MeshAssignFile_button.Visible = false;
            MeshAssignFile_button.Enabled = false;
            MeshAssignFile.Text = meshName;
            MeshAssignRegion_checkedListBox.Items.AddRange(Enum.GetNames(typeof(XmodsEnums.CASPartRegionTS4)));
            MeshAssignLOD_numericUpDown.Value = lod;
            MeshAssignLOD_numericUpDown.ReadOnly = true;
            MeshAssignLayer_textBox.Text = layer.ToString();
            foreach (int i in regions)
            {
                MeshAssignRegion_checkedListBox.SetItemChecked(i, true);
            }
            isNewImport = false;
        }

        private void MeshAssignFullHair_button_Click(object sender, EventArgs e)
        {
            MeshAssignLayer_textBox.Text = "10003";
            for (int i = 0; i < MeshAssignRegion_checkedListBox.Items.Count; i++)
            {
                MeshAssignRegion_checkedListBox.SetItemChecked(i, Enum.GetName(typeof(XmodsEnums.CASPartRegionTS4), i).StartsWith("HairHat"));
            }
        }

        private void MeshAssignHatStraight_button_Click(object sender, EventArgs e)
        {
            MeshAssignLayer_textBox.Text = "10001";
            for (int i = 0; i < MeshAssignRegion_checkedListBox.Items.Count; i++)
            {
                MeshAssignRegion_checkedListBox.SetItemChecked(i, 
                    Enum.GetName(typeof(XmodsEnums.CASPartRegionTS4), i).Contains("HairHatB") ||
                    Enum.GetName(typeof(XmodsEnums.CASPartRegionTS4), i).Contains("HairHatD"));
            }
        }

        private void MeshAssignHatTilted_button_Click(object sender, EventArgs e)
        {
            MeshAssignLayer_textBox.Text = "10001";
            for (int i = 0; i < MeshAssignRegion_checkedListBox.Items.Count; i++)
            {
                MeshAssignRegion_checkedListBox.SetItemChecked(i,
                    Enum.GetName(typeof(XmodsEnums.CASPartRegionTS4), i).Contains("HairHatC") ||
                    Enum.GetName(typeof(XmodsEnums.CASPartRegionTS4), i).Contains("HairHatD"));
            }
        }

        private void MeshAssignGo_button_Click(object sender, EventArgs e)
        {
            if (isNewImport && string.Compare(MeshAssignFile.Text, " ") <= 0)
            {
                MessageBox.Show("You must select a mesh file to import!");
                return;
            }
            if (MeshAssignRegion_checkedListBox.CheckedIndices.Count == 0)
            {
                MessageBox.Show("You can't have a mesh with no Region!");
                return;
            }
            int tmp;
            if (!Int32.TryParse(MeshAssignLayer_textBox.Text, out tmp))
            {
                MessageBox.Show("You haven't entered a valid number for the Layer!");
                return;
            }
            this.DialogResult = DialogResult.OK;
        }

        private void MeshAssignCancel_button_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void MeshAssignFile_button_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "GEOM mesh files (*.geom; *.simgeom)|*.geom; *.simgeom|All files (*.*)|*.*";
            openFileDialog1.Title = "Select TS4 GEOM mesh file";
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.CheckFileExists = true;
            if (openFileDialog1.ShowDialog() == DialogResult.OK) MeshAssignFile.Text = openFileDialog1.FileName;
        }

        private void LayerHelp_button_Click(object sender, EventArgs e)
        {
            Form1.LayerHelp();
        }
    }
}
