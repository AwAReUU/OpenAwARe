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
        public Task<Resource> GetResource(int id)
        {
            var resource = Client.GetInstance().Get<GetResourceRequestBody, Resource>("ingr/getResource", new GetResourceRequestBody
            {
                id = id
            }).Then((res) =>
            {
                Debug.Log(res); // {"ResourceID":1,"Name":"Water","Type":"Water","GramsPerModel":null,"ModelID":1}

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

        public Task<List<Resource>> GetResources(IEnumerable<int> ids)
        {
            return Task.Run(async () =>
            {
                List<Resource> resources = new();
                foreach (int id in ids)
                {
                    resources.Add(await this.GetResource(id));
                }
                return resources;
            });
        }
    }
}

