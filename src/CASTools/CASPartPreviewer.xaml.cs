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
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Xmods.DataLib;

namespace XMODS
{
    /// <summary>
    /// Interaction logic for CASPartPreviewer.xaml
    /// </summary>
    public partial class CASPartPreviewer : UserControl
    {
        AxisAngleRotation3D rot_x;
        AxisAngleRotation3D rot_y;
        ScaleTransform3D zoom = new ScaleTransform3D(1, 1, 1);
        Transform3DGroup modelTransform, cameraTransform;
        AmbientLight AmbLight = new AmbientLight();
        DirectionalLight DirLight1 = new DirectionalLight();
        DirectionalLight DirLight2 = new DirectionalLight();
        PerspectiveCamera Camera1 = new PerspectiveCamera();
        Model3DGroup modelGroup = new Model3DGroup();
        Viewport3D myViewport = new Viewport3D();
        GeometryModel3D[] myMeshes = null;
        bool[] isGlass = null;
        bool[] isWings = null;
        GeometryModel3D myHead = new GeometryModel3D();
        GeometryModel3D myTop = new GeometryModel3D();
        GeometryModel3D myBottom = new GeometryModel3D();
        GeometryModel3D myFeet = new GeometryModel3D();
        GeometryModel3D myBody = new GeometryModel3D();
        GeometryModel3D myEars = new GeometryModel3D();
        GeometryModel3D myTail = new GeometryModel3D();
        DiffuseMaterial mySkinColor = new DiffuseMaterial();
        DiffuseMaterial mySkin = new DiffuseMaterial();
        DiffuseMaterial myUnderwear = new DiffuseMaterial();
        DiffuseMaterial myDiffuse = new DiffuseMaterial();
        SpecularMaterial mySpecular = new SpecularMaterial();
        DiffuseMaterial myShadow = new DiffuseMaterial();
        DiffuseMaterial myBumpmap = new DiffuseMaterial();
        EmissiveMaterial myGlowmap = new EmissiveMaterial();
        MaterialGroup myMaterials = new MaterialGroup();
        MaterialGroup glassMaterials = new MaterialGroup();
        MaterialGroup wingMaterials = new MaterialGroup();
        
        DiffuseMaterial myWingDiffuse = new DiffuseMaterial();
        SpecularMaterial myWingSpecular = new SpecularMaterial();

        int currentLod;
        XmodsEnums.Species currentSpecies;
        XmodsEnums.Age currentAge;
        XmodsEnums.Gender currentGender;

        public CASPartPreviewer()
        {
            InitializeComponent();
            rot_x = new AxisAngleRotation3D(new Vector3D(1, 0, 0), 0);
            rot_y = new AxisAngleRotation3D( new Vector3D(0, 1, 0), 0);

            cameraTransform = new Transform3DGroup();
            cameraTransform.Children.Add(zoom);
           // cameraTransform.Children.Add(center);
            modelTransform = new Transform3DGroup();
            modelTransform.Children.Add(new RotateTransform3D(rot_y));
            modelTransform.Children.Add(new RotateTransform3D(rot_x));

            AmbLight.Color = Color.FromArgb(255, 40, 40, 40);
            DirLight1.Color = Colors.White;
            DirLight1.Direction = new Vector3D(.5, -.5, -1);
            DirLight2.Color = Colors.White;
            DirLight2.Direction = new Vector3D(-.5, -.5, -1);

            Camera1.FarPlaneDistance = 20;
            Camera1.NearPlaneDistance = 0.05;
            Camera1.FieldOfView = 45;
            Camera1.LookDirection = new Vector3D(0, 0, -3);
            Camera1.UpDirection = new Vector3D(0, 1, 0);
            ModelVisual3D modelsVisual = new ModelVisual3D();
            modelsVisual.Content = modelGroup;

           // skinMesh.Transform = modelTransform;
           // glassMesh.Transform = modelTransform;
            myHead.Transform = modelTransform;
            myTop.Transform = modelTransform;
            myBottom.Transform = modelTransform;
            myFeet.Transform = modelTransform;
            myBody.Transform = modelTransform;
            myEars.Transform = modelTransform;
            myTail.Transform = modelTransform;

            myViewport.Camera = Camera1;
            myViewport.Children.Add(modelsVisual);
            myViewport.Height = 535;
            myViewport.Width = 504;
            myViewport.Camera.Transform = cameraTransform;
            this.canvas1.Children.Insert(0, myViewport);

            Canvas.SetTop(myViewport, 0);
            Canvas.SetLeft(myViewport, 0);
            this.Width = myViewport.Width;
            this.Height = myViewport.Height;
        }

        internal void SimMeshes(GEOM[] simgeoms, out MeshGeometry3D[] meshGeometry, out bool[] isGlass, out bool[] isWings)
        {
            List<MeshGeometry3D> geoms = new List<MeshGeometry3D>();
            List<bool> glass = new List<bool>();
            List<bool> wings = new List<bool>();
            foreach (GEOM g in simgeoms)
            {
                geoms.Add(SimMesh(g));
                glass.Add(g.ShaderType == XmodsEnums.Shader.SimGlass);
                wings.Add(g.ShaderType == XmodsEnums.Shader.SimWings);
            }
            // MeshGeometry3D[] tmp = new MeshGeometry3D[2];
            // tmp[0] = SimMesh(skinGeoms.ToArray());
            // tmp[1] = SimMesh(glassGeoms.ToArray());
            // return tmp;

            meshGeometry = geoms.ToArray();
            isGlass = glass.ToArray();
            isWings = wings.ToArray();
        }

        internal void SimMeshes(GEOM[] simgeoms, MorphMap deform_Shape, MorphMap deform_Normals, out MeshGeometry3D[] meshGeometry, out bool[] isGlass)
        {
            List<MeshGeometry3D> geoms = new List<MeshGeometry3D>();
            List<bool> glass = new List<bool>();
            foreach (GEOM g in simgeoms)
            {
                geoms.Add(SimMesh(g, deform_Shape, deform_Normals));
                glass.Add(g.ShaderType != XmodsEnums.Shader.SimSkin);
            }
            // MeshGeometry3D[] tmp = new MeshGeometry3D[2];
            // tmp[0] = SimMesh(skinGeoms.ToArray());
            // tmp[1] = SimMesh(glassGeoms.ToArray());
            // return tmp;

            meshGeometry = geoms.ToArray();
            isGlass = glass.ToArray();
        }

        MeshGeometry3D SimMesh(GEOM simgeom)
        {
            return SimMesh(simgeom, null, null);
        }
        MeshGeometry3D SimMesh(GEOM simgeom, DMap morphShape)
        {
            return SimMesh(simgeom, null, null);
        }
        MeshGeometry3D SimMesh(GEOM simgeom, MorphMap morphShape, MorphMap morphNormals)
        {
            MeshGeometry3D mesh = new MeshGeometry3D();
            Point3DCollection verts = new Point3DCollection();
            Vector3DCollection normals = new Vector3DCollection();
            PointCollection uvs = new PointCollection();
            Int32Collection facepoints = new Int32Collection();
            float centerOffset = 0.80f;

            GEOM g = simgeom;
            GEOM.GeometryState geostate = g.GeometryStates.FirstOrDefault() ?? new GEOM.GeometryState() { VertexCount = g.numberVertices, PrimitiveCount = g.numberFaces };

            for (int i = geostate.MinVertexIndex; i < geostate.VertexCount; i++)
            {
                float[] pos = g.getPosition(i);
                float[] norm = g.getNormal(i);
                if (morphShape != null && g.hasUVset(1))
                {
                    List<float[]> stitchList = g.GetStitchUVs(i);
                    Vector3 shapeVector = new Vector3();
                    Vector3 normVector = new Vector3();
                    if (stitchList.Count > 0)
                    {
                        foreach (float[] stitch in stitchList)
                        {
                            int x = (int)(Math.Abs((morphShape.MapWidth - 1) * stitch[0]) - morphShape.MinCol);
                            int y = (int)(((morphShape.MapHeight - 1) * stitch[1]) - morphShape.MinRow);
                            if (x >= 0 && x < (morphShape.MaxCol - morphShape.MinCol + 1) &&
                                y >= 0 && y < (morphShape.MaxRow - morphShape.MinRow + 1))
                            {
                               // Vector3 deltaShape = morphShape.GetAdjustedDelta(x, y, pos[0] < 0, (byte)(g.getTagval(i) & 0xFF));
                                Vector3 deltaShape = morphShape.GetAdjustedDelta(x, y, pos[0] < 0, (byte)(g.getTagval(i) & 0x3F));  //as of Cats & Dogs
                                shapeVector += deltaShape;
                                if (morphNormals != null)
                                {
                                  //  Vector3 deltaNorm = morphNormals.GetAdjustedDelta(x, y, pos[0] < 0, (byte)(g.getTagval(i) & 0xFF));
                                    Vector3 deltaNorm = morphNormals.GetAdjustedDelta(x, y, pos[0] < 0, (byte)(g.getTagval(i) & 0x3F));
                                    normVector += deltaNorm;
                                }
                            }
                        }
                        shapeVector = shapeVector / (float)stitchList.Count;
                        normVector = normVector / (float)stitchList.Count;
                    }
                    else
                    {
                        float[] uv1 = g.getUV(i, 1);
                        int x = (int)(Math.Abs(morphShape.MapWidth * uv1[0]) - morphShape.MinCol);
                        int y = (int)((morphShape.MapHeight * uv1[1]) - morphShape.MinRow);
                        if (x >= 0 && x < (morphShape.MaxCol - morphShape.MinCol) &&
                            y >= 0 && y < (morphShape.MaxRow - morphShape.MinRow))
                        {
                            shapeVector = morphShape.GetAdjustedDelta(x, y, pos[0] < 0, (byte)(g.getTagval(i) & 0x3F));
                            if (morphNormals != null)
                            {
                                normVector = morphNormals.GetAdjustedDelta(x, y, pos[0] < 0, (byte)(g.getTagval(i) & 0x3F));
                            }
                         }
                    }
                    pos[0] -= shapeVector.X;
                    pos[1] -= shapeVector.Y;
                    pos[2] -= shapeVector.Z;
                    norm[0] -= normVector.X;
                    norm[1] -= normVector.Y;
                    norm[2] -= normVector.Z;
                }
                verts.Add(new Point3D(pos[0], pos[1] - centerOffset, pos[2]));
                normals.Add(new Vector3D(norm[0], norm[1], norm[2]));
                float[] uv = g.getUV(i, 0);
                uvs.Add(new Point(uv[0], uv[1]));
            } 

            for (int i = geostate.StartIndex; i < geostate.PrimitiveCount; i++)
            {
                int[] face = g.getFaceIndices(i);
                facepoints.Add(face[0]);
                facepoints.Add(face[1]);
                facepoints.Add(face[2]);
            }

            mesh.Positions = verts;
            mesh.TriangleIndices = facepoints;
            mesh.Normals = normals;
            mesh.TextureCoordinates = uvs;
            return mesh;
        }

        internal byte[] GetMorph(int x, int y, System.Drawing.Bitmap skin, System.Drawing.Bitmap robe,
                                    bool robeDataPresent, uint vertexColor)
        {
            System.Drawing.Color skinColor = skin.GetPixel(Math.Abs(x), y);
            byte[] tmp = new byte[] { skinColor.B, skinColor.G, skinColor.R };
            if (robeDataPresent)
            {
                System.Drawing.Color robeColor = robe.GetPixel(Math.Abs(x), y);
                float robeWeight = (vertexColor & 0xFF) / 255f;
                tmp[0] = (byte)(((1f - robeWeight) * tmp[0]) + (robeWeight * robeColor.B));
                tmp[1] = (byte)(((1f - robeWeight) * tmp[1]) + (robeWeight * robeColor.G));
                tmp[2] = (byte)(((1f - robeWeight) * tmp[2]) + (robeWeight * robeColor.R));
            }
            return tmp;
        }

        internal ImageBrush GetImageBrush(System.Drawing.Image image)
        {
            BitmapImage bmpImg = new BitmapImage();
            MemoryStream ms = new MemoryStream();
            image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            ms.Position = 0;
            bmpImg.BeginInit();
            bmpImg.StreamSource = ms;
            bmpImg.EndInit();
            ImageBrush img = new ImageBrush();
            img.ImageSource = bmpImg;
            img.Stretch = Stretch.Fill;
            img.TileMode = TileMode.None;
            img.ViewportUnits = BrushMappingMode.Absolute;
            return img;
        }

        public void Start_Mesh(GEOM[] simgeoms, int lod, System.Drawing.Image diffuseTexture, System.Drawing.Image specularTexture,
            System.Drawing.Image shadowTexture, System.Drawing.Image bumpTexture, System.Drawing.Image glowTexture, 
            XmodsEnums.Species species, XmodsEnums.Age age, XmodsEnums.Gender gender,
            Color skinColor, bool showSkinOverlay, bool showLight1, bool showLight2,
            bool showHead, bool showTop, bool showBottom, bool showFeet, bool showUndies,
            bool showBody, bool showEars, bool showTail)
        {
            // MeshGeometry3D[] simMesh = SimMeshes(simgeoms);
            // skinMesh.Geometry = simMesh[0];
            // glassMesh.Geometry = simMesh[1];
            var wingPreview = simgeoms.Any(x => x.ShaderType == XmodsEnums.Shader.SimWings);

            currentLod = lod;
            currentAge = age;
            currentGender = gender;
            currentSpecies = species;

            rot_x.Angle = sliderXRot.Value;
            rot_y.Angle = sliderYRot.Value;
            Camera1.Position = new Point3D(-sliderXMove.Value, -sliderYMove.Value, -sliderZoom.Value);

            GetPartGeometries(lod, species, age, gender);
            GetPartMaterials(species, age, gender);

            myMaterials.Children.Clear();
            glassMaterials.Children.Clear();
            wingMaterials.Children.Clear();
            mySkinColor = new DiffuseMaterial(new SolidColorBrush(skinColor));
            myMaterials.Children.Add(mySkinColor);
            if (showSkinOverlay) myMaterials.Children.Add(mySkin);
            if (showUndies) myMaterials.Children.Add(myUnderwear);

            if (shadowTexture != null)
            {
                myShadow = new DiffuseMaterial(GetImageBrush(shadowTexture));
                myMaterials.Children.Add(myShadow);
            }
            if (diffuseTexture != null)
            {
                if (wingPreview)
                {
                    myWingDiffuse = new DiffuseMaterial(GetImageBrush(diffuseTexture));
                    wingMaterials.Children.Add(myWingDiffuse);
                }
                else
                {
                    myDiffuse = new DiffuseMaterial(GetImageBrush(diffuseTexture));
                    myMaterials.Children.Add(myDiffuse);
                    glassMaterials.Children.Add(myDiffuse);
                }
            }
            if (specularTexture != null)
            {
                if (wingPreview)
                {
                    myWingSpecular = new SpecularMaterial(GetImageBrush(specularTexture), 25d);
                    wingMaterials.Children.Add(mySpecular);
                }
                else
                {
                    mySpecular = new SpecularMaterial(GetImageBrush(specularTexture), 25d);
                    myMaterials.Children.Add(mySpecular);
                    glassMaterials.Children.Add(mySpecular);
                }
            }
            if (bumpTexture != null)
            {
                myBumpmap = new DiffuseMaterial(GetImageBrush(bumpTexture));
            }
            if (glowTexture != null)
            {
                myGlowmap = new EmissiveMaterial(GetImageBrush(glowTexture));
                myMaterials.Children.Add(myGlowmap);
                glassMaterials.Children.Add(myGlowmap);
            }

            myHead.Material = myMaterials;
            myTop.Material = myMaterials;
            myBottom.Material = myMaterials;
            myFeet.Material = myMaterials;
            myBody.Material = myMaterials;
            myEars.Material = myMaterials;
            myTail.Material = myMaterials;
            
            modelGroup.Children.Clear();
            modelGroup.Children.Add(AmbLight);
            if (showLight1) modelGroup.Children.Add(DirLight1);
            if (showLight2) modelGroup.Children.Add(DirLight2);
            if (showHead) modelGroup.Children.Add(myHead);
            if (showTop) modelGroup.Children.Add(myTop);
            if (showBottom) modelGroup.Children.Add(myBottom);
            if (showFeet) modelGroup.Children.Add(myFeet);
            if (showBody) modelGroup.Children.Add(myBody);
            if (showEars) modelGroup.Children.Add(myEars);
            if (showTail) modelGroup.Children.Add(myTail);

            MeshGeometry3D[] myGeoms;
            SimMeshes(simgeoms, out myGeoms, out isGlass, out isWings);
            myMeshes = new GeometryModel3D[myGeoms.Length];
            for (int i = 0; i < myMeshes.Length; i++)
            {
                myMeshes[i] = new GeometryModel3D();
                myMeshes[i].Geometry = myGeoms[i];
                myMeshes[i].Transform = modelTransform;
                if (isGlass[i])
                {
                    myMeshes[i].Material = glassMaterials;
                }
                else if (isWings[i])
                {
                    myMeshes[i].Material = wingMaterials;
                }
                else
                {
                    myMeshes[i].Material = myMaterials;
                }
                modelGroup.Children.Add(myMeshes[i]);
            }
        }

        public void ResetPreviewerView(XmodsEnums.Species species, XmodsEnums.Age age)
        {
            if (species != XmodsEnums.Species.Human)
            {
                if (age <= XmodsEnums.Age.Child) Camera1.Position = new Point3D(0, -0.6, 1.0);
                else if (species == XmodsEnums.Species.Cat || species == XmodsEnums.Species.LittleDog) Camera1.Position = new Point3D(0, -0.5, 1.25);
                else Camera1.Position = new Point3D(0, -0.25, 2);
            }
            else if ((age & XmodsEnums.Age.Infant) > 0) Camera1.Position = new Point3D(0, -0.15, 1.5);
            else if (age == XmodsEnums.Age.Toddler) Camera1.Position = new Point3D(0, -0.2, 2.0);
            else if (age == XmodsEnums.Age.Child) Camera1.Position = new Point3D(0, -0.15, 2.4);
            else Camera1.Position = new Point3D(0, 0, 2.8);

            sliderXMove.Value = -Camera1.Position.X;
            sliderYMove.Value = -Camera1.Position.Y;
            sliderZoom.Value = -Camera1.Position.Z;
            sliderXRot.Value = rot_x.Angle = 0;
            sliderYRot.Value = rot_y.Angle = 0;
        }

        private void GetPartGeometries(int lod, XmodsEnums.Species species, XmodsEnums.Age age, XmodsEnums.Gender gender)
        {
            GetPartGeometries(lod, species, age, gender, null, null);
        }
        private void GetPartGeometries(int lod, XmodsEnums.Species species, XmodsEnums.Age age, XmodsEnums.Gender gender, MorphMap deformShape)
        {
            GetPartGeometries(lod, species, age, gender, deformShape, null);
        }
        private void GetPartGeometries(int lod, XmodsEnums.Species species, XmodsEnums.Age age, XmodsEnums.Gender gender, MorphMap deformShape, MorphMap deformNormals)
        {
            System.Resources.ResourceManager rm = Properties.Resources.ResourceManager;
            string specifier = "";
            if ((age & XmodsEnums.Age.Infant) > 0) specifier = (species == XmodsEnums.Species.Human ? "i" : "c");
            else if (age == XmodsEnums.Age.Toddler) specifier = (species == XmodsEnums.Species.Human ? "p" : "c");
            else if (age == XmodsEnums.Age.Child) specifier = "c";
            else specifier = species == XmodsEnums.Species.Human ? "y" : "a";
            if (species != XmodsEnums.Species.Human) specifier +=
                (age == XmodsEnums.Age.Child && species == XmodsEnums.Species.LittleDog) ? "d" :
                (species == XmodsEnums.Species.Fox)?"x":
                species.ToString().Substring(0, 1).ToLower();
            else if (age <= XmodsEnums.Age.Child || (age & XmodsEnums.Age.Infant) > 0) specifier += "u";
            else specifier += (gender == XmodsEnums.Gender.Male || gender == XmodsEnums.Gender.Female) ? gender.ToString().Substring(0, 1).ToLower() : "m";

            myHead.Geometry = SimMesh(new GEOM(new BinaryReader(new MemoryStream((byte[])rm.GetObject(specifier + "Head_lod" + lod.ToString())))), deformShape, deformNormals);
            myFeet.Geometry = SimMesh(new GEOM(new BinaryReader(new MemoryStream((byte[])rm.GetObject(specifier + "Shoes_lod" + lod.ToString())))), deformShape, deformNormals);
            if (species == XmodsEnums.Species.Human)
            {
                myTop.Geometry = SimMesh(new GEOM(new BinaryReader(new MemoryStream((byte[])rm.GetObject(specifier + "Top_lod" + lod.ToString())))), deformShape, deformNormals);
                myBottom.Geometry = SimMesh(new GEOM(new BinaryReader(new MemoryStream((byte[])rm.GetObject(specifier + "Bottom_lod" + lod.ToString())))), deformShape, deformNormals);
            }
            else
            {
                myBody.Geometry = SimMesh(new GEOM(new BinaryReader(new MemoryStream((byte[])rm.GetObject(specifier + "Body_lod" + lod.ToString())))), deformShape, deformNormals);
                if(species != XmodsEnums.Species.Horse)
                    myEars.Geometry = SimMesh(new GEOM(new BinaryReader(new MemoryStream((byte[])rm.GetObject(specifier + "EarsUp_lod" + lod.ToString())))));
                myTail.Geometry = SimMesh(new GEOM(new BinaryReader(new MemoryStream((byte[])rm.GetObject(specifier + "Tail_lod" + lod.ToString())))));
            }
        }
        private void SetEarsGeometry(int lod, XmodsEnums.Species species, XmodsEnums.Age age, XmodsEnums.Gender gender, XmodsEnums.BodySubType subType)
        {
            System.Resources.ResourceManager rm = Properties.Resources.ResourceManager;
            string specifier = "";
            if ((age & XmodsEnums.Age.Infant) > 0) specifier = (species == XmodsEnums.Species.Human ? "i" : "c"); 
            else if (age == XmodsEnums.Age.Toddler) specifier = (species == XmodsEnums.Species.Human ? "p" : "c");
            else if (age == XmodsEnums.Age.Child) specifier = "c";
            else specifier = (species == XmodsEnums.Species.Human ? "y" : "a");
            if (species != XmodsEnums.Species.Human) specifier +=
                (age == XmodsEnums.Age.Child && species == XmodsEnums.Species.LittleDog) ? "d" :
                (species == XmodsEnums.Species.Fox)?"x":
                species.ToString().Substring(0, 1).ToLower();
            else if (age <= XmodsEnums.Age.Child || (age & XmodsEnums.Age.Infant) > 0) specifier += "u";
            else specifier += gender.ToString().Substring(0, 1).ToLower();
            if (subType == XmodsEnums.BodySubType.EarsDown)
            {
                myEars.Geometry = SimMesh(new GEOM(new BinaryReader(new MemoryStream((byte[])rm.GetObject(specifier + "EarsDown_lod" + lod.ToString())))));
            }
            else
            {
                myEars.Geometry = SimMesh(new GEOM(new BinaryReader(new MemoryStream((byte[])rm.GetObject(specifier + "EarsUp_lod" + lod.ToString())))));
            }
        }
        private void SetTailGeometry(int lod, XmodsEnums.Species species, XmodsEnums.Age age, XmodsEnums.Gender gender, XmodsEnums.BodySubType subType)
        {
            System.Resources.ResourceManager rm = Properties.Resources.ResourceManager;
            string specifier = "";
            if ((age & XmodsEnums.Age.Infant) > 0) specifier = (species == XmodsEnums.Species.Human ? "i" : "c"); 
            else if (age == XmodsEnums.Age.Toddler) specifier = (species == XmodsEnums.Species.Human ? "p" : "c");
            else if (age == XmodsEnums.Age.Child) specifier = "c";
            else specifier = (species == XmodsEnums.Species.Human ? "y" : "a");
            if (species != XmodsEnums.Species.Human) specifier +=
                (age == XmodsEnums.Age.Child && species == XmodsEnums.Species.LittleDog) ? "d" :
                (species == XmodsEnums.Species.Fox)?"x":
                species.ToString().Substring(0, 1).ToLower();
            else if (age <= XmodsEnums.Age.Child || (age & XmodsEnums.Age.Infant) > 0) specifier += "u";
            else specifier += gender.ToString().Substring(0, 1).ToLower();
            if (subType == XmodsEnums.BodySubType.TailRing)
            {
                myTail.Geometry = SimMesh(new GEOM(new BinaryReader(new MemoryStream((byte[])rm.GetObject(specifier + "TailRing_lod" + lod.ToString())))));
            }
            else if (subType == XmodsEnums.BodySubType.TailScrew)
            {
                myTail.Geometry = SimMesh(new GEOM(new BinaryReader(new MemoryStream((byte[])rm.GetObject(specifier + "TailScrew_lod" + lod.ToString())))));
            }
            else if (subType == XmodsEnums.BodySubType.TailStub)
            {
                myTail.Geometry = SimMesh(new GEOM(new BinaryReader(new MemoryStream((byte[])rm.GetObject(specifier + "TailStub_lod" + lod.ToString())))));
            }
            else
            {
                myTail.Geometry = SimMesh(new GEOM(new BinaryReader(new MemoryStream((byte[])rm.GetObject(specifier + "Tail_lod" + lod.ToString())))));
            }
        }

        private void GetPartMaterials(XmodsEnums.Species species, XmodsEnums.Age age, XmodsEnums.Gender gender)
        {
            if (species == XmodsEnums.Species.Human)
            {
                if ((age & XmodsEnums.Age.TeenToElder) > 0)
                {
                    if (gender == XmodsEnums.Gender.Female)
                    {
                        mySkin = new DiffuseMaterial(GetImageBrush(Properties.Resources.FemaleSkin));
                        myUnderwear = new DiffuseMaterial(GetImageBrush(Properties.Resources.FemaleUnderwear));
                    }
                    else
                    {
                        mySkin = new DiffuseMaterial(GetImageBrush(Properties.Resources.MaleSkin));
                        myUnderwear = new DiffuseMaterial(GetImageBrush(Properties.Resources.MaleUnderwear));
                    }
                }
                else if ((age & XmodsEnums.Age.Child) > 0)
                {
                    mySkin = new DiffuseMaterial(GetImageBrush(Properties.Resources.ChildSkin));
                    myUnderwear = new DiffuseMaterial(GetImageBrush(Properties.Resources.ChildUnderwear));
                }
                else        //toddlers and infants
                {
                    mySkin = new DiffuseMaterial(GetImageBrush(Properties.Resources.ToddlerSkin));
                    myUnderwear = new DiffuseMaterial(GetImageBrush(Properties.Resources.ChildUnderwear));
                }
            }
            else if (species == XmodsEnums.Species.Cat)
            {
                mySkin = new DiffuseMaterial(GetImageBrush(Properties.Resources.CatSkin));
            }
            else if(species == XmodsEnums.Species.Dog)
            {
                mySkin = new DiffuseMaterial(GetImageBrush(Properties.Resources.DogSkin));
            }
            else if(species == XmodsEnums.Species.Fox)
            {
                mySkin = new DiffuseMaterial(GetImageBrush(Properties.Resources.FoxSkin));
                
            }
            else if(species == XmodsEnums.Species.Horse)
            {
                mySkin = new DiffuseMaterial(GetImageBrush(Properties.Resources.HorseSkin));
                
            }

        }

        public void Stop_Mesh()
        {
            modelGroup.Children.Clear();
        }

        private void sliderXRot_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            rot_x.Angle = sliderXRot.Value;
        }
        private void sliderYRot_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            rot_y.Angle = sliderYRot.Value;
        }

        private void sliderZoom_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Camera1.Position = new Point3D(Camera1.Position.X, Camera1.Position.Y, -sliderZoom.Value);
        }

        private void sliderYMove_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Camera1.Position = new Point3D(Camera1.Position.X, -sliderYMove.Value, Camera1.Position.Z);
        }

        private void sliderXMove_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Camera1.Position = new Point3D(-sliderXMove.Value, Camera1.Position.Y, Camera1.Position.Z);
        }

        public void SetRegionMesh(int index, bool showMesh)
        {
            if (showMesh & !modelGroup.Children.Contains(myMeshes[index]))
            {
                modelGroup.Children.Add(myMeshes[index]);
            }
            else if (!showMesh & modelGroup.Children.Contains(myMeshes[index]))
            {
                modelGroup.Children.Remove(myMeshes[index]);
            }
        }

        public void SetHeadDisplay(bool showHead)
        {
            SetPartDisplay(showHead, myHead);
        }

        public void SetTopDisplay(bool showTop)
        {
            SetPartDisplay(showTop, myTop);
        }

        public void SetBottomDisplay(bool showBottom)
        {
            SetPartDisplay(showBottom, myBottom);
        }

        public void SetFeetDisplay(bool showFeet)
        {
            SetPartDisplay(showFeet, myFeet);
        }

        public void SetBodyDisplay(bool showBody)
        {
            SetPartDisplay(showBody, myBody);
        }

        public void SetEarsDisplay(bool showEars, XmodsEnums.BodySubType subType)
        {
            SetEarsGeometry(currentLod, currentSpecies, currentAge, currentGender, subType);
            SetPartDisplay(showEars, myEars);
        }

        public void SetTailDisplay(bool showTail, XmodsEnums.BodySubType subType)
        {
            SetTailGeometry(currentLod, currentSpecies, currentAge, currentGender, subType);
            SetPartDisplay(showTail, myTail);
        }

        public void SetPartDisplay(bool showPart, GeometryModel3D part)
        {
            if (showPart && !modelGroup.Children.Contains(part))
            {
                if (modelGroup.Children.Count > 1)
                {
                    modelGroup.Children.Insert(modelGroup.Children.Count - 2, part);
                }
                else if (modelGroup.Children.Count > 0)
                {
                    modelGroup.Children.Insert(modelGroup.Children.Count - 1, part);
                }
                else
                {
                    modelGroup.Children.Add(part);
                }
            }
            else if (!showPart && modelGroup.Children.Contains(part))
            {
                modelGroup.Children.Remove(part);
            }
        }

        public void SetSkinColor(System.Drawing.Color color)
        {
            myMaterials.Children.Remove(mySkinColor);
            mySkinColor = new DiffuseMaterial(new SolidColorBrush(Color.FromArgb(color.A, color.R, color.G, color.B)));
            myMaterials.Children.Insert(0, mySkinColor);
        }

        public void SetSkinOverlay(bool showOverlay)
        {
            if (showOverlay && !myMaterials.Children.Contains(mySkin))
            {
                myMaterials.Children.Insert(1, mySkin);
            }
            else if (!showOverlay && myMaterials.Children.Contains(mySkin))
            {
                myMaterials.Children.Remove(mySkin);
            }
        }

        public void SetUndies(bool showUndies)
        {
            if (showUndies && !myMaterials.Children.Contains(myUnderwear))
            {
                int ind = 1;
                if (myMaterials.Children.Contains(mySkin)) ind++;
                myMaterials.Children.Insert(ind, myUnderwear);
            }
            else if (!showUndies && myMaterials.Children.Contains(myUnderwear))
            {
                myMaterials.Children.Remove(myUnderwear);
            }
        }

        public void SetDiffuse(bool showDiffuse)
        {
            if (showDiffuse && !myMaterials.Children.Contains(myDiffuse))
            {
                myMaterials.Children.Add(myDiffuse);
            }
            else if (!showDiffuse && myMaterials.Children.Contains(myDiffuse))
            {
                myMaterials.Children.Remove(myDiffuse);
            }
            if (showDiffuse && !glassMaterials.Children.Contains(myDiffuse))
            {
                glassMaterials.Children.Add(myDiffuse);
            }
            else if (!showDiffuse && glassMaterials.Children.Contains(myDiffuse))
            {
                glassMaterials.Children.Remove(myDiffuse);
            }
            if (showDiffuse && !wingMaterials.Children.Contains(myWingDiffuse))
            {
                wingMaterials.Children.Add(myWingDiffuse);
            }
            else if (!showDiffuse && wingMaterials.Children.Contains(myWingDiffuse))
            {
                wingMaterials.Children.Remove(myWingDiffuse);
            }
        }

        public void SetSpecular(bool showSpecular)
        {
            if (showSpecular && !myMaterials.Children.Contains(mySpecular))
            {
                myMaterials.Children.Add(mySpecular);
            }
            else if (!showSpecular && myMaterials.Children.Contains(mySpecular))
            {
                myMaterials.Children.Remove(mySpecular);
            }
            if (showSpecular && !glassMaterials.Children.Contains(mySpecular))
            {
                glassMaterials.Children.Add(mySpecular);
            }
            else if (!showSpecular && glassMaterials.Children.Contains(mySpecular))
            {
                glassMaterials.Children.Remove(mySpecular);
            }
            if (showSpecular && !wingMaterials.Children.Contains(myWingSpecular))
            {
                wingMaterials.Children.Add(myWingSpecular);
            }
            else if (!showSpecular && glassMaterials.Children.Contains(mySpecular))
            {
                wingMaterials.Children.Remove(myWingSpecular);
            }
        }

        public void SetGlowmap(bool showGlowmap)
        {
            if (showGlowmap && !myMaterials.Children.Contains(myGlowmap))
            {
                myMaterials.Children.Add(myGlowmap);
            }
            else if (!showGlowmap && myMaterials.Children.Contains(myGlowmap))
            {
                myMaterials.Children.Remove(myGlowmap);
            }
            if (showGlowmap && !glassMaterials.Children.Contains(myGlowmap))
            {
                glassMaterials.Children.Add(myGlowmap);
            }
            else if (!showGlowmap && glassMaterials.Children.Contains(myGlowmap))
            {
                glassMaterials.Children.Remove(myGlowmap);
            }
        }

        public void SetShadow(bool showShadow)
        {
            if (showShadow && !myMaterials.Children.Contains(myShadow))
            {
                SetBumpmap(false);
                int ind = myMaterials.Children.IndexOf(myDiffuse);
                if (ind > -1)
                {
                    myMaterials.Children.Insert(ind, myShadow);
                }
                else
                {
                    myMaterials.Children.Add(myShadow);
                }
            }
            else if (!showShadow && myMaterials.Children.Contains(myShadow))
            {
                myMaterials.Children.Remove(myShadow);
            }
        }

        public void SetBumpmap(bool showBumpmap)
        {
            if (showBumpmap && !myMaterials.Children.Contains(myBumpmap))
            {
                myMaterials.Children.Add(myBumpmap);
            }
            else if (!showBumpmap && myMaterials.Children.Contains(myBumpmap))
            {
                myMaterials.Children.Remove(myBumpmap);
            }
            if (showBumpmap && !glassMaterials.Children.Contains(myBumpmap))
            {
                glassMaterials.Children.Add(myBumpmap);
            }
            else if (!showBumpmap && glassMaterials.Children.Contains(myBumpmap))
            {
                glassMaterials.Children.Remove(myBumpmap);
            }
        }

        public void SetLights(bool light1On, bool light2On)
        {
            if (light1On && !modelGroup.Children.Contains(DirLight1))
            {
                modelGroup.Children.Insert(0, DirLight1);
            }
            else if (!light1On && modelGroup.Children.Contains(DirLight1))
            {
                modelGroup.Children.Remove(DirLight1);
            }
            if (light2On && !modelGroup.Children.Contains(DirLight2))
            {
                modelGroup.Children.Insert(0, DirLight2);
            }
            else if (!light2On && modelGroup.Children.Contains(DirLight2))
            {
                modelGroup.Children.Remove(DirLight2);
            }
        }

        public void SetLOD(GEOM[] simgeoms, int lod)
        {
            currentLod = lod;
            GetPartGeometries(lod, currentSpecies, currentAge, currentGender);
           // MeshGeometry3D[] simMesh = SimMeshes(simgeoms);
           // skinMesh.Geometry = simMesh[0];
           // glassMesh.Geometry = simMesh[1];

            for (int i = 0; i < myMeshes.Length; i++)
            {
                modelGroup.Children.Remove(myMeshes[i]);
            }

            MeshGeometry3D[] myGeoms;
            SimMeshes(simgeoms, out myGeoms, out isGlass, out isWings);
            myMeshes = new GeometryModel3D[myGeoms.Length];
            for (int i = 0; i < myMeshes.Length; i++)
            {
                myMeshes[i] = new GeometryModel3D();
                myMeshes[i].Geometry = myGeoms[i];
                myMeshes[i].Transform = modelTransform;
                if (isGlass[i])
                {
                    myMeshes[i].Material = glassMaterials;
                }
                else if (isWings[i])
                {
                    myMeshes[i].Material = wingMaterials;
                }
                else
                {
                    myMeshes[i].Material = myMaterials;
                }
                modelGroup.Children.Add(myMeshes[i]);
            }
        }

        public void SetGender(XmodsEnums.Gender gender)
        {
            currentGender = gender;
            GetPartGeometries(currentLod, currentSpecies, currentAge, gender);
            bool hasSkin = false, hasUnderwear = false;
            if (myMaterials.Children.Contains(myUnderwear))
            {
                myMaterials.Children.Remove(myUnderwear);
                hasUnderwear = true;
            }
            if (myMaterials.Children.Contains(mySkin))
            {
                myMaterials.Children.Remove(mySkin);
                hasSkin = true;
            }
            GetPartMaterials(currentSpecies, currentAge, gender);
            if (hasUnderwear)
            {
                myMaterials.Children.Insert(1, myUnderwear);
            }
            if (hasSkin)
            {
                myMaterials.Children.Insert(1, mySkin);
            }
        }

        public void SetMorph(GEOM[] simgeoms, MorphMap morphShape, MorphMap morphNormals)
        {
            GetPartGeometries(currentLod, currentSpecies, currentAge, currentGender, morphShape, morphNormals);

            for (int i = 0; i < myMeshes.Length; i++)
            {
                modelGroup.Children.Remove(myMeshes[i]);
            }

            MeshGeometry3D[] myGeoms;
            SimMeshes(simgeoms, morphShape, morphNormals, out myGeoms, out isGlass);
            myMeshes = new GeometryModel3D[myGeoms.Length];
            for (int i = 0; i < myMeshes.Length; i++)
            {
                myMeshes[i] = new GeometryModel3D();
                myMeshes[i].Geometry = myGeoms[i];
                myMeshes[i].Transform = modelTransform;
                if (isGlass[i])
                {
                    myMeshes[i].Material = glassMaterials;
                }
                else if (isWings[i])
                {
                    myMeshes[i].Material = wingMaterials;
                }
                else
                {
                    myMeshes[i].Material = myMaterials;
                }
                modelGroup.Children.Add(myMeshes[i]);
            }
        }
    }
}
