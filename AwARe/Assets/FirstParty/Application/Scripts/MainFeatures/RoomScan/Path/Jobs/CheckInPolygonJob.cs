// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

using System;
using System.Diagnostics.CodeAnalysis;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
namespace AwARe.RoomScan.Path.Jobs
{
    /// <summary>
    /// Job for checking whether a point is inside the polygon.
    /// </summary>
    [BurstCompile]
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
        [ExcludeFromCodeCoverage]
        public void Execute(int index)
        {
            if (nativeGrid[index] == checkPositivePolygon) return;
            int x = index / columns;
            int y = index % columns;
            if (CheckInPolygon(polygonWalls, (x, y)) == false) return;

            result[index] = true;
        }

        /// <summary>
        /// check if a point is in a polygon (represented as a list of lines)
        /// done by shooting a ray to the right from the point and counting the number of intersections with polygon edges.
        /// </summary>
        /// <param name="polygonWalls">List of lines that make up the polygon.</param>
        /// <param name="point">point to check if it is inside the polygon.</param>
        /// <returns>true if the point lies inside the polygon, false otherwise.</returns>
        public readonly bool CheckInPolygon(
            NativeArray<((int x, int y) p1, (int x, int y) p2)> polygonWalls, 
            (int x, int y) point)
        {
            int numberOfIntersections = 0;

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

                    //rounding to integer in the same way DrawLine does it
                    double preintersectx = (point.y - b) / a;
                    if (preintersectx - (int)preintersectx == 0.5) return false;
                    if (preintersectx - (int)preintersectx > 0.5) intersectx = (int)(preintersectx + 1);
                    else intersectx = (int)preintersectx;
                }

                //if the intersection point is the point, we have a point that lies exactly on the polygon edge. this should've been prevented already.
                if (intersectx == point.x) return false;
                
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

                numberOfIntersections++;
            }

            return numberOfIntersections % 2 != 0;
        }
    }
}