using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using IngredientLists;

namespace ObjectGeneration
{
    public class ObjectCreationManager : MonoBehaviour
    {
        [SerializeField] private ARPlaneManager planeManager;
        [SerializeField] private PolygonManager polygonManager;
        [SerializeField] private GameObject placeButton;

        private IngredientList selectedList { get; set; }

        /// <summary>
        /// Set the current ingredientList.
        /// </summary>
        /// <param name="ingredientList"></param>
        public void SetSelectedList(IngredientList ingredientList) => selectedList = ingredientList;

        /// <summary>
        /// HalfExtents are distances from center to bounding box walls.
        /// </summary>
        /// <param name="prefab"></param>
        /// <returns></returns>
        public static Vector3 GetHalfExtents(GameObject prefab)
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
        /// <param name="forcePlace"></param>
        /// <returns></returns>
        private GameObject TryPlaceObject(
            Renderable renderable,
            Vector3 position,
            List<Vector3> polygonPoints)
        {
            // Check if the box of the new object will overlap with any other colliders
            Vector3 boxCenter = position;
            boxCenter.y += renderable.halfExtents.y;
            if (Physics.CheckBox(
                boxCenter,
                renderable.halfExtents,
                Quaternion.identity,
                LayerMask.GetMask("Placed Objects"))) //only check collisions with other materials.
                return null;

            // Check if the collider doesn't cross the polygon border
            List<Vector3> objectCorners = CalculateColliderCorners(renderable, position);
            if (!PolygonHelper.ObjectColliderInPolygon(objectCorners, polygonPoints))
                return null;

            // Adjust object size according to scalar
            GameObject newObject = Instantiate(renderable.prefab, position, Quaternion.identity);
            newObject.layer = LayerMask.NameToLayer("Placed Objects");
            newObject.transform.localScale = 
                new Vector3(renderable.scaling, renderable.scaling, renderable.scaling);

            // Add collider after changing object size
            BoxCollider bc = newObject.AddComponent<BoxCollider>();
            //RotateToUser(newObject);
            CreateVisualBox(bc);

            return newObject;
        }

        /// <summary>
        /// Called when the place button is clicked. 
        /// </summary>
        public void OnPlaceButtonClick()
        {
            PipelineManager pipelineManager = new();
            List<Renderable> renderables = pipelineManager.GetRenderableDict(selectedList);

            AutoGenerateObjects(renderables);
        }

        /// <summary>
        /// Generates a list of objects given a dictionary of (modelID, Quantity) 
        /// on a polygon given by points
        /// </summary>
        /// <param name="spawnDict"></param>
        /// <param name="polygon"></param>
        private void AutoGenerateObjects(List<Renderable> renderables)
        {
            //Polygon from scan:
            //GameObject polygon = polygonManager.GetPolygon();
            //List<Vector3> polygonPoints = polygon.GetComponent<Polygon>().GetPointsList();

            //Mock polygon:
            List<Vector3> polygonPoints = PolygonHelper.GetMockPolygon();

            PolygonSpawnPointHandler spawnPointHandler = new PolygonSpawnPointHandler(polygonPoints);

            List<Vector3> validSpawnPoints = spawnPointHandler.GetValidSpawnPoints();
            foreach (var renderable in renderables) //prefab iterator
            {
                float currentRatioUsage = 0;
                float availableSurfaceArea = EstimateAvailableSurfaceArea(validSpawnPoints.Count);
                //float spaceNeeded = ComputeSpaceNeeded(spawnDict);

                //If we place an object, store its position as key, and the height as value.
                //If we run out of ground space, we can start stacking the objects at these locations.
                //this dictionary is reset for each different objects, so that only clones of the same object
                //can be stacked on eachother.
                Dictionary<Vector3, float> prefabStacks = new();

                for (int i = 0; i < renderable.quantity; i++) //quantity iterator
                    SpawnParticularObj(
                        renderable,
                        validSpawnPoints,
                        ref prefabStacks,
                        availableSurfaceArea,
                        ref currentRatioUsage,
                        polygonPoints
                    );
            }
        }

        private float EstimateAvailableSurfaceArea(int spawnPoints) =>
            spawnPoints * 0.1f * 0.1f * 0.9f;


        /// <summary>
        /// try to spawn an object, and try to stack if it doesnt fit
        /// </summary>
        /// <param name="objIndex"></param>
        /// <param name="objAmount"></param>
        /// <param name="validSpawnPoints"></param>
        private void SpawnParticularObj(
            Renderable so,
            List<Vector3> validSpawnPoints,
            ref Dictionary<Vector3, float> objStacks,
            float availableSurfaceArea,
            ref float currentRatioUsage,
            List<Vector3> polygonPoints)
        {
            for (int j = 0; j < validSpawnPoints.Count; j++) //spawn iterator
            {
                float ratioUsage = so.halfExtents.x * so.halfExtents.z * 4 / availableSurfaceArea;
                if (currentRatioUsage + ratioUsage < so.allowedSurfaceUsage)
                {
                    GameObject placedObj = TryPlaceObject(so, validSpawnPoints[j], polygonPoints);
                    if (placedObj) //placement successful
                    {
                        float height = validSpawnPoints[j].y + 2 * so.halfExtents.y;
                        objStacks.Add(validSpawnPoints[j], height);
                        currentRatioUsage += ratioUsage;
                        return;
                    }
                }
            }
            //If the program reaches here, it ran out of "ground space", and will need to stack.
            TryStack(so, ref objStacks, polygonPoints);
        }

        /// <summary>
        /// The sum of the area of all gameobjects that will be spawned
        /// </summary>
        /// <param name="spawnDict"></param>
        /// <returns></returns>
        float ComputeSpaceNeeded(Dictionary<int, Renderable> spawnDict)
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
            Renderable so,
            ref Dictionary<Vector3, float> objStacks,
            List<Vector3> polygonPoints)
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
                GameObject placedObj = TryPlaceObject(so, newPos, polygonPoints);
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

        private List<Vector3> CalculateColliderCorners(Renderable renderable, Vector3 position)
        {
            List<Vector3> corners = new();

            // Get the size of the BoxCollider
            Vector3 size = renderable.halfExtents;

            // Calculate the corners
            corners.Add(position + new Vector3(-size.x, 0, -size.z));
            corners.Add(position + new Vector3(size.x, 0, -size.z));
            corners.Add(position + new Vector3(-size.x, 0, size.z));
            corners.Add(position + new Vector3(size.x, 0, size.z));

            return corners;
        }


        //debug method for displaying spawnlocations in scene.
        //void OnDrawGizmos()
        //{
        //    GameObject polygon = polygonManager.GetPolygon();
        //    List<Vector3> polygonPoints = polygon.GetComponent<Polygon>().GetPointsList();

        //    PolygonSpawnPointHandler spawnPointHandler = new(polygonPoints);
        //    List<Vector3> validSpawnPoints = spawnPointHandler.GetValidSpawnPoints();

        //    Gizmos.color = Color.red;
        //    foreach (var p in validSpawnPoints)
        //        Gizmos.DrawSphere(p, 0.05f);
        //}
    }
}