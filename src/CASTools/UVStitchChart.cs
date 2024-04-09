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
    public partial class UVStitchChart : Form
    {
        public UVStitchChart(GEOM geom)
        {
            InitializeComponent();

            GEOM.UVStitch[] stitches = geom.UVStitches;
            int numstitch = 0;
            if (stitches != null)
            {
                foreach (GEOM.UVStitch adj in stitches)
                {
                    if (adj.Count > numstitch) numstitch = adj.Count;
                }
            }

            for (int i = 0; i < geom.numberUVsets; i++)
            {
                chart1.Series.Add("UV" + i.ToString());
                chart1.Series[i].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Point;
            }

            int st = 0;
            for (int i = geom.numberUVsets; i < geom.numberUVsets + numstitch; i++)
            {
                chart1.Series.Add("Stitch" + st.ToString());
                chart1.Series[i].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Point;
                st++;
            }

            int numcols = geom.numberUVsets + numstitch;
            for (int v = 0; v < geom.numberVertices; v++)
            {
                for (int i = 0; i < geom.numberUVsets; i++)
                {
                    float[] tmp = geom.getUV(v, i);
                    chart1.Series[i].Points.AddXY(tmp[0], 1f - tmp[1]);
                }

                if (stitches != null)
                {
                    for (int i = 0; i < stitches.Length; i++)
                    {
                        if (stitches[i].Index == v)
                            for (int j = 0; j < stitches[i].Count; j++)
                            {
                                float[] tmp = stitches[i].UV1Coordinates[j];
                                chart1.Series[geom.numberUVsets + j].Points.AddXY(tmp[0], 1f - tmp[1]);
                            }
                    }
                }
            }

        }
    }
}
