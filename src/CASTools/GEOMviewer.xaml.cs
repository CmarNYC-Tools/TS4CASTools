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
    /// Interaction logic for GEOMviewer.xaml
    /// </summary>
    public partial class GEOMviewer : UserControl
    {
        AxisAngleRotation3D rot_x;
        AxisAngleRotation3D rot_y;
        ScaleTransform3D zoom = new ScaleTransform3D(1, 1, 1);
        Transform3DGroup modelTransform, cameraTransform;
        DirectionalLight DirLight1 = new DirectionalLight();
        DirectionalLight DirLight2 = new DirectionalLight();
        PerspectiveCamera Camera1 = new PerspectiveCamera();
        Model3DGroup modelGroup = new Model3DGroup();
        Viewport3D myViewport = new Viewport3D();
        GeometryModel3D myMesh = null;
        MaterialGroup myMaterial = new MaterialGroup();

        public GEOMviewer()
        {
            InitializeComponent();
            rot_x = new AxisAngleRotation3D(new Vector3D(1, 0, 0), 0);
            rot_y = new AxisAngleRotation3D( new Vector3D(0, 1, 0), 0);

            cameraTransform = new Transform3DGroup();
            cameraTransform.Children.Add(zoom);
            modelTransform = new Transform3DGroup();
            modelTransform.Children.Add(new RotateTransform3D(rot_y));
            modelTransform.Children.Add(new RotateTransform3D(rot_x));

            DirLight1.Color = Colors.White;
            DirLight1.Direction = new Vector3D(.5, -.5, -1);

            Camera1.FarPlaneDistance = 20;
            Camera1.NearPlaneDistance = 0.05;
            Camera1.FieldOfView = 45;
            Camera1.Position = new Point3D(0, 0, 2.8);
            Camera1.LookDirection = new Vector3D(0, 0, -3);
            Camera1.UpDirection = new Vector3D(0, 1, 0);
            ModelVisual3D modelsVisual = new ModelVisual3D();
            modelsVisual.Content = modelGroup;

            myViewport.Camera = Camera1;
            myViewport.Children.Add(modelsVisual);
            myViewport.Height = 537;
            myViewport.Width = 530;
            myViewport.Camera.Transform = cameraTransform;
            this.canvas1.Children.Insert(0, myViewport);

            Canvas.SetTop(myViewport, 0);
            Canvas.SetLeft(myViewport, 0);
            this.Width = myViewport.Width;
            this.Height = myViewport.Height;
        }

        MeshGeometry3D SimMesh(GEOM simgeom)
        {
            MeshGeometry3D mesh = new MeshGeometry3D();
            Point3DCollection verts = new Point3DCollection();
            Vector3DCollection normals = new Vector3DCollection();
            PointCollection uvs = new PointCollection();
            Int32Collection facepoints = new Int32Collection();
            int indexOffset = 0;
            float centerOffset = 0.80f;

            GEOM g = simgeom;

            for (int i = 0; i < g.numberVertices; i++)
            {
                float[] pos = g.getPosition(i);
                verts.Add(new Point3D(pos[0], pos[1] - centerOffset, pos[2]));
                float[] norm = g.getNormal(i);
                normals.Add(new Vector3D(norm[0], norm[1], norm[2]));
                float[] uv = g.getUV(i, 0);
                uvs.Add(new Point(uv[0], uv[1]));
            }

            for (int i = 0; i < g.numberFaces; i++)
            {
                int[] face = g.getFaceIndices(i);
                facepoints.Add(face[0] + indexOffset);
                facepoints.Add(face[1] + indexOffset);
                facepoints.Add(face[2] + indexOffset);
            }

            indexOffset += g.numberVertices;

            mesh.Positions = verts;
            mesh.TriangleIndices = facepoints;
            mesh.Normals = normals;
            mesh.TextureCoordinates = uvs;
            return mesh;
        }

        public void Start_Mesh(GEOM simgeom)
        {
            MeshGeometry3D myGeom = SimMesh(simgeom);
            myMaterial.Children.Clear();
            myMaterial.Children.Add(new DiffuseMaterial(new SolidColorBrush(Colors.LightGray)));
            myMesh = new GeometryModel3D(myGeom, myMaterial);
            myMesh.Transform = modelTransform;
            modelGroup.Children.Clear();
            modelGroup.Children.Add(DirLight1);
            modelGroup.Children.Add(myMesh);
        }

        public void Stop_Mesh()
        {
            modelGroup.Children.Clear();
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

        private void sliderYRot_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            rot_y.Angle = sliderYRot.Value;
        }

        private void sliderXRot_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            rot_x.Angle = sliderXRot.Value;
        }
    }
}
