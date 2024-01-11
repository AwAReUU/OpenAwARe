// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

using System.Collections.Generic;
using System.Linq;

using AwARe.Data.Objects;
using AwARe.InterScenes.Objects;
using AwARe.Objects;
using AwARe.ResourcePipeline.Logic;
using AwARe.ResourcePipeline.Objects;
using UnityEngine;
using Ingredients = AwARe.IngredientList.Logic;

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
        /// used to clear the scene from previously generated objects.
        /// </value>
        private ObjectDestroyer destroyer;

        /// <value>
        /// <c>IngredientList</c> that we are going to render.
        /// </value>
        private Ingredients.IngredientList SelectedList { get; set; }

        [SerializeField] private Data.Objects.Room selectedRoom;

        [SerializeField] private RoomLiner roomLiner;
        [SerializeField] private GameObject polygonPrefab;
        [SerializeField] private GameObject roomPrefab;

        /// <value>
        /// <c>path</c> the Mesh from the generated path.
        /// </value>
        private Mesh pathMesh { get; set; }

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
        /// The Polygon drawer.
        /// </summary>
        [SerializeField] private RoomScan.Polygons.Objects.PolygonDrawer polygonDrawer;
        
        void Awake() {
            this.pathMesh = new Mesh(); // Empty mesh for now. Once Path gen. is done, generate the mesh from PathData.
        }

        private void LoadRoom()
        {
            // Clean up last room
            if(selectedRoom != null) { Destroy(selectedRoom.gameObject); }
            selectedRoom = null;

            // Load data from storage
            Data.Logic.Room roomData = Storage.Get().ActiveRoom;
            if (roomData == null) return;

            // Construct new room
            selectedRoom = Instantiate(roomPrefab).GetComponent<Room>();
            var positivePolygon = Instantiate(polygonPrefab, selectedRoom.transform).GetComponent<Polygon>();
            positivePolygon.Data = roomData.PositivePolygon;
            selectedRoom.PositivePolygon = positivePolygon;

            var negativePolygons = new List<Polygon>();
            foreach (var p in roomData.NegativePolygons)
            {
                var negativePolygon = Instantiate(polygonPrefab, selectedRoom.transform).GetComponent<Polygon>();
                negativePolygon.Data = p;
                negativePolygons.Add(negativePolygon);
            }
            selectedRoom.NegativePolygons = negativePolygons;

            ShowNegativePolygons(selectedRoom);

            // Visualize new room
            //roomLiner = selectedRoom.GetComponent<RoomLiner>();
            //roomLiner.room = selectedRoom;
            //roomLiner.ResetLiners();
            //roomLiner.UpdateLines();
        }

        /// <summary>
        /// Called when the place button is clicked. Manages the conversion of the selected
        /// IngredientList to renderables, and the placement of the renderables in the scene.
        /// </summary>
        public void OnPlaceButtonClick()
        {
            SetSelectedList(RetrieveIngredientlist());

            List<Renderable> renderables = new PipelineManager().GetRenderableList(SelectedList);

            // Get the stored room as an object.
            LoadRoom();

            Data.Logic.Room roomData = selectedRoom.Data;
            float roomSpace        = roomData.PositivePolygon.Area;
            float renderablesSpace = ComputeRenderableSpaceNeeded(renderables);

            // Divide renderables in seperate rooms when there is not enough space 
            if (renderablesSpace > roomSpace) 
                PlaceRoom(true);
            else PlaceRenderables(renderables, roomData, this.pathMesh);
        }

        /// <summary>
        /// Try to place all <paramref name="renderables"/> inside of the <paramref name="room"/>.
        /// </summary>
        /// <param name="renderables">Objects to place in the Polygon.</param>
        /// <param name="room">Room consisting of polygons to place the objects in.</param>
        public void PlaceRenderables(List<Renderable> renderables, Data.Logic.Room room, Mesh pathMesh)
        {
            // clear the scene of any previously instantiated GameObjects 
            destroyer = gameObject.GetComponent<ObjectDestroyer>();
            destroyer.DestroyAllObjects();
            new ObjectPlacer().PlaceRenderables(renderables, room, pathMesh);
        }
        
        /// <summary>
        /// Tries to place a partial list of renderables by distributing renderables in two seperate rooms.
        /// </summary>
        /// <param name="isFirstRoom">Serves as a 'switch' between the two rooms.</param>
        public void PlaceRoom(bool isFirstRoom)
        {
            SetSelectedList(RetrieveIngredientlist());
            List<Renderable> renderables = new PipelineManager().GetRenderableList(SelectedList);

            if (isFirstRoom) 
                renderables = renderables.Where(renderable => renderable.resourceType == ResourceType.Animal ||
                                                              renderable.resourceType == ResourceType.Water).ToList();
            else 
                renderables = renderables.Where(renderable => renderable.resourceType == ResourceType.Plant).ToList();

            Data.Logic.Room room = Storage.Get().ActiveRoom;
            PlaceRenderables(renderables, room, this.pathMesh);
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

        /// <summary>
        /// Returns the total area that all given renderables will cover.
        /// </summary>
        /// <param name="renderables">All the renderables that will be included in the calculation.</param>
        private float ComputeRenderableSpaceNeeded(List<Renderable> renderables)
        {
            float sumArea = 0;
            foreach (var renderable in renderables) 
                sumArea += renderable.ComputeSpaceNeeded();
            
            return sumArea;
        }

        /// <summary>
        /// Shows the meshes of the negative polygons.
        /// </summary>
        /// <param name="room">The room of which the negative polygons should be shown.</param>
        private void ShowNegativePolygons(Data.Objects.Room room)
        {
            foreach (Polygon p in room.NegativePolygons)
                p.GetComponent<Mesher>().UpdateMesh();
        }

        //debug method for displaying spawn locations in scene.
        void OnDrawGizmos()
        {
            Data.Logic.Room room = Storage.Get().ActiveRoom;
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