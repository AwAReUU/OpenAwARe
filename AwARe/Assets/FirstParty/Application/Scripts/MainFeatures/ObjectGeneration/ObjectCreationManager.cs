// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using AwARe.Data.Objects;
using AwARe.InterScenes.Objects;
using AwARe.Objects;
using AwARe.ResourcePipeline.Logic;
using AwARe.ResourcePipeline.Objects;
using AwARe.RoomScan.Path;

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

        /// <value>
        /// <c>Room</c> that we are going to render.
        /// </value>
        private Data.Logic.Room SelectedRoom { get; set; }

        [SerializeField] private Data.Objects.Room roomObject;

        /// <value>
        /// list of renderables that are present in the current room.
        /// </value>
        public List<Renderable> currentRoomRenderables;

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

        private PathData path;

        private ObjGenUIHandler uiHandler;

        private void Start()
        {
            uiHandler = GetComponent<ObjGenUIHandler>();

            // Set the no list selected popup active if the selected list is null
            uiHandler.SetNoListSelectedPopup(RetrieveIngredientlist() == null);

            uiHandler.SetSplitRoomsPopup(false);
        }

        private void LoadRoom()
        {
            // Load data from storage
            Data.Logic.Room roomData = Storage.Get().ActiveRoom;
            if (roomData == null) return;

            // Construct new room
            SelectedRoom = roomData;
            roomObject.Data = SelectedRoom;

            path = Storage.Get().ActivePath;

            // Visualize new room
            var roomLiner = roomObject.GetComponent<RoomLiner>();
            roomLiner.ResetLiners();
            roomLiner.UpdateLines();

            ShowNegativePolygons(roomObject);
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

            if (SelectedRoom == null)
                Debug.LogError("No room selected");

            float roomSpace = SelectedRoom.PositivePolygon.Area;
            float renderablesSpace = ComputeRenderableSpaceNeeded(renderables);

            // Divide renderables in seperate rooms when there is not enough space 
            if (renderablesSpace > roomSpace)
            {
                PlaceRoom(true);
                uiHandler.SetSplitRoomsPopup(true);
            }
            else PlaceRenderables(renderables, SelectedRoom, path);
        }

        /// <summary>
        /// Try to place all <paramref name="renderables"/> inside of the <paramref name="room"/>.
        /// </summary>
        /// <param name="renderables">Objects to place in the Polygon.</param>
        /// <param name="room">Room consisting of polygons to place the objects in.</param>
        /// <param name="path">The path in the room.</param>
        public void PlaceRenderables(List<Renderable> renderables, Data.Logic.Room room, PathData path)
        {
            currentRoomRenderables = renderables;
            StartCoroutine(PlaceAfterDestroy(renderables, room, path));
        }

        /// <summary>
        /// Coroutine that only starts placing the renderables after all currently placed objects have been destroyed.
        /// </summary>
        /// <param name="renderables">Objects to place in the polygon.</param>
        /// <param name="room">Room consisting of polygons to place the objects in.</param>
        /// <param name="pathData">Mesh on which objects will not be placed.</param>
        /// <returns>IEnumerator.</returns>
        private IEnumerator PlaceAfterDestroy(List<Renderable> renderables, Data.Logic.Room room, PathData pathData)
        {
            //Wait untill de ObjectDestroyer is done.
            destroyer = gameObject.GetComponent<ObjectDestroyer>();
            yield return StartCoroutine(destroyer.DestroyAllObjects());
            new ObjectPlacer().PlaceRenderables(renderables, room, pathData);
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
                renderables = renderables.Where(renderable => renderable.ResourceType == ResourceType.Animal ||
                                                              renderable.ResourceType == ResourceType.Water).ToList();
            else
                renderables = renderables.Where(renderable => renderable.ResourceType == ResourceType.Plant ||
                                                              renderable.ResourceType == ResourceType.Water).ToList();


            Data.Logic.Room room = Storage.Get().ActiveRoom;
            PlaceRenderables(renderables, room, path);
        }

        /// <summary>
        /// Returns the total area that all given renderables will cover.
        /// </summary>
        /// <param name="renderables">All the renderables that will be included in the calculation.</param>
        /// <returns>The sum of space needed for the given Renderable list.</returns>
        public float ComputeRenderableSpaceNeeded(List<Renderable> renderables)
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
            foreach (Polygon p in room.negativePolygons)
                p.GetComponent<Mesher>().UpdateMesh();
        }

        [ExcludeFromCodeCoverage]
        /// <summary>
        /// displays spawn points on grid for debugging.
        /// </summary>.
        void OnDrawGizmos()
        {
            Data.Logic.Room room = Storage.Get().ActiveRoom;
            if (room == null)
                return;

            path = Storage.Get().ActivePath;

            PolygonSpawnPointHandler spawnPointHandler = new();
            List<Vector3> validSpawnPoints = spawnPointHandler.GetValidSpawnPoints(room, path);

            Gizmos.color = Color.red;
            foreach (var p in validSpawnPoints)
                Gizmos.DrawSphere(p, 0.05f);
        }
    }
}