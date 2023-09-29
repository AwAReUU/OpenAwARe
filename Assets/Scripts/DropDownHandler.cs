using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DropdownHandler : MonoBehaviour
{
    private TMP_Dropdown dropdown;

    private void Start()
    {
        dropdown = GetComponent<TMP_Dropdown>();
        //SetDropdownOptions();
        dropdown.onValueChanged.AddListener(delegate
        {
            DropdownValueChanged(dropdown);
        });
    }

    private void SetDropdownOptions()
    {
        dropdown.options.Clear();
        foreach (var obj in ObjectPrefabs.I.prefabs)
        {
            dropdown.options.Add(new TMP_Dropdown.OptionData() { text = obj.name });
        }
    }

    private void DropdownValueChanged(TMP_Dropdown dropdown)
    {
        switch (dropdown.value)
        {
            case 0:     // Go to ingredient/recipe manager
                break;
            case 1:     // Auto-generate objects
                FindObjectOfType<ObjectCreationManager>().AutoGenerateObjects();
                break;
            case 2:     // Clear world of generated objects
                FindObjectOfType<ObjectCreationManager>().DestroyAllObjects();
                break;
            case 3:     // Go to Questionaire / Diary?
                FindObjectOfType<SceneSwitcher>().ChangeScene(1);
                break;
            case 4:     // Go to Settings
                break;
            default:
                break;
        }

    }
}
