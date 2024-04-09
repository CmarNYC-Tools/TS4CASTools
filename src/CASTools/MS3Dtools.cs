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
        MS3D displayMS3D;
        string myFile;

        internal bool GetMS3DData(string file, out MS3D outMS3D, bool verbose)
        {
            Stream myStream = null;
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.FileName = file;
            MS3D newMS3D = null;
            try
            {
                if ((myStream = openFileDialog1.OpenFile()) != null)
                {
                    using (myStream)
                    {
                        myStream.Position = 0;
                        BinaryReader br = new BinaryReader(myStream);
                        newMS3D = new MS3D(br);
                    }
                    myStream.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: Could not read file " + openFileDialog1.FileName + ". Original error: " + ex.Message + Environment.NewLine + ex.StackTrace.ToString());
                myStream.Close();
                outMS3D = newMS3D;
                return false;
            }
            if (verbose && newMS3D.VertexExtraArray.Length == 0)
            {
                DialogResult res = MessageBox.Show("Some bone assignment and/or vertex numbering information may be missing from the MS3D mesh. Continue anyway?", "Incomplete file", MessageBoxButtons.OKCancel);
                outMS3D = newMS3D;
                if (res == DialogResult.Cancel) return false;
            }
            outMS3D = newMS3D;
            return true;
        }

        internal string WriteMS3DFile(string title, MS3D myMS3D, string defaultFilename)
        {
            Stream myStream = null;
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = MS3Dfilter;
            saveFileDialog1.FilterIndex = 1;
            saveFileDialog1.Title = title;
            saveFileDialog1.AddExtension = true;
            saveFileDialog1.CheckPathExists = true;
            saveFileDialog1.DefaultExt = "ms3d";
            saveFileDialog1.OverwritePrompt = true;
            saveFileDialog1.Title = title;
            if (String.CompareOrdinal(defaultFilename, " ") > 0) saveFileDialog1.FileName = defaultFilename;
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if ((myStream = saveFileDialog1.OpenFile()) != null)
                    {
                        using (myStream)
                        {
                            BinaryWriter bw = new BinaryWriter(myStream);
                            myMS3D.Write(bw);
                        }
                        myStream.Close();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not write file " + saveFileDialog1.FileName + ". Original error: " + ex.Message + Environment.NewLine + ex.StackTrace.ToString());
                    myStream.Close();
                }
                return saveFileDialog1.FileName;
            }
            else
            {
                return "";
            }
        }

        private void MS3Dfilename_button_Click(object sender, EventArgs e)
        {
            MS3Dfilename.Text = GetFilename("Select MS3D file to examine", MS3Dfilter);
            MS3DdataDisplay();
        }

        private void MS3DdataDisplay()
        {
            myFile = MS3Dfilename.Text;
            if (String.CompareOrdinal(myFile, "") > 0)
            {
                if (!File.Exists(myFile))
                {
                    MessageBox.Show(myFile + " does not exist!");
                    return;
                }
                GetMS3DData(myFile, out displayMS3D, false);
                MS3DnumVerts.Text = displayMS3D.NumberVertices.ToString();
                MS3DnumFaces.Text = displayMS3D.NumberFaces.ToString();
                MS3DnumGroups.Text = displayMS3D.NumberGroups.ToString();
                MS3DnumMaterials.Text = displayMS3D.NumberMaterials.ToString();
                string[] gcomments = displayMS3D.GroupComments;
                MS3DgroupComments.Text = "";
                for (int i = 0; i < gcomments.Length; i++)
                {
                    MS3DgroupComments.Text += ("Group " + i.ToString() + ": " + displayMS3D.getGroupName(i) + Environment.NewLine +
                        gcomments[i] + Environment.NewLine + Environment.NewLine);
                }
                MS3DjointsList.Text = displayMS3D.JointsList;
            }
        }

        private void MS3DVertexDisplay_button_Click(object sender, EventArgs e)
        {
            MS3DVertexDisplay mvd = new MS3DVertexDisplay(displayMS3D, myFile);
            mvd.Show();
        }

        private void MS3DFaceDisplay_button_Click(object sender, EventArgs e)
        {
            MS3DFacesDisplay mfd = new MS3DFacesDisplay(displayMS3D, myFile);
            mfd.Show();
        }

        private void MS3DcopyUVfrom_button_Click(object sender, EventArgs e)
        {
            MS3DcopyUVfrom.Text = GetFilename("Select MS3D mesh to copy UV from", MS3Dfilter);
        }

        private void MS3DcopyUVto_button_Click(object sender, EventArgs e)
        {
            MS3DcopyUVto.Text = GetFilename("Select MS3D mesh to copy UV to", MS3Dfilter);
        }

        private void MS3DcopyUVGo_button_Click(object sender, EventArgs e)
        {
            if (!File.Exists(MS3DcopyUVfrom.Text) || !File.Exists(MS3DcopyUVto.Text))
            {
                MessageBox.Show("The source and/or target mesh does not exist!");
                return;
            }
            MS3D ms3dFrom, ms3dTo;
            if (!GetMS3DData(MS3DcopyUVfrom.Text, out ms3dFrom, false) | !GetMS3DData(MS3DcopyUVto.Text, out ms3dTo, false))
            {
                MessageBox.Show("Can't read one or both MS3D meshes!");
                return;
            }
            ms3dTo.CopyUV(ms3dFrom);
            WriteMS3DFile("Save modified MS3D mesh", ms3dTo, "");
        }

        private void MS3Dmerge1_button_Click(object sender, EventArgs e)
        {
            MS3Dmerge1.Text = GetFilename("Select first .ms3d file", MS3Dfilter);
        }

        private void MS3Dmerge2_button_Click(object sender, EventArgs e)
        {
            MS3Dmerge2.Text = GetFilename("Select second .ms3d file", MS3Dfilter);
        }

        private void MS3DmergeGo_button_Click(object sender, EventArgs e)
        {
            if (String.Compare(MS3Dmerge1.Text, " ") <= 0 || String.Compare(MS3Dmerge2.Text, " ") <= 0)
            {
                MessageBox.Show("You must select two .ms3d mesh files!");
                return;
            }
            MS3D ms3d1 = null;
            MS3D ms3d2 = null;
            if (GetMS3DData(MS3Dmerge1.Text, out ms3d1, false) & GetMS3DData(MS3Dmerge2.Text, out ms3d2, false))
            {
                ms3d1.Merge(ms3d2);
                WriteMS3DFile("Save merged .ms3d mesh file", ms3d1, "");
            }
            else
            {
                MessageBox.Show("Can't read one or both .ms3d mesh files");
            }
        }
    }
}
