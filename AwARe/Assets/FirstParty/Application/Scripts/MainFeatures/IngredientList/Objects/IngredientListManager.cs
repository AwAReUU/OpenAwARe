using System;
using System.Collections.Generic;

using AwARe.Database;
using AwARe.Database.Logic;
using AwARe.IngredientList.Logic;

using UnityEngine;

namespace AwARe.IngredientList.Objects
{
    /// <summary>
    /// Manages data shared and transitions between all screens related to <see cref="IngredientList"/> selection, creation and editing.
    /// </summary>
    public class IngredientListManager : MonoBehaviour
    {
        /// <summary>
        /// Gets the list of all <see cref="IngredientList"/>s that have been created by the user.
        /// </summary>
        /// <value>A <see cref="List"/> containing every <see cref="IngredientList"/>.</value>
        public List<Logic.IngredientList> Lists { get; private set; }

        /// <summary>
        /// Gets the current <see cref="IngredientList"/> that is being edited.
        /// </summary>
        /// <value>The currently selected <see cref="IngredientList"/>.</value>
        public Logic.IngredientList SelectedList { get; private set; } = null;

        /// <summary>
        /// Gets the current <see cref="Ingredient"/> that is being edited.
        /// </summary>
        /// <value>The currently selected <see cref="Ingredient"/>.</value>
        public Ingredient SelectedIngredient { get; private set; } = null;

        // objects assigned within unity
        [SerializeField] private GameObject listsOverviewScreen;  // displays the list of ingredient Lists
        [SerializeField] private GameObject ingredientListScreen; // displays the list of ingredients
        [SerializeField] private GameObject searchScreen;         // for searching new ingredients to add to the list
        [SerializeField] private GameObject ingredientScreen;     // for altering the quantity(type) of an ingredient & displays information about the ingredient
        
        /// <summary>
        /// An <see cref="Action"/> that is invoked every time a change is made to <see cref="SelectedList"/>.
        /// </summary>
        public event Action OnIngredientListChanged;

        IIngredientDatabase ingredientDatabase;

        IngredientFileHandler fileHandler;

        private void Awake()
        {
            ingredientDatabase = new MockupIngredientDatabase();
            fileHandler = new IngredientFileHandler(ingredientDatabase);
            Lists = fileHandler.ReadFile();
            if(Lists.Count == 0)
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

            SelectedIngredient = null;
            SelectedList = list;

            ingredientListScreen.SetActive(true);
        }

        /// <summary>
        /// Notifies objects subscribed to this action that the current IngredientList has been changed.
        /// </summary>
        private void NotifyListChanged()
        {
            OnIngredientListChanged?.Invoke();
        }

        /// <summary>
        /// Closes the IngredientListScreen, calls the fileHandler to either save all lists or load the old lists, and opens the ListsOverviewScreen.
        /// </summary>
        /// <param name="save"> Whether to save the changes made to the currently selected IngredientList. </param>
        public void ChangeToListsOverviewScreen(bool save)
        {
            ingredientListScreen.SetActive(false);

            if (save)
                fileHandler.SaveLists(Lists);
            else
                Lists = fileHandler.ReadFile();
            SelectedList = null;

            listsOverviewScreen.SetActive(true);
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
        /// <param name="ingredient"> The ingredient of which the ingredient screen is opened. </param>
        /// <param name="fromScreen"> The screen from which the Ingredient Screen is opened. </param>
        public void ChangeToIngredientScreen(Ingredient ingredient, GameObject fromScreen)
        {
            fromScreen.SetActive(false);

            SelectedIngredient = ingredient;

            ingredientScreen.SetActive(true);
        }

        /// <summary>
        /// Adds the given ingredient to the ingredient list.
        /// </summary>
        /// <param name="ingredient"> The ingredient that is to be added. </param>
        /// /// <param name="quantity"> The quantity of the ingredient that is to be added. </param>
        public void AddIngredient(Ingredient ingredient, float quantity)
        {
            
            SelectedList.AddIngredient(ingredient, quantity);
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
        /// Changes the name of a list into it's newly give name
        /// </summary>
        /// <param name="name"> The name that is to be given to the list. </param>
        public void ChangeListName(string name)
        {
            SelectedList.ChangeName(name);
            NotifyListChanged();
        }

        /// <summary>
        /// Sets the quantity and type of the currently selected ingredient inside the IngredientList to the given quantity and type and saves the ingredient list.
        /// </summary>
        /// <param name="newQuantity"> new quantity of the ingredient. </param>
        /// <param name="newType"> New quantity type of the ingredient. </param>
        public void UpdateIngredient(float newQuantity, QuantityType newType)
        {
            
            SelectedList.UpdateIngredient(SelectedIngredient, newQuantity, newType);
            NotifyListChanged();
        }

        /// <summary>
        /// Adds a new, empty IngredientList to the overview and calls the fileHandler to save all lists.
        /// </summary>
        public void CreateList()
        {
            // TODO: let user pick list name --> add seperate screen

            Lists.Add(new Logic.IngredientList("MyList", new Dictionary<Ingredient, (float, QuantityType)>()));
            fileHandler.SaveLists(Lists);
        }

        /// <summary>
        /// Removes the given list from the overview and calls the fileHandler to save all lists.
        /// </summary>
        /// <param name="list"></param>
        public void DeleteList(Logic.IngredientList list)
        {
            Lists.Remove(list);
            fileHandler.SaveLists(Lists);
        }

        /// <summary>
        /// Shows the popup.
        /// </summary>
        /// <param name="popup"> The PopUp to be shown.</param>
        public void PopUpOn(GameObject popup)
        {
            Debug.Log("PopUpOn() called");
            popup.SetActive(true);
        }

        /// <summary>
        /// Hides the popup.
        /// </summary>
        /// <param name="popup"> The PopUp to be hidden.</param>
        public void PopUpOff(GameObject popup)
        {
           popup.SetActive(false);
        }
    }
}