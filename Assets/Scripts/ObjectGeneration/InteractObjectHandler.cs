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

    public void ColorObject(GameObject target)
    {
        if (target.GetComponent<MeshRenderer>().material == black)
        {
            target.GetComponent<MeshRenderer>().material = normal;
        }
        else
        {
            target.GetComponent<MeshRenderer>().material = black;
        }
    }
    
    public void ToggleDataWindow(GameObject target)
    {
        GameObject dataWindow = target.transform.GetChild(0).gameObject;
        if (dataWindow.activeSelf)
            dataWindow.SetActive(false);
        else
            dataWindow.SetActive(true);
    }

    public void DestroyTarget(GameObject target)
    {
        Destroy(target);
    }

    public void DestroyAllObjects()
    {
        GameObject[] animals = GameObject.FindGameObjectsWithTag("Animal");
        foreach (GameObject animal in animals)
        {
            Destroy(animal);
        }
    }
}
