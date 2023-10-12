using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using System.Data;

public class SearchScreen : MonoBehaviour
{
    [SerializeField] private GameObject thisScreen;
    [SerializeField] private IngredientListManager ingredientListManager;

    [SerializeField] private GameObject backButton;
    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private GameObject ContentHolder;
    [SerializeField] private List<GameObject> items = new List<GameObject>();
    [SerializeField] private GameObject SearchBar;

    private List<Ingredient> searchResults = new List<Ingredient>();
    private IDatabase database = new DatabaseMockup();
    // Start is called before the first frame update
    void Start()
    {
    }
    private void OnEnable()
    {
        Button backB = backButton.GetComponent<Button>();
        backB.onClick.AddListener(delegate { OnBackButtonClick(); });
    }

    private void RenderList()
    {
        // destroy old items
        foreach (GameObject item in items)
        {
            Destroy(item);
        }
        // make list empty
        items = new List<GameObject>();
        // create a new object for every search result
        foreach (Ingredient result in searchResults) 
        {
            AddContentItem(result.name, result.type);
        }
    }

    /// <summary>
    /// Add an item to the searchbar's scrollview. 
    /// </summary>
    /// <param name="itemName"></param>
    /// <param name="qtyType"></param>
    public void AddContentItem(string itemName, QuantityType qtyType)
    {
        GameObject button = Instantiate(buttonPrefab, ContentHolder.transform);
        button.GetComponentInChildren<TMP_Text>().text = itemName;
        //Get Button object from GameObject to attach event
        items.Add(button);
        Button b = button.GetComponent<Button>();
        b.onClick.AddListener(delegate { OnItemClicked(itemIndex: 0); });
        button.SetActive(true);
    }

    private void OnItemClicked(int itemIndex)
    {
        thisScreen.SetActive(false);
        ingredientListManager.OpenIngredientScreen(itemIndex);
    }

    /// <summary>
    /// Find all the items that match the current text in the search bar.
    /// TODO: replace with sql query search
    /// </summary>
    public void OnSearchClick() 
    {
        string searchText = SearchBar.GetComponent<TMP_InputField>().text;
        int searchTextLength = searchText.Length;

        searchResults = database.Search(searchText);
        RenderList();
    }

    public void OnBackButtonClick()
    {
        thisScreen.SetActive(false);
        ingredientListManager.OpenList(ingredientListManager.currentListIndex);
    }

}
