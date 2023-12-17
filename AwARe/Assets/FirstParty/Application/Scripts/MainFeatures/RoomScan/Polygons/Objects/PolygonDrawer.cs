// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

using AwARe.Data.Objects;
using AwARe.RoomScan.Polygons.Logic;

using AYellowpaper;

using UnityEngine;
using UnityEngine.Serialization;

namespace AwARe.RoomScan.Polygons.Objects
{
    public class PolygonDrawer : MonoBehaviour
    {
        [SerializeField] private PolygonManager manager;

        [SerializeField] private GameObject polygonBase; // the object that is instantiated to create the lines
        [SerializeField] private LineRenderer activeLine; // the polygonLine from the last Polygon point to the current pointer position
        [SerializeField] private LineRenderer closeLine; // the polygonLine from the current pointer position to the first Polygon point
        
        private Polygon activePolygon;
        private Liner polygonLine; // the polygonLine representing the Polygon
        private PolygonLinerLogic polygonLineLogic;

        private Vector3 PointedAt => manager.PointedAt;
        private Data.Logic.Polygon Polygon => activePolygon.Data;

        private void Update()
        {
            if(activePolygon != null)
                UpdateLines();
        }

        /// <summary>
        /// Starts the drawing process.
        /// </summary>
        public void StartDrawing()
        {
            GameObject obj = Instantiate(polygonBase, transform);
            activePolygon = obj.GetComponent<Polygon>();
            polygonLine = obj.GetComponent<Liner>();
            polygonLineLogic = obj.GetComponent<PolygonLinerLogic>();
            
            obj.SetActive(true);
            activePolygon.Data = new();
            polygonLineLogic.closedLine = false;

            activeLine.gameObject.SetActive(true);
            closeLine.gameObject.SetActive(true);
            UpdateLines();
        }

        
        /// <summary>
        /// Draws the added point of the Polygon.
        /// </summary>
        public void AddPoint()
        {
            if (activePolygon == null)
                return;

            Polygon.Points.Add(PointedAt);
            Debug.Log($"Point Added: {PointedAt}");
            polygonLine.UpdateLine();
        }
        
        /// <summary>
        /// Ends the drawing process.
        /// </summary>
        public void FinishDrawing(out Data.Logic.Polygon result)
        {
            activeLine.gameObject.SetActive(false);
            closeLine.gameObject.SetActive(false);
            result = activePolygon.Data;
            Destroy(activePolygon.gameObject);
            activePolygon = null;
        }

        private void UpdateLines()
        {
            int count = Polygon.Points.Count;

            if (count == 0)
            {
                // Don't draw any line
                activeLine.positionCount = closeLine.positionCount = 0;
                return;
            }

            // Draw active line
            activeLine.positionCount = 2;
            activeLine.SetPositions(new[]{ Polygon.Points[^1], PointedAt });

            if (count <= 1)
            {
                // Don't draw closing line
                closeLine.positionCount = 0;
                return;
            }

            // Draw closing line
            closeLine.positionCount = 2;
            closeLine.SetPositions(new[]{ PointedAt, Polygon.Points[0] });
        }
    }
}