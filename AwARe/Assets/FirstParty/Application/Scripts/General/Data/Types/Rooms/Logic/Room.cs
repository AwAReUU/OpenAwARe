// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AwARe.Data.Logic
{
    /// <summary>
    /// A class representing the room.
    /// </summary>
    public class Room : IEquatable<Room>
    {
        /// <summary>
        /// Gets or sets the main polygon.
        /// </summary>
        public string RoomName { get; set; }

        /// <value>
        /// The main polygon.
        /// </value>
        public Polygon PositivePolygon { get; set; }

        /// <summary>
        /// Gets or sets the subtracted polygons.
        /// </summary>
        /// <value>
        /// The subtracted polygons.
        /// </value>
        public List<Polygon> NegativePolygons { get; set; }


        /// <summary>
        /// Gets or sets the height of the room.
        /// </summary>
        public float RoomHeight { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Room"/> class.
        /// Either fills it with the given polygons or creates new ones.
        /// </summary>
        /// <param name="posPolygon">The positive Polygon of the room.</param>
        /// <param name="negPolygons">The negative Polygons of the room.</param>
        /// <param name="roomName">The name of the room.</param>
        /// <param name="roomHeight">The height of the room.</param>
        public Room(Polygon posPolygon = null, List<Polygon> negPolygons = null, string roomName = null, float roomHeight = 0)
        {
            RoomName = roomName;
            RoomHeight = roomHeight;
            PositivePolygon = posPolygon;
            NegativePolygons = negPolygons ?? new List<Polygon>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Room"/> class.
        /// Creates a deep copy of a <see cref="Room"/>.
        /// </summary>
        /// <param name="room">The <see cref="Room"/> to copy.</param>
        private Room(Room room)
        {
            RoomName = room.RoomName;
            RoomHeight = room.RoomHeight;
            PositivePolygon = room.PositivePolygon?.Clone();
            NegativePolygons = room.NegativePolygons.Select(x => x.Clone()).ToList();
        }

        /// <inheritdoc/>
        public bool Equals(Room other) =>
            (PositivePolygon == null && other.PositivePolygon == null || PositivePolygon.Equals(other.PositivePolygon)) &&
            NegativePolygons.SequenceEqual(other.NegativePolygons);

        /// <summary>
        /// Creates a deep copy of this <see cref="Room"/>.
        /// </summary>
        /// <returns>A Deep copy of this <see cref="Room"/>.</returns>
        public Room Clone() => new(this);
    }
}
