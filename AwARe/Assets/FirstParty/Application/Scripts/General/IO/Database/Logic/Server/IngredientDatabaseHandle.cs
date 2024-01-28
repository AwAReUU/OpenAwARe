
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using AwARe.IngredientList.Logic;
using AwARe.Server.Logic;
using Proyecto26;
using UnityEditor.Compilation;
using UnityEngine;

namespace AwARe.Database.Logic
{
    [Serializable]
    struct GetIngredientRequestBody
    {
        public int id;
    }

    [Serializable]
    struct SearchRequestBody
    {
        public string query;
    }

    [Serializable]
    struct IngredientResponse
    {
        public int IngredientID;
        public string PrefName;
        public float GramsPerML;
        public float GramsPerPiece;
    }

    /// <summary>
    /// A handle to the remote database, that implements the Ingredient Database interface.
    /// The user must be logged in and connected to the server for this to work.
    /// </summary>
    public class IngredientDatabaseHandle : IIngredientDatabase
    {
        /// <inheritdoc/>
        public Task<Ingredient> GetIngredient(int id)
        {
            var ingredient = Client.GetInstance().Get<GetIngredientRequestBody, Ingredient>("ingr/getIngredient", new GetIngredientRequestBody
            {
                id = id
            }).Then((res) =>
            {
                IngredientResponse ingr = JsonUtility.FromJson<IngredientResponse>(res);

                return new Ingredient(ingr.IngredientID, ingr.PrefName, ingr.GramsPerML, ingr.GramsPerPiece);
            }).Catch((err) =>
                {
                    if (err.StatusCode == 403)
                    {
                        // Unauthorized. User must login.
                        Debug.LogError("Failed to get ingredient from the server. You're not logged in.");
                    }
                    else
                    {
                        Debug.LogError("Failed to get ingredient from the server.: [" + err.StatusCode + "] " + err.ServerMessage);
                    }
                }).Send();

            if (ingredient == null) Debug.LogError("Failed to fetch ingredient from remote database.");
            return ingredient;
        }

        /// <inheritdoc/>
        public Task<List<Ingredient>> GetIngredients(IEnumerable<int> ids)
        {
            return Task.Run(async () =>
            {
                List<Ingredient> ingredients = new();
                foreach (int id in ids)
                {
                    ingredients.Add(await this.GetIngredient(id));
                }
                return ingredients;
            });
        }

        /// <inheritdoc/>
        public Task<List<Ingredient>> Search(string term)
        {
            var results = Client.GetInstance().Get<SearchRequestBody, List<Ingredient>>("ingr/search", new SearchRequestBody
            {
                query = term
            }).Then((res) =>
            {
                IngredientResponse[] responses = JsonHelper.FromJsonString<IngredientResponse>("{ \"Items\": " + res + "}");
                var ingredients = new List<Ingredient>();
                foreach (IngredientResponse ingr in responses)
                {
                    ingredients.Add(new Ingredient(ingr.IngredientID, ingr.PrefName, ingr.GramsPerML, ingr.GramsPerPiece));
                }
                return ingredients;
            }).Catch((err) =>
                {
                    if (err.StatusCode == 403)
                    {
                        // Unauthorized. User must login.
                        Debug.LogError("Failed to search ingredients on the server. You're not logged in.");
                    }
                    else
                    {
                        Debug.LogError("Failed to search ingredients on the server.: [" + err.StatusCode + "] " + err.ServerMessage);
                    }
                }).Send();


            return results;
        }
    }
}