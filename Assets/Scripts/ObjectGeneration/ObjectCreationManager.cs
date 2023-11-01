using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using Databases;
using ResourceLists;
using AwARe.DataStructures;
using Unity.VisualScripting;

public class ObjectCreationManager : MonoBehaviour
{
    [SerializeField] private ARPlaneManager planeManager;
    [SerializeField] private GameObject placeButton;
    [SerializeField] private InputField inputSize;
    [SerializeField] private InputField inputPigAmount;
    [SerializeField] private InputField inputChickenAmount;
    [SerializeField] private InputField inputWheatAmount;
    [SerializeField] private InputField inputDuckAmount;

    /// <summary> HalfExtents are distances from center to bounding box walls. </summary>
    private Vector3 GetHalfExtents(GameObject prefab)
    {
        //Temporarily instantiate the object to get the BoxCollider size
        GameObject tempObj = Instantiate(prefab, new Vector3(0, 0, 0), Quaternion.identity);
        BoxCollider tempCollider = tempObj.AddComponent<BoxCollider>();
        Vector3 halfExtents = tempCollider.size / 2;
        Destroy(tempObj);

        return halfExtents;
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
        SpawnParams so,
        Vector3 position,
        bool forcePlace = false)
    {
        // Check if the box of the new object will overlap with any other colliders
        Vector3 boxCenter = position;
        boxCenter.y += so.halfExtents.y;
        if (!Physics.CheckBox(
            boxCenter,
            so.halfExtents,
            Quaternion.identity,
            LayerMask.GetMask("Material2")) || forcePlace) //only check collisions with other materials.
        {
            // Adjust object size according to scalar
            GameObject newObject = Instantiate(so.prefab, position, Quaternion.identity);
            newObject.layer = LayerMask.NameToLayer("Material2");
            newObject.transform.localScale = new Vector3(so.scaling, so.scaling, so.scaling);

            // Add collider after changing object size
            BoxCollider bc = newObject.AddComponent<BoxCollider>();
            //RotateToUser(newObject);
            CreateVisualBox(bc);

            return newObject;
        }
        //else { Debug.Log("collision"); }
        return null;
    }

    /// <summary> Given a dictionary in the form (resourceID, quantity), generates these resources and their respective quantities in the form of their corresponding GameObjects </summary>
    public void PrintResourceList()
    {
        // Get input values 
        int PigAmount = int.Parse(inputPigAmount.text);
        int ChickenAmount = int.Parse(inputChickenAmount.text);
        int WheatAmount = int.Parse(inputWheatAmount.text);
        int DuckAmount = int.Parse(inputDuckAmount.text);

        // Get databases
        MockupResourceDatabase resourceDatabase = new MockupResourceDatabase();
        MockupModelDatabase modelDatabase = new MockupModelDatabase();

        // make a list of (resourceID, Quantity)
        Dictionary<int, int> resourceList = new Dictionary<int, int>()
        {
          { 14, PigAmount     },
          { 13, ChickenAmount },
          { 17, WheatAmount   },
          { 15, DuckAmount    },
        };

        Dictionary<int, int> modelList = ResourceListToModelList(resourceList, resourceDatabase);
        Dictionary<int, SpawnParams> spawnDict = GenerateSpawnDict(modelList, modelDatabase);

        AutoGenerateObjects(spawnDict);
    }


    /// <summary> Converts the dictionary from the form (resourceID, Quantity) to the form (modelID, Quantity) </summary>
    public Dictionary<int, int> ResourceListToModelList(Dictionary<int, int> resourceList, MockupResourceDatabase resourceDatabase)
    {
        var modelList = new Dictionary<int, int>();
        foreach (var obj in resourceList)
        {
            int modelID = resourceDatabase.GetResource(obj.Key).ModelID;
            modelList[modelID] = obj.Value;
        }
        return modelList;
    }

    private Dictionary<int, SpawnParams> GenerateSpawnDict(Dictionary<int, int> modelList, MockupModelDatabase modelDatabase)
    {
        float sizeMultiplier = float.Parse(inputSize.text);
        Dictionary<int, SpawnParams> spawnDict = new();
        foreach (var kvp in modelList)
        {
            SpawnParams sp = new();
            string modelpath = @"Prefabs/" + modelDatabase.GetModel(kvp.Key).PrefabPath;
            GameObject model = Resources.Load<GameObject>(modelpath);
            Vector3 halfExtents = GetHalfExtents(model);
            float modelSizeMultiplier = modelDatabase.GetModel(kvp.Key).RealHeight / (2 * halfExtents.y) * sizeMultiplier;
            sp.prefab = model;
            sp.quantity = kvp.Value;
            sp.scaling = modelSizeMultiplier;
            sp.halfExtents = halfExtents * modelSizeMultiplier;
            spawnDict.Add(kvp.Key, sp);
        }

        spawnDict = SetSurfaceRatios(spawnDict);
        return spawnDict;
    }
    class SpawnParams
    {
        public int quantity;
        public float allowedSurfaceUsage; //percentage
        public GameObject prefab;
        public Vector3 halfExtents;
        public float scaling;
    }

    /// <summary> Generates the unity Gameobjects given a model list (modelID, Quantity) and a modelDatabase. The modelID is used to find the corresponding prefab name in the modelDatabase. </summary>
    private void AutoGenerateObjects(Dictionary<int, SpawnParams> spawnDict)
    {
        ObjectSpawnPointHandler osph = new(planeManager); //(<-remove the word "Test" to use scanned planes)

        List<Vector3> validSpawnPoints = osph.GetValidSpawnPoints();
        foreach (var obj in spawnDict) //prefab iterator
        {
            float currentRatioUsage = 0;
            float availableSurfaceArea = EstimateAvailableSurfaceArea(validSpawnPoints.Count);
            //float spaceNeeded = ComputeSpaceNeeded(spawnDict);

            //If we place an object, store its position as key, and the height as value.
            //If we run out of ground space, we can start stacking the objects at these locations.
            //this dictionary is reset for each different objects, so that only clones of the same object
            //can be stacked on eachother.
            Dictionary<Vector3, float> prefabStacks = new();

            for (int i = 0; i < obj.Value.quantity; i++) //quantity iterator
                SpawnParticularObj(
                    obj.Value,
                    validSpawnPoints,
                    ref prefabStacks,
                    availableSurfaceArea,
                    ref currentRatioUsage
                );
        }
    }

    private float EstimateAvailableSurfaceArea(int spawnPoints) =>
        spawnPoints * 0.1f * 0.1f * 0.9f;

    /// <summary>
    /// For each unique object, find out the percentage of space it will need.
    /// </summary>
    /// <param name="spawnDict"></param>
    /// <returns></returns>
    private Dictionary<int, SpawnParams> SetSurfaceRatios(Dictionary<int, SpawnParams> spawnDict)
    {
        //compute the sum of area of all gameobjects that will be spawned.
        float sumArea = 0;
        foreach (var so in spawnDict.Values)
        {
            int quantity = so.quantity;
            Vector3 halfExtents = so.halfExtents;
            float areaPerClone = halfExtents.x * halfExtents.z * 4;
            float areaClonesSum = areaPerClone * quantity;
            so.allowedSurfaceUsage = areaClonesSum;
            sumArea += areaClonesSum;
        }
        foreach (int key in spawnDict.Keys.ToList())
            spawnDict[key].allowedSurfaceUsage /= sumArea;

        return spawnDict;
    }

    /// <summary>
    /// try to spawn an object, and try to stack if it doesnt fit
    /// </summary>
    /// <param name="objIndex"></param>
    /// <param name="objAmount"></param>
    /// <param name="validSpawnPoints"></param>
    private void SpawnParticularObj(
        SpawnParams so,
        List<Vector3> validSpawnPoints,
        ref Dictionary<Vector3, float> objStacks,
        float availableSurfaceArea,
        ref float currentRatioUsage)
    {
        for (int j = 0; j < validSpawnPoints.Count; j++) //spawn iterator
        {
            float ratioUsage = so.halfExtents.x * so.halfExtents.z * 4 / availableSurfaceArea;
            if (currentRatioUsage + ratioUsage < so.allowedSurfaceUsage)
            {
                bool placedObj = TryPlaceObject(so, validSpawnPoints[j]);
                if (placedObj) //placement successful
                {
                    float height = validSpawnPoints[j].y + 2 * so.halfExtents.y;
                    objStacks.Add(validSpawnPoints[j], height);
                    currentRatioUsage += ratioUsage;
                    return;
                }
            }
            //else Debug.Log("Out of ratio");
        }
        //If the program reaches here, it ran out of "ground space", and will need to stack.
        TryStack(so, ref objStacks);
    }

    /// <summary>
    /// The sum of the area of all gameobjects that will be spawned
    /// </summary>
    /// <param name="spawnDict"></param>
    /// <returns></returns>
    float ComputeSpaceNeeded(Dictionary<int, SpawnParams> spawnDict)
    {
        //compute the sum of area of all gameobjects that will be spawned.
        float sumArea = 0;
        foreach (var obj in spawnDict)
        {
            int quantity = obj.Value.quantity;
            Vector3 halfExtents = obj.Value.halfExtents;
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
        SpawnParams so,
        ref Dictionary<Vector3, float> objStacks)
    {
        while (objStacks.Keys.ToList().Count > 0)
        {
            //Step 1: find the smallest stack.
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
            if (smallestStackHeight == float.MaxValue) //error scenario
                return false;

            //step 2: check if it doesnt reach through the roof.

            //prevent placement if that stack will reach higher than 3 meters
            //with the additional current object on top
            float stackHeight = objStacks[smallestStackPos];
            float newHeight = stackHeight + so.halfExtents.y * 2;
            const float maxHeight = 3.0f;
            if (newHeight >= maxHeight)
            {
                objStacks.Remove(smallestStackPos);
                return false;
            }

            //Step 3: Test placement on new spot, check for collisions.
            Vector3 newPos = smallestStackPos;
            newPos.y = stackHeight;
            GameObject placedObj = TryPlaceObject(so, newPos);
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
