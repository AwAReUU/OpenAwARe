using System;
using System.Collections.Generic;

using AwARe.Server.Logic;
using AwARe.IngredientList.Logic;
using UnityEngine;

namespace AwARe.Database.Logic
{
    /// <summary>
    /// Implementation of IIngredientDatabase that sends requests to the server to query data.
    /// </summary>
    internal class ServerIngredientDatabase : IIngredientDatabase
    {
        private Client client;

        public ServerIngredientDatabase(Client client)
        {
            this.client = client;
        }

        public Ingredient GetIngredient(int id)
        {
            Ingredient ingredient;
            Client.Get("/ingr/getIngredient", id)
                  .Then(res => 
                   { 
                       // get and parse all the ingredient's properties
                       int ingredientId = int.Parse(res["IngredientID"]);
                       string name = res["PrefName"];

                       
                   }).Catch(
                       err => Debug.Log(err))
                  .Send();

            throw new NotImplementedException();
        }

        public List<Ingredient> GetIngredients(IEnumerable<int> ids)
        {
            throw new NotImplementedException();
        }

        public List<Ingredient> Search(string term)
        {
            throw new NotImplementedException();
        }
    }
}
