// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

using System;
using System.Collections.Generic;
using System.Linq;
using AwARe.Data.Logic;
using UnityEngine;

namespace AwARe.RoomScan.Path
{
    /// <summary>
    /// Class that contains methods for generating a decent startstate for the path.
    /// </summary>
    public class PathGenerator
    {
        private float scaleFactor;
        private (float, float) moveTransform;
        private float averageHeight;

        private float startTime;

        /// <summary>
        /// Create a path from a given positive polygon and list of negative polygons that represent the room.
        /// </summary>
        /// <param name="positive">the polygon whose volume represents the area where you can walk.</param>
        /// <param name="negatives">list of polygons whose volume represent the area where you cannot walk.</param>
        /// <returns>a 'pathdata' which represents a path through the room.</returns>
        public PathData GeneratePath(Polygon positive, List<Polygon> negatives)
        {
            startTime = Time.realtimeSinceStartup;

            // Determine the grid. still empty. also initalizes the scalefactor and movetransform variables.
            bool[,] grid = CreateGrid(positive);

            // Print all points; for debugging purposes
            for (int i = 0; i < positive.GetPoints().Length; i++)
            {
                Debug.Log("Point " + i + ": " + positive.GetPoints()[i].x + ", 0, " + positive.GetPoints()[i].z);
            }

            List<((int, int), (int, int))> positiveGridLines;
            List<List<((int, int), (int, int))>> negativeGridLines;
            (positiveGridLines, negativeGridLines) = GetGridlines(positive, negatives);

            PrintTime("fillStart");
            FloodFillHandler floodFillHandler = new();
            floodFillHandler.FillGrid(ref grid, positiveGridLines, negativeGridLines);
            PrintTime("fillEnd");

            // Apply erosion
            PrintTime("erosionStart");
            ErosionHandler erosionHandler = new();
            grid = erosionHandler.Erode(grid, 30);
            PrintTime("erosionEnd");

            PrintTime("largestShapeStart");
            grid = erosionHandler.KeepLargestShape(grid);
            PrintTime("largestShapeEnd");

            //do the thinning until only a skeleton remains
            PrintTime("thinningStart");
            ThinningHandler thinningHandler = new();
            bool thinning = true;
            while (thinning) { grid = thinningHandler.ThinnedGrid(grid, out thinning); }
            PrintTime("thinningEnd");

            PrintTime("filterStart");
            PostFilteringHandler postFilteringHandler = new();
            postFilteringHandler.PostFiltering(ref grid, 50);
            PrintTime("filterEnd");

            //at this point, grid contains the skeleton path as a thin line of booleans
            //now we need to convert this to a pathdata

            return ConvertToPath(grid);
        }

        /// <summary>
        /// creates an empty grid of booleans with its size based on the size of a given polygon
        /// also initalizes the movetransform, averageheight and scalefactor variables.
        /// </summary>
        /// <param name="polygon">the polygon from which to create the grid.</param>
        /// <returns>2d array of booleans that are set to false.</returns>
        private bool[,] CreateGrid(Polygon polygon)
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
        /// Converts the polygons to lists of start- and endpoints of the lines.
        /// </summary>
        /// <param name="positive">The positive polygon.</param>
        /// <param name="negatives">The negative polygons.</param>
        /// <returns>Lists with start- and endpoints of the lines.</returns>
        private (List<((int, int), (int, int))>, List<List<((int, int), (int, int))>>) GetGridlines(Polygon positive, List<Polygon> negatives)
        {
            List<((int, int), (int, int))> positiveGridLines = new();
            List<List<((int, int), (int, int))>> negativeGridLines = new();

            // Determine all line segments in polygon space and grid space
            List<(Vector3, Vector3)> positiveLines = GenerateLines(positive);
            List<List<(Vector3, Vector3)>> negativeLines = new();

            // Convert the positive lines to grid space
            for (int i = 0; i < positiveLines.Count; i++)
            {
                positiveGridLines.Add((ToGridSpace(positiveLines[i].Item1), ToGridSpace(positiveLines[i].Item2)));
            }

            // Convert the negative lines to grid space
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

            return (positiveGridLines, negativeGridLines);
        }

        /// <summary>
        /// Turns the polygon into a list of its line segments signified by start- and endpoints.
        /// </summary>
        /// <param name="polygon">the polygon from which to create the list.</param>
        /// <returns>list of line segments, signified by 2 points.</returns>
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
        /// transform a 'polygon space' point into 'grid space'.
        /// </summary>
        /// <param name="point">the point to be transformed.</param>
        /// <returns>the transformed point. </returns>
        private (int, int) ToGridSpace(Vector3 point)
        {
            Matrix4x4 scale = Matrix4x4.Scale(new Vector3(scaleFactor, 1, scaleFactor));
            Vector3 transformedPoint = scale.MultiplyPoint3x4(point);
            Vector3 movedPoint = new(transformedPoint.x + moveTransform.Item1, 1, transformedPoint.z + moveTransform.Item2);

            return ((int)Math.Round(movedPoint.x), (int)Math.Round(movedPoint.z));
        }

        /// <summary>
        /// transform a 'grid space' point into 'polygon space'.
        /// </summary>
        /// <param name="point">the point to be transformed.</param>
        /// <returns>the transformed point. </returns>
        private Vector3 ToPolygonSpace((int, int) point)
        {
            Vector3 movedPoint = new(point.Item1 - moveTransform.Item1, averageHeight, point.Item2 - moveTransform.Item2);
            Matrix4x4 scale = Matrix4x4.Scale(new Vector3(1 / scaleFactor, 1, 1 / scaleFactor));
            Vector3 transformedPoint = scale.MultiplyPoint3x4(movedPoint);

            return transformedPoint;
        }

        /// <summary>
        /// Convert the grid to a PathData.
        /// </summary>
        /// <param name="grid">The grid to convert to a path.</param>
        /// <returns>The PathData.</returns>
        private PathData ConvertToPath(bool[,] grid)
        {
            int rows = grid.GetLength(0);
            int cols = grid.GetLength(1);

            List<(int, int)> prePathDataPoints = new();
            List<((int, int), (int, int))> prePathDataEdges = new();
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    bool gridPoint = grid[i, j];
                    if (gridPoint)
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

            PathData path = new()
            {
                points = pathDataPoints,
                edges = pathDataEdges
            };

            return path;
        }

        /// <summary>
        /// Prints the time since starting generating the path to the log; for debugging purposes.
        /// </summary>
        /// <param name="s">The string to print with the time.</param>
        private void PrintTime(string s)
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