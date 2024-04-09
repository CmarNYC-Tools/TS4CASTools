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
    public partial class MS3DFacesDisplay : Form
    {
        MS3D myMS3D;
        string displayFile;

        public MS3DFacesDisplay(MS3D displayMS3D, string filename)
        {
            InitializeComponent();
            myMS3D = displayMS3D;
            displayFile = filename;
        }

        private void MS3DFacesDisplay_Load(object sender, EventArgs e)
        {
            this.Text = "MS3D Faces List: " + displayFile;
            MS3DFacesDisplay_dataGridView.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            MS3DFacesDisplay_dataGridView.Columns.Add("FP", "Face Points");
            MS3DFacesDisplay_dataGridView.Columns.Add("FN", "Normals");
            MS3DFacesDisplay_dataGridView.Columns.Add("FUV", "UV");
            MS3DFacesDisplay_dataGridView.Columns.Add("FG", "Group");
            MS3DFacesDisplay_dataGridView.Columns[0].Width = 75;
            MS3DFacesDisplay_dataGridView.Columns[1].Width = 250;
            MS3DFacesDisplay_dataGridView.Columns[2].Width = 150;
            MS3DFacesDisplay_dataGridView.Columns[3].Width = 75;
            MS3DFacesDisplay_dataGridView.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.TopCenter;
            MS3DFacesDisplay_dataGridView.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.TopCenter;
            MS3DFacesDisplay_dataGridView.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.TopCenter;
            MS3DFacesDisplay_dataGridView.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.TopCenter;
            MS3DFacesDisplay_dataGridView.RowTemplate.Height = (int)(MS3DFacesDisplay_dataGridView.DefaultCellStyle.Font.Height * 3.75);
               // TextRenderer.MeasureText("Text", MS3DFacesDisplay_dataGridView.DefaultCellStyle.Font).Height * 3 + 5;
            MS3DFacesDisplay_dataGridView.RowHeadersWidth = 75;
            MS3DFacesDisplay_dataGridView.RowHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.TopCenter;
            if (myMS3D.NumberFaces > 0)
                MS3DFacesDisplay_dataGridView.Rows.Add(myMS3D.NumberFaces);
            string[] datalist = new string[4];
            for (int i = 0; i < myMS3D.NumberFaces; i++)
            {
                MS3DFacesDisplay_dataGridView.Rows[i].HeaderCell.Value = i.ToString();
                ushort[] faceset = myMS3D.getFace(i).VertexIndices;
                datalist[0] = faceset[0].ToString() + Environment.NewLine +
                              faceset[1].ToString() + Environment.NewLine +
                              faceset[2].ToString();
                float[][] normalset = myMS3D.getFace(i).VertexNormals;
                datalist[1] = "";
                for (int j = 0; j < 3; j++)
                {
                    datalist[1] += normalset[j][0].ToString() + ", " + normalset[j][1].ToString() + ", " +
                        normalset[j][2].ToString() + Environment.NewLine;
                }
                float[] uset = myMS3D.getFace(i).U; 
                float[] vset = myMS3D.getFace(i).V;
                datalist[2] = "";
                for (int j = 0; j < 3; j++)
                {
                    datalist[2] += uset[j].ToString() + ", " + vset[j].ToString() + Environment.NewLine;
                }
                datalist[3] = myMS3D.getFace(i).GroupIndex.ToString();
                MS3DFacesDisplay_dataGridView.Rows[i].SetValues(datalist);
            }

        }
    }
}
