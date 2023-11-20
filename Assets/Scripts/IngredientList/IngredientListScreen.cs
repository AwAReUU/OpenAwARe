using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace IngredientLists
{
    public class IngredientListScreen : MonoBehaviour
    {
        [SerializeField] private IngredientListManager ingredientListManager;

        // the objects drawn on screen to display the list
        List<GameObject> ingredientObjects = new();
        GameObject addIngredientObj;

        // (assigned within unity)
        [SerializeField] private GameObject backButton;
        [SerializeField] private GameObject listItemObject;
        [SerializeField] private GameObject listTitle;
        [SerializeField] private GameObject addIngredientButton;
        [SerializeField] private GameObject saveButton;
        [SerializeField] private Transform scrollViewContent;
        [SerializeField] private GameObject loadListButton;
 
        private void OnEnable()
        {
            backButton.SetActive(true);
            Button backB = backButton.GetComponent<Button>();
            backB.onClick.AddListener(delegate { OnBackButtonClick(); });

            Button saveB = saveButton.GetComponent<Button>();
            saveB.onClick.AddListener(delegate { OnSaveButtonClick(); });

            Button loadB = loadListButton.GetComponent<Button>();
            loadB.onClick.AddListener(delegate { OnLoadListButtonClick(); });

            DisplayList();
        }

        private void OnDisable()
        {
            Button backB = backButton.GetComponent<Button>();
            backB.onClick.RemoveAllListeners();

            Button saveB = saveButton.GetComponent<Button>();
            saveB.onClick.RemoveAllListeners();

            Button loadB = loadListButton.GetComponent<Button>();
            loadB.onClick.RemoveAllListeners();

            RemoveIngredientObjects();
        }

        /// <summary>
        /// Just go back to the home screen, without removing the selected list.
        /// </summary>
        private void OnLoadListButtonClick()
        {
            //ingredientListManager.CloseListScreen();
            CreateSelectedListGameObject();
            backButton.SetActive(false);
        }
        private void CreateSelectedListGameObject() 
        {
            GameObject selectedListObject = new();
            selectedListObject.name = "SelectedListGameObject";
            selectedListObject.AddComponent<IngrListGameObject>();
            IngrListGameObject ingrListGameObject = new IngrListGameObject();
            ingrListGameObject.selectedList = ingredientListManager.SelectedList;
            DontDestroyOnLoad(selectedListObject);
        }
        
        /// <summary>
        /// Creates GameObjects with an "edit" and "delete" button for all the ingredients in this is List and adds them to the ScrollView 
        /// and adds an "Add Ingredient" button to the end of the ScrollView.
        /// </summary>
        private void DisplayList()
        {
            RemoveIngredientObjects();

            // display list name
            listTitle.GetComponent<TMP_InputField>().text = ingredientListManager.SelectedList.ListName;

            // display each ingredient
            foreach (var (ingredient, (quantity, quantityType)) in ingredientListManager.SelectedList.Ingredients)
            {
                // create a new list item to display this ingredient
                GameObject listItem = Instantiate(listItemObject, scrollViewContent);
                listItem.SetActive(true);

                // change the text to match the ingredient info
                Button ingredientButton = listItem.transform.GetChild(0).GetComponent<Button>();
                Button delButton = listItem.transform.GetChild(1).GetComponent<Button>();

                ingredientButton.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = ingredient.Name;
                ingredientButton.transform.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text = quantity.ToString();
                ingredientButton.transform.GetChild(2).GetComponent<TMPro.TextMeshProUGUI>().text = quantityType.ToString();
                ingredientObjects.Add(listItem);

                // add listener to the ingredient button
                ingredientButton.onClick.AddListener(() => { OnIngredientButtonClick(ingredient); });

                // add listener to this ingredient's delete button
                delButton.onClick.AddListener(() => { OnDeleteButtonClick(ingredient); });
            }

            // add the "Add ingredient" button to the bottom of the list.
            addIngredientObj = Instantiate(addIngredientButton, scrollViewContent);
            addIngredientObj.SetActive(true);
            Button addIngredientBtn = addIngredientObj.GetComponent<Button>();
            addIngredientBtn.onClick.AddListener(() => { OnAddIngredientButtonClick(); });
        }

        /// <summary>
        /// Destroys all objects in the scrollview and empties ingredientObjects.
        /// </summary>
        private void RemoveIngredientObjects()
        {
            foreach (GameObject o in ingredientObjects)
            {
                Destroy(o);
            }
            Destroy(addIngredientObj);
            ingredientObjects = new List<GameObject>();
        }

        /// <summary>
        /// Calls an instance of IngredientListManager to change to the IngredientScreen of the ingredient that was selected.
        /// </summary>
        /// <param name="ingredient"> The ingredient of which the button is clicked </param>
        private void OnIngredientButtonClick(Ingredient ingredient)
        {
            ingredientListManager.ChangeToIngredientScreen(ingredient, this.gameObject);
        }

        /// <summary>
        /// Calls an instance of IngredientListManager to delete the given ingredient from the ingredient list, then displays the updated list.
        /// </summary>
        /// <param name="ingredient"> The Ingredient that will be deleted from the list </param>
        public void OnDeleteButtonClick(Ingredient ingredient)
        {
            ingredientListManager.DeleteIngredient(ingredient);
            DisplayList();
        }


        /// <summary>
        /// Calls an instance of IngredientListManager to close the IngredientListScreen and go back to the ListsOverviewScreen.
        /// </summary>
        private void OnSaveButtonClick()
        {
            ingredientListManager.ChangeToListsOverviewScreen(true);
        }

        /// <summary>
        /// Calls an instance of IngredientListManager to close the IngredientListScreen and go back to the ListsOverviewScreen.
        /// </summary>
        private void OnBackButtonClick()
        {
            ingredientListManager.ChangeToListsOverviewScreen(false);
        }

        /// <summary>
        /// Calls an instance of IngredientListManager to close the IngredientListScreen and open the SearchScreen to add a new ingredient.
        /// </summary>
        private void OnAddIngredientButtonClick()
        {
            ingredientListManager.ChangeToSearchScreen();
        }
    }
}