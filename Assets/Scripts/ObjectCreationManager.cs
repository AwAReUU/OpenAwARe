using System.Collections.Specialized;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ObjectCreationManager : MonoBehaviour
{
    private List<Vector3> validSpawnLocations = new List<Vector3>();

    public void TryPlaceObjectOnTouch(ARRaycastHit hit, bool rotateToUser = true, bool forceCreate = false)
    {
        Pose pose = hit.pose;

        if (forceCreate)
        {
            //* Ignore any constrains and force create the object, only requirement being a plane hit
            GameObject newObject = Instantiate(ObjectPrefabs.I.prefabs[ObjectPrefabs.I.prefabIndex], pose.position, pose.rotation);

            if (rotateToUser)
                RotateToUser(newObject);

            return;
        }

        // Temporarily instantiate the object to get the BoxCollider size
        GameObject tempObj = Instantiate(ObjectPrefabs.I.prefabs[ObjectPrefabs.I.prefabIndex], new Vector3(0, 0, 0), Quaternion.identity);
        BoxCollider tempCollider = tempObj.AddComponent<BoxCollider>();
        Vector3 halfExtents = tempCollider.size / 2;
        float centerHeight = tempCollider.size.y / 2;
        Destroy(tempObj);

        // Create the position where the new object should be placed (+ add slight hover to prevent floor collisions)
        Vector3 newPosition = new Vector3(pose.position.x, pose.position.y + centerHeight + 0.1f, pose.position.z);

        // Create a list of colliders that prevent an object from being placed
        Collider[] overlappingColliders;

        // Check if the box overlaps with any other colliders
        if (!TryPlaceObject(ObjectPrefabs.I.prefabs[ObjectPrefabs.I.prefabIndex], newPosition, halfExtents, centerHeight))
        {
            // Log a message if you want to know when an object can't be placed
            Debug.Log("Can't place object. It would overlap with another.");

            // Get all overlapping colliders + draw a fading cube around each overlapping collider for visualization
            overlappingColliders = Physics.OverlapBox(newPosition, halfExtents, pose.rotation);
            foreach (var collider in overlappingColliders)
            {
                BoxCollider box = collider as BoxCollider;
                if (box)
                {
                    CreateVisualBox(box);
                }
            }
        }
    }

    private bool TryPlaceObject(GameObject obj, Vector3 position, Vector3 halfExtents, float centerHeight)
    {
        // Check if the box overlaps with any other colliders
        if (!Physics.CheckBox(position, halfExtents, Quaternion.identity))
        {
            GameObject newObject = Instantiate(obj, position, Quaternion.identity);
            newObject.AddComponent<BoxCollider>();
            RotateToUser(newObject);

            position.y -= centerHeight;
            newObject.transform.position = position;

            return true;
        }

        return false;
    }

    //* Function is called whenever button is clicked to generate objects
    public void AutoGenerateObjects()
    {
        ARPlaneManager planeManager = GetComponent<ARPlaneManager>();

        //TODO: add realistic colliders directly in the prefabs for better accuracy (I've made this a task in backlog)
        // Temporarily instantiate the object to get the BoxCollider size
        GameObject tempObj = Instantiate(ObjectPrefabs.I.prefabs[ObjectPrefabs.I.prefabIndex], Vector3.zero, Quaternion.identity);
        BoxCollider tempCollider = tempObj.AddComponent<BoxCollider>();

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


    public void CreateVisualBox(BoxCollider boxCollider)
    {
        // Create a new GameObject
        GameObject visualBox = new GameObject("Visual Box");

        // Set visual box properties
        MeshRenderer meshRenderer = visualBox.AddComponent<MeshRenderer>();
        MeshFilter meshFilter = visualBox.AddComponent<MeshFilter>();
        meshFilter.mesh = Resources.GetBuiltinResource<Mesh>("Cube.fbx");
        meshRenderer.material = new Material(Shader.Find("Standard"));
        meshRenderer.material.color = new Color(1.0f, 0.0f, 0.0f, 1.0f);

        // Set the size, position, and rotation
        visualBox.transform.position = boxCollider.transform.TransformPoint(boxCollider.center);
        visualBox.transform.rotation = boxCollider.transform.rotation;
        visualBox.transform.localScale = Vector3.Scale(boxCollider.transform.lossyScale, boxCollider.size);

        // Transparent material
        meshRenderer.material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
        meshRenderer.material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        meshRenderer.material.SetInt("_ZWrite", 0);
        meshRenderer.material.DisableKeyword("_ALPHATEST_ON");
        meshRenderer.material.DisableKeyword("_ALPHABLEND_ON");
        meshRenderer.material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
        meshRenderer.material.renderQueue = 3000;

        // Start fading out the visualBox
        StartCoroutine(FadeOutAndDestroy(visualBox, 3f));
    }

    public IEnumerator FadeOutAndDestroy(GameObject target, float duration)

    {
        float counter = 0;
        MeshRenderer meshRenderer = target.GetComponent<MeshRenderer>();
        Color startColor = meshRenderer.material.color;

        while (counter < duration)
        {
            counter += Time.deltaTime;
            float alpha = Mathf.Lerp(1, 0, counter / duration);
            meshRenderer.material.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            yield return null;
        }

        Destroy(target);
    }
    
    public void DestroyAllObjects()
    {
        GameObject[] animals = GameObject.FindGameObjectsWithTag("Animal");
        foreach (GameObject animal in animals)
        {
            Destroy(animal);
        }
    }

    //? What is this for? It's never used.
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        foreach (var p in validSpawnLocations)
        {
            Gizmos.DrawSphere(p, 0.05f);
        }
    }
}
