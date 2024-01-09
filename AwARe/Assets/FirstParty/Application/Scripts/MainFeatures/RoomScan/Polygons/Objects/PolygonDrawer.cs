// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

using AwARe.Data.Logic;
using AwARe.RoomScan.Polygons.Logic;

using UnityEngine;

namespace AwARe.RoomScan.Polygons.Objects
{
    public class PolygonDrawer : MonoBehaviour
    {
        [SerializeField] private GameObject lineObject; // the object that is instantiated to create the lines
        private LineRenderer tempLine; // the line from the last polygon point to the current pointer position
        private LineRenderer closeLine; // the line from the current pointer position to the first polygon point
        private LineRenderer line; // the line representing the polygon
        private Vector3 pointer = Vector3.zero;

        /// <summary>
        /// Sets the polygon currently being scanned.
        /// </summary>
        public Polygon ScanningPolygon {private get; set; }

        void Start()
        {
            tempLine = Instantiate(lineObject, gameObject.transform).GetComponent<LineRenderer>();
            SetLineColor(tempLine, Color.green);
            closeLine = Instantiate(lineObject, gameObject.transform).GetComponent<LineRenderer>();
            SetLineColor(closeLine, Color.green);
            line = lineObject.GetComponent<LineRenderer>();
        }

        void Update()
        {
            if (ScanningPolygon != null)
            {
                UpdateCloseLine();
                UpdateTempLine();
            }
        }

        /// <summary>
        /// Resets all lines.
        /// </summary>
        public void Reset()
        {
            pointer = Vector3.zero;
            UpdateLine(line, new Polygon());
            tempLine.gameObject.SetActive(true);
            closeLine.gameObject.SetActive(true);
        }

        /// <summary>
        /// Sets the pointer to the current position.
        /// </summary>
        /// <param name="pointer">Where the AR camera is pointing to.</param>
        public void SetPointer(Vector3 pointer)
        {
            this.pointer = pointer;
        }

        /// <summary>
        /// Draws the added point of the polygon.
        /// </summary>
        public void AddPoint()
        {
            ScanningPolygon.AddPoint(pointer);

            UpdateLine(line, ScanningPolygon);
        }

        /// <summary>
        /// Makes the objects of the scanning lines inactive.
        /// </summary>
        public void ClearScanningLines()
        {
            tempLine.gameObject.SetActive(false);
            closeLine.gameObject.SetActive(false);
        }

        /// <summary>
        /// Updates the line to match the given polygon.
        /// </summary>
        /// <param name="line">The line that needs to be updated.</param>
        /// <param name="polygon">The polygon that the line should represent.</param>
        public void UpdateLine(LineRenderer line, Polygon polygon)
        {
            line.positionCount = polygon.AmountOfPoints();
            line.SetPositions(polygon.GetPoints());
        }

        /// <summary>
        /// Updates the line from the last polygon point to the current pointer position.
        /// </summary>
        private void UpdateTempLine()
        {
            if (ScanningPolygon.AmountOfPoints() > 0)
            {
                tempLine.positionCount = 2;
                Vector3[] points = { ScanningPolygon.GetPoints()[^1], this.pointer };
                tempLine.SetPositions(points);
            }
            else
            {
                tempLine.positionCount = 0;
            }
        }

        /// <summary>
        /// Updates the line from the current pointer position to the first polygon point.
        /// </summary>
        private void UpdateCloseLine()
        {
            if (ScanningPolygon.AmountOfPoints() > 1)
            {
                closeLine.positionCount = 2;
                Vector3[] points = { ScanningPolygon.GetFirstPoint(), pointer };
                closeLine.SetPositions(points);
            }
            else
            {
                closeLine.positionCount = 0;
            }
        }

        /// <summary>
        /// Draws the positive polygon and all negative polygons of a room.
        /// </summary>
        /// <param name="room">The room to draw.</param>
        public void DrawRoomPolygons(Room room)
        {
            DrawPolygon(room.PositivePolygon);
            foreach (Polygon p in room.NegativePolygons)
            {
                DrawPolygon(p, true);
            }
        }

        /// <summary>
        /// Instantiates a new line and uses it to draw the given polygon.
        /// </summary>
        /// <param name="newPolygon">The polygon to draw.</param>
        /// <param name="isNegPolygon">Whether the polygon is negative.</param>
        public void DrawPolygon(Polygon newPolygon, bool isNegPolygon = false)
        {
            GameObject newLineObject = Instantiate(lineObject, transform);
            LineRenderer newLine = newLineObject.GetComponent<LineRenderer>();
            if (isNegPolygon)
            {
                SetLineColor(newLine, Color.red);
            }
            newLine.loop = true;
            UpdateLine(newLine, newPolygon);
        }
        
        /// <summary>
        /// Sets the start and end color of a line.
        /// </summary>
        /// <param name="line">The lineRenderer of which the color needs changing.</param>
        /// <param name="color">The color to change to.</param>
        private void SetLineColor(LineRenderer line, Color color)
        {
            line.startColor = line.endColor = color;
        }
    }
}