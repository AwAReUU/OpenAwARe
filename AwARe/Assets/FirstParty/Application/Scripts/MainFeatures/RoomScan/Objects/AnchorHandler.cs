// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

using System.Collections.Generic;
using AwARe.UI.Objects;
using UnityEngine;

namespace AwARe.RoomScan.Objects
{
    /// <summary>
    /// Handles placing the anchors.
    /// </summary>
    public class AnchorHandler : MonoBehaviour
    {
        /// <summary>
        /// A game object representing an anchor.
        /// </summary>
        [SerializeField] private GameObject anchorVisual;
        
        /// <summary>
        /// The pointer object.
        /// </summary>
        [SerializeField] private Pointer pointer;

        /// <summary>
        /// Gets the session anchors used for saving/loading rooms.
        /// </summary>
        /// <value>
        /// The current session anchors.
        /// </value>
        public List<Vector3> SessionAnchors { get; private set; }= new();

        /// <summary>
        /// The number of anchors that need to be placed.
        /// </summary>
        public readonly int anchorCount = 2;

        /// <summary>
        /// Add an anchor to the sessionAnchors list, fails if list is full.
        /// </summary>
        public void TryAddAnchor()
        {
            Vector3? pointedAt = pointer.PointedAt;

            if (!pointedAt.HasValue || AnchoringFinished()) return;

            Vector3 anchorPoint = pointedAt.Value;

            SessionAnchors.Add(anchorPoint);
            if (anchorVisual == null) return;

            GameObject anchorVisualObject;
            anchorVisualObject = Instantiate(anchorVisual, anchorPoint, Quaternion.identity) as GameObject;
            anchorVisualObject.transform.SetParent(transform, true);
            anchorVisualObject.name = "Anchor_" + SessionAnchors.Count;
        }

        /// <summary>
        /// Remove the last anchor from the sessionAnchors.
        /// </summary>
        public void TryRemoveLastAnchor()
        {
            if (SessionAnchors.Count == 0) return;

            GameObject anchorVisualObject = GameObject.Find("Anchor_" + SessionAnchors.Count);
            if (anchorVisualObject != null)
                Destroy(anchorVisualObject);

            SessionAnchors.RemoveAt(SessionAnchors.Count - 1);
        }

        /// <summary>
        /// Checks if there are enough anchors.
        /// </summary>
        /// <returns>Whether the number of anchors is equal to the needed number of anchors.</returns>
        public bool AnchoringFinished() =>
            SessionAnchors.Count >= anchorCount;
    }
}
