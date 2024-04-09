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
    public partial class GEOMDataDisplay : Form
    {
        GEOM myGEOM;
        string displayFile = "";
        bool doID, doPos, doNorm, doUV, doBones, doTangents, doTags, doStitch, doSeamStitch;
        int numstitch;
        public GEOMDataDisplay(GEOM displayGEOM, string filename, bool IDs, bool Position, bool Normals, bool UVs, bool Stitch, bool SeamStitch, bool Bones, bool Tangents, bool Tags)
        {
            InitializeComponent();
            myGEOM = displayGEOM;
            displayFile = filename;
            doID = IDs;
            doPos = Position;
            doNorm = Normals;
            doUV = UVs;
            doStitch = Stitch;
            doSeamStitch = SeamStitch;
            doBones = Bones;
            doTangents = Tangents;
            doTags = Tags;
        }

        private void GEOMDataDisplay_onLoad(object sender, EventArgs e)
        {
            int[] formats = myGEOM.vertexFormatList;
            int numVerts = myGEOM.numberVertices;
            this.Text = "Mesh Vertex Data: " + displayFile;
            GEOM.UVStitch[] stitches = myGEOM.UVStitches;
            GEOM.SeamStitch[] seamStitches = myGEOM.SeamStitches;
            numstitch = 0;
            if (stitches != null)
            {
                foreach (GEOM.UVStitch adj in stitches)
                {
                    if (adj.Count > numstitch) numstitch = adj.Count;
                }
            }
            int wr = TextRenderer.MeasureText(myGEOM.numberVertices.ToString(), GEOMDataDisplay_dataGridView.Font).Width;
            GEOMDataDisplay_dataGridView.Columns.Add("VertexSeq", "Vertex Sequence");
            GEOMDataDisplay_dataGridView.Columns[0].Width = Math.Max(75, wr);
            GEOMDataDisplay_dataGridView.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            int col = 1;
            if (doID)
            {
                GEOMDataDisplay_dataGridView.Columns.Add("VertexID", "Vertex ID");
                int w = TextRenderer.MeasureText(myGEOM.maxVertexID.ToString(), GEOMDataDisplay_dataGridView.Font).Width;
                GEOMDataDisplay_dataGridView.Columns[col].Width = Math.Max(90, w);
                GEOMDataDisplay_dataGridView.Columns[col].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                col = col + 1;
            }
            if (doPos)
            {
                GEOMDataDisplay_dataGridView.Columns.Add("Cx", "X");
                GEOMDataDisplay_dataGridView.Columns.Add("Cy", "Y");
                GEOMDataDisplay_dataGridView.Columns.Add("Cz", "Z");
                for (int i = 0; i < 3; i++)
                {
                    GEOMDataDisplay_dataGridView.Columns[col + i].Width = 90;
                }
                col = col + 3;
            }
            if (doNorm)
            {
                GEOMDataDisplay_dataGridView.Columns.Add("Nx", "X");
                GEOMDataDisplay_dataGridView.Columns.Add("Ny", "Y");
                GEOMDataDisplay_dataGridView.Columns.Add("Nz", "Z");
                for (int i = 0; i < 3; i++)
                {
                    GEOMDataDisplay_dataGridView.Columns[col + i].Width = 90;
                }
                col = col + 3;
            }
            if (doUV)
            {
                for (int i = 0; i < myGEOM.numberUVsets; i++)
                {
                    GEOMDataDisplay_dataGridView.Columns.Add("UV" + i.ToString() + "x", "U");
                    GEOMDataDisplay_dataGridView.Columns.Add("UV" + i.ToString() + "y", "V");
                    for (int j = 0; j < 2; j++)
                    {
                        GEOMDataDisplay_dataGridView.Columns[col + j].Width = 90;
                    }
                    col = col + 2;
                }
            }
            if (doStitch)
            {
                for (int i = 0; i < numstitch; i++)
                {
                    GEOMDataDisplay_dataGridView.Columns.Add("Stitch" + i.ToString() + "x", "U");
                    GEOMDataDisplay_dataGridView.Columns.Add("Stitch" + i.ToString() + "y", "V");
                    for (int j = 0; j < 2; j++)
                    {
                        GEOMDataDisplay_dataGridView.Columns[col + j].Width = 90;
                    }
                    col = col + 2;
                }
            }
            if (doSeamStitch)
            {
                GEOMDataDisplay_dataGridView.Columns.Add("SeamStitch", "Seam Stitch");
                GEOMDataDisplay_dataGridView.Columns[col].Width = 100;
                col = col + 1;
            }
            if (doBones)
            {
                GEOMDataDisplay_dataGridView.Columns.Add("Bone1", "One");
                GEOMDataDisplay_dataGridView.Columns.Add("Bone2", "Two");
                GEOMDataDisplay_dataGridView.Columns.Add("Bone3", "Three");
                GEOMDataDisplay_dataGridView.Columns.Add("Bone4", "Four");
                for (int i = 0; i < 4; i++)
                {
                    GEOMDataDisplay_dataGridView.Columns[col + i].Width = 70;
                }
                col = col + 4;
            }
            if (doTangents)
            {
                GEOMDataDisplay_dataGridView.Columns.Add("Tx", "X");
                GEOMDataDisplay_dataGridView.Columns.Add("Ty", "Y");
                GEOMDataDisplay_dataGridView.Columns.Add("Tz", "Z");
                for (int i = 0; i < 3; i++)
                {
                    GEOMDataDisplay_dataGridView.Columns[col + i].Width = 90;
                }
                col = col + 3;
            }
            if (doTags)
            {
                GEOMDataDisplay_dataGridView.Columns.Add("Tags", "Color");
                GEOMDataDisplay_dataGridView.Columns[col].Width = 90;
                col = col + 1;
            }

            if (col == 1)
            {
                GEOMDataDisplay_dataGridView.ColumnCount = 1;
                GEOMDataDisplay_dataGridView.Rows.Add(1);
                GEOMDataDisplay_dataGridView.Columns[0].Width = 870;
                GEOMDataDisplay_dataGridView.Rows[0].SetValues("No vertex data information selected for display!");
                return;
            }

            GEOMDataDisplay_dataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            GEOMDataDisplay_dataGridView.ColumnHeadersHeight = GEOMDataDisplay_dataGridView.ColumnHeadersHeight * 2;
            GEOMDataDisplay_dataGridView.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.BottomCenter;
            GEOMDataDisplay_dataGridView.RowHeadersVisible = false;
            if (myGEOM.numberVertices > 0)
                GEOMDataDisplay_dataGridView.Rows.Add(myGEOM.numberVertices);
           // col = col - 1;
            int c;
            int len = numVerts.ToString().Length;
            int len2 = myGEOM.maxVertexID.ToString().Length;
            for (int i = 0; i < numVerts; i++)
            {
                string[] dataList = new string[col];
                dataList[0] = i.ToString().PadLeft(len, ' ');
                c = 1;
                if (doID)
                {
                    dataList[c] = myGEOM.getVertexID(i).ToString().PadLeft(len2, ' ');
                    c++;
                }
                if (doPos)
                {  
                    float[] tmp = myGEOM.getPosition(i);
                    for (int j = 0; j < 3; j++)
                        dataList[c + j] = tmp[j].ToString("G6");
                    c = c + 3;
                }
                if (doNorm)
                {
                    float[] tmp = myGEOM.getNormal(i);
                    for (int j = 0; j < 3; j++)
                        dataList[c + j] = tmp[j].ToString("G6");
                    c = c + 3;
                }
                if (doUV)
                {
                    for (int uv = 0; uv < myGEOM.numberUVsets; uv++)
                    {
                        float[] tmp = myGEOM.getUV(i, uv);
                        for (int j = 0; j < 2; j++)
                            dataList[c + j] = tmp[j].ToString("G6");
                        c = c + 2;
                    }
                }
                if (doStitch & stitches != null)
                {
                    int ctmp = c;
                    for (int st = 0; st < stitches.Length; st++)
                    {
                        if (stitches[st].Index == i)
                        {
                            for (int j = 0; j < stitches[st].Count; j++)
                            {
                                float[] tmp = stitches[st].UV1Coordinates[j];
                                for (int k = 0; k < 2; k++)
                                    dataList[c + k] = tmp[k].ToString("G6");
                                c = c + 2;
                            }
                        }
                    }
                    c = ctmp + (numstitch * 2);
                }
                if (doSeamStitch & seamStitches != null)
                {
                    for (int st = 0; st < seamStitches.Length; st++)
                    {
                        if (seamStitches[st].Index == i)
                        {
                            dataList[c] = "0x" + seamStitches[st].VertID.ToString("X4") + " (" + Enum.GetName(typeof(GEOM.SeamType), seamStitches[st].SeamType) + ")";
                        }
                    }
                    c++;
                }
                if (doBones)
                {
                    byte[] btmp = myGEOM.getBones(i);
                    byte[] tmp = myGEOM.getBoneWeights(i);
                    for (int j = 0; j < 4; j++)
                        dataList[c + j] = btmp[j].ToString() + " : " + tmp[j].ToString();
                    c = c + 4;
                }
                if (doTangents)
                {
                    float[] tmp = myGEOM.getTangent(i);
                    for (int j = 0; j < 3; j++)
                        dataList[c + j] = tmp[j].ToString("G6");
                    c = c + 3;
                }
                if (doTags)
                {
                    uint tmp = myGEOM.getTagval(i);
                    dataList[c] = Convert.ToString(tmp, 16).ToUpper().PadLeft(8, '0');
                    c++;
                }
                GEOMDataDisplay_dataGridView.Rows[i].SetValues(dataList);
            }

           // for (int i = 0; i < myGEOM.numberVerts; i++)
           // {
           //     GEOMDataDisplay_dataGridView.Rows[i].SetValues(idData[i], posData[i], normData[i]);
           // }
        }

        private void GEOMDataDisplaygrid_Paint(object sender, PaintEventArgs e)
        {
            GEOMDataDisplaygrid_init(e.Graphics);
        }
        private void GEOMDataDisplaygrid_Scroll(object sender, ScrollEventArgs e)
        {
            Graphics g = GEOMDataDisplay_dataGridView.CreateGraphics();
            GEOMDataDisplaygrid_init(g);
        }
        private void GEOMDataDisplaygrid_Resize(object sender, EventArgs e)
        {
            Graphics g = GEOMDataDisplay_dataGridView.CreateGraphics();
            GEOMDataDisplaygrid_init(g);

        }

        private void GEOMDataDisplaygrid_init(Graphics g)
        {
            int c = 1;
            if (doID)
            {
                c = c + 1;
            }
            if (doPos)
            {
                header_Paint(c, 3, "Position", g);
                c = c + 3;
            }
            if (doNorm)
            {
                header_Paint(c, 3, "Normals", g);
                c = c + 3;
            }
            if (doUV)
            {
                for (int i = 0; i < myGEOM.numberUVsets; i++)
                {
                    header_Paint(c, 2, "UV" + i.ToString(), g);
                    c = c + 2;
                }
            }
            if (doStitch)
            {
                for (int i = 0; i < numstitch; i++)
                {
                    header_Paint(c, 2, "Stitch" + i.ToString(), g);
                    c = c + 2;
                }
            }
            if (doSeamStitch)
            {
                c++;
            }
            if (doBones)
            {
                header_Paint(c, 4, "Bones", g); 
                c = c + 4;
            }
            if (doTangents)
            {
                header_Paint(c, 3, "Tangents", g); 
                c = c + 3;
            }
        }
        private void header_Paint(int colno, int colspan, string headertext, Graphics g)
        {
            Rectangle r1 = GEOMDataDisplay_dataGridView.GetCellDisplayRectangle(colno, -1, true);
            for (int i = 1; i < colspan; i++)
            {
                Rectangle r2 = GEOMDataDisplay_dataGridView.GetCellDisplayRectangle(colno + i, -1, true);
                r1.Width += r2.Width;
            }
            
            r1.X += 1;
            r1.Y += 2;
            r1.Width -= 2;
            r1.Height = (r1.Height / 2) - 6;

            using (SolidBrush br = new SolidBrush(GEOMDataDisplay_dataGridView.ColumnHeadersDefaultCellStyle.BackColor))
            {
                g.FillRectangle(br, r1);
            }

            //draw text
            using (SolidBrush br = new SolidBrush(this.GEOMDataDisplay_dataGridView.ColumnHeadersDefaultCellStyle.ForeColor))
            {
                StringFormat sf = new StringFormat();
                sf.Alignment = StringAlignment.Center;
                g.DrawString(headertext, GEOMDataDisplay_dataGridView.ColumnHeadersDefaultCellStyle.Font, br, r1, sf);
            }
        }
    }
}
