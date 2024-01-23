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
        // The parent element
        [SerializeField] private IngredientListManager manager; // TODO: Replace with parent UI element. (For division of responsibilities)

        // UI elements to control
        [SerializeField] private TMP_InputField searchBar;
        [SerializeField] private GameObject resultTemplate;

        // Tracked UI elements
        private readonly List<GameObject> results = new();

        /// <summary>
        /// Corrects this UI element to represent data.
        /// </summary>
        private void DisplayResults()
        {
            // Empties the old results display.
            RemoveResults();

            // Create a new object for every search result
            foreach (Ingredient result in manager.SearchResults)
            {
                GameObject itemObject = Instantiate(resultTemplate, resultTemplate.transform.parent);
                itemObject.SetActive(true);
                results.Add(itemObject);
                var item = itemObject.GetComponent<SearchItem>();
                item.SetItem(result);
            }
        }

        /// <summary>
        /// Destroys all displayed result objects.
        /// </summary>
        private void RemoveResults()
        {
            foreach (GameObject item in results)
                Destroy(item);
            results.Clear();
        }

        /// <summary>
        /// Calls an instance of manager to add the given ingredient to the ingredientlist and open its IngredientScreen.
        /// </summary>
        /// <param name="ingredient"> The search result that was selected. </param>
        public void OnItemClick(Ingredient ingredient)
        {
            manager.ChangeToIngredientScreen(new(ingredient, 0, QuantityType.G), true, this.gameObject);
        }

        /// <summary>
        /// Finds all ingredients in the database that match with the text written in the searchbar and displays the results.
        /// </summary>
        public void OnSearchClick()
        {
            manager.SearchIngredient(searchBar.text);
            DisplayResults();
        }

        /// <summary>
        /// Calls an instance of manager to close this screen and go back to the IngredientListScreen.
        /// </summary>
        public void OnBackButtonClick()
        {
            manager.ChangeToIngredientListScreen(manager.SelectedList, this.gameObject);
        }
    }
}