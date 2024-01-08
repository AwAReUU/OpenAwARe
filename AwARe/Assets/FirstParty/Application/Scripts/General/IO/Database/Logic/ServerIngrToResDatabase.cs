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
    internal class ServerIngrToResDatabase : IIngrToResDatabase
    {
        private Client client;

        public ServerIngrToResDatabase(Client client)
        {
            this.client = client;
        }

        public Dictionary<int, float> GetResourceIDs(Ingredient ingredient)
        {
            throw new NotImplementedException();
        }
    }
}