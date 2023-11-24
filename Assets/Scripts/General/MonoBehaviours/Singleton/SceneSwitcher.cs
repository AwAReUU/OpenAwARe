using AwARe.MonoBehaviours;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR.ARCore;
using UnityEngine.XR.ARFoundation;

public class SceneSwitcher : MonoBehaviour
{
    private static SceneSwitcher instance;

    private void Awake()
    {
        Singleton.Awake(ref instance, this);
        DontDestroyOnLoad(this.gameObject);
    }

    protected virtual void OnDestroy() =>
        Singleton.OnDestroy(ref instance, this);

    public static SceneSwitcher Get() =>
        Singleton.Get(ref instance, Instantiate);

    public static SceneSwitcher Instantiate() =>
        new GameObject("SceneSwitcher").AddComponent<SceneSwitcher>();

    private void ResetARSession()
    {
        ARSession arSession = GetComponent<ARSession>();
        if (arSession != null)
            arSession.Reset();
    }

    public void ChangeScene(int index)
    {
        if (SceneManager.sceneCount <= index)
        {
            Debug.Log("Scene " + index + " does not exist.");
            return;
        }

        ResetARSession();
        SceneManager.LoadScene(index);
    }

    public void ChangeScene(string name)
    {
        ResetARSession();
        SceneManager.LoadScene(name);
    }
}
