using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoldableButton : MonoBehaviour
{
    [SerializeField] private GameObject foldBtn;
    [SerializeField] private GameObject foldableBtns;

    // the buttons is able to fold in and out by keeping info about the number of clicks
    public void OnFoldableButtonClick()
    {
        foldableBtns.SetActive(!foldableBtns.activeInHierarchy);
    
    }
}
