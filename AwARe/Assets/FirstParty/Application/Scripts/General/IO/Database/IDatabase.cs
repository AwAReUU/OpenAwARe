using System.Collections.Generic;

using AwARe.IngredientList.Logic;
using AwARe.ResourcePipeline.Logic;

namespace AwARe.Database
{
    public interface IIngredientDatabase
    {

        // returns the full data of an ingredient, given its unique IngredientID
        public Ingredient GetIngredient(int id);
        public List<Ingredient> GetIngredients(IEnumerable<int> ids);

        // returns a List of Ingredients with a (possible) name containing the search term,
        // without any duplicates
        public List<Ingredient> Search(string term);
    }

    public interface IIngrToResDatabase
    {
        // returns a dictionary of the resource IDs for this ingredient,
        // together with the amount of grams of the resource per instance of the ingredient
        public Dictionary<int, float> GetResourceIDs(Ingredient ingredient);
    }

    public interface IResourceDatabase
    {
        // returns the full data of a resource, given its unique Resource ID
        public Resource GetResource(int id);
        public List<Resource> GetResources(IEnumerable<int> ids);
    }

    public interface IModelDatabase
    {
        public Model GetModel(int id);

        public List<Model> GetModels(IEnumerable<int> ids);
    }
}