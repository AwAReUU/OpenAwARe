// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

using UnityEngine;

namespace AwARe.Data.Objects
{
    /// <summary>
    /// The component for storing polygon data.
    /// </summary>
    public class Polygon : MonoBehaviour, IDataHolder<Logic.Polygon>
    {
        [SerializeField] private Logic.Polygon data;

        /// <summary>
        /// Gets or sets the data-type Polygon represented by this GameObject.
        /// </summary>
        /// <value>
        /// The data-type Polygon represented.
        /// </value>
        public Logic.Polygon Data { get => data; set => data = value; }

        /// <summary>
        /// Adds this component to a given GameObject and initializes the components members.
        /// </summary>
        /// <param name="gameObject">The GameObject this component is added to.</param>
        /// <param name="room">A room.</param>
        /// <returns>The added component.</returns>
        public static Polygon AddComponentTo(GameObject gameObject, Logic.Polygon data)
        {
            var polygon = gameObject.AddComponent<Polygon>();
            polygon.SetComponent(data);
            return polygon;
        }

        public void SetComponent(Logic.Polygon data) =>
            this.Data = data;
    }
}