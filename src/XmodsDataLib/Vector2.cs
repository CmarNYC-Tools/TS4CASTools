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
    public struct Vector2
    {
        private float x, y;

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

        public float[] Coordinates
        {
            get { return new float[] { x, y }; }
            set
            {
                x = value[0];
                y = value[1];
            }
        }

        public Vector2(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public Vector2(float[] coordinates)
        {
            this.x = coordinates[0];
            this.y = coordinates[1];
        }

        public Vector2(Vector2 vector)
        {
            this.x = vector.X;
            this.y = vector.Y;
        }

        public static bool operator ==(Vector2 v1, Vector2 v2)
        {
            const float EPSILON = 1e-4f;
            return
            (
               (Math.Abs(v1.X - v2.X) < EPSILON) &&
               (Math.Abs(v1.Y - v2.Y) < EPSILON) 
            );
        }

        public override bool Equals(object obj)
        {
            if (obj is Vector2)
            {
                Vector2 other = (Vector2)obj;
                return (other == this);
            }
            else
            {
                return false;
            }
        }

        public bool Equals(Vector2 obj)
        {
            return obj == this;
        }

        public static bool operator !=(Vector2 v1, Vector2 v2)
        {
            return !(v1 == v2);
        }

        public override int GetHashCode()
        {
            return
            (
               (int)((X + Y) % Int32.MaxValue)
            );
        }

        public override string ToString()
        {
            return this.X.ToString() + ", " + this.Y.ToString();
        }

        public string ToString(string format)
        {
            return this.X.ToString(format) + ", " + this.Y.ToString(format);
        }

    }
}
