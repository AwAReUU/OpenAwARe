using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using AwARe.Database;

using UnityEngine;

namespace AwARe.IngredientList.Logic
{
    public class IngredientFileHandler
    {
        readonly string filePath;
        readonly IIngredientDatabase ingredientDatabase;

        public IngredientFileHandler(IIngredientDatabase database)
        {
            filePath = Application.persistentDataPath + "/ingredientLists";
            this.ingredientDatabase = database;
        }

        // Converts the file contents to an IngredientList
        public List<IngredientList> ReadFile()
        {
            if (!File.Exists(filePath))
            {
                return new List<IngredientList>();
            }

            string json = File.ReadAllText(filePath);

            JSONIngredientInfo info = JsonUtility.FromJson<JSONIngredientInfo>(json);

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
                    {
                        throw new Exception("Cannot convert string to QuantityType.");
                    }
                    try
                    {
                        ingredients.Add(ingredientDatabase.GetIngredient(ingredientID), (ingredientQuantity, ingredientQuantityType));
                    }
                    catch(System.InvalidOperationException)
                    {
                        Debug.LogWarning("Could not find ingredient. Ingredient will be removed");
                    }
                }
                lists.Add(new IngredientList(info.listNames[i], ingredients));
            }

            return lists;
        }

        // Convert the lists to JSON and write this to the file
        public void SaveLists(List<IngredientList> ingredientLists)
        {
            int numberOfLists = ingredientLists.Count;

            JSONIngredientInfo info = new()
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
                    info.ingredientIDs[i] += ingredient.ID.ToString() + ",";
                    info.ingredientQuantities[i] += ingredientLists[i].GetQuantity(ingredient).ToString() + ",";
                    info.ingredientQuantityTypes[i] += ingredientLists[i].GetQuantityType(ingredient).ToString() + ",";
                }
            }

            string json = JsonUtility.ToJson(info);

            File.WriteAllText(filePath, json);
        }
    }

    [Serializable]
    public class JSONIngredientInfo
    {
        public string[] listNames;
        public string[] ingredientIDs;
        public string[] ingredientQuantities;
        public string[] ingredientQuantityTypes;
    }
}