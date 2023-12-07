using System.Collections.Generic;

using AwARe.InterScenes.Objects;
using AwARe.ResourcePipeline.Objects;
using Rooms = AwARe.RoomScan.Polygons.Logic;
using Ingredients = AwARe.IngredientList.Logic;

using UnityEngine;
using AwARe.RoomScan.Path;

namespace AwARe.ObjectGeneration
{
    /// <summary>
    /// Class <c>ObjectCreationManager</c> Handles placement of objects, 
    /// by managing some helper classes and implementing some placement methods.
    /// </summary>
    public class ObjectCreationManager : MonoBehaviour
    {
        //[SerializeField] private GameObject placeButton;

        /// <value>
        /// <c>IngredientList</c> that we are going to render.
        /// </value>
        private Ingredients.IngredientList SelectedList { get; set; }

        /// <value>
        /// <c>path</c> the Mesh from the generated path.
        /// </value>
        private Mesh pathMesh { get; set; } = new Mesh(); // Empty mesh for now. Once Path gen. is done, generate the mesh from PathData.

        /// <summary>
        /// Set the current ingredientList.
        /// </summary>
        /// <param name="ingredientList"><c>IngredientList</c> that we are going to render.</param>
        public void SetSelectedList(Ingredients.IngredientList ingredientList) => SelectedList = ingredientList;

        /// <summary>
        /// Obtain the currently selected ingredientList from the DontDestroyOnload-GameObject.
        /// </summary>
        /// <returns>The ingredient list that was selected by the user.</returns>
        private Ingredients.IngredientList RetrieveIngredientlist() => Storage.Get().ActiveIngredientList;

        /// <summary>
        /// The polygon drawer.
        /// </summary>
        [SerializeField] private RoomScan.Polygons.Objects.PolygonDrawer polygonDrawer;

        /// <summary>
        /// Called when the place button is clicked. Manages the conversion of the selected
        /// IngredientList to renderables, and the placement of the renderables in the scene.
        /// </summary>
        public void OnPlaceButtonClick()
        {
            SetSelectedList(RetrieveIngredientlist());

            List<Renderable> renderables = new PipelineManager().GetRenderableList(SelectedList);
            Rooms.Room room = Storage.Get().ActiveRoom;
            if (room == null)
                return;

            polygonDrawer.DrawRoomPolygons(room);

            // TODO:
            // Once pathgen is done, create mesh from PathData
            // this.pathMesh = pathData.CreateMesh()

            PlaceRenderables(renderables, room, this.pathMesh);
        }

        /// <summary>
        /// Try to place all <paramref name="renderables"/> inside of the <paramref name="room"/>.
        /// </summary>
        /// <param name="renderables">Objects to place in the polygon.</param>
        /// <param name="room">Room consisting of polygons to place the objects in.</param>
        public void PlaceRenderables(List<Renderable> renderables, Rooms.Room room, Mesh pathMesh) =>
            new ObjectPlacer().PlaceRenderables(renderables, room, pathMesh);

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

        //debug method for displaying spawn locations in scene.
        void OnDrawGizmos()
        {
            Rooms.Room room = Storage.Get().ActiveRoom;
            if (room == null)
                return;

            PolygonSpawnPointHandler spawnPointHandler = new();
            List<Vector3> validSpawnPoints = spawnPointHandler.GetValidSpawnPoints(room, this.pathMesh);

            Gizmos.color = Color.red;
            foreach (var p in validSpawnPoints)
                Gizmos.DrawSphere(p, 0.05f);
        }
    }
}