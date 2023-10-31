using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectGenDropDownHandler : DropdownHandler
{
    protected override ObjectPrefabs prefabs => ObjectPrefabsObjectGen.I;
}
