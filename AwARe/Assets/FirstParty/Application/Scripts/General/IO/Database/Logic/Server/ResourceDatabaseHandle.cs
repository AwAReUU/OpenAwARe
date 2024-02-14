// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AwARe.ResourcePipeline.Logic;
using AwARe.Server.Logic;
using Proyecto26;
using UnityEngine;

namespace AwARe.Database.Logic
{
    [Serializable]
    struct GetResourceRequestBody
    {
        public int id;
    }

    [Serializable]
    struct ResourceResponse
    {
        public int ResourceID;
        public string Name;
        public string Type;
        public int GramsPerModel;
        public int ModelID;
    }

    /// <summary>
    /// Implementation of the Resource Database interface, that uses the remote database.
    /// </summary>
    public class ResourceDatabaseHandle : IResourceDatabase
    {
        /// <inheritdoc/>
        public Task<Resource> GetResource(int id)
        {
            var resource = Client.GetInstance().Get<GetResourceRequestBody, Resource>("ingr/getResource", new GetResourceRequestBody
            {
                id = id
            }).Then((res) =>
            {
                ResourceResponse response = JsonUtility.FromJson<ResourceResponse>(res);
                ResourceType resourceType = ResourceType.Plant;

                switch (response.Type)
                {
                    case "Water":
                        resourceType = ResourceType.Water;
                        break;
                    case "Plant":
                        resourceType = ResourceType.Plant;
                        break;
                    case "Animal":
                        resourceType = ResourceType.Animal;
                        break;
                }

                return new Resource(response.ResourceID, response.Name, resourceType, response.GramsPerModel, response.ModelID);

            }).Catch((err) =>
                {
                    if (err.StatusCode == 403)
                    {
                        // Unauthorized. User must login.
                        Debug.LogError("Failed to get resource from the server. You're not logged in.");
                    }
                    else
                    {
                        Debug.LogError("Failed to get resource from the server.: [" + err.StatusCode + "] " + err.ServerMessage);
                    }
                }).Send();

            return resource;
        }

        /// <inheritdoc/>
        public Task<Resource[]> GetResources(IEnumerable<int> ids)
        {
            List<Task<Resource>> tasks = new();
            foreach (int id in ids)
            {
                tasks.Add(this.GetResource(id));
            }
            return Task.WhenAll(tasks);
        }
    }
}

