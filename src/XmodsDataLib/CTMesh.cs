/* Xmods Data Library, a library to support tools for The Sims 4,
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
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Globalization;
using System.Linq;

namespace Xmods.DataLib
{
    internal class CTMESH
    {
        const int MAX_VERTICES = Int32.MaxValue;
        const int MAX_TRIANGLES = Int32.MaxValue;

        Vector3[] vertices;                             // positions
        Face[] faces;                                   // numTriangles times

        public int NumberFaces { get { return faces.Length; } }

        public CTMESH.Face[] FaceArray { get { return this.faces; } }

        public Face getFace(int faceIndex)
        {
            return faces[faceIndex];
        }

        internal FacePoint GetFacePointWithIndex(int index)
        {
            foreach (Face f in this.faces)
            {
                foreach (FacePoint fp in f.points)
                {
                    if (fp.Index == index) return fp;
                }
            }
            return null;
        }

        internal CTMESH(GEOM geom)
        {
            if (geom.numberVertices > MAX_VERTICES) throw new MeshException("This mesh has too many vertices and cannot be converted to a CAS Tools Mesh!");
            if (geom.numberFaces > MAX_TRIANGLES) throw new MeshException("This mesh has too many triangles and cannot be converted to a CAS Tools Mesh!");

            this.vertices = new Vector3[geom.numberVertices];
            for (int i = 0; i < geom.numberVertices; i++)
            {
                this.vertices[i] = new Vector3(geom.getPosition(i));
            }

            faces = new Face[geom.numberFaces];
            for (int i = 0; i < geom.numberFaces; i++)
            {
                int[] f = geom.getFaceIndices(i);                  //vertex indices
                FacePoint[] points = new FacePoint[3];

                for (int p = 0; p < 3; p++)                 //for each vertex in the face
                {
                    Vector3 pos = new Vector3(geom.getPosition(f[p]));
                    Vector3 norm = new Vector3(geom.getNormal(f[p]));
                    Vector2[] uv = new Vector2[geom.numberUVsets];
                    for (int u = 0; u < geom.numberUVsets; u++)
                    {
                        uv[u] = new Vector2(geom.getUV(f[p], u));
                    }
                    points[p] = new FacePoint(f[p], pos, norm, uv);
                }

                faces[i] = new Face(f, points);
            }
        }

        internal void AutoUV(CTMESH reference, int UVset, int numberOfPoints)
        {
            if (reference.faces[0].points[0].UV.Length <= UVset) throw new MeshException("Reference mesh does not have UV set " + UVset.ToString() + "!");
            for (int i = 0; i < this.NumberFaces; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    while (this.faces[i].points[j].UV.Length <= UVset)
                    {
                        Vector2[] newUVs = new Vector2[this.faces[i].points[j].UV.Length + 1];
                        Array.Copy(this.faces[i].points[j].UV, newUVs, this.faces[i].points[j].UV.Length);
                        this.faces[i].points[j].UV = newUVs;
                    }
                }
            }

            Triangle[] tr = new Triangle[reference.NumberFaces];
            for (int i = 0; i < reference.NumberFaces; i++ )
            {
                tr[i] = new Triangle(reference.faces[i].points[0].Position, reference.faces[i].points[1].Position, reference.faces[i].points[2].Position);
            }

            foreach (Face f in this.faces)
            {
                Triangle t = new Triangle(f.points[0].Position, f.points[1].Position, f.points[2].Position);
                Face nearFace = reference.faces[t.NearestTriangleIndex(tr)];

                for (int i = 0; i < 3; i++)
                {
                    Vector3 v = f.points[i].Position;
                    FacePoint nearPoint = nearFace.points[v.NearestPointIndexSimple(nearFace.Positions)];

                    List<int> faceLinkedVerts = new List<int>();
                    foreach (Face fr in reference.faces)
                    {
                        List<int> tmp = new List<int>(fr.vertexIndices);
                        if (tmp.Contains(nearPoint.Index)) faceLinkedVerts.AddRange(tmp);
                    }
                    faceLinkedVerts = faceLinkedVerts.Distinct().ToList();

                    int[] ind = new int[numberOfPoints];
                    float[] distance = new float[numberOfPoints];
                    Vector3[] position = new Vector3[numberOfPoints];
                    for (int ii = 0; ii < numberOfPoints; ii++)
                    {
                        distance[ii] = float.MaxValue;
                    }
                    foreach (int ii in faceLinkedVerts)
                    {
                        Vector3 pos = reference.vertices[ii];
                        if (v.positionMatches(pos))     //if position matches
                        {
                            ind = new int[] { ii };
                            position = new Vector3[] { pos };
                            break;
                        }
                        float tmp = v.Distance(pos);
                        for (int j = 0; j < numberOfPoints; j++)
                        {
                            if (tmp <= distance[j])
                            {
                                for (int k = numberOfPoints - 2; k >= j; k--)
                                {
                                    ind[k + 1] = ind[k];
                                    distance[k + 1] = distance[k];
                                    position[k + 1] = position[k];
                                }
                                ind[j] = ii;
                                distance[j] = tmp;
                                position[j] = pos;
                                break;
                            }
                        }
                    }
                    float[] weights = v.GetInterpolationWeights(position, 2f);

                    float newU = 0f, newV = 0f;
                    for (int j = 0; j < weights.Length; j++)
                    {
                        Vector2 linkedUV = (reference.GetFacePointWithIndex(ind[j])).UV[UVset];
                        newU += linkedUV.X * weights[j];
                        newV += linkedUV.Y * weights[j];
                    }

                    f.points[i].UV[UVset] = new Vector2(newU, newV);
                }
            }
        }

        internal class Face
        {
            internal int[] vertexIndices = new int[3];
            internal FacePoint[] points = new FacePoint[3];
            internal Vector3[] Positions { get { return new Vector3[] { this.points[0].Position, this.points[1].Position, this.points[2].Position }; } }

            internal Face(int[] VertexIndices, FacePoint[] facePoints)
            {
                vertexIndices = new int[] { VertexIndices[0], VertexIndices[1], VertexIndices[2] };
                points = new FacePoint[] { new FacePoint(facePoints[0]), new FacePoint(facePoints[1]), new FacePoint(facePoints[2]) };
            }
        }

        internal class FacePoint
        {
            internal int Index;
            internal Vector3 Position;
            internal Vector3 Normals;
            internal Vector2[] UV;
            internal FacePoint(int index, Vector3 position, Vector3 normals, Vector2[] uv)
            {
                this.Index = index;
                this.Position = new Vector3(position);
                this.Normals = new Vector3(normals);
                this.UV = new Vector2[uv.Length];
                for (int i = 0; i < uv.Length; i++)
                {
                    this.UV[i] = new Vector2(uv[i]);
                }
            }
            internal FacePoint(FacePoint other)
            {
                this.Index = other.Index;
                this.Position = new Vector3(other.Position);
                this.Normals = new Vector3(other.Normals);
                this.UV = new Vector2[other.UV.Length];
                for (int i = 0; i < other.UV.Length; i++)
                {
                    this.UV[i] = new Vector2(other.UV[i]);
                }
            }
        }

        [global::System.Serializable]
        public class MeshException : ApplicationException
        {
            public MeshException() { }
            public MeshException(string message) : base(message) { }
            public MeshException(string message, Exception inner) : base(message, inner) { }
            protected MeshException(
              System.Runtime.Serialization.SerializationInfo info,
              System.Runtime.Serialization.StreamingContext context)
                : base(info, context) { }
        }
    }
}
