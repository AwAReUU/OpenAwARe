using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using UnityEngine.InputSystem;
//using System.Text.Json;
//using System.Text.Json.Serialization;

public class IngredientListManager : MonoBehaviour
{
    public List<IngredientList> IngredientLists { get; private set; }

    public IngredientList SelectedList { get; private set; } = null;

    public Ingredient SelectedIngredient { get; private set; } = null;

    // objects assigned within unity
    [SerializeField] private GameObject listsOverviewScreen;  // displays the list of ingredient lists
    [SerializeField] private GameObject ingredientListScreen; // displays the list of ingredients
    [SerializeField] private GameObject searchScreen;         // for searching new ingredients to add to the list
    [SerializeField] private GameObject ingredientScreen;     // for altering the quantity(type) of an ingredient & displays information about the ingredient 
    
    IIngredientDatabase ingredientDatabase;

    IngredientFileHandler fileHandler;

    private void Awake()
    {
        ingredientDatabase = new MockupIngredientDatabase();
        fileHandler = new IngredientFileHandler(ingredientDatabase);
        IngredientLists = fileHandler.ReadFile();

        listsOverviewScreen.SetActive(true);
    }

    public void OpenList(IngredientList list, GameObject fromScreen)
    {
        fromScreen.SetActive(false);

        SelectedIngredient = null;
        SelectedList = list;

        ingredientListScreen.SetActive(true);
    }

    public void CloseList()
    {
        ingredientListScreen.SetActive(false);
        
        fileHandler.SaveLists(IngredientLists);
        SelectedList = null;

        listsOverviewScreen.SetActive(true);
    }

    public void ChangeToSearchScreen()
    {
        ingredientListScreen.SetActive(false);
        searchScreen.SetActive(true);
    }

    public void ChangeToIngredientScreen(Ingredient ingredient, GameObject fromScreen) 
    {
        fromScreen.SetActive(false);

        SelectedIngredient = ingredient;

        ingredientScreen.SetActive(true);
    }

    public void AddIngredient(Ingredient ingredient, float quantity)
    {
        SelectedList.AddIngredient(ingredient, quantity);
        fileHandler.SaveLists(IngredientLists);
    }

    public void DeleteIngredient(Ingredient ingredient)
    {
        SelectedList.RemoveIngredient(ingredient);
        fileHandler.SaveLists(IngredientLists);
    }

    public void CreateList()
    {
        // TODO: let user pick list name --> add seperate screen

        // adds four ingredients to the list for testing (to be removed later!)
        Dictionary<Ingredient, float> testList = new()
        {
            { ingredientDatabase.GetIngredient(0), 2 },
            { ingredientDatabase.GetIngredient(1), 200 },
            { ingredientDatabase.GetIngredient(4), 500 },
            { ingredientDatabase.GetIngredient(5), 300 }
        };

        IngredientLists.Add(new IngredientList("MyList", testList));
        fileHandler.SaveLists(IngredientLists);
    }

    public void DeleteList(int i)
    {
        IngredientLists.Remove(IngredientLists[i]);
        fileHandler.SaveLists(IngredientLists);
    }
}

