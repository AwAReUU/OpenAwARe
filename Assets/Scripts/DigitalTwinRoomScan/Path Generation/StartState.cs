using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR.MagicLeap;

public class StartState 
{
    public PathData GetStartState(Polygon positive, List<Polygon> negatives)
    {
        //shoot a ray from every corner point in the positive mesh
        //shoot a ray from every corner point in the negative mesh in the opposite direction
        //ray direction = make the 2 lines of the point into a parallelogram by virtually copying

        List<(Vector3, Vector3)> positiveLines = GenerateLines(positive);
        List<(Vector3, Vector3)> allRays = generateRays(positiveLines);
        List<(Vector3, Vector3)> allLines = new List<(Vector3, Vector3)>();
        allLines.Concat(positiveLines);

        for (int i = 0; i < negatives.Count; i++)
        {
            List<(Vector3, Vector3)> negativeLines = GenerateLines(negatives[i]);
            allLines.Concat(negativeLines);
            List<(Vector3, Vector3)> negativeRays = generateRays(negativeLines, true);

            for(int j = 0; j < negativeRays.Count; j++)
            {
                allRays.Add(negativeRays[i]);
            }
        }

        List<Vector3> allIntersections = new();

        //find all ray intersections by checking each ray against each other ray
        for(int i = 0; i < allRays.Count - 1; i++)
        {
            for (int j = i + 1; j < allRays.Count; j++)
            {
                bool intersects;
                Vector3 point = IntersectionPoint(allRays[i].Item1, allRays[i].Item2, allRays[j].Item1, allRays[j].Item2, out intersects, 3);
                
                if(!intersects)
                {
                    continue;
                }

                //check if the point is inside the positive polygon and outside the negative polygons
                if(CheckInPolygon(positive, point))
                {
                    bool addpoint = true;
                    for(int k = 0; k < negatives.Count; k++)
                    {
                        if (CheckInPolygon(negatives[k], point))
                        {
                            addpoint = false;
                            break;
                        }
                    }
                    if(addpoint)
                    {
                        allIntersections.Add(point);
                    }
                }
            }
        }

        return FindOptimalPath(allIntersections, allLines);
    }

    /// <summary>
    /// Find the optimal path using a greedy approach
    /// </summary>
    /// <param name="points"> the points to create a path between </param>
    /// <param name="walls"> all the lines of polygons (both positive and negative)</param>
    /// <returns> the path </returns>
    private PathData FindOptimalPath(List<Vector3> pathPoints, List<(Vector3, Vector3)> walls)
    {
        PathData result = new();

        result.points = new();
        result.radius = 1;      //arbitrarily chosen number, feel free to alter to something better

        //greedy appraoch: choose the closest intercection found (but only if it dont cross a polygon line), that is the next one on the path
        //how to choose first one: one closest to a wall? just the first intersection found?
        //lets start with it just being the first intersection and see how well that works out

        //initialize the first point on the path
        Vector3 latest = pathPoints[0];
        result.points.Add(latest);
        pathPoints.Remove(latest);

        while (pathPoints.Count > 0)
        {
            Vector3 best = pathPoints[0];
            float bestdist = math.sqrt(math.pow(latest.x - best.x, 2) + math.pow(latest.y - best.y, 2));

            //continually find and remove the best intersection in the remaining intersections
            for (int i = 1; i < pathPoints.Count; i++)
            {
                Vector3 current = pathPoints[i];

                //check if the distance to the current point is better than that of the best point
                float dist = math.sqrt(math.pow(latest.x - current.x, 2) + math.pow(latest.y - current.y, 2));
                if (dist >= bestdist)
                {
                    continue;
                }

                //check if the path between the previous and this point moves through any walls
                bool valid = true;
                for (int j = 0; j < walls.Count; j++)
                {
                    bool intersects;
                    _ = IntersectionPoint(latest, current, walls[j].Item1, walls[j].Item2, out intersects);

                    if (intersects)
                    {
                        valid = false;
                        break;
                    }
                }

                if (valid)
                {
                    best = current;
                    bestdist = dist;
                }
            }

            //add the best point to the end of the path and remove it from the list of remaining intersections
            result.points.Add(best);
            pathPoints.Remove(best);
        }

        return result;
    }

    private List<Vector3> ClusterPoints(List<Vector3> points)
    {

        //temp
        return new();
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
                if(!(t1 >= 0 && t1 <= 1 && t2 >= 0 && t2 <= 1))
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
            //line 1 is infinite only in the direction of point 2. line 2 is finite in foth directions
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
    /// turns the polygon into a list of its line segments signified by start and endpoint
    /// </summary>
    /// <param name="polygon">the polygon from which to create the list</param>
    /// <returns>list of line segments, signified by 2 points</returns>
    
    //potential (unit) test: check if the second item of every point is equal to the first item of the next point in the list
    private List<(Vector3, Vector3)> GenerateLines(Polygon polygon)
    {
        List<(Vector3, Vector3)> results = new();
        List<Vector3> points = polygon.GetPoints().ToList();
        //add a duplicate of the first point to the end of the list
        points.Add(new Vector3(points[0].x, points[0].y));

        for(int i = 0; i < points.Count -1; i++)
        {
            results.Add((points[i], points[i + 1]));
        }

        return results;
    }

    /// <summary>
    /// generate a list of rays based on a given list of line segments
    /// it is assumed each segment in the list is adjacent to the previous segment,
    /// and that the first segment is adjacent to the last
    /// </summary>
    /// <param name="lines">list of line segments</param>
    /// <param name="mode">if false, generate rays for a 'positive' polygon. Otherwise generate them for a 'negative' polygon</param>
    /// <returns>list of rays signified by 2 points. fired from point 1 in the direction of point 2.</returns>
    private List<(Vector3, Vector3)> generateRays(List<(Vector3, Vector3)> lines, bool mode = false)
    {
        List<(Vector3, Vector3)> rays = new();

        //generate all the rays
        //check the direction of the rays by comparing with line intersect in the correct mode
        //any who are in the incorrect direction (as for a positive) are flipped

        //add a copy of the first line segment to the end of the list
        lines.Add((new Vector3(lines[0].Item1.x, lines[0].Item1.y), new Vector3(lines[0].Item2.x, lines[0].Item2.y)));

        //generate all the rays
        for(int i = 0; i < lines.Count - 1; i++)
        {
            Vector3 rayStart = lines[i].Item1;
            Vector3 rayEnd = lines[i + 1].Item2 + (lines[i].Item1 - lines[i].Item2);
            rays.Add((rayStart, rayEnd));
        }

        //remove the copied line
        lines.RemoveAt(lines.Count - 1);
        //check if the direction of the ray is correct

        //warning for potential bug: if the #intersections is still even after flipping, then the ray intersects with its own origin point
        //if it does this, potential fixes are: slighlty move the origin point, or alter the intersection methods t1 and t2 checks to be 
        //to be '>' and '<' instead of '>=' and '<='.
        //i do not believe this can happen because the origin point should then intersect 2 polygon edges, thus always adding 2 to the number
        //of intersections, and so even will remain even and odd will remain odd, not influencing the outcome.

        for(int i = 0; i < rays.Count; i++)
        {
            int intersections = 0;

            for(int j = 0; j < lines.Count; j++)
            {
                bool intersects;
                //use mode 2 as we shoot a ray out of a point, looking for intersections with finite line segments
                var _ = IntersectionPoint(rays[i].Item1, rays[i].Item2, lines[j].Item1, lines[j].Item2, out intersects, 2);
                if (intersects) intersections++;
            }
            //if the number of intersections is even, it is going out of the polygon, and we need to flip the direction
            if(intersections % 2 == 0)
            {
                rays[i] = FlipRay(rays[i]);
            }
        }

        if(mode)
        {
            for (int i = 0; i < rays.Count; i++)
            {
                rays[i] = FlipRay(rays[i]);
            }
        }

        return rays;
    }

    /// <summary>
    /// flips a given ray, effectively spins it 180 degrees around its origin point.
    /// </summary>
    /// <param name="ray"> the ray to be flipped </param>
    /// <returns> the flipped ray </returns>
    private (Vector3, Vector3) FlipRay((Vector3, Vector3) ray)
    {
        Vector3 origin = ray.Item1;
        Vector3 destination = ray.Item1 - (ray.Item2 - ray.Item1);

        return (origin, destination);
    }

    private bool CheckInPolygon(Polygon polygon, Vector3 point)
    {

        //ideas:
        //shoot ray towards a random point in the polygon         //problem: point is adjacent to 2 edges
        //shoot ray towards average center point in the polygon   //problem: could still pass through a perfect edge point with low probability
        //do the second one

        List<(Vector3, Vector3)> lines = GenerateLines(polygon);

        Vector3 centerPolygonPoint = Vector3.zero;
        float averageFactor = 1 / polygon.GetPoints().Length;
        for(int i = 0; i < polygon.GetPoints().Length; i++)
        {
            centerPolygonPoint = centerPolygonPoint + (polygon.GetPoints()[i] * averageFactor);
        }

        (Vector3, Vector3) ray = (point, centerPolygonPoint);

        int intersections = 0;
        for (int j = 0; j < lines.Count; j++)
        {
            bool intersects;
            //use mode 2 as we shoot a ray out of a point, looking for intersections with finite line segments
            var _ = IntersectionPoint(ray.Item1, ray.Item2, lines[j].Item1, lines[j].Item2, out intersects, 2);
            if (intersects) intersections++;
        }

        //if the number of intersections is even, the point is outside of the polygon
        if (intersections % 2 == 0)
        {
            return false;
        }

        return true;
    }
}

    //genlines method voor de polygons. 1 voor beide tegelijk is goed                                    done
    //genrays method voor de polygons. pos en neg een apparte, omdat neg rays de andere kant op moeten.  done (alternatively)
    //alternatively, 1 voor beide, en dan een method om pos rays in neg rays te veranderen               done
    //method om ray intersections te calculaten en de intersection points te returnen                    done
    //method om een path te maken tussen deze intersection points                                        done (greedy)
    //maybe een method om nearby intersections te refinen door clusteren                                 optional todo
    //maybe een method om de 'roundness' van de polygon te detecten en de polygon te refinen door wat cornerpoints te mergen    optional todo
    //maybe (probably) make rays end at when they encounter an edge                                      todo?
    //find a way such that the startstate will always produce a path                                     todo

    //'thinning' method
    //potentially use from BV assignments:
    //drawline method, see 'Assignment 3'
    //floodfill method, see 'Assignment 2'
    //maybe convolve image? for hit/miss transform with thinning
    //ideas:
    //first, create a grid with a certain 'resolution'
    //tranform the polygons such that their points are the same relative to each other, but they also fit in the grid for the drawline method
    //create the grid properly with drawline and floodfill
    //apply thinning with the 'Golay' structuring elements found in BV slides
    //find the path by tracing the thinning residue. potentially make in-between 'node' structure
    //convert the node structure back into a regular path structure (or maybe not, discuss with Joep)
    //transform the path back into 'polygon-space'