using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KBoundingBox : MonoBehaviour
{
    public GameObject boundingBoxPrefab; // Assign bounding box prefab 
    private List<Vector3> touchLocations = new List<Vector3>();
    private GameObject currentBoundingBox;

    void Update()
    {
        // Check for touch input
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0); // Get the first touch 

            if (touch.phase == TouchPhase.Began)
            {
                Vector3 touchPosition = Camera.main.ScreenToWorldPoint(touch.position);
                touchLocations.Add(touchPosition);

                // When we have three points, calculate and display the bounding box
                if (touchLocations.Count == 4)
                {
                    CreateBoundingBox();
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

        // Calculate width, depth, and height
        float width = Vector3.Distance(point1, point2);
        float depth = Vector3.Distance(point2, point3);
        float height = Vector3.Distance(point3, point4);

        // Calculate the middle position for the bounding box
        Vector3 middlePosition = (point1 + point2 + point3 + point4) / 4;

        // Create a new bounding box
        if (boundingBoxPrefab != null)
        {
            if (currentBoundingBox != null)
            {
                Destroy(currentBoundingBox);
            }

            // Position the bounding box at the center position
            currentBoundingBox = Instantiate(boundingBoxPrefab, middlePosition, Quaternion.identity);

            // Set the scale of the bounding box based on width, depth, and height
            currentBoundingBox.transform.localScale = new Vector3(width, height, depth);
        }

        // Clear the list for the next bounding box
        touchLocations.Clear();
    }


}
