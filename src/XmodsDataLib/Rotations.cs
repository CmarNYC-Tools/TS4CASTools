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
   The author may be contacted at modthesims.info, username cmarNYC. 
 
   Code for Euler conversion is from: http://www.euclideanspace.com/maths/geometry/rotations/conversions/quaternionToEuler/index.htm 
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace Xmods.DataLib
{
    public class Quaternion
    {
        float x, y, z, w;

        public float X { get { return this.x; } }
        public float Y { get { return this.y; } }
        public float Z { get { return this.z; } }
        public float W { get { return this.w; } }
        public float[] Coordinates { get { return new float[] { this.x, this.y, this.z, this.w }; } }

        public Quaternion(float x, float y, float z, float w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        public Quaternion(float[] quat)
        {
            this.x = quat[0];
            this.y = quat[1];
            this.z = quat[2];
            this.w = quat[3];
        }

        public static Quaternion Identity
        {
            get { return new Quaternion(0, 0, 0, 1); }
        }

        public bool isEmpty
        {
            get { return this.x == 0d && this.y == 0d && this.z == 0d && (this.w == 1d || this.w == 0d); }
        }

        public bool isIdentity
        {
            get { return this.x == 0d && this.y == 0d && this.z == 0d && this.w == 1d; }
        }

        public bool isNormalized
        {
            get
            {
                double magnitude = this.x * this.x + this.y * this.y + this.z * this.z + this.w * this.w;
                return (Math.Abs(magnitude - 1d) <= .000001d);
            }
        }

        public void Normalize()
        {
            double magnitude = Math.Sqrt(this.x * this.x + this.y * this.y + this.z * this.z + this.w * this.w);
            this.x = (float)(this.x / magnitude);
            this.y = (float)(this.y / magnitude);
            this.z = (float)(this.z / magnitude);
            this.w = (float)(this.w / magnitude);
        }

        public void Balance()
        {
            double m = this.x * this.x - this.y * this.y - this.z * this.z;
            if (m <= 1d)
            {
                this.w = (float)Math.Sqrt(1d - m);
            }
            else
            {
                this.Normalize();
            }
        }

        public static Quaternion operator *(Quaternion q, Quaternion r)
        {
            return new Quaternion(r.w * q.x + r.x * q.w - r.y * q.z + r.z * q.y,
                                  r.w * q.y + r.x * q.z + r.y * q.w - r.z * q.x,
                                  r.w * q.z - r.x * q.y + r.y * q.x + r.z * q.w,
                                  r.w * q.w - r.x * q.x - r.y * q.y - r.z * q.z);
        }

        public static Quaternion operator *(Quaternion q, float f)
        {
            Quaternion tmp = new Quaternion(q.x * f, q.y * f, q.z * f, q.w);
            tmp.Normalize();
            return tmp;
        }

        public static Quaternion operator *(Quaternion q, Vector3 v)
        {
            Quaternion tmp = new Quaternion(v.X, v.Y, v.Z, 0);
            return q * tmp;
        }

        public Quaternion Conjugate()
        {
            return new Quaternion(-this.x, -this.y, -this.z, this.w);
        }

        public Quaternion Inverse()
        {
            float norm = this.x * this.x + this.y * this.y + this.z * this.z + this.w * this.w;
            if (norm > 0f)
            {
                Quaternion q = new Quaternion(-this.x / norm, -this.y / norm, -this.z / norm, this.w / norm);
                q.Normalize();
                return q;
            }
            else
            {
                return Quaternion.Identity;
            }
        }

        public Euler toEuler()
        {
            Quaternion q = this;
            float[] res = new float[3];
            res[0] = (float)Math.Atan2(2 * (q.y * q.z + q.w * q.x), q.w * q.w - q.x * q.x - q.y * q.y + q.z * q.z);
            double r21 = -2 * (q.x * q.z - q.w * q.y);
            if (r21 < -1d) r21 = -1;
            if (r21 > 1d) r21 = 1;
            res[1] = (float)Math.Asin(r21);
            res[2] = (float)Math.Atan2(2 * (q.x * q.y + q.w * q.z), q.w * q.w + q.x * q.x - q.y * q.y - q.z * q.z);
            Euler tmp = new Euler(res[0], res[1], res[2]);
            return tmp;
        }

        public Matrix3D toMatrix3D()
        {
            float[,] matrix = new float[3, 3];
            matrix[0, 0] = 1f - (2f * this.y * this.y) - (2f * this.z * this.z);
            matrix[0, 1] = (2f * this.x * this.y) - (2f * this.z * this.w);
            matrix[0, 2] = (2f * this.x * this.z) + (2f * this.y * this.w);
            matrix[1, 0] = (2f * this.x * this.y) + (2f * this.z * this.w);
            matrix[1, 1] = 1f - (2f * this.x * this.x) - (2f * this.z * this.z);
            matrix[1, 2] = (2f * this.y * this.z) - (2f * this.x * this.w);
            matrix[2, 0] = (2f * this.x * this.z) - (2f * this.y * this.w);
            matrix[2, 1] = (2f * this.y * this.z) + (2f * this.x * this.w);
            matrix[2, 2] = 1f - (2f * this.x * this.x) - (2f * this.y * this.y);
            return new Matrix3D(matrix);
        }

        public Matrix4D toMatrix4D()
        {
            return this.toMatrix4D(new Vector3(0, 0, 0));
        }

        public Matrix4D toMatrix4D(Vector3 offset)
        {
            double[,] matrix = new double[4, 4];
            matrix[0, 0] = 1d - (2d * this.y * this.y) - (2d * this.z * this.z);
            matrix[0, 1] = (2d * this.x * this.y) - (2d * this.z * this.w);
            matrix[0, 2] = (2d * this.x * this.z) + (2d * this.y * this.w);
            matrix[0, 3] = offset.X;
            matrix[1, 0] = (2d * this.x * this.y) + (2d * this.z * this.w);
            matrix[1, 1] = 1d - (2d * this.x * this.x) - (2d * this.z * this.z);
            matrix[1, 2] = (2d * this.y * this.z) - (2d * this.x * this.w);
            matrix[1, 3] = offset.Y;
            matrix[2, 0] = (2d * this.x * this.z) - (2d * this.y * this.w);
            matrix[2, 1] = (2d * this.y * this.z) + (2d * this.x * this.w);
            matrix[2, 2] = 1d - (2d * this.x * this.x) - (2d * this.y * this.y);
            matrix[2, 3] = offset.Z;
            matrix[3, 0] = 0d;
            matrix[3, 1] = 0d;
            matrix[3, 2] = 0d;
            matrix[3, 3] = 1d;

            return new Matrix4D(matrix);
        }

        public Matrix4D toMatrix4D(Vector3 offset, Vector3 scale)
        {
            double[,] matrix = new double[4, 4];
            matrix[0, 0] = scale.X - (2d * this.y * this.y) - (2d * this.z * this.z);
            matrix[0, 1] = (2d * this.x * this.y) - (2d * this.z * this.w);
            matrix[0, 2] = (2d * this.x * this.z) + (2d * this.y * this.w);
            matrix[0, 3] = offset.X;
            matrix[1, 0] = (2d * this.x * this.y) + (2d * this.z * this.w);
            matrix[1, 1] = scale.Y - (2d * this.x * this.x) - (2d * this.z * this.z);
            matrix[1, 2] = (2d * this.y * this.z) - (2d * this.x * this.w);
            matrix[1, 3] = offset.Y;
            matrix[2, 0] = (2d * this.x * this.z) - (2d * this.y * this.w);
            matrix[2, 1] = (2d * this.y * this.z) + (2d * this.x * this.w);
            matrix[2, 2] = scale.Z - (2d * this.x * this.x) - (2d * this.y * this.y);
            matrix[2, 3] = offset.Z;
            matrix[3, 0] = 0d;
            matrix[3, 1] = 0d;
            matrix[3, 2] = 0d;
            matrix[3, 3] = 1d;

            return new Matrix4D(matrix);
        }

        public Vector3 toVector3()
        {
            return new Vector3((float)this.x, (float)this.y, (float)this.z);
        }

        public override string ToString()
        {
            return this.x.ToString() + ", " + this.y.ToString() + ", " + this.z.ToString() + ", " + this.w.ToString();
        }

        public string ToString(string format)
        {
            return this.x.ToString(format) + ", " + this.y.ToString(format) + ", " + this.z.ToString(format) + ", " + this.w.ToString(format);
        }
    }

    public class Euler
    {
        float x, y, z;

        public float X { get { return this.x; } }
        public float Y { get { return this.y; } }
        public float Z { get { return this.z; } }

        public float[] xyzRotation { get { return new float[] { this.x, this.y, this.z }; } }

        public Euler(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
    }

    public class AxisAngle
    {
        double x, y, z, angle;

        public double X { get { return this.x; } }
        public double Y { get { return this.y; } }
        public double Z { get { return this.z; } }
        public double Angle { get { return this.angle; } }

        public AxisAngle(float[] values)
        {
            this.x = values[0];
            this.y = values[1];
            this.z = values[2];
            this.angle = values[3];
        }

        public AxisAngle(double[] values)
        {
            this.x = values[0];
            this.y = values[1];
            this.z = values[2];
            this.angle = values[3];
        }

        public AxisAngle(float angle, Vector3 axis)
        {
            this.x = axis.X;
            this.y = axis.Y;
            this.z = axis.Z;
            this.angle = angle;
        }

        public AxisAngle(float angle, float[] axis)
        {
            this.x = axis[0];
            this.y = axis[1];
            this.z = axis[2];
            this.angle = angle;
        }

        public void Normalize()
        {
            double magnitude = Math.Sqrt(this.x * this.x + this.y * this.y + this.z * this.z);
            if (magnitude == 0) throw new ApplicationException("Cannot normalize AxisAngle: " + this.x.ToString() + " " + this.y.ToString() + " " + this.z.ToString());
            this.x /= magnitude;
            this.y /= magnitude;
            this.z /= magnitude;
        }

        public Matrix3D ToMatrix()
        {
            double c = Math.Cos(angle);
            double s = Math.Sin(angle);
            double t = 1.0 - c;
            this.Normalize();
            float[,] m = new float[3, 3];

            m[0, 0] = (float)(c + x * x * t);
            m[1, 1] = (float)(c + y * y * t);
            m[2, 2] = (float)(c + z * z * t);

            double tmp1 = x * y * t;
            double tmp2 = z * s;
            m[1, 0] = (float)(tmp1 + tmp2);
            m[0, 1] = (float)(tmp1 - tmp2);
            tmp1 = x * z * t;
            tmp2 = y * s;
            m[2, 0] = (float)(tmp1 - tmp2);
            m[0, 2] = (float)(tmp1 + tmp2);
            tmp1 = y * z * t;
            tmp2 = x * s;
            m[2, 1] = (float)(tmp1 + tmp2);
            m[1, 2] = (float)(tmp1 - tmp2);

            return new Matrix3D(m);
        }
    }

    public class Matrix3D
    {
        float[,] matrix;

        public float[,] Matrix
        {
            get
            {
                return new float[,] { { this.matrix[0,0], this.matrix[0,1], this.matrix[0,2] },
                                      { this.matrix[1,0], this.matrix[1,1], this.matrix[1,2] },
                                      { this.matrix[2,0], this.matrix[2,1], this.matrix[2,2] } };
            }
        }

        public Matrix3D(float[,] matrix)
        {
            this.matrix = new float[,] { { matrix[0,0], matrix[0,1], matrix[0,2] },
                                         { matrix[1,0], matrix[1,1], matrix[1,2] },
                                         { matrix[2,0], matrix[2,1], matrix[2,2] } };
        }

        public static Vector3 operator *(Matrix3D m, Vector3 v)
        {
            float x1 = 0, y1 = 0, z1 = 0;
            for (int i = 0; i < 3; i++)
            {
                x1 += m.matrix[0, i] * v.Coordinates[i];
                y1 += m.matrix[1, i] * v.Coordinates[i];
                z1 += m.matrix[2, i] * v.Coordinates[i];
            }
            return new Vector3(x1, y1, z1);
        }

        public static Matrix3D operator *(Matrix3D m, float f)
        {
            float[,] res = new float[3, 3];
            for (int r = 0; r < 3; r++)
            {
                for (int c = 0; c < 3; c++)
                {
                    res[r, c] = m.matrix[r, c] * f;
                }
            }
            return new Matrix3D(res);
        }

        public static float[] operator *(Matrix3D m, float[] v)
        {
            float[] tmp = new float[3];
            for (int i = 0; i < 3; i++)
            {
                tmp[0] += m.matrix[0, i] * v[i];
                tmp[1] += m.matrix[1, i] * v[i];
                tmp[2] += m.matrix[2, i] * v[i];
            }
            return tmp;
        }

        public static Matrix3D operator *(Matrix3D m1, Matrix3D m2)
        {
            float[][] v = new float[3][];

            for (int i = 0; i < 3; i++)
            {
                v[i] = m1 * new float[] { m2.matrix[0, i], m2.matrix[1, i], m2.matrix[2, i] };
            }
            return new Matrix3D(new float[,] { { v[0][0], v[1][0], v[2][0] },
                                               { v[0][1], v[1][1], v[2][1] },
                                               { v[0][2], v[1][2], v[2][2] } });
        }

        public static Matrix3D Identity
        {
            get { return new Matrix3D(new float[,] { { 1f, 0f, 0f }, { 0f, 1f, 0f }, { 0f, 0f, 1f } }); }
        }
        
        public static Matrix3D RotateZupToYup
        {
            get { return new Matrix3D(new float[,] { { 1f, 0f, 0f }, { 0f, 0f, 1f }, { 0f, -1f, 0f } }); }
        }

        public Matrix3D Inverse()
        {
            // computes the inverse of a matrix
            float det = this.matrix[0, 0] * (this.matrix[1, 1] * this.matrix[2, 2] - this.matrix[2, 1] * this.matrix[1, 2]) -
                         this.matrix[0, 1] * (this.matrix[1, 0] * this.matrix[2, 2] - this.matrix[1, 2] * this.matrix[2, 0]) +
                         this.matrix[0, 2] * (this.matrix[1, 0] * this.matrix[2, 1] - this.matrix[1, 1] * this.matrix[2, 0]);

            float invdet = 1f / det;

            float[,] minv = new float[3, 3];
            minv[0, 0] = (this.matrix[1, 1] * this.matrix[2, 2] - this.matrix[2, 1] * this.matrix[1, 2]) * invdet;
            minv[0, 1] = (this.matrix[0, 2] * this.matrix[2, 1] - this.matrix[0, 1] * this.matrix[2, 2]) * invdet;
            minv[0, 2] = (this.matrix[0, 1] * this.matrix[1, 2] - this.matrix[0, 2] * this.matrix[1, 1]) * invdet;
            minv[1, 0] = (this.matrix[1, 2] * this.matrix[2, 0] - this.matrix[1, 0] * this.matrix[2, 2]) * invdet;
            minv[1, 1] = (this.matrix[0, 0] * this.matrix[2, 2] - this.matrix[0, 2] * this.matrix[2, 0]) * invdet;
            minv[1, 2] = (this.matrix[1, 0] * this.matrix[0, 2] - this.matrix[0, 0] * this.matrix[1, 2]) * invdet;
            minv[2, 0] = (this.matrix[1, 0] * this.matrix[2, 1] - this.matrix[2, 0] * this.matrix[1, 1]) * invdet;
            minv[2, 1] = (this.matrix[2, 0] * this.matrix[0, 1] - this.matrix[0, 0] * this.matrix[2, 1]) * invdet;
            minv[2, 2] = (this.matrix[0, 0] * this.matrix[1, 1] - this.matrix[1, 0] * this.matrix[0, 1]) * invdet;

            return new Matrix3D(minv);
        }

        public Matrix3D Transpose()
        {
            float[,] mt = new float[,] { { this.matrix[0,0], this.matrix[1,0], this.matrix[2,0] },
                                           { this.matrix[0,1], this.matrix[1,1], this.matrix[2,1] },
                                           { this.matrix[0,2], this.matrix[1,2], this.matrix[2,2] } };
            return new Matrix3D(mt);
        }

        public override string ToString()
        {
            string str = "";
            for (int r = 0; r < 3; r++)
            {
                for (int c = 0; c < 3; c++)
                {
                    str += this.matrix[r, c].ToString();
                    if (c != 2 || r != 2) str += ", ";
                }
                str += Environment.NewLine;
            }
            return str;
        }
    }

    /// <summary>
    /// Supports 4x4 matrixes only
    /// </summary>
    public struct Matrix4D
    {
        double[,] matrix;

        public double[,] Matrix
        {
            get
            {
                return new double[,] { { this.matrix[0,0], this.matrix[0,1], this.matrix[0,2], this.matrix[0,3] },
                                       { this.matrix[1,0], this.matrix[1,1], this.matrix[1,2], this.matrix[1,3] },
                                       { this.matrix[2,0], this.matrix[2,1], this.matrix[2,2], this.matrix[2,3] },
                                       { this.matrix[3,0], this.matrix[3,1], this.matrix[3,2], this.matrix[3,3] } };
            }
        }

        public double[] Values
        {
            get
            {
                return new double[] { this.matrix[0,0], this.matrix[0,1], this.matrix[0,2], this.matrix[0,3],
                                      this.matrix[1,0], this.matrix[1,1], this.matrix[1,2], this.matrix[1,3],
                                      this.matrix[2,0], this.matrix[2,1], this.matrix[2,2], this.matrix[2,3],
                                      this.matrix[3,0], this.matrix[3,1], this.matrix[3,2], this.matrix[3,3] };
            }
        }

        public Matrix4D(double[,] array4x4)
        {
            this.matrix = new double[,] { { array4x4[0,0], array4x4[0,1], array4x4[0,2], array4x4[0,3] },
                                          { array4x4[1,0], array4x4[1,1], array4x4[1,2], array4x4[1,3] },
                                          { array4x4[2,0], array4x4[2,1], array4x4[2,2], array4x4[2,3] },
                                          { array4x4[3,0], array4x4[3,1], array4x4[3,2], array4x4[3,3] } };
        }

        public Matrix4D(double[] array)
        {
            this.matrix = new double[,] { { array[0], array[1], array[2], array[3] },
                                          { array[4], array[5], array[6], array[7] },
                                          { array[8], array[9], array[10], array[11] },
                                          { array[12], array[13], array[14], array[15] } };
        }

        public static Matrix4D Identity
        {
            get { return new Matrix4D(new double[,] { { 1d, 0d, 0d, 0d }, { 0d, 1d, 0d, 0d }, { 0d, 0d, 1d, 0d }, { 0d, 0d, 0d, 1d } }); }
        }

        public static Matrix4D FromOffset(Vector3 offset)
        {
            return new Matrix4D(new double[,] { { 1d, 0d, 0d, offset.X }, { 0d, 1d, 0d, offset.Y }, { 0d, 0d, 1d, offset.Z }, { 0d, 0d, 0d, 1d } });
        }

        public static Matrix4D FromOffset(double[] offset)
        {
            return new Matrix4D(new double[,] { { 1d, 0d, 0d, offset[0] }, { 0d, 1d, 0d, offset[1] }, { 0d, 0d, 1d, offset[2] }, { 0d, 0d, 0d, 1d } });
        }

        public static Matrix4D FromScale(Vector3 scale)
        {
            return new Matrix4D(new double[,] { { scale.X, 0d, 0d, 0d }, { 0d, scale.Y, 0d, 0d }, { 0d, 0d, scale.Z, 0d }, { 0d, 0d, 0d, 1d } });
        }

        public static Matrix4D FromScale(double[] scale)
        {
            return new Matrix4D(new double[,] { { scale[0], 0d, 0d, 0d }, { 0d, scale[1], 0d, 0d }, { 0d, 0d, scale[2], 0d }, { 0d, 0d, 0d, 1d } });
        }

        public static Matrix4D FromAxisAngle(AxisAngle aa)
        {
            aa.Normalize();
            Matrix4D m = new Matrix4D();
            m.matrix = new double[,] { { 0, 0, 0, 0 },
                                       { 0, 0, 0, 0 },
                                       { 0, 0, 0, 0 },
                                       { 0, 0, 0, 0 } };
            double c = Math.Cos(aa.Angle);
            double s = Math.Sin(aa.Angle);
            double t = 1.0 - c;

            m.matrix[0, 0] = c + aa.X * aa.X * t;
            m.matrix[1, 1] = c + aa.Y * aa.Y * t;
            m.matrix[2, 2] = c + aa.Z * aa.Z * t;

            double tmp1 = aa.X * aa.Y * t;
            double tmp2 = aa.Z * s;
            m.matrix[1, 0] = tmp1 + tmp2;
            m.matrix[0, 1] = tmp1 - tmp2;
            tmp1 = aa.X * aa.Z * t;
            tmp2 = aa.Y * s;
            m.matrix[2, 0] = tmp1 - tmp2;
            m.matrix[0, 2] = tmp1 + tmp2; tmp1 = aa.Y * aa.Z * t;
            tmp2 = aa.X * s;
            m.matrix[2, 1] = tmp1 + tmp2;
            m.matrix[1, 2] = tmp1 - tmp2;

            m.matrix[3, 3] = 1;

            return m;
        }

        public static Matrix4D RotateZupToYup
        {
            get { return new Matrix4D(new double[,] { { 1f, 0f, 0f, 0f }, { 0f, 0f, 1f, 0f }, { 0f, -1f, 0f, 0f }, { 0f, 0f, 0f, 1f } }); }
        }

        public static Matrix4D RotateYupToZup
        {
            get { return new Matrix4D(new double[,] { { 1f, 0f, 0f, 0f }, { 0f, 0f, -1f, 0f }, { 0f, 1f, 0f, 0f }, { 0f, 0f, 0f, 1f } }); }
        }

        public Matrix3D ToMatrix3D()
        {
            return new Matrix3D(new float[,] { { (float)this.matrix[0,0], (float)this.matrix[0,1], (float)this.matrix[0,2] },
                                               { (float)this.matrix[1,0], (float)this.matrix[1,1], (float)this.matrix[1,2] },
                                               { (float)this.matrix[2,0], (float)this.matrix[2,1], (float)this.matrix[2,2] } });
        }

        /// <summary>
        /// Rounds values close to zero
        /// </summary>
        public void Clean()
        {
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (Math.Abs(this.matrix[i, j]) < .0000002d) this.matrix[i, j] = 0d;
                }
            }
        }

        public Vector3 Scale
        {
            get
            {
                Vector3 sx = new Vector3((float)this.matrix[0, 0], (float)this.matrix[0, 1], (float)this.matrix[0, 2]);
                Vector3 sy = new Vector3((float)this.matrix[1, 0], (float)this.matrix[1, 1], (float)this.matrix[1, 2]);
                Vector3 sz = new Vector3((float)this.matrix[2, 0], (float)this.matrix[2, 1], (float)this.matrix[2, 2]);
                return new Vector3(sx.Magnitude, sy.Magnitude, sz.Magnitude);
            }
        }

        public Vector3 Offset
        {
            get
            {
                return new Vector3((float)this.matrix[0, 3], (float)this.matrix[1, 3], (float)this.matrix[2, 3]);
            }
        }

        public Matrix4D RemoveOffset()
        {
            double[,] d = new double[4, 4];
            Array.Copy(this.matrix, d, 16);
            d[0, 3] = 0d;
            d[1, 3] = 0d;
            d[2, 3] = 0d;
            return new Matrix4D(d);
        }

        public Matrix4D Inverse()
        {
            double[,] m = this.matrix;
            double det = m[0, 3] * m[1, 2] * m[2, 1] * m[3, 0] - m[0, 2] * m[1, 3] * m[2, 1] * m[3, 0] - m[0, 3] * m[1, 1] * m[2, 2] * m[3, 0] + m[0, 1] * m[1, 3] * m[2, 2] * m[3, 0] +
                         m[0, 2] * m[1, 1] * m[2, 3] * m[3, 0] - m[0, 1] * m[1, 2] * m[2, 3] * m[3, 0] - m[0, 3] * m[1, 2] * m[2, 0] * m[3, 1] + m[0, 2] * m[1, 3] * m[2, 0] * m[3, 1] +
                         m[0, 3] * m[1, 0] * m[2, 2] * m[3, 1] - m[0, 0] * m[1, 3] * m[2, 2] * m[3, 1] - m[0, 2] * m[1, 0] * m[2, 3] * m[3, 1] + m[0, 0] * m[1, 2] * m[2, 3] * m[3, 1] +
                         m[0, 3] * m[1, 1] * m[2, 0] * m[3, 2] - m[0, 1] * m[1, 3] * m[2, 0] * m[3, 2] - m[0, 3] * m[1, 0] * m[2, 1] * m[3, 2] + m[0, 0] * m[1, 3] * m[2, 1] * m[3, 2] +
                         m[0, 1] * m[1, 0] * m[2, 3] * m[3, 2] - m[0, 0] * m[1, 1] * m[2, 3] * m[3, 2] - m[0, 2] * m[1, 1] * m[2, 0] * m[3, 3] + m[0, 1] * m[1, 2] * m[2, 0] * m[3, 3] +
                         m[0, 2] * m[1, 0] * m[2, 1] * m[3, 3] - m[0, 0] * m[1, 2] * m[2, 1] * m[3, 3] - m[0, 1] * m[1, 0] * m[2, 2] * m[3, 3] + m[0, 0] * m[1, 1] * m[2, 2] * m[3, 3];
            double invdet = 1d / det;

            double[,] minv = new double[4, 4];
            minv[0, 0] = (m[1, 2] * m[2, 3] * m[3, 1] - m[1, 3] * m[2, 2] * m[3, 1] + m[1, 3] * m[2, 1] * m[3, 2] - m[1, 1] * m[2, 3] * m[3, 2] - m[1, 2] * m[2, 1] * m[3, 3] + m[1, 1] * m[2, 2] * m[3, 3]) * invdet;
            minv[0, 1] = (m[0, 3] * m[2, 2] * m[3, 1] - m[0, 2] * m[2, 3] * m[3, 1] - m[0, 3] * m[2, 1] * m[3, 2] + m[0, 1] * m[2, 3] * m[3, 2] + m[0, 2] * m[2, 1] * m[3, 3] - m[0, 1] * m[2, 2] * m[3, 3]) * invdet;
            minv[0, 2] = (m[0, 2] * m[1, 3] * m[3, 1] - m[0, 3] * m[1, 2] * m[3, 1] + m[0, 3] * m[1, 1] * m[3, 2] - m[0, 1] * m[1, 3] * m[3, 2] - m[0, 2] * m[1, 1] * m[3, 3] + m[0, 1] * m[1, 2] * m[3, 3]) * invdet;
            minv[0, 3] = (m[0, 3] * m[1, 2] * m[2, 1] - m[0, 2] * m[1, 3] * m[2, 1] - m[0, 3] * m[1, 1] * m[2, 2] + m[0, 1] * m[1, 3] * m[2, 2] + m[0, 2] * m[1, 1] * m[2, 3] - m[0, 1] * m[1, 2] * m[2, 3]) * invdet;
            minv[1, 0] = (m[1, 3] * m[2, 2] * m[3, 0] - m[1, 2] * m[2, 3] * m[3, 0] - m[1, 3] * m[2, 0] * m[3, 2] + m[1, 0] * m[2, 3] * m[3, 2] + m[1, 2] * m[2, 0] * m[3, 3] - m[1, 0] * m[2, 2] * m[3, 3]) * invdet;
            minv[1, 1] = (m[0, 2] * m[2, 3] * m[3, 0] - m[0, 3] * m[2, 2] * m[3, 0] + m[0, 3] * m[2, 0] * m[3, 2] - m[0, 0] * m[2, 3] * m[3, 2] - m[0, 2] * m[2, 0] * m[3, 3] + m[0, 0] * m[2, 2] * m[3, 3]) * invdet;
            minv[1, 2] = (m[0, 3] * m[1, 2] * m[3, 0] - m[0, 2] * m[1, 3] * m[3, 0] - m[0, 3] * m[1, 0] * m[3, 2] + m[0, 0] * m[1, 3] * m[3, 2] + m[0, 2] * m[1, 0] * m[3, 3] - m[0, 0] * m[1, 2] * m[3, 3]) * invdet;
            minv[1, 3] = (m[0, 2] * m[1, 3] * m[2, 0] - m[0, 3] * m[1, 2] * m[2, 0] + m[0, 3] * m[1, 0] * m[2, 2] - m[0, 0] * m[1, 3] * m[2, 2] - m[0, 2] * m[1, 0] * m[2, 3] + m[0, 0] * m[1, 2] * m[2, 3]) * invdet;
            minv[2, 0] = (m[1, 1] * m[2, 3] * m[3, 0] - m[1, 3] * m[2, 1] * m[3, 0] + m[1, 3] * m[2, 0] * m[3, 1] - m[1, 0] * m[2, 3] * m[3, 1] - m[1, 1] * m[2, 0] * m[3, 3] + m[1, 0] * m[2, 1] * m[3, 3]) * invdet;
            minv[2, 1] = (m[0, 3] * m[2, 1] * m[3, 0] - m[0, 1] * m[2, 3] * m[3, 0] - m[0, 3] * m[2, 0] * m[3, 1] + m[0, 0] * m[2, 3] * m[3, 1] + m[0, 1] * m[2, 0] * m[3, 3] - m[0, 0] * m[2, 1] * m[3, 3]) * invdet;
            minv[2, 2] = (m[0, 1] * m[1, 3] * m[3, 0] - m[0, 3] * m[1, 1] * m[3, 0] + m[0, 3] * m[1, 0] * m[3, 1] - m[0, 0] * m[1, 3] * m[3, 1] - m[0, 1] * m[1, 0] * m[3, 3] + m[0, 0] * m[1, 1] * m[3, 3]) * invdet;
            minv[2, 3] = (m[0, 3] * m[1, 1] * m[2, 0] - m[0, 1] * m[1, 3] * m[2, 0] - m[0, 3] * m[1, 0] * m[2, 1] + m[0, 0] * m[1, 3] * m[2, 1] + m[0, 1] * m[1, 0] * m[2, 3] - m[0, 0] * m[1, 1] * m[2, 3]) * invdet;
            minv[3, 0] = (m[1, 2] * m[2, 1] * m[3, 0] - m[1, 1] * m[2, 2] * m[3, 0] - m[1, 2] * m[2, 0] * m[3, 1] + m[1, 0] * m[2, 2] * m[3, 1] + m[1, 1] * m[2, 0] * m[3, 2] - m[1, 0] * m[2, 1] * m[3, 2]) * invdet;
            minv[3, 1] = (m[0, 1] * m[2, 2] * m[3, 0] - m[0, 2] * m[2, 1] * m[3, 0] + m[0, 2] * m[2, 0] * m[3, 1] - m[0, 0] * m[2, 2] * m[3, 1] - m[0, 1] * m[2, 0] * m[3, 2] + m[0, 0] * m[2, 1] * m[3, 2]) * invdet;
            minv[3, 2] = (m[0, 2] * m[1, 1] * m[3, 0] - m[0, 1] * m[1, 2] * m[3, 0] - m[0, 2] * m[1, 0] * m[3, 1] + m[0, 0] * m[1, 2] * m[3, 1] + m[0, 1] * m[1, 0] * m[3, 2] - m[0, 0] * m[1, 1] * m[3, 2]) * invdet;
            minv[3, 3] = (m[0, 1] * m[1, 2] * m[2, 0] - m[0, 2] * m[1, 1] * m[2, 0] + m[0, 2] * m[1, 0] * m[2, 1] - m[0, 0] * m[1, 2] * m[2, 1] - m[0, 1] * m[1, 0] * m[2, 2] + m[0, 0] * m[1, 1] * m[2, 2]) * invdet;

            return new Matrix4D(minv);
        }

        public Matrix4D Transpose()
        {
            double[,] mt = new double[,] { { this.matrix[0,0], this.matrix[1,0], this.matrix[2,0], this.matrix[3,0] },
                                           { this.matrix[0,1], this.matrix[1,1], this.matrix[2,1], this.matrix[3,1] },
                                           { this.matrix[0,2], this.matrix[1,2], this.matrix[2,2], this.matrix[3,2] },
                                           { this.matrix[0,3], this.matrix[1,3], this.matrix[2,3], this.matrix[3,3] } };
            return new Matrix4D(mt);
        }

        public static Matrix4D operator *(Matrix4D m, float f)
        {
            double[,] res = new double[4, 4];
            for (int r = 0; r < 4; r++)
            {
                for (int c = 0; c < 4; c++)
                {
                    res[r, c] = m.matrix[r, c] * f;
                }
            }
            return new Matrix4D(res);
        }

        public static Vector3 operator *(Matrix4D m, Vector3 v)
        {
            double x1 = 0, y1 = 0, z1 = 0, ex = 0; ;
            double[] tmp = new double[] { v.X, v.Y, v.Z, 1f };
            for (int i = 0; i < 4; i++)
            {
                x1 += m.matrix[0, i] * tmp[i];
                y1 += m.matrix[1, i] * tmp[i];
                z1 += m.matrix[2, i] * tmp[i];
                ex += m.matrix[3, i] * tmp[i];
            }
            return new Vector3((float)x1, (float)y1, (float)z1);
        }

        public static Matrix4D operator *(Matrix4D m1, Matrix4D m2)
        {
            double[][] v = new double[4][];

            for (int i = 0; i < 4; i++)
            {
                v[i] = m1 * new double[] { m2.matrix[0, i], m2.matrix[1, i], m2.matrix[2, i], m2.matrix[3, i] };
            }
            return new Matrix4D(new double[,] { { v[0][0], v[1][0], v[2][0], v[3][0] },
                                                 { v[0][1], v[1][1], v[2][1], v[3][1] },
                                                 { v[0][2], v[1][2], v[2][2], v[3][2] },
                                                 { v[0][3], v[1][3], v[2][3], v[3][3] } });
        }

        public static double[] operator *(Matrix4D m, double[] v)
        {
            double[] tmp = new double[4];
            for (int i = 0; i < 4; i++)
            {
                tmp[0] += m.matrix[0, i] * v[i];
                tmp[1] += m.matrix[1, i] * v[i];
                tmp[2] += m.matrix[2, i] * v[i];
                tmp[3] += m.matrix[3, i] * v[i];
            }
            return tmp;
        }

        public override string ToString()
        {
            string str = "";
            for (int r = 0; r < 4; r++)
            {
                for (int c = 0; c < 4; c++)
                {
                    str += this.matrix[r, c].ToString("G7");
                    if (c != 3 || r != 3) str += ", ";
                }
                if (r < 4) str += Environment.NewLine;
            }
            return str;
        }

        public string ToUnpunctuatedString()
        {
            string str = "";
            for (int r = 0; r < 4; r++)
            {
                for (int c = 0; c < 4; c++)
                {
                    str += this.matrix[r, c].ToString("G7", System.Globalization.CultureInfo.InvariantCulture) + " ";
                }
            }
            return str;
        }
    }
}
