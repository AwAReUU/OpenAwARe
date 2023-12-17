//// /*                                                                                       *\
////     This program has been developed by students from the bachelor Computer Science at
////     Utrecht University within the Software Project course.
////
////     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
//// \*                                                                                       */

//using System;
//using System.Collections.Generic;
//using System.Linq;

//using Unity.Collections;
//using Unity.Jobs;
//using Unity.Burst;
//namespace AwARe.RoomScan.Path.Jobs
//{
//    /// <summary>
//    /// Job for checking whether a point is inside the Polygon.
//    /// </summary>
//    [BurstCompile]
//    public struct CheckInPolygonJob : IJobParallelFor
//    {
//        /// <summary> The input array. </summary>
//        [ReadOnly] public NativeArray<bool> nativeGrid;

//        /// <summary> The number of columns of the grid. </summary>
//        [ReadOnly] public int columns;

//        /// <summary> Whether this check is for the positive Polygon. </summary>
//        [ReadOnly] public bool checkPositivePolygon;

//        /// <summary> The lines of the Polygon, represented by start- and endpoints. </summary>
//        [ReadOnly] public NativeArray<((int x, int y) p1, (int x, int y) p2)> polygonWalls;

//        /// <summary> The resulting array. </summary>
//        [WriteOnly] public NativeArray<bool> result;

//        /// <inheritdoc/>
//        public void Execute(int index)
//        {
//            if (nativeGrid[index] == checkPositivePolygon) return;
//            int x = index / columns;
//            int y = index % columns;
//            if (CheckInPolygon(polygonWalls, (x, y)) == false) return;

//            result[index] = true;
//        }

//        /// <summary>
//        /// check if a point is in a Polygon (represented as a list of lines)
//        /// done by shooting a ray to the right from the point and counting the number of intersections with Polygon edges.
//        /// </summary>
//        /// <param name="polygonWalls">List of lines that make up the Polygon.</param>
//        /// <param name="point">point to check if it is inside the Polygon.</param>
//        /// <returns>true if the point lies inside the Polygon, false otherwise.</returns>
//        private readonly bool CheckInPolygon(NativeArray<((int x, int y) p1, (int x, int y) p2)> polygonWalls, (int x, int y) point)
//        {
//            NativeArray<(double x, double y)> intersections = new(polygonWalls.Length, Allocator.Temp);

//            int numberOfIntersections = 0;

//            for (int i = 0; i < polygonWalls.Length; i++)
//            {
//                double intersecty = point.y;
//                double intersectx;

//                double divider = polygonWalls[i].p2.x - polygonWalls[i].p1.x;
//                if (divider == 0) { intersectx = polygonWalls[i].p2.x; }
//                else
//                {
//                    double a = (polygonWalls[i].p2.y - polygonWalls[i].p1.y) / divider;
//                    //if a is 0, the ray and the wall are parallel and they dont intersect
//                    if (a == 0) continue;
//                    double b = polygonWalls[i].p1.y - polygonWalls[i].p1.x * a;

//                    //rounding to integer in the same way DrawLine does it
//                    double preintersectx = (point.y - b) / a;
//                    if (preintersectx - (int)preintersectx > 0.5) intersectx = (int)(preintersectx + 1);
//                    else intersectx = (int)preintersectx;
//                }

//                //check that the intersection point lies on the ray we shot, continue if it doesn't
//                if (intersectx <= point.x) continue;

//                //check that the intersection point lies on the wall, continue if it doesn't
//                if (intersectx < Math.Min(polygonWalls[i].p1.x, polygonWalls[i].p2.x) ||
//                    intersectx > Math.Max(polygonWalls[i].p1.x, polygonWalls[i].p2.x)
//                    || intersecty < Math.Min(polygonWalls[i].p1.y, polygonWalls[i].p2.y) ||
//                    intersecty > Math.Max(polygonWalls[i].p1.y, polygonWalls[i].p2.y)) { continue; }

//                //if the intersection point is the exact endpoint of a wall, this causes problems. cancel the whole operation
//                //we cannot be sure if it lies inside or outside the Polygon
//                if ((intersectx, intersecty) == polygonWalls[i].p1 || (intersectx, intersecty) == polygonWalls[i].p2) { return false; }

//                //add this intersection to the list if it is a new one
//                if (NotInIntersections((intersectx, intersecty), intersections))
//                {
//                    intersections[numberOfIntersections] = (intersectx, intersecty);
//                    numberOfIntersections++;
//                }
//            }

//            bool uneven = numberOfIntersections % 2 != 0;
//            intersections.Dispose();
//            return uneven;
//        }

//        /// <summary>
//        /// Checks whether the intersection point is already present in the array of intersections.
//        /// </summary>
//        /// <param name="intersection">The point to check.</param>
//        /// <param name="intersections">The previously found intersections.</param>
//        /// <returns>Whether the intersection point is already present in the array of intersections.</returns>
//        private readonly bool NotInIntersections((double x, double y) intersection, NativeArray<(double x, double y)> intersections)
//        {
//            foreach (var i in intersections)
//            {
//                if (i == intersection)
//                    return false;
//            }
//            return true;
//        }
//    }
//}