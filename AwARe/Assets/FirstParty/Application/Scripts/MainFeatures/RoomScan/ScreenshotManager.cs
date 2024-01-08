// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

using System;
using AwARe.RoomScan.Polygons.Logic;
using UnityEngine;
using UnityEngine.UI;

namespace AwARe.RoomScan
{
    /// <summary>
    /// Handles all behaviour regarding screenshots.
    /// </summary>
    public class ScreenshotManager : MonoBehaviour
    {
        Image image;

        private void Start()
        {
            // create an image
            image = gameObject.AddComponent<Image>();

            // make the image transparent
            Color color = image.color;
            color.a = 0.5f;
            image.color = color;
        }

        private string GetPath(Room room, int index) => "" + index; // TODO: add room name

        private void TakeScreenshot(Room room, int index)
        {
            ScreenCapture.CaptureScreenshot(GetPath(room, index)); 
        }

        private void DisplayScreenshot(Room room, int index)
        {
            image.sprite = RetrieveScreenshot(GetPath(room, index));
        }

        private Sprite RetrieveScreenshot(string filePath)
        {
            if (System.IO.File.Exists(filePath))
            {
                byte[] bytes = System.IO.File.ReadAllBytes(filePath);
                Texture2D texture = new Texture2D(1, 1);
                texture.LoadImage(bytes);
                return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            }
            throw new Exception("Screenshot cannot be found.");
        }

        private void ClearImage() => image.sprite = null;
    }
}
