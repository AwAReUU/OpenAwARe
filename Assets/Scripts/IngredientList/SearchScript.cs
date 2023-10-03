using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class SearchScript : MonoBehaviour
{
    [SerializeField] private GameObject thisScreen;
    [SerializeField] private MainManager mainManager;

    [SerializeField] private GameObject backButton;
    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private GameObject ContentHolder;
    [SerializeField] private List<GameObject> items = new List<GameObject>();
    [SerializeField] private GameObject SearchBar;

    // Start is called before the first frame update
    void Start()
    {
        InitDummyList();
    }
    private void OnEnable()
    {
        Button backB = backButton.GetComponent<Button>();
        backB.onClick.AddListener(delegate { OnBackButtonClick(); });
    }

    private void InitDummyList()
    {
        for (int i = 0; i < 20; i++) 
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
        b.onClick.AddListener(delegate { OnItemClicked(itemIndex: 0); });
        button.SetActive(true);
    }

    private void OnItemClicked(int itemIndex)
    {
        thisScreen.SetActive(false);
        mainManager.OpenIngredientScreen(itemIndex);
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

    public void OnBackButtonClick()
    {
        thisScreen.SetActive(false);
        mainManager.OpenList(mainManager.currentListIndex);
    }

}
