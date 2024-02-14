// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

using AwARe.Database;
using UnityEngine;

namespace AwARe.IngredientList.Logic
{
    /// <summary>
    /// Handles the loading and parsing of a local JSON file to a <see cref="List{T}"/> of <see cref="IngredientList"/> and vice versa.
    /// </summary>
    public class IngredientFileHandler
    { 
        readonly string filePath;
        readonly IIngredientDatabase ingredientDatabase;

        /// <summary>
        /// Initializes a new instance of the <see cref="IngredientFileHandler"/> class.
        /// </summary>
        /// <param name="database">The database implementation used for querying <c>Ingredients</c> using their saved IDs.</param>
        public IngredientFileHandler(IIngredientDatabase database)
        {
            filePath = Application.persistentDataPath + "/ingredientLists";
            ingredientDatabase = database;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IngredientFileHandler"/> class.
        /// Sets the filePath used for read/write to the given value.
        /// </summary>
        /// <param name="database"></param>
        /// <param name="path"></param>
        public IngredientFileHandler(IIngredientDatabase database, string path)
        {
            filePath = path;
            ingredientDatabase = database;
        }

        /// <summary>
        /// Converts the file contents to an <see cref="IngredientList"/> List.
        /// </summary>
        /// <returns>The List of <see cref="IngredientList"/>s from converting th file contents.</returns>
        /// <exception cref="Exception">Thrown when a conversion from string to QuantityType fails.</exception>
        public async Task<List<IngredientList>> ReadFile()
        {
            if (!File.Exists(filePath))
                return new List<IngredientList>();

            JSONIngredientListsObject info = IngredientListsJsonHelper.GetJsonIngredientInfo(filePath);

            List<IngredientList> lists = new();

            // reconstruct all Lists
            for (int i = 0; i < info.listNames.Length; i++)
            {
                Dictionary<Ingredient, (float, QuantityType)> ingredients = new();

                string[] ingredientIDs;
                string[] ingredientQuantities;
                string[] ingredientQuantityTypes;

                try
                {
                    ingredientIDs = info.ingredientIDs[i].Split(",");
                    ingredientQuantities = info.ingredientQuantities[i].Split(",");
                    ingredientQuantityTypes = info.ingredientQuantityTypes[i].Split(",");
                }
                catch (System.NullReferenceException)
                {
                    Debug.LogWarning("IngredientLists file is not in correct format. Lists will be deleted");
                    File.Delete(filePath); // empties the saved ingredientLists
                    return lists;
                }

                // add the ingredients to the Lists
                for (int j = 0; j < ingredientIDs.Length - 1; j++)
                {
                    int ingredientID = int.Parse(ingredientIDs[j]);
                    float ingredientQuantity = float.Parse(ingredientQuantities[j]);

                    bool stringToQType = Enum.TryParse(ingredientQuantityTypes[j], out QuantityType ingredientQuantityType);
                    if (!stringToQType)
                        throw new Exception("Cannot convert string to QuantityType.");
                    try
                    {
                        var ingr = await ingredientDatabase.GetIngredient(ingredientID);
                        if (ingr != null)
                        {
                            ingredients.Add(ingr, (ingredientQuantity, ingredientQuantityType));
                        }
                    }
                    catch (System.InvalidOperationException)
                    {
                        Debug.LogWarning("Could not find ingredient. Ingredient will be removed");
                    }
                }
                lists.Add(new IngredientList(info.listNames[i], ingredients));
            }

            return lists;
        }

        /// <summary>
        /// Converts the <see cref="IngredientList"/> Lists to JSON and writes this to the local file.
        /// </summary>
        /// <param name="ingredientLists">The ingredient lists to be saved to JSON format.</param>
        public void SaveLists(List<IngredientList> ingredientLists)
        {
            string json = IngredientListsJsonHelper.IngredientListsToJSONString(ingredientLists);
            File.WriteAllText(filePath, json);
        }
    }

    /// <summary>
    /// JSON object format that a List of IngredientLists can be converted to.
    /// </summary>
    [Serializable]
    public class JSONIngredientListsObject
    {
        /// <summary>
        /// The ListName of every IngredientList that was in the List.
        /// </summary>
        public string[] listNames;

        /// <summary>
        /// The lists of all IDs of the ingredients in every IngredientList, converted to string.
        /// </summary>
        public string[] ingredientIDs;

        /// <summary>
        /// The lists of all quantities of the ingredients in every IngredientList, converted to string.
        /// </summary>
        public string[] ingredientQuantities;

        /// <summary>
        /// The lists of all QuantityTypes of the ingredients in every IngredientList, converted to string.
        /// </summary>
        public string[] ingredientQuantityTypes;
    }

    /// <summary>
    /// Class <c>IngredientListsJsonHelper</c> provides utility methods for IngredientListsJson objects.
    /// </summary>
    public class IngredientListsJsonHelper
    {
        /// <summary>
        /// Read the given path into a json object.
        /// </summary>
        /// <param name="path">Path at which the file to be read is located.</param>
        /// <returns>JsonObject read from file at the given path.</returns>
        public static JSONIngredientListsObject GetJsonIngredientInfo(string path)
        {
            string json = File.ReadAllText(path);
            JSONIngredientListsObject info = JsonUtility.FromJson<JSONIngredientListsObject>(json);
            return info;
        }

        /// <summary>
        /// Converts a list of <see cref="IngredientList"/> to a json string.
        /// </summary>
        /// <param name="ingredientLists">Ingredientlists to convert.</param>
        /// <returns>A Json string containing the data from the given IngredientLists.</returns>
        public static string IngredientListsToJSONString(List<IngredientList> ingredientLists)
        {
            int numberOfLists = ingredientLists.Count;

            JSONIngredientListsObject info = new()
            {
                listNames = new string[numberOfLists],
                ingredientIDs = new string[numberOfLists],
                ingredientQuantities = new string[numberOfLists],
                ingredientQuantityTypes = new string[numberOfLists]
            };

            // convert the Lists to strings
            for (int i = 0; i < numberOfLists; i++)
            {
                info.listNames[i] = ingredientLists[i].ListName;

                info.ingredientIDs[i] = "";
                info.ingredientQuantities[i] = "";
                info.ingredientQuantityTypes[i] = "";

                for (int j = 0; j < ingredientLists[i].NumberOfIngredients(); j++)
                {
                    Ingredient ingredient = ingredientLists[i].Ingredients.ElementAt(j).Key;
                    info.ingredientIDs[i] += ingredient.IngredientID.ToString() + ",";
                    info.ingredientQuantities[i] += ingredientLists[i].GetQuantity(ingredient).ToString() + ",";
                    info.ingredientQuantityTypes[i] += ingredientLists[i].GetQuantityType(ingredient).ToString() + ",";
                }
            }
            string json = JsonUtility.ToJson(info);
            return json;
        }
    }
}