using System.Collections.Generic;

using AwARe.IngredientList.Logic;

using TMPro;

using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace AwARe.IngredientList.Objects
{
    public class IngredientListScreen : MonoBehaviour
    {
        [FormerlySerializedAs("ingredientListManager")][SerializeField] private IngredientListManager manager;

        // the objects drawn on screen to display the list
        List<GameObject> ingredientObjects = new();


        // (assigned within unity)
        [SerializeField] private GameObject deleteListPopup;
        [SerializeField] private GameObject unsavedChangesPopup;
        [SerializeField] private TMP_InputField listTitle;
        [SerializeField] private GameObject listItemTemplate;
        [SerializeField] private GameObject addButton;

        private void OnEnable()
        {
            unsavedChangesPopup.SetActive(false);
            DisplayList();
        }

        private void OnDisable()
        {
            RemoveIngredientObjects();
        }

        /// <summary>
        /// Changes the name of the list to the name that is put into the inputfield in unity
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
        private void DisplayList()
        {
            RemoveIngredientObjects();

            // display list name
            listTitle.text = manager.SelectedList.ListName;

            // display each ingredient
            foreach (var (ingredient, (quantity, quantityType)) in manager.SelectedList.Ingredients)
            {
                // create a new list item to display this ingredient
                GameObject itemObject = Instantiate(listItemTemplate, listItemTemplate.transform.parent);
                itemObject.SetActive(true);
                var item = itemObject.GetComponent<IngredientListItem>();
                item.SetIngredient(ingredient, quantity, quantityType);
                ingredientObjects.Add(itemObject);
            }

            addButton.transform.SetAsLastSibling();
        }

        /// <summary>
        /// Destroys all objects in the scrollview and empties ingredientObjects.
        /// </summary>
        private void RemoveIngredientObjects()
        {
            foreach (GameObject o in ingredientObjects)
                Destroy(o);
            ingredientObjects = new();
        }

        /// <summary>
        /// Calls an instance of IngredientListManager to change to the IngredientScreen of the ingredient that was selected.
        /// </summary>
        /// <param name="ingredient"> The ingredient of which the button is clicked </param>
        public void OnIngredientButtonClick(Ingredient ingredient)
        {
            manager.ChangeToIngredientScreen(ingredient, this.gameObject);
        }
        
        /// <summary>
        /// Calls an instance of IngredientListManager to delete the given ingredient from the ingredient list, then displays the updated list.
        /// </summary>
        /// <param name="ingredient"> The Ingredient that will be deleted from the list </param>
        public void OnDeleteButtonClick()
        {
            deleteListPopup.SetActive(true);
        }

        /// <summary>
        /// Calls an instance of IngredientListManager to delete the given ingredient from the ingredient list, then displays the updated list.
        /// </summary>
        /// <param name="ingredient"> The Ingredient that will be deleted from the list </param>
        public void OnDeleteItemButtonClick(Ingredient ingredient)
        {
            manager.DeleteIngredient(ingredient);
            DisplayList();
        }

        public void DeleteList()
        {
            manager.DeleteList(manager.SelectedList);
            Leave();
        }

        public void Save() =>
            manager.SaveLists();

        public void Discard() =>
            manager.LoadLists();

        /// <summary>
        /// Calls PopUpChoices when a list has been edited to warn the user if they really want to go back or if no editing has happend
        /// an instance of IngredientListManager is called to close the IngredientListScreen and go back to the ListsOverviewScreen.
        /// </summary>
        public void OnBackButtonClick()
        {
            if (manager.ChangesMade)
                unsavedChangesPopup.SetActive(true);
            else
                Leave();
        }

        public void DiscardAndLeave()
        {
            Discard();
            Leave();
        }

        public void SaveAndLeave()
        {
            Save();
            Leave();
        }

        /// <summary>
        /// Calls PopUpChoices when a list has been edited to warn the user if they really want to go back or if no editing has happend
        /// an instance of IngredientListManager is called to close the IngredientListScreen and go back to the ListsOverviewScreen.
        /// </summary>
        public void Leave()
        {
            RemoveIngredientObjects();
            manager.ChangeToListsOverviewScreen();
        }

        /// <summary>
        /// Calls an instance of IngredientListManager to close the IngredientListScreen and open the SearchScreen to add a new ingredient.
        /// </summary>
        public void OnAddIngredientButtonClick() =>
            manager.ChangeToSearchScreen();
    }
}