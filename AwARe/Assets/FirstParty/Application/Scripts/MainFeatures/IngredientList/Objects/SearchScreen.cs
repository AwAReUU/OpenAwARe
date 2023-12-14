// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

using System.Collections.Generic;

using AwARe.Database;
using AwARe.Database.Logic;
using AwARe.IngredientList.Logic;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

namespace AwARe.IngredientList.Objects
{
    /// <summary>
    /// <para>
    ///     Handles the UI of the ingredient search screen.
    /// </para>
    /// <para>
    ///     Allows the user to enter a search term to look up ingredients to add to a list.
    ///     Displays search results and allows the selection of a result to add to the list.
    /// </para>
    /// </summary>
    public class SearchScreen : MonoBehaviour
    {
        [SerializeField] private IngredientListManager ingredientListManager;

        [SerializeField] private GameObject backButton;
        [SerializeField] private GameObject ingredientButtonPrefab;
        [SerializeField] private GameObject ContentHolder;
        [SerializeField] private List<GameObject> searchResultObjects = new();
        [SerializeField] private GameObject SearchBar;

        private List<Ingredient> searchResults = new();
        readonly private IIngredientDatabase database = new MockupIngredientDatabase();

        private void OnEnable()
        {
            Button backB = backButton.GetComponent<Button>();
            backB.onClick.AddListener(delegate { OnBackButtonClick(); });
        }
        private void OnDisable()
        {
            Button backB = backButton.GetComponent<Button>();
            backB.onClick.RemoveAllListeners();
        }

        /// <summary>
        /// Creates new GameObjects for every ingredient in the search results.
        /// </summary>
        private void DisplayResults()
        {
            RemoveItemObjects();

            // create a new object for every search result
            foreach (Ingredient result in searchResults)
            {
                GameObject button = Instantiate(ingredientButtonPrefab, ContentHolder.transform);
                button.GetComponentInChildren<TMP_Text>().text = result.Name;
                //Get Button object from GameObject to attach event
                searchResultObjects.Add(button);
                Button b = button.GetComponent<Button>();
                b.onClick.AddListener(delegate { OnItemClicked(item: result); });
                button.SetActive(true);
            }
        }

        /// <summary>
        /// Destroys all displayed ingredient objects.
        /// </summary>
        private void RemoveItemObjects()
        {
            foreach (GameObject item in searchResultObjects)
            {
                Destroy(item);
            }

            // make list empty
            searchResultObjects = new List<GameObject>();
        }

        /// <summary>
        /// Calls an instance of ingredientListManager to add the given ingredient to the list and open its IngredientScreen.
        /// </summary>
        /// <param name="item"> The search result that was selected. </param>
        private void OnItemClicked(Ingredient item)
        {
            ingredientListManager.AddIngredient(item, 0);
            ingredientListManager.ChangeToIngredientScreen(item, this.gameObject);
        }

        /// <summary>
        /// Finds all ingredients in the database that match with the text written in the searchbar and displays the results.
        /// </summary>
        public void OnSearchClick()
        {
            string searchText = SearchBar.GetComponent<TMP_InputField>().text;

            searchResults = database.Search(searchText);
            DisplayResults();
        }

        /// <summary>
        /// Calls an instance of ingredientListManager to close this screen and go back to the IngredientListScreen.
        /// </summary>
        public void OnBackButtonClick()
        {
            ingredientListManager.ChangeToIngredientListScreen(ingredientListManager.SelectedList, this.gameObject);
        }
    }
}