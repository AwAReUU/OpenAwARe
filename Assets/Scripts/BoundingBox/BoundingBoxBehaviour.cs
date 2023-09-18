using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundingBoxBehaviour : MonoBehaviour
{
    public GameObject childObject;

    // Start is called before the first frame update
    void Start()
    {
        OnStart();
    }

    protected virtual void OnStart()
    {
        SetLocalTransform();
    }

    protected virtual void SetLocalTransform()
    {
        childObject.transform.parent = this.transform;
    }
}
