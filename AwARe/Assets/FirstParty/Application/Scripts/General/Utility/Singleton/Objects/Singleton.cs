// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AwARe.Objects
{
    /*
    EXAMPLE OF A SINGLETON MONOBEHAVIOUR:

    public abstract class SingletonExample : MonoBehaviour
    {
        private static SingletonExample instance;

        private void Awake() =>
            Singleton.Awake(ref instance, this);

        protected virtual void OnDestroy() =>
            Singleton.OnDestroy(ref instance, this);

        public static SingletonExample Get() =>
            Singleton.Get(ref instance, Instantiate);

        public static SingletonExample Instantiate() =>
            new GameObject("SingletonExample").AddComponent<SingletonExample>();
    }
    */

    /// <summary>
    /// Utility class for Singleton behaviour.
    /// </summary>
    public static class Singleton
    {
        /// <summary>
        /// Get the current instance or create and assign a new instance if null.
        /// </summary>
        /// <typeparam name="T">A Unity Singleton type.</typeparam>
        /// <param name="instance">A reference to the instance member.</param>
        /// <param name="instantiate">An instantiation callback.</param>
        /// <returns>The current instance.</returns>
        public static T Get<T>(ref T instance, Func<T> instantiate)
            where T : MonoBehaviour =>
            instance ??= instantiate();

        /// <summary>
        /// Behaviour on <c>Awake</c>.
        /// Destroys self if an instance is already available.
        /// Assigns instance to self if no instance is recorded as of yet.
        /// </summary>
        /// <typeparam name="T">A Unity Singleton type.</typeparam>
        /// <param name="instance">A reference to the instance member.</param>
        /// <param name="me">Current MonoBehaviour handled.</param>
        public static void Awake<T>(ref T instance, T me)
            where T : MonoBehaviour
        {
            if (instance != null && instance != me)
                Object.Destroy(me.gameObject);
            else
                instance = me;
        }
        
        /// <summary>
        /// Behaviour on <c>Awake</c>.
        /// Sets current instance to null if the referred to instance is destroyed.
        /// </summary>
        /// <typeparam name="T">A Unity Singleton type.</typeparam>
        /// <param name="instance">A reference to the instance member.</param>
        /// <param name="me">Current MonoBehaviour handled.</param>
        public static void OnDestroy<T>(ref T instance, T me)
            where T : MonoBehaviour
        {
            if(instance == me)
                instance = null;
        }
    }
}