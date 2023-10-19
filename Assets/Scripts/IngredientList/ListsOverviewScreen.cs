using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ListsOverviewScreen : MonoBehaviour
{
    [SerializeField] private GameObject backButton;
    [SerializeField] private IngredientListManager ingredientListManager;

    // the objects drawn on screen to display the lists
    List<GameObject> listObjects = new();

    // (assigned within unity)
    [SerializeField] private Transform scrollViewContent;
    [SerializeField] private GameObject listItemObject; //list item 'prefab'
 
    private void OnEnable()
    {
        Button backB = backButton.GetComponent<Button>();
        backB.onClick.AddListener(delegate { OnBackButtonClick(); });
        backButton.SetActive(true);

        DisplayLists();
    }

    private void OnDisable()
    {
        Button backB = backButton.GetComponent<Button>();
        backB.onClick.RemoveAllListeners();
        RemoveListObjects();
    }

    private void DisplayLists()
    {
        RemoveListObjects();

        for (int i = 0; i < ingredientListManager.IngredientLists.Count; i++)
        {
            // create a new list item to display this list
            GameObject listItem = Instantiate(listItemObject, scrollViewContent);
            listItem.SetActive(true);

            // change the text to match the list info
            Button delButton = listItem.transform.GetChild(1).GetComponent<Button>();
            Button listButton = listItem.transform.GetChild(0).GetComponent<Button>();
            listButton.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text =
                ingredientListManager.IngredientLists[i].ListName;
            listButton.transform.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text =
                ingredientListManager.IngredientLists[i].NumberOfIngredients().ToString();

            listObjects.Add(listItem);

            //store i in an int for pass-by value to the lambda expression.
            //else it will not work
            int itemIndex = i;
            listButton.onClick.AddListener(() => { OnListButtonClick(itemIndex); });
            delButton.onClick.AddListener(() => { OnDeleteButtonClick(itemIndex); });
        }
    }

    /// <summary>
    /// removes currently displayed objects
    /// </summary>
    private void RemoveListObjects()
    {
        foreach (GameObject o in listObjects)
        {
            Destroy(o);
        }
        listObjects = new List<GameObject>();
    }

    private void OnAddListButtonClick()
    {
        ingredientListManager.CreateList();
        DisplayLists();
    }

    private void OnDeleteButtonClick(int i)
    {
        ingredientListManager.DeleteList(i);
        DisplayLists();
    }

    private void OnListButtonClick(int i)
    {
        IngredientList selectedList = ingredientListManager.IngredientLists[i];
        ingredientListManager.OpenList(selectedList, this.gameObject);
    }

    /// <summary>
    /// Go back to previous screen
    /// </summary>
    private void OnBackButtonClick()
    {
        //TODO: Open main menu
    }
}
