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
        SetDropdownOptions();
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
        ObjectPrefabs.I.prefabIndex = dropdown.value;
    }
}
