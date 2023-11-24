using AwARe.DataStructures;
using IngredientLists;
using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Unity.VisualScripting;
using Unity.XR.CoreUtils;

using UnityEngine;
using UnityEngine.Serialization;

using Object = UnityEngine.Object;

namespace AwARe.MonoBehaviours
{
    /// <summary>
    /// Example class
    /// </summary>
    public abstract class ARSecretary : MonoBehaviour
    {
        private static ARSecretary instance;

        [FormerlySerializedAs("ARSession")][SerializeField] private ARSession session;
        [FormerlySerializedAs("XROrigin")][SerializeField] private XROrigin origin;
        [FormerlySerializedAs("Camera")][SerializeField] private Camera cam;

        public ARSession Session
        {
            get => Safe.Load(ref session, FindObjectOfType<ARSession>);
            private set => session = value;
        }

        public XROrigin Origin
        {
            get => Safe.Load(ref origin, FindObjectOfType<XROrigin>);
            private set => origin = value;
        }

        public Camera Camera
        {
            get => Safe.Load(ref cam, FindObjectOfType<Camera>);
            private set => cam = value;
        }

        public new T GetComponent<T>()
            where T : Component =>
            (Origin ? Origin.GetComponent<T>() : null)                                  // a.?F() <=> a ? a.F() : null
            ?? (Origin ? Origin.GetComponent<T>() : null)
            ?? (Origin ? Origin.GetComponent<T>() : null);

        private void Awake()
        {
            Singleton.Awake(ref instance, this);
            DontDestroyOnLoad(this.gameObject);
        }

        protected virtual void OnDestroy() =>
            Singleton.OnDestroy(ref instance, this);

        public static ARSecretary Get() =>
            Singleton.Get(ref instance, Instantiate);

        public static ARSecretary Instantiate() =>
            new GameObject("ARSecretary").AddComponent<ARSecretary>();
    }
}