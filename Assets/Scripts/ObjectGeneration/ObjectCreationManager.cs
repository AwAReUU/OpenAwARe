using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

public class ObjectCreationManager : MonoBehaviour
{
    [SerializeField] private ARPlaneManager planeManager;
    [SerializeField] private GameObject placeListButton;
    [SerializeField] private GameObject placeButton;

    public InputField inputAmount;
    public InputField inputSize;

    /// <summary>
    /// HalfExtents are distances from center to bounding box walls.
    /// </summary>
    /// <param name="g"></param>
    /// <returns></returns>
    private Vector3 GetHalfExtents(GameObject prefab)
    {
        //Temporarily instantiate the object to get the BoxCollider size
        GameObject tempObj = Instantiate(prefab, new Vector3(0, 0, 0), Quaternion.identity);
        BoxCollider tempCollider = tempObj.AddComponent<BoxCollider>();
        Vector3 halfExtents = tempCollider.size / 2;
        Destroy(tempObj);

        return halfExtents;
    }

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

        Vector3 halfExtents = GetHalfExtents(ObjectPrefabs.I.prefabs[ObjectPrefabs.I.prefabIndex]);

        // Create the position where the new object should be placed (+ add slight hover to prevent floor collisions)
        Vector3 newPosition = new Vector3(pose.position.x, pose.position.y, pose.position.z);

        // Create a list of colliders that prevent an object from being placed
        Collider[] overlappingColliders;

        // Check if the box overlaps with any other colliders
        if (!TryPlaceObject(ObjectPrefabs.I.prefabs[ObjectPrefabs.I.prefabIndex], newPosition, halfExtents, 1))
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

    private bool TryPlaceObject(GameObject obj, Vector3 position, Vector3 halfExtents, float sizeMultiplier)
    {
        // Check if the box overlaps with any other colliders
        if (!Physics.CheckBox(
            position,
            halfExtents,
            Quaternion.identity,
            LayerMask.GetMask("Material2"))) //only check collisions with other materials.
        {
            // Adjust object size according to scalar
            GameObject newObject = Instantiate(obj, position, Quaternion.identity);
            newObject.layer = LayerMask.NameToLayer("Material2");
            newObject.transform.localScale = new Vector3(sizeMultiplier, sizeMultiplier, sizeMultiplier);

            // Add collider after changing object size
            BoxCollider bc = newObject.AddComponent<BoxCollider>();
            //RotateToUser(newObject);
            CreateVisualBox(bc);

            return true;
        }
        else { Debug.Log("collision"); }

        return false;
    }

    //* Function is called whenever PlaceListButton is clicked to generate objects
    //* Spawns a hardcoded list.
    public void OnPlaceListButtonClick() 
    {
        Debug.Log("onplace list click");
        //<prefabId, quantity>
        Dictionary<int, int> spawnDict = new Dictionary<int, int>() 
        { { 0, 2 }, { 1, 1 }, { 2, 2 }, { 3, 3 }, { 4, 1 } };
        AutoGenerateObjects(spawnDict);
    }
    //* Function is called whenever PlaceButton is clicked to generate objects
    //* Spawns selected object from dropdown.
    public void OnPlaceButtonClick() 
    {
        Debug.Log("onplace click");
        int objectAmount = int.Parse(inputAmount.text);
        float sizeMultiplier = float.Parse(inputSize.text);
        AutoGenerateObjects(objectAmount, sizeMultiplier, ObjectPrefabs.I.prefabs[ObjectPrefabs.I.prefabIndex]);
    }

    /// <summary>
    /// overload method: spawn from dictionary.
    /// Will be replaced with material list
    /// </summary>
    /// <param name="spawnDict"></param>
    public void AutoGenerateObjects(Dictionary<int,int> spawnDict)
    {
        ObjectSpawnPointHandler osph = new(0.1f, planeManager);
        List<Vector3> validSpawnPoints = osph.GetValidSpawnPoints();

        foreach(var obj in spawnDict) //prefab iterator
        {
            Vector3 halfExtents = GetHalfExtents(ObjectPrefabs.I.prefabs[obj.Key]);

            for (int i = 0; i < obj.Value; i++) //quantity iterator
            {
                for (int j = 0; j < validSpawnPoints.Count; j++) //spawn iterator
                {
                    if (TryPlaceObject(ObjectPrefabs.I.prefabs[obj.Key], validSpawnPoints[j], halfExtents, 1))
                        break;
                    //Else-> spawning failed, try again.
                }
            }
        }
    }
    

    /// <summary>
    /// overload method: spawn from input by user.
    /// will be replaced by material list
    /// </summary>
    /// <param name="objectAmount"></param>
    /// <param name="sizeMultiplier"></param>
    /// <param name="gameObject"></param>
    public void AutoGenerateObjects(int objectAmount, float sizeMultiplier, GameObject gameObject)
    {
        Vector3 halfExtents = GetHalfExtents(gameObject);
        halfExtents *= sizeMultiplier;
        Debug.Log(halfExtents);

        ObjectSpawnPointHandler osph = new(0.1f, planeManager);
        List<Vector3> validSpawnPoints = osph.GetValidSpawnPoints();

        int materialsToPlace = objectAmount;
        for (int i = 0; i < validSpawnPoints.Count; i++)
        {
            if (TryPlaceObject(gameObject, validSpawnPoints[i], halfExtents, sizeMultiplier))
            {
                Debug.Log("placing successful");

                materialsToPlace--;
                if (materialsToPlace == 0)
                {
                    Debug.Log("finished placing all objects");
                    break;
                }
            }
            else
            {
                Debug.Log("placement failed");
            }
        }
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

    //debug method for displaying spawnlocations in scene.
    //void OnDrawGizmos()
    //{
    //    ObjectSpawnPointHandler osph = new(0.1f, planeManager);
    //    List<Vector3> validSpawnPoints = osph.GetValidSpawnPoints();

    //    Gizmos.color = Color.red;
    //    foreach (var p in validSpawnPoints)
    //        Gizmos.DrawSphere(p, 0.05f);
    //}
}
