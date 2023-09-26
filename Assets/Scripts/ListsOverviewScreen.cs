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
    [SerializeField] private GameObject deleteButtonObject;
    [SerializeField] private GameObject listObject;

    [SerializeField] private GameObject scrollView;
    [SerializeField] private Transform scrollViewContent;

    [SerializeField] private GameObject listItemObject;
 
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

        Debug.Log(listObject.GetComponent<RectTransform>().sizeDelta);
  
        Debug.Log(listItemObject.GetComponent<RectTransform>().sizeDelta);

        Vector2 listObjectSize = listItemObject.GetComponent<RectTransform>().sizeDelta;
        Vector2 scrollViewSize = scrollView.GetComponent<RectTransform>().sizeDelta;
        Vector2 scrollViewPos = scrollView.GetComponent<RectTransform>().anchoredPosition;
        Debug.Log("scrollviewpos " + scrollViewPos);
        for (int i = 0; i < ingredientListManager.ingredientLists.Count; i++)
        {
            Button ingredientList = GameObject.Find("IngredientList").GetComponent<Button>();
            Button delBtn = GameObject.Find("DeleteButton").GetComponent<Button>();

            Vector3 listItemPosition = new Vector3
            {
                x = scrollViewPos.x,
                y = listObject.transform.localPosition.y - (listObjectSize.y + objectDist) * i,
                z = 0
            };

            // create a new list item to display this list
            GameObject newItem = Instantiate(listItemObject, scrollViewContent);
            newItem.GetComponent<RectTransform>().localPosition = listItemPosition;

            newItem.SetActive(true);

            // change the text to match the list info
            //newItem.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = ingredientListManager.ingredientLists[i].listName;
            //newItem.transform.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text = ingredientListManager.ingredientLists[i].NumberOfIngredients().ToString();
            //int itemIndex = i;
            //newItem.transform.GetComponent<Button>().onClick.AddListener(() => { OnListButtonClick(itemIndex); });
            //listObjects.Add(newItem);

            // create a deleteButton for this list
            Button btn = GameObject.Find("DeleteButton").GetComponent<Button>();
            //GameObject deleteButton = Instantiate(deleteButtonObject, scrollViewContent);
            //Vector3 deleteButtonPosition = new Vector3
            //{
            //    x = listItemPosition.x + (listObjectSize.x / 2),
            //    y = listItemPosition.y,
            //    z = listItemPosition.z
            //};
            //deleteButton.GetComponent<RectTransform>().localPosition = deleteButtonPosition;
            //deleteButton.SetActive(true);
            //deleteButton.transform.GetComponent<Button>().onClick.AddListener(() => { OnDeleteButtonClick(itemIndex); });
            //listObjects.Add(deleteButton);
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
