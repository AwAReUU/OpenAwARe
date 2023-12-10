// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;


namespace AwARe.RoomScan.Path.Jobs
{
    //[Unity.Burst.BurstCompile]
    /// <summary>
    /// Job for checking whether a point is inside the polygon.
    /// </summary>
    public struct CheckInPolygonJob : IJobParallelFor
    {
        /// <summary> The input array. </summary>
        [ReadOnly] public NativeArray<bool> nativeGrid;

        /// <summary> The number of columns of the grid. </summary>
        [ReadOnly] public int columns;

        /// <summary> Whether this check is for the positive polygon. </summary>
        [ReadOnly] public bool checkPositivePolygon;

        /// <summary> The lines of the polygon, represented by start- and endpoints. </summary>
        [ReadOnly] public NativeArray<((int x, int y) p1, (int x, int y) p2)> polygonWalls;

        /// <summary> The resulting array. </summary>
        [WriteOnly] public NativeArray<bool> result;

        /// <inheritdoc/>
        public void Execute(int index)
        {
            if (nativeGrid[index] == checkPositivePolygon) return;
            int x = index / columns;
            int y = index % columns;
            if (CheckInPolygon(polygonWalls, (x, y)) == false) return;

            //nativeResultGrid[index] = checkPositivePolygon;
            result[index] = true;
        }

        /// <summary>
        /// check if a point is in a polygon (represented as a list of lines)
        /// done by shooting a ray to the right from the point and counting the number of intersections with polygon edges.
        /// </summary>
        /// <param name="polygonWalls">List of lines that make up the polygon.</param>
        /// <param name="point">point to check if it is inside the polygon.</param>
        /// <returns>true if the point lies inside the polygon, false otherwise.</returns>
        private readonly bool CheckInPolygon(NativeArray<((int x, int y) p1, (int x, int y) p2)> polygonWalls, (int x, int y) point)
        {
            List<(double x, double y)> intersections = new();

            for (int i = 0; i < polygonWalls.Length; i++)
            {
                double intersecty = point.y;
                double intersectx;

                double divider = polygonWalls[i].p2.x - polygonWalls[i].p1.x;
                if (divider == 0) { intersectx = polygonWalls[i].p2.x; }
                else
                {
                    double a = (polygonWalls[i].p2.y - polygonWalls[i].p1.y) / divider;
                    //if a is 0, the ray and the wall are parallel and they dont intersect
                    if (a == 0) continue;
                    double b = polygonWalls[i].p1.y - polygonWalls[i].p1.x * a;
                    intersectx = (int)Math.Round((point.y - b) / a);
                }

                //check that the intersection point lies on the ray we shot, continue if it doesn't
                if (intersectx < point.x) continue;

                //check that the intersection point lies on the wall, continue if it doesn't
                if (intersectx < Math.Min(polygonWalls[i].p1.x, polygonWalls[i].p2.x) ||
                    intersectx > Math.Max(polygonWalls[i].p1.x, polygonWalls[i].p2.x)
                    || intersecty < Math.Min(polygonWalls[i].p1.y, polygonWalls[i].p2.y) ||
                    intersecty > Math.Max(polygonWalls[i].p1.y, polygonWalls[i].p2.y)) { continue; }

                //if the intersection point is the exact endpoint of a wall, this causes problems. cancel the whole operation
                //we cannot be sure if it lies inside or outside the polygon
                if ((intersectx, intersecty) == polygonWalls[i].p1 || (intersectx, intersecty) == polygonWalls[i].p2) { return false; }

                //add this intersection to the list if it is a new one
                if (!intersections.Contains((intersectx, intersecty))) { intersections.Add((intersectx, intersecty)); }
            }

            return intersections.Count % 2 != 0;
        }
    }
}