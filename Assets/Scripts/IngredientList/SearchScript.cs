using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class SearchScript : MonoBehaviour
{
    [SerializeField] private GameObject thisScreen;
    [SerializeField] private IngredientListManager ingredientListManager;

    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private GameObject ContentHolder;
    [SerializeField] private List<GameObject> items = new List<GameObject>();
    [SerializeField] private GameObject SearchBar;

    // Start is called before the first frame update
    void Start()
    {
        InitDummyList();
    }

    private void InitDummyList()
    {
        for (int i = 0; i < 10; i++) 
        {
            AddContentItem("item " + i*135, QuantityType.PCS);
        }
    }

    public void AddContentItem(string itemName, QuantityType qtyType)
    {
        GameObject button = Instantiate(buttonPrefab, ContentHolder.transform);
        button.GetComponentInChildren<TMP_Text>().text = itemName;
        //Get Button object from GameObject to attach event
        items.Add(button);
        Button b = button.GetComponent<Button>();
        b.onClick.AddListener(delegate { OnItemClicked(); });
        button.SetActive(true);
    }

    private void OnItemClicked()
    {
        Ingredient ingredient = ingredientListManager.currentIngredientList.ingredients[0];
        thisScreen.SetActive(false);
        ingredientListManager.OpenIngredientScreen(ingredient);
    }

    /// <summary>
    /// Find all the items that match the current text in the search bar.
    /// TODO: replace with sql query search
    /// </summary>
    public void OnSearchClick() 
    {
        string searchText = SearchBar.GetComponent<TMP_InputField>().text;
        int searchTextLength = searchText.Length;

        for(int i = 0; i < items.Count; i++)
        {
            GameObject elem = items[i];
            string itemText = elem.GetComponentInChildren<TMP_Text>().text;
            if (itemText.Length >= searchTextLength) 
            {
                if (itemText.Contains(searchText))
                {
                    Debug.Log("true");
                    elem.SetActive(true);
                }
                else
                    elem.SetActive(false);
            }
        }
    }

}
