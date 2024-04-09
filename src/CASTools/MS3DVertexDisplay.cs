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
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Xmods.DataLib;

namespace XMODS
{
    public partial class MS3DVertexDisplay : Form
    {
        MS3D myMS3D;
        string displayFile = "";
        public MS3DVertexDisplay(MS3D displayMS3D, string filename)
        {
            InitializeComponent();
            myMS3D = displayMS3D;
            displayFile = filename;
        }

        private void MS3DVertexDisplay_onLoad(object sender, EventArgs e)
        {
            int numVerts = myMS3D.NumberVertices;
            this.Text = "MS3D Vertex Data: " + displayFile;

            int wr = TextRenderer.MeasureText(myMS3D.NumberVertices.ToString(), MS3DVertexDisplay_dataGridView.Font).Width;
            MS3DVertexDisplay_dataGridView.Columns.Add("VertexSeq", "Vertex Sequence");
            MS3DVertexDisplay_dataGridView.Columns[0].Width = Math.Max(75, wr);
            MS3DVertexDisplay_dataGridView.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            int col = 1;
            MS3DVertexDisplay_dataGridView.Columns.Add("VertexID", "Vertex ID");
            int w = TextRenderer.MeasureText(myMS3D.MaxVertexID.ToString(), MS3DVertexDisplay_dataGridView.Font).Width;
            MS3DVertexDisplay_dataGridView.Columns[col].Width = Math.Max(90, w);
            MS3DVertexDisplay_dataGridView.Columns[col].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            col = col + 1;
            MS3DVertexDisplay_dataGridView.Columns.Add("Px", "X");
            MS3DVertexDisplay_dataGridView.Columns.Add("Py", "Y");
            MS3DVertexDisplay_dataGridView.Columns.Add("Pz", "Z");
            for (int i = 0; i < 3; i++)
            {
                MS3DVertexDisplay_dataGridView.Columns[col + i].Width = 90;
            }
            col = col + 3;
            MS3DVertexDisplay_dataGridView.Columns.Add("Bone1", "One");
            MS3DVertexDisplay_dataGridView.Columns.Add("Bone2", "Two");
            MS3DVertexDisplay_dataGridView.Columns.Add("Bone3", "Three");
            MS3DVertexDisplay_dataGridView.Columns.Add("Bone4", "Four");
            for (int i = 0; i < 4; i++)
            {
                MS3DVertexDisplay_dataGridView.Columns[col + i].Width = 70;
            }
            col = col + 4;
            MS3DVertexDisplay_dataGridView.Columns.Add("Tags", "Color/TagVal");
            MS3DVertexDisplay_dataGridView.Columns[col].Width = 90;
            col = col + 1; 
            MS3DVertexDisplay_dataGridView.Columns.Add("References", "References");
            MS3DVertexDisplay_dataGridView.Columns[col].Width = 90;
            col = col + 1;

            MS3DVertexDisplay_dataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            MS3DVertexDisplay_dataGridView.ColumnHeadersHeight = MS3DVertexDisplay_dataGridView.ColumnHeadersHeight * 2;
            MS3DVertexDisplay_dataGridView.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.BottomCenter;
            MS3DVertexDisplay_dataGridView.RowHeadersVisible = false;
            if (myMS3D.NumberVertices > 0)
                MS3DVertexDisplay_dataGridView.Rows.Add(myMS3D.NumberVertices);
           // col = col - 1;
            string[] dataList = new string[col];
            int c;
            int len = numVerts.ToString().Length;
            int len2 = myMS3D.MaxVertexID.ToString().Length;
            bool hasExtra = myMS3D.VertexExtraArray.Length > 0;
            for (int i = 0; i < numVerts; i++)
            {
                dataList[0] = i.ToString().PadLeft(len, ' ');
                c = 1;
                if (hasExtra) dataList[1] = myMS3D.getVertexExtra(i).VertexID.ToString().PadLeft(len2, ' ');
                else dataList[1] = "";
                c = c + 1;
                float[] tmp = myMS3D.getVertex(i).Position;
                for (int j = 0; j < 3; j++)
                    dataList[c + j] = tmp[j].ToString("G6");
                c = c + 3;
                sbyte[] btmp = myMS3D.getBones(i);
                byte[] wtmp = myMS3D.getBoneWeights(i);
                for (int j = 0; j < 4; j++)
                    dataList[c + j] = btmp[j].ToString() + " : " + wtmp[j].ToString();
                c = c + 4;
                if (hasExtra)
                {
                    uint ttmp = myMS3D.getVertexExtra(i).VertexColor;
                    dataList[c] = Convert.ToString(ttmp, 16).ToUpper().PadLeft(8, '0');
                }
                else
                {
                    dataList[c] = "";
                }
                c = c + 1;
                dataList[c] = myMS3D.getVertex(i).ReferenceCount.ToString();
                for (int j = 0; j < dataList.Length; j++)
                {
                    if (dataList[j] == null) dataList[j] = "";
                }
                MS3DVertexDisplay_dataGridView.Rows[i].SetValues(dataList);
            }
        }

        private void MS3DVertexDisplaygrid_Paint(object sender, PaintEventArgs e)
        {
            MS3DVertexDisplaygrid_init(e.Graphics);
        }
        private void MS3DVertexDisplaygrid_Scroll(object sender, ScrollEventArgs e)
        {
            Graphics g = MS3DVertexDisplay_dataGridView.CreateGraphics();
            MS3DVertexDisplaygrid_init(g);
        }
        private void MS3DVertexDisplaygrid_Resize(object sender, EventArgs e)
        {
            Graphics g = MS3DVertexDisplay_dataGridView.CreateGraphics();
            MS3DVertexDisplaygrid_init(g);
        }

        private void MS3DVertexDisplaygrid_init(Graphics g)
        {
            header_Paint(2, 3, "Position", g);
            header_Paint(5, 4, "Bones", g); 
        }
        private void header_Paint(int colno, int colspan, string headertext, Graphics g)
        {
            if (colno < 0 || colno > MS3DVertexDisplay_dataGridView.Columns.Count) return;
            Rectangle r1 = MS3DVertexDisplay_dataGridView.GetCellDisplayRectangle(colno, -1, true);
            for (int i = 1; i < colspan; i++)
            {
                Rectangle r2 = MS3DVertexDisplay_dataGridView.GetCellDisplayRectangle(colno + i, -1, true);
                r1.Width += r2.Width;
            }
            
            r1.X += 1;
            r1.Y += 2;
            r1.Width -= 2;
            r1.Height = (r1.Height / 2) - 6;

            using (SolidBrush br = new SolidBrush(MS3DVertexDisplay_dataGridView.ColumnHeadersDefaultCellStyle.BackColor))
            {
                g.FillRectangle(br, r1);
            }

            //draw text
            using (SolidBrush br = new SolidBrush(this.MS3DVertexDisplay_dataGridView.ColumnHeadersDefaultCellStyle.ForeColor))
            {
                StringFormat sf = new StringFormat();
                sf.Alignment = StringAlignment.Center;
                g.DrawString(headertext, MS3DVertexDisplay_dataGridView.ColumnHeadersDefaultCellStyle.Font, br, r1, sf);
            }
        }
    }
}
