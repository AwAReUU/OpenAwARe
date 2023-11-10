using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.ARFoundation;


public interface IObjectSpawnPointHandler
{
    public List<Vector3> GetValidSpawnPoints();

}

public class TestObjectSpawnPointHandler : IObjectSpawnPointHandler
{
    public TestObjectSpawnPointHandler(ARPlaneManager planeManager = null, float gridSpacing = 0.1f)
    {
        gridSpacing_ = gridSpacing;
    }
    private readonly float gridSpacing_;

    /// <summary>
    /// Just return a hardcoded "fake plane" of spawnpoints
    /// </summary>
    /// <returns></returns>
    public List<Vector3> GetValidSpawnPoints()
    {
        List<Vector3> result = new List<Vector3>();
        float y = -0.87f;

        //get all pnts in bounding box in grid pattern with space "spacing" in between.
        for (float x = -8f; x <= -3; x += gridSpacing_)
        {
            for (float z = -8f; z <= -3; z += gridSpacing_)
            {
                Vector3 gridPoint = new Vector3(x, y, z);
                result.Add(gridPoint);
            }
        }
        return result;
    }
}

public class ObjectSpawnPointHandler : IObjectSpawnPointHandler
{
    private readonly float gridSpacing;
    private readonly ARPlaneManager planeManager;
    public ObjectSpawnPointHandler(ARPlaneManager manager, float spacing = 0.1f)
    {
        gridSpacing = spacing;
        planeManager = manager;
    }

    public ObjectSpawnPointHandler(float spacing = 0.1f)
    {
        gridSpacing = spacing;
    }

    /// <summary>
    /// Get a list of planes where objects are allowed to be spawned on.
    /// </summary>
    /// <param name="planeManager"></param>
    /// <returns></returns>
    private List<ARPlane> GetSpawnPlanes()
    {
        List<ARPlane> validPlanes = new List<ARPlane>();
        foreach (var plane in planeManager.trackables)
        {
            if (plane.alignment == UnityEngine.XR.ARSubsystems.PlaneAlignment.Vertical)
                continue; //skip vertical planes
            if (plane.alignment == UnityEngine.XR.ARSubsystems.PlaneAlignment.HorizontalDown)
                continue; //skip ceilings
            validPlanes.Add(plane);
        }
        return validPlanes;
    }

    public List<Vector3> GetValidSpawnPoints()
    {
        List<Vector3> spawnPoints = new List<Vector3>();
        foreach (var plane in GetSpawnPlanes())
            spawnPoints.AddRange(GetGridPoints(plane, gridSpacing));

        return spawnPoints;
    }

    // Gets A Vector3 list as input that represent the spawn plane (polygon)
    public List<Vector3> GetValidSpawnPoints(List<Vector3> polygon)
    {
        List<Vector3> spawnPoints = new List<Vector3>();
        spawnPoints = GetGridPoints(polygon, gridSpacing);

        return spawnPoints;
    }

    /// <summary>
    /// Divides a plane into a grid, and obtains all points on the intersections
    /// </summary>
    /// <param name="plane"></param>
    /// <param name="spacing"></param>
    /// <returns></returns>
    private List<Vector3> GetGridPoints(ARPlane plane, float spacing)
    {
        List<Vector3> result = new List<Vector3>();

        Bounds bounds = plane.gameObject.GetComponent<Renderer>().bounds;
        float y = plane.transform.position.y;

        //get all pnts in bounding box in grid pattern with space "spacing" in between.
        for (float x = bounds.min.x; x <= bounds.max.x; x += spacing)
        {
            for (float z = bounds.min.z; z <= bounds.max.z; z += spacing)
            {
                Vector3 gridPoint = new Vector3(x, y, z);

                //The ray origin is the pnt, but moved slightely up.
                Vector3 rayOrigin = gridPoint + Vector3.up;

                //cast a ray to the plane. check if it hits.
                Ray ray = new Ray(rayOrigin, -plane.normal);
                RaycastHit hit;

                //if it hits the plane, we know that the gridpoint is on top of the plane.
                if (Physics.Raycast(ray, out hit, 5))
                    if (hit.collider.gameObject == plane.gameObject)
                    { result.Add(gridPoint); Debug.Log(gridPoint); }
            }
        }
        return result;
    }

    private List<Vector3> GetGridPoints(List<Vector3> polygon, float spacing)
    {
        List<Vector3> result = new List<Vector3>();

        // Calculate the bounds of the polygon
        Bounds bounds = CalculateBounds(polygon);

        // Define the height of the polygon
        float y = polygon[0].y;

        // Get all points in bounding box in grid pattern with spacing "spacing" in between
        for (float x = bounds.min.x; x <= bounds.max.x; x += spacing)
        {
            for (float z = bounds.min.z; z <= bounds.max.z; z += spacing)
            {
                Vector3 gridPoint = new Vector3(x, y, z);

                // Check if the grid point is inside the polygon
                if (IsPointInsidePolygon(polygon, gridPoint))
                {
                    result.Add(gridPoint);
                    //Debug.Log(gridPoint);
                }
            }
        }
        return result;
    }

    private Bounds CalculateBounds(List<Vector3> points)
    {
        Bounds bounds = new Bounds(points[0], Vector3.zero);
        foreach (var point in points)
        {
            bounds.Encapsulate(point);
        }
        return bounds;
    }

    private bool IsPointInsidePolygon(List<Vector3> polygon, Vector3 point)
    {
        bool isInside = false;
        int j = polygon.Count - 1;

        for (int i = 0; i < polygon.Count; i++)
        {
            Vector3 pi = polygon[i];
            Vector3 pj = polygon[j];

            if (pi.z < point.z && pj.z >= point.z || pj.z < point.z && pi.z >= point.z)
            {
                if (pi.x + (point.z - pi.z) / (pj.z - pi.z) * (pj.x - pi.x) < point.x)
                {
                    isInside = !isInside;
                }
            }
            j = i;
        }

        return isInside;
    }

    private bool PointNotInPolygons(Vector3 p, List<Polygon> polygons)
    {
        foreach (Polygon polygon in polygons)
        {
            List<Vector3> points = polygon.GetPointsList();
            if (IsPointInsidePolygon(points, p)) return false;
        }
        return true;
    }
}
