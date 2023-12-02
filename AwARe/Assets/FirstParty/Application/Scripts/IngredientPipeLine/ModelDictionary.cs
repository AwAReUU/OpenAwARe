using System;
using System.Collections.Generic;

namespace IngredientPipeLine
{
    /// <summary>
    /// Class <c>ModelDictionary</c> is responsible for storing the Models. 
    /// </summary>
    public class ModelDictionary
    {
        // dictionary with models and their respective quantities
        public Dictionary<Model, float> Models { get; }

        /// <summary>
        /// Constructor. Sets the Models dictionary to the given parameter
        /// if it has not been set yet.
        /// </summary>
        /// <param name="models">Models dictionary to set.</param>
        public ModelDictionary(Dictionary<Model, float> models = null)
        {
            this.Models = models ?? new Dictionary<Model, float>();
        }

        /// <summary>
        /// Get count of the models dictionary
        /// </summary>
        /// <returns>The count of the models dictionary</returns>
        public int NumberOfModels() => Models.Count;
        /// <summary>
        /// Add a model to the dictionary.
        /// </summary>
        /// <param name="model">model to add to the dictionary</param>
        /// <param name="quantity">quantity of this model</param>
        public void AddModel(Model model, float quantity) => Models.Add(model, quantity);
    }

    /// <summary>
    /// Class <c>Model</c> contains the information about a model from the database that we will need.
    /// </summary>
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

        /// <summary>
        /// Construct a Model with the given parameters.
        /// </summary>
        /// <param name="id">id</param>
        /// <param name="type">ResourceType</param>
        /// <param name="path">Prefab path</param>
        /// <param name="length">real life length</param>
        /// <param name="width">real life width</param>
        /// <param name="height">real life height</param>
        /// <param name="distanceX">space it needs in the x direction</param>
        /// <param name="distanceY">space it needs in the y(should be z?) direction</param>
        public Model(
            int id,
            ResourceType type,
            string path,
            float length,
            float width,
            float height,
            float distanceX,
            float distanceY)
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

        /// <summary>
        /// Check if both models are the same.
        /// </summary>
        /// <param name="obj">object to compare the other model to</param>
        /// <returns>Whether they are the same.</returns>
        public override bool Equals(object obj) => obj is Model m && this.Equals(m);

        /// <summary>
        /// Check if both models are the same, by checking if their Ids are the same.
        /// </summary>
        /// <param name="m">Model to compare the other model to.</param>
        /// <returns>Whether they are the same.</returns>
        public bool Equals(Model m) => ID == m.ID;

        public override int GetHashCode() => ID.GetHashCode();
    }
}