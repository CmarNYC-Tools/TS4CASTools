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
using System.Windows.Forms;

// Adapted from article "A Vector Type for C#" by R. Potter on codeproject.com

namespace Xmods.DataLib
{
    public struct Vector3 : IEquatable<Vector3>
    {
        private float x, y, z;

        public float X
        {
            get { return x; }
            set { x = value; }
        }

        public float Y
        {
            get { return y; }
            set { y = value; }
        }

        public float Z
        {
            get { return z; }
            set { z = value; }
        }

        public float[] Coordinates
        {
            get { return new float[] { x, y, z }; }
            set
            {
                x = value[0];
                y = value[1];
                z = value[2];
            }
        }

        public float Magnitude
        {
            get 
            {
                double tmp = (x * x) + (y * y) + (z * z);
                return (float) Math.Sqrt(tmp); 
            }
        }

        public Vector3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public Vector3(float[] coordinates)
        {
            this.x = coordinates[0];
            this.y = coordinates[1];
            this.z = coordinates[2];
        }

        public Vector3(Vector3 vector)
        {
            this.x = vector.X;
            this.y = vector.Y;
            this.z = vector.Z;
        }

        public static Vector3 operator +(Vector3 v1, Vector3 v2)
        {
            return
            (
               new Vector3
               (
                  v1.X + v2.X,
                  v1.Y + v2.Y,
                  v1.Z + v2.Z
               )
            );
        }

        public static Vector3 operator -(Vector3 v1, Vector3 v2)
        {
            return
            (
               new Vector3
               (
                   v1.X - v2.X,
                   v1.Y - v2.Y,
                   v1.Z - v2.Z
               )
            );
        }

        public static bool operator ==(Vector3 v1, Vector3 v2)
        {
            return
            (
               (AlmostEquals(v1.X, v2.X)) &&
               (AlmostEquals(v1.Y, v2.Y)) &&
               (AlmostEquals(v1.Z, v2.Z))
            );
        }

        public override bool Equals(object obj)
        {
            // Check object other is a Vector3 object
            if (obj is Vector3)
            {
                // Convert object to Vector3
                Vector3 otherVector = (Vector3)obj;

                // Check for equality
                return otherVector == this;
            }
            else
            {
                return false;
            }
        }

        public bool Equals(Vector3 obj)
        {
            return obj == this;
        }

        public bool CloseTo(Vector3 other, float maxSeparation)
        {
            float distance = this.Distance(other);
            return distance <= maxSeparation;
        }

        public bool isZero()
        {
            const float Epsilon = 1e-8f;
            return Math.Abs(this.x) < Epsilon && Math.Abs(this.y) < Epsilon && Math.Abs(this.z) < Epsilon;
        }

        public static bool operator !=(Vector3 v1, Vector3 v2)
        {
            return !(v1 == v2);
        }

        public static Vector3 operator *(Vector3 v1, float s2)
        {
            return
            (
               new Vector3
               (
                  v1.X * s2,
                  v1.Y * s2,
                  v1.Z * s2
               )
            );
        }

        public static Vector3 operator *(float s1, Vector3 v2)
        {
            return v2 * s1;
        }

        public static Vector3 operator /(Vector3 v1, float s2)
        {
            return
            (
               new Vector3
               (
                  v1.X / s2,
                  v1.Y / s2,
                  v1.Z / s2
               )
            );
        }

        public static Vector3 Cross(Vector3 v1, Vector3 v2)
        {
            return
            (
               new Vector3
               (
                  v1.Y * v2.Z - v1.Z * v2.Y,
                  v1.Z * v2.X - v1.X * v2.Z,
                  v1.X * v2.Y - v1.Y * v2.X
               )
            );
        }

        public Vector3 Cross(Vector3 other)
        {
            return Cross(this, other);
        }

        public static float Dot(Vector3 v1, Vector3 v2)
        {
            return
            (
               v1.X * v2.X +
               v1.Y * v2.Y +
               v1.Z * v2.Z
            );
        }

        public float Dot(Vector3 other)
        {
            return Dot(this, other);
        }

        public static Vector3 Normalize(Vector3 v1)
        {
            // Check for divide by zero errors
            if (v1.Magnitude == 0)
            {
                throw new DivideByZeroException("Cannot normalize a vector with magnitude of zero!");
            }
            else
            {
                // find the inverse of the vector's magnitude
                float inverse = 1 / v1.Magnitude;
                return
                (
                   new Vector3
                   (
                    // multiply each component by the inverse of the magnitude
                      v1.X * inverse,
                      v1.Y * inverse,
                      v1.Z * inverse
                   )
                );
            }
        }

        public void Normalize()
        {
            Vector3 n = Normalize(this);
            this.x = n.x;
            this.y = n.y;
            this.z = n.z;
        }

        public Vector3 VertexNormal(Triangle[] Faces)
        {
            Vector3 vertNormal = new Vector3(0f, 0f, 0f);
            for (int i = 0; i < Faces.Length; i++)
            {
                vertNormal += Faces[i].Normal();
            }
            return Normalize(vertNormal);
        }

        public static float Distance(Vector3 v1, Vector3 v2)
        {
            return
            (
               (float) Math.Sqrt
               (
                   (v1.X - v2.X) * (v1.X - v2.X) +
                   (v1.Y - v2.Y) * (v1.Y - v2.Y) +
                   (v1.Z - v2.Z) * (v1.Z - v2.Z)
               )
            );
        }

        public float Distance(Vector3 other)
        {
            return Distance(this, other);
        }

        public static float Angle(Vector3 v1, Vector3 v2)
        {
            return
            (
               (float) Math.Acos
               (
                  Normalize(v1).Dot(Normalize(v2))
               )
            );
        }

        public float Angle(Vector3 other)
        {
            return Angle(this, other);
        }

        public static Vector3 Centroid(Vector3 P1, Vector3 P2, Vector3 P3)
        {
            return new Vector3((P1.x + P2.x + P3.x) / 3f, (P1.y + P2.y + P3.y) / 3f, (P1.z + P2.z + P3.z) / 3f);
        }

        public Vector3 ProjectToPlane(Plane p)
        {
            float t = ((p.A + this.X) + (p.B * this.Y) + (p.C * this.Z) + p.D) / 
                      ((p.A * p.A) + (p.B * p.B) + (p.C * p.C));
            return new Vector3(this.X + (p.A * t), this.Y + (p.B * t), this.Z + (p.C * t));
        }

        public Vector3 ProjectToLine(Vector3 Point1, Vector3 Point2)
        {
            Vector3 tmp = Point2 - Point1;
            tmp.Normalize();
            Vector3 tmp2 = this - Point1;
            Vector3 tmp3 = Vector3.Dot(tmp, tmp2) * tmp;
            return new Vector3(Point1 + tmp3);
        }

        public bool Between(Vector3 Point1, Vector3 Point2)
        {
            float min = Math.Min(Point1.X, Point2.X);
            float max = Math.Max(Point1.X, Point2.X);
            if (min > this.X | this.X > max)
            {
                return false;
            }
            min = Math.Min(Point1.Y, Point2.Y);
            max = Math.Max(Point1.Y, Point2.Y);
            if (min > this.Y | this.Y > max)
            {
                return false;
            }
            min = Math.Min(Point1.Z, Point2.Z);
            max = Math.Max(Point1.Z, Point2.Z);
            if (min > this.Z | this.Z > max)
            {
                return false;
            }
            return true;
        }

        public float[] GetInterpolationWeights(Vector3[] points, float weightingFactor)
        {
            float[] weights = new float[points.Length];

            if (points.Length == 1)
            {
                weights[0] = 1f;
                return weights;
            }
            for (int i = 0; i < points.Length; i++)
            {
                if (Vector3.Distance(points[i], this) == 0f)
                {
                    weights[i] = 1f;
                    return weights;
                }
            }

            float[] d = new float[points.Length];
            float dt = 0;
            for (int i = 0; i < points.Length; i++)
            {
                d[i] = 1f / (float)Math.Pow(Vector3.Distance(points[i], this), weightingFactor);
                dt += d[i];
            }

            for (int i = 0; i < points.Length; i++)
            {
                weights[i] = d[i] / dt;
            }
            //string a = "Distance: ";
            //string b = "Weights: ";
            //string x = "Positions: ";
            //for (int i = 0; i < weights.Length; i++)
            //{
            //    a += d[i].ToString() + ", ";
            //    b += weights[i].ToString() + ", ";
            //    x += points[i].x.ToString() + "," + points[i].y.ToString() + "," + points[i].z.ToString() + ", ";
            //}
            //MessageBox.Show(a + System.Environment.NewLine + b + System.Environment.NewLine + x);
            return weights;
        }

        public int[] GetReferenceMeshPoints(Vector3[] refMeshPositions, int[][] refMeshFaces, bool interpolate, bool restrictToNearestFace)
        {
            return GetReferenceMeshPoints(refMeshPositions, refMeshFaces, interpolate, restrictToNearestFace, 3);
        }

        public int[] GetReferenceMeshPoints(Vector3[] refMeshPositions, int[][] refMeshFaces, bool interpolate, bool restrictToNearestFace, int numberOfPoints)
        {
                                    //number of points only valid if not restricted to nearest face
            if (restrictToNearestFace)
            {
                float distance = 99999999f;
                List<int> ind = new List<int>(1);
                for (int j = 0; j < refMeshPositions.Length; j++)
                {
                    //Vector3 posref = new Vector3(refMesh.getPosition(j));
                    float tmp = this.Distance(refMeshPositions[j]);
                    if (tmp < distance)
                    {
                        distance = tmp;
                        ind.Clear();
                        ind.Add(j);
                    }
                    else if (tmp == distance)
                    {
                        ind.Add(j);
                    }
                }
                if (ind.Count == 0) throw new ApplicationException("No vertices in reference mesh!");
                if (interpolate)
                {
                    Triangle refTri = new Triangle();
                    float faceDistance = 99999999f;
                    bool found = false;
                    int refFace = 0;
                    foreach (int i in ind)
                    {
                        for (int j = 0; j < refMeshFaces.GetLength(0); j++)
                        {
                            bool foundtmp = false;
                            for (int k = 0; k < 3; k++)
                            {
                                if (refMeshFaces[j][k] == i)
                                {
                                    foundtmp = true;
                                    continue;
                                }
                            }
                            if (foundtmp)
                            {
                                Triangle tmp = new Triangle(refMeshPositions[refMeshFaces[j][0]],
                                                            refMeshPositions[refMeshFaces[j][1]],
                                                            refMeshPositions[refMeshFaces[j][2]]);
                                Plane tmpP = new Plane(tmp);
                                if (tmp.PointInside(this.ProjectToPlane(tmpP)) &
                                   (this.Distance(this.ProjectToPlane(tmpP)) < faceDistance))
                                {
                                    found = true;
                                    refTri = tmp;
                                    refFace = j;
                                    faceDistance = this.Distance(this.ProjectToPlane(tmpP));
                                }
                            }
                        }
                    }

                    if (found)
                    {
                        return refMeshFaces[refFace];
                    }
                    else
                    {
                        int ind1 = ind[0];
                        int ind2 = ind[0];
                        foreach (int i in ind)
                        {
                            Vector3 refVert = new Vector3(refMeshPositions[i]);
                            float linedistance = 99999999f;
                            for (int j = 0; j < refMeshFaces.GetLength(0); j++)
                            {
                                for (int k = 0; k < 3; k++)
                                {
                                    if (refMeshFaces[j][k] == i)
                                    {
                                        for (int k2 = 0; k2 < 3; k2++)
                                        {
                                            if (k != k2)
                                            {
                                                Vector3 tmp = new Vector3(refMeshPositions[refMeshFaces[j][k2]]);
                                                try
                                                {
                                                    if (this.ProjectToLine(refVert, tmp).Between(refVert, tmp) &
                                                        this.Distance(this.ProjectToLine(refVert, tmp)) < linedistance)
                                                    {
                                                        ind1 = i;
                                                        ind2 = refMeshFaces[j][k2];
                                                        linedistance = this.Distance(this.ProjectToLine(refVert, tmp));
                                                    }
                                                }
                                                catch
                                                {
                                                    continue;
                                                }
                                            }
                                        }
                                    }
                                    continue;
                                }
                            }
                        }
                        if (ind1 != ind2)
                        {
                            return new int[] { ind1, ind2 };
                        }
                        else
                        {
                            return new int[] { ind[0] };
                        }
                    }
                }
                else
                {
                    return new int[] { ind[0] };
                }

            }

            else
            {
                int[] ind = new int[numberOfPoints];
                float[] distance = new float[numberOfPoints];
                Vector3[] position = new Vector3[numberOfPoints];
                for (int i = 0; i < numberOfPoints; i++)
                {
                    distance[i] = 999999f;
                }
                for (int j = 0; j < refMeshPositions.Length; j++)
                {
                    if (this == refMeshPositions[j]) return new int[] { j };    //if exact match
                    float tmp = this.Distance(refMeshPositions[j]);
                    for (int i = 0; i < numberOfPoints; i++)
                    {
                        if (tmp <= distance[i] && position[i] != refMeshPositions[j])
                        {
                            for (int k = numberOfPoints-2; k >= i; k--)
                            {
                                ind[k + 1] = ind[k];
                                distance[k + 1] = distance[k];
                                position[k + 1] = position[k];
                            }
                            ind[i] = j;
                            distance[i] = tmp;
                            position[i] = refMeshPositions[j];
                            break;
                        }
                    }
                }
                //string temp = "";
                //foreach (int i in ind) temp += i.ToString() + ", ";
                //MessageBox.Show(temp);
                return ind;
            }
        }

        public int NearestTriangleIndex(Triangle[] triangleArray)
        {
            float tmpLen = float.MaxValue;
            int ind = 0;
            for (int i = 0; i < triangleArray.Length; i++)
            {
                if (this.Distance(triangleArray[i].Centroid()) < tmpLen)
                {
                    ind = i;
                    tmpLen = this.Distance(triangleArray[i].Centroid());
                }
            }
            return ind;
        }

        public int NearestPointIndexSimple(Vector3[] RefPointsArray)
        {
            float minDistance = float.MaxValue;
            int ind = 0;
            for (int i = 0; i < RefPointsArray.Length; i++)
            {
                if (this.Distance(RefPointsArray[i]) < minDistance)
                {
                    minDistance = this.Distance(RefPointsArray[i]);
                    ind = i;
                }
            }
            return ind;
        }

        public Vector3 NearestPointSimple(Vector3[] RefPointsArray)
        {
            int ind = this.NearestPointIndexSimple(RefPointsArray);
            return RefPointsArray[ind];
        }

        public int NearestPointIndex(Vector3 thisFacesCentroid, Vector3[] RefPointsArray, Vector3[] refPointsFacesCentroids)
        {
            float minDistance = float.MaxValue;
            List<int> workingIndexes = new List<int>();
            for (int i = 0; i < RefPointsArray.Length; i++)
            {
                if (this.Distance(RefPointsArray[i]) < minDistance)
                {
                    workingIndexes.Clear();
                    workingIndexes.Add(i);
                    minDistance = this.Distance(RefPointsArray[i]);
                }
                else if (this.Distance(RefPointsArray[i]) == minDistance)
                {
                    workingIndexes.Add(i);
                }
            }
            float minFaceDistance = float.MaxValue;
            int ind = 0;
            for (int i = 0; i < workingIndexes.Count; i++)
            {
                if (thisFacesCentroid.Distance(refPointsFacesCentroids[workingIndexes[i]]) < minFaceDistance)
                {
                    ind = workingIndexes[i];
                    minFaceDistance = thisFacesCentroid.Distance(refPointsFacesCentroids[workingIndexes[i]]);
                }
            }
            return ind;
        }

        public Vector3 NearestPoint(Vector3 thisFacesCentroid, Vector3[] RefPointsArray, Vector3[] refPointsFacesCentroids)
        {
            int ind = this.NearestPointIndex(thisFacesCentroid, RefPointsArray, refPointsFacesCentroids);
            return RefPointsArray[ind];
        }

        public int[] GetFaceReferenceMeshPoints(Vector3[] refMeshPositions, int[][] refMeshFaces, int[][] refFaceRefs, Triangle[] currentVertFaces, int numberOfPoints)
        {
            int[] ind = new int[numberOfPoints];
            for (int i = 0; i < numberOfPoints; i++) { ind[i] = -1; }
            float[] distance = new float[numberOfPoints];

            float[] refMeshDistances = new float[refMeshPositions.Length];
            for (int j = 0; j < refMeshPositions.Length; j++)
            {
                refMeshDistances[j] = this.Distance(refMeshPositions[j]);
            }
            int closestVert = ArrayMinimumIndex(refMeshDistances);      //closest vertex
            float closestDistance = refMeshDistances[closestVert];

                //code to handle seams - 2 or more verts in the same position with different UVs
            List<int> dupVerts = new List<int>();
            for (int j = 0; j < refMeshPositions.Length; j++)           //search for all verts in same position
            {
                if (refMeshPositions[j] == refMeshPositions[closestVert]) dupVerts.Add(j);
            }
            if (dupVerts.Count > 1)       //if more than one vert in that position, determine which is linked to ref faces closest to the face we're working on
            {
                Vector3 myCentroid = new Vector3();
                foreach (Triangle tri in currentVertFaces)
                {
                    myCentroid += tri.Centroid();
                }
                myCentroid = myCentroid / (float)currentVertFaces.Length;       //get average centroid

                Vector3[] refCentroid = new Vector3[dupVerts.Count];            //average centroids for duplicate verts
                float[] centroidDistance = new float[dupVerts.Count];
                for (int i = 0; i < dupVerts.Count; i++)    //for each duplicate position vertex
                {
                    int[] faces = refFaceRefs[dupVerts[i]];  //refFaceRefs[index of vertex position][index of face the vertex is in]
                    for (int j = 0; j < faces.Length; j++)   //for each face
                    {
                        int[] face = refMeshFaces[faces[j]];
                        refCentroid[i] += Centroid(refMeshPositions[face[0]], refMeshPositions[face[1]],
                                                    refMeshPositions[face[2]]);
                    }
                    refCentroid[i] = refCentroid[i] / (float)refFaceRefs[dupVerts[i]].Length;
                    centroidDistance[i] = myCentroid.Distance(refCentroid[i]);
                }
                int tmp = ArrayMinimumIndex(centroidDistance);              //get closest fit
                closestVert = dupVerts[tmp];
            }

            ind[0] = closestVert;
            distance[0] = closestDistance;
            List<int> VertsLinkedByFaces = new List<int>();
            int[] linkedFaces = refFaceRefs[closestVert];
            foreach (int i in linkedFaces)
            {
                foreach (int j in refMeshFaces[i])
                {
                    if (!VertsLinkedByFaces.Contains(j)) VertsLinkedByFaces.Add(j);
                }
            }

            foreach (int j in VertsLinkedByFaces)
            {
                for (int i = 0; i < numberOfPoints; i++)
                {
                    if (refMeshDistances[j] <= distance[i])
                    {
                        for (int k = i; k < numberOfPoints - 1; k++)
                        {
                            ind[k + 1] = ind[k];
                            distance[k + 1] = distance[k];
                        }
                        ind[i] = j;
                        distance[i] = refMeshDistances[j];
                        continue;
                    }
                }
            }
            return ind;
        }

        internal int ArrayMinimumIndex(float[] array)
        {
            int tmp = -1;
            float tmpVal = float.MaxValue;
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] < tmpVal) tmp = i;
            }
            return tmp;
        }

        public override int GetHashCode()
        {
            return
            (
               (int)((X + Y + Z) % Int32.MaxValue)
            );
        }

        public override string ToString()
        {
            return this.X.ToString() + ", " + this.Y.ToString() + ", " + this.Z.ToString();
        }

        public string ToString(string format)
        {
            return this.X.ToString(format) + ", " + this.Y.ToString(format) + ", " + this.Z.ToString(format);
        }

        public static Vector3 Parse(string coordinateString)
        {
            string[] coordsStr = coordinateString.Split(new char[] { ',' });
            if (coordsStr.Length != 3) throw new FormatException("Input not in correct format: " + coordinateString);
            float[] coords = new float[3];
            for (int i = 0; i < 3; i++)
            {
                if (!float.TryParse(coordsStr[i], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out coords[i]))
                    throw new FormatException("Input not in correct format: " + coordinateString);
            }
            return new Vector3(coords);
        }

        internal static bool AlmostEquals(float f1, float f2)
        {
            const float EPSILON = 1e-4f;
            return (Math.Abs(f1 - f2) < EPSILON);
        }

        internal bool positionMatches(float[] other)
        {
            return this.positionMatches(new Vector3(other));
        }
        internal bool positionMatches(float x, float y, float z)
        {
            return this.positionMatches(new Vector3(x, y, z));
        }
        internal bool positionMatches(Vector3 other)
        {
            const float EPSILON = 1e-4f;
            if (Math.Abs(this.x - other.x) < EPSILON && Math.Abs(this.y - other.y) < EPSILON && Math.Abs(this.z - other.z) < EPSILON) return true;
            return false;
        }
    }
}
