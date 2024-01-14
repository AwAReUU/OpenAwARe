using System.Collections;

using UnityEngine;

namespace AwARe.DevTools.ObjectGeneration
{
    /// <summary>
    /// Functionality to visualize BoxColliders.
    /// </summary>
    public class BoxColliderVisualizer
    {
        public BoxColliderVisualizer(BoxCollider boxCollider)
        {
            //TODO: DONT create a new monobehavior gameobject for each boxcollider spawned.
            GameObject gameObject = new GameObject();
            BoxColliderHelperMonoBehaviour boxColliderHelper
                = gameObject.AddComponent<BoxColliderHelperMonoBehaviour>();
            boxColliderHelper.CreateVisualBox(boxCollider);
        }
    }

    /// <summary>
    /// Holds functionality to create & destroy visualizations of boxcolliders.
    /// </summary>
    public class BoxColliderHelperMonoBehaviour : MonoBehaviour
    {
        /// <summary>
        /// Render a boxCollider that fades out after some time.
        /// </summary>
        /// <param name="boxCollider">BoxCollider to visualize.</param>
        public void CreateVisualBox(BoxCollider boxCollider)
        {
            // Create a new GameObject
            GameObject visualBox = new GameObject("Visual Box");

            // Set visual box properties
            MeshRenderer meshRenderer = visualBox.AddComponent<MeshRenderer>();
            MeshFilter meshFilter = visualBox.AddComponent<MeshFilter>();
            meshFilter.mesh = Resources.GetBuiltinResource<Mesh>("Cube.fbx");
            meshRenderer.material = new Material(Shader.Find("Standard"));
            meshRenderer.material.color = new Color(1.0f, 0.0f, 0.0f, 1.0f);

            // Set the size, position, and rotation
            visualBox.transform.position = boxCollider.transform.TransformPoint(boxCollider.center);
            visualBox.transform.rotation = boxCollider.transform.rotation;
            visualBox.transform.localScale = Vector3.Scale(boxCollider.transform.lossyScale, boxCollider.size);

            // Transparent material
            meshRenderer.material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
            meshRenderer.material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            meshRenderer.material.SetInt("_ZWrite", 0);
            meshRenderer.material.DisableKeyword("_ALPHATEST_ON");
            meshRenderer.material.DisableKeyword("_ALPHABLEND_ON");
            meshRenderer.material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
            meshRenderer.material.renderQueue = 3000;

            // Start fading out the visualBox
            StartCoroutine(FadeOutAndDestroy(visualBox, 3f));
        }

        /// <summary>
        /// Fade out the given <paramref name="target"/> GameObject during <paramref name="duration"/> seconds. 
        /// After this, destroy it.
        /// </summary>
        /// <param name="target">BoxCollider to fade out.</param>
        /// <param name="duration">Time that the animation will take.</param>
        /// <returns></returns>
        private IEnumerator FadeOutAndDestroy(GameObject target, float duration)
        {
            float counter = 0;
            MeshRenderer meshRenderer = target.GetComponent<MeshRenderer>();
            Color startColor = meshRenderer.material.color;

            while (counter < duration)
            {
                counter += Time.deltaTime;
                float alpha = Mathf.Lerp(1, 0, counter / duration);
                meshRenderer.material.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
                yield return null;
            }

            Destroy(target);
        }
    }
}