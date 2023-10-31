using UnityEngine;

public abstract class ObjectPrefabs : MonoBehaviour
{
    //* A singleton class containing a list of all potential objects/ingredients
    public abstract ObjectPrefabs Me { get; protected set; }
    public GameObject[] prefabs;
    public int prefabIndex;

    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.

        if (Me != null && Me != this)
            Destroy(this);
        else
            Me = this;
    }

    public void SetIndex(int index) =>
        prefabIndex = index;
}


