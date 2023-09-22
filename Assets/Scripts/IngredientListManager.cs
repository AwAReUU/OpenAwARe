using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;
//using System.Text.Json;
//using System.Text.Json.Serialization;

public class IngredientListManager : MonoBehaviour
{
    public List<IngredientList> ingredientLists { get; private set; }

    public IngredientList currentIngredientList;

    // objects assigned within unity
    [SerializeField] private GameObject listsOverviewScreen;
    [SerializeField] private GameObject ingredientListScreen;
    [SerializeField] private GameObject addIngredientScreen;
    [SerializeField] private GameObject ingredientInfoScreen;

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

        JSONInfo info = JsonUtility.FromJson<JSONInfo>(json);

        List<IngredientList> lists = new List<IngredientList>();
        
        // reconstruct all lists
        for (int i = 0; i < info.listNames.Length; i++)
        {
            List<Ingredient> ingredients = new List<Ingredient>();

            string[] ingredientNames = info.ingredientNames[i].Split(",");
            string[] ingredientQuantityTypes = info.ingredientQuantityTypes[i].Split(",");
            string[] ingredientQuantities = info.ingredientQuantities[i].Split(",");

            // add the ingredients to the lists
            for (int j = 0; j < ingredientNames.Length - 1; j++)
            {
                float ingredientQuantity = float.Parse(ingredientQuantities[j]);
                ingredients.Add(new Ingredient(ingredientNames[j], (QuantityType) Enum.Parse(typeof(QuantityType), ingredientQuantityTypes[j]), ingredientQuantity));
            }
            lists.Add(new IngredientList(info.listNames[i], ingredients));
        }

        return lists;
    }

    public void SaveFile()
    {
        JSONInfo info = new JSONInfo();
        
        info.listNames = new string[ingredientLists.Count];
        info.ingredientNames = new string[ingredientLists.Count];
        info.ingredientQuantityTypes = new string[ingredientLists.Count];
        info.ingredientQuantities = new string[ingredientLists.Count];

        // convert the lists to strings
        for (int i = 0; i < ingredientLists.Count; i++)
        {
            info.listNames[i] = ingredientLists[i].listName;

            info.ingredientNames[i] = "";
            info.ingredientQuantityTypes[i] = "";
            info.ingredientQuantities[i] = "";

            for (int j = 0; j < ingredientLists[i].NumberOfIngredients(); j++)
            {
                info.ingredientNames[i] += ingredientLists[i].ingredients[j].name + ",";
                info.ingredientQuantityTypes[i] += ingredientLists[i].ingredients[j].type.ToString() + ",";
                info.ingredientQuantities[i] += ingredientLists[i].ingredients[j].quantity + ",";
            }
        }

        string json = JsonUtility.ToJson(info);

        File.WriteAllText(filePath, json);
    }

    public void OpenList(int i)
    {
        listsOverviewScreen.SetActive(false);
        currentIngredientList = ingredientLists[i];
        ingredientListScreen.SetActive(true);
    }

    public void CloseList()
    {
        ingredientListScreen.SetActive(false);
        currentIngredientList = null;
        listsOverviewScreen.SetActive(true);
        SaveFile();
    }

    public void AddIngredient(Ingredient ingredient)
    {
        // TODO: created method inbetween with:
        // ingredientListScreen.SetActive(false);
        // addIngredientScreen.SetActive(true);
        currentIngredientList.AddIngredient(ingredient);
        SaveFile();
    }

    public void DeleteIngredient(int i)
    {
        currentIngredientList.RemoveIngredient(i);
        SaveFile();
    }

    public void CreateList()
    {
        // TODO: let user pick list name --> add seperate screen

        // adds four ingredients to the list for testing (to be removed later!)
        List<Ingredient> testList = new List<Ingredient>();
        testList.Add(new Ingredient("banana", QuantityType.PCS, 2));
        testList.Add(new Ingredient("water", QuantityType.L, 0.5f));
        testList.Add(new Ingredient("pork", QuantityType.G, 500));
        testList.Add(new Ingredient("strawberry", QuantityType.G, 300));

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
public class JSONInfo
{
    public string[] listNames;
    public string[] ingredientNames;
    public string[] ingredientQuantityTypes;
    public string[] ingredientQuantities;
}
