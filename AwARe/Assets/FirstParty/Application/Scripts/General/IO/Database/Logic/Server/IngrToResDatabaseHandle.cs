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
using AwARe.IngredientList.Logic;
using AwARe.Server.Logic;
using Proyecto26;
using UnityEngine;

namespace AwARe.Database.Logic
{
    [Serializable]
    struct GetRequirementsRequestBody
    {
        public int id;
    }

    [Serializable]
    struct RequirementResponse
    {
        public int IngredientID;
        public int ResourceID;
        public float ResPerIngr;
    }

    /// <summary>
    /// Implementation of the Ingredient-to-Resource Database interface, that uses the remote database.
    /// </summary>
    public class IngrToResDatabaseHandle : IIngrToResDatabase
    {
        public Task<Dictionary<int, float>> GetResourceIDs(Ingredient ingredient)
        {
            var ingrToResource = Client.GetInstance().Get<GetRequirementsRequestBody, Dictionary<int, float>>("ingr/getRequirements", new GetRequirementsRequestBody
            {
                id = ingredient.IngredientID
            }).Then((res) =>
            {
                RequirementResponse[] responses = JsonHelper.FromJsonString<RequirementResponse>("{ \"Items\": " + res + "}");
                var requirements = new Dictionary<int, float>();
                foreach (RequirementResponse req in responses)
                {
                    requirements.Add(req.ResourceID, req.ResPerIngr);
                }
                return requirements;
            }).Catch((err) =>
                {
                    if (err.StatusCode == 403)
                    {
                        // Unauthorized. User must login.
                        Debug.LogError("Failed to get ingredient-to-resource from the server. You're not logged in.");
                    }
                    else
                    {
                        Debug.LogError("Failed to get ingredient-to-resource from the server.: [" + err.StatusCode + "] " + err.ServerMessage);
                    }
                }).Send();

            return ingrToResource;
        }
    }
}
