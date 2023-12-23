// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

using AYellowpaper;
using UnityEngine;

namespace AwARe.Objects
{
    /// <summary>
    /// Undertakes and manages dynamic meshes. <br/>
    /// Sets the mesh filter to the mesh provided by logic member.
    /// </summary>
    public class Mesher : MonoBehaviour
    {
        /// <summary>
        /// The mesh-filter to control.
        /// </summary>
        public MeshFilter meshFilter;

        /// <summary>
        /// The mesh constructor.
        /// </summary>
        public InterfaceReference<IMesher> logic;

        // Tracking variables
        private bool newMesh = false;
        
        /// <summary>
        /// Gets or sets the mesh constructor.
        /// </summary>
        /// <value>
        /// The mesh constructor.
        /// </value>
        public IMesher Logic
        {
            get => logic.Value;
            set => logic = new(value);
        }

        /// <summary>
        /// Adds this component to a given GameObject and initializes the components members.
        /// </summary>
        /// <param name="gameObject">The GameObject this component is added to.</param>
        /// <param name="meshFilter">A meshfilter.</param>
        /// <param name="logic">A mesh constructor.</param>
        /// <returns>The added component.</returns>
        public static Mesher AddComponentTo(GameObject gameObject, MeshFilter meshFilter, IMesher logic)
        {
            var mesher = gameObject.AddComponent<Mesher>();
            mesher.meshFilter = meshFilter;
            mesher.Logic = logic;
            return mesher;
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
        /// Create and set a new mesh.
        /// </summary>
        private void CreateMesh()
        {
            meshFilter.mesh = Logic.Mesh;
            newMesh = false;
        }
    }
}