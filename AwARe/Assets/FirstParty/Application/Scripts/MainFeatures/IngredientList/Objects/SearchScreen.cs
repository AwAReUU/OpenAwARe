using System.Collections.Generic;

using AwARe.Database;
using AwARe.Database.Logic;
using AwARe.IngredientList.Logic;

using TMPro;

using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace AwARe.IngredientList.Objects
{
    public class SearchScreen : MonoBehaviour
    {
        [FormerlySerializedAs("ingredientListManager")][SerializeField] private IngredientListManager manager;

        [SerializeField] private TMP_InputField searchBar;
        [SerializeField] private GameObject itemTemplate;

        private List<GameObject> results = new();

        /// <summary>
        /// Creates new GameObjects for every ingredient in the search results.
        /// </summary>
        private void DisplayResults()
        {
            RemoveResults();

            // create a new object for every search result
            foreach (Ingredient result in manager.SearchResults)
            {
                GameObject itemObject = Instantiate(itemTemplate, itemTemplate.transform.parent);
                itemObject.SetActive(true);
                results.Add(itemObject);
                var item = itemObject.GetComponent<SearchItem>();
                item.SetItem(result);
            }
        }

        /// <summary>
        /// Destroys all displayed ingredient objects.
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
        /// Finds all ingredients in the database that match with the text entered in the searchbar and displays the results.
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