using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

public class SceneSwitcher : MonoBehaviour
{
    public void ChangeScene(int index)
    {
        if (SceneManager.sceneCount <= index)
        {
            Debug.Log("Scene " + index + " does not exist.");
            return;
        }

        GetComponent<ARSession>().Reset();
        SceneManager.LoadScene(index);
    }

    public void ChangeScene(string name)
    {
        GetComponent<ARSession>().Reset();
        SceneManager.LoadScene(name);
    }
}
