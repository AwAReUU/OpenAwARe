// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

using AYellowpaper;
using UnityEngine;
using UnityEngine.Scripting;

namespace AwARe.Data.Objects
{
    public class Liner : MonoBehaviour
    {
        public LineRenderer lineRenderer;

        public InterfaceReference<ILiner> logic;

        public ILiner Logic
        {
            get => logic.Value;
            set => logic = new(value);
        }

        private bool newLine = false;

        public static void AddComponentTo(GameObject gameObject, LineRenderer lineRenderer, ILiner logic)
        {
            var liner = gameObject.AddComponent<Liner>();
            liner.lineRenderer = lineRenderer;
            liner.Logic = logic;
        }

        public void Update()
        {
            if (newLine && logic != null)
                CreateLine();
        }

        /// <summary>
        /// Update the line next Update-frame to represent the current data.
        /// </summary>
        public void UpdateLine() { newLine = true; Debug.Log("UpdateLine()");}

        /// <summary>
        /// Create a new mesh for the GameObject.
        /// </summary>
        private void CreateLine()
        {
            Debug.Log("CreateLine()");
            Vector3[] line = Logic.Line;
            lineRenderer.positionCount = line.Length;
            lineRenderer.SetPositions(line);
            newLine = false;
        }
    }
}