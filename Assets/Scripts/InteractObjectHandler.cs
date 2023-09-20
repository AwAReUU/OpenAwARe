using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class InteractObjectHandler : MonoBehaviour
{
    [SerializeField] private Material black;
    [SerializeField] private Material normal;

    public void ColorObject(GameObject obj)
    {
        if (obj.GetComponent<MeshRenderer>().material == black)
        {
            obj.GetComponent<MeshRenderer>().material = normal;
        }
        else
        {
            obj.GetComponent<MeshRenderer>().material = black;
        }
    }
    
    public void ToggleDataWindow(GameObject obj)
    {
        GameObject dataWindow = obj.transform.GetChild(0).gameObject;
        if (dataWindow.activeSelf)
            dataWindow.SetActive(false);
        else
            dataWindow.SetActive(true);
    }

    public void DestroyObject(GameObject obj)
    {
        Destroy(obj);
    }

    public void DestroyAllObjects()
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag("Animal");
        foreach (GameObject obj in objs)
        {
            Destroy(obj);
        }
    }
}
