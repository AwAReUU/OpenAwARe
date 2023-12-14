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
        /// <param name="cutTreshold">the minimum length a branch must have in pixels. shorter branches will be removed.</param>
        /// <param name="mergeTreshold">the second minimum length a branch must have in pixels. shorter branches will be merged if not removed.</param>
        public void PostFiltering(ref bool[,] grid, int cutTreshold, int mergeTreshold)
        {
            List<(int, int)> endPoints = new();
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
                        rolledOutNeighbours[8] = grid[x - 1, y - 1];    //add a copy of the first point looked at to the end
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

                    if (numberOfBranches == 1) endPoints.Add((x, y));
                    else if (numberOfBranches > 2) junctions.Add((x, y));
                }
            }

            List<List<(int x, int y)>> subPaths = new();

            //find the subpaths and remove any that are too short
            for (int i = 0; i < endPoints.Count; i++)
            {
                Queue<(int, int)> queue = new();
                queue.Enqueue(endPoints[i]);
                List<(int x, int y)> consideredPoints = new()
                {
                    endPoints[i]
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
                                for (int j = 0; j < addedPoints.Count; j++)
                                {
                                    consideredPoints.Remove(addedPoints[j]);
                                }

                                //add the junction to the subpath
                                consideredPoints.Add(consideredPoint);

                                stop = true;
                                break;
                            }

                            queue.Enqueue(consideredPoint);
                            consideredPoints.Add(consideredPoint);
                            addedPoints.Add(consideredPoint);
                        }
                    }
                }


                //if stop isn't true, we haven't found a junction and as such not a true subpath
                if (!stop) continue;

                //delete the found subpath / points if it is too short
                //length is detemerined using the euclidian distance between the endpoint and junction this line spans, since lines are mostly straight
                //if not deleted, put it in to list of subpaths to potentially merge
                float par1 = consideredPoints[^1].x - consideredPoints[0].x;
                float par2 = consideredPoints[^1].y - consideredPoints[0].y;
                double distance = Math.Sqrt((par1 * par1) + (par2 * par2));
                if (distance < cutTreshold)
                {
                    for (int j = 0; j < consideredPoints.Count; j++)
                    {
                        //do not cut away the junction, but do cut away all other points
                        if (junctions.Contains(consideredPoints[j])) continue;
                        grid[consideredPoints[j].x, consideredPoints[j].y] = false;
                    }
                }
                else
                {
                    subPaths.Add(consideredPoints);
                }
            }

            //find all subpaths that share a junction and merge them if they aren't long enough
            for (int i = 0; i < junctions.Count; i++)
            {
                List<List<(int x, int y)>> sharedPaths = new();

                //find all subpaths that have this junction
                for (int j = 0; j < subPaths.Count; j++)
                {
                    //the last point added to a subpath should be a junction, check if it is the current junction
                    if (subPaths[j][^1] == junctions[i])
                    {
                        //check if the path is short enough to be merged
                        //add it to the list of t-be merged paths if it is.
                        float par1 = subPaths[j][0].x - subPaths[j][^1].x;
                        float par2 = subPaths[j][0].y - subPaths[j][^1].y;
                        double distance = Math.Sqrt((par1 * par1) + (par2 * par2));
                        if (distance < mergeTreshold)
                        {
                            sharedPaths.Add(subPaths[j]);
                        }
                    }
                }

                //we cannot merge 0 or 1 paths.
                if (sharedPaths.Count <= 1) continue;

                //point to draw to is the average of endpoint of the subpaths.
                (int x, int y) secondPoint = (0, 0);
                for (int j = 0; j < sharedPaths.Count; j++)
                {
                    secondPoint.x += sharedPaths[j][0].x;
                    secondPoint.y += sharedPaths[j][0].y;
                }
                secondPoint.x /= sharedPaths.Count;
                secondPoint.y /= sharedPaths.Count;

                //make sure that we do not merge any lines that would end up in an invalid space
                // bool validpoint = true;
                // for (int j = 0; j < negativeGridLines.Count; j++)
                // {
                //     if (CheckInPolygon(negativeGridLines[j], secondpoint))
                //     {
                //         validpoint = false;
                //         break;
                //     }
                // }
                // if (!validpoint) continue;

                //remove the subpaths that are merged from the grid
                for (int j = 0; j < sharedPaths.Count; j++)
                {
                    for (int f = 0; f < sharedPaths[j].Count; f++)
                    {
                        //do not remove the junction, but do remove all other points
                        if (junctions.Contains(sharedPaths[j][f])) continue;
                        grid[sharedPaths[j][f].x, sharedPaths[j][f].y] = false;
                    }
                }

                //draw the new merged subpath
                LineDrawer.DrawLine(ref grid, (junctions[i], secondPoint));
            }
        }
    }
}