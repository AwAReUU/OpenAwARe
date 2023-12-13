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
        public void PostFiltering(ref bool[,] grid, List<List<((int, int), (int, int))>> negativeGridLines, int cutTreshold, int mergeTreshold)
        {
            List<(int, int)> endpoints = new List<(int, int)>();
            List<(int, int)> junctions = new List<(int, int)>();

            //find the endpoints and junctions
            for (int x = 0; x < grid.GetLength(0); x++)
            {
                for (int y = 0; y < grid.GetLength(1); y++)
                {
                    //if the pixel we are looking at is false, we do not need to potentially set it to false
                    if (!grid[x, y]) continue;

                    //'roll out' the gridpoints surrounding the current point
                    bool[] rolledoutneighbours = new bool[9];

                    //look up-left
                    if (!(x - 1 < 0 || y - 1 < 0))
                    {
                        rolledoutneighbours[0] = grid[x - 1, y - 1];
                        rolledoutneighbours[8] = grid[x - 1, y - 1];    //add a copy of the first point looked at to the end
                    }
                    //look up
                    if (!(y - 1 < 0)) rolledoutneighbours[1] = grid[x, y - 1];
                    //look up-right
                    if (!(x + 1 > grid.GetLength(0) || y - 1 < 0)) rolledoutneighbours[2] = grid[x + 1, y - 1];
                    //look right
                    if (!(x + 1 > grid.GetLength(0))) rolledoutneighbours[3] = grid[x + 1, y];
                    //look bottom-right
                    if (!(x + 1 > grid.GetLength(0) || y + 1 > grid.GetLength(1))) rolledoutneighbours[4] = grid[x + 1, y + 1];
                    //look bottom
                    if (!(y + 1 > grid.GetLength(1))) rolledoutneighbours[5] = grid[x, y + 1];
                    //look bottom-left
                    if (!(x - 1 < 0 || y + 1 > grid.GetLength(1))) rolledoutneighbours[6] = grid[x - 1, y + 1];
                    //look left
                    if (!(x - 1 < 0)) rolledoutneighbours[7] = grid[x - 1, y];

                    //count number branching paths coming from this pixel by looking at the number of changes from true to false
                    int numberofbranches = 0;
                    for (int i = 0; i < 8; i++)
                    {
                        if (rolledoutneighbours[i] && !rolledoutneighbours[i + 1]) numberofbranches++;
                    }

                    if (numberofbranches == 1) endpoints.Add((x, y));
                    else if (numberofbranches > 2) junctions.Add((x, y));
                }
            }

            List<List<(int x, int y)>> subpaths = new List<List<(int x, int y)>>();

            //find the subpaths and remove any that are too short
            for (int i = 0; i < endpoints.Count; i++)
            {
                Queue<(int, int)> queue = new Queue<(int, int)>();
                queue.Enqueue(endpoints[i]);
                List<(int x, int y)> consideredpoints = new List<(int x, int y)>();
                consideredpoints.Add(endpoints[i]);
                bool stop = false;

                while (queue.Count > 0 && !stop)
                {
                    (int x, int y) currentpoint = queue.Dequeue();
                    List<(int x, int y)> addedpoints = new List<(int x, int y)>();

                    //consider points in the 8-neighbourhood
                    for (int x = -1; x < 2; x++)
                    {
                        if (stop) break;

                        for (int y = -1; y < 2; y++)
                        {
                            (int x, int y) consideredpoint = (currentpoint.x + x, currentpoint.y + y);

                            //various checks to see if the point is valid.
                            //checks if it is true, in bounds, not already considered and 
                            if (consideredpoint == currentpoint) continue;
                            if (consideredpoint.x < 0 || consideredpoint.x > grid.GetLength(0)
                            || consideredpoint.y < 0 || consideredpoint.y > grid.GetLength(1)) continue;
                            if (!grid[consideredpoint.x, consideredpoint.y]) continue;
                            if (consideredpoints.Contains(consideredpoint)) continue;

                            //if we reach a junction, the path is complete
                            if (junctions.Contains(consideredpoint))
                            {
                                //remove other points considered this round to avoid problems
                                for (int j = 0; j < addedpoints.Count; j++)
                                {
                                    consideredpoints.Remove(addedpoints[j]);
                                }

                                //add the junction to the subpath
                                consideredpoints.Add(consideredpoint);

                                stop = true;
                                break;
                            }

                            queue.Enqueue(consideredpoint);
                            consideredpoints.Add(consideredpoint);
                            addedpoints.Add(consideredpoint);
                        }
                    }
                }


                //if stop isn't true, we haven't found a junction and as such not a true subpath
                if (!stop) continue;

                //delete the found subpath / points if it is too short
                //length is detemerined using the euclidian distance between the endpoint and junction this line spans, since lines are mostly straight
                //if not deleted, put it in to list of subpaths to potentially merge
                float par1 = consideredpoints[consideredpoints.Count - 1].x - consideredpoints[0].x;
                float par2 = consideredpoints[consideredpoints.Count - 1].y - consideredpoints[0].y;
                double distance = Math.Sqrt((par1 * par1) + (par2 * par2));
                if (distance < cutTreshold)
                {
                    for (int j = 0; j < consideredpoints.Count; j++)
                    {
                        //do not cut away the junction, but do cut away all other points
                        if (junctions.Contains(consideredpoints[j])) continue;
                        grid[consideredpoints[j].x, consideredpoints[j].y] = false;
                    }
                }
                else
                {
                    subpaths.Add(consideredpoints);
                }
            }

            //find all subpaths that share a junction and merge them if they aren't long enough
            for (int i = 0; i < junctions.Count; i++)
            {
                List<List<(int x, int y)>> sharedpaths = new List<List<(int x, int y)>>();

                //find all subpaths that have this junction
                for (int j = 0; j < subpaths.Count; j++)
                {
                    //the last point added to a subpath should be a junction, check if it is the current junction
                    if (subpaths[j][subpaths[j].Count - 1] == junctions[i])
                    {
                        //check if the path is short enough to be merged
                        //add it to the list of t-be merged paths if it is.
                        float par1 = subpaths[j][0].x - subpaths[j][subpaths[j].Count - 1].x;
                        float par2 = subpaths[j][0].y - subpaths[j][subpaths[j].Count - 1].y;
                        double distance = Math.Sqrt((par1 * par1) + (par2 * par2));
                        if (distance < mergeTreshold)
                        {
                            sharedpaths.Add(subpaths[j]);
                        }
                    }
                }

                //we cannot merge 0 or 1 paths.
                if (sharedpaths.Count <= 1) continue;

                //point to draw to is the average of endpoint of the subpaths.
                (int x, int y) secondpoint = (0, 0);
                for (int j = 0; j < sharedpaths.Count; j++)
                {
                    secondpoint.x += sharedpaths[j][0].x;
                    secondpoint.y += sharedpaths[j][0].y;
                }
                secondpoint.x = secondpoint.x / sharedpaths.Count;
                secondpoint.y = secondpoint.y / sharedpaths.Count;

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
                for (int j = 0; j < sharedpaths.Count; j++)
                {
                    for (int f = 0; f < sharedpaths[j].Count; f++)
                    {
                        //do not remove the junction, but do remove all other points
                        if (junctions.Contains(sharedpaths[j][f])) continue;
                        grid[sharedpaths[j][f].x, sharedpaths[j][f].y] = false;
                    }
                }

                //draw the new merged subpath
                LineDrawer.DrawLine(ref grid, (junctions[i], secondpoint));
            }
        }
    }
}