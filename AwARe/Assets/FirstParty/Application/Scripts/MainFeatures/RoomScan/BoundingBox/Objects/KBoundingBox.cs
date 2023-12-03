using System.Collections.Generic;

using UnityEngine;

namespace AwARe.RoomScan.BoundingBox.Objects
{
    public class KBoundingBox : MonoBehaviour
    {
        public GameObject boundingBoxPrefab; // Assign bounding box prefab 
        private List<Vector3> touchLocations = new List<Vector3>();
        List<GameObject> touchMarks = new List<GameObject>();
        private GameObject currentBoundingBox;
        private GameObject touchMark;
        public GameObject markerprefab;

        void Update()
        {
            // Check for touch input
            if (UnityEngine.Input.touchCount > 0)
            {
                Touch touch = UnityEngine.Input.GetTouch(0); // Get the first touch 

                if (touch.phase == TouchPhase.Began)
                {
                    Vector3 touchPosition = Camera.main.ScreenToWorldPoint(touch.position);
                    touchLocations.Add(touchPosition);
                    touchMark = Instantiate(markerprefab,touchPosition, Quaternion.identity);
                    touchMarks.Add(touchMark);
                    touchMark.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);

                    // When we have four points, calculate and display the bounding box
                    if (touchLocations.Count == 4)
                    {
                        CreateBoundingBox();
                    
                    }
                    // if you touch the screen after the box has been created the markers and the box dissapear
                    // this is so that you have a clean slate to build a new box.
                    if (touchLocations.Count > 4)
                    {
                        Destroy(currentBoundingBox);
                        foreach (GameObject touchMark in touchMarks)
                        {
                            Destroy(touchMark);
                        }
                        touchLocations.Clear();
                    }

                }
            }
        }

        private void CreateBoundingBox()
        {
            if (touchLocations.Count < 4)
                return;

            Vector3 point1 = touchLocations[0];
            Vector3 point2 = touchLocations[1];
            Vector3 point3 = touchLocations[2];
            Vector3 point4 = touchLocations[3];

            // Calculate the center of the bounding box
            Vector3 middlePosition = (point1 + point2 + point3 + point4) / 4;

            // Calculate the width, depth, and height vectors
            Vector3 widthVector = point2 - point1;
            Vector3 depthVector = point3 - point2;
            Vector3 heightVector = point4 - point3;

            // Calculate the sizes of the box sides
            float width = widthVector.magnitude;
            float depth = depthVector.magnitude;
            float height = heightVector.magnitude;


            // Calculate the orientation of the bounding box
            Quaternion orientation = Quaternion.LookRotation(widthVector, Vector3.up);

            // Create a new bounding box
            if (boundingBoxPrefab != null)
            {
                if (currentBoundingBox != null)
                {
                    Destroy(currentBoundingBox);
                }

                // Position the bounding box at the center position
                currentBoundingBox = Instantiate(boundingBoxPrefab, middlePosition, orientation);


                // Set the scale of the bounding box based on width, depth, and height
                currentBoundingBox.transform.localScale = new Vector3(width, height, depth);
            }
 
        
        }

    }
}
