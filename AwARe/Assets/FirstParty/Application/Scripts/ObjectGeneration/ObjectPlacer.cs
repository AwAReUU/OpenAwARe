using RoomScan;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace ObjectGeneration
{
    public class ObjectPlacer
    {
        /// <summary>
        /// Tries to place a <paramref name="renderable"></paramref> at <paramref name="position"></paramref>.
        /// </summary>
        /// <param name="renderable">Renderable object to place in the scene.</param>
        /// <param name="position">Exact position at which we will try to place the renderable.</param>
        /// <param name="room">The room containing the polygons in which we will try to place.</param>
        /// <returns>Whether the object has been placed.</returns>
        private bool TryPlaceObject(
            Renderable renderable,
            Vector3 position,
            Room room)
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
            if (!PolygonHelper.ObjectColliderInPolygon(objectCorners, room))
                return false;

            // Adjust object size according to scalar
            GameObject newObject = Object.Instantiate(renderable.GetPrefab(), position, Quaternion.identity);
            newObject.layer = LayerMask.NameToLayer("Placed Objects");
            newObject.transform.localScale =
                new Vector3(renderable.GetScaling(), renderable.GetScaling(), renderable.GetScaling());

            // Add collider after changing object size
            BoxCollider bc = newObject.AddComponent<BoxCollider>();

            BoxCollidervisualizer visualBox = new(bc);

            return true;
        }

        /// <summary>
        /// Automatically place objects by trying multiple different spawnPoints
        /// on a polygon described by points.
        /// </summary>
        /// <param name="renderables">All items that we are going to place.</param>
        /// <param name="room">Room to place the renderables in.</param>
        public void PlaceRenderables(List<Renderable> renderables, Room room)
        {
            if (renderables.Count == 0)
                return;
            //Polygon from scan:
            //GameObject polygon = polygonManager.GetPolygon();
            //List<Vector3> polygonPoints = polygon.GetComponent<Polygon>().GetPointsList();

            //Mock polygon:
            //List<Vector3> polygonPoints = polygonManager.GetPolygon().GetPointsList();

            PolygonSpawnPointHandler spawnPointHandler = new PolygonSpawnPointHandler();
            List<Vector3> validSpawnPoints = spawnPointHandler.GetValidSpawnPoints(room);

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
                    SpawnRenderable(
                        renderable,
                        validSpawnPoints,
                        ref prefabStacks,
                        availableSurfaceArea,
                        ref currentRatioUsage,
                        room);
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
        /// <param name="room">Room to place the object in. Used later on to check if the renderable does not cross polygon borders.</param>
        private void SpawnRenderable(
            Renderable renderable,
            List<Vector3> validSpawnPoints,
            ref Dictionary<Vector3, float> objStacks,
            float availableSurfaceArea,
            ref float currentRatioUsage,
            Room room)
        {
            for (int i = 0; i < validSpawnPoints.Count; i++) //spawn iterator
            {
                float ratioUsage = renderable.GetHalfExtents().x * renderable.GetHalfExtents().z * 4 / availableSurfaceArea;
                if (currentRatioUsage + ratioUsage < renderable.allowedSurfaceUsage)
                {
                    bool hasPlaced = TryPlaceObject(renderable, validSpawnPoints[i], room);
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
            TryStack(renderable, ref objStacks, room);
        }

        /// <summary>
        /// Try to stack the object on one of the stacks in <paramref name="objStacks"/> 
        /// until successful or out of stacks.
        /// </summary>
        /// <param name="renderable">Renderable to place.</param>
        /// <param name="objStacks">Dictionary containing locations of instances of this prefab.</param>
        /// <param name="room">Room in which the renderables are placed.</param>
        /// <returns>Whether the stacking was successful.</returns>
        private bool TryStack(
            Renderable renderable,
            ref Dictionary<Vector3, float> objStacks,
            Room room)
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
                bool hasPlaced = TryPlaceObject(renderable, newPos, room);
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
    }
}