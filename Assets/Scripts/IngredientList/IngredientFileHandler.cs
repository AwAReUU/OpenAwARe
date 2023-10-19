using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;
using System.Linq;
using UnityEngine.InputSystem;

public class IngredientFileHandler
{
    readonly string filePath;
    readonly IIngredientDatabase ingredientDatabase;

    public IngredientFileHandler(IIngredientDatabase database)
    {
        filePath = Application.persistentDataPath + "/ingredientLists";
        this.ingredientDatabase = database;
    }

    public List<IngredientList> ReadFile()
    {
        if (!File.Exists(filePath))
        {
            return new List<IngredientList>();
        }

        string json = File.ReadAllText(filePath);

        JSONIngredientInfo info = JsonUtility.FromJson<JSONIngredientInfo>(json);

        List<IngredientList> lists = new();

        // reconstruct all lists
        for (int i = 0; i < info.listNames.Length; i++)
        {
            Dictionary<Ingredient, float> ingredients = new();

            string[] ingredientIDs;
            string[] ingredientQuantities;

            try
            {
                ingredientIDs = info.ingredientIDs[i].Split(",");
                ingredientQuantities = info.ingredientQuantities[i].Split(",");
            }
            catch (System.NullReferenceException)
            {
                Debug.LogWarning("IngredientLists file is not in correct format. Lists will be deleted");
                File.Delete(filePath); // empties the saved ingredientLists
                return lists;
            }

            // add the ingredients to the lists
            for (int j = 0; j < ingredientIDs.Length - 1; j++)
            {
                int ingredientID = int.Parse(ingredientIDs[j]);
                float ingredientQuantity = float.Parse(ingredientQuantities[j]);

                ingredients.Add(ingredientDatabase.GetIngredient(ingredientID), ingredientQuantity);
            }
            lists.Add(new IngredientList(info.listNames[i], ingredients));
        }

        return lists;
    }

    public void SaveLists(List<IngredientList> ingredientLists)
    {
        int numberOfLists = ingredientLists.Count;

        JSONIngredientInfo info = new()
        {
            listNames = new string[numberOfLists],
            ingredientIDs = new string[numberOfLists],
            ingredientQuantities = new string[numberOfLists]
        };

        // convert the lists to strings
        for (int i = 0; i < numberOfLists; i++)
        {
            info.listNames[i] = ingredientLists[i].ListName;

            info.ingredientIDs[i] = "";
            info.ingredientQuantities[i] = "";

            for (int j = 0; j < ingredientLists[i].NumberOfIngredients(); j++)
            {
                Ingredient ingredient = ingredientLists[i].Ingredients.ElementAt(j).Key;
                info.ingredientIDs[i] += ingredient.ID + ",";
                info.ingredientQuantities[i] += ingredientLists[i].Ingredients[ingredient] + ",";
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
}