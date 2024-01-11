// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

using System.Collections.Generic;

using AwARe.IngredientList.Logic;
using AwARe.ResourcePipeline.Logic;

namespace AwARe.Database
{
    /// <summary>
    /// Interface that defines methods needed to be implemented to query Ingredient Data.
    /// </summary>
    public interface IIngredientDatabase
    {
        /// <summary>
        /// Selects a full row in the Ingredient table that matches the given ID.
        /// </summary>
        /// <param name="id">The unique identifier of the desired ingredient.</param>
        /// <returns>The full data of the ingredient, which ID matches.</returns>
        public Ingredient GetIngredient(int id);

        /// <summary>
        /// Selects the full rows in the Ingredient table that match the given IDs.
        /// </summary>
        /// <param name="ids">The unique identifiers of the desired ingredients.</param>
        /// <returns>The full data of the ingredients, which IDs match.</returns>
        public List<Ingredient> GetIngredients(IEnumerable<int> ids);

        // returns a List of Ingredients with a (possible) name containing the search term,
        // without any duplicates
        /// <summary>
        /// Selects all ingredients in the database that have a possible name that matches the search term.
        /// </summary>
        /// <param name="term"> The search term to look up ingredients by. </param>
        /// <returns>A List of Ingredients with a (possible) name containing the search term,
        /// sorted by the position of the term within that name, without any duplicates.</returns>
        public List<Ingredient> Search(string term);
    }

    /// <summary>
    /// Interface that defines methods needed to be implemented to find resources needed to produce an ingredient.
    /// </summary>
    public interface IIngrToResDatabase
    {
        // returns a dictionary of the resource IDs for this ingredient,
        // together with the amount of grams of the resource per instance of the ingredient
        /// <summary>
        /// Selects all resource IDs and their quantities required to produce 1 gram of the given ingredient.
        /// </summary>
        /// <param name="ingredient">The ingredient to retrieve the resources from.</param>
        /// <returns>A Dictionary were the keys are ResourceID
        /// and the values are the grams of resource required to produce 1 gram of ingredient.</returns>
        public Dictionary<int, float> GetResourceIDs(Ingredient ingredient);
    }

    /// <summary>
    /// Interface that defines methods needed to be implemented to query Resource data.
    /// </summary>
    public interface IResourceDatabase
    {
        /// <summary>
        /// Selects a full row in the Resource table that matches the given ID.
        /// </summary>
        /// <param name="id">The unique identifier of the desired resource.</param>
        /// <returns>The full data of the resource, which ID matches.</returns>
        public Resource GetResource(int id);

        /// <summary>
        /// Selects the full rows in the Resource table that match the given IDs.
        /// </summary>
        /// <param name="ids">The unique identifiers of the desired resources.</param>
        /// <returns>The full data of the resources, which IDs match.</returns>
        public List<Resource> GetResources(IEnumerable<int> ids);
    }

    /// <summary>
    /// Interface that defines methods needed to be implemented to query Model data.
    /// </summary>
    public interface IModelDatabase
    {
        /// <summary>
        /// Selects a full row in the Model table that matches the given ID.
        /// </summary>
        /// <param name="id">The unique identifier of the desired model.</param>
        /// <returns>The full data of the model, which ID matches.</returns>
        public Model GetModel(int id);

        /// <summary>
        /// Selects the full rows in the Model table that match the given IDs.
        /// </summary>
        /// <param name="ids">The unique identifiers of the desired models.</param>
        /// <returns>The full data of the models, which IDs match.</returns>
        public List<Model> GetModels(IEnumerable<int> ids);
    }
}