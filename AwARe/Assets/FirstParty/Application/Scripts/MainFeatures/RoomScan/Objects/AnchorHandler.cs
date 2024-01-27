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
    public class AnchorHandler : MonoBehaviour
    {
        /// <summary>
        /// A gameobject representing an anchor.
        /// </summary>
        [SerializeField] private GameObject anchorVisual;
        
        /// <summary>
        /// The pointer object.
        /// </summary>
        [SerializeField] private Pointer pointer;

        /// <summary>
        /// The session anchors used for saving/loading rooms.
        /// </summary>
        public List<Vector3> SessionAnchors { get; private set; }= new();

        /// <summary>
        /// The number of anchors that need to be placed.
        /// </summary>
        public int AnchorCount { get; } = 2;

        /// <summary>
        /// Add an anchor to the sessionAnchors list, fails if list is full.
        /// </summary>
        public void TryAddAnchor()
        {
            Vector3 anchorPoint = pointer.PointedAt;

            if (SessionAnchors.Count >= AnchorCount) return;

            SessionAnchors.Add(anchorPoint);
            if (anchorVisual != null)
            {
                GameObject anchorVisualObject;
                anchorVisualObject = Instantiate(anchorVisual, anchorPoint, Quaternion.identity) as GameObject;
                anchorVisualObject.transform.parent = transform;
                anchorVisualObject.name = "Anchor_" + SessionAnchors.Count.ToString();
            }
        }

        /// <summary>
        /// Remove the last anchor from the sessionAnchors.
        /// </summary>
        public void TryRemoveLastAnchor()
        {
            if (SessionAnchors.Count == 0) return;

            GameObject anchorVisualObject = GameObject.Find("Anchor_" + SessionAnchors.Count.ToString());
            if (anchorVisualObject != null)
            {
                Destroy(anchorVisualObject);
            }

            SessionAnchors.RemoveAt(SessionAnchors.Count - 1);
        }
    }
}
