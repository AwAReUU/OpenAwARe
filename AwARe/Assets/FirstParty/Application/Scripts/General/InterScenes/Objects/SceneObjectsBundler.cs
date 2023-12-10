// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

using UnityEngine;

namespace AwARe.InterScenes.Objects
{
    /// <summary>
    /// A behaviour for an object that takes all root objects of a scene and bundles them together under itself. <br/>
    /// This calls is mostly used to simulate loading and unloading of scenes.
    /// </summary>
    public class SceneObjectsBundler : MonoBehaviour
    {
        /// <summary>
        /// Called when the script instance is being loaded.
        /// </summary>
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

        /// <summary>
        /// Bundle all root objects together and parent under this GameObject.
        /// </summary>
        public void Bundle()
        {
            var me = gameObject;
            var objects = me.scene.GetRootGameObjects();
            foreach (var obj in objects)
                if (obj != me)
                    obj.transform.SetParent(me.transform);
        }
    }
}
