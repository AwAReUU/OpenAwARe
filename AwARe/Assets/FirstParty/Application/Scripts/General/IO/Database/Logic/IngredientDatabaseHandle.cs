
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Threading.Tasks;
using AwARe.IngredientList.Logic;
using AwARe.Server.Logic;
using UnityEngine;

namespace AwARe.Database.Logic
{
    [Serializable]
    struct GetIngredientRequestBody
    {
        public int id;
    }

    /// <summary>
    /// A handle to the remote database, that implements the Ingredient Database interface.
    /// The user must be logged in and connected to the server for this to work.
    /// </summary>
    public class IngredientDatabaseHandle : IIngredientDatabase
    {
        public Task<Ingredient> GetIngredient(int id)
        {
            Debug.Log("GETINGR");
            var ingredient = Client.GetInstance().Post<GetIngredientRequestBody, Ingredient>("ingr/getIngredient", new GetIngredientRequestBody
            {
                id = id
            }).Then((res) =>
            {
                // Warning: This is an async method that may run after "LoadScene(...)"!

                // TODO:
                Debug.Log(res);

                return new Ingredient(0, "test");
            }).Catch((err) =>
            {
                // Warning: This is an async method that may run after "LoadScene(...)"!

                if (err.StatusCode == 403)
                {
                    // Unauthorized. User must login.
                    Debug.LogError("Failed to get ingredient from the server. You're not logged in.");
                }
                else
                {
                    Debug.LogError("Failed to get ingredient from the server.: " + err.ServerMessage);
                }
            }).Send();
            Debug.Log("end");

            // while (locked) { }
            return ingredient;
        }

        public Task<List<Ingredient>> GetIngredients(IEnumerable<int> ids)
        {
            Debug.Log("GETINGRs");
            return null;
        }

        public Task<List<Ingredient>> Search(string term)
        {
            Debug.Log("SEARCH");
            return null;
        }
    }
}