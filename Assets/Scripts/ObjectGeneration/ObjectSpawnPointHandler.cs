using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using System.Linq;


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
    private readonly float gridSpacing_;
    private readonly ARPlaneManager planeManager_;

    [SerializeField] public Polygon polygon;
    [SerializeField] private List<Polygon> negPolygons = null;
    
    public ObjectSpawnPointHandler(Polygon polygon, List<Polygon> negPolygons, ARPlaneManager planeManager, float gridSpacing = 0.1f)
    {
        gridSpacing_ = gridSpacing;
        planeManager_ = planeManager;
        this.polygon = polygon;
        this.negPolygons = negPolygons;
    }
    /// <summary>
    /// Get a list of planes where objects are allowed to be spawned on.
    /// </summary>
    /// <param name="planeManager"></param>
    /// <returns></returns>
    private List<ARPlane> GetSpawnPlanes()
    {
        List<ARPlane> validPlanes = new List<ARPlane>();
        foreach (var plane in planeManager_.trackables)
        {
            if (plane.alignment == UnityEngine.XR.ARSubsystems.PlaneAlignment.Vertical)
                continue; //skip vertical planes
            if (plane.alignment == UnityEngine.XR.ARSubsystems.PlaneAlignment.HorizontalDown)
                continue; //skip ceilings
            validPlanes.Add(plane);
        }
        return validPlanes;
    }
    /*
    public List<Vector3> GetValidSpawnPoints()
    {
        List<Vector3> spawnPoints = new List<Vector3>();
        foreach (var plane in GetSpawnPlanes())
            spawnPoints.AddRange(GetGridPoints(plane, gridSpacing_));

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
    */
    public List<Vector3> GetValidSpawnPoints()
    {
        List<Vector3> spawnPoints = new();
        spawnPoints.AddRange(GetGridPoints(polygon, negPolygons, gridSpacing_));

        return spawnPoints;
    }

    private List<Vector3> GetGridPoints(Polygon polygon, List<Polygon> negPolygons, float spacing)
    {
        List<Vector3> result = new();

        Vector3[] polygonPoints = polygon.GetPoints();

        (float xMin, float zMin, float xMax, float zMax) = GetBounds(polygonPoints);
        float y = GetHeight(polygonPoints);
        Debug.Log(xMin);
        Debug.Log(xMax);
        Debug.Log(xMax);
        Debug.Log(zMax);
        //get all pnts in bounding box in grid pattern with space "spacing" in between.
        for (float x = xMin; x <= xMax; x += spacing)
        {
            for (float z = zMin; z <= zMax; z += spacing)
            {
                Vector3 gridPoint = new Vector3(x, y, z);

                Debug.Log(gridPoint);
                //if it hits the plane, we know that the gridpoint is on top of the plane.
                if (PointInPolygon(gridPoint, polygon) &&
                    PointNotInPolygons(gridPoint, negPolygons))
                    { result.Add(gridPoint); Debug.Log(gridPoint); }
            }
        }
        return result;
    }
    private bool PointInPolygon(Vector3 p, Polygon polygon)
    {
        Vector3[] points = polygon.GetPoints();
        
        bool inPolygon = false;
        int j = points.Length - 1;
        
        for (int i = 0; i < points.Length; i++)
        {
            Debug.Log(j);
            Debug.Log(i);
            Debug.Log(points.Length);
            if (points[i].z < p.z && points[j].z >= p.z ||
                points[j].z < p.z && points[i].z >= p.z)
            {
                if (points[i].x + (p.z - points[i].z) /
                    (points[j].z - points[i].z) *
                    (points[j].x - points[i].x) < p.x)
                {
                    inPolygon = !inPolygon;
                }
            }

            j = i;
        }
        Debug.Log(inPolygon);
        return inPolygon;
    }
    private bool PointNotInPolygons(Vector3 p, List<Polygon> polygons)
    {
        foreach(Polygon polygon in polygons)
        {
            if (PointInPolygon(p, polygon)) return false;
        }
        return true;
    }

    private (float, float, float, float) GetBounds(Vector3[] points)
    {
        float xMin = points[0].x;
        float zMin = points[0].z;
        float xMax = points[0].x;
        float zMax = points[0].z;
        for (int i = 1; i < points.Length;  i++)
        {
            xMin = Mathf.Min(xMin, points[i].x);
            zMin = Mathf.Min(zMin, points[i].z);
            xMax = Mathf.Max(xMax, points[i].x);
            zMax = Mathf.Max(zMax, points[i].z);
        }
        return (xMin, zMin, xMax, zMax);
    }
    private float GetHeight(Vector3[] points)
    {
        return points.Average(p => p.y);
    }
    
}
