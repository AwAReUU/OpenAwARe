using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class <c>ObjectDestroyer</c> Implements methods to remove all 
/// generated objects from the scene.
/// </summary>
public class ObjectDestroyer : MonoBehaviour
{
    /// <summary>
    /// Destroy all GameObjects in the Material layer
    /// </summary>
    public void DestroyAllObjects()
    {
        int layer = LayerMask.NameToLayer("Placed Objects");
        GameObject[] generatedObjects = FindGameObjectsInLayer(layer);
        if (generatedObjects == null)
            return;

        foreach (GameObject target in generatedObjects)
            Destroy(target);
    }

    /// <summary>
    /// Obtain all Gameobjects in <paramref name="layer"/>.
    /// </summary>
    /// <param><c>layer</c> is the layer to find all objects from</param>
    /// <returns>All GameObjects in that layer</returns>
    private GameObject[] FindGameObjectsInLayer(int layer)
    {
        var goArray = FindObjectsOfType(typeof(GameObject)) as GameObject[];
        var goList = new List<GameObject>();
        for (int i = 0; i < goArray.Length; i++)
            if (goArray[i].layer == layer)
                goList.Add(goArray[i]);

        if (goList.Count == 0)
            return null;
        return goList.ToArray();
    }
}
