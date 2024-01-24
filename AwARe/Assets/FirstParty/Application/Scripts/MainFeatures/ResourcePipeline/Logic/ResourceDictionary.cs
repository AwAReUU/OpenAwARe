using System;
using System.Collections.Generic;

namespace AwARe.ResourcePipeline.Logic
{
    /// <summary>
    /// Class <c>ResourceList</c> is responsible for storing the resources.
    /// </summary>
    public class ResourceDictionary
    {
        /// <value>
        /// Resources with their corresponding quantities.
        /// </value>
        public Dictionary<Resource, float> Resources { get; private set; }

        /// <summary>
        /// Constructor. Initializes Resources using the resources parameter.
        /// If this parameter is null, it initializes a new Resources dictionary.
        /// </summary>
        /// <param name="resources">ResourceList to set</param>
        public ResourceDictionary(Dictionary<Resource, float> resources = null)
        {
            Resources = resources ?? new Dictionary<Resource, float>();
        }

        /// <summary>
        /// Get the number of unique resources
        /// </summary>
        /// <returns></returns>
        public int NumberOfResources() => Resources.Count;

        /// <summary>
        /// Add a Resource to the <see cref="ResourceDictionary"/>
        /// </summary>
        /// <param name="resource">Resource to add.</param>
        /// <param name="quantity">quantity of the resource to add.</param>
        public void AddResource(Resource resource, float quantity) => Resources.Add(resource, quantity);
    }

    /// <summary>
    /// Class <c>Resource</c> is responsible for storing some properties of a Resource.
    /// </summary>
    public class Resource : IEquatable<Resource>
    {
        /// <value>
        /// ID of the resource.
        /// </value>
        public int ID { get; }
        /// <value>
        /// Name of the resource.
        /// </value>
        public string Name { get; }
        /// <value>
        /// ResourceType of the resource. (Animal/Plant/Water)
        /// </value>
        public ResourceType Type { get; }
        /// <value>
        /// Amount of grams of this resource that can be represented in one model
        /// </value>
        public int? GramsPerModel { get; }
        /// <value>
        /// The ID of the model that corresponds to this resource.
        /// </value>
        public int ModelID { get; }

        /// <summary>
        /// Constructor. Create a Resource using the given parameters.
        /// </summary>
        /// <param name="id">ID of the resource.</param>
        /// <param name="name">Name of the resource.</param>
        /// <param name="type">ResourceType of the resource. (Animal/Plant/Water)</param>
        /// <param name="gramsPerModel">Amount of grams of this resource that can be represented in one model</param>
        /// <param name="modelID">The ID of the model that corresponds to this resource.</param>
        public Resource(int id, string name, ResourceType type, int? gramsPerModel, int modelID)
        {
            ID = id;
            Name = name;
            Type = type;
            GramsPerModel = gramsPerModel;
            ModelID = modelID;
        }

        /// <summary>
        /// Check if both Resources are the same.
        /// </summary>
        /// <param name="obj">object to compare the other Resource to</param>
        /// <returns>Whether they are the same.</returns>
        public override bool Equals(object obj) => obj is Resource r && this.Equals(r);

        /// <summary>
        /// Check if both resources are the same, by checking if their Ids are the same.
        /// </summary>
        /// <param name="r">Resource to compare the other Resource to.</param>
        /// <returns>Whether they are the same.</returns>
        public bool Equals(Resource r) => ID == r.ID;

        /// <summary>
        /// Use ID of <see cref="Resource"/> for hashing, so that it can be used as dictionary key.
        /// </summary>
        /// <returns>HashCode</returns>
        public override int GetHashCode() => ID.GetHashCode();

    }

    /// <summary>
    /// Animal, Plant, Water.
    /// </summary>
    public enum ResourceType
    {
        Animal,
        Plant,
        Water
    }
}