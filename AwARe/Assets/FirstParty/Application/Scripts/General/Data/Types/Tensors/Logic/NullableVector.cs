using System;

using AwARe.Data.Logic.Interfaces;

using UnityEngine;

namespace AwARe.Data.Logic
{
    public struct NullableVector3 : ITensor3<float?>
    {
        private float? X, Y, Z;


        public NullableVector3(float? x, float? y, float? z)
        {
            this.X = x; this.Y = y; this.Z = z;
        }

        public NullableVector3((float?, float?, float?) tuple)
        {
            this.X = tuple.Item1; this.Y = tuple.Item2; this.Z = tuple.Item3;
        }

        public NullableVector3(Vector3 vector)
        {
            this.X = vector.x; this.Y = vector.y; this.Z = vector.z;
        }


        public static explicit operator Vector3(NullableVector3 p) => p.ToVector3();
        public static implicit operator NullableVector3(Vector3 v) => new(v);
        public static implicit operator NullableVector3(Point3 v) => (Vector3)v;

        public float? x { get => X; set => X = value; }
        public float? y { get => Y; set => Y = value; }
        public float? z { get => Z; set => Z = value; }
        public float? this[int idx]
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

        public float?[] ToArray()
        {
            return new[] { X, Y, Z };
        }

        public Vector3 ToVector3()
        {
            return new Vector3(X ?? 0, Y ?? 0, Z ?? 0);
        }

        public static NullableVector3 zero => (NullableVector3)(Vector3.zero);
        public static NullableVector3 one => (NullableVector3)(Vector3.one);

        static public float? safeOp(Func<float, float> op, float? v) => v == null ? null : op(v.Value);
        static public float? safeOp(Func<float, float?> op, float? v) => v == null ? null : op(v.Value);
        static public float? safeOp(Func<float, float, float> op, float? l, float? r) => (l == null || r == null) ? null : op(l.Value, r.Value);
        static public float? safeOp(Func<float, float, float?> op, float? l, float? r) => (l == null || r == null) ? null : op(l.Value, r.Value);

        static public NullableVector3 elementWiseOp(Func<float?, float?> op, NullableVector3 v) => new(op(v.x), op(v.y), op(v.z));
        static public NullableVector3 elementWiseOp(Func<float?, float?, float?> op, float? left, NullableVector3 right) => new(op(left, right.x), op(left, right.y), op(left, right.z));
        static public NullableVector3 elementWiseOp(Func<float?, float?, float?> op, NullableVector3 left, float? right) => new(op(left.x, right), op(left.y, right), op(left.z, right));
        static public NullableVector3 elementWiseOp(Func<float?, float?, float?> op, NullableVector3 left, NullableVector3 right) => new(op(left.x, right.x), op(left.y, right.y), op(left.z, right.z));

        static private float? safeAdd(float? l, float? r) => safeOp((float l, float r) => l + r, l, r);
        static public NullableVector3 operator +(float? left, NullableVector3 right) => elementWiseOp(safeAdd, left, right);
        static public NullableVector3 operator +(NullableVector3 left, float? right) => elementWiseOp(safeAdd, left, right);
        static public NullableVector3 operator +(NullableVector3 left, NullableVector3 right) => elementWiseOp(safeAdd, left, right);

        static private float? safeSub(float? l, float? r) => safeOp((float l, float r) => l - r, l, r);
        static public NullableVector3 operator -(float? left, NullableVector3 right) => elementWiseOp(safeSub, left, right);
        static public NullableVector3 operator -(NullableVector3 left, float? right) => elementWiseOp(safeSub, left, right);
        static public NullableVector3 operator -(NullableVector3 left, NullableVector3 right) => elementWiseOp(safeSub, left, right);

        static private float? safeMul(float? l, float? r) => safeOp((float l, float r) => l * r, l, r);
        static public NullableVector3 operator *(float? left, NullableVector3 right) => elementWiseOp(safeMul, left, right);
        static public NullableVector3 operator *(NullableVector3 left, float? right) => elementWiseOp(safeMul, left, right);
        static public NullableVector3 operator *(NullableVector3 left, NullableVector3 right) => elementWiseOp(safeMul, left, right);

        static private float? safeDiv(float? l, float? r) => safeOp((float l, float r) => (r == 0) ? null : l * r, l, r);
        static public NullableVector3 operator /(float? left, NullableVector3 right) => elementWiseOp(safeDiv, left, right);
        static public NullableVector3 operator /(NullableVector3 left, float? right) => elementWiseOp(safeDiv, left, right);
        static public NullableVector3 operator /(NullableVector3 left, NullableVector3 right) => elementWiseOp(safeDiv, left, right);

        static private float? safeMin(float? l, float? r) => safeOp(Mathf.Min, l, r);
        static public NullableVector3 Min(NullableVector3 left, NullableVector3 right) => elementWiseOp(safeMin, left, right);
        static private float? safeMax(float? l, float? r) => safeOp(Mathf.Max, l, r);
        static public NullableVector3 Max(NullableVector3 left, NullableVector3 right) => elementWiseOp(safeMin, left, right);

        static public float MinEl(NullableVector3 v) =>
            MinEl(v, out _);
        static public float MinEl(NullableVector3 v, out int arg)
        {
            var a = v.ToArray();
            var min = float.PositiveInfinity; arg = -1;
            for (int i = 0; i < 3; i++) if (a[i].HasValue && a[i].Value < min) { min = a[i].Value; arg = i; }
            return min;
        }

        static public float MaxEl(NullableVector3 v) =>
            MaxEl(v, out _);
        static public float MaxEl(NullableVector3 v, out int arg)
        {
            var a = v.ToArray();
            var max = float.PositiveInfinity; arg = -1;
            for (int i = 0; i < 3; i++) if (a[i].HasValue && a[i].Value > max) { max = a[i].Value; arg = i; }
            return max;
        }

        static private float? safeFloor(float? v) => safeOp(Mathf.Floor, v);
        static public NullableVector3 Floor(Vector3 v) => elementWiseOp(safeFloor, v);
        static private float? safeCeil(float? v) => safeOp(Mathf.Ceil, v);
        static public NullableVector3 Ceil(Vector3 v) => elementWiseOp(safeCeil, v);
    }
}

