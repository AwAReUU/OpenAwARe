using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class DevScreenSelector : MonoBehaviour
{
    [SerializeField] private GameObject[] devScreens;

    // enum Devs { Joep, Kizi, Marco, Martijn, Max, Nathan, Nick, Sander, Seana }
    // [SerializeField] private Devs startingDev;

    //  private void Awake()
    //  {
    //     // Debug.Log((int)startingDev);
    //     // SelectDevScreen((int)startingDev);
    //  }

    public void SelectDevScreen(int index)
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
