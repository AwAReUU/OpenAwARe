using AwARe.DataTypes.Interfaces;
using RSG;
using System;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace AwARe.DataTypes
{
    public struct BinaryTensor3 : ITensor3<bool>
    {
        private bool X, Y, Z;

        public BinaryTensor3(bool x, bool y, bool z)
        {
            this.X = x; this.Y = y; this.Z = z;
        }

        public BinaryTensor3((bool, bool, bool) tuple)
        {
            this.X = tuple.Item1; this.Y = tuple.Item2; this.Z = tuple.Item3;
        }

        public BinaryTensor3(bool[] array)
        {
            int l = array.Length;
            this.X = l > 0 && array[0];
            this.Y = l > 1 && array[1];
            this.Z = l > 2 && array[2];
        }

        public BinaryTensor3(ITensor3<bool> tensor)
        {
            this.X = tensor.x; this.Y = tensor.y; this.Z = tensor.z;
        }

        public static explicit operator Point3(BinaryTensor3 t) => t.ToPoint3();
        public static implicit operator bool(BinaryTensor3 t) => All(t);

        public bool x { readonly get => X; set => X = value; }
        public bool y { readonly get => Y; set => Y = value; }
        public bool z { readonly get => Z; set => Z = value; }
        public bool this[int idx]
        {
            readonly get => idx switch
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

        public override readonly String ToString() =>
            $"({X}, {Y}, {Z})";

        public readonly bool[] ToArray() =>
            new[] { X, Y, Z };

        public readonly Point3 ToPoint3()
        {
            static int toInt(bool x) => x ? 1 : 0;
            return new Point3(toInt(X), toInt(Y), toInt(Z));
        }

        public static BinaryTensor3 zero => new(false, false, false);
        public static BinaryTensor3 one => new(true, true, true);

        static public BinaryTensor3 elementWiseOp<T>(Func<T, bool> op, ITensor3<T> v) => new(op(v.x), op(v.y), op(v.z));
        static public BinaryTensor3 elementWiseOp<L,R>(Func<L,R,bool> op, L left, ITensor3<R> right) => new(op(left, right.x), op(left, right.y), op(left, right.z));
        static public BinaryTensor3 elementWiseOp<L,R>(Func<L,R,bool> op, ITensor3<L> left, R right) => new(op(left.x, right), op(left.y, right), op(left.z, right));
        static public BinaryTensor3 elementWiseOp<L,R>(Func<L,R,bool> op, ITensor3<L> left, ITensor3<R> right) => new(op(left.x, right.x), op(left.y, right.y), op(left.z, right.z));
        static public BinaryTensor3 elementWiseOp<T>(Func<T[], bool> op, ITensor3<T>[] array) => new(op(array.Select(p => p.x).ToArray()), op(array.Select(p => p.y).ToArray()), op(array.Select(p => p.z).ToArray()));

        static public BinaryTensor3 operator !(BinaryTensor3 t) => elementWiseOp(b => !b, t);

        static public bool Any(BinaryTensor3 t) => t.X || t.y || t.z;
        static public bool All(BinaryTensor3 t) => t.X && t.y && t.z;
    }
}

