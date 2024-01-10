// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using AwARe.RoomScan.Polygons.Logic;

namespace AwARe.RoomScan
{
    /// <summary>
    /// Handles all behaviour regarding screenshots.
    /// </summary>
    public class ScreenshotManager : MonoBehaviour
    {
        [SerializeField] private Canvas canvas;
        [SerializeField] private Sprite maskSprite;
        [SerializeField] private Sprite outlineSprite;

        private Image image;
        private GameObject maskObj;

        void Start()
        {
            SetupImage();

            maskObj.SetActive(false);
        }

        void Update()
        {
            if ((UnityEngine.Input.touchCount > 0 && UnityEngine.Input.GetTouch(0).phase == TouchPhase.Began)
                || UnityEngine.Input.GetMouseButtonDown(0))
            {
                //StartCoroutine(TakeScreenshot(0));
            }
        }

        /// <summary>
        /// Returns a file path based on the given room and index of the anchorpoint.
        /// </summary>
        /// <param name="room">The room that the screenshot corresponds with.</param>
        /// <param name="index">The number of the anchorpoint that the screenshot corresponds with.</param>
        /// <returns>A string representing the file path.</returns>
        private string GetPath(Room room, int index) => $"{Application.persistentDataPath}{index}.png"; // TODO: add room name

        /// <summary>
        /// Takes a screenshot for the given room with the anchorpoint's index
        /// at the end of the frame and saves it locally.
        /// </summary>
        /// <param name="room">The room that the screenshot corresponds with.</param>
        /// <param name="index">The number of the anchorpoint that the screenshot corresponds with.</param>
        private IEnumerator TakeScreenshot(Room room, int index)
        {
            yield return new WaitForEndOfFrame();

            Texture2D screenshot = ScreenCapture.CaptureScreenshotAsTexture();
            System.IO.File.WriteAllBytes(GetPath(room, index), screenshot.EncodeToPNG());
        }

        /// <summary>
        /// Displays the specified screenshot on the screen.
        /// </summary>
        /// <param name="room">The room that the screenshot corresponds with.</param>
        /// <param name="index">The number of the anchorpoint that the screenshot corresponds with.</param>
        /// <param name="transparent">Whether the image displaying the screenshot should be transparent.</param>
        private void DisplayScreenshot(Room room, int index, bool transparent = false)
        {
            // set the images color either to transparent or solid
            Color color = image.color;
            if (transparent)    color.a = 0.4f;
            else                color.a = 1;
            image.color = color;

            maskObj.SetActive(true);
            image.sprite = RetrieveScreenshot(GetPath(room, index));
        }

        /// <summary>
        /// Hides the screenshot being displayed on the screen.
        /// </summary>
        private void HideScreenshot()
        {
            image.sprite = null;
            maskObj.SetActive(false);
        }

        /// <summary>
        /// Deletes the specified screenshot.
        /// </summary>
        /// <param name="room">The room that the screenshot corresponds with.</param>
        /// <param name="index">The number of the anchorpoint that the screenshot corresponds with.</param>
        private void DeleteScreenshot(Room room, int index)
        {
            File.Delete(GetPath(room, index));
        }

        /// <summary>
        /// Retrieves the screenshot from local memory at the given file path.
        /// </summary>
        /// <param name="filePath">The path where the screenshot is located.</param>
        /// <returns>The screenshot.</returns>
        private Sprite RetrieveScreenshot(string filePath)
        {
            if (System.IO.File.Exists(filePath))
            {
                byte[] bytes = System.IO.File.ReadAllBytes(filePath);
                Texture2D texture = new(1, 1);
                texture.LoadImage(bytes);
                return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            }
            throw new Exception("Screenshot cannot be found.");
        }

        private void SetupImage()
        {
            // create a mask object
            maskObj = new GameObject("ScreenshotMask");
            maskObj.transform.parent = canvas.transform;
            Image maskImage = maskObj.AddComponent<Image>();
            Mask mask = maskObj.AddComponent<Mask>();
            maskImage.sprite = maskSprite;
            mask.showMaskGraphic = false;

            // create an image object
            GameObject imageObj = new("ScreenshotImage");
            imageObj.transform.parent = maskObj.transform;
            image = imageObj.AddComponent<Image>();

            // set image size to match screensize
            float maskSize = Screen.width / 1.5f;
            maskImage.rectTransform.sizeDelta = new Vector2(maskSize, maskSize);
            maskImage.rectTransform.localPosition = new Vector3(0, 0, 0);
            maskImage.raycastTarget = false;

            image.rectTransform.sizeDelta = new Vector2(Screen.width, Screen.height);

            GameObject outlineImageObj = new("OutlineImage");
            outlineImageObj.transform.parent = canvas.transform;
            Image outlineImage = outlineImageObj.AddComponent<Image>();
            outlineImage.sprite = outlineSprite;
            outlineImage.rectTransform.sizeDelta = new Vector2(maskSize, maskSize);
            outlineImage.rectTransform.localPosition = new Vector3(0, 0, 0);
        }

        /// <summary>
        /// Checks whether the specified screenshot has been saved.
        /// </summary>
        /// <param name="room">The room that the screenshot corresponds with.</param>
        /// <param name="index">The number of the anchorpoint that the screenshot corresponds with.</param>
        /// <returns>Whether the specified screenshot has been saved.</returns>
        bool ScreenshotSaved(Room room, int index)
        {
            return System.IO.File.Exists(GetPath(room, index));
        }
    }
}
