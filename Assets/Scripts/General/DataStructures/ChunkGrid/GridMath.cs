using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

using AwARe.DataTypes;
using Unity.VisualScripting;
using System.Collections;

namespace AwARe.DataStructures
{
    public static class GridMath
    {



    }

    public class GridRay : IEnumerable<Point3>
    {
        private const float eps = 0.00001f;

        readonly Vector3 start;
        readonly Vector3 end;

        readonly Vector3 v;
        readonly Vector3 d;

        readonly Point3 incr;

        public GridRay(Vector3 start, Vector3 end, Point3 gridSize)
        {
            (this.start, this.end) = FitRayInGrid(start, end, gridSize);
            (v, d) = Parametrize(start, end);
            incr = GetIdxIncrement(v);
        }

        public static (Vector3, Vector3) Parametrize(Vector3 start, Vector3 end) =>
            (end - start, start);

        public static (Vector3, Vector3) FitRayInGrid(Vector3 start, Vector3 end, Point3 gridSize) =>
            FitRayInBox(start, end, Point3.zero, gridSize);

        public static (Vector3, Vector3) FitRayInBox(Vector3 start, Vector3 end, Vector3 min, Vector3 max)
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

            // Get the new start and end coordinates of the ray, which lay in bounds.
            var g = GetParametrization(v, d);
            start = g(start_l); end = g(end_l);
            return (start, end);
        }

        public static Func<NullableVector3, NullableVector3> GetParametersFunction(Vector3 v, Vector3 d) =>
            coords => (coords - d) / v;

        public NullableVector3 GetPossibleParameters(NullableVector3 coordinates) =>
            GetParametersFunction(v, d)(coordinates);

        public static Func<float,Vector3> GetParametrization(Vector3 v, Vector3 d) =>
            l => v * l +d;

        public Vector3 GetCoordinates(float l) =>
            GetParametrization(v, d)(l);

        public Point3 GetIdxIncrement(Vector3 v) =>
            (Point3)(new Vector3(Mathf.Sign(v.x), Mathf.Sign(v.y), Mathf.Sign(v.z)));

        public (float, Point3) NextCellAndParameter((float, Point3) current)
        {
            (float current_l, Point3 current_idx) = current;

            // Get next parameter
            var ls = GetPossibleParameters(current_idx + incr);
            var next_l = NullableVector3.MinEl(ls, out int arg);

            // Get next index
            var next_idx = current_idx;
            next_idx[arg] += incr[arg];

            return(next_l, next_idx);
        }

        public bool IsPastEnd(float current_l) =>
            current_l > 0;

        public IEnumerator<Point3> GetEnumerator() =>
            new RayEnumarator(this);

        IEnumerator IEnumerable.GetEnumerator() =>
            GetEnumerator();

        private class RayEnumarator : IEnumerator<Point3>
        {
            private Point3 current_idx = Point3.zero;
            private float current_l = -1;
            private GridRay ray;

            public RayEnumarator(GridRay ray)
            {
                this.ray = ray;
            }

            public Point3 Current => current_idx;

            object IEnumerator.Current => Current;

            public void Dispose() { }

            public bool MoveNext()
            {
                throw new NotImplementedException();

                /* 
                if (current_l == -1)
                { current_l = 0; current_idx = ray.GetCoordinates(current_l); return true; }

                if (ray.IsPastEnd(current_l))
                    return false;

                current_l = ray.Get
                */
            }

            public void Reset() => current_l = -1;
        }
    }
}
