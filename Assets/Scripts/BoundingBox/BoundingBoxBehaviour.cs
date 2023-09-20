using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundingBoxBehaviour : MonoBehaviour
{
    protected GameObject childObject;
    public Func<Transform, GameObject> getChild;

    // Start is called before the first frame update
    void Start()
    {
        OnStart();
    }

    protected virtual void OnStart()
    {
        childObject = getChild(this.transform);
        SetLocalTransform();
    }

    protected virtual void SetLocalTransform() { }
}
