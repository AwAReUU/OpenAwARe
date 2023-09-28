using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ListsOverviewScreen : MonoBehaviour
{
    [SerializeField] private IngredientListManager ingredientListManager;

    // the objects drawn on screen to display the lists
    List<GameObject> listObjects = new List<GameObject>();

    // (assigned within unity)
    [SerializeField] private Transform scrollViewContent;
    [SerializeField] private GameObject listItemObject; //list item 'prefab'
 
    private void OnEnable()
    {
        DisplayLists();
    }

    private void OnDisable()
    {
        RemoveListObjects();
    }

    void DisplayLists()
    {
        RemoveListObjects();

        for (int i = 0; i < ingredientListManager.ingredientLists.Count; i++)
        {
            // create a new list item to display this list
            GameObject listItem = Instantiate(listItemObject, scrollViewContent);
            listItem.SetActive(true);

            // change the text to match the list info
            Button delButton = listItem.transform.GetChild(1).GetComponent<Button>();
            Button listButton = listItem.transform.GetChild(0).GetComponent<Button>();
            listButton.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text =
                ingredientListManager.ingredientLists[i].listName;
            listButton.transform.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text =
                ingredientListManager.ingredientLists[i].NumberOfIngredients().ToString();

            listObjects.Add(listItem);

            //store i in an int for pass-by value to the lambda expression.
            //else it will not work
            int itemIndex = i;
            delButton.onClick.AddListener(() => { OnDeleteButtonClick(itemIndex); });
        }
    }

    /// <summary>
    /// removes currently displayed objects
    /// </summary>
    void RemoveListObjects()
    {
        foreach (GameObject o in listObjects)
        {
            Destroy(o);
        }
        listObjects = new List<GameObject>();
    }

    public void OnAddListButtonClick()
    {
        ingredientListManager.CreateList();
        DisplayLists();
    }

    public void OnDeleteButtonClick(int i)
    {
        ingredientListManager.DeleteList(i);
        DisplayLists();
    }

    public void OnListButtonClick(int i)
    {
        ingredientListManager.OpenList(i);
    }
}
