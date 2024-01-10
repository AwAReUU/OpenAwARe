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
using System.IO;
using System.Collections;

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

        bool displayed = false;
        bool setup = true;

        void Start()
        {
            SetupImage();

            maskObj.SetActive(false);

            File.Delete(GetPath(0));
        }
        void Update()
        {
            if (!setup)
            {
                setup = true;
                SetupImage();
            }
            if ((UnityEngine.Input.touchCount > 0 && UnityEngine.Input.GetTouch(0).phase == TouchPhase.Began)
                || UnityEngine.Input.GetMouseButtonDown(0))
            {
                if(displayed)
                {
                    displayed = false;
                    maskObj.SetActive(false);
                    File.Delete(GetPath(0));
                }
                else
                {
                    StartCoroutine(TakeScreenshot(0));
                    Debug.Log("screenshot");
                }
            }
            else
            {
                if (System.IO.File.Exists(GetPath(0)) && !displayed)
                {
                    Debug.Log("display");
                    DisplayScreenshot(0);
                    displayed = true;
                }
            }
        }

        private string GetPath(/*Room room, */int index) => $"{Application.persistentDataPath}{index}.png"; // TODO: add room name

        private IEnumerator TakeScreenshot(/*Room room, */int index)
        {
            yield return new WaitForEndOfFrame();


            Texture2D screenshot = ScreenCapture.CaptureScreenshotAsTexture();
            System.IO.File.WriteAllBytes(GetPath(/*room, */0), screenshot.EncodeToPNG());

            // TODO: display text: screenshot taken
        }

        private void DisplayScreenshot(/*Room room, */int index)
        {
            //image.gameObject.SetActive(true);
            maskObj.SetActive(true);
            image.sprite = RetrieveScreenshot(GetPath(/*room, */index));
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

        private void ClearImage()
        {
            image.sprite = null;
            //image.gameObject.SetActive(false);
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
            GameObject imageObj = new GameObject("ScreenshotImage");
            imageObj.transform.parent = maskObj.transform;
            image = imageObj.AddComponent<Image>();

            // set image size to match screensize
            //maskImage.rectTransform.pivot = new Vector2(0, 0);
            float maskSize = Screen.width / 1.5f;
            maskImage.rectTransform.sizeDelta = new Vector2(maskSize, maskSize);
            maskImage.rectTransform.localPosition = new Vector3(0, 0, 0);
            maskImage.raycastTarget = false;

            image.rectTransform.sizeDelta = new Vector2(Screen.width, Screen.height);

            // make the image transparent
            Color color = image.color;
            color.a = 0.5f;
            image.color = color;

            GameObject outlineImageObj = new GameObject("OutlineImage");
            outlineImageObj.transform.parent = canvas.transform;
            Image outlineImage = outlineImageObj.AddComponent<Image>();
            outlineImage.sprite = outlineSprite;
            outlineImage.rectTransform.sizeDelta = new Vector2(maskSize, maskSize);
            outlineImage.rectTransform.localPosition = new Vector3(0, 0, 0);

            ClearImage();
        }
    }
}
