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
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Xmods.DataLib;

namespace XMODS
{
    public partial class SlotRayDataDisplay : Form
    {
        GEOM geom;
        GEOM.SlotrayIntersection[] slotRay;
        string displayFile;
        int version;

        public SlotRayDataDisplay(GEOM mesh, string filename)
        {
            InitializeComponent();
            geom = mesh;
            slotRay = mesh.SlotrayAdjustments;
            displayFile = filename;
            version = mesh.meshVersion;
         }

        private void SlotRayDataDisplay_Load(object sender, EventArgs e)
        {
            this.Text = "Slotray Intersection Data: " + displayFile;
            SlotRay_dataGridView.Rows.Clear();
            if (version >= 14)
            {
                SlotRay_dataGridView.Columns[0].HeaderText = "Slot Hash";
                SlotRay_dataGridView.Columns[7].HeaderText = "Pivot Hash";
            }
            int counter = 0;
            for (int i = 0; i < slotRay.Length; i++)
            {
                GEOM.SlotrayIntersection s = slotRay[i];
                string[] str = new string[8];
                if (s.ParentVersion >= 14)
                {
                    str[0] = "0x" + s.SlotBone.ToString("X8");
                }
                else
                {
                    str[0] = s.SlotBone.ToString();
                }
                str[1] = s.TrianglePointIndices[0].ToString() + ", " + s.TrianglePointIndices[1].ToString() + ", " + s.TrianglePointIndices[2].ToString();
                Triangle tri = new Triangle(geom.getPosition(s.TrianglePointIndices[0]), geom.getPosition(s.TrianglePointIndices[1]), geom.getPosition(s.TrianglePointIndices[2]));
                Vector3 intersection = tri.WorldCoordinates(s.Coordinates);
                str[2] = s.Coordinates.ToString() + Environment.NewLine + "(" + intersection.ToString() + ")";
                str[3] = s.Distance.ToString();
                str[4] = s.OffsetFromIntersectionOS.ToString();
                str[5] = s.SlotAveragePosOS.ToString();
                str[6] = s.TransformToLS.ToString();
                if (s.ParentVersion >= 14)
                {
                    if (s.PivotBoneHash > 0) str[7] = "0x" + s.PivotBoneHash.ToString("X8");
                }
                else
                {
                    str[7] = s.PivotBoneIndex.ToString();
                }                                
                int l = SlotRay_dataGridView.Rows.Add(str);
                SlotRay_dataGridView.Rows[l].HeaderCell.Value = counter.ToString();
                counter++;

                //Vector3 rayDirection = intersection - s.SlotAveragePosOS;
                //rayDirection.Normalize();
                //Vector3 origin_AvgPosInside = intersection - (s.Distance * rayDirection);
                //Vector3 origin_AvgPosOutside = intersection + (s.Distance * rayDirection);
            }
        }
    }
}
