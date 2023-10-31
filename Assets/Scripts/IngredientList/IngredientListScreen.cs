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

        private void OnEnable()
        {
            backButton.SetActive(true);
            Button backB = backButton.GetComponent<Button>();
            backB.onClick.AddListener(delegate { OnBackButtonClick(); });

            Button saveB = saveButton.GetComponent<Button>();
            saveB.onClick.AddListener(delegate { OnSaveButtonClick(); });

            DisplayList();
        }

        private void OnDisable()
        {
            Button backB = backButton.GetComponent<Button>();
            backB.onClick.RemoveAllListeners();

            Button saveB = saveButton.GetComponent<Button>();
            saveB.onClick.RemoveAllListeners();

            RemoveIngredientObjects();
        }

        private void OnSaveButtonClick()
        {
            ingredientListManager.CloseList();
        }

        private void DisplayList()
        {
            RemoveIngredientObjects();

            // display list name
            listTitle.GetComponent<TMP_InputField>().text = ingredientListManager.SelectedList.ListName;

            Dictionary<Ingredient, (float, QuantityType)> ingredients = ingredientListManager.SelectedList.Ingredients;

            // display each ingredient
            for (int i = 0; i < ingredientListManager.SelectedList.Ingredients.Count; i++)
            {
                // create a new list item to display this ingredient
                GameObject listItem = Instantiate(listItemObject, scrollViewContent);
                listItem.SetActive(true);

                // change the text to match the ingredient info
                Button ingredientButton = listItem.transform.GetChild(0).GetComponent<Button>();
                Button delButton = listItem.transform.GetChild(1).GetComponent<Button>();

                Ingredient ingredient = ingredients.ElementAt(i).Key;
                (float quantity, QuantityType quantityType) = ingredients[ingredient];

                ingredientButton.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = ingredient.Name;
                ingredientButton.transform.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text = quantity.ToString();
                ingredientButton.transform.GetChild(2).GetComponent<TMPro.TextMeshProUGUI>().text = quantityType.ToString();
                ingredientObjects.Add(listItem);

                // store i in an int for pass-by value to the lambda expression
                int itemIndex = i;
                ingredientButton.onClick.AddListener(() => { OnIngredientButtonClick(itemIndex); });

                // create a deleteButton for this ingredient
                delButton.onClick.AddListener(() => { OnDeleteButtonClick(ingredient); });
            }
            // add the "Add ingredient" button to the bottom of the list.
            addIngredientObj = Instantiate(addIngredientButton, scrollViewContent);
            addIngredientObj.SetActive(true);
            Button addIngredientBtn = addIngredientObj.GetComponent<Button>();
            addIngredientBtn.onClick.AddListener(() => { OnAddIngredientButtonClick(); });
        }

        /// <summary>
        /// Destroys all objects in the scrollview
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

        private void OnIngredientButtonClick(int index)
        {
            Ingredient ingredient = ingredientListManager.SelectedList.Ingredients.ElementAt(index).Key;
            ingredientListManager.ChangeToIngredientScreen(ingredient, this.gameObject);
        }

        public void OnDeleteButtonClick(Ingredient ingredient)
        {
            ingredientListManager.DeleteIngredient(ingredient);
            DisplayList();
        }

        private void OnBackButtonClick()
        {
            ingredientListManager.CloseList();
        }

        private void OnAddIngredientButtonClick()
        {
            ingredientListManager.ChangeToSearchScreen();
        }
    }
}