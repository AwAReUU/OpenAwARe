// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

using AwARe.Data.Logic;
using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Serialization;

namespace AwARe.Data.Objects
{
    public class RoomLiner : MonoBehaviour
    {
        [SerializeField] public Room room;
        private Logic.Room Data => room.Data;

        [SerializeField] private Liner positivePolygonLiner;
        [SerializeField] private Liner negativePolygonLinerPrefab;
        private List<Liner> negativePolygonLiners;

        //[SerializeField] Liner pathLiner; TODO: Implement Path representation if in lines.

        private bool newLines = false;
        private bool newLiners = false;

        public static void AddComponentTo(Room room, Liner positivePolygonLiner, Liner negativePolygonLinerPrefab, ILiner logic)
        {
            var liner = room.gameObject.AddComponent<RoomLiner>();
            liner.room = room;
            liner.positivePolygonLiner = positivePolygonLiner;
            liner.negativePolygonLinerPrefab = negativePolygonLinerPrefab;
        }

        private void Start()
        {
            ResetLiners();
        }

        public void Update()
        {
            if (newLiners)
                CreateLiners();
            if (newLines)
                CreateLines();
        }

        /// <summary>
        /// Update the line next Update-frame to represent the current data.
        /// </summary>
        public void UpdateLines() =>
            newLines = true;

        /// <summary>
        /// Create a new mesh for the GameObject.
        /// </summary>
        private void CreateLines()
        {
            if(positivePolygonLiner != null)
                positivePolygonLiner.UpdateLine();
            
            foreach (var liner in negativePolygonLiners)
                liner.UpdateLine();
        }

        public void UpdateLiners() =>
            newLiners = true;

        public void CreateLiners()
        {
            if(positivePolygonLiner != null)
                positivePolygonLiner.Logic = new PolygonLinerLogic(room.PositivePolygon);

            if (negativePolygonLinerPrefab != null)
            {
                List<Polygon> negativePolygons = room.NegativePolygons;
                int i = 0, liner_count = negativePolygonLiners.Count, polygon_count = negativePolygons.Count;
                for (; i < liner_count && i < polygon_count; i++)
                    negativePolygonLiners[i].Logic = new PolygonLinerLogic(negativePolygons[i]);
                for (; i < liner_count; i++)
                    negativePolygonLiners[i].Logic = null;
                for (; i < polygon_count; i++)
                {
                    var liner = Instantiate(negativePolygonLinerPrefab, transform);
                    liner.Logic = new PolygonLinerLogic(negativePolygons[i]);
                    negativePolygonLiners.Add(liner);
                }
            }

            newLiners = false;
        }
        
        public void ResetLiners()
        {
            if(positivePolygonLiner != null)
                positivePolygonLiner.logic = null;

            foreach (var liner in negativePolygonLiners)
                Destroy(liner.gameObject);
            negativePolygonLiners = new();

            UpdateLiners();
        }
    }
}