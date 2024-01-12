// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

using System.Collections.Generic;
using AwARe.IngredientList.Logic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace AwARe.IngredientList.Objects
{
    /// <summary>
    /// <para>
    ///     Handles the UI of the IngredientList screen.
    /// </para>
    /// <para>
    ///     Shows an overview of all <see cref="Ingredient"/>s in an <see cref="IngredientList"/>
    ///     and allows the editing of ListName and selection of Ingredients to edit.
    /// </para>
    /// </summary>
    public class IngredientListScreen : MonoBehaviour
    {
        // The parent element
        [SerializeField] private IngredientListManager manager;

        // the objects drawn on screen to display the list
        
        // UI elements to control/copy
        [SerializeField] private TMP_InputField listTitle;
        [SerializeField] private GameObject ingredientTemplate;
        [SerializeField] private GameObject addButton;
        [SerializeField] private GameObject deleteListPopup;
        [SerializeField] private GameObject unsavedChangesPopup;

        // Tracked UI elements
        readonly List<GameObject> ingredients = new();

        private void OnEnable()
        {
            deleteListPopup.SetActive(false);
            unsavedChangesPopup.SetActive(false);
            DisplayIngredients();
        }

        private void OnDisable()
        {
            RemoveIngredients();
        }

        /// <summary>
        /// Changes the name of the list to the name that is put into the inputfield in unity.
        /// </summary>
        public void OnChangeListName()
        {
            string newName = listTitle.text;
            manager.ChangeListName(newName);
        }
        
        /// <summary>
        /// Creates GameObjects with an "edit" and "delete" button for all the ingredients in this is List and adds them to the ScrollView 
        /// and adds an "Add Ingredient" button to the end of the ScrollView.
        /// </summary>
        private void DisplayIngredients()
        {
            RemoveIngredients();

            // display list name
            listTitle.text = manager.SelectedList.ListName;

            // display each ingredient
            foreach (var (ingredient, (quantity, quantityType)) in manager.SelectedList.Ingredients)
            {
                // create a new list item to display this ingredient
                GameObject itemObject = Instantiate(ingredientTemplate, ingredientTemplate.transform.parent);
                itemObject.SetActive(true);
                var item = itemObject.GetComponent<IngredientListItem>();
                item.SetItem(new(ingredient, quantity, quantityType));
                ingredients.Add(itemObject);
            }

            addButton.transform.SetAsLastSibling();
        }

        /// <summary>
        /// Destroys all objects in the scrollview and empties ingredients.
        /// </summary>
        private void RemoveIngredients()
        {
            foreach (GameObject o in ingredients)
                Destroy(o);
            ingredients.Clear();
        }

        /// <summary>
        /// Change to the IngredientScreen of the item that was selected.
        /// </summary>
        /// <param name="entry"> The ingredient, quantity and quantity type of the item. </param>
        public void OnItemClick(Logic.IngredientList.Entry entry)
        {
            manager.ChangeToIngredientScreen(entry, false, this.gameObject);
        }
        
        /// <summary>
        /// Calls an instance of IngredientListManager to delete the given ingredient from the ingredient list, then displays the updated list.
        /// </summary>
        /// <param name="ingredient"> The Ingredient that will be deleted from the list. </param>
        public void OnDeleteButtonClick()
        {
            deleteListPopup.SetActive(true);
        }

        /// <summary>
        /// Calls an instance of IngredientListManager to delete the given ingredient from the ingredient list, then displays the updated list.
        /// </summary>
        /// <param name="ingredient"> The Ingredient that will be deleted from the list. </param>
        public void OnDeleteItemButtonClick(Ingredient ingredient)
        {
            manager.DeleteIngredient(ingredient);
            DisplayIngredients();
        }

        /// <summary>
        /// Delete the currently viewed list.
        /// </summary>
        public void DeleteList()
        {
            manager.DeleteList(manager.SelectedList);
            Leave();
        }
        
        /// <summary>
        /// Save changes made.
        /// </summary>
        public void Save() =>
            manager.SaveLists();
        
        /// <summary>
        /// Discard changes made.
        /// </summary>
        public void Discard() =>
            manager.LoadLists();

        /// <summary>
        /// Calls PopUpChoices when a list has been edited to warn the user if they really want to go back or if no editing has happened
        /// an instance of IngredientListManager is called to close the IngredientListScreen and go back to the ListsOverviewScreen.
        /// </summary>
        public void OnBackButtonClick()
        {
            if (manager.ChangesMade)
                unsavedChangesPopup.SetActive(true);
            else
                Leave();
        }

        /// <summary>
        /// Discard changes made and leave this screen.
        /// </summary>
        public void DiscardAndLeave()
        {
            Discard();
            Leave();
        }
        
        /// <summary>
        /// Save changes made and leave this screen.
        /// </summary>
        public void SaveAndLeave()
        {
            Save();
            Leave();
        }

        /// <summary>
        /// Clean-up and got back to the lists overview screen.
        /// </summary>
        public void Leave()
        {
            RemoveIngredients();
            manager.ChangeToListsOverviewScreen();
        }

        /// <summary>
        /// Calls an instance of IngredientListManager to close the IngredientListScreen and open the SearchScreen to add a new ingredient.
        /// </summary>
        public void OnAddIngredientButtonClick() =>
            manager.ChangeToSearchScreen();
    }
}