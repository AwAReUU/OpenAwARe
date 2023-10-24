using UnityEngine;

public class ObjectPrefabsObjectGen : ObjectPrefabs
{
    //* A singleton class containing a list of all potential objects/ingredients
    public static ObjectPrefabs I { get; private set; }
    public override ObjectPrefabs Me { get => I; protected set => I = value; }
}
