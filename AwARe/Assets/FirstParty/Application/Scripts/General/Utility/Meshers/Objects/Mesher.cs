// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

using AwARe.Data.Objects;
using AYellowpaper;

using UnityEngine;

namespace AwARe.Objects
{
    /// <summary>
    /// Dynamic mesh behaviour.
    /// </summary>
    public class Mesher : MonoBehaviour
    {
        public MeshFilter meshFilter;
        public InterfaceReference<IMesher> logic;
        
        public IMesher Logic
        {
            get => logic.Value;
            set => logic = new(value);
        }

        private bool newMesh = false;
        
        public static void AddComponentTo(GameObject gameObject, MeshFilter meshFilter, IMesher logic)
        {
            var mesher = gameObject.AddComponent<Mesher>();
            mesher.meshFilter = meshFilter;
            mesher.Logic = logic;
        }

        private void Update()
        {
            if (newMesh)
                CreateMesh();
        }

        /// <summary>
        /// Update the mesh next Update-frame to represent the current data.
        /// </summary>
        public void UpdateMesh() =>
            newMesh = true;

        /// <summary>
        /// Create a new mesh for the GameObject.
        /// </summary>
        private void CreateMesh()
        {
            meshFilter.mesh = Logic.Mesh;
            newMesh = false;
        }
    }
}