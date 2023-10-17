using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ObjectCreationManager : MonoBehaviour
{
    [SerializeField] GameObject spawnListButton;
    private List<Vector3> validSpawnLocations = new List<Vector3>();

    //<prefabId, quantity>
    private Dictionary<int, int> spawnDict = new Dictionary<int, int>() { {0,2}, {1,1}, {2,2}, {3,3}, {4,1} };

    void Start()
    {
        string foodType = FirstLetterToUpper("crops");
        string prefName = "garlic".ToLower();
        string prefabPath = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "Prefabs", foodType, prefName);
        Debug.Log(prefabPath);

        //string dbPath = @"C:\Users\Martijn\Documents\GitHub\awARe\Assets\Scripts\ObjectGeneration\awaredb.db";
        //string connectionString = $"Data Source={dbPath};Version=3;";
        //string q = "SELECT * FROM Ingredient WHERE IngredientID = 1;";

        IngredientResourceDatabaseMock dbm = new();

        //var result = GetIngredientDetails(1);
    }

    //public (string PrefName, string FoodType)? GetIngredientDetails(int ingredientId)
    //{
    //    using (var connection = new SQLiteConnection(connectionString))
    //    {
    //        connection.Open();
    //        string query = "SELECT PrefName, FoodType FROM Ingredient WHERE IngredientID = @id";

    //        using (var command = new SQLiteCommand(query, connection))
    //        {
    //            command.Parameters.AddWithValue("@id", ingredientId);

    //            using (var reader = command.ExecuteReader())
    //            {
    //                if (reader.Read())
    //                {
    //                    string prefName = reader["PrefName"].ToString();
    //                    string foodType = reader["FoodType"].ToString();
    //                    return (prefName, foodType);
    //                }
    //            }
    //        }
    //    }

    //    return null; // Return null if no data found
    //}


//private DataTable Query(string query)
//{
//    //TODO: REPLACE WITH REAL SQL QUERY
//    DataTable dt = new DataTable() { Columns = {
//            { "IngredientId", typeof(int) },
//            { "PrefName", typeof(string) },
//            { "FoodType", typeof(string) }
//    }, Rows = { { 1 }, { "Apple"}, { "fruit"} } };

//    return dt;
//}

public string FirstLetterToUpper(string str)
    {
        if (str == null)
            return null;

        if (str.Length > 1)
            return char.ToUpper(str[0]) + str.Substring(1);

        return str.ToUpper();
    }

    //C:\Users\Martijn\Documents\GitHub\awARe\Assets\Prefabs\Crops\garlic.fbx
    //C:\Users\Martijn\Documents\GitHub\awARe\Assets\Prefabs\Crops

    /// <summary>
    /// Temporarily instantiate the object to get the BoxCollider size
    /// </summary>
    /// <param name="g"></param>
    /// <returns></returns>
    private Vector3 GetHalfExtents(GameObject prefab) 
    {
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
        Vector3 newPosition = new Vector3(pose.position.x, pose.position.y + 0.1f, pose.position.z);

        // Create a list of colliders that prevent an object from being placed
        Collider[] overlappingColliders;

        // Check if the box overlaps with any other colliders
        if (!TryPlaceObject(ObjectPrefabs.I.prefabs[ObjectPrefabs.I.prefabIndex], newPosition, halfExtents))
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

    private bool TryPlaceObject(GameObject obj, Vector3 position, Vector3 halfExtents)
    {
        float sizeMultiplier = 1.0f;
        // Check if the box overlaps with any other colliders
        if (!Physics.CheckBox(
            position,
            halfExtents,
            Quaternion.identity,
            LayerMask.GetMask("Material"))) //only check collisions with other materials.
        {
            // Adjust object size according to scalar
            GameObject newObject = Instantiate(obj, position, Quaternion.identity);
            newObject.layer = LayerMask.NameToLayer("Material");
            newObject.transform.localScale = new Vector3(sizeMultiplier, sizeMultiplier, sizeMultiplier);

            // Add collider after changing object size
            _ = newObject.AddComponent<BoxCollider>();

            RotateToUser(newObject);

            return true;
        }
        else { Debug.Log("collision"); }

        return false;
    }
    //* Function is called whenever button is clicked to generate objects
    public void OnPlaceListButtonClick() 
    {
        AutoGenerateObjects(spawnDict);
    }
    public void AutoGenerateObjects(Dictionary<int,int> spawnDict_)
    {
        //objectAmount = int.Parse(inputAmount.text);
        //float sizeMultiplier = 1.0f;//float.Parse(inputSize.text);
        ARPlaneManager planeManager = GetComponent<ARPlaneManager>();

        validSpawnLocations = GetValidSpawnLocations(planeManager);

        foreach(var obj in spawnDict_) //prefab iterator
        {
            Vector3 halfExtents = GetHalfExtents(ObjectPrefabs.I.prefabs[obj.Key]);
            for (int i = 0; i < obj.Value; i++) //quantity iterator
            {
                for (int j = 0; j < validSpawnLocations.Count; j++) //spawn iterator
                {
                    if (TryPlaceObject(ObjectPrefabs.I.prefabs[obj.Key], validSpawnLocations[j], halfExtents))
                        break;
                    //else
                        //Debug.Log("placement failed");
                }
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
