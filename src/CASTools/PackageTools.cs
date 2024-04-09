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
using Xmods.DataLib;
using s4pi.Interfaces;
using s4pi.Package;

namespace XMODS
{
    public partial class Form1 : Form
    {
        //Fix bone problems that cause crashing

        private void BoneFixFilename_button_Click(object sender, EventArgs e)
        {
            BoneFixFilename.Text = GetFilename("Select package to fix", Packagefilter);
        }

        private void BoneFixGo_button_Click(object sender, EventArgs e)
        {
            testPack = OpenPackage(BoneFixFilename.Text, true);
            if (testPack == null)
            {
                MessageBox.Show("Cannot read package file!");
                return;
            }
            Predicate<IResourceIndexEntry> gameconv = r => r.ResourceType == (uint)XmodsEnums.ResourceTypes.GEOM;
            List<IResourceIndexEntry> rgList = testPack.FindAll(gameconv);
            if (rgList == null)
            {
                MessageBox.Show("No GEOM meshes found in the package!");
                return;
            }
            int minBones = (int)MinBones_numericUpDown.Value;

            foreach (IResourceIndexEntry rg in rgList)
            {
                GEOM S4geom = new GEOM();
                Stream sg = testPack.GetResource(rg);
                sg.Position = 0;
                BinaryReader brg = new BinaryReader(sg);
                try
                {
                    S4geom = new GEOM(brg);
                }
                catch
                {
                    MessageBox.Show("Can't read the GEOM file: " + rg.ToString());
                    return;
                }

                if (!S4geom.BoneFixer(minBones))
                {
                    if (S4geom.numberBones > 60)
                    {
                        MessageBox.Show("At least one mesh in this package has more than 60 bones - unable to fix!");
                    }
                    else
                    {
                        MessageBox.Show("At least one mesh in this package has bone assignments outside the bone list - unable to fix!");
                    }
                    return;
                }

                testPack.DeleteResource(rg);
                MemoryStream mg = new MemoryStream();
                BinaryWriter bwg = new BinaryWriter(mg);
                S4geom.WriteFile(bwg);
                IResourceIndexEntry irieMesh = testPack.AddResource(rg, mg, true);
                irieMesh.Compressed = (ushort)0x5A42;
                sg.Dispose();
            }
            MessageBox.Show("Done!");
            WritePackage("Save modified package", testPack, "");
        }

        // Mesh Conversion

        private void MeshGameConvertSelect_button_Click(object sender, EventArgs e)
        {
            MeshGameConvertFilename.Text = GetFilename("Select Package", Packagefilter);
        }

        private void MeshGameConvertGo_button_Click(object sender, EventArgs e)
        {
            MeshGameConvertGo();
        }

        private void MeshGameConvertGo()
        {
            testPack = OpenPackage(MeshGameConvertFilename.Text, true);
            if (testPack == null)
            {
                MessageBox.Show("Cannot read package file!");
                return;
            }
            Predicate<IResourceIndexEntry> gameconv = r => r.ResourceType == (uint)XmodsEnums.ResourceTypes.GEOM;
            List<IResourceIndexEntry> rgList = testPack.FindAll(gameconv);
            if (rgList == null)
            {
                MessageBox.Show("No GEOM meshes found in the package!");
                return;
            }
            foreach (IResourceIndexEntry rg in rgList)
            {
                GEOM S4geom = new GEOM();
                Stream sg = testPack.GetResource(rg);
                sg.Position = 0;
                BinaryReader brg = new BinaryReader(sg);
                try
                {
                    S4geom = new GEOM(brg);
                }
                catch
                {
                    MessageBox.Show("Can't read the GEOM file: " + rg.ToString());
                    return;
                }

                for (int i = 0; i < S4geom.numberVertices; i++)
                {
                    byte[] bw = S4geom.getBoneWeights(i);
                    byte tmp = bw[2];
                    bw[2] = bw[0];
                    bw[0] = tmp;
                    S4geom.setBoneWeights(i, bw);
                }

                testPack.DeleteResource(rg);
                MemoryStream mg = new MemoryStream();
                BinaryWriter bwg = new BinaryWriter(mg);
                S4geom.WriteFile(bwg);
                IResourceIndexEntry irieMesh = testPack.AddResource(rg, mg, true);
                irieMesh.Compressed = (ushort)0x5A42;
                sg.Dispose();
            }
            MessageBox.Show("Done!");
            WritePackage("Save modified package", testPack, "");
        }

    }
}
