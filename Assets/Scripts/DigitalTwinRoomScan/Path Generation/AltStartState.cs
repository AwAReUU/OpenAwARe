using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;

public class AltStartState
{
    float scalefactor;
    (float, float) movetransform;

    List<bool[,]> frontGolayElements = new();
    List<bool[,]> backGolayElements = new();

    /// <summary>
    /// Create a path from a given positive polygon and list of negative polygons that represent the room
    /// </summary>
    /// <param name="positive">the polygon whose volume represents the area where you can walk</param>
    /// <param name="negatives">list of polygons whose volume represent the area where you cannot walk</param>
    /// <returns>a 'pathdata' which represents a path through the room</returns>
    public PathData GetStartState(Polygon positive, List<Polygon> negatives)
    {
        //determine the grid. still empty. also initalizes the scalefactor and movetransform variables.
        bool[,] grid = MakeGrid(positive);

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

        FillGrid(ref grid, positiveGridLines, negativeGridLines);


        //this is probably where to appy erosion


        //do the thinning until only a skeleton remains
        //note: testing in an external duplicate to easily visualize the grid found that this takes a notable bit of time (~approx 15 seconds)
        createGolayElements();
        bool thinning = true;
        while(thinning)
        {
            grid = ThinnedGrid(grid, out thinning);
        }

        //at this point, contains the skeleton path as a thin line of booleans

        List<(int, int)> prePathDataPoints = new();
        List<((int, int), (int, int))> prePathDataEdges = new();
        for (int i = 0; i < grid.GetLength(0); i++)
        {
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                bool gridpoint = grid[i, j];
                if (gridpoint)
                {
                    prePathDataPoints.Add((i, j));

                    //compute edges to the left, to the bottom -left, -middle and -right squares.
                    //we compute in this pattern to prevent adding duplicate edges

                    //look right
                    if (!(i + 1 > grid.GetLength(0) - 1))
                    {
                        if (grid[i + 1, j]) prePathDataEdges.Add(((i, j), (i + 1, j)));
                    }

                    //look bottom-left
                    if (!(i - 1 < 0 || i - 1 > grid.GetLength(0) - 1 || j + 1 > grid.GetLength(1) - 1))
                    {
                        if (grid[i - 1, j + 1]) prePathDataEdges.Add(((i, j), (i - 1, j + 1)));
                    }

                    //look bottom-middle
                    if (!(j + 1 > grid.GetLength(1) - 1))
                    {
                        if (grid[i, j + 1]) prePathDataEdges.Add(((i, j), (i, j + 1)));
                    }

                    //look bottom-right
                    if (!(i + 1 > grid.GetLength(0) - 1 || j + 1 > grid.GetLength(1) - 1))
                    {
                        if (grid[i + 1, j + 1]) prePathDataEdges.Add(((i, j), (i + 1, j + 1)));
                    }

                }
            }
        }
      
        //convert to polygon space
        List<Vector3> pathDataPoints = new();
        List<(Vector3, Vector3)> pathDataEdges = new();

        for (int i = 0; i < prePathDataPoints.Count; i++)
        {
            pathDataPoints.Add(ToPolygonSpace(prePathDataPoints[i]));
        }

        for (int i = 0; i < prePathDataEdges.Count; i++)
        {
            pathDataEdges.Add((ToPolygonSpace(prePathDataEdges[i].Item1), ToPolygonSpace(prePathDataEdges[i].Item2)));
        }

        PathData path = new();
        path.points = pathDataPoints;
        path.edges = pathDataEdges;

        //then here we extend pathdata endpoints
        path = ExtendEndPoints(path, positive, negatives, 10);

        return path;
    }

    /// <summary>
    /// extend the enpoints of the path to the walls
    /// </summary>
    /// <param name="path">the path from which the endpoints are extended</param>
    /// <param name="positive">the positive polygon from which the path was made</param>
    /// <param name="negatives">the list of negative polygons from which the path was made</param>
    /// <param name="minConsideredPercentage">the percentage of the path from the endpoint to consider when determining the direction in which it should be extended</param>
    /// <returns>new or altered path</returns>
    private PathData ExtendEndPoints(PathData path, Polygon positive, List<Polygon> negatives, int minConsideredPercentage)
    {
        List<(Vector3, Vector3)> allWalls = GenerateLines(positive);
        for (int i = 0; i < negatives.Count; i++)
        {
            allWalls.Concat(GenerateLines(negatives[i]));
        }

        List<Vector3> endpoints = new();
        List<Vector3> junctions = new();
        Dictionary<Vector3, int> pointFrequencies = new();

        //count how often each point appears in the edges list
        for (int i = 0; i < path.edges.Count; i++)
        {
            Vector3 point1 = path.edges[i].Item1;
            Vector3 point2 = path.edges[i].Item2;

            if (pointFrequencies.ContainsKey(point1)) pointFrequencies[point1]++;
            else pointFrequencies.Add(point1, 1);
            if (pointFrequencies.ContainsKey(point2)) pointFrequencies[point2]++;
            else pointFrequencies.Add(point2, 1);
        }
        //each point that appears only once in the edges list is an endpoint that must potentially be extended
        //each point that appears more than twice in the edges list is a junction
        for (int i = 0; i < path.edges.Count; i++)
        {
            Vector3 point1 = path.edges[i].Item1;
            Vector3 point2 = path.edges[i].Item2;

            if (pointFrequencies[point1] == 1 && !endpoints.Contains(point1)) endpoints.Add(point1);
            if (pointFrequencies[point2] == 1 && !endpoints.Contains(point2)) endpoints.Add(point2);

            if (pointFrequencies[point1] > 2 && !junctions.Contains(point1)) junctions.Add(point1);
            if (pointFrequencies[point2] > 2 && !junctions.Contains(point2)) junctions.Add(point2);
        }

        //make subpaths from the endpoints to the junctions. or if there are no junctions, to other endpoints
        List<PathData> subpaths = new();
        for (int i = 0; i < endpoints.Count; i++)
        {

            PathData subpath = new();
            Vector3 currentpoint = endpoints[i];
            //keep adding edges to the subpath until we reach a junction or there are no edges left to add
            while (!junctions.Contains(currentpoint) || subpath.edges.Count == path.edges.Count)
            {
                //it should always find either 1 or 2 edges in the list, if it finds more than that, something went wrong with making junctions list
                List<(Vector3, Vector3)> edgesfound = path.edges.FindAll(res => res.Item1 == currentpoint || res.Item1 == currentpoint);

                //make sure to add the correct edge
                if (!subpath.edges.Contains(edgesfound[0]))
                {
                    subpath.edges.Add(edgesfound[0]);
                    if (currentpoint == edgesfound[0].Item1) currentpoint = edgesfound[0].Item2;
                    else currentpoint = edgesfound[0].Item1;
                }
                else
                {
                    subpath.edges.Add(edgesfound[1]);
                    if (currentpoint == edgesfound[1].Item1) currentpoint = edgesfound[1].Item2;
                    else currentpoint = edgesfound[1].Item1;
                }
            }

            subpaths.Add(subpath);
        }

        //do this for every endpoint and subpath, make into for loop
        for (int i = 0; i < subpaths.Count(); i++)
        {
            //compute the totel length of the subpath
            float totalpathlength = 0;
            float[] edgelengths = new float[subpaths[i].edges.Count];
            for (int j = 0; j < subpaths[i].edges.Count; j++)
            {
                //use pythagoras to add length of an edge to the total path length
                float length = (float)Math.Sqrt(Math.Pow(subpaths[i].edges[j].Item2.x - subpaths[i].edges[j].Item1.x, 2) 
                                              + Math.Pow(subpaths[i].edges[j].Item2.y - subpaths[i].edges[j].Item1.y, 2));
                totalpathlength += length;
                edgelengths[j] = length;
            }

            //compute how much length we are allowed to consider for the linear regression
            float remaininglength = totalpathlength * minConsideredPercentage / 100;
            List<Vector3> pathRegressionPoints = new();
            //the edges are added to the subpaths' list of edges in order from endpoint first, so we do not have to worry about sorting the list here
            int iterator = 0;
            while (remaininglength > 0)
            {
                if (!pathRegressionPoints.Contains(subpaths[i].edges[iterator].Item1)) pathRegressionPoints.Add(subpaths[i].edges[iterator].Item1);
                if (!pathRegressionPoints.Contains(subpaths[i].edges[iterator].Item2)) pathRegressionPoints.Add(subpaths[i].edges[iterator].Item2);
                remaininglength -= edgelengths[iterator];
                iterator++;
            }
            
            //use linear regression to find intersections on the line we use to extend the path
            List<Vector3> intersections = LinearRegressionIntersections(pathRegressionPoints, allWalls);

            //find the closest intersection, as this is the wall that we need to extend to
            Vector3 closest = intersections[0];
            float closestDist = (float)Math.Sqrt(Math.Pow(intersections[0].x - endpoints[i].x, 2) + Math.Pow(intersections[0].y - endpoints[i].y, 2));
            for (int j = 1; j < intersections.Count; j++)
            {
                //pythagoras again
                float distance = (float)Math.Sqrt(Math.Pow(intersections[j].x - endpoints[i].x, 2) + Math.Pow(intersections[j].y - endpoints[i].y , 2));
                if(distance < closestDist)
                {
                    closest = intersections[j];
                    closestDist = distance;
                }
            }

            //create and add the new edge that extends the enpoint to the wall
            (Vector3, Vector3) newEdge = (endpoints[i], closest);
            path.edges.Add(newEdge);
        }

        return path;
    }

    /// <summary>
    /// use linear regression to find get a new line / ray which is the direction we wish to extend the path in.
    /// then compute intersections with line segments (the polygon, walls) and return those
    /// </summary>
    /// <param name="pathpoints">the points to consider for linear regression</param>
    /// <param name="walls">the walls to get intersections with</param>
    /// <returns>a list of intersections with walls</returns>
    private List<Vector3> LinearRegressionIntersections(List<Vector3> pathpoints, List<(Vector3, Vector3)> walls)
    {
        //various data that is needed to perform linear regression, the mathematical sum of various components of the sample:
        //the sum of the x values, the y values, the x*y values, the X^2 values, the Y^2 values
        double sumX = 0, sumY = 0, sumXY = 0, sumX2 = 0, sumY2 = 0;
        int n = pathpoints.Count;

        for (int i = 0; i < pathpoints.Count; i++)
        {
            sumX += pathpoints[i].x;
            sumY += pathpoints[i].y;
            sumXY += pathpoints[i].x * pathpoints[i].y;
            sumX2 += pathpoints[i].x * pathpoints[i].x;
            sumY2 += pathpoints[i].y * pathpoints[i].y;
        }

        //the 2 lines found by linear regression
        //formula in the form y = m1 * x + b1
        double m1 = (n * sumXY - sumX * sumY) / (n * sumX2 - sumX * sumX);
        double b1 = (sumY - m1 * sumX) / n;
        //formula in the form x = m2 * y + b2
        double m2 = (n * sumXY - sumX * sumY) / (n * sumY2 - sumY * sumY);
        double b2 = (sumX - m2 * sumY) / n;

        //find the intersections of walls with a line that follows the average of the 2 lines found above
        List<Vector3> intersections = new();
        for (int i = 0; i < walls.Count; i++)
        {
            Vector3 point1 = walls[i].Item1;
            Vector3 point2 = walls[i].Item2;
            //construct the line of the wall segment in the form y = ax + b
            double a = (point2.y - point1.y) / (point2.x - point1.x);
            double b = point1.y + a * point1.x;

            //calculate the intersection coordinates of the average 
            double x = (2 * b - b1 + b2) / (m1 + (1 / m2) - 2 * a);
            double y = a * x + b;

            //check if the intersection point lies on the wall segment
            if(x < Math.Min(point1.x, point2.x) || x > Math.Max(point1.x, point2.x) 
            || y < Math.Min(point1.y, point2.y) || y > Math.Max(point1.y, point2.y)) { continue; }

            intersections.Add(new Vector3((float)x, (float)y));
        }

        return intersections;
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
        points.Add(new Vector3(points[0].x, points[0].y));

        for (int i = 0; i < points.Count - 1; i++)
        {
            results.Add((points[i], points[i + 1]));
        }

        return results;
    }

    /// <summary>
    /// creates an empty grid of booleans with its size based on the size of a given polygon
    /// </summary>
    /// <param name="polygon">the polygon from which to create the grid</param>
    /// <returns>2d array of booleans that are set to false</returns>
    private bool[,] MakeGrid(Polygon polygon)
    {
        //determine the maximum height and width of the polygon
        Vector3[] points = polygon.GetPoints();

        float minX = points[0].x;
        float minY = points[0].y;
        float maxX = points[0].x;
        float maxY = points[0].y;
        for(int i = 1; i < points.Length; i++)
        {
            if (points[i].x < minX) minX = points[i].x;
            if (points[i].x > maxX) maxX = points[i].x;
            if (points[i].y < minY) minY = points[i].y;
            if (points[i].y > maxY) maxY = points[i].y;
        }

        float xDiff = maxX - minX;
        float yDiff = maxY - minY;

        int xlength;
        int ylength;

        //desired size: ~500 in the longest dimension
        int longestside = 500;
        if (xDiff > yDiff)
        {
            xlength = longestside;
            ylength = (int)Math.Ceiling(longestside * (yDiff / xDiff));
            scalefactor = xlength / xDiff;
        }
        else
        {
            ylength = longestside;
            xlength = (int)Math.Ceiling(longestside * (xDiff / yDiff));
            scalefactor = ylength / yDiff;
        }

        //the direction to move in to get from the polygon space to the grid space; addition.
        movetransform = ((-minX) * scalefactor, (-minY) * scalefactor);

        return new bool[xlength + 1, ylength + 1];
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
        for (int i = 0; i < positiveLines.Count; i++)
        {
            DrawLine(ref grid, positiveLines[i]);
        }

        //draw the lines for all the negative polygons
        for (int i = 0; i < negativeLines.Count; i++)
        {
            for (int j = 0; j < negativeLines[i].Count; j++)
            {
                DrawLine(ref grid, negativeLines[i][j]);
            }
        }

        //at this point we have an outline of the walkable space in the grid
        //we need to fill this outline

        //floodfill from a position in the positive polygon, but outside negative polygons

        //construct 'center of mass' of positive polygon. we fire a ray in this direction to determine if the point we wish
        //to begin the floodfill from is a valid point
        (int x, int y) avgPositivePoint = (0, 0);
        for (int i = 0; i < positiveLines.Count; i++)
        {
            (int x, int y) point = positiveLines[i].Item1;
            avgPositivePoint.x += point.x;
            avgPositivePoint.y += point.y;
        }
        avgPositivePoint = (avgPositivePoint.x / positiveLines.Count, avgPositivePoint.y / positiveLines.Count);

        //find a valid point. We consider the 8-neighbourhood of each cornerpoint of the positive polygon as these
        //are likely to find a valid point.
        bool foundValidPoint = false;
        (int x, int y) foundPoint = (0, 0);
        for (int i = 0; i < positiveLines.Count; i++)
        {
            if (foundValidPoint) break;

            //the point from which to grab the 8-neighbourhood
            (int x, int y) originPoint = positiveLines[i].Item1;

            //double for loop with x and y is to consider the 8-neighbourhood of the point as origin points.
            for (int x = -1; x < 2; x++)
            {
                if (foundValidPoint) break;

                for (int y = -1; y < 2; y++)
                {
                    if (foundValidPoint) break;

                    //the point we are currently considering
                    (int x, int y) consideredPoint = (originPoint.x + x, originPoint.y + y);

                    //if the current point in consideration lies in an invalid place, consider a different point
                    if (consideredPoint.x < 0 || consideredPoint.x > grid.GetLength(0) ||
                        consideredPoint.y < 0 || consideredPoint.y > grid.GetLength(1) || (x == 0 && y == 0)) continue;

                    //check if the current point is in the positive polygon. if not, consider a different point
                    if (!CheckInPolygon(positiveLines, consideredPoint, avgPositivePoint))
                    {
                        continue;
                    }

                    //check if the current point is in a negative polygon. if it is, consider a different point
                    bool inNegative = false;
                    for (int j = 0; j < negativeLines.Count; j++)
                    {
                        //compute the 'center mass' of the current negative polygon
                        (int x, int y) avgNegativePoint = (0, 0);
                        for (int z = 0; z < negativeLines[j].Count; z++)
                        {
                            (int x, int y) point = negativeLines[j][z].Item1;
                            avgNegativePoint.x += point.x;
                            avgNegativePoint.y += point.y;
                        }
                        avgNegativePoint = (avgNegativePoint.x / negativeLines[j].Count, avgNegativePoint.y / negativeLines[j].Count);

                        //check if the origin is in the current negative polygon
                        if (CheckInPolygon(negativeLines[j], consideredPoint, avgNegativePoint))
                        {
                            inNegative = true;
                            break;
                        }
                    }

                    //if all the above checks pass, we have found a valid point
                    if (!inNegative)
                    {
                        foundValidPoint = true;
                        foundPoint = consideredPoint;
                        break;
                    }
                }
            }
        }

        //TODO FOR BUG FIX
        //IMPROVE FLOODFILL STARTPOINT FINDING
        //startpoint is not found right, this causes a game-breaking bug, bugs are bad
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////here//////////////////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        //potentially put a secondary, less efficient method that is guarranteed to find a valid start point (if it exists)

        if (foundValidPoint)
        {
            FloodArea(ref grid, foundPoint);
        }
        else
        {
            Debug.Log("could not find a valid point to start the floodfill from. Have you implemented secondary floodfill startpoint finding yet?");
        } 
    }

    //check if a point (rayOrigin) is in a given polygon (list of lines) by counting the number of intersection the ray has with the polygon

    /// <summary>
    /// check if a point is in a polygon (represented as a list of lines) by counting the number of intersections
    /// a ray shot from the point to the destination has with the lines.
    /// </summary>
    /// <param name="lines">list of lines that make up the polygon</param>
    /// <param name="point">point to check if it is inside the polygon</param>
    /// <param name="rayDestination">direction to shoot the ray in from the point. in most cases, you want this to be the center of the polygon</param>
    /// <returns>true if the point lies inside the polygon, false otherwise</returns>
    private bool CheckInPolygon(List<((int, int), (int, int))> lines, (int x, int y) point, (int x, int y) rayDestination)
    {
        (Vector3, Vector3) ray = (new Vector3(point.x, point.y), new Vector3(rayDestination.x, rayDestination.y));

        int intersections = 0;
        for (int j = 0; j < lines.Count; j++)
        {
            bool intersects;
            Vector3 lineStart = new Vector3(lines[j].Item1.Item1, lines[j].Item1.Item2);
            Vector3 lineEnd = new Vector3(lines[j].Item2.Item1, lines[j].Item2.Item2);

            //use mode 2 as we shoot a ray out of a point, looking for intersections with finite line segments
            _ = IntersectionPoint(ray.Item1, ray.Item2, lineStart, lineEnd, out intersects, 2);
            if (intersects) intersections++;
        }

        //if the number of intersections is even, the origin point is outside of the polygon
        if (intersections % 2 == 0)
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Calculate the intersection point of 2 lines or rays
    /// </summary>
    /// <param name="l1p1">line 1 point 1</param>
    /// <param name="l1p2">line 1 point 2</param>
    /// <param name="l2p1">line 2 point 1</param>
    /// <param name="l2p2">line 2 point 2</param>
    /// <param name="intersects"> returned bool that determines if the lines intersect. </param>
    /// <param name="mode"> the mode of the method, defaults to 0.
    /// in mode 0, both lines provided are treated as finite lines in both directions
    /// in mode 1, line 1 is treated as finite in both directions but line 2 is treated as infinite in both directions
    /// in mode 2, line 1 is infinite in the direction of point 2, but finite in the direction of point 1. line 1 is finite in both directions
    /// in mode 3, both lines are  infinite in the direction of point 2, but finite in the direction of point 1.
    /// in mode 4, both lines are  infinite in both directions.
    /// </param>
    /// <returns> point of intersection </returns>
    private Vector3 IntersectionPoint(Vector3 l1p1, Vector3 l1p2, Vector3 l2p1, Vector3 l2p2, out bool intersects, byte mode = 0)
    {
        double dx1 = l1p2.x - l1p1.x;
        double dy1 = l1p2.y - l1p1.y;
        double dx2 = l2p2.x - l2p1.x;
        double dy2 = l2p2.y - l2p1.y;

        double divider = dx2 * dy1 - dy2 * dx1;

        if (divider == 0)
        {
            //lines are parallel and thus don't intersect
            intersects = false;
            return Vector3.zero;
        }

        double t1 = ((l1p1.x - l2p1.x) * dy2 + (l2p1.y - l1p1.y) * dx2) / divider;
        double t2 = ((l1p1.x - l2p1.x) * dy1 + (l2p1.y - l1p1.y) * dx1) / divider;

        //potential bug: intinite / finite in the wrong direction

        switch (mode)
        {
            //both lines are finite in both directions
            case 0:
                if (!(t1 >= 0 && t1 <= 1 && t2 >= 0 && t2 <= 1))
                {
                    intersects = false;
                    return Vector3.zero;
                }
                break;
            //line 1 is finite in both directions. line 2 is infinite in both directions
            case 1:
                if (!(t1 >= 0 && t1 <= 1))
                {
                    intersects = false;
                    return Vector3.zero;
                }
                break;
            //line 1 is infinite only in the direction of point 2. line 2 is finite in both directions
            case 2:
                if (!(t1 >= 0 && t2 >= 0 && t2 <= 1))
                {
                    intersects = false;
                    return Vector3.zero;
                }
                break;
            //both lines are infinite only in the direction of point 2.
            case 3:
                if (!(t1 >= 0 && t2 >= 0))
                {
                    intersects = false;
                    return Vector3.zero;
                }
                break;
            //both lines are infinite in both directions
            case 4:
                break;
        }

        intersects = true;
        float intersectx = (float)(l1p1.x + t1 * dx1);
        float intersecty = (float)(l1p1.y + t1 * dy1);
        Vector3 intersectionPoint = new Vector3(intersectx, intersecty);
        return intersectionPoint;
    }

    /// <summary>
    /// draw a line of 'true' values between 2 points on a given grid of booleans
    /// </summary>
    /// <param name="grid">grid of booleans to draw the line on</param>
    /// <param name="linepoints"> line to draw. points are coordinates in the grid </param>
    private void DrawLine(ref bool[,] grid, ((int, int), (int, int)) linepoints)
    {
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
            float y = a * x + b;
            if (y - (int)y > 0.5) grid[x, (int)(y + 1)] = true;
            else grid[x, (int)y] = true;
        }

        for (int y = Math.Min(y1, y2); y < Math.Max(y1, y2); y++)
        {
            float x = c * y + d;
            if (x - (int)x > 0.5) grid[(int)(x + 1), y] = true;
            else grid[(int)x, y] = true;
        }
    }

    /// <summary>
    /// flood an area of the grid with 'true' from a given start position. 
    /// it is assumed that there is a boundary of 'true' values surrounding the startpoint
    /// if there isn't, the entire grid will be flooded with true
    /// </summary>
    /// <param name="grid">the array of booleans to flood</param>
    /// <param name="startpos">the position in the grid to start from</param>
    private void FloodArea(ref bool[,] grid, (int, int) startpos)
    {
        int width = grid.GetLength(0);
        int height = grid.GetLength(1);

        Queue<(int, int)> queue = new Queue<(int, int)>();
        queue.Enqueue(startpos);

        while (queue.Count > 0)
        {
            (int x, int y) current = queue.Dequeue();
            if(!grid[current.x, current.y])
            {
                grid[current.x, current.y] = true;
                EnqueueNeighbors(ref queue, current, width, height);
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

    /// <summary>
    /// applies one iteration of the thinning operation to the given grid and returns it
    /// </summary>
    /// <param name="grid">the grid to thin</param>
    /// <param name="changed">will be set to true if the grid was thinned. will be set to false if the grid wasn't changed</param>
    /// <returns>the thinned grid</returns>
    private bool[,] ThinnedGrid(bool[,] grid, out bool changed)
    {
        bool[,] res = new bool[grid.GetLength(0), grid.GetLength(1)];
        changed = false;

        for(int x = 0; x < grid.GetLength(0); x++)
        {
            for(int y = 0; y < grid.GetLength(1); y++)
            {
                //if the grid is false in this position, it will remain false
                if(!grid[x, y])
                {
                    res[x, y] = false;
                    continue;
                }

                //if i have a hit in this position, it will be set to false
                if(CheckHitorMiss(grid, x, y))
                {
                    res[x, y] = false;
                    changed = true;
                    continue;
                }

                //if i dont have a hit the grid keeps its old value
                res[x, y] = grid[x, y];
            }
        }
        return res;
    }

    /// <summary>
    /// check wether a given position on the grid is a hit or a miss for the thinning operation
    /// uses the 'L Golay' structuring elements to check this.
    /// </summary>
    /// <param name="grid">the grid to check in</param>
    /// <param name="x">the x position of the point in the grid to check</param>
    /// <param name="y">the y position of the point in the grid to check</param>
    /// <returns></returns>
    private bool CheckHitorMiss(bool[,] grid, int x, int y)
    {
        //front- and backGolayElements should have the same number of entries. if not, something went very wrong somehow
        for (int i = 0; i < frontGolayElements.Count; i++)
        {
            //3x3 elements
            bool[,] frontElement = frontGolayElements[i];
            bool[,] backElement = backGolayElements[i];

            int offset = frontElement.GetLength(0) / 2;

            bool hit = true;
            for (int a = 0; a < frontElement.GetLength(0); a++)
            {
                if (!hit) break;

                for(int b = 0; b < frontElement.GetLength(0); b++)
                {
                    //if frontelement is true, the grid element at this position must also be true for it to be a hit
                    //if frontelement is false the grid element at this position may be true or false
                    // if backelement is true, the grid element at this position must be false for it to be a hit
                    // if backelement is false the grid element at this position may be true or false

                    //the position falls outside of the grid and is treated as if the grid there is false
                    if(x - offset + a < 0 || x - offset + a > grid.GetLength(0) - 1 || 
                       y - offset + b < 0 || y - offset + b > grid.GetLength(1) - 1)
                    {
                        //the frontelement check
                        if(frontElement[a, b])
                        {
                            hit = false;
                            break;
                        }

                        //since this place falls outside of the grid and is considered false, it always falls in the background element
                        //thus we do not need to perform the background element check, since it will always succeed
                    }
                    else
                    {
                        bool posValue = grid[x - offset + a, y - offset + b];

                        //the front element check
                        if(frontElement[a, b] && !posValue)
                        {
                            hit = false;
                            break;
                        }

                        //the back element check
                        if(backElement[a, b] && posValue)
                        {
                            hit = false;
                            break;
                        }
                    }

                }
            }
            if (hit) return true;
        }
        return false;
    }

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
        bool[,] elem3 = new bool[3, 3] { { true, false, false }, { true, true, false }, { true, false, false} };
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
        bool[,] elem2 = new bool[3, 3] { { false, true, false }, { false, false, true }, { false, false, false } };
        backGolayElements.Add(elem2);
        bool[,] elem3 = new bool[3, 3] { { false, false, true }, { false, false, true }, { false, false, true } };
        backGolayElements.Add(elem3);
        bool[,] elem4 = new bool[3, 3] { { false, false, false }, { false, false, true }, { false, true, false } };
        backGolayElements.Add(elem4);
        bool[,] elem5 = new bool[3, 3] { { false, false, false }, { false, false, false }, { true, true, true } };
        backGolayElements.Add(elem5);
        bool[,] elem6 = new bool[3, 3] { { false, false, false }, { true, false, false }, { false, true, false } };
        backGolayElements.Add(elem6);
        bool[,] elem7 = new bool[3, 3] { { true, false, false }, { true, false, false }, { true, false, false } };
        backGolayElements.Add(elem7);
        bool[,] elem8 = new bool[3, 3] { { false, true, false }, { true, false, false }, { false, false, false } };
        backGolayElements.Add(elem8);
    }

    /// <summary>
    /// transform a 'polygon space' point into 'grid space'
    /// </summary>
    /// <param name="point">the point to be transformed</param>
    /// <returns>the transformed point </returns>
    private (int, int) ToGridSpace(Vector3 point)
    {
        Matrix4x4 scale = Matrix4x4.Scale(new Vector3(scalefactor, scalefactor, 1));
        Vector3 transformedPoint = scale.MultiplyPoint3x4(point);
        Vector3 movedpoint = new Vector3(transformedPoint.x + movetransform.Item1, transformedPoint.y + movetransform.Item2);

        return ((int)Math.Round(movedpoint.x), (int)Math.Round(movedpoint.y));
    }

    /// <summary>
    /// transform a 'grid space' point into 'polygon space'
    /// </summary>
    /// <param name="point">the point to be transformed</param>
    /// <returns>the transformed point </returns>
    private Vector3 ToPolygonSpace((int, int) point)
    {
        Vector3 movedpoint = new Vector3(point.Item1 - movetransform.Item1, point.Item2 - movetransform.Item2);
        Matrix4x4 scale = Matrix4x4.Scale(new Vector3(1 / scalefactor, 1 / scalefactor, 1));
        Vector3 transformedPoint = scale.MultiplyPoint3x4(movedpoint);

        return transformedPoint;
    }
}

//todo primary:
//test things (unit tests, mock data)