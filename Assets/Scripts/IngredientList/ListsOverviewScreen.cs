using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ListsOverviewScreen : MonoBehaviour
{
    [SerializeField] private MainManager mainManager;

    // the objects drawn on screen to display the lists
    List<GameObject> listObjects = new List<GameObject>();

    // (assigned within unity)
    [SerializeField] private Transform scrollViewContent;
    [SerializeField] private GameObject listItemObject; //list item 'prefab'
 
    private void OnEnable()
    {
        //Debug.Log("count:  " + ingredientListManager.ingredientLists.Count);
        DisplayLists();
    }

    private void OnDisable()
    {
        RemoveListObjects();
    }

    void DisplayLists()
    {
        RemoveListObjects();

        for (int i = 0; i < mainManager.ingredientLists.Count; i++)
        {
            // create a new list item to display this list
            GameObject listItem = Instantiate(listItemObject, scrollViewContent);
            listItem.SetActive(true);

            // change the text to match the list info
            Button delButton = listItem.transform.GetChild(1).GetComponent<Button>();
            Button listButton = listItem.transform.GetChild(0).GetComponent<Button>();
            listButton.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text =
                mainManager.ingredientLists[i].listName;
            listButton.transform.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text =
                mainManager.ingredientLists[i].NumberOfIngredients().ToString();

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
        mainManager.CreateList();
        DisplayLists();
    }

    public void OnDeleteButtonClick(int i)
    {
        mainManager.DeleteList(i);
        DisplayLists();
    }

    public void OnListButtonClick(int i)
    {
        mainManager.OpenList(i);
    }
}