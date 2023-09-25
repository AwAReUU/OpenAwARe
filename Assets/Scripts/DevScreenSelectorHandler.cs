using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;


public class DevScreenSelectorHandler : MonoBehaviour
{
    [SerializeField] private GameObject[] devScreens;

    public void SelectActiveDevScreen(int index)
    {
        for (int i = 0; i < devScreens.Length; i++)
        {
            if (i == index)
            {
                devScreens[i].SetActive(true);
            }
            else
                devScreens[i].SetActive(false);
        }
    }
}
