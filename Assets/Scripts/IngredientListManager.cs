using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

public class IngredientListManager : MonoBehaviour
{
    List<IngredientList> ingredientLists;
    List<GameObject> listObjects = new List<GameObject>();

    // objects assigned within unity
    public GameObject listObject;
    public GameObject ingredientObject;
    public GameObject backButton;
    public GameObject deleteButton;
    public GameObject addIngredientButton;
    public GameObject addListButton;

    string filePath;

    private void Awake()
    {
        filePath = Application.persistentDataPath + "/ingredientLists";
        //File.Delete(filePath);
        ingredientLists = ReadFile();

        DisplayLists();

        addListButton.transform.GetComponent<Button>().onClick.AddListener(CreateList);
        addListButton.SetActive(true);
    }

    List<IngredientList> ReadFile()
    {
        if (!File.Exists(filePath))
        {
            return new List<IngredientList>();
        }
            
        string json = File.ReadAllText(filePath);

        JSONInfo info = JsonUtility.FromJson<JSONInfo>(json);

        List<IngredientList> lists = new List<IngredientList>();
        
        for (int i = 0; i < info.listNames.Length; i++)
        {
            List<Ingredient> ingredients = new List<Ingredient>();

            string[] ingredientNames = info.ingredientNames[i].Split(",");
            string[] ingredientQuantityTypes = info.ingredientQuantityTypes[i].Split(",");
            string[] ingredientQuantities = info.ingredientQuantities[i].Split(",");

            for (int j = 0; j < ingredientNames.Length - 1; j++)
            {
                float ingredientQuantity = float.Parse(ingredientQuantities[j]);
                ingredients.Add(new Ingredient(ingredientNames[j], (QuantityType) Enum.Parse(typeof(QuantityType), ingredientQuantityTypes[j]), ingredientQuantity));
            }
            lists.Add(new IngredientList(info.listNames[i], ingredientObject, deleteButton, ingredients));
        }

        return lists;
    }

    void SaveFile()
    {
        JSONInfo info = new JSONInfo();
        
        info.listNames = new string[ingredientLists.Count];
        info.ingredientNames = new string[ingredientLists.Count];
        info.ingredientQuantityTypes = new string[ingredientLists.Count];
        info.ingredientQuantities = new string[ingredientLists.Count];
        //info.ingredientNames = new List<string>[ingredientLists.Count];
        //info.ingredientQuantityTypes = new List<string>[ingredientLists.Count];
        //info.ingredientQuantities = new List<float>[ingredientLists.Count];

        for (int i = 0; i < ingredientLists.Count; i++)
        {
            info.listNames[i] = ingredientLists[i].listName;

            info.ingredientNames[i] = "";
            info.ingredientQuantityTypes[i] = "";
            info.ingredientQuantities[i] = "";
            //info.ingredientNames[i] = new List<string>();
            //info.ingredientQuantityTypes[i] = new List<string>();
            //info.ingredientQuantities[i] = new List<float>();

            for (int j = 0; j < ingredientLists[i].NumberOfIngredients(); j++)
            {
                info.ingredientNames[i] += ingredientLists[i].ingredients[j].name + ",";
                info.ingredientQuantityTypes[i] += ingredientLists[i].ingredients[j].type.ToString() + ",";
                info.ingredientQuantities[i] += ingredientLists[i].ingredients[j].quantity + ",";
                /*info.ingredientNames[i].Add(ingredientLists[i].ingredients[j].name);
                info.ingredientQuantityTypes[i].Add(ingredientLists[i].ingredients[j].type.ToString());
                info.ingredientQuantities[i].Add(ingredientLists[i].ingredients[j].quantity);*/
            }
        }

        string json = JsonUtility.ToJson(info);
        //string json = JsonSerializer.Serialize(info);

        File.WriteAllText(filePath, json);
    }

    public void CreateList()
    {
        // TODO: let user pick name

        // adds four ingredients to the lsit for testing (TO BE REMOVED later)
        List<Ingredient> testList = new List<Ingredient>();
        testList.Add(new Ingredient("banana", QuantityType.PCS, 2));
        testList.Add(new Ingredient("water", QuantityType.L, 0.5f));
        testList.Add(new Ingredient("pork", QuantityType.G, 500));
        testList.Add(new Ingredient("strawberry", QuantityType.G, 300));

        ingredientLists.Add(new IngredientList("MyList", ingredientObject, deleteButton, testList));
        DisplayLists();
        SaveFile();
    }

    public void DeleteList(int i)
    {
        ingredientLists.Remove(ingredientLists[i]);
        DisplayLists();
        SaveFile();
    }

    void DisplayLists()
    {
        RemoveListObjects();

        float objectDist = 50; // distance between list objects
        float yPos;

        for (int i = 0; i < ingredientLists.Count; i++)
        {
            // calculate height for list object
            yPos = listObject.transform.localPosition.y - (listObject.GetComponent<RectTransform>().sizeDelta.y + objectDist) * i;

            // create a new list item to display this list
            GameObject newItem = Instantiate(listObject, listObject.transform.parent);
            newItem.GetComponent<RectTransform>().localPosition = new Vector3(listObject.transform.localPosition.x, yPos, 0); // set item position to the correct height
            newItem.SetActive(true);

            // change the text to match the list info
            newItem.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = ingredientLists[i].listName;
            newItem.transform.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text = ingredientLists[i].NumberOfIngredients().ToString();
            int itemIndex = i;
            newItem.transform.GetComponent<Button>().onClick.AddListener(() => { OpenList(itemIndex); });
            listObjects.Add(newItem);

            // create a deleteButton for this list
            GameObject newButton = Instantiate(deleteButton, deleteButton.transform.parent);
            newButton.GetComponent<RectTransform>().localPosition = new Vector3(deleteButton.transform.localPosition.x, yPos, 0); // set item position to the correct height
            newButton.SetActive(true);
            newButton.transform.GetComponent<Button>().onClick.AddListener(() => { DeleteList(itemIndex); });
            listObjects.Add(newButton);
        }
    }

    public void OpenList(int i)
    {
        RemoveListObjects();

        ingredientLists[i].DisplayList();

        // set the backbutton to close this list on click
        backButton.transform.GetComponent<Button>().onClick.AddListener(() => { CloseList(i); });
        backButton.SetActive(true);

        // set the addIngredientButton to add a banana to this list on click
        addIngredientButton.transform.GetComponent<Button>().onClick.AddListener(() => { ingredientLists[i].AddIngredient(new Ingredient("banana", QuantityType.PCS, 2)); });
        addIngredientButton.SetActive(true);
        
        addListButton.SetActive(false);
    }

    void CloseList(int i)
    {
        ingredientLists[i].RemoveIngredientObjects();

        DisplayLists();

        backButton.SetActive(false);
        backButton.transform.GetComponent<Button>().onClick.RemoveAllListeners();

        addIngredientButton.SetActive(false);
        addIngredientButton.transform.GetComponent<Button>().onClick.RemoveAllListeners();

        addListButton.SetActive(true);
        SaveFile();
    }

    void RemoveListObjects()
    {
        foreach (GameObject o in listObjects)
        {
            Destroy(o);
        }
        listObjects = new List<GameObject>();
    }
}

[Serializable]
public class JSONInfo
{
    public string[] listNames;
    public string[] ingredientNames;
    public string[] ingredientQuantityTypes;
    public string[] ingredientQuantities;
    //public List<string>[] ingredientNames;
    //public List<string>[] ingredientQuantityTypes;
    //public List<float>[] ingredientQuantities;
}
