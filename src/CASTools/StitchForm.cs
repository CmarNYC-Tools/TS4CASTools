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
    public partial class StitchForm : Form
    {
        public StitchForm(GEOM geom)
        {
            InitializeComponent();
            int wr = TextRenderer.MeasureText(geom.numberVertices.ToString(), Stitch_dataGridView.Font).Width;
            Stitch_dataGridView.Columns.Add("VertexSeq", "Vertex Sequence");
            Stitch_dataGridView.Columns[0].Width = Math.Max(75, wr);
            Stitch_dataGridView.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            GEOM.UVStitch[] stitches = geom.UVStitches;
            int numstitch = 0;
            if (stitches != null)
            {
                foreach (GEOM.UVStitch adj in stitches)
                {
                    if (adj.Count > numstitch) numstitch = adj.Count;
                }
            }
            int numcols = geom.numberUVsets + numstitch + 1;

            for (int i = 1; i <= geom.numberUVsets; i++)
            {
                Stitch_dataGridView.Columns.Add("UV" + (i-1).ToString(), "UV" + (i-1).ToString());
                Stitch_dataGridView.Columns[i].Width = 200;
            }

            int st = 0;
            for (int i = geom.numberUVsets + 1; i <= geom.numberUVsets + numstitch; i++)
            {
                Stitch_dataGridView.Columns.Add("Stitch" + st.ToString(), "Stitch" + st.ToString());
                Stitch_dataGridView.Columns[i].Width = 200;
                st++;
            }

            for (int v = 0; v < geom.numberVertices; v++)
            {
                string[] tmp = new string[numcols];
                tmp[0] = v.ToString();
                for (int i = 0; i < geom.numberUVsets; i++)
                {
                    float[] uv = geom.getUV(v, i);
                    tmp[i + 1] = uv[0].ToString() + ", " + uv[1].ToString();
                }

                if (stitches != null)
                {
                    for (int i = 0; i < stitches.Length; i++)
                    {
                        if (stitches[i].Index == v)
                        for (int j = 0; j < stitches[i].Count; j++)
                        {
                            float[] s = stitches[i].UV1Coordinates[j];
                            tmp[geom.numberUVsets + j + 1] = s[0].ToString() + ", " + s[1].ToString();
                        }
                    }
                }
                Stitch_dataGridView.Rows.Add(tmp);
            }

        }
    }
}
