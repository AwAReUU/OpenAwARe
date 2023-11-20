using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUpScript : MonoBehaviour
{
    [SerializeField] private GameObject popupPanel;
    // Start is called before the first frame update
    void Start()
    {
        this.popupPanel.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PopUpOn()
    {
        this.popupPanel.SetActive(true);
    }

    public void PopUpOff()
    {
        this.popupPanel.SetActive(false);
    }
}
