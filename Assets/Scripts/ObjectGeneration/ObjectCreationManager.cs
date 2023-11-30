using System.Collections.Generic;

using AwARe.MonoBehaviours;

using UnityEngine;
using IngredientLists;
using IngredientPipeLine;

using RoomScan;

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
        /// <c>IngredientList</c> that we are going to render.
        /// </value>
        private IngredientList SelectedList { get; set; }

        /// <summary>
        /// Set the current ingredientList.
        /// </summary>
        /// <param name="ingredientList"><c>IngredientList</c> that we are going to render.</param>
        public void SetSelectedList(IngredientList ingredientList) => SelectedList = ingredientList;

        /// <summary>
        /// Obtain the currently selected ingredientList from the DontDestroyOnload-GameObject.
        /// </summary>
        /// <returns>The ingredient list that was selected by the user.</returns>
        private IngredientList RetrieveIngredientlist() => Storage.Get().ActiveIngredientList;

        /// <summary>
        /// Called when the place button is clicked. Manages the conversion of the selected
        /// IngredientList to renderables, and the placement of the renderables in the scene.
        /// </summary>
        public void OnPlaceButtonClick()
        {
            SetSelectedList(RetrieveIngredientlist());

            List<Renderable> renderables = new PipelineManager().GetRenderableList(SelectedList);

            PlaceRenderables(renderables, polygonManager.Room);
        }

        /// <summary>
        /// Try to place all <paramref name="renderables"/> inside of the <paramref name="room"/>.
        /// </summary>
        /// <param name="renderables">Objects to place in the polygon.</param>
        /// <param name="room">Room consisting of polygons to place the objects in.</param>
        public void PlaceRenderables(List<Renderable> renderables, Room room) =>
            new ObjectPlacer().PlaceRenderables(renderables, room);

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
        void OnDrawGizmos()
        {
            Room room = polygonManager.Room;

            PolygonSpawnPointHandler spawnPointHandler = new();
            List<Vector3> validSpawnPoints = spawnPointHandler.GetValidSpawnPoints(room);

            Gizmos.color = Color.red;
            foreach (var p in validSpawnPoints)
                Gizmos.DrawSphere(p, 0.05f);
        }
    }
}