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
        private GameObject outlineImageObj;

        private Vector3 defaultPos;
        private Vector2 defaultSize;

        public bool screenshotDisplayed {get; private set;} = false;

        void Start()
        {
            SetupImage();

            maskObj.SetActive(false);

            string basePath = $"{Application.persistentDataPath}/screenshots";
            if(!Directory.Exists(basePath))
            {
                Directory.CreateDirectory(basePath);
            }
        }

        /// <summary>
        /// Returns a file path based on the given room and index of the anchorpoint.
        /// </summary>
        /// <param name="room">The room that the screenshot corresponds with.</param>
        /// <param name="index">The number of the anchorpoint that the screenshot corresponds with.</param>
        /// <returns>A string representing the file path.</returns>
        private string GetPath(Data.Logic.Room room, int index) => $"{Application.persistentDataPath}/screenshots/{room.RoomName}{index}.png";

        /// <summary>
        /// Takes a screenshot for the given room with the anchorpoint's index
        /// at the end of the frame.
        /// </summary>
        /// <param name="room">The room that the screenshot corresponds with.</param>
        /// <param name="index">The number of the anchorpoint that the screenshot corresponds with.</param>
        public /*IEnumerator*/ Texture2D TakeScreenshot(Data.Logic.Room room, int index)
        {
            //yield return new WaitForEndOfFrame();

            return ScreenCapture.CaptureScreenshotAsTexture();

            //System.IO.File.WriteAllBytes(GetPath(room, index), screenshot.EncodeToPNG());
        }

        
        /// <summary>
        /// Save the given screenshot locally.
        /// </summary>
        /// <param name="screenshot">The screenshot to save.</param>
        /// <param name="room">The room that the screenshot corresponds with.</param>
        /// <param name="index">The number of the anchorpoint that the screenshot corresponds with.</param>
        public void SaveScreenshot(Texture2D screenshot, Data.Logic.Room room, int index)
        {
            System.IO.File.WriteAllBytes(GetPath(room, index), screenshot.EncodeToPNG());
        }

        /// <summary>
        /// Displays the specified screenshot on the screen.
        /// </summary>
        /// <param name="room">The room that the screenshot corresponds with.</param>
        /// <param name="index">The number of the anchorpoint that the screenshot corresponds with.</param>
        /// <param name="transparent">Whether the image displaying the screenshot should be transparent.</param>
        public void DisplayScreenshot(
            Sprite screenshot,
            bool transparent = false,
            ImageSize size = ImageSize.Large
            )
        {
            // set the images color either to transparent or solid
            Color color = image.color;
            if (transparent) color.a = 0.4f;
            else color.a = 1;
            image.color = color;

            switch (size)
            {
                case ImageSize.Large:
                    SetImagePos(defaultPos);
                    SetImageSize(defaultSize);
                    break;
                case ImageSize.Medium:
                    SetImagePos(defaultPos);
                    SetImageSize(defaultSize / 2);
                    break;
                case ImageSize.Small:
                    SetImagePos(defaultPos - new Vector3(0, Screen.height / 1.5f, 0));
                    SetImageSize(defaultSize / 3);
                    break;
            }

            maskObj.SetActive(true);
            outlineImageObj.SetActive(true);
            image.sprite = screenshot;

            screenshotDisplayed = true;
        }

        public void DisplayScreenshotFromFile(
            Data.Logic.Room room,
            int index,
            bool transparent = false,
            ImageSize size = ImageSize.Large
            )
        {
            DisplayScreenshot(RetrieveScreenshot(GetPath(room, index)), transparent, size);
        }

        /// <summary>
        /// Hides the screenshot being displayed on the screen.
        /// </summary>
        public void HideScreenshot()
        {
            image.sprite = null;
            maskObj.SetActive(false);
            outlineImageObj.SetActive(false);

            screenshotDisplayed = false;
        }

        public Sprite TextureToSprite(Texture2D texture)
        {
            return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        }

        /// <summary>
        /// Deletes the specified screenshot.
        /// </summary>
        /// <param name="room">The room that the screenshot corresponds with.</param>
        /// <param name="index">The number of the anchorpoint that the screenshot corresponds with.</param>
        private void DeleteScreenshot(Data.Logic.Room room, int index)
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
                return TextureToSprite(texture);
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
            maskImage.raycastTarget = false;

            image.rectTransform.sizeDelta = new Vector2(Screen.width, Screen.height);

            // create object for outline of the image
            outlineImageObj = new("OutlineImage");
            outlineImageObj.transform.parent = canvas.transform;
            Image outlineImage = outlineImageObj.AddComponent<Image>();
            outlineImage.sprite = outlineSprite;

            defaultPos = new Vector3(0, 0, 0);
            defaultSize = new Vector2(maskSize, maskSize);

            SetImagePos(defaultPos);
            SetImageSize(defaultSize);

            maskObj.SetActive(false);
            outlineImageObj.SetActive(false);
        }

        private void SetImagePos(Vector3 position)
        {
            maskObj.GetComponent<Image>().rectTransform.localPosition = position;
            outlineImageObj.GetComponent<Image>().rectTransform.localPosition = position;
        }

        private void SetImageSize(Vector2 size)
        {
            maskObj.GetComponent<Image>().rectTransform.sizeDelta = size;
            outlineImageObj.GetComponent<Image>().rectTransform.sizeDelta = size;
        }

        public enum ImageSize
        {
            Small,
            Medium,
            Large
        }
    }
}
