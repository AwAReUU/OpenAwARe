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
