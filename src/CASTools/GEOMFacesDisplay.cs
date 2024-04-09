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
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Xmods.DataLib;

namespace XMODS
{
    public partial class GEOMFacesDisplay : Form
    {
        GEOM myGEOM;
        string displayFile;

        public GEOMFacesDisplay(GEOM displayGEOM, string filename)
        {
            myGEOM = displayGEOM;
            displayFile = filename;
            InitializeComponent();
        }

        private void GEOMFacesDisplay_Load(object sender, EventArgs e)
        {
            this.Text = "GEOM Faces List: " + displayFile;
            GEOMFacesDisplay_dataGridView.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            GEOMFacesDisplay_dataGridView.Columns.Add("FP1", "Face Point 1");
            GEOMFacesDisplay_dataGridView.Columns.Add("FP2", "Face Point 2");
            GEOMFacesDisplay_dataGridView.Columns.Add("FP3", "Face Point 3");
            GEOMFacesDisplay_dataGridView.Columns[0].Width = 75;
            GEOMFacesDisplay_dataGridView.Columns[1].Width = 75;
            GEOMFacesDisplay_dataGridView.Columns[2].Width = 75;
            GEOMFacesDisplay_dataGridView.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            GEOMFacesDisplay_dataGridView.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            GEOMFacesDisplay_dataGridView.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            GEOMFacesDisplay_dataGridView.RowHeadersWidth = 75;
            GEOMFacesDisplay_dataGridView.RowHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            if (myGEOM.numberFaces > 0)
                GEOMFacesDisplay_dataGridView.Rows.Add(myGEOM.numberFaces);
            string[] datalist = new string[3];
            for (int i = 0; i < myGEOM.numberFaces; i++)
            {
                GEOMFacesDisplay_dataGridView.Rows[i].HeaderCell.Value = i.ToString();
                int[] faceset = myGEOM.getFaceIndices(i);
                for (int j = 0; j < 3; j++)
                {
                    datalist[j] = faceset[j].ToString();
                }
                GEOMFacesDisplay_dataGridView.Rows[i].SetValues(datalist);
            }

        }
    }
}
