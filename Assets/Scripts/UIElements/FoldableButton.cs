using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoldableButton : MonoBehaviour
{
    [SerializeField] private GameObject foldBtn;
    [SerializeField] private GameObject foldableBtns;

    private int clickCount = 0;

    // foldable buttons 
    void Start()
    {
  
    }

   
    // the buttons is able to fold in and out by keeping info about the number of clicks
    public void OnFoldableButtonClick()
    {
        clickCount++;

        if (clickCount % 2 == 0)
        {
            // Execute method for even clicks
            UnFold();
        }
        else
        {
            // Execute method for odd clicks
            Fold();
        }
    
    }
    // all buttons that can be shown from the foldable button are shown
    public void UnFold()
    {
        this.foldableBtns.SetActive(true);
    }

    // all buttons that can be shown from the foldable button are not shown
    public void Fold()
    {
        this.foldableBtns.SetActive(false);
    }



  

}
