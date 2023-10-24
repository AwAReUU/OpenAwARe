using System;
using System.Collections.Generic;

namespace ModelLists
{
    public class ModelList
    {
        // dictionary with models and their respective quantities
        public Dictionary<Model, float> Models { get; private set; }

        public ModelList(Dictionary<Model, float> models = null)
        {
            if (models != null)
                this.Models = models;
            else
                this.Models = new();
        }

        public int NumberOfModels()
        {
            return Models.Count;
        }

        public void AddModel(Model model, float quantity)
        {
            Models.Add(model, quantity);
        }
    }

    public class Model : IEquatable<Model>
    {
        public int ID { get; }
        public ResourceType Type { get; }
        public string PrefabPath { get; }
        public float RealLength { get; }
        public float RealWidth { get; }
        public float RealHeight { get; }
        public float DistanceX { get; }
        public float DistanceY { get; }

        public Model(int id, ResourceType type, string path, float length, float width, float height, float distanceX, float distanceY)
        {
            ID = id;
            Type = type;
            PrefabPath = path; 
            RealLength = length;
            RealWidth = width;
            RealHeight = height; 
            DistanceX = distanceX;
            DistanceY = distanceY;
        }

        public override bool Equals(object obj) => obj is Model m && this.Equals(m);

        public bool Equals(Model m) => ID == m.ID;

        public override int GetHashCode() => ID.GetHashCode();

    }

    public enum ResourceType
    {
        Animal,
        Plant,
        Water
    }
}