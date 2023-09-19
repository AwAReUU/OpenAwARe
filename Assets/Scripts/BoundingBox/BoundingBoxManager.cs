using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundingBoxManager : PlaceObject
{
    public Vector3 location;
    public Vector3 rotation;
    public Vector3 scale;

    public GameObject boundingBox;
    public GameObject childObject;

    protected GameObject sceneBox;

    protected override void Interact(Vector2 screenPoint)
    {
        if (sceneBox == null)
            SpawnBoxedObject(location, Quaternion.Euler(rotation), scale);
        else
            DestroyBoxedObject();
    }

    public void SpawnBoxedObject(Vector3 location, Quaternion rotation, Vector3 scale)
    {
        sceneBox = Instantiate(this.boundingBox, location, rotation);
        sceneBox.transform.localScale = scale;

        BoundingBoxBehaviour boundingBoxBehaviour = sceneBox.GetComponent(typeof(BoundingBoxBehaviour)) as BoundingBoxBehaviour;
        GameObject childObject = Instantiate(this.childObject, sceneBox.transform);
        boundingBoxBehaviour.childObject = childObject;

    }

    public void DestroyBoxedObject()
    {
        Destroy(sceneBox);
        sceneBox = null;
    }
}
