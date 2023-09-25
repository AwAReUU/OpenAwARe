using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPrefabs : MonoBehaviour
{
    public static ObjectPrefabs I { get; private set; }
    public GameObject[] prefabs;
    public int prefabIndex;

    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.

        if (I != null && I != this)
        {
            Destroy(this);
        }
        else
        {
            I = this;
        }
    }

    public void SetIndex(int index)
    {
        prefabIndex = index;
    }
}
