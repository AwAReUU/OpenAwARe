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
    internal class ServerResourceDatabase : IResourceDatabase
    {
        private Client client;

        public ServerResourceDatabase(Client client)
        {
            this.client = client;
        }
        public Resource GetResource(int id)
        {
            throw new NotImplementedException();
        }

        public List<Resource> GetResources(IEnumerable<int> ids)
        {
            throw new NotImplementedException();
        }
    }
}