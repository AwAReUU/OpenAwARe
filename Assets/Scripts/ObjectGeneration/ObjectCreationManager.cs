using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

public class ObjectCreationManager : MonoBehaviour
{
    [SerializeField] private ARPlaneManager planeManager;
    [SerializeField] private GameObject placeListButton;
    [SerializeField] private GameObject placeButton;

    [SerializeField] private InputField inputAmount;
    [SerializeField] private InputField inputSize;

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
            GameObject newObject = Instantiate(ObjectPrefabsObjectGen.I.prefabs[ObjectPrefabsObjectGen.I.prefabIndex], pose.position, pose.rotation);

            if (rotateToUser)
                RotateToUser(newObject);

            return;
        }

        Vector3 halfExtents = GetHalfExtents(ObjectPrefabsObjectGen.I.prefabs[ObjectPrefabsObjectGen.I.prefabIndex]);

        // Create the position where the new object should be placed (+ add slight hover to prevent floor collisions)
        Vector3 newPosition = new Vector3(pose.position.x, pose.position.y, pose.position.z);

        // Create a list of colliders that prevent an object from being placed
        Collider[] overlappingColliders;

        // Check if the box overlaps with any other colliders
        if (!TryPlaceObject(ObjectPrefabsObjectGen.I.prefabs[ObjectPrefabsObjectGen.I.prefabIndex], newPosition, halfExtents, 1))
        {
            Debug.Log("Can't place object. It would overlap with another.");

            // Get all overlapping colliders + draw a fading cube around each overlapping collider for visualization
            overlappingColliders = Physics.OverlapBox(newPosition, halfExtents, pose.rotation);
            foreach (var collider in overlappingColliders)
            {
                BoxCollider box = collider as BoxCollider;
                if (box)
                    CreateVisualBox(box);
            }
        }
    }

    /// <summary>
    /// return gameobject if successful
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="position"></param>
    /// <param name="halfExtents"></param>
    /// <param name="sizeMultiplier"></param>
    /// <returns></returns>
    private GameObject TryPlaceObject(
        GameObject obj,
        Vector3 position,
        Vector3 halfExtents,
        float sizeMultiplier,
        bool forcePlace = false)
    {
        // Check if the box of the new object will overlap with any other colliders
        Vector3 boxCenter = position;
        boxCenter.y += halfExtents.y;
        if (!Physics.CheckBox(
            boxCenter,
            halfExtents,
            Quaternion.identity,
            LayerMask.GetMask("Material2")) || forcePlace) //only check collisions with other materials.
        {
            // Adjust object size according to scalar
            GameObject newObject = Instantiate(obj, position, Quaternion.identity);
            newObject.layer = LayerMask.NameToLayer("Material2");
            newObject.transform.localScale = new Vector3(sizeMultiplier, sizeMultiplier, sizeMultiplier);

            // Add collider after changing object size
            BoxCollider bc = newObject.AddComponent<BoxCollider>();
            //RotateToUser(newObject);
            CreateVisualBox(bc);

            return newObject;
        }
        //else { Debug.Log("collision"); }

        return null;
    }

    //* Function is called whenever PlaceListButton is clicked to generate objects
    //* Spawns a hardcoded list.
    public void OnPlaceListButtonClick()
    {
        //<prefabId, quantity>
        Dictionary<int, int> spawnDict = new Dictionary<int, int>()
        { { 0, 16 }, { 1, 20 }, { 2, 8 }, { 3, 12 }, { 4, 20 } };
        AutoGenerateObjects(spawnDict);
    }
    //* Function is called whenever PlaceButton is clicked to generate objects
    //* Spawns selected object from dropdown.
    public void OnPlaceButtonClick()
    {
        int objectAmount = int.Parse(inputAmount.text);
        float sizeMultiplier = float.Parse(inputSize.text);
        AutoGenerateObjects(objectAmount, sizeMultiplier, ObjectPrefabsObjectGen.I.prefabs[ObjectPrefabsObjectGen.I.prefabIndex]);
    }

    /// <summary>
    /// overload method: spawn from dictionary.
    /// Will be replaced with material list
    /// </summary>
    /// <param name="spawnDict"></param>
    public void AutoGenerateObjects(Dictionary<int,int> spawnDict)
    {
        TestObjectSpawnPointHandler osph = new(planeManager); //(<-remove the word "Test" to use actual planes)
        List<Vector3> validSpawnPoints = osph.GetValidSpawnPoints();
        Dictionary<int, float> areaRatios = GetAreaRatios(spawnDict);
        foreach (var obj in spawnDict) //prefab iterator
        {
            Debug.Log("spawning: " + ObjectPrefabsObjectGen.I.prefabs[obj.Key].name);
            //Debug.Log("Allowed ratio: " + areaRatios[obj.Key]);
            float allowedRatioUsage = areaRatios[obj.Key];
            float currentRatioUsage = 0;
            float availableSurfaceArea = EstimateAvailableSurfaceArea(validSpawnPoints.Count);
            float spaceNeeded = ComputeSpaceNeeded(spawnDict);
            //Debug.Log("available spawnSpace: " + availableSurfaceArea);
            //Debug.Log("spaceNeeded: " + spaceNeeded);
            //If we place an object, store its position as key, and the height as value.
            //If we run out of ground space, we can start stacking the objects at these locations.
            //this dictionary is reset for each different objects, so that only clones of the same object
            //can be stacked on eachother.
            Dictionary<Vector3, float> objStacks = new();

            for (int i = 0; i < obj.Value; i++) //quantity iterator
                SpawnParticularObj(
                    obj.Key,
                    validSpawnPoints,
                    ref objStacks,
                    allowedRatioUsage,
                    availableSurfaceArea,
                    ref currentRatioUsage);
        }
    }

    private float EstimateAvailableSurfaceArea(int spawnPoints) =>
        spawnPoints * 0.1f * 0.1f * 0.9f;

    /// <summary>
    /// For each unique object, find out the percentage of space it will need.
    /// </summary>
    /// <param name="spawnDict"></param>
    /// <returns></returns>
    private Dictionary<int, float> GetAreaRatios(Dictionary<int, int> spawnDict)
    {
        Dictionary<int, float> areaRatios = new();
        //compute the sum of area of all gameobjects that will be spawned.
        float sumArea = 0;
        foreach (var obj in spawnDict)
        {
            int quantity = obj.Value;
            GameObject curObj = ObjectPrefabsObjectGen.I.prefabs[obj.Key];
            Vector3 halfExtents = GetHalfExtents(curObj);
            float areaPerClone = halfExtents.x * halfExtents.z * 4;
            float areaClonesSum = areaPerClone * quantity;
            areaRatios.Add(obj.Key, areaClonesSum);
            sumArea += areaClonesSum;
        }
        foreach (int key in areaRatios.Keys.ToList())
            areaRatios[key] /= sumArea;

        return areaRatios;
    }

    /// <summary>
    /// try to spawn an object, and try to stack if it doesnt fit
    /// </summary>
    /// <param name="objIndex"></param>
    /// <param name="objAmount"></param>
    /// <param name="validSpawnPoints"></param>
    private void SpawnParticularObj(
        int objIndex,
        List<Vector3> validSpawnPoints,
        ref Dictionary<Vector3, float> objStacks,
        float allowedRatioUsage,
        float availableSurfaceArea,
        ref float currentRatioUsage)
    {
        GameObject curObj = ObjectPrefabsObjectGen.I.prefabs[objIndex];
        Vector3 halfExtents = GetHalfExtents(curObj);

        for (int j = 0; j < validSpawnPoints.Count; j++) //spawn iterator
        {
            float ratioUsage = (halfExtents.x * halfExtents.z * 4) / availableSurfaceArea;
            if (currentRatioUsage + ratioUsage < allowedRatioUsage)
            {
                GameObject placedObj = TryPlaceObject(curObj, validSpawnPoints[j], halfExtents, 1);
                if (placedObj) //placement successful
                {
                    float height = placedObj.transform.position.y + 2 * halfExtents.y; 
                    objStacks.Add(validSpawnPoints[j], height);
                    currentRatioUsage += ratioUsage;
                    return;
                }
            }
            //else Debug.Log("Out of ratio");
        }
        //If the program reaches here, it ran out of "ground space", and will need to stack.
        TryStack(curObj, ref objStacks, halfExtents);
    }

    float ComputeSpaceNeeded(Dictionary<int, int> spawnDict)
    {
        //compute the sum of area of all gameobjects that will be spawned.
        float sumArea = 0;
        foreach (var obj in spawnDict)
        {
            int quantity = obj.Value;
            GameObject curObj = ObjectPrefabsObjectGen.I.prefabs[obj.Key];
            Vector3 halfExtents = GetHalfExtents(curObj);
            float area = halfExtents.x * halfExtents.z * 4;
            sumArea += area * quantity;
        }

        return sumArea;
    }

    /// <summary>
    /// Given a prefab, size (in form of halfextents) and a objStacks, try to 
    /// stack the object on each key of objStacks until successful.
    /// </summary>
    /// <param name="gameObject">prefab</param>
    /// <param name="objStacks">dictionary containing locations of instances of this prefab</param>
    /// <param name="halfExtents">size of the prefab</param>
    /// <returns></returns>
    private bool TryStack(
        GameObject gameObject,
        ref Dictionary<Vector3, float> objStacks,
        Vector3 halfExtents)
    {
        while (objStacks.Keys.ToList().Count > 0)
        {
            float smallestStackHeight = float.MaxValue;
            Vector3 smallestStackPos = Vector3.zero;
            foreach (KeyValuePair<Vector3, float> kvp in objStacks)
            {
                if (kvp.Value < smallestStackHeight)
                {
                    smallestStackHeight = kvp.Value;
                    smallestStackPos = kvp.Key;
                }
            }
            if (smallestStackHeight == float.MaxValue)
                return false;

            //prevent placement if that stack will reach higher than 3 meters
            //with the additional current object on top
            float stackHeight = objStacks[smallestStackPos];
            float newHeight = stackHeight + halfExtents.y * 2;
            const float maxHeight = 3.0f;
            if (newHeight >= maxHeight)
            {
                objStacks.Remove(smallestStackPos);
                return false;
            }

            Vector3 newPos = smallestStackPos;
            newPos.y = stackHeight;

            GameObject placedObj = TryPlaceObject(gameObject, newPos, halfExtents, 1);
            if (placedObj)
            {
                objStacks[smallestStackPos] = newHeight;
                return true;
            }
            else 
                objStacks.Remove(smallestStackPos);
        }
        //Debug.Log("out of available stacks for this gameobject");
        return false;
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

        ObjectSpawnPointHandler osph = new(planeManager);
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
    
    public void DestroyAllObjects()
    {
        GameObject[] animals = GameObject.FindGameObjectsWithTag("Animal");
        foreach (GameObject animal in animals)
        {
            Destroy(animal);
        }
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
