// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

using System;
using System.Collections.Generic;
//using AwARe.RoomScan.Path.Jobs;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace AwARe.RoomScan.Path
{
    /// <summary>
    /// Handles everything regarding the floodfill of the bitmap representing the room.
    /// </summary>
    public class FloodFillHandler
    {
        /// <summary>
        /// Fill an empty grid of booleans with the projection of the walkable space. these booleans are set to true.
        /// </summary>
        /// <param name="grid">the empty grid.</param>
        /// <param name="positiveLines">the line segments in 'grid space' making up the positive Polygon.</param>
        /// <param name="negativeLines">line segments in 'grid space' making up negative polygons.</param>
        public void FillGrid(ref bool[,] grid, List<((int, int), (int, int))> positiveLines, List<List<((int, int), (int, int))>> negativeLines)
        {
            //draw the lines for the positive Polygon
            for (int i = 0; i < positiveLines.Count; i++) { LineDrawer.DrawLine(ref grid, positiveLines[i]); }

            List<(int x, int y)> foundPoints = new();

            int rows = grid.GetLength(0);
            int cols = grid.GetLength(1);
            int gridSize = rows * cols;

            NativeArray<bool> resultGrid = new(gridSize, Allocator.TempJob);
            NativeArray<((int x, int y) p1, (int x, int y) p2)> polygonLines =
                new(positiveLines.Count, Allocator.TempJob);

            for (int i = 0; i < positiveLines.Count; i++) { polygonLines[i] = positiveLines[i]; }

            //CheckInPolygonJob positivePolygonCheckJob = new()
            //{
            //    nativeGrid = GridConverter.ToNativeGrid(grid),
            //    columns = grid.GetLength(1),
            //    checkPositivePolygon = true,
            //    polygonWalls = polygonLines,

            //    //nativeResultGrid = resultGrid
            //    result = resultGrid
            //};

            //JobHandle posPolCheckJobHandle = positivePolygonCheckJob.Schedule(gridSize, 64);

            //posPolCheckJobHandle.Complete();

            polygonLines.Dispose();

            for (int i = 0; i < resultGrid.Length; i++)
            {
                int x = i / cols;
                int y = i % cols;
                if (resultGrid[i])
                    foundPoints.Add((x, y));
            }

            resultGrid.Dispose();

            //fill in the positive Polygon
            for (int i = 0; i < foundPoints.Count; i++)
            {
                //grid[foundPoints[i].x, foundPoints[i].y] = true;
                FloodArea(ref grid, (foundPoints[i].x, foundPoints[i].y));
            }

            //carve out the negative polygons
            for (int n = 0; n < negativeLines.Count; n++)
            {
                foundPoints = new();

                //carve out the lines for the current negative Polygon
                for (int i = 0; i < negativeLines[n].Count; i++) { LineDrawer.DrawLine(ref grid, negativeLines[n][i], true); }

                //find the points in the current negative Polygon
                for (int x = 0; x < grid.GetLength(0); x++)
                {
                    for (int y = 0; y < grid.GetLength(1); y++)
                    {
                        //it is useless to set a point that is already false to false
                        if (!grid[x, y]) continue;

                        //check if current point is in the negative Polygon. if not, continue
                        if (!CheckInPolygon(negativeLines[n], (x, y))) continue;

                        //if the loop makes it past all of the above checks, we have found a valid point
                        foundPoints.Add((x, y));
                    }
                }

                //erase the points that lie in the negative Polygon
                for (int i = 0; i < foundPoints.Count; i++)
                {
                    //grid[foundPoints[i].x, foundPoints[i].y] = false;
                    FloodArea(ref grid, (foundPoints[i].x, foundPoints[i].y), true);
                }
            }
        }

        /// <summary>
        /// check if a point is in a Polygon (represented as a list of lines)
        /// done by shooting a ray to the right from the point and counting the number of intersections with Polygon edges.
        /// </summary>
        /// <param name="polygonWalls">List of lines that make up the Polygon.</param>
        /// <param name="point">Point to check if it is inside the Polygon.</param>
        /// <returns>True if the point lies inside the Polygon, false otherwise.</returns>
        private bool CheckInPolygon(List<((int x, int y) p1, (int x, int y) p2)> polygonWalls, (int x, int y) point)
        {
            List<(double x, double y)> intersections = new();

            for (int i = 0; i < polygonWalls.Count; i++)
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
                //we cannot be sure if it lies inside or outside the Polygon
                if ((intersectx, intersecty) == polygonWalls[i].p1 || (intersectx, intersecty) == polygonWalls[i].p2) { return false; }

                //add this intersection to the list if it is a new one
                if (!intersections.Contains((intersectx, intersecty))) { intersections.Add((intersectx, intersecty)); }
            }

            if (intersections.Count % 2 == 0) { return false; }
            else return true;
        }

        //floodfill is used to fill in the positive or carve out the negative polygons
        #region floodfill

        /// <summary>
        /// flood an area of the grid with 'true' from a given start position. 
        /// it is assumed that there is a boundary of 'true' values surrounding the startpoint
        /// if there isn't, the entire grid will be flooded with true.
        /// </summary>
        /// <param name="grid">the array of booleans to flood.</param>
        /// <param name="startpos">the position in the grid to start from.</param>
        /// <param name="reverse">if true, will 'reverse' floodfill. instead of filling areas with true, fills them with false.</param>
        private void FloodArea(ref bool[,] grid, (int, int) startpos, bool reverse = false)
        {
            int width = grid.GetLength(0);
            int height = grid.GetLength(1);

            Queue<(int, int)> queue = new();
            queue.Enqueue(startpos);

            while (queue.Count > 0)
            {
                (int x, int y) current = queue.Dequeue();
                if ((reverse && grid[current.x, current.y]) || (!reverse && !grid[current.x, current.y]))
                {
                    grid[current.x, current.y] = !grid[current.x, current.y];
                    EnqueueNeighbors(ref queue, current, width, height);
                }
            }
        }

        /// <summary>
        /// enqueue the 4 neighbours of a given position into the given queue if possible.
        /// </summary>
        /// <param name="queue">the queue to put neighbours in.</param>
        /// <param name="pos">the position to get neighbours from.</param>
        /// <param name="width">the maximum width of the grid. neighbour positions beyond this poitn will not be enqueued.</param>
        /// <param name="height">the maximum height of the grid. neighbour positions beyond this point will not be enqueued.</param>
        private void EnqueueNeighbors(ref Queue<(int, int)> queue, (int x, int y) pos, int width, int height)
        {
            //enqueue right
            if (pos.x + 1 >= 0 && pos.x + 1 < width && pos.y >= 0 && pos.y < height)
                queue.Enqueue((pos.x + 1, pos.y));
            //enqueue bottom
            if (pos.x >= 0 && pos.x < width && pos.y + 1 >= 0 && pos.y + 1 < height)
                queue.Enqueue((pos.x, pos.y + 1));
            //enqueue left
            if (pos.x - 1 >= 0 && pos.x - 1 < width && pos.y >= 0 && pos.y < height)
                queue.Enqueue((pos.x - 1, pos.y));
            //enqueue top
            if (pos.x >= 0 && pos.x < width && pos.y - 1 >= 0 && pos.y - 1 < height)
                queue.Enqueue((pos.x, pos.y - 1));
        }

        #endregion
    }
}
