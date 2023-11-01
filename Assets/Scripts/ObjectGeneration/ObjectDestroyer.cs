using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectDestroyer : MonoBehaviour
{
    public void DestroyAllObjects()
    {
        //! Tag("Animal") should be replaced/generalized in the future, not just here but in the prefabs as a whole
        GameObject[] generatedObjects = GameObject.FindGameObjectsWithTag("Animal");
        foreach (GameObject target in generatedObjects)
            Destroy(target);
    }
}
