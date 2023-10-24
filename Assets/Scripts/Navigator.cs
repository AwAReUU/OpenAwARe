using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Navigator : MonoBehaviour
{
    private TMP_Dropdown dropdown;
    private SceneSwitcher sceneSwitcher;

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

    private void DropdownValueChanged(TMP_Dropdown dropdown)
    {
        SceneSwitcher sceneSwitcher = new SceneSwitcher();
        switch (dropdown.value)
        {
            case 0:     // Go to ingredient/recipe manager
                Debug.Log("h");
                sceneSwitcher.ChangeScene("IngredientListUI");
                break;
            case 1:     // Auto-generate objects
                Dictionary<int, int> spawnDict = new Dictionary<int, int>()
                { { 0, 2 }, { 1, 1 }, { 2, 2 }, { 3, 3 }, { 4, 1 } };
                FindObjectOfType<ObjectCreationManager>().AutoGenerateObjects(spawnDict);
                break;
            case 2:     // Clear world of generated objects
                FindObjectOfType<ObjectCreationManager>().DestroyAllObjects();
                break;
            case 3:     // Go to Questionaire / Diary?
                sceneSwitcher.ChangeScene("QuestionnairePage");
                break;
            case 4:     // Go to Settings
                break;
            case 5:     // Go to Home Screen
                sceneSwitcher.ChangeScene("HomeScreen");
                break;
            default:
                break;
        }

    }
}
