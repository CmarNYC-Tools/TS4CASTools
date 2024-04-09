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

namespace Xmods.DataLib
{
    public class Triangle
    {
        private Vector3 p1;
        private Vector3 p2;
        private Vector3 p3;

        public Vector3 Point1
        {
            get { return p1; }
            set { p1 = value; }
        }
        public Vector3 Point2
        {
            get { return p2; }
            set { p2 = value; }
        }
        public Vector3 Point3
        {
            get { return p3; }
            set { p3 = value; }
        }

        public Vector3[] TrianglePoints
        {
            get { return new Vector3[] { this.p1, this.p2, this.p3 }; }
        }

        public Vector3 Centroid()
        {
            return Centroid(this);
        }

        public Triangle()
        {
            this.p1 = new Vector3();
            this.p2 = new Vector3();
            this.p3 = new Vector3();
        }

        public Triangle(Vector3 Pnt1, Vector3 Pnt2, Vector3 Pnt3)
        {
            this.p1 = Pnt1;
            this.p2 = Pnt2;
            this.p3 = Pnt3;
        }

        public Triangle(Vector3[] Points)
        {
            this.p1 = Points[0];
            this.p2 = Points[1];
            this.p3 = Points[2];
        }

        public Triangle(float[] Point1, float[] Point2, float[] Point3)
        {
            this.p1 = new Vector3(Point1);
            this.p2 = new Vector3(Point2);
            this.p3 = new Vector3(Point3);
        }

        public Triangle(Triangle other)
        {
            this.p1 = new Vector3(other.p1);
            this.p2 = new Vector3(other.p2);
            this.p3 = new Vector3(other.p3);
        }

        public static Vector3 Normal(Triangle Face)
        {
            Vector3 tmp = Vector3.Cross((Face.p2 - Face.p1), (Face.p3 - Face.p1));
            return Vector3.Normalize(tmp);
        }

        public Vector3 Normal()
        {
            return Normal(this);
        }

        public static Vector3 Normal(Vector3 Point1, Vector3 Point2, Vector3 Point3)
        {
            Triangle t = new Triangle(Point1, Point2, Point3);
            return Normal(t);
        }

        public static Vector3 Normal(Vector3[] Points)
        {
            Triangle t = new Triangle(Points);
            return Normal(t);
        }

        public static bool PointInside(Triangle triangle, Vector3 Point)
        {
            return (Triangle.SameSide(Point, triangle.p1, triangle.p2, triangle.p3) & 
                    Triangle.SameSide(Point, triangle.p2, triangle.p1, triangle.p3) &
                    Triangle.SameSide(Point, triangle.p3, triangle.p1, triangle.p2));
        }

        public bool PointInside(Vector3 Point)
        {
            return PointInside(this, Point);
        }

        public static bool PointInside(Vector3 FacePoint1, Vector3 FacePoint2, Vector3 FacePoint3, Vector3 Point)
        {
            return PointInside(new Triangle(FacePoint1, FacePoint2, FacePoint3), Point);
        }

        private static bool SameSide(Vector3 Pnt1, Vector3 Pnt2, Vector3 FacePntA, Vector3 FacePntB)
        {
            Vector3 p1 = Vector3.Cross(FacePntB - FacePntA, Pnt1 - FacePntA);
            Vector3 p2 = Vector3.Cross(FacePntB - FacePntA, Pnt2 - FacePntA);
            return (Vector3.Dot(p1, p2) >= 0);
        }

        public bool RayIntersection(Vector3 rayOrigin, Vector3 rayVector, out Vector3 intersectionPoint, out float distance)
        {
            intersectionPoint = new Vector3();
            distance = 0f;
            const float EPSILON = 0.0000001f;
            Vector3 vertex0 = this.p1;
            Vector3 vertex1 = this.p2;  
            Vector3 vertex2 = this.p3;
            Vector3 edge1, edge2, h, s, q;
            float a,f,u,v;
            edge1 = vertex1 - vertex0;
            edge2 = vertex2 - vertex0;
            h = rayVector.Cross(edge2);
            a = edge1.Dot(h);
            if (a > -EPSILON && a < EPSILON) return false;    // This ray is parallel to this triangle.
            f = 1.0f/a;
            s = rayOrigin - vertex0;
            u = f * (s.Dot(h));
            if (u < 0.0 || u > 1.0) return false;
            q = s.Cross(edge1);
            v = f * rayVector.Dot(q);
            if (v < 0.0 || u + v > 1.0) return false;
            // At this stage we can compute t to find out where the intersection point is on the line.
            float t = f * edge2.Dot(q);
            if (t > EPSILON) // ray intersection
            {
                intersectionPoint = rayOrigin + rayVector * t;
                distance = t;
                return true;
            }
            else // This means that there is a line intersection but not a ray intersection.
                return false;
        }

        public static Vector3 Centroid(Triangle triangle)
        {
            return new Vector3((triangle.p1.X + triangle.p2.X + triangle.p3.X) / 3f, (triangle.p1.Y + triangle.p2.Y + triangle.p3.Y) / 3f, (triangle.p1.Z + triangle.p2.Z + triangle.p3.Z) / 3f);
        }

        public Vector3 BarycentricCoordinates(Vector3 p)   //return point barycentric coordinates
        {
            float denominatorInverse = 1f / ((p2.Y - p3.Y) * (p1.X - p3.X) + (p3.X - p2.X) * (p1.Y - p3.Y));
            float w1 = ((p2.Y - p3.Y) * (p.X - p3.X) + (p3.X - p2.X) * (p.Y - p3.Y)) * denominatorInverse;
            float w2 = ((p3.Y - p1.Y) * (p.X - p3.X) + (p1.X - p3.X) * (p.Y - p3.Y)) * denominatorInverse;
            float w3 = 1f - w1 - w2;
            return new Vector3(new float[] { w1, w2, w3 });
        }

        public Vector3 WorldCoordinates(Vector3 barycentricCoordinates)   //return point Cartesian coordinates
        {
            return barycentricCoordinates.X * this.p1 + barycentricCoordinates.Y * this.p2 + barycentricCoordinates.Z * this.p3;
        }
        public Vector3 WorldCoordinates(Vector2 barycentricCoordinates)   //return point Cartesian coordinates given barycentric w1, w2; w0 = 1 - w1 - w2
        {
            float w0 = 1f - barycentricCoordinates.X - barycentricCoordinates.Y;
            return w0 * this.p1 + barycentricCoordinates.X * this.p2 + barycentricCoordinates.Y * this.p3;
        }

        public int NearestTriangleIndex(Triangle[] triangleArray)
        {
            float tmpLen = float.MaxValue;
            int ind = 0;
            for (int i = 0; i < triangleArray.Length; i++)
            {
                if (this.Centroid().Distance(triangleArray[i].Centroid()) < tmpLen)
                {
                    ind = i;
                    tmpLen = this.Centroid().Distance(triangleArray[i].Centroid());
                }
            }
            return ind;
        }

        public Triangle NearestTriangle(Triangle[] triangleArray)
        {
            int ind = this.NearestTriangleIndex(triangleArray);
            return triangleArray[ind];
        }

        public bool Equals(Triangle other)
        {
            return (this.p1.Equals(other.p1) && this.p2.Equals(other.p2) && this.p3.Equals(other.p3));
        }

        public override string ToString()
        {
            return "(" + this.Point1.ToString() + "), (" + this.Point2.ToString() + "), (" + this.Point3.ToString() + ")";
        }
    }
}
