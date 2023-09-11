using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSelector : MonoBehaviour
{
    [SerializeField] private PlaceObject placeObject;

    public void SelectObject(int index)
    {
        placeObject.prefabIndex = index;
    }
}
