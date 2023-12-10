// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

using System;
using System.Collections.Generic;
using System.Linq;


using Unity.Collections;
using Unity.Jobs;
using Unity.Burst;

using AwARe.Data.Logic;
using UnityEngine;
using System.Collections.Concurrent;
using System.Data;
using System.Threading.Tasks;

namespace AwARe.RoomScan.Path
{

    /// <summary>
    /// Class that contains methods for generating a decent startstate for the path
    /// </summary>
    public class StartState
    {
        float scaleFactor;
        (float, float) moveTransform;
        float averageHeight;

        List<bool[,]> frontGolayElements = new();
        List<bool[,]> backGolayElements = new();

        private float startTime;

        /// <summary>
        /// Create a path from a given positive polygon and list of negative polygons that represent the room
        /// </summary>
        /// <param name="positive">the polygon whose volume represents the area where you can walk</param>
        /// <param name="negatives">list of polygons whose volume represent the area where you cannot walk</param>
        /// <returns>a 'pathdata' which represents a path through the room</returns>
        public PathData GetStartState(Polygon positive, List<Polygon> negatives)
        {
            startTime = Time.realtimeSinceStartup;

            //determine the grid. still empty. also initalizes the scalefactor and movetransform variables.
            bool[,] grid = MakeGrid(positive);

            for (int i = 0; i < positive.GetPoints().Length; i++)
            {
                Debug.Log("Point " + i + ": " + positive.GetPoints()[i].x + ", 0, " + positive.GetPoints()[i].z);
            }

            List<((int, int), (int, int))> positiveGridLines = new();
            List<List<((int, int), (int, int))>> negativeGridLines = new();

            //determine all line segments in polygon space and grid space
            List<(Vector3, Vector3)> positiveLines = GenerateLines(positive);
            List<List<(Vector3, Vector3)>> negativeLines = new();

            //convert the positive lines to grid space
            for (int i = 0; i < positiveLines.Count; i++)
            {
                positiveGridLines.Add((ToGridSpace(positiveLines[i].Item1), ToGridSpace(positiveLines[i].Item2)));
            }

            //convert the negative lines to grid space
            for (int i = 0; i < negatives.Count; i++)
            {
                List<(Vector3, Vector3)> negativeLinesPart = GenerateLines(negatives[i]);
                negativeLines.Add(negativeLinesPart);

                List<((int, int), (int, int))> negativeGridLinesPart = new();
                for (int j = 0; j < negativeLinesPart.Count; j++)
                {
                    negativeGridLinesPart.Add((ToGridSpace(negativeLinesPart[j].Item1), ToGridSpace(negativeLinesPart[j].Item2)));
                }

                negativeGridLines.Add(negativeGridLinesPart);
            }

            PrintTime("fillStart");
            FillGrid(ref grid, positiveGridLines, negativeGridLines);
            PrintTime("fillEnd");

            PrintTime("erosionStart");

            ErosionHandler erosionHandler = new ErosionHandler();
            grid = erosionHandler.Erode(grid, 30);

            PrintTime("erosionEnd");

            //do the thinning until only a skeleton remains
            //note: testing in an external duplicate to easily visualize the grid found that this takes a notable bit of time (~approx 15 seconds)
            //thinning can probably be done in parallel to significantly speed this up, but need to figure out how to do this.
            //current thinning gives lines to corners, these need to be dealt with in the post-startstate algorithm
            createGolayElements();
            PrintTime("thinningStart");
            bool thinning = true;
            while (thinning) { grid = ThinnedGrid(grid, out thinning); }

            PrintTime("thinningEnd");
            PostFiltering(ref grid, 50);

            PrintTime("filterEnd");
            //at this point, grid contains the skeleton path as a thin line of booleans
            //now we need to convert this to a pathdata

            int rows = grid.GetLength(0);
            int cols = grid.GetLength(1);

            List<(int, int)> prePathDataPoints = new();
            List<((int, int), (int, int))> prePathDataEdges = new();
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    bool gridpoint = grid[i, j];
                    if (gridpoint)
                    {
                        prePathDataPoints.Add((i, j));

                        //compute edges to the left, to the bottom -left, -middle and -right squares.
                        //we compute in this pattern to prevent adding duplicate edges

                        //look right
                        if (!(i + 1 > rows - 1))
                        {
                            if (grid[i + 1, j]) prePathDataEdges.Add(((i, j), (i + 1, j)));
                        }

                        //look bottom-left
                        if (!(i - 1 < 0 || i - 1 > rows - 1 || j + 1 > cols - 1))
                        {
                            if (grid[i - 1, j + 1]) prePathDataEdges.Add(((i, j), (i - 1, j + 1)));
                        }

                        //look bottom-middle
                        if (!(j + 1 > cols - 1))
                        {
                            if (grid[i, j + 1]) prePathDataEdges.Add(((i, j), (i, j + 1)));
                        }

                        //look bottom-right
                        if (!(i + 1 > rows - 1 || j + 1 > cols - 1))
                        {
                            if (grid[i + 1, j + 1]) prePathDataEdges.Add(((i, j), (i + 1, j + 1)));
                        }

                    }
                }
            }

            //convert to polygon space
            List<Vector3> pathDataPoints = new();
            List<(Vector3, Vector3)> pathDataEdges = new();

            foreach ((int, int) point in prePathDataPoints) { pathDataPoints.Add(ToPolygonSpace(point)); }

            for (int i = 0; i < prePathDataEdges.Count; i++)
            {
                pathDataEdges.Add((ToPolygonSpace(prePathDataEdges[i].Item1), ToPolygonSpace(prePathDataEdges[i].Item2)));
            }

            PathData path = new();
            path.points = pathDataPoints;
            path.edges = pathDataEdges;

            return path;
        }

        /// <summary>
        /// turns the polygon into a list of its line segments signified by start and endpoint
        /// </summary>
        /// <param name="polygon">the polygon from which to create the list</param>
        /// <returns>list of line segments, signified by 2 points</returns>
        private List<(Vector3, Vector3)> GenerateLines(Polygon polygon)
        {
            List<(Vector3, Vector3)> results = new();
            List<Vector3> points = polygon.GetPoints().ToList();
            //add a duplicate of the first point to the end of the list
            points.Add(new Vector3(points[0].x, points[0].y, points[0].z));

            for (int i = 0; i < points.Count - 1; i++) { results.Add((points[i], points[i + 1])); }

            return results;
        }

        /// <summary>
        /// creates an empty grid of booleans with its size based on the size of a given polygon
        /// also initalizes the movetransform, averageheight and scalefactor variables
        /// </summary>
        /// <param name="polygon">the polygon from which to create the grid</param>
        /// <returns>2d array of booleans that are set to false</returns>
        private bool[,] MakeGrid(Polygon polygon)
        {
            //determine the maximum height and width of the polygon
            Vector3[] points = polygon.GetPoints();

            float minX = points[0].x;
            float minZ = points[0].z;
            float maxX = points[0].x;
            float maxZ = points[0].z;
            for (int i = 1; i < points.Length; i++)
            {
                if (points[i].x < minX) minX = points[i].x;
                if (points[i].x > maxX) maxX = points[i].x;
                if (points[i].z < minZ) minZ = points[i].z;
                if (points[i].z > maxZ) maxZ = points[i].z;
            }

            //compute the average heigh of the polygon for later use
            averageHeight = 0;
            for (int i = 0; i < points.Length; i++) { averageHeight += points[i].y / points.Length; }

            float xDiff = maxX - minX;
            float zDiff = maxZ - minZ;

            int xlength;
            int zlength;

            //testing revealed that unity-vector3 points acquired relate to the real world on a 1:100 scale (in centimeters)
            //so a difference of 0.01 in vector3 coords equated to 1 centimeter in the real world
            scaleFactor = 100;
            xlength = (int)Math.Round(xDiff * scaleFactor);
            zlength = (int)Math.Round(zDiff * scaleFactor);


            //the direction to move in to get from the polygon space to the grid space; addition.
            moveTransform = ((-minX) * scaleFactor, (-minZ) * scaleFactor);

            return new bool[xlength + 1, zlength + 1];
        }

        /// <summary>
        /// Fill an empty grid of booleans with the projection of the walkable space. these booleans are set to true
        /// </summary>
        /// <param name="grid">the empty grid</param>
        /// <param name="positiveLines">the line segments in 'grid space' making up the positive polygon</param>
        /// <param name="negativeLines">line segments in 'grid space' making up negative polygons</param>
        private void FillGrid(ref bool[,] grid, List<((int, int), (int, int))> positiveLines, List<List<((int, int), (int, int))>> negativeLines)
        {
            //draw the lines for the positive polygon
            for (int i = 0; i < positiveLines.Count; i++) { DrawLine(ref grid, positiveLines[i]); }

            List<(int x, int y)> foundPoints = new();

            int rows = grid.GetLength(0);
            int cols = grid.GetLength(1);
            int gridSize = rows * cols;

            NativeArray<bool> resultGrid = new NativeArray<bool>(gridSize, Allocator.TempJob);
            NativeArray<((int x, int y) p1, (int x, int y) p2)> polygonLines =
                new NativeArray<((int x, int y) p1, (int x, int y) p2)>(positiveLines.Count, Allocator.TempJob);


            NativeArray<bool> result = new NativeArray<bool>(gridSize, Allocator.Temp);

            for (int i = 0; i < positiveLines.Count; i++) { polygonLines[i] = positiveLines[i]; }


            for (int i = 0; i < result.Length; i++)
                result[i] = false;

            CheckInPolygonJob positivePolygonCheckJob = new CheckInPolygonJob()
            {
                nativeGrid = ToNativeGrid(grid),
                columns = grid.GetLength(1),
                checkPositivePolygon = true,
                polygonWalls = polygonLines,

                //nativeResultGrid = resultGrid
                result = resultGrid
            };

            JobHandle posPolCheckJobHandle = positivePolygonCheckJob.Schedule(gridSize, 64);

            posPolCheckJobHandle.Complete();

            polygonLines.Dispose();

            for (int i = 0; i < resultGrid.Length; i++)
            {
                int x = i / cols;
                int y = i % cols;
                if (resultGrid[i])
                    foundPoints.Add((x, y));
            }

            resultGrid.Dispose();

            /*
            //floodfill from a position in the positive polygon
            List<(int x, int y)> foundPoints = new();

            for (int x = 0; x < grid.GetLength(0); x++)
            {
                for (int y = 0; y < grid.GetLength(1); y++)
                {
                    //it is useless to set a point that is already true to true
                    if (grid[x, y]) continue;

                    //check if current point is in positive polygon. if not, continue
                    if (!CheckInPolygon(positiveLines, (x, y))) continue;

                    //if the loop makes it past all of the above checks, we have found a valid point
                    foundPoints.Add((x, y));
                }
            }
            */
            //fill in the positive polygon
            for (int i = 0; i < foundPoints.Count; i++)
            {
                //grid[foundPoints[i].x, foundPoints[i].y] = true;
                FloodArea(ref grid, (foundPoints[i].x, foundPoints[i].y));
            }

            //carve out the negative polygons
            for (int n = 0; n < negativeLines.Count; n++)
            {
                foundPoints = new();

                //carve out the lines for the current negative polygon
                for (int i = 0; i < negativeLines[n].Count; i++) { DrawLine(ref grid, negativeLines[n][i], true); }

                //find the points in the current negative polygon
                for (int x = 0; x < grid.GetLength(0); x++)
                {
                    for (int y = 0; y < grid.GetLength(1); y++)
                    {
                        //it is useless to set a point that is already false to false
                        if (!grid[x, y]) continue;

                        //check if current point is in the negative polygon. if not, continue
                        if (!CheckInPolygon(negativeLines[n], (x, y))) continue;

                        //if the loop makes it past all of the above checks, we have found a valid point
                        foundPoints.Add((x, y));
                    }
                }

                //erase the points that lie in the negative polygon
                for (int i = 0; i < foundPoints.Count; i++)
                {
                    //grid[foundPoints[i].x, foundPoints[i].y] = false;
                    FloodArea(ref grid, (foundPoints[i].x, foundPoints[i].y), true);
                }
            }
        }

        private NativeArray<bool> ToNativeGrid(bool[,] grid)
        {
            int rows = grid.GetLength(0);
            int cols = grid.GetLength(1);
            int gridSize = rows * cols;

            NativeArray<bool> toNativeGrid = new NativeArray<bool>(gridSize, Allocator.TempJob);

            for (int x = 0; x < rows; x++)
            {
                for (int y = 0; y < cols; y++)
                {
                    int index = x * cols + y;
                    toNativeGrid[index] = grid[x, y];
                }
            }

            return toNativeGrid;
        }

        private bool[,] ToGrid(NativeArray<bool> nativeArray, int rows, int columns)
        {
            bool[,] toGrid = new bool[rows, columns];
            int gridSize = nativeArray.Length;

            for (int i = 0; i < gridSize; i++)
            {
                int x = i / columns;
                int y = i % columns;
                toGrid[x, y] = nativeArray[i];
            }

            return toGrid;
        }

        /// <summary>
        /// check if a point is in a polygon (represented as a list of lines)
        /// done by shooting a ray to the right from the point and counting the number of intersections with polygon edges
        /// </summary>
        /// <param name="lines">list of lines that make up the polygon</param>
        /// <param name="point">point to check if it is inside the polygon</param>
        /// <returns>true if the point lies inside the polygon, false otherwise</returns>
        private bool CheckInPolygon(List<((int x, int y) p1, (int x, int y) p2)> polygonwalls, (int x, int y) point)
        {
            List<(double x, double y)> intersections = new();

            for (int i = 0; i < polygonwalls.Count; i++)
            {
                double intersecty = point.y;
                double intersectx;

                double divider = polygonwalls[i].p2.x - polygonwalls[i].p1.x;
                if (divider == 0) { intersectx = polygonwalls[i].p2.x; }
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
                if (intersectx < Math.Min(polygonwalls[i].p1.x, polygonwalls[i].p2.x) ||
                    intersectx > Math.Max(polygonwalls[i].p1.x, polygonwalls[i].p2.x)
                    || intersecty < Math.Min(polygonwalls[i].p1.y, polygonwalls[i].p2.y) ||
                    intersecty > Math.Max(polygonwalls[i].p1.y, polygonwalls[i].p2.y)) { continue; }

                //if the intersection point is the exact endpoint of a wall, this causes problems. cancel the whole operation
                //we cannot be sure if it lies inside or outside the polygon
                if ((intersectx, intersecty) == polygonwalls[i].p1 || (intersectx, intersecty) == polygonwalls[i].p2) { return false; }

                //add this intersection to the list if it is a new one
                if (!intersections.Contains((intersectx, intersecty))) { intersections.Add((intersectx, intersecty)); }
            }

            if (intersections.Count % 2 == 0) { return false; }
            else return true;
        }

        /// <summary>
        /// draw a line of 'true' values between 2 points on a given grid of booleans
        /// </summary>
        /// <param name="grid">grid of booleans to draw the line on</param>
        /// <param name="linepoints"> line to draw. points are coordinates in the grid </param>
        private void DrawLine(ref bool[,] grid, ((int, int), (int, int)) linepoints, bool carve = false)
        {
            bool setToValue = !carve;

            int x1 = linepoints.Item1.Item1;
            int y1 = linepoints.Item1.Item2;
            int x2 = linepoints.Item2.Item1;
            int y2 = linepoints.Item2.Item2;

            float xdiff = x2 - x1;
            float ydiff = y2 - y1;
            float a;
            float c;

            if (xdiff == 0)
                a = 0;
            else
                a = ydiff / xdiff;
            if (ydiff == 0)
                c = 0;
            else
                c = xdiff / ydiff;

            float b = (float)y1 - a * (float)x1;
            float d = (float)x1 - c * (float)y1;

            for (int x = Math.Min(x1, x2); x < Math.Max(x1, x2); x++)
            {
                if (x < 0 || x > grid.GetLength(0)) continue;

                float y = a * x + b;
                if (y - (int)y > 0.5)
                {
                    if (y + 1 < 0 || y + 1 > grid.GetLength(1)) continue;
                    grid[x, (int)(y + 1)] = setToValue;
                }
                else
                {
                    if (y < 0 || y > grid.GetLength(1)) continue;
                    grid[x, (int)y] = setToValue;
                }
            }

            for (int y = Math.Min(y1, y2); y < Math.Max(y1, y2); y++)
            {
                if (y < 0 || y > grid.GetLength(1)) continue;

                float x = c * y + d;
                if (x - (int)x > 0.5)
                {
                    if (x + 1 < 0 || x + 1 > grid.GetLength(0)) continue;
                    grid[(int)(x + 1), y] = setToValue;
                }
                else
                {
                    if (x < 0 || x > grid.GetLength(0)) continue;
                    grid[(int)x, y] = setToValue;
                }
            }
        }

        /// <summary>
        /// applies one iteration of the thinning operation to the given grid and returns it
        /// one iteration in the case means one 'thin' with each Golay element
        /// </summary>
        /// <param name="grid">the grid to thin</param>
        /// <param name="changed">will be set to true if the grid was thinned. will be set to false if the grid wasn't changed</param>
        /// <returns>the thinned grid</returns>
        public bool[,] ThinnedGrid(bool[,] grid, out bool changed)
        {
            changed = false;

            for (int i = 0; i < frontGolayElements.Count; i++)
            {
                /*
                int rows = grid.GetLength(0);
                int cols = grid.GetLength(1);

                NativeArray<bool> resArray = new NativeArray<bool>(rows*cols, Allocator.Temp);

                Parallel.For(
                    0,
                    rows,
                    x =>
                    {
                        Parallel.For(
                            0,
                            cols,
                            y =>
                            {
                                if (!grid[x, y])
                                {
                                    resArray[x * cols + y] = false;
                                }
                                //if i have a hit in this position, it will be set to false
                                else if (CheckHitorMiss(grid, x, y, i))
                                {
                                    resArray[x * cols + y] = false;
                                    hasChanged = true;
                                }
                                else
                                {
                                    //if i dont have a hit the grid keeps its old value (which should be true)
                                    resArray[x * cols + y] = grid[x, y];
                                }
                            });
                    });
                */
                /*
                for(int x = 0; x < grid.GetLength(0); x++)
                {
                    for (int y = 0; y < grid.GetLength(1); y++)
                    {
                        if (!grid[x, y])
                        {
                            res[x, y] = false;
                        }
                        //if i have a hit in this position, it will be set to false
                        else if (CheckHitorMiss(grid, x, y, i))
                        {
                            res[x, y] = false;
                            hasChanged = true;
                        }
                        else
                        {
                            //if i dont have a hit the grid keeps its old value (which should be true)
                            res[x, y] = grid[x, y];
                        }
                    }
                }
                */

                
                int rows = grid.GetLength(0);
                int cols = grid.GetLength(1);
                int gridSize = rows * cols;

                NativeArray<bool> resultGrid = new NativeArray<bool>(gridSize, Allocator.TempJob);

                NativeArray<bool> frontElement = new NativeArray<bool>(ToNativeGrid(frontGolayElements[i]), Allocator.TempJob);
                NativeArray<bool> backElement = new NativeArray<bool>(ToNativeGrid(backGolayElements[i]), Allocator.TempJob);

                int elementLength = frontGolayElements[i].GetLength(0);

                CheckHitOrMissJob hitOrMissCheckJob = new CheckHitOrMissJob()
                {
                    nativeGrid = ToNativeGrid(grid),
                    columns = cols,
                    rows = rows,
                    frontElement = frontElement,
                    backElement = backElement,
                    elementLength = elementLength,

                    result = resultGrid
                };

                JobHandle hitOrMissCheckJobHandle = hitOrMissCheckJob.Schedule(gridSize, 64);

                hitOrMissCheckJobHandle.Complete();

                bool[,] resGrid = ToGrid(resultGrid, rows, cols);

                if (!changed)
                    changed = !EqualArrays(grid, resGrid);

                frontElement.Dispose();
                backElement.Dispose();

                Array.Copy(resGrid, grid, grid.Length);
                resultGrid.Dispose();
                
                //Array.Copy(ToGrid(resArray, rows, cols), grid, grid.Length);
            }

            //changed = hasChanged;
            return grid;
        }

        private bool EqualArrays(bool[,] a, bool[,] b)
        {
            if(a.Length != b.Length) return false;
            for (int i = 0; i < a.GetLength(0); i++)
            {
                for (int j = 0; j < a.GetLength(1); j++)
                {
                    if (a[i,j] != b[i,j])
                        return false;
                }
            }
            return true;
        }

        /// <summary>
        /// removes path branches that are too short
        /// </summary>
        /// <param name="grid">the grid containing the skeleton path</param>
        /// <param name="treshold">the minimum length a branch must have in pixels. shorter branches will be removed</param>
        public void PostFiltering(ref bool[,] grid, int treshold)
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
                        rolledoutneighbours[8] = grid[x - 1, y - 1]; //add a copy of the first point looked at to the end
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
                                for (int j = 0; j < addedpoints.Count; j++) { consideredpoints.Remove(addedpoints[j]); }

                                stop = true;
                                break;
                            }

                            queue.Enqueue(consideredpoint);
                            consideredpoints.Add(consideredpoint);
                            addedpoints.Add(consideredpoint);
                        }
                    }
                }

                //delete the found subpath / points als de length te klein is.
                //length is detemerined using the euclidian distance between de endpoint and junction this line spans, since lines are mostly straight

                float par1 = (consideredpoints[consideredpoints.Count - 1].x - consideredpoints[0].x);
                float par2 = (consideredpoints[consideredpoints.Count - 1].y - consideredpoints[0].y);
                double distance = Math.Sqrt((par1 * par1) + (par2 * par2));
                if (distance < treshold)
                {
                    for (int j = 0; j < consideredpoints.Count; j++) { grid[consideredpoints[j].x, consideredpoints[j].y] = false; }
                }
            }
        }

        /// <summary>
        /// transform a 'polygon space' point into 'grid space'
        /// </summary>
        /// <param name="point">the point to be transformed</param>
        /// <returns>the transformed point </returns>
        private (int, int) ToGridSpace(Vector3 point)
        {
            Matrix4x4 scale = Matrix4x4.Scale(new Vector3(scaleFactor, 1, scaleFactor));
            Vector3 transformedPoint = scale.MultiplyPoint3x4(point);
            Vector3 movedpoint = new Vector3(transformedPoint.x + moveTransform.Item1, 1, transformedPoint.z + moveTransform.Item2);

            return ((int)Math.Round(movedpoint.x), (int)Math.Round(movedpoint.z));
        }

        /// <summary>
        /// transform a 'grid space' point into 'polygon space'
        /// </summary>
        /// <param name="point">the point to be transformed</param>
        /// <returns>the transformed point </returns>
        private Vector3 ToPolygonSpace((int, int) point)
        {
            Vector3 movedpoint = new Vector3(point.Item1 - moveTransform.Item1, averageHeight, point.Item2 - moveTransform.Item2);
            Matrix4x4 scale = Matrix4x4.Scale(new Vector3(1 / scaleFactor, 1, 1 / scaleFactor));
            Vector3 transformedPoint = scale.MultiplyPoint3x4(movedpoint);

            return transformedPoint;
        }

        //floodfill is used to fill in the positive or carve out the negative polygons

        #region floodfill

        /// <summary>
        /// flood an area of the grid with 'true' from a given start position. 
        /// it is assumed that there is a boundary of 'true' values surrounding the startpoint
        /// if there isn't, the entire grid will be flooded with true
        /// </summary>
        /// <param name="grid">the array of booleans to flood</param>
        /// <param name="startpos">the position in the grid to start from</param>
        /// <param name="reverse">if true, will 'reverse' floodfill. instead of filling areas with true, fills them with false</param>
        private void FloodArea(ref bool[,] grid, (int, int) startpos, bool reverse = false)
        {
            int width = grid.GetLength(0);
            int height = grid.GetLength(1);

            Queue<(int, int)> queue = new Queue<(int, int)>();
            queue.Enqueue(startpos);

            while (queue.Count > 0)
            {
                (int x, int y) current = queue.Dequeue();
                if (reverse)
                {
                    if (grid[current.x, current.y])
                    {
                        grid[current.x, current.y] = false;
                        EnqueueNeighbors(ref queue, current, width, height);
                    }
                }
                else
                {
                    if (!grid[current.x, current.y])
                    {
                        grid[current.x, current.y] = true;
                        EnqueueNeighbors(ref queue, current, width, height);
                    }
                }
            }
        }

        //enqueue the 4 neighbours of a given position into the given queue if possible

        /// <summary>
        /// enqueue the 4 neighbours of a given position into the given queue if possible
        /// </summary>
        /// <param name="queue">the queue to put neighbours in</param>
        /// <param name="pos">the position to get neighbours from</param>
        /// <param name="width">the maximum width of the grid. neighbour positions beyond this poitn will not be enqueued</param>
        /// <param name="height">the maximum height of the grid. neighbour positions beyond this point will not be enqueued</param>
        private void EnqueueNeighbors(ref Queue<(int, int)> queue, (int x, int y) pos, int width, int height)
        {
            //enqueue right
            if (pos.x + 1 > 0 && pos.x + 1 < width && pos.y > 0 && pos.y < height)
                queue.Enqueue((pos.x + 1, pos.y));
            //enqueue bottom
            if (pos.x > 0 && pos.x < width && pos.y + 1 > 0 && pos.y + 1 < height)
                queue.Enqueue((pos.x, pos.y + 1));
            //enqueue left
            if (pos.x - 1 > 0 && pos.x - 1 < width && pos.y > 0 && pos.y < height)
                queue.Enqueue((pos.x - 1, pos.y));
            //enqueue top
            if (pos.x > 0 && pos.x < width && pos.y - 1 > 0 && pos.y - 1 < height)
                queue.Enqueue((pos.x, pos.y - 1));
        }

        #endregion


        //the code relating to the structuring elements used in the hit-or-miss operation

        #region structuringElements

        /// <summary>
        /// create all the 'L Golay' structuring elements used for the hit-or-miss part of the thinning operation
        /// </summary>
        private void createGolayElements()
        {
            CreateFrontGolayElements();
            CreateBackGolayElements();
        }

        /// <summary>
        ///initialize the 'foreground' elements for the hit-or-miss operation
        /// </summary>
        private void CreateFrontGolayElements()
        {
            bool[,] elem1 = new bool[3, 3] { { false, false, false }, { false, true, false }, { true, true, true } };
            frontGolayElements.Add(elem1);
            bool[,] elem2 = new bool[3, 3] { { false, false, false }, { true, true, false }, { true, true, false } };
            frontGolayElements.Add(elem2);
            bool[,] elem3 = new bool[3, 3] { { true, false, false }, { true, true, false }, { true, false, false } };
            frontGolayElements.Add(elem3);
            bool[,] elem4 = new bool[3, 3] { { true, true, false }, { true, true, false }, { false, false, false } };
            frontGolayElements.Add(elem4);
            bool[,] elem5 = new bool[3, 3] { { true, true, true }, { false, true, false }, { false, false, false } };
            frontGolayElements.Add(elem5);
            bool[,] elem6 = new bool[3, 3] { { false, true, true }, { false, true, true }, { false, false, false } };
            frontGolayElements.Add(elem6);
            bool[,] elem7 = new bool[3, 3] { { false, false, true }, { false, true, true }, { false, false, true } };
            frontGolayElements.Add(elem7);
            bool[,] elem8 = new bool[3, 3] { { false, false, false }, { false, true, true }, { false, true, true } };
            frontGolayElements.Add(elem8);
        }

        /// <summary>
        /// initialize the 'background' elements for the hit-or-miss operation
        /// </summary>
        private void CreateBackGolayElements()
        {
            bool[,] elem1 = new bool[3, 3] { { true, true, true }, { false, false, false }, { false, false, false } };
            backGolayElements.Add(elem1);
            bool[,] elem2 = new bool[3, 3] { { false, true, true }, { false, false, true }, { false, false, false } };
            backGolayElements.Add(elem2);
            bool[,] elem3 = new bool[3, 3] { { false, false, true }, { false, false, true }, { false, false, true } };
            backGolayElements.Add(elem3);
            bool[,] elem4 = new bool[3, 3] { { false, false, false }, { false, false, true }, { false, true, true } };
            backGolayElements.Add(elem4);
            bool[,] elem5 = new bool[3, 3] { { false, false, false }, { false, false, false }, { true, true, true } };
            backGolayElements.Add(elem5);
            bool[,] elem6 = new bool[3, 3] { { false, false, false }, { true, false, false }, { true, true, false } };
            backGolayElements.Add(elem6);
            bool[,] elem7 = new bool[3, 3] { { true, false, false }, { true, false, false }, { true, false, false } };
            backGolayElements.Add(elem7);
            bool[,] elem8 = new bool[3, 3] { { true, true, false }, { true, false, false }, { false, false, false } };
            backGolayElements.Add(elem8);
        }

        #endregion

        void PrintTime(string s)
        {
            Debug.Log(s + ": " + (Time.realtimeSinceStartup - startTime).ToString());
        }
    }
}


//todo primary:
//improve visualisatie zodat je ook negative polygons kan tekenen voordat je het pad bepaald
//for above: zorg dat de path gen gebeurt bij de click van een andere button dan de autocomplete button
//improve performance
//test things (unit tests)

//evt de pathdata opschonen, maar zou daarvoor wel hough moeten gebruiken en testing showed dat die niet perfect werkt (somewhat decent tho)
//improve performance door het te multithreaden of indien mogelijk op de gpu te runnen

//scale testing met debug log seems to be about 1:100 scale, 61 cm meetlat vierkant gaat in ongeveer 0.61 increments. dus 1 vector3 = 1 meter
//make polygon 'real-scale' hiermee in plaats van set length 500?

//'merge' / collapse corner noodles that share the same junction to a straighter line? could be cool


//todo primary new:
//'merge' / collapse corner noodles 
//clean up polygonmanager en scene