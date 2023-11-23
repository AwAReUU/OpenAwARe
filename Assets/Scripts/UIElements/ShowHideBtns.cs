using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowHideBtns : MonoBehaviour
{
    [SerializeField] private GameObject popupPanel;
    [SerializeField] private GameObject unFoldBtn;
    [SerializeField] private GameObject unFoldBtns;

    // Start is called before the first frame update

    // this is meant for a manager empty object that sets the gameobjects on active or inactive at the start 
    // and has a method for showing the buttons 
    void Start()
    {
        this.popupPanel.SetActive(false);
        this.unFoldBtn.SetActive(false);
        this.unFoldBtns.SetActive(false);
    }

    public void ShowBtns()
    {
        this.unFoldBtn.SetActive(true);
        this.unFoldBtns.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
