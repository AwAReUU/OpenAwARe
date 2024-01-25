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
using UnityEngine;

namespace AwARe.Database.Logic
{
    [Serializable]
    struct GetResourceRequestBody
    {
        public int id;
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
                Debug.Log(res);
                // resources ingr = JsonUtility.FromJson<resources>(res);
                // return ingr;
                return null;
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

