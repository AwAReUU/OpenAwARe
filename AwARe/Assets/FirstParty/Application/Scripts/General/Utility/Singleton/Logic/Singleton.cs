using System;

using UnityEngine;

using Object = UnityEngine.Object;

namespace AwARe.Logic
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

    public static class Singleton
    {
        public static T Get<T>(ref T instance, Func<T> instantiate)
            where T : MonoBehaviour =>
            instance ??= instantiate();

        public static void Awake<T>(ref T instance, T me)
            where T : MonoBehaviour
        {
            if (instance != null && instance != me)
                Object.Destroy(me.gameObject);
            else
                instance = me;
        }

        public static void OnDestroy<T>(ref T instance, T me)
            where T : MonoBehaviour
        {
            if(instance == me)
                instance = null;
        }
    }
}