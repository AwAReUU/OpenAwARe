// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

using System;
using UnityEngine;
using Object = UnityEngine.Object;

public static class Mover
{
    public static void MoveAllChildren(Transform parent, Transform newParent, bool destroyOldParent = false)
    {
        if (parent == null)
            throw new ArgumentNullException(nameof(parent));
        if (newParent == null)
            throw new ArgumentNullException(nameof(newParent));

        for (var i = 0; i < parent.childCount; i++)
            parent.GetChild(i).transform.SetParent(newParent, false);

        if(destroyOldParent) Object.Destroy(parent.gameObject);
    }
}