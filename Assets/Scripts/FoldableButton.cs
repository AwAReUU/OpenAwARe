using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainButtonNavigation : MonoBehaviour
{
    [SerializeField] private GameObject foldBtn;
    [SerializeField] private GameObject cleanBtn;
    [SerializeField] private GameObject settingsBtn;
    [SerializeField] private GameObject homeBtn;
    [SerializeField] private GameObject generatenewBtn;
    [SerializeField] private GameObject ingredientlistBtn;
    [SerializeField] private GameObject questionnaireBtn;
   
    private int clickCount = 0;

    void Start()
    {
        this.foldBtn.SetActive(true);
        this.cleanBtn.SetActive(false);
        this.settingsBtn.SetActive(false);
        this.homeBtn.SetActive(false);
        this.generatenewBtn.SetActive(false);
        this.ingredientlistBtn.SetActive(false);
        this.questionnaireBtn.SetActive(false);
        
    }

   

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
    public void UnFold()
    {
        this.foldBtn.SetActive(true);
        this.cleanBtn.SetActive(false);
        this.settingsBtn.SetActive(false);
        this.homeBtn.SetActive(false);
        this.generatenewBtn.SetActive(false);
        this.ingredientlistBtn.SetActive(false);
        this.questionnaireBtn.SetActive(false);

    }


    public void Fold()
    {
        this.foldBtn.SetActive(true);
        this.cleanBtn.SetActive(true);
        this.settingsBtn.SetActive(true);
        this.homeBtn.SetActive(true);
        this.generatenewBtn.SetActive(true);
        this.ingredientlistBtn.SetActive(true);
        this.questionnaireBtn.SetActive(true);

    }



  

}
