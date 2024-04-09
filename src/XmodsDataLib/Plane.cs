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
    public class Plane        //from paulbourke.net/geometry/planeeq
    {
        private float a, b, c, d;

        public float A
        {
            get { return a; }
            set { a = value; }
        }
        public float B
        {
            get { return b; }
            set { b = value; }
        }
        public float C
        {
            get { return c; }
            set { c = value; }
        }
        public float D
        {
            get { return d; }
            set { d = value; }
        }

        public Plane(Vector3 V1, Vector3 V2, Vector3 V3)
        {
            this.a = V1.Y * (V2.Z - V3.Z) + V2.Y * (V3.Z - V1.Z) + V3.Y * (V1.Z - V2.Z);
            this.b = V1.Z * (V2.X - V3.X) + V2.Z * (V3.X - V1.X) + V3.Z * (V1.X - V2.X);
            this.c = V1.X * (V2.Y - V3.Y) + V2.X * (V3.Y - V1.Y) + V3.X * (V1.Y - V2.Y);
            this.d = - (V1.X * ((V2.Y * V3.Z) - (V3.Y * V2.Z)) + 
                        V2.X * ((V3.Y * V1.Z) - (V1.Y * V3.Z)) + 
                        V3.X * ((V1.Y * V2.Z) - (V2.Y * V1.Z)));
        }

        public Plane(Triangle triangle) : this(triangle.Point1, triangle.Point2, triangle.Point3) { }

        public Plane(Vector3 V1, Vector3 V2)
        {
            Vector3 n = Vector3.Cross(V2, V1);
            this.a = n.X;
            this.b = n.Y;
            this.c = n.Z;
            this.d = - ((this.a * V1.X) + (this.b * V1.Y) + (this.c * V1.Z));
        }

        public static float Distance(Vector3 Pnt, Plane P)
        {
            return
            (
                (float) Math.Abs(
                ((P.A * Pnt.X) + (P.B * Pnt.Y) + (P.C * Pnt.Z) + P.D) /
                Math.Sqrt((P.A * P.A) + (P.B * P.B) + (P.C * P.C)))
            );
        }
        public float Distance(Vector3 Pnt)
        {
            return Distance(Pnt, this);
        }

        public static float Side(Vector3 Pnt, Plane P)    //positive if V on same side as normal, negative if on opposite side, zero if in plane
        {
            return (P.A * Pnt.X) + (P.B * Pnt.Y) + (P.C * Pnt.Z) + P.D;
        }
        public float Side(Vector3 V)
        {
            return Side(V, this);
        }

       // public static Vector3 ProjectToPlane(Vector3 Plane1, Vector3 Plane2, Vector3 Plane3, Vector3 Point)
       // {
            //x1 = argument0; y1 = argument1; z1 = argument2;
            //x2 = argument3; y2 = argument4; z2 = argument5;
            //x3 = argument6; y3 = argument7; z3 = argument8;
            //x4 = argument9; y4 = argument10; z4 = argument11;

            //float vx1 = Plane2.X - Plane1.X; float vy1 = Plane2.Y - Plane1.Y; float vz1 = Plane2.Z - Plane1.Z;
            //float vx2 = Plane3.X - Plane1.X; float vy2 = Plane3.Y - Plane1.Y; float vz2 = Plane3.Z - Plane1.Z;

         //   Vector3 v1 = Plane3 - Plane1;
         //   Vector3 v2 = Plane2 - Plane1;

            //vx = vy2*vz1 - vy1*vz2;
            //vy = vx1*vz2 - vx2*vz1;
            //vz = vx2*vy1 - vx1*vy2;

         //   Vector3 vn = Vector3.Cross(v1, v2);

            //ds = sqrt(vx*vx + vy*vy + vz*vz);

          //  float ds = (float) Math.Sqrt(Vector3.Dot(vn, vn));

          //  if (ds != 0)
          //  {
                //i = vx*x1 + vy*y1 + vz*z1;
                //ii = vx*x4 + vy*y4 + vz*z4;
                //di = abs(i - ii)/ds;

           //     float i = (vn.X * Plane1.X) + (vn.Y * Plane1.Y) + (vn.Z * Plane1.Z);
           //     float ii = (vn.X * Point.X) + (vn.Y * Point.Y) + (vn.Z * Point.Z);
           //     float di = (float)(Math.Abs(i - ii) / ds);

                //xx = x4 + vx*di/ds;
                //yy = y4 + vy*di/ds;
                //zz = z4 + vz*di/ds;

           //     float px = (float)Point.X + (vn.X * di / ds);
           //     float py = (float)Point.Y + (vn.Y * di / ds);
           //     float pz = (float)Point.Z + (vn.Z * di / ds);

           //     return new Vector3(px, py, pz);
           // }
           // else
           // {
           //     return new Vector3(0, 0, 0);
           // }
       // }

       // public static Vector3 ClosestPoint(Plane Pl, Vector3 Pnt)
       // {
       //     Vector3 pn = new Vector3(Pl.A, Pl.B, Pl.C);     //plane normal
       //     float d = Vector3.Dot(pn, Pnt - (pn * Pl.D));
       //     return Pnt - (pn * d);
       // }
       // public Vector3 ClosestPoint(Vector3 Pnt)
       // {
       //     return ClosestPoint(this, Pnt);
       // }

        public override string ToString()
        {
            return this.A.ToString() + "x + " + this.B.ToString() + "y + " + this.C.ToString() + "z + " + this.D.ToString() + " = 0";
        }
    }
}
