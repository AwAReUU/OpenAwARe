using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Question : MonoBehaviour
{

    //options in subclasses:
    //checkbox (toggle in unity)
    //text input (inputfield)
    //radiobutton ()


    public TextMeshPro text;
    public Image background;
    public Toggle option;
    public TMP_InputField textinput;
    public ToggleGroup MRBEANS;

    //use gameobject with a togglegroup for radiobuttons. maybe make its own class?
    public GameObject radiobuttons;
    


    private bool ifyes = false;
    private Question ifyesquestion;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
