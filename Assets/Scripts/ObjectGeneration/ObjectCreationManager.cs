using System;
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
        /// <summary>
        /// Used to communicate with the polygon.
        /// </summary>
        [SerializeField] private PolygonManager polygonManager;
        //[SerializeField] private GameObject placeButton;

        /// <value>
        /// <c>IngredientList</c> that we are going to render
        /// </value>
        private IngredientList SelectedList { get; set; }

        /// <summary>
        /// Set the current ingredientList.
        /// </summary>
        /// <param name="ingredientList"><c>IngredientList</c> that we are going to render</param>
        public void SetSelectedList(IngredientList ingredientList) => SelectedList = ingredientList;

        /// <summary>
        /// Obtain the currently selected ingredientList from the DontDestroyOnload-GameObject.
        /// </summary>
        /// <returns>The ingredient list that was selected by the user</returns>
        private IngredientList RetrieveIngredientlist() => Storage.Get().ActiveIngredientList;

        /// <summary>
        /// Called when the place button is clicked. Manages the conversion of the selected
        /// IngredientList to renderables, and the placement of the renderables in the scene.
        /// </summary>
        public void OnPlaceButtonClick()
        {
            SetSelectedList(RetrieveIngredientlist());
            PipelineManager pipelineManager = new();
            List<Renderable> renderables = pipelineManager.GetRenderableList(SelectedList);
            List<Vector3> polygonPoints = polygonManager.GetPolygon().GetPointsList();

            OnPlaceButtonClick(renderables, polygonPoints);
        }

        /// <summary>
        /// Called when the place button is clicked. Manages the conversion of the selected
        /// IngredientList to renderables, and the placement of the renderables in the scene.
        /// </summary>
        public void OnPlaceButtonClick(List<Renderable> renderables, List<Vector3> polygonPoints)
        {
            ObjectPlacer objectPlacer = new ObjectPlacer();
            objectPlacer.PlaceRenderables(renderables, polygonPoints);
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