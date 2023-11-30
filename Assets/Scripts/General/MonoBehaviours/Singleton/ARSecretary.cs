// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.XR.ARFoundation;

namespace AwARe.MonoBehaviours
{
    /// <summary>
    /// Secretary class providing safe access to AR Objects, Components and Managers.
    /// </summary>
    public class ARSecretary : MonoBehaviour
    {
        // Singleton instance
        private static ARSecretary instance;

        // AR Support members
        [FormerlySerializedAs("ARSession")][SerializeField] private ARSession session;
        [FormerlySerializedAs("XROrigin")][SerializeField] private XROrigin origin;
        [FormerlySerializedAs("Camera")][SerializeField] private Camera cam;
        
        /// <summary>
        /// Gets the current AR Session.
        /// </summary>
        /// <value>The current AR Session.</value>
        public ARSession Session
        {
            get => Safe.Load(ref session, FindObjectOfType<ARSession>);
            private set => session = value;
        }
        
        /// <summary>
        /// Gets the current AR Session Origin.
        /// </summary>
        /// <value>The current AR Session Origin.</value>
        public XROrigin Origin
        {
            get => Safe.Load(ref origin, FindObjectOfType<XROrigin>);
            private set => origin = value;
        }

        /// <summary>
        /// Gets the current AR Camera.
        /// </summary>
        /// <value>The current AR Camera.</value>
        public Camera Camera
        {
            get => Safe.Load(ref cam, FindObjectOfType<Camera>);
            private set => cam = value;
        }

        /// <summary>
        /// Get the component of type T from Origin, Session, Camera or itself, if present.
        /// </summary>
        /// <typeparam name="T">Type of the component.</typeparam>
        /// <returns>The component, if present.</returns>
        public new T GetComponent<T>()
            where T : Component =>
            (Origin ? Origin.GetComponent<T>() : null)
            ?? (Session ? Session.GetComponent<T>() : null)
            ?? (Camera ? Camera.GetComponent<T>() : null)
            ?? gameObject.GetComponent<T>();

        private void Awake()
        {
            // Singleton behaviour
            Singleton.Awake(ref instance, this);
            // Do not unload its scene
            SceneSwitcher sceneSwitcher = SceneSwitcher.Get();
            sceneSwitcher.Keepers.Add(gameObject.scene);
            // Keep alive between scenes
            DontDestroyOnLoad(this.gameObject);
        }

        private void OnDestroy() =>
            Singleton.OnDestroy(ref instance, this);

        /// <summary>
        /// Get its current instance.
        /// Instantiate a new instance if necessary.
        /// </summary>
        /// <returns>An instance of itself.</returns>
        public static ARSecretary Get() =>
            Singleton.Get(ref instance, Instantiate);

        /// <summary>
        /// Instantiate a new instance of itself.
        /// </summary>
        /// <returns>An instance of itself.</returns>
        public static ARSecretary Instantiate() =>
            new GameObject("ARSecretary").AddComponent<ARSecretary>();
    }
}