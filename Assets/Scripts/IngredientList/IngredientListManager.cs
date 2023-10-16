using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;
using System.Linq;
using UnityEngine.InputSystem;
//using System.Text.Json;
//using System.Text.Json.Serialization;

public class IngredientListManager : MonoBehaviour
{
    public List<IngredientList> ingredientLists { get; private set; }

    //public IngredientList currentIngredientList;
    public int currentListIndex = -1;
    //public int currentIngredientIndex = -1;
    public Ingredient currentIngredient;

    // objects assigned within unity
    [SerializeField] private GameObject listsOverviewScreen;
    [SerializeField] private GameObject ingredientListScreen;
    [SerializeField] private GameObject addIngredientScreen;
    [SerializeField] private GameObject ingredientScreen;

    string filePath;

    private void Awake()
    {
        filePath = Application.persistentDataPath + "/ingredientLists";
        //File.Delete(filePath); // use this for emptying the saved ingredientLists
        ingredientLists = ReadFile();
        listsOverviewScreen.SetActive(true);
    }

    List<IngredientList> ReadFile()
    {
        if (!File.Exists(filePath))
        {
            return new List<IngredientList>();
        }
            
        string json = File.ReadAllText(filePath);

        JSONIngredientInfo info = JsonUtility.FromJson<JSONIngredientInfo>(json);

        List<IngredientList> lists = new List<IngredientList>();
        
        // reconstruct all lists
        for (int i = 0; i < info.listNames.Length; i++)
        {
            Dictionary<Ingredient, float> ingredients = new();

            string[] ingredientIDs = info.ingredientIDs[i].Split(",");
            string[] ingredientNames = info.ingredientNames[i].Split(",");
            string[] ingredientQuantityTypes = info.ingredientQuantityTypes[i].Split(",");
            string[] ingredientQuantities = info.ingredientQuantities[i].Split(",");

            // add the ingredients to the lists
            for (int j = 0; j < ingredientIDs.Length - 1; j++)
            {
                int ingredientID = int.Parse(ingredientIDs[j]);
                float ingredientQuantity = float.Parse(ingredientQuantities[j]);
                ingredients.Add(new Ingredient(ingredientID, ingredientNames[j], (QuantityType) Enum.Parse(typeof(QuantityType), ingredientQuantityTypes[j])), ingredientQuantity);
            }
            lists.Add(new IngredientList(info.listNames[i], ingredients));
        }

        return lists;
    }

    public void SaveFile()
    {
        JSONIngredientInfo info = new JSONIngredientInfo();
        
        info.listNames = new string[ingredientLists.Count];
        info.ingredientIDs = new string[ingredientLists.Count];
        info.ingredientNames = new string[ingredientLists.Count];
        info.ingredientQuantityTypes = new string[ingredientLists.Count];
        info.ingredientQuantities = new string[ingredientLists.Count];

        // convert the lists to strings
        for (int i = 0; i < ingredientLists.Count; i++)
        {
            info.listNames[i] = ingredientLists[i].ListName;

            info.ingredientIDs[i] = "";
            info.ingredientNames[i] = "";
            info.ingredientQuantityTypes[i] = "";
            info.ingredientQuantities[i] = "";

            for (int j = 0; j < ingredientLists[i].NumberOfIngredients(); j++)
            {
                Ingredient ingredient = ingredientLists[i].Ingredients.ElementAt(j).Key;
                info.ingredientIDs[i] += ingredient.ID + ",";
                info.ingredientNames[i] += ingredient.Name + ",";
                info.ingredientQuantityTypes[i] += ingredient.Type.ToString() + ",";
                info.ingredientQuantities[i] += ingredientLists[i].Ingredients[ingredient] + ",";
            }
        }

        string json = JsonUtility.ToJson(info);

        File.WriteAllText(filePath, json);
    }

    public void OpenList(int i)
    {
        listsOverviewScreen.SetActive(false);
        currentListIndex = i;
        ingredientListScreen.SetActive(true);
    }

    public void CloseList()
    {
        ingredientListScreen.SetActive(false);
        listsOverviewScreen.SetActive(true);
        SaveFile();
    }

    public void OpenSearchScreen()
    {
        ingredientListScreen.SetActive(false);
        addIngredientScreen.SetActive(true);
    }

    public void OpenIngredientScreen(int itemIndex) 
    {
        //Debug.Log(" setting current ingredient to " + itemIndex);
        currentIngredient = ingredientLists[currentListIndex].Ingredients.ElementAt(itemIndex).Key;
        ingredientScreen.SetActive(true);
    }

    public void AddIngredient(Ingredient ingredient, float quantity)
    {
        ingredientLists[currentListIndex].AddIngredient(ingredient, quantity);
        SaveFile();
    }

    public void DeleteIngredient(Ingredient ingredient)
    {
        ingredientLists[currentListIndex].RemoveIngredient(ingredient);
        SaveFile();
    }

    public void CreateList()
    {
        // TODO: let user pick list name --> add seperate screen

        // adds four ingredients to the list for testing (to be removed later!)
        Dictionary<Ingredient, float> testList = new()
        {
            { new Ingredient(0, "banana", QuantityType.PCS), 2 },
            { new Ingredient(1, "water", QuantityType.L), 0.5f },
            { new Ingredient(2, "pork", QuantityType.G), 500 },
            { new Ingredient(3, "strawberry", QuantityType.G), 300 }
        };

        ingredientLists.Add(new IngredientList("MyList", testList));
        SaveFile();
    }

    public void DeleteList(int i)
    {
        ingredientLists.Remove(ingredientLists[i]);
        SaveFile();
    }
}

[Serializable]
public class JSONIngredientInfo
{
    public string[] listNames;
    public string[] ingredientIDs;
    public string[] ingredientNames;
    public string[] ingredientQuantityTypes;
    public string[] ingredientQuantities;
}
