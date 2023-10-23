using AwARe.DataTypes.Interfaces;
using System;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace AwARe.DataTypes
{
    public struct Point2 : ITensor2<int>
    {
        private int X, Y;

        public Point2(int x, int y)
        {
            this.X = x; this.Y = y;
        }

        public Point2((int, int) tuple)
        {
            this.X = tuple.Item1; this.Y = tuple.Item2;
        }

        public Point2(int[] array)
        {
            int l = array.Length;
            this.X = l > 0 ? array[0] : 0;
            this.Y = l > 1 ? array[1] : 0;
        }

        public Point2(Vector2 vector)
        {
            this.X = (int)vector.x; this.Y = (int)vector.y;
        }

        public static implicit operator Vector2(Point2 p) => p.ToVector2();
        public static explicit operator Point2(Vector2 v) => new(v);

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

        public static Point2 zero => new(0, 0);
        public static Point2 one => new(1, 1);

        static private Point2 elementWiseOp(Func<float, int> op, Vector2 v) => new(op(v.x), op(v.y));
        static private Point2 elementWiseOp(Func<int, int, int> op, int left, Point2 right) => new(op(left, right.x), op(left, right.y));
        static private Point2 elementWiseOp(Func<int, int, int> op, Point2 left, int right) => new(op(left.x, right), op(left.y, right));
        static private Point2 elementWiseOp(Func<int, int, int> op, Point2 left, Point2 right) => new(op(left.x, right.x), op(left.y, right.y));
        static private Point2 elementWiseOp(Func<int[], int> op, Point2[] points) => new(op(points.Select(p => p.x).ToArray()), op(points.Select(p => p.y).ToArray()));
        static private (int?, int?) safeElementWiseOp(Func<int, int, int?> op, int left, Point2 right) => (op(left, right.x), op(left, right.y));
        static private (int?, int?) safeElementWiseOp(Func<int, int, int?> op, Point2 left, int right) => (op(left.x, right), op(left.y, right));
        static private (int?, int?) safeElementWiseOp(Func<int, int, int?> op, Point2 left, Point2 right) => (op(left.x, right.x), op(left.y, right.y));

        static public Point2 operator +(int left, Point2 right) => elementWiseOp((l, r) => l + r, left, right);
        static public Point2 operator +(Point2 left, int right) => elementWiseOp((l, r) => l + r, left, right);
        static public Point2 operator +(Point2 left, Point2 right) => elementWiseOp((l, r) => l + r, left, right);

        static public Point2 operator -(int left, Point2 right) => elementWiseOp((l, r) => l - r, left, right);
        static public Point2 operator -(Point2 left, int right) => elementWiseOp((l, r) => l - r, left, right);
        static public Point2 operator -(Point2 left, Point2 right) => elementWiseOp((l, r) => l - r, left, right);

        static public Point2 operator *(int left, Point2 right) => elementWiseOp((l, r) => l * r, left, right);
        static public Point2 operator *(Point2 left, int right) => elementWiseOp((l, r) => l * r, left, right);
        static public Point2 operator *(Point2 left, Point2 right) => elementWiseOp((l, r) => l * r, left, right);

        static public Point2 operator /(int left, Point2 right) => elementWiseOp((l, r) => l / r, left, right);
        static public Point2 operator /(Point2 left, int right) => elementWiseOp((l, r) => l / r, left, right);
        static public Point2 operator /(Point2 left, Point2 right) => elementWiseOp((l, r) => l / r, left, right);

        static private int? SafeDivInt(int left, int right) => (right == 0) ? null : (left / right);
        static public (int?, int?) SafeDiv(int left, Point2 right) => safeElementWiseOp(SafeDivInt, left, right);
        static public (int?, int?) SafeDiv(Point2 left, int right) => safeElementWiseOp(SafeDivInt, left, right);
        static public (int?, int?) SafeDiv(Point2 left, Point2 right) => safeElementWiseOp(SafeDivInt, left, right);

        static public Point2 Min(Point2 left, Point2 right) => elementWiseOp(Math.Min, left, right);
        static public Point2 Min(Point2[] points) => elementWiseOp(p => p.Min(), points);
        static public Point2 Max(Point2 left, Point2 right) => elementWiseOp(Math.Max, left, right);
        static public Point2 Max(Point2[] points) => elementWiseOp(p => p.Max(), points);

        static public Point2 Floor(Vector2 v) => elementWiseOp(Mathf.FloorToInt, v);
        static public Point2 Ceil(Vector2 v) => elementWiseOp(Mathf.CeilToInt, v);
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


        public static implicit operator Vector3(Point3 p) => p.ToVector3();
        public static explicit operator Point3(Vector3 v) => new(v);

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
        public static Point3 zero => new(0, 0, 0);
        public static Point3 one => new(1, 1, 1);

        static private Point3 elementWiseOp(Func<float, int> op, Vector3 v) => new(op(v.x), op(v.y), op(v.z));
        static private Point3 elementWiseOp(Func<int, int, int> op, int left, Point3 right) => new(op(left, right.x), op(left, right.y), op(left, right.z));
        static private Point3 elementWiseOp(Func<int, int, int> op, Point3 left, int right) => new(op(left.x, right), op(left.y, right), op(left.z, right));
        static private Point3 elementWiseOp(Func<int, int, int> op, Point3 left, Point3 right) => new(op(left.x, right.x), op(left.y, right.y), op(left.z, right.z));
        static private Point3 elementWiseOp(Func<int[], int> op, Point3[] points) => new(op(points.Select(p=>p.x).ToArray()), op(points.Select(p => p.y).ToArray()), op(points.Select(p => p.z).ToArray()));
        static private (int?, int?, int?) safeElementWiseOp(Func<int, int, int?> op, int left, Point3 right) => (op(left, right.x), op(left, right.y), op(left, right.z));
        static private (int?, int?, int?) safeElementWiseOp(Func<int, int, int?> op, Point3 left, int right) => (op(left.x, right), op(left.y, right), op(left.z, right));
        static private (int?, int?, int?) safeElementWiseOp(Func<int, int, int?> op, Point3 left, Point3 right) => (op(left.x, right.x), op(left.y, right.y), op(left.z, right.z));

        static public Point3 operator +(int left, Point3 right) => elementWiseOp((l, r) => l + r, left, right);
        static public Point3 operator +(Point3 left, int right) => elementWiseOp((l, r) => l + r, left, right);
        static public Point3 operator +(Point3 left, Point3 right) => elementWiseOp((l, r) => l + r, left, right);

        static public Point3 operator -(int left, Point3 right) => elementWiseOp((l, r) => l - r, left, right);
        static public Point3 operator -(Point3 left, int right) => elementWiseOp((l, r) => l - r, left, right);
        static public Point3 operator -(Point3 left, Point3 right) => elementWiseOp((l, r) => l - r, left, right);

        static public Point3 operator *(int left, Point3 right) => elementWiseOp((l, r) => l * r, left, right);
        static public Point3 operator *(Point3 left, int right) => elementWiseOp((l, r) => l * r, left, right);
        static public Point3 operator *(Point3 left, Point3 right) => elementWiseOp((l, r) => l * r, left, right);

        static public Point3 operator /(int left, Point3 right) => elementWiseOp((l, r) => l / r, left, right);
        static public Point3 operator /(Point3 left, int right) => elementWiseOp((l, r) => l / r, left, right);
        static public Point3 operator /(Point3 left, Point3 right) => elementWiseOp((l, r) => l / r, left, right);

        static private int? SafeDivInt(int left, int right) => (right == 0) ? null : (left / right);
        static public (int?, int?, int?) SafeDiv(int left, Point3 right) => safeElementWiseOp(SafeDivInt, left, right);
        static public (int?, int?, int?) SafeDiv(Point3 left, int right) => safeElementWiseOp(SafeDivInt, left, right);
        static public (int?, int?, int?) SafeDiv(Point3 left, Point3 right) => safeElementWiseOp(SafeDivInt, left, right);

        static public Point3 Min(Point3 left, Point3 right) => elementWiseOp(Math.Min, left, right);
        static public Point3 Min(Point3[] points) => elementWiseOp(p => p.Min(), points);
        static public Point3 Max(Point3 left, Point3 right) => elementWiseOp(Math.Max, left, right);
        static public Point3 Max(Point3[] points) => elementWiseOp(p => p.Max(), points);

        static public Point3 Floor(Vector3 v) => elementWiseOp(Mathf.FloorToInt, v);
        static public Point3 Ceil(Vector3 v) => elementWiseOp(Mathf.CeilToInt, v);
    }

    public struct Point4 : ITensor4<int>
    {

        private int X, Y, Z, W;

        public Point4(int x, int y, int z, int w)
        {
            this.X = x; this.Y = y; this.Z = z; this.W = w;
        }

        public Point4((int, int, int, int) tuple)
        {
            this.X = tuple.Item1; this.Y = tuple.Item2; this.Z = tuple.Item3; this.W = tuple.Item4;
        }

        public Point4(int[] array)
        {
            int l = array.Length;
            this.X = l > 0 ? array[0] : 0;
            this.Y = l > 1 ? array[1] : 0;
            this.Z = l > 2 ? array[2] : 0;
            this.W = l > 3 ? array[3] : 0;
        }

        public Point4(Vector4 vector)
        {
            this.X = (int)vector.x; this.Y = (int)vector.y; this.Z = (int)vector.z; this.W = (int)vector.w;
        }


        public static implicit operator Vector4(Point4 p) => p.ToVector4();
        public static explicit operator Point4(Vector4 v) => new(v);

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

        public static Point4 zero => new(0, 0, 0, 0);
        public static Point4 one => new(1, 1, 1, 1);

        static private Point4 elementWiseOp(Func<float, int> op, Vector4 v) => new(op(v.x), op(v.y), op(v.z), op(v.w));
        static private Point4 elementWiseOp(Func<int, int, int> op, int left, Point4 right) => new(op(left, right.x), op(left, right.y), op(left, right.z), op(left, right.w));
        static private Point4 elementWiseOp(Func<int, int, int> op, Point4 left, int right) => new(op(left.x, right), op(left.y, right), op(left.z, right), op(left.w, right));
        static private Point4 elementWiseOp(Func<int, int, int> op, Point4 left, Point4 right) => new(op(left.x, right.x), op(left.y, right.y), op(left.z, right.z), op(left.w, right.w));
        static private Point4 elementWiseOp(Func<int[], int> op, Point4[] points) => new(op(points.Select(p => p.x).ToArray()), op(points.Select(p => p.y).ToArray()), op(points.Select(p => p.z).ToArray()), op(points.Select(p => p.w).ToArray()));
        static private (int?, int?, int?, int?) safeElementWiseOp(Func<int, int, int?> op, int left, Point4 right) => (op(left, right.x), op(left, right.y), op(left, right.z), op(left, right.w));
        static private (int?, int?, int?, int?) safeElementWiseOp(Func<int, int, int?> op, Point4 left, int right) => (op(left.x, right), op(left.y, right), op(left.z, right), op(left.w, right));
        static private (int?, int?, int?, int?) safeElementWiseOp(Func<int, int, int?> op, Point4 left, Point4 right) => (op(left.x, right.x), op(left.y, right.y), op(left.z, right.z), op(left.w, right.w));

        static public Point4 operator +(int left, Point4 right) => elementWiseOp((l, r) => l + r, left, right);
        static public Point4 operator +(Point4 left, int right) => elementWiseOp((l, r) => l + r, left, right);
        static public Point4 operator +(Point4 left, Point4 right) => elementWiseOp((l, r) => l + r, left, right);

        static public Point4 operator -(int left, Point4 right) => elementWiseOp((l, r) => l - r, left, right);
        static public Point4 operator -(Point4 left, int right) => elementWiseOp((l, r) => l - r, left, right);
        static public Point4 operator -(Point4 left, Point4 right) => elementWiseOp((l, r) => l - r, left, right);

        static public Point4 operator *(int left, Point4 right) => elementWiseOp((l, r) => l * r, left, right);
        static public Point4 operator *(Point4 left, int right) => elementWiseOp((l, r) => l * r, left, right);
        static public Point4 operator *(Point4 left, Point4 right) => elementWiseOp((l, r) => l * r, left, right);

        static public Point4 operator /(int left, Point4 right) => elementWiseOp((l, r) => l / r, left, right);
        static public Point4 operator /(Point4 left, int right) => elementWiseOp((l, r) => l / r, left, right);
        static public Point4 operator /(Point4 left, Point4 right) => elementWiseOp((l, r) => l / r, left, right);

        static private int? SafeDivInt(int left, int right) => (right == 0) ? null : (left / right);
        static public (int?, int?, int?, int?) SafeDiv(int left, Point4 right) => safeElementWiseOp(SafeDivInt, left, right);
        static public (int?, int?, int?, int?) SafeDiv(Point4 left, int right) => safeElementWiseOp(SafeDivInt, left, right);
        static public (int?, int?, int?, int?) SafeDiv(Point4 left, Point4 right) => safeElementWiseOp(SafeDivInt, left, right);

        static public Point4 Min(Point4 left, Point4 right) => elementWiseOp(Math.Min, left, right);
        static public Point4 Min(Point4[] points) => elementWiseOp(p => p.Min(), points);
        static public Point4 Max(Point4 left, Point4 right) => elementWiseOp(Math.Max, left, right);
        static public Point4 Max(Point4[] points) => elementWiseOp(p => p.Max(), points);

        static public Point4 Floor(Vector4 v) => elementWiseOp(Mathf.FloorToInt, v);
        static public Point4 Ceil(Vector4 v) => elementWiseOp(Mathf.CeilToInt, v);
    }
}

