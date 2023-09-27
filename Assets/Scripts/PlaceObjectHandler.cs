using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using System.Linq;

public class PlaceObjectHandler : MonoBehaviour
{
    [SerializeField]
    private GameObject prefab;

    private List<Vector3> allPnts = new List<Vector3>();
    private List<Vector3> GetVertices(ARPlane plane)
    {
        GameObject go = plane.gameObject;
        return new List<Vector3> { go.transform.position };
    }
    public void PlaceObjects()
    {
        ARPlaneManager planeManager = GetComponent<ARPlaneManager>();
        foreach (var plane in planeManager.trackables)
        {
            List<Vector3> planeVertices = GetVertices(plane);

            for (int i = 0; i < planeVertices.Count; i++)
            {
                GameObject o = Instantiate(prefab, planeVertices[i], Quaternion.identity);
            }
        }
    }

    public void PlaceObjectsList()
    {
        ARPlaneManager planeManager = GetComponent<ARPlaneManager>();

        // Temporarily instantiate the object to get the BoxCollider size
        GameObject tempObj = Instantiate(prefab, new Vector3(0, 0, 0), Quaternion.identity);
        BoxCollider tempCollider = tempObj.AddComponent<BoxCollider>();
        // Assuming the box collider is at the object's origin
        Vector3 halfExtents = tempCollider.size / 2;
        float center_Height = tempCollider.size.y / 2;
        Destroy(tempObj);

        //place the first object in the middle of the plane.
        //ARPlane plane = GetFirstPlane(planeManager.trackables);
        //middleObject = Instantiate(prefab, plane.gameObject.transform.position, Quaternion.identity);

        foreach (var plane in planeManager.trackables)
        {
            if (plane.alignment == UnityEngine.XR.ARSubsystems.PlaneAlignment.Vertical)
                continue;
            List<Vector3> gridPnts = GetGridPnts(plane);
            allPnts.AddRange(gridPnts);
        }

        int materialsToPlace = 50;
        for (int i = 0; i < allPnts.Count; i++) 
        {
            // Create the position where the new object should be placed (+ add slight hover to prevent floor collisions)
            Vector3 newPosition = new Vector3(allPnts[i].x, allPnts[i].y + center_Height + 0.01f, allPnts[i].z);

            if (TryPlaceObject(prefab, newPosition, halfExtents, center_Height))
            {
                Debug.Log("placing successful");

                materialsToPlace--;
                if (materialsToPlace == 0)
                {
                    Debug.Log("finished placing all objects");
                    return;
                }
            }
            else
            {
                Debug.Log("placement failed");
                //placement failed
            }
        }
        
    }

    private ARPlane GetFirstPlane(TrackableCollection<ARPlane> trackables)
    {
        foreach (var plane in trackables) 
        {
            Vector3 middle = plane.gameObject.transform.position;
            
            return plane;
        }
        return null;
    }

    /// <summary>
    /// Given a plane, find all pnts in a grid-like pattern. 
    /// </summary>
    /// <param name="plane"></param>
    /// <param name="spacing"></param>
    /// <returns></returns>
    private List<Vector3> GetGridPnts(ARPlane plane, float spacing = 0.1f)
    {
        List<Vector3> result = new List<Vector3>();

        Bounds bounds = plane.gameObject.GetComponent<Renderer>().bounds;
        //get all pnts in bounding box in grid pattern with space "spacing" in between.
        for (float x = bounds.min.x; x <= bounds.max.x; x += spacing)
        {
            for (float z = bounds.min.z; z <= bounds.max.z; z += spacing)
            {
                float y = plane.transform.position.y;
                Vector3 gridPoint = new Vector3(x, y, z);

                //check if it is on top of the plane or not
                Vector3 rayOrigin = gridPoint + Vector3.up;
                Ray ray = new Ray(rayOrigin, -plane.normal);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, 5))
                {
                    if (hit.collider.gameObject == plane.gameObject)
                    {
                        result.Add(gridPoint);
                    }
                }
            }
        }
        return result;
    }

    private bool TryPlaceObject(GameObject gameObject, Vector3 newPosition, Vector3 halfExtents, float center_height)
    {
        // Check if the box overlaps with any other colliders
        if (!Physics.CheckBox(newPosition, halfExtents, Quaternion.identity))
        {
            GameObject obj = Instantiate(gameObject, newPosition, Quaternion.identity);
            BoxCollider boxCollider = obj.AddComponent<BoxCollider>();

            // Customize the BoxCollider
            //boxCollider.size = tempCollider.size;
            //boxCollider.center = tempCollider.center;

            Vector3 position = obj.transform.position;
            position.y -= center_height;
            obj.transform.position = position;
            Vector3 cameraPosition = Camera.main.transform.position;
            Vector3 direction = cameraPosition - position;
            Vector3 targetRotationEuler = Quaternion.LookRotation(direction).eulerAngles;
            Vector3 scaledEuler = Vector3.Scale(targetRotationEuler, obj.transform.up.normalized);
            Quaternion targetRotation = Quaternion.Euler(scaledEuler);
            obj.transform.rotation *= targetRotation;
            return true;

        }
        return false;
    }

    void OnDrawGizmos() 
    {
        Gizmos.color = Color.red;
        foreach (var p in allPnts)
        {
            Gizmos.DrawSphere(p, 0.05f);
        }
    }
}