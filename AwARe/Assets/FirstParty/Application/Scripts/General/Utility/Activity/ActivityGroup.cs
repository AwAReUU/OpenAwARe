// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AwARe
{
    public class ActivityGroup : MonoBehaviour
    {
        [SerializeField] private List<ActivityGroupMember> members = new();

        private void Awake()
        {
            foreach (var member in members)
                member.group = this;
        }

        public void Add(GameObject member)
        {
            if (!member)
                return;
            if (!member.TryGetComponent(out ActivityGroupMember component))
                component = member.AddComponent<ActivityGroupMember>();
            Add(component);
        }

        public void Add(ActivityGroupMember member)
        {
            if (!member)
                return;
            member.group = this;
            members.Add(member);
        }

        public void Remove(GameObject member)
        {
            if (!member)
                return;
            Remove(member ? member.GetComponent<ActivityGroupMember>() : null);
        }

        public void Remove(ActivityGroupMember member)
        {
            if (!member)
                return;
            Destroy(member);
        }

        internal void GotDestroyed(ActivityGroupMember member) =>
            members.Remove(member);

        public void SetActive(bool status, SetMode setMode = SetMode.Single, ActivityGroupMember member = null)
        {
            if(setMode == SetMode.Single)
                foreach (var other in members.Where(m => !m.Equals(member)))
                    other.gameObject.SetActive(false);
            if (member)
                member.gameObject.SetActive(status);
        }

        public void OnDestroy()
        {
            foreach (var member in members)
                Destroy(member);
        }

        public enum SetMode { Single, Additive }
    }
}