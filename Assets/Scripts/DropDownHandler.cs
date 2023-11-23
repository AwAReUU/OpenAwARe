using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class DropdownHandler : MonoBehaviour
{
    private TMP_Dropdown dropdown;
    protected abstract ObjectPrefabs prefabs { get; }

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
        foreach (var obj in prefabs.prefabs)
        {
            dropdown.options.Add(new TMP_Dropdown.OptionData() { text = obj.name });
        }
    }

    private void DropdownValueChanged(TMP_Dropdown dropdown)
    {
        prefabs.prefabIndex = dropdown.value;
    }
}

