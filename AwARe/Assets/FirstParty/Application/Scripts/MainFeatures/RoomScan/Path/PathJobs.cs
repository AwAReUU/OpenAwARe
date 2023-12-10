using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Windows;


namespace AwARe.RoomScan.Path
{
    public struct CheckInPolygonJob : IJobParallelFor
    {
        [ReadOnly] public NativeArray<bool> nativeGrid;
        [ReadOnly] public int columns;
        [ReadOnly] public bool checkPositivePolygon;
        [ReadOnly] public NativeArray<((int x, int y) p1, (int x, int y) p2)> polygonWalls;

        //[WriteOnly] public NativeArray<bool> nativeResultGrid;
        [WriteOnly] public NativeArray<bool> result;

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
        /// done by shooting a ray to the right from the point and counting the number of intersections with polygon edges
        /// </summary>
        /// <param name="lines">list of lines that make up the polygon</param>
        /// <param name="point">point to check if it is inside the polygon</param>
        /// <returns>true if the point lies inside the polygon, false otherwise</returns>
        private bool CheckInPolygon(NativeArray<((int x, int y) p1, (int x, int y) p2)> polygonwalls, (int x, int y) point)
        {
            List<(double x, double y)> intersections = new();

            for (int i = 0; i < polygonwalls.Length; i++)
            {
                double intersecty = point.y;
                double intersectx;

                double divider = polygonwalls[i].p2.x - polygonwalls[i].p1.x;
                if (divider == 0)
                {
                    intersectx = polygonwalls[i].p2.x;
                }
                else
                {
                    double a = (polygonwalls[i].p2.y - polygonwalls[i].p1.y) / divider;
                    //if a is 0, the ray and the wall are parallel and they dont intersect
                    if (a == 0) continue;
                    double b = polygonwalls[i].p1.y - polygonwalls[i].p1.x * a;
                    intersectx = (int)Math.Round((point.y - b) / a);
                }
                //check that the intersection point lies on the ray we shot, continue if it doesn't
                if (intersectx < point.x) continue;

                //check that the intersection point lies on the wall, continue if it doesn't
                if (intersectx < Math.Min(polygonwalls[i].p1.x, polygonwalls[i].p2.x) || intersectx > Math.Max(polygonwalls[i].p1.x, polygonwalls[i].p2.x)
                 || intersecty < Math.Min(polygonwalls[i].p1.y, polygonwalls[i].p2.y) || intersecty > Math.Max(polygonwalls[i].p1.y, polygonwalls[i].p2.y)) { continue; }

                //if the intersection point is the exact endpoint of a wall, this causes problems. cancel the whole operation
                //we cannot be sure if it lies inside or outside the polygon
                if ((intersectx, intersecty) == polygonwalls[i].p1 || (intersectx, intersecty) == polygonwalls[i].p2)
                {
                    return false;
                }

                //add this intersection to the list if it is a new one
                if (!intersections.Contains((intersectx, intersecty)))
                {
                    intersections.Add((intersectx, intersecty));
                }
            }

            return intersections.Count % 2 != 0;
        }
    }

    //[Unity.Burst.BurstCompile]
    public struct CheckHitOrMissJob : IJobParallelFor
    {
        [ReadOnly] public NativeArray<bool> nativeGrid;
        [ReadOnly] public int columns;
        [ReadOnly] public int rows;
        [ReadOnly] public NativeArray<bool> frontElement;
        [ReadOnly] public NativeArray<bool> backElement;
        [ReadOnly] public int elementLength;

        //[WriteOnly] public NativeArray<bool> nativeResultGrid;
        [WriteOnly] public NativeArray<bool> result;

        public void Execute(int index)
        {
            if (!nativeGrid[index])
            {
                result[index] = false;
                return;
            }

            int x = index / columns;
            int y = index % columns;
            if (CheckHitOrMiss(x, y))
            {
                result[index] = false;
            }
            else
            {
                result[index] = nativeGrid[index];
            }
        }

        /// <summary>
        /// check wether a given position on the grid is a hit or a miss for the thinning operation
        /// uses the 'L Golay' structuring elements to check this.
        /// </summary>
        /// <param name="grid">the grid to check in</param>
        /// <param name="x">the x position of the point in the grid to check</param>
        /// <param name="y">the y position of the point in the grid to check</param>
        /// <returns></returns>
        public bool CheckHitOrMiss(int x, int y)
        {
            int offset = elementLength / 2;

            bool hit = true;
            for (int a = 0; a < elementLength; a++)
            {
                if (!hit) break;

                for (int b = 0; b < elementLength; b++)
                {
                    //if frontelement is true, the grid element at this position must also be true for it to be a hit
                    //if frontelement is false the grid element at this position may be true or false
                    //if backelement is true, the grid element at this position must be false for it to be a hit
                    //if backelement is false the grid element at this position may be true or false

                    int index = a * elementLength + b;

                    //the position falls outside of the grid and is treated as if the grid there is false
                    if (x - offset + a < 0 || x - offset + a > rows - 1 ||
                       y - offset + b < 0 || y - offset + b > columns - 1)
                    {
                        //the frontelement check
                        if (frontElement[index])
                        {
                            hit = false;
                            break;
                        }

                        //since this place falls outside of the grid and is considered false, it always falls in the background element
                        //thus we do not need to perform the background element check, since it will always succeed
                    }
                    else
                    {
                        bool posValue = nativeGrid[(x - offset + a) * columns + (y - offset + b)];

                        //the front element check
                        if (frontElement[index] && !posValue)
                        {
                            hit = false;
                            break;
                        }

                        //the back element check
                        if (backElement[index] && posValue)
                        {
                            hit = false;
                            break;
                        }
                    }

                }
            }

            return hit;
        }
    }

    public struct ErosionScanJob : IJobParallelFor
    {
        [ReadOnly] public NativeArray<bool> input;
        [ReadOnly] public int elemSize;
        [ReadOnly] public int rows;
        [ReadOnly] public int columns;
        [ReadOnly] public int elemDiv;

        //[WriteOnly] public NativeArray<bool> nativeResultGrid;
        [WriteOnly] public NativeArray<bool> result;

        public void Execute(int index)
        {
            Func<(int, int), bool> inputFunction = GetInputFunction(input, rows, columns);

            // Perform function on cell
            result[index] = All(index, inputFunction);
        }

        /// <summary>
        /// Checks whether all elements in the input are true.
        /// </summary>
        /// <param name="input">A 2D boolean array.</param>
        /// <returns>Whether all elements in the input array are true.</returns>
        private bool All(int index, Func<(int, int), bool> inputFunction)
        {
            int x = index / columns;
            int y = index % columns;

            // If one element is false, return false, otherwise return true.
            for (int i = 0, x_2 = x - elemDiv; i < elemSize; i++, x_2++)
                for (int j = 0, y_2 = y - elemDiv; j < elemSize; j++, y_2++)
                {
                    if(!inputFunction((x_2, y_2)))
                        return false;
                }
            return true;
        }

        /// <summary>
        /// Uses an array to create a function for determining the value of a bit in the bitmap.
        /// </summary>
        /// <param name="input">The array.</param>
        /// <returns>A function that will return false if the values are outside the boundary, or returns the value otherwise.</returns>
        private Func<(int, int), bool> GetInputFunction(NativeArray<bool> input, int rows, int columns)
        {
            return ((int, int) xy) =>
            {
                (int x, int y) = xy;
                bool inBounds = (x >= 0) && (x < rows) && (y >= 0) && (y < columns);
                return inBounds && input[x * columns + y];
            };
        }
    }
}