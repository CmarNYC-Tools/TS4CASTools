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
using System.Xml;
using Xmods.DataLib;
using s4pi.Interfaces;
using s4pi.Package;
using Collada141;
using System.Xml.Serialization;

namespace XMODS
{
    public partial class Form1 : Form
    {
        internal bool GetDAEData(string file, bool flipYZ, out DAE dae, bool verbose)
        {
            try
            {
                dae = new DAE(file, flipYZ);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: Could not load file " + file + ". Error: " + ex.Message + Environment.NewLine + ex.StackTrace.ToString());
                dae = null;
                return false;
            }
            return true;
        }

        public string LoadCollada(string path)
        {
            COLLADA dae = COLLADA.Load(path);
            string output = "";
            output += "Up axis: " + dae.asset.up_axis.ToString() + Environment.NewLine + Environment.NewLine;
            // Iterate on libraries
            foreach (var item in dae.Items)
            {
                if (item is library_geometries)
                {
                    var geometries = item as library_geometries;
                    if (geometries == null)
                        continue;

                    // Iterate on geometry in library_geometries 
                    foreach (var geom in geometries.geometry)
                    {
                        if (geom == null) continue;
                        output += "Geometry ID: " + geom.id + " Geometry Name: " + geom.name + Environment.NewLine;

                        var mesh = geom.Item as mesh;
                        if (mesh == null) continue;

                        // Dump source[] for geom
                        if (mesh.source != null)
                        {
                            foreach (var source in mesh.source)
                            {
                                if (source == null) continue;
                                var float_array = source.Item as float_array;
                                if (float_array == null) continue;

                                output += " source ID: " + source.id + " source name: " + source.name + " : count: " + float_array.count.ToString() +
                                    Environment.NewLine;
                                var tech_common = source.technique_common as sourceTechnique_common;
                                if (tech_common != null)
                                {
                                    if (tech_common.accessor != null)
                                    {
                                        output += "  accessor: count " + tech_common.accessor.count.ToString() + " stride " +
                                            tech_common.accessor.stride.ToString() + " params: ";
                                        foreach (param p in tech_common.accessor.param)
                                        {
                                            output += p.name;
                                        }
                                        output += Environment.NewLine;
                                    }
                                }

                                //foreach (var mesh_source_value in float_array.Values)
                                //    output += mesh_source_value + ", ";
                                //output += Environment.NewLine + Environment.NewLine;
                            }
                        }

                        var meshVerts = mesh.vertices;
                        if (meshVerts != null)
                        {
                            var vertInputs = meshVerts.input;
                            output += " vertices ID: " + meshVerts.id + " vertices name: " + meshVerts.name + " : source count: " + vertInputs.Length.ToString() +
                                Environment.NewLine;
                            foreach (var vertInput in vertInputs)
                                output += "  input semantic: " + vertInput.semantic + " source: " + vertInput.source + Environment.NewLine;
                        }

                        // Dump Items[] for geom
                        if (mesh.Items == null) continue;
                        foreach (var meshItem in mesh.Items)
                        {
                            if (meshItem is polygons)
                            {
                                polygons polys = meshItem as polygons;
                                var inputs = polys.input;
                                output += " polygons: " + polys.count.ToString() + " inputs: " + inputs.Length.ToString() + Environment.NewLine;
                                foreach (var polyItem in polys.Items)
                                {
                                    if (polyItem is string)
                                    {
                                        string[] pi = polyItem.ToString().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                                        output += "  points: " + pi.Length.ToString() + " elements" + Environment.NewLine;
                                    }
                                }
                                foreach (var input in inputs)
                                    output += "  input semantic: " + input.semantic + " source: " + input.source + " offset: " + input.offset.ToString() +
                                        (input.setSpecified ? " Set: " + input.set.ToString() : "") + Environment.NewLine;
                            }
                            else if (meshItem is polylist)
                            {
                                polylist polys = meshItem as polylist;
                                var inputs = polys.input;
                                output += " polylist: " + polys.count.ToString() + " inputs: " + inputs.Length.ToString() + Environment.NewLine;
                                string[] vc = polys.vcount.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                                string[] p = polys.p.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                                if (polys.vcount != null) output += "  vcount: " + vc.Length.ToString() + " elements;";
                                if (polys.p != null) output += " indices: " + p.Length.ToString() + " elements";
                                output += Environment.NewLine;
                                foreach (var input in inputs)
                                    output += "  input semantic: " + input.semantic + " source: " + input.source + " offset: " + input.offset.ToString() +
                                        (input.setSpecified ? " Set: " + input.set.ToString() : "") + Environment.NewLine;
                            }
                            else if (meshItem is vertices)
                            {
                                var vertices = meshItem as vertices;
                                var inputs = vertices.input;
                                output += " vertices input: " + inputs.Length.ToString() + Environment.NewLine;
                            }
                            else if (meshItem is triangles)
                            {
                                var triangles = meshItem as triangles;
                                var inputs = triangles.input;
                                output += " triangles: " + triangles.count.ToString() + " elements; inputs: " + inputs.Length.ToString() + Environment.NewLine;
                                if (triangles.p != null)
                                {
                                    string[] p = triangles.p.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                                    output += "  indices: " + p.Length.ToString() + " elements" + Environment.NewLine;
                                }
                                foreach (var input in inputs)
                                    output += "  input semantic: " + input.semantic + " source: " + input.source + " offset: " + input.offset.ToString() +
                                        (input.setSpecified ? " Set: " + input.set.ToString() : "") + Environment.NewLine;
                                //output += "\\t Indices " + triangles.p + ", ";
                            }
                            else
                            {
                                output += " mesh item: " + meshItem.ToString() + Environment.NewLine;
                            }
                        }
                        output += Environment.NewLine;
                    }
                }

                else if (item is library_controllers)
                {
                    var rigs = item as library_controllers;
                    if (rigs == null || rigs.controller == null) continue;
                    foreach (var rig in rigs.controller)
                    {
                        if (rig == null) continue;
                        output += "Rig ID: " + rig.id + " Rig Name: " + rig.name + Environment.NewLine;
                        if (rig.Item is skin && rig.Item != null)
                        {
                            var rigSkin = rig.Item as skin;
                            output += " Mesh: " + rigSkin.source1 + Environment.NewLine;
                            foreach (source s in rigSkin.source)
                            {
                                var access = s.technique_common.accessor;
                                string param = access.param[0].name;
                                output += " Rig input: " + s.id + access.source + " " + param + " : ";
                                if (string.Compare(param, "JOINT") == 0)
                                {
                                    output += " joint names array: ";
                                    var joints = s.Item as Name_array;
                                    if (joints != null && joints.Values != null)
                                    {
                                        // output += joints._Text_;
                                        for (int i = 0; i < joints.Values.Length; i++) output += joints.Values[i] + ", "; //joint names
                                    }
                                }
                                else if (string.Compare(param, "TRANSFORM") == 0)
                                {
                                    output += " inverse transform: " + access.count.ToString() + " elements";
                                }
                                else if (string.Compare(param, "WEIGHT") == 0)
                                {
                                    output += " weights array: " + access.count.ToString() + " elements";
                                }
                                else
                                {
                                    output += access.count.ToString() + " elements";
                                }
                                output += Environment.NewLine;
                            }
                           // string[] vcount = rigSkin.vertex_weights.vcount.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                            string[] v = rigSkin.vertex_weights.v.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                            output += " vcount (array of number weights per vertex): " + rigSkin.vertex_weights.count.ToString() + " elements"; // number of weights per vertex
                            output += "; v count (array of joint index, weight index): " + v.Length.ToString() + " elements" + Environment.NewLine;   // bone assignments
                        }
                        output += Environment.NewLine;
                    }
                }

                else if (item is library_visual_scenes)
                {
                    library_visual_scenes scenes = item as library_visual_scenes;
                    if (scenes == null || scenes.visual_scene == null) continue;
                    foreach (visual_scene scene in scenes.visual_scene)
                    {
                        foreach (node n in scene.node)
                        {
                            if (n.instance_geometry != null)
                            {
                                output += Environment.NewLine + "Scene Geometry Transform: " + Environment.NewLine + ReadTransform(n);                                       
                                foreach (instance_geometry g in n.instance_geometry)
                                {
                                    output += Environment.NewLine + " geometry: " + g.name + " url: " + g.url;
                                }
                            }

                            else if (n.instance_controller != null)
                            {
                                output += Environment.NewLine + "Scene Controller Transform: " + Environment.NewLine + ReadTransform(n);
                                foreach (instance_controller c in n.instance_controller)
                                {
                                    output += Environment.NewLine + " controller: " + c.name + " url: " + c.url;
                                }
                            }
                            else if (n.type == NodeType.NODE & n.node1 != null)
                            {
                                output += Environment.NewLine + "Skeleton:" + Environment.NewLine + ReadTransform(n);
                                foreach (var b in n.node1)
                                {
                                    if (b is node && b.type == NodeType.JOINT)
                                    {
                                        output += ReadSkeletonJoint(b as node, " - ");
                                    }
                                }
                            }
                            else if (n.type == NodeType.JOINT)
                            {
                                output += ReadSkeletonJoint(n, " - ");
                            }
                        }
                    }
                }
            }     
            
            return output;
        }

        internal string ReadSkeletonJoint(node joint, string parentName)
        {
            if (joint.type != NodeType.JOINT) return "";
            string output = " " + joint.sid + " / parent: " + parentName + Environment.NewLine;
           // output += ReadTransform(joint);

            if (joint.node1 == null) return output;
            foreach (var jointNode in joint.node1)
            {
                if (jointNode is node && ((node)jointNode).type == NodeType.JOINT)
                {
                    output += ReadSkeletonJoint(jointNode as node, joint.sid);
                }
            }
            return output;
        }

        private string ReadTransform(node n)
        {
            string output = "";
            if (n.Items == null) return output;
            for (int i = 0; i < n.Items.Length; i++)
            {
                if (n.ItemsElementName[i] == ItemsChoiceType2.matrix)
                {
                    matrix mj = n.Items[i] as matrix;
                    output += "  Transform Matrix: ";
                    for (int j = 0; j < mj.Values.Length; j++) output += mj.Values[j].ToString() + " ";
                }
                else if (n.ItemsElementName[i] == ItemsChoiceType2.translate)
                {
                    TargetableFloat3 mj = n.Items[i] as TargetableFloat3;
                    output += "  Translation: ";
                    for (int j = 0; j < mj.Values.Length; j++) output += mj.Values[j].ToString() + " ";
                }
                else if (n.ItemsElementName[i] == ItemsChoiceType2.rotate)
                {
                    rotate mj = n.Items[i] as rotate;
                    output += "  Rotation: ";
                    for (int j = 0; j < mj.Values.Length; j++) output += mj.Values[j].ToString() + " ";
                }
                else if (n.ItemsElementName[i] == ItemsChoiceType2.scale)
                {
                    TargetableFloat3 mj = n.Items[i] as TargetableFloat3;
                    output += "  Scale: ";
                    for (int j = 0; j < mj.Values.Length; j++) output += mj.Values[j].ToString() + " ";
                }
                output += Environment.NewLine;
            }
            return output;
        }

        private void DisplayColladaFile_button_Click(object sender, EventArgs e)
        {
            string file = GetFilename("Select Collada .DAE mesh file", DAEfilter);
            DisplayColladaFile.Text = file;
            ColladaOutput.Text = LoadCollada(file);
        }
    }
}
