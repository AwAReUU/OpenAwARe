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
    /// Undertakes and manages dynamic lines. <br/>
    /// Sets the line renderer to the line provided by the logic member.
    /// </summary>
    public class Liner : MonoBehaviour
    {
        /// <summary>
        /// The line renderer to control.
        /// </summary>
        public LineRenderer lineRenderer;

        /// <summary>
        /// The line constructor, providing the sequence of points to render the line by.
        /// </summary>
        public InterfaceReference<ILinerLogic> logic;

        /// <summary>
        /// Gets the line constructor.
        /// </summary>
        /// <value>
        /// The line constructor.
        /// </value>
        private ILinerLogic Logic =>
            logic.Value;

        // Tracking variables.
        private bool newLine = false;

        /// <summary>
        /// Adds this component to a given GameObject and initializes the components members.
        /// </summary>
        /// <param name="gameObject">The GameObject this component is added to.</param>
        /// <param name="lineRenderer">A line renderer.</param>
        /// <param name="logic">A line constructor.</param>
        /// <returns>The added component.</returns>
        public static Liner AddComponentTo(GameObject gameObject, LineRenderer lineRenderer, InterfaceReference<ILinerLogic> logic)
        {
            var liner = gameObject.AddComponent<Liner>();
            liner.lineRenderer = lineRenderer;
            liner.logic = logic;
            return liner;
        }

        protected void Update()
        {
            if (newLine && logic != null)
                CreateLine();
        }

        /// <inheritdoc/>
        public void UpdateLine() =>
            newLine = true;

        /// <summary>
        /// Create and set a new line.
        /// </summary>
        private void CreateLine()
        {
            Vector3[] line = Logic.Line;
            lineRenderer.positionCount = line.Length;
            lineRenderer.SetPositions(line);
            newLine = false;
        }
    }
}