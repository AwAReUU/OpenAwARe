using Databases;
using System.Collections.Generic;
using UnityEngine;
//using System.Text.Json;
//using System.Text.Json.Serialization;

namespace IngredientLists
{
    public class IngredientListManager : MonoBehaviour
    {
        public List<IngredientList> Lists { get; private set; }

        public IngredientList SelectedList { get; private set; } = null;

        public Ingredient SelectedIngredient { get; private set; } = null;

        // objects assigned within unity
        [SerializeField] private GameObject listsOverviewScreen;  // displays the list of ingredient Lists
        [SerializeField] private GameObject ingredientListScreen; // displays the list of ingredients
        [SerializeField] private GameObject searchScreen;         // for searching new ingredients to add to the list
        [SerializeField] private GameObject ingredientScreen;     // for altering the quantity(type) of an ingredient & displays information about the ingredient 

        IIngredientDatabase ingredientDatabase;

        IngredientFileHandler fileHandler;

        private void Awake()
        {
            ingredientDatabase = new MockupIngredientDatabase();
            fileHandler = new IngredientFileHandler(ingredientDatabase);
            Lists = fileHandler.ReadFile();

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

            fileHandler.SaveLists(Lists);
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
            fileHandler.SaveLists(Lists);
        }

        public void DeleteIngredient(Ingredient ingredient)
        {
            SelectedList.RemoveIngredient(ingredient);
            fileHandler.SaveLists(Lists);
        }

        public void CreateList()
        {
            // TODO: let user pick list name --> add seperate screen

            // adds four ingredients to the list for testing (to be removed later!)
            Dictionary<Ingredient, (float, QuantityType)> testList = new()
        {
            { ingredientDatabase.GetIngredient(1), (2, QuantityType.PCS) },
            { ingredientDatabase.GetIngredient(2), (200, QuantityType.G) },
            { ingredientDatabase.GetIngredient(4), (500, QuantityType.G) },
            { ingredientDatabase.GetIngredient(5), (300, QuantityType.G) }
        };

            Lists.Add(new IngredientList("MyList", testList));
            fileHandler.SaveLists(Lists);
        }

        public void DeleteList(IngredientList list)
        {
            Lists.Remove(list);
            fileHandler.SaveLists(Lists);
        }
    }
}