// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

using System;

using UnityEngine;
using static AwARe.ActivityGroup;

namespace AwARe
{
    public class ActivityGroupMember : MonoBehaviour
    {
        [NonSerialized] public ActivityGroup group;

        public void SetActive(bool status) =>
            SetActive(status, SetMode.Single);

        public void SetActive(bool status, SetMode setMode)
        {
            if(group != null)
                group.SetActive(status, setMode, this);
        }

        public void Toggle() =>
            Toggle(SetMode.Single);

        public void Toggle(SetMode setMode) =>
            SetActive(!gameObject.activeSelf, setMode);

        public void OnDestroy()
        {
            if(group != null)
                group.GotDestroyed(this);
        }
    }
}