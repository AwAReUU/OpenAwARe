using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

using AwARe.DataTypes;
using Unity.VisualScripting;
using System.Collections;
using Newtonsoft.Json.Linq;
using UnityEngine.Video;
using System.Threading;

namespace AwARe.DataStructures
{
    public static class GridMath
    {
        private static DiscreteRay CreateRay(Vector3 start, Vector3 end, IGridSize gridSize) =>
            CreateRay(start, end, gridSize.GridSize);
        private static DiscreteRay CreateRay(Vector3 start, Vector3 end, Point3 gridSize) =>
            new(start, end, gridSize);
    }

    public class DiscreteRay : IEnumerable<CellRay>
    {
        private const float eps = 0.00001f;

        public readonly Vector3 start;
        public readonly Vector3 end;

        public readonly float l_start;
        public readonly float l_end;

        public readonly Vector3 v;
        public readonly Vector3 d;

        public DiscreteRay(Vector3 start, Vector3 end, Point3 gridSize)
        {
            this.start = start; this.end = end;
            (l_start, l_end) = GetSection(start, end, gridSize);
            (v, d) = Parametrize(start, end);
        }

        public static (Vector3, Vector3) Parametrize(Vector3 start, Vector3 end) =>
            (end - start, start);

        public static (float, float) GetSection(Vector3 start, Vector3 end, Point3 gridSize) =>
            GetSection(start, end, Point3.zero, gridSize);

        public static (float, float) GetSection(Vector3 start, Vector3 end, Vector3 min, Vector3 max)
        {
            // Parametrize the (finite) ray for computation
            (Vector3 v, Vector3 d) = Parametrize(start, end);
            float start_l = 0, end_l = 1;

            // Compute the parameter on the bounds
            var f = GetParametersFunction(v, d);
            var ls_bound_min = f(min);
            var ls_bound_max = f(max);

            // Compute the minimal and maximal values on the (finite) ray s.t. the points are in bounds.
            var ls_min = NullableVector3.Min(ls_bound_max, ls_bound_min);
            var ls_max = NullableVector3.Max(ls_bound_max, ls_bound_min);
            start_l = Mathf.Max(start_l, NullableVector3.MaxEl(ls_min));
            end_l = Mathf.Min(end_l, NullableVector3.MinEl(ls_max));

            return (start_l, end_l);
        }

        public static Func<NullableVector3, NullableVector3> GetParametersFunction(Vector3 v, Vector3 d) =>
            coords => (coords - d) / v;

        public NullableVector3 GetPossibleParameters(NullableVector3 coordinates) =>
            GetParametersFunction(v, d)(coordinates);

        public static Func<float, Vector3> GetParametrization(Vector3 v, Vector3 d) =>
            l => v * l + d;

        public Vector3 GetCoordinates(float l) =>
            GetParametrization(v, d)(l);


        // Enumerator helpers
        public float GetNext(float current)
        {
            // Get next parameter
            var next_l = SmallIncr(current);
            var point = GetCoordinates(next_l);
            var extremes = NullableVector3.elementWiseOp(NullableVector3.safeOp(GetBounds1D), v, point);

            var ls = GetPossibleParameters(extremes);
            next_l = NullableVector3.MinEl(ls, out int arg);

            return next_l;
        }

        public static float SmallIncr(float v)
        {
            var e = eps;
            var w = v + e;
            while (w == v)
            { e *= 2; w = v + e; }
            return w;
        }

        public static NullableVector3 GetBounds(Vector3 v, Vector3 xyz) =>
            NullableVector3.elementWiseOp(NullableVector3.safeOp(GetBounds1D), v, xyz);

        public static float? GetBounds1D(float v, float x) =>
            (v < 0) ? Mathf.Floor(x) : (v > 0) ? Mathf.Ceil(x) : null;

        public float GetStart() =>
            l_start;

        public bool IsPastEnd(float current_l) =>
            current_l > l_end;

        public IEnumerator<CellRay> GetEnumerator() =>
            new DiscreteRayEnumerator(this);

        IEnumerator IEnumerable.GetEnumerator() =>
            GetEnumerator();
    }

    public class DiscreteRayParameterEnumerator : IEnumerator<float>
    {
        private float? current;
        private DiscreteRay ray;

        public DiscreteRayParameterEnumerator(DiscreteRay ray)
        {
            this.ray = ray;
        }

        public float Current => current.Value;

        object IEnumerator.Current => Current;

        public void Dispose() { }

        public bool MoveNext()
        {
            current = current.HasValue ? ray.GetNext(current.Value) : current = ray.GetStart();

            return ray.IsPastEnd(current.Value);
        }

        public void Reset() => current = null;
    }

    public class DiscreteRayEnumerator : IEnumerator<CellRay>
    {
        private CellRay current;

        private readonly DiscreteRay ray;
        private readonly DiscreteRayParameterEnumerator rayEnumerator;
        private readonly float totalLength;

        public DiscreteRayEnumerator(DiscreteRay ray)
        {
            this.ray = ray;
            totalLength = Vector3.Magnitude(ray.v);
            rayEnumerator = new DiscreteRayParameterEnumerator(ray);

            Reset();
        }

        public CellRay Current => current;

        object IEnumerator.Current => Current;

        public void Dispose() { }

        public bool MoveNext()
        {
            var l_start = rayEnumerator.Current;
            var start = current.End;

            if (!rayEnumerator.MoveNext())
                return false;

            var l_end = rayEnumerator.Current;
            var end = ray.GetCoordinates(l_end);

            Point3 idx = (Point3)((start + end) / 2);
            float length = (l_end - l_start) * totalLength;

            current = new(idx, length, start, end);
            return true;
        }

        public void Reset()
        {
            rayEnumerator.Reset();
            rayEnumerator.MoveNext();
            var l = rayEnumerator.Current;
            current = new(Point3.zero, 0, Vector3.zero, ray.GetCoordinates(l));
        }
    }

    public class CellRay
    {
        public CellRay(Point3 idx, float length, Vector3 start, Vector3 end)
        {
            Idx = idx; Length = length; Start = start; End = end;
        }

        public Point3 Idx { get; private set; }
        public float Length { get; private set; }
        public Vector3 Start { get; private set; }
        public Vector3 End { get; private set; }
    }
}