using IngredientLists;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Navigator : MonoBehaviour
{
    //! INSANELY DIRTY VERSION FOR DEMO THIS CODE SUX BALLS
    private TMP_Dropdown dropdown;
    private SceneSwitcher sceneSwitcher;
    public GameObject ingredientListUI;
    public GameObject objectGeneration;
    public GameObject questionnaire;
    public GameObject roomScan;

    private void Start()
    {
        dropdown = GetComponent<TMP_Dropdown>();
        //sceneSwitcher = FindObjectOfType<SceneSwitcher>();
        //SetDropdownOptions();
        dropdown.onValueChanged.AddListener(delegate
        {
            DropdownValueChanged(dropdown);
        });

        dropdown.captionText.alpha = 0;
    }

    private void SetDropdownOptions()
    {
        dropdown.options.Clear();
        foreach (var obj in ObjectPrefabsMainUI.I.prefabs)
        {
            dropdown.options.Add(new TMP_Dropdown.OptionData() { text = obj.name });
        }
    }

    //! DIRTY MODE VOOR DEMO
    private void DropdownValueChanged(TMP_Dropdown dropdown)
    {
        SceneSwitcher sceneSwitcher = new SceneSwitcher();
        switch (dropdown.value)
        {
            case 0:     // Go to ingredient/recipe manager
                dummyChangeScene(ingredientListUI);
                break;
            case 1:     // Auto-generate objects
                //Dictionary<int, int> spawnDict = new Dictionary<int, int>()
                //{ { 0, 2 }, { 1, 1 }, { 2, 2 }, { 3, 3 }, { 4, 1 } };
                //FindObjectOfType<ObjectCreationManager>().AutoGenerateObjects(spawnDict);

                IngredientList selectedList = GetSelectedList();
                SetSelectedListObjectGen(selectedList);
                
                dummyChangeScene(objectGeneration);
                break;
            case 2:     // Clear world of generated objects
                GameObject[] generatedObjects = GameObject.FindGameObjectsWithTag("Animal");
                foreach (GameObject target in generatedObjects)
                    Destroy(target);
                break;
            case 3:     // Go to Questionaire / Diary?
                dummyChangeScene(questionnaire);
                break;
            case 4:     // Go to Settings //*Room scan
                dummyChangeScene(roomScan);
                break;
            case 5:     // Go to Home Screen
                //sceneSwitcher.ChangeScene("HomeScreen");
                break;
            default:
                break;
        }
    }

    public void dummyChangeScene(GameObject targetDummyScene)
    {
        ingredientListUI.SetActive(false);
        objectGeneration.SetActive(false);
        questionnaire.SetActive(false);
        roomScan.SetActive(false);

        targetDummyScene.SetActive(true);
    }

    /// <summary>
    /// Extracts the currently selected list from the ingredientlist scene.
    /// </summary>
    /// <returns></returns>
    private IngredientList GetSelectedList() 
    {
        GameObject ingredientListManager = ingredientListUI.GetNamedChild("IngredientListManager");
        IngredientListManager component = ingredientListManager.GetComponent<IngredientListManager>();
        return component.SelectedList;
    }

    /// <summary>
    /// Pass the selectedList to the ObjectGeneration scene.
    /// </summary>
    /// <param name="selectedList">IngredientList to be rendered in object gen</param>
    private void SetSelectedListObjectGen(IngredientList selectedList) 
    {
        GameObject objectCreationManager = objectGeneration.GetNamedChild("ObjectCreationManager");
        ObjectCreationManager component = objectCreationManager.GetComponent<ObjectCreationManager>();
        component.SetSelectedList(selectedList);
    }


    // private void DropdownValueChanged(TMP_Dropdown dropdown)
    // {
    //     SceneSwitcher sceneSwitcher = new SceneSwitcher();
    //     switch (dropdown.value)
    //     {
    //         case 0:     // Go to ingredient/recipe manager
    //             Debug.Log("h");
    //             sceneSwitcher.ChangeScene("IngredientListUI");
    //             break;
    //         case 1:     // Auto-generate objects
    //             //Dictionary<int, int> spawnDict = new Dictionary<int, int>()
    //             //{ { 0, 2 }, { 1, 1 }, { 2, 2 }, { 3, 3 }, { 4, 1 } };
    //             //FindObjectOfType<ObjectCreationManager>().AutoGenerateObjects(spawnDict);
    //             sceneSwitcher.ChangeScene("ObjectGeneration");
    //             break;
    //         case 2:     // Clear world of generated objects
    //             FindObjectOfType<ObjectCreationManager>().DestroyAllObjects();
    //             break;
    //         case 3:     // Go to Questionaire / Diary?
    //             sceneSwitcher.ChangeScene("QuestionnairePage");
    //             break;
    //         case 4:     // Go to Settings
    //             break;
    //         case 5:     // Go to Home Screen
    //             sceneSwitcher.ChangeScene("HomeScreen");
    //             break;
    //         default:
    //             break;
    //     }

    // }
}
