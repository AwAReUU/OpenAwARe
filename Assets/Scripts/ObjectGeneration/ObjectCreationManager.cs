using System.Collections;
using System.Collections.Generic;
using System.Linq;

using AwARe.MonoBehaviours;

using UnityEngine;
using IngredientLists;
using IngredientPipeLine;

namespace ObjectGeneration
{
    /// <summary>
    /// Class <c>ObjectCreationManager</c> Handles placement of objects, 
    /// by managing some helper classes and implementing some placement methods.
    /// </summary>
    public class ObjectCreationManager : MonoBehaviour
    {
        [SerializeField] private PolygonManager polygonManager;
        [SerializeField] private GameObject placeButton;

        /// <value>
        /// <c>IngredientList</c> that we are going to render
        /// </value>
        private IngredientList selectedList { get; set; }

        /// <summary>
        /// Set the current ingredientList.
        /// </summary>
        /// <param name="ingredientList"><c>IngredientList</c> that we are going to render</param>
        public void SetSelectedList(IngredientList ingredientList) => selectedList = ingredientList;

        /// <summary>
        /// Obtain the currently selected ingredientList from the DontDestroyOnload-GameObject.
        /// </summary>
        /// <returns>Ingredientlist form the DontDestroyOnLoad-GameObject</returns>
        private IngredientList RetrieveIngredientlist() => Storage.Get().ActiveIngredientList;

        /// <summary>
        /// Called when the place button is clicked. Manages the conversion of the selected
        /// IngredientList to renderables, and the placement of the renderables in the scene.
        /// </summary>
        public void OnPlaceButtonClick()
        {        
            SetSelectedList(RetrieveIngredientlist());
            PipelineManager pipelineManager = new();
            List<Renderable> renderables = pipelineManager.GetRenderableList(selectedList);

            AutoGenerateObjects(renderables);
        }

        /// <summary>
        /// Tries to place a <paramref name="renderable"></paramref> at <paramref name="position"></paramref>
        /// </summary>
        /// <param name="renderable">Renderable object to place in the scene.</param>
        /// <param name="position">Exact position at which we will try to place the renderable.</param>
        /// <param name="polygonPoints">The polygon described by points in which we will try to place.</param>
        /// <returns>Whether the object has been placed.</returns>
        private bool TryPlaceObject(
            Renderable renderable,
            Vector3 position,
            List<Vector3> polygonPoints)
        {
            // Check if the box of the new object will overlap with any other colliders
            Vector3 boxCenter = position;
            boxCenter.y += renderable.GetHalfExtents().y;
            if (Physics.CheckBox(
                boxCenter,
                renderable.GetHalfExtents(),
                Quaternion.identity,
                LayerMask.GetMask("Placed Objects"))) //only check collisions with other materials.
                return false;

            // Check if the collider doesn't cross the polygon border
            List<Vector3> objectCorners = Renderable.CalculateColliderCorners(renderable, position);
            if (!PolygonHelper.ObjectColliderInPolygon(objectCorners, polygonPoints))
                return false;

            // Adjust object size according to scalar
            GameObject newObject = Instantiate(renderable.GetPrefab(), position, Quaternion.identity);
            newObject.layer = LayerMask.NameToLayer("Placed Objects");
            newObject.transform.localScale = 
                new Vector3(renderable.GetScaling(), renderable.GetScaling(), renderable.GetScaling());

            // Add collider after changing object size
            BoxCollider bc = newObject.AddComponent<BoxCollider>();

            CreateVisualBox(bc);

            return true;
        }

        /// <summary>
        /// Automatically place objects by trying multiple different spawnPoints
        /// on a polygon described by points.
        /// </summary>
        /// <param name="renderables">All items that we are going to place.</param>
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
                //this dictionary is reset for each different object, so that only clones of the same object
                //can be stacked on each other.
                Dictionary<Vector3, float> prefabStacks = new();

                for (int i = 0; i < renderable.GetQuantity(); i++) //quantity iterator
                    SpawnParticularObj(
                        renderable,
                        validSpawnPoints,
                        ref prefabStacks,
                        availableSurfaceArea,
                        ref currentRatioUsage,
                        polygonPoints);
            }
        }

        /// <summary>
        /// Estimate the surface area of the spawn polygon by squaring the distance between the points
        /// and multiplying this by a factor (not all space is usable on a sloped line).
        /// </summary>
        /// <param name="spawnPointCount">The amount of spawnPoints</param>
        /// <returns>Estimated surface area.</returns>
        private float EstimateAvailableSurfaceArea(int spawnPointCount) =>
            spawnPointCount * 0.1f * 0.1f * 0.9f;



        /// <summary>
        /// Try to spawn an object, and try to stack if it does not fit.
        /// </summary>
        /// <param name="renderable">Renderable to place.</param>
        /// <param name="validSpawnPoints">All spawnPoints at which it could be placed.</param>
        /// <param name="objStacks">Dictionary containing location of each instance of the current renderable.</param>
        /// <param name="availableSurfaceArea">The percentage of surface area that this renderable is allowed to use.</param>
        /// <param name="currentRatioUsage">The current percentage of surface area used by this renderable.</param>
        /// <param name="polygonPoints">Polygon to place the object in. Used later on to check if the renderable does not cross polygon border.</param>
        private void SpawnParticularObj(
            Renderable renderable,
            List<Vector3> validSpawnPoints,
            ref Dictionary<Vector3, float> objStacks,
            float availableSurfaceArea,
            ref float currentRatioUsage,
            List<Vector3> polygonPoints)
        {
            for (int i = 0; i < validSpawnPoints.Count; i++) //spawn iterator
            {
                float ratioUsage = renderable.GetHalfExtents().x * renderable.GetHalfExtents().z * 4 / availableSurfaceArea;
                if (currentRatioUsage + ratioUsage < renderable.allowedSurfaceUsage)
                {
                    bool hasPlaced = TryPlaceObject(renderable, validSpawnPoints[i], polygonPoints);
                    if (hasPlaced)
                    {
                        float height = validSpawnPoints[i].y + 2 * renderable.GetHalfExtents().y;
                        objStacks.Add(validSpawnPoints[i], height);
                        currentRatioUsage += ratioUsage;
                        return;
                    }
                }
            }
            //If the program reaches here, it ran out of available "ground space", and will need to stack.
            TryStack(renderable, ref objStacks, polygonPoints);
        }

        /// <summary>
        /// Try to stack the object on one of the stacks in <paramref name="objStacks"/> 
        /// until successful or out of stacks.
        /// </summary>
        /// <param name="renderable">Renderable to place.</param>
        /// <param name="objStacks">Dictionary containing locations of instances of this prefab.</param>
        /// <param name="polygonPoints">Polygon in which the renderable placed.</param>
        /// <returns>Whether the stacking was successful.</returns>
        private bool TryStack(
            Renderable renderable,
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
                //TODO: replace with height from digital twin roomscan.
                float stackHeight = objStacks[smallestStackPos];
                float newHeight = stackHeight + renderable.GetHalfExtents().y * 2;
                const float maxHeight = 3.0f;
                if (newHeight >= maxHeight)
                {
                    objStacks.Remove(smallestStackPos);
                    return false;
                }

                //Step 3: Test placement on new spot, check for collisions.
                Vector3 newPos = smallestStackPos;
                newPos.y = stackHeight;
                bool hasPlaced = TryPlaceObject(renderable, newPos, polygonPoints);
                if (hasPlaced)
                {
                    objStacks[smallestStackPos] = newHeight;
                    return true;
                }
                else
                    objStacks.Remove(smallestStackPos);
            }
            //Out of available stacks for this gameObject:
            return false;
        }

        /// <summary>
        /// Render a boxCollider that fades out after some time.
        /// </summary>
        /// <param name="boxCollider">BoxCollider to visualize.</param>
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

        /// <summary>
        /// Fade out the given <paramref name="target"/> GameObject during <paramref name="duration"/> seconds. 
        /// After this, destroy it.
        /// </summary>
        /// <param name="target">BoxCollider to fade out.</param>
        /// <param name="duration">Time that the animation will take.</param>
        /// <returns></returns>
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

        /// <summary>
        /// Rotate a gameObject to face the user.
        /// </summary>
        /// <param name="target">Object to rotate.</param>
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