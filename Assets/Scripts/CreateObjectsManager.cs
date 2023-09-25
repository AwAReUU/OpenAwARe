using System.Collections.Specialized;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class CreateObjectsManager : MonoBehaviour
{
    private List<Vector3> validSpawnLocations = new List<Vector3>();

    public void CreateObjectOnHit(ARRaycastHit hit, bool rotateToUser = true)
    {
        Pose pose = hit.pose;
        GameObject newObject = Instantiate(ObjectPrefabs.I.prefabs[ObjectPrefabs.I.prefabIndex], pose.position, pose.rotation);

        if (rotateToUser)
            RotateToUser(newObject);
    }

    private void RotateToUser(GameObject target)
    {
        Vector3 position = target.transform.position;
        Vector3 cameraPosition = Camera.main.transform.position;
        Vector3 direction = cameraPosition - position;
        Vector3 targetRotationEuler = Quaternion.LookRotation(direction).eulerAngles;
        Vector3 scaledEuler = Vector3.Scale(targetRotationEuler, target.transform.up.normalized);
        Quaternion targetRotation = Quaternion.Euler(scaledEuler);
        target.transform.rotation = targetRotation;
    }

    //* Function is called whenever button is clicked to generate objects
    public void AutoGenerateObjects()
    {
        ARPlaneManager planeManager = GetComponent<ARPlaneManager>();

        //TODO: add realistic colliders directly in the prefabs for better accuracy
        //* Temporarily instantiate the object to get the BoxCollider size
        GameObject tempObj = Instantiate(ObjectPrefabs.I.prefabs[ObjectPrefabs.I.prefabIndex], Vector3.zero, Quaternion.identity);
        BoxCollider tempCollider;
        if (tempObj.GetComponent<BoxCollider>() == null)
        {
            tempCollider = tempObj.AddComponent<BoxCollider>();
        }
        else
            tempCollider = tempObj.GetComponent<BoxCollider>();

        // Assuming the box collider is at the object's origin
        Vector3 halfExtents = tempCollider.size / 2;
        float centerHeight = tempCollider.size.y / 2;
        Destroy(tempObj);

        validSpawnLocations = GetValidSpawnLocations(planeManager);
        int materialsToPlace = 50;
        for (int i = 0; i < validSpawnLocations.Count; i++)
        {
            // Create the position where the new object should be placed (+ add slight hover to prevent floor collisions)
            Vector3 newPosition = validSpawnLocations[i] + new Vector3(0, centerHeight + 0.01f, 0);

            if (TryPlaceObject(ObjectPrefabs.I.prefabs[ObjectPrefabs.I.prefabIndex], newPosition, halfExtents, centerHeight))
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
            }
        }
    }

    private bool TryPlaceObject(GameObject gameObject, Vector3 position, Vector3 halfExtents, float centerHeight)
    {
        // Check if the box overlaps with any other colliders
        if (!Physics.CheckBox(position, halfExtents, Quaternion.identity))
        {
            GameObject newObject = Instantiate(gameObject, position, Quaternion.identity);
            newObject.AddComponent<BoxCollider>();
            RotateToUser(newObject);

            position.y -= centerHeight;
            return true;
        }

        return false;
    }

    private List<Vector3> GetGridPoints(ARPlane plane, float spacing = 0.1f)
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
                        result.Add(gridPoint);
            }
        }

        return result;
    }

    List<Vector3> GetValidSpawnLocations(ARPlaneManager planeManager)
    {
        List<Vector3> result = new List<Vector3>();
        foreach (var plane in planeManager.trackables)
        {
            if (plane.alignment == UnityEngine.XR.ARSubsystems.PlaneAlignment.Vertical)
                continue; //skip vertical planes
            if (plane.alignment == UnityEngine.XR.ARSubsystems.PlaneAlignment.HorizontalDown)
                continue; //skip ceilings

            List<Vector3> gridPoints = GetGridPoints(plane);
            result.AddRange(gridPoints);
        }
        
        return result;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        foreach (var p in validSpawnLocations)
        {
            Gizmos.DrawSphere(p, 0.05f);
        }
    }
}
