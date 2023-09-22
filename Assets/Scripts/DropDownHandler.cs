using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DropDownHandler : MonoBehaviour
{
    [SerializeField] private List<GameObject> items;
    private TMP_Dropdown dropdown;

    private void Awake() 
    {
        TMP_Dropdown dropdown = GetComponent<TMP_Dropdown>();
        SetDropdownOptions();
    }

    private void SetDropdownOptions()
    {
        dropdown.options.Clear();
        foreach (var item in items)
        {
            dropdown.options.Add(new TMP_Dropdown.OptionData() { text = item.name });
        }
    }
}
