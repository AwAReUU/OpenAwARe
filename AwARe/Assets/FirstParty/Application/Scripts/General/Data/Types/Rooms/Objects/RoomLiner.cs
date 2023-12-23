// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

using System.Collections.Generic;
using System.Linq;
using AwARe.Objects;
using UnityEngine;

namespace AwARe.Data.Objects
{
    /// <summary>
    /// Undertakes and manages dynamic lines of a room data type. <br/>
    /// Controls the liners of the various room components. <br/>
    /// The room components MUST always have Liner components.
    /// </summary>
    public class RoomLiner : MonoBehaviour
    {
        /// <summary>
        /// The room represented.
        /// </summary>
        public Room room;

        /// <summary>
        /// Indicates if the polygons should be drawn.
        /// </summary>
        public bool drawPolygons;
        //public bool drawPath;

        // Tracking data
        private bool newLines = false;
        private bool newLiners = false;
        private Liner positivePolygonLiner;
        private List<Liner> negativePolygonLiners;
        // private Liner pathLiner;
        
        /// <summary>
        /// Adds this component to a given GameObject and initializes the components members.
        /// </summary>
        /// <param name="gameObject">The GameObject this component is added to.</param>
        /// <param name="room">A room.</param>
        /// <returns>The added component.</returns>
        public static RoomLiner AddComponentTo(GameObject gameObject, Room room)
        {
            var liner = gameObject.AddComponent<RoomLiner>();
            liner.room = room;
            return liner;
        }

        private void Start()
        {
            ResetLiners();
        }

        private void Update()
        {
            if (newLiners)
                ResetLiners();
            if (newLines)
                CreateLines();
        }

        /// <summary>
        /// Update the lines next Update-frame to represent the current data.
        /// </summary>
        public void UpdateLines() =>
            newLines = true;
        
        /// <summary>
        /// Create and set new lines.
        /// </summary>
        private void CreateLines()
        {
            if(positivePolygonLiner != null)
                positivePolygonLiner.UpdateLine();
            
            foreach (var liner in negativePolygonLiners)
                liner.UpdateLine();
        }
        
        /// <summary>
        /// Update the liners next Update-frame, in case the room has changed.
        /// </summary>
        public void UpdateLiners() =>
            newLiners = true;
        
        /// <summary>
        /// Get the new liners.
        /// </summary>
        public void ResetLiners()
        {
            positivePolygonLiner = room.PositivePolygon.GetComponent<Liner>();
            negativePolygonLiners = room.NegativePolygons.Select(x => x.GetComponent<Liner>()).ToList();

            newLiners = false;
        }
    }
}