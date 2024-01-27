// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

using System;
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
        private Vector3 defaultScale;

        /// <summary>
        /// Gets a value indicating whether a screenshot is currently being displayed.
        /// </summary>
        public bool ScreenshotDisplayed {get; private set;} = false;

        void Start()
        {
            SetupImage();

            maskObj.SetActive(false);
            outlineImageObj.SetActive(false);

            string basePath = $"{Application.persistentDataPath}/screenshots";
            if(!Directory.Exists(basePath))
                Directory.CreateDirectory(basePath);
        }

        /// <summary>
        /// Returns a file path based on the given room and index of the anchorpoint.
        /// </summary>
        /// <param name="roomName">The name of the room that the screenshot corresponds with.</param>
        /// <param name="index">The number of the anchorpoint that the screenshot corresponds with.</param>
        /// <returns>A string representing the file path.</returns>
        private string GetPath(string roomName, int index) => $"{Application.persistentDataPath}/screenshots/{roomName}{index}.png";

        /// <summary>
        /// Takes a screenshot and returns it as a Texture2D.
        /// </summary>
        /// <returns>The screenshot.</returns>
        public Texture2D TakeScreenshot() =>
            ScreenCapture.CaptureScreenshotAsTexture();

        /// <summary>
        /// Save the given screenshot locally.
        /// </summary>
        /// <param name="screenshot">The screenshot to save.</param>
        /// <param name="room">The room that the screenshot corresponds with.</param>
        /// <param name="index">The number of the anchorpoint that the screenshot corresponds with.</param>
        public void SaveScreenshot(Texture2D screenshot, Data.Logic.Room room, int index) =>
            System.IO.File.WriteAllBytes(GetPath(room.RoomName, index), screenshot.EncodeToPNG());

        /// <summary>
        /// Displays the specified screenshot on the screen.
        /// </summary>
        /// <param name="screenshot">The screenshot to be displayed.</param>
        /// <param name="transparent">Whether the image displaying the screenshot should be transparent.</param>
        /// <param name="size">The relative size the screenshot should be shown in.</param>
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
                    SetImageScale(defaultScale);
                    break;
                case ImageSize.Small:
                    SetImagePos(defaultPos + new Vector3(0, 350, 0));
                    SetImageScale(defaultScale / 3);
                    break;
            }

            maskObj.SetActive(true);
            outlineImageObj.SetActive(true);
            image.sprite = screenshot;

            ScreenshotDisplayed = true;
        }

        /// <summary>
        /// Gets a screenshot from the file and displays it on screen.
        /// </summary>
        /// <param name="roomName">The name of the room corresponding with the screenshot.</param>
        /// <param name="index">The index corresponding with the screenshot.</param>
        /// <param name="transparent">Whether the image displaying the screenshot should be transparent.</param>
        /// <param name="size">The relative size the screenshot should be shown in.</param>
        public void DisplayScreenshotFromFile(
            string roomName,
            int index,
            bool transparent = false,
            ImageSize size = ImageSize.Large
            )
        {
            DisplayScreenshot(RetrieveScreenshot(GetPath(roomName, index)), transparent, size);
        }

        /// <summary>
        /// Hides the screenshot being displayed on the screen.
        /// </summary>
        public void HideScreenshot()
        {
            image.sprite = null;
            maskObj.SetActive(false);
            outlineImageObj.SetActive(false);

            ScreenshotDisplayed = false;
        }

        /// <summary>
        /// Converts a texture2D to a sprite.
        /// </summary>
        /// <param name="texture">The texture to convert.</param>
        /// <returns>The sprite.</returns>
        public Sprite TextureToSprite(Texture2D texture)
        {
            return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        }

        /// <summary>
        /// Deletes the specified screenshot.
        /// </summary>
        /// <param name="roomName">The name of the room that the screenshot corresponds with.</param>
        /// <param name="index">The number of the anchorpoint that the screenshot corresponds with.</param>
        public void DeleteScreenshot(string roomName, int index)
        {
            File.Delete(GetPath(roomName, index));
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

        /// <summary>
        /// Create and initialize the necessary mask and image objects needed for displaying a screenshot.
        /// </summary>
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

            Vector2 size = new Vector2(maskSize, maskSize);

            maskObj.GetComponent<Image>().rectTransform.sizeDelta = size;
            outlineImageObj.GetComponent<Image>().rectTransform.sizeDelta = size;
            
            defaultScale = maskObj.transform.localScale;

            SetImagePos(defaultPos);
            SetImageScale(defaultScale);

            maskObj.SetActive(false);
            outlineImageObj.SetActive(false);
        }

        /// <summary>
        /// Sets the position of the image to the given position.
        /// </summary>
        /// <param name="position">The position to set the object to.</param>
        private void SetImagePos(Vector3 position)
        {
            maskObj.GetComponent<Image>().rectTransform.localPosition = position;
            outlineImageObj.GetComponent<Image>().rectTransform.localPosition = position;
        }

        /// <summary>
        /// Sets the scale of the image to the given scale.
        /// </summary>
        /// <param name="scale">The scale to set the object to.</param>
        private void SetImageScale(Vector3 scale)
        {
            maskObj.transform.localScale = scale;
            outlineImageObj.transform.localScale = scale;
        }

        /// <summary>
        /// The size the screenshot image should be shown in.
        /// </summary>
        public enum ImageSize
        {
            Small,
            Large
        }
    }
}
