using System;
using System.Collections.Generic;

namespace ResourceLists
{
    public class ResourceList
    {
        public Dictionary<Resource, float> Resources { get; private set; }

        public ResourceList(Dictionary<Resource, float> resources = null)
        {
            if (resources != null)
                this.Resources = resources;
            else
                this.Resources = new();
        }

        public int NumberOfResources()
        {
            return Resources.Count;
        }

        public void AddResource(Resource resource, float quantity)
        {
            Resources.Add(resource, quantity);
        }
    }

    public class Resource : IEquatable<Resource>
    {
        public int ID { get; }
        public string Name { get; }
        public ResourceType Type { get; }
        public int? GramsPerModel { get; }
        public int ModelID { get; }

        public Resource(int id, string name, ResourceType type, int? gramsPerModel, int modelID)
        {
            ID = id;
            Name = name;
            Type = type;
            GramsPerModel = gramsPerModel;
            ModelID = modelID;
        }

        public override bool Equals(object obj) => obj is Resource m && this.Equals(m);

        public bool Equals(Resource m) => ID == m.ID;

        public override int GetHashCode() => ID.GetHashCode();

    }

    public enum ResourceType
    {
        Animal,
        Plant,
        Water
    }
}