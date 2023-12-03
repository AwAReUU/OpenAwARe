using System;

using UnityEngine;

namespace AwARe.Objects
{
    public class Container : MonoBehaviour
    {
        protected GameObject childObject;
        public Func<Transform, GameObject> getChild;

        // Start is called before the first frame update
        void Start()
        {
            OnStart();
        }

        protected virtual void OnStart()
        {
            childObject = getChild(this.transform);
            SetLocalTransform();
        }

        protected virtual void SetLocalTransform() { }
    }
}