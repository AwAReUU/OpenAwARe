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
    [SerializeField] private GameObject addListButton;
    [SerializeField] private GameObject deleteButton;
    [SerializeField] private GameObject listObject;

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

        float objectDist = 50; // distance between list objects
        float yPos;

        for (int i = 0; i < ingredientListManager.ingredientLists.Count; i++)
        {
            // calculate height for list object
            yPos = listObject.transform.localPosition.y - (listObject.GetComponent<RectTransform>().sizeDelta.y + objectDist) * i;

            // create a new list item to display this list
            GameObject newItem = Instantiate(listObject, listObject.transform.parent);
            newItem.GetComponent<RectTransform>().localPosition = new Vector3(listObject.transform.localPosition.x, yPos, 0); // set item position to the correct height
            newItem.SetActive(true);

            // change the text to match the list info
            newItem.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = ingredientListManager.ingredientLists[i].listName;
            newItem.transform.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text = ingredientListManager.ingredientLists[i].NumberOfIngredients().ToString();
            int itemIndex = i;
            newItem.transform.GetComponent<Button>().onClick.AddListener(() => { OnListButtonClick(itemIndex); });
            listObjects.Add(newItem);

            // create a deleteButton for this list
            GameObject newButton = Instantiate(deleteButton, deleteButton.transform.parent);
            newButton.GetComponent<RectTransform>().localPosition = new Vector3(deleteButton.transform.localPosition.x, yPos, 0); // set item position to the correct height
            newButton.SetActive(true);
            newButton.transform.GetComponent<Button>().onClick.AddListener(() => { OnDeleteButtonClick(itemIndex); });
            listObjects.Add(newButton);
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
