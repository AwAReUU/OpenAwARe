using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AwARe.MonoBehaviours
{
    public class SceneObjectsBundler : MonoBehaviour
    {
        private void Awake()
        {
            // Get myself, my scene and all object in my scene
            var me = gameObject;
            var scene = me.scene;

            // Rename myself to my scene
            me.name = scene.name;

            // Parent all other objects in scene to me
            Bundle();

            // Move me and everything else to active scene if not in it
            SceneSwitcher.Get();

            // Destroy this behaviour, stop repeating it
            Destroy(this);
        }

        public void Bundle()
        {
            var me = gameObject;
            var objects = me.scene.GetRootGameObjects();
            foreach (var obj in objects)
                if (obj != me)
                    obj.transform.parent = me.transform;
        }
    }
}
