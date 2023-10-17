using AwARe.DataTypes.Interfaces;
using System;
using UnityEngine;

namespace AwARe.DataTypes
{
    public struct Point2 : ITensor2<int>
    {
        private int X, Y;
        public int x { get => X; set => X = value; }
        public int y { get => Y; set => Y = value; }
        public int this[int idx]
        {
            get => idx switch
            {
                0 => X,
                1 => Y,
                _ => throw new ArgumentOutOfRangeException(nameof(idx)),
            };
            set
            {
                switch (idx)
                {
                    case 0: X = value; return;
                    case 1: Y = value; return;
                    default: throw new ArgumentOutOfRangeException(nameof(idx));
                };
            }
        }

        public int[] ToArray()
        {
            return new[] { X, Y };
        }

        public Vector2 ToVector2()
        {
            return new Vector2(X, Y);
        }
    }

    public struct Point3 : ITensor3<int>
    {
        private int X, Y, Z;

        public Point3(int x, int y, int z)
        {
            this.X = x; this.Y = y; this.Z = z;
        }

        public Point3((int, int, int) tuple)
        {
            this.X = tuple.Item1; this.Y = tuple.Item2; this.Z = tuple.Item3;
        }

        public Point3(int[] array)
        {
            int l = array.Length;
            this.X = l > 0 ? array[0] : 0;
            this.Y = l > 1 ? array[1] : 0;
            this.Z = l > 2 ? array[2] : 0;
        }

        public Point3(Vector3 vector)
        {
            this.X = (int)vector.x; this.Y = (int)vector.y; this.Z = (int)vector.z;
        }

        public int x { get => X; set => X = value; }
        public int y { get => Y; set => Y = value; }
        public int z { get => Z; set => Z = value; }
        public int this[int idx]
        {
            get => idx switch
            {
                0 => X,
                1 => Y,
                2 => Z,
                _ => throw new ArgumentOutOfRangeException(nameof(idx)),
            };
            set
            {
                switch (idx)
                {
                    case 0: X = value; return;
                    case 1: Y = value; return;
                    case 2: Z = value; return;
                    default: throw new ArgumentOutOfRangeException(nameof(idx));
                };
            }
        }

        public int[] ToArray()
        {
            return new[] { X, Y, Z };
        }

        public Vector3 ToVector3()
        {
            return new Vector3(X, Y, Z);
        }

        static public Point3 operator +(int left, Point3 right) => new Point3(left + right.x, left + right.y, left + right.z);
        static public Point3 operator +(Point3 left, int right) => new Point3(left.x + right, left.y + right, left.z + right);
        static public Point3 operator +(Point3 left, Point3 right) => new Point3(left.x + right.x, left.y + right.y, left.z + right.z);
        static public Point3 operator -(int left, Point3 right) => new Point3(left - right.x, left - right.y, left - right.z);
        static public Point3 operator -(Point3 left, int right) => new Point3(left.x - right, left.y - right, left.z - right);
        static public Point3 operator -(Point3 left, Point3 right) => new Point3(left.x - right.x, left.y - right.y, left.z - right.z);
        static public Point3 operator *(int left, Point3 right) => new Point3(left * right.x, left * right.y, left * right.z);
        static public Point3 operator *(Point3 left, int right) => new Point3(left.x * right, left.y * right, left.z * right);
        static public Point3 operator *(Point3 left, Point3 right) => new Point3(left.x * right.x, left.y * right.y, left.z * right.z);
        static public Point3 operator /(int left, Point3 right) => new Point3(left / right.x, left / right.y, left / right.z);
        static public Point3 operator /(Point3 left, int right) => new Point3(left.x / right, left.y / right, left.z / right);
        static public Point3 operator /(Point3 left, Point3 right) => new Point3(left.x / right.x, left.y / right.y, left.z / right.z);
    }

    public struct Point4 : ITensor4<int>
    {

        private int X, Y, Z, W;
        public int x { get => X; set => X = value; }
        public int y { get => Y; set => Y = value; }
        public int z { get => Z; set => Z = value; }
        public int w { get => W; set => W = value; }
        public int this[int idx]
        {
            get => idx switch
            {
                0 => X,
                1 => Y,
                2 => Z,
                3 => W,
                _ => throw new ArgumentOutOfRangeException(nameof(idx)),
            };
            set
            {
                switch (idx)
                {
                    case 0: X = value; return;
                    case 1: Y = value; return;
                    case 2: Z = value; return;
                    case 3: W = value; return;
                    default: throw new ArgumentOutOfRangeException(nameof(idx));
                };
            }
        }

        public int[] ToArray()
        {
            return new[] { X, Y, Z, W };
        }

        public Vector4 ToVector4()
        {
            return new Vector4(X, Y, Z, W);
        }
    }
}

