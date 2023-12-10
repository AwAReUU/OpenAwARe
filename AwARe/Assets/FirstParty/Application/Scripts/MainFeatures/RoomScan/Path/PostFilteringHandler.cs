// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

using System;
using System.Collections.Generic;

namespace AwARe.RoomScan.Path
{
    /// <summary>
    /// Handles everything regarding postfiltering of the bitmap representing the path.
    /// </summary>
    public class PostFilteringHandler
    {

        /// <summary>
        /// removes path branches that are too short.
        /// </summary>
        /// <param name="grid">the grid containing the skeleton path.</param>
        /// <param name="treshold">the minimum length a branch must have in pixels. shorter branches will be removed.</param>
        public void PostFiltering(ref bool[,] grid, int treshold)
        {
            List<(int, int)> endpoints = new();
            List<(int, int)> junctions = new();

            //find the endpoints and junctions
            for (int x = 0; x < grid.GetLength(0); x++)
            {
                for (int y = 0; y < grid.GetLength(1); y++)
                {
                    //if the pixel we are looking at is false, we do not need to potentially set it to false
                    if (!grid[x, y]) continue;

                    //'roll out' the gridpoints surrounding the current point
                    bool[] rolledOutNeighbours = new bool[9];

                    //look up-left
                    if (!(x - 1 < 0 || y - 1 < 0))
                    {
                        rolledOutNeighbours[0] = grid[x - 1, y - 1];
                        rolledOutNeighbours[8] = grid[x - 1, y - 1]; //add a copy of the first point looked at to the end
                    }

                    //look up
                    if (!(y - 1 < 0)) rolledOutNeighbours[1] = grid[x, y - 1];
                    //look up-right
                    if (!(x + 1 > grid.GetLength(0) || y - 1 < 0)) rolledOutNeighbours[2] = grid[x + 1, y - 1];
                    //look right
                    if (!(x + 1 > grid.GetLength(0))) rolledOutNeighbours[3] = grid[x + 1, y];
                    //look bottom-right
                    if (!(x + 1 > grid.GetLength(0) || y + 1 > grid.GetLength(1))) rolledOutNeighbours[4] = grid[x + 1, y + 1];
                    //look bottom
                    if (!(y + 1 > grid.GetLength(1))) rolledOutNeighbours[5] = grid[x, y + 1];
                    //look bottom-left
                    if (!(x - 1 < 0 || y + 1 > grid.GetLength(1))) rolledOutNeighbours[6] = grid[x - 1, y + 1];
                    //look left
                    if (!(x - 1 < 0)) rolledOutNeighbours[7] = grid[x - 1, y];

                    //count number branching paths coming from this pixel by looking at the number of changes from true to false
                    int numberOfBranches = 0;
                    for (int i = 0; i < 8; i++)
                    {
                        if (rolledOutNeighbours[i] && !rolledOutNeighbours[i + 1]) numberOfBranches++;
                    }

                    if (numberOfBranches == 1) endpoints.Add((x, y));
                    else if (numberOfBranches > 2) junctions.Add((x, y));
                }
            }

            //find the subpaths and remove any that are too short
            for (int i = 0; i < endpoints.Count; i++)
            {
                Queue<(int, int)> queue = new();
                queue.Enqueue(endpoints[i]);
                List<(int x, int y)> consideredPoints = new()
                {
                    endpoints[i]
                };
                bool stop = false;

                while (queue.Count > 0 && !stop)
                {
                    (int x, int y) currentPoint = queue.Dequeue();
                    List<(int x, int y)> addedPoints = new();

                    //consider points in the 8-neighbourhood
                    for (int x = -1; x < 2; x++)
                    {
                        if (stop) break;

                        for (int y = -1; y < 2; y++)
                        {
                            (int x, int y) consideredPoint = (currentPoint.x + x, currentPoint.y + y);

                            //various checks to see if the point is valid.
                            //checks if it is true, in bounds, not already considered and 
                            if (consideredPoint == currentPoint) continue;
                            if (consideredPoint.x < 0 || consideredPoint.x > grid.GetLength(0)
                                || consideredPoint.y < 0 || consideredPoint.y > grid.GetLength(1)) continue;
                            if (!grid[consideredPoint.x, consideredPoint.y]) continue;
                            if (consideredPoints.Contains(consideredPoint)) continue;

                            //if we reach a junction, the path is complete
                            if (junctions.Contains(consideredPoint))
                            {
                                //remove other points considered this round to avoid problems
                                for (int j = 0; j < addedPoints.Count; j++) { consideredPoints.Remove(addedPoints[j]); }

                                stop = true;
                                break;
                            }

                            queue.Enqueue(consideredPoint);
                            consideredPoints.Add(consideredPoint);
                            addedPoints.Add(consideredPoint);
                        }
                    }
                }

                //delete the found subpath / points als de length te klein is.
                //length is detemerined using the euclidian distance between de endpoint and junction this line spans, since lines are mostly straight

                float par1 = (consideredPoints[consideredPoints.Count - 1].x - consideredPoints[0].x);
                float par2 = (consideredPoints[consideredPoints.Count - 1].y - consideredPoints[0].y);
                double distance = Math.Sqrt((par1 * par1) + (par2 * par2));
                if (distance < treshold)
                {
                    for (int j = 0; j < consideredPoints.Count; j++) { grid[consideredPoints[j].x, consideredPoints[j].y] = false; }
                }
            }
        }
    }
}
