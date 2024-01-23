// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

using System;
using System.Collections.Generic;
using AwARe.Database;
using AwARe.Database.Logic;
using AwARe.IngredientList.Logic;
using AwARe.InterScenes.Objects;
using UnityEngine;
using static AwARe.IngredientList.Logic.IngredientList;

namespace AwARe.IngredientList.Objects
{
    /// <summary>
    /// Manages data shared and transitions between all screens related to <see cref="IngredientList"/> selection, creation and editing.
    /// </summary>
    public class IngredientListManager : MonoBehaviour
    {
        // objects assigned within unity
        [SerializeField] private GameObject listsOverviewScreen;  // displays the list of ingredient Lists
        [SerializeField] private GameObject ingredientListScreen; // displays the list of ingredients
        [SerializeField] private GameObject searchScreen;         // for searching new ingredients to add to the list
        [SerializeField] private GameObject ingredientScreen;     // for altering the quantity(type) of an ingredient & displays information about the ingredient

        IngredientFileHandler fileHandler;

        public List<Logic.IngredientList> Lists { get; private set; }

        public int IndexList { get; private set; } = -1;
        //public Logic.IngredientList CheckedList { get; private set; } = null;

        public Logic.IngredientList SelectedList { get; private set; } = null;

        public Entry SelectedEntry { get; private set; } = new (null, 0, QuantityType.G);
        public bool SelectedIsNew { get; private set; } = true;

        public bool ChangesMade { get; private set; }

        public List<Ingredient> SearchResults { get; private set; } = new();

        public IIngredientDatabase IngredientDatabase { get; private set; }

        public event Action OnIngredientListChanged;

        private void Awake()
        {
            IngredientDatabase = new MockupIngredientDatabase();
            fileHandler = new IngredientFileHandler(IngredientDatabase);
            Lists = fileHandler.ReadFile();
            InitializeLists();

            listsOverviewScreen.SetActive(true);
        }

        /// <summary>
        /// If no list exists yet, create a default one. Set the first list as the selected list.
        /// </summary>
        private void InitializeLists()
        {
            // Set AVG default list.
            if (Lists.Count == 0)
            {
                Dictionary<Ingredient, (float, QuantityType)> exampleRecipe = new()
                {
                    { new Ingredient(12, "Steak"), (150, QuantityType.G) },
                    { new Ingredient(17, "Potato"), (250, QuantityType.G) },
                    { new Ingredient(18, "Beet"), (300, QuantityType.G) }
                };

                Lists.Add(new Logic.IngredientList("Steak+", exampleRecipe));
                fileHandler.SaveLists(Lists);
            }

            // Set default checked list.
            if (Lists.Count > 0)
            {
                IndexList = 0;
                var list = Lists[0];
                //CheckedList = list;
                SelectedList = list;
                Storage.Get().ActiveIngredientList = list;
            }
        }

        /// <summary>
        /// Notifies objects subscribed to this action that the current IngredientList has been changed.
        /// </summary>
        private void NotifyListChanged()
        {
            ChangesMade = true;
            OnIngredientListChanged?.Invoke();
        }

        /// <summary>
        /// Closes the IngredientListScreen, calls the fileHandler to either save all lists or load the old lists, and opens the ListsOverviewScreen.
        /// </summary>
        public void ChangeToListsOverviewScreen()
        {
            ingredientListScreen.SetActive(false);
            SelectedList = null;
            listsOverviewScreen.SetActive(true);
        }

        /// <summary>
        /// Closes the previous screen, selects the given ingredient list and opens the IngredientListScreen.
        /// </summary>
        /// <param name="list"> The ingredientList of which the IngredientListScreen is opened. </param>
        /// <param name="fromScreen"> The screen from which the IngredientListScreen is opened. </param>
        public void ChangeToIngredientListScreen(Logic.IngredientList list, GameObject fromScreen)
        {
            fromScreen.SetActive(false);

            SelectedEntry = null;
            SelectedList = list;
            ingredientListScreen.SetActive(true);
        }

        /// <summary>
        /// Closes the IngredientListScreen and opens the SearchScreen.
        /// </summary>
        public void ChangeToSearchScreen()
        {
            ingredientListScreen.SetActive(false);
            searchScreen.SetActive(true);
        }

        /// <summary>
        /// Closes the previous screen, selects the given ingredient and opens its respective IngredientScreen.
        /// </summary>
        /// <param name="entry">The selected entree of the ingredient list.</param>
        /// <param name="isNew">True if the entree is not added to the selected ingredient list yet.</param>
        /// <param name="fromScreen"> The screen from which the Ingredient Screen is opened. </param>
        public void ChangeToIngredientScreen(Entry entry, bool isNew, GameObject fromScreen)
        {
            fromScreen.SetActive(false);

            SelectedEntry = entry;
            SelectedIsNew = isNew;

            ingredientScreen.SetActive(true);
        }

        /// <summary>
        /// Save all lists.
        /// </summary>
        public void SaveLists()
        {
            fileHandler.SaveLists(Lists);
            ChangesMade = false;
        }

        /// <summary>
        /// Save all lists.
        /// </summary>
        public void LoadLists()
        {
            Lists = fileHandler.ReadFile();
            ChangesMade = false;
        }

        /// <summary>
        /// Update which list is currentely checked.
        /// </summary>
        /// <param name="index">The newly selected index.</param>
        /// <param name="list">The newly selected list.</param>
        public void CheckList(int index, Logic.IngredientList list)
        {
            IndexList = index;
            //CheckedList = list;
            Storage.Get().ActiveIngredientList = list;
        }

        /// <summary>
        /// Adds a new, empty IngredientList to the overview and calls the fileHandler to save all lists.
        /// </summary>
        public void CreateList()
        {
            // TODO: let user pick list name --> add separate screen

            Lists.Add(new Logic.IngredientList("MyList", new Dictionary<Ingredient, (float, QuantityType)>()));
            fileHandler.SaveLists(Lists);
        }

        /// <summary>
        /// Removes the given list from the overview and calls the fileHandler to save all lists.
        /// </summary>
        /// <param name="list">The list to remove.</param>
        public void DeleteList(Logic.IngredientList list)
        {
            Lists.Remove(list);
            fileHandler.SaveLists(Lists);
        }

        /// <summary>
        /// Adds the given ingredient to the ingredient list.
        /// </summary>
        /// <param name="ingredient"> The ingredient that is to be added. </param>
        /// <param name="quantity"> The amount/quantity to add. </param>
        /// <param name="type"> The type of the quantity. </param>
        public void AddIngredient(Ingredient ingredient, float quantity, QuantityType type)
        {
            SelectedList.AddIngredient(ingredient, quantity, type);
            NotifyListChanged();
        }

        /// <summary>
        /// Changes the name of a list into it's newly give name.
        /// </summary>
        /// <param name="name"> The name that is to be given to the list. </param>
        public void ChangeListName(string name)
        {
            SelectedList.ChangeName(name);
            NotifyListChanged();
        }

        /// <summary>
        /// Removes the given ingredient from the ingredient list.
        /// </summary>
        /// <param name="ingredient"> The ingredient that is to be deleted. </param>
        public void DeleteIngredient(Ingredient ingredient)
        {
            SelectedList.RemoveIngredient(ingredient);
            NotifyListChanged();
        }

        /// <summary>
        /// Sets the quantity and type of the currently selected ingredient inside the IngredientList to the given quantity and type and saves the ingredient list.
        /// </summary>
        /// <param name="newQuantity"> new quantity of the ingredient. </param>
        /// <param name="newType"> New quantity type of the ingredient. </param>
        public void UpdateIngredient(float newQuantity, QuantityType newType)
        {
            SelectedList.UpdateIngredient(SelectedEntry.ingredient, newQuantity, newType);
            NotifyListChanged();
        }

        /// <summary>
        /// Reads/Parses the quantity and quantity type.
        /// </summary>
        /// <param name="ingredient">The ingredient which quantities to read.</param>
        /// <param name="quantityS">The quantity as a string.</param>
        /// <param name="typeS">The quantity type as a string.</param>
        /// <param name="quantity">The quantity as a number.</param>
        /// <param name="type">The quantity type as a type/enum.</param>
        /// <exception cref="Exception">Throw exception when parsing or result type is invalid.</exception>
        public void ReadQuantity(Ingredient ingredient, string quantityS, string typeS, out float quantity, out QuantityType type)
        {
            // try converting the quantity string to a Quantity
            if (!float.TryParse(quantityS, out quantity))
            {
                throw new Exception("Cannot convert string to Quantity.");
            }

            // try converting the quantity type string to a QuantityType
            if (!Enum.TryParse(typeS, out type))
            {
                throw new Exception("Cannot convert string to QuantityType.");
            }

            // check whether the quantity type is valid for this ingredient
            if (!ingredient.QuantityPossible(type))
            {
                throw new Exception("QuantityType " + type.ToString() + " not available for this ingredient.");
            }
        }

        /// <summary>
        /// Finds all ingredients in the database that match with the text entered in the search bar and displays the results.
        /// </summary>
        /// <param name="term">The search term.</param>
        /// <returns>The search results.</returns>
        public List<Ingredient> SearchIngredient(string term) =>
            SearchResults = IngredientDatabase.Search(term);
    }
}