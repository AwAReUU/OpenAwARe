using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundingBox
{
    public Transform transform;

    public Transform Transform => transform;

    // From global world coordinates to local coordinates. The local coordinates have the bounds on 0 and 1.
    public Transform MapToLocalCoordinates(Transform transfrom)
    {
        return this.transform; // TODO
    }
}
