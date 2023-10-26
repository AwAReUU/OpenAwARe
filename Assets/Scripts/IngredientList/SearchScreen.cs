using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using RSG.Promises;
using Databases;

namespace IngredientLists
{
    public class SearchScreen : MonoBehaviour
    {
        [SerializeField] private IngredientListManager ingredientListManager;

        [SerializeField] private GameObject backButton;
        [SerializeField] private GameObject ingredientButtonPrefab;
        [SerializeField] private GameObject ContentHolder;
        [SerializeField] private List<GameObject> items = new();
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

        private void DisplayResults()
        {
            RemoveItemObjects();

            // create a new object for every search result
            searchResults.Each(result => AddContentItem(result));
        }

        /// <summary>
        /// Destroys all displayed ingredient objects
        /// </summary>
        private void RemoveItemObjects()
        {
            foreach (GameObject item in items)
            {
                Destroy(item);
            }

            // make list empty
            items = new List<GameObject>();
        }

        /// <summary>
        /// Adds an item to the searchbar's scrollview. 
        /// </summary>
        /// <param name="itemName"></param>
        /// <param name="qtyType"></param>
        public void AddContentItem(Ingredient searchResult)
        {
            GameObject button = Instantiate(ingredientButtonPrefab, ContentHolder.transform);
            button.GetComponentInChildren<TMP_Text>().text = searchResult.Name;
            //Get Button object from GameObject to attach event
            items.Add(button);
            Button b = button.GetComponent<Button>();
            b.onClick.AddListener(delegate { OnItemClicked(item: searchResult); });
            button.SetActive(true);
        }

        private void OnItemClicked(Ingredient item)
        {
            ingredientListManager.AddIngredient(item, 0);
            ingredientListManager.ChangeToIngredientScreen(item, this.gameObject);
        }

        /// <summary>
        /// Find all the items that match the current text in the search bar.
        /// </summary>
        public void OnSearchClick()
        {
            string searchText = SearchBar.GetComponent<TMP_InputField>().text;

            searchResults = database.Search(searchText);
            DisplayResults();
        }

        public void OnBackButtonClick()
        {
            ingredientListManager.OpenList(ingredientListManager.SelectedList, this.gameObject);
        }
    }
}