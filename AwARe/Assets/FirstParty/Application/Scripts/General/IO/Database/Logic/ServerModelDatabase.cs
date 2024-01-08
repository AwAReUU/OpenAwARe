using System;
using System.Collections.Generic;

using AwARe.Server.Logic;
using AwARe.IngredientList.Logic;
using UnityEngine;
using AwARe.ResourcePipeline.Logic;

namespace AwARe.Database.Logic
{
    /// <summary>
    /// Implementation of IIngredientDatabase that sends requests to the server to query data.
    /// </summary>
    internal class ServerModelDatabase : IModelDatabase
    {
        private Client client;

        public ServerModelDatabase(Client client)
        {
            this.client = client;
        }
        public Model GetModel(int id)
        {
            throw new NotImplementedException();
        }

        public List<Model> GetModels(IEnumerable<int> ids)
        {
            throw new NotImplementedException();
        }
    }
}