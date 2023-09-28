using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.UI;

public class Question : MonoBehaviour
{
    private ToggleGroup RadiobuttonGroup;

    //'Template' objects are instantiated when a new answer option is created for the question
    [SerializeField]
    private GameObject textinputTemplate;
    [SerializeField]
    private GameObject checkboxTemplate;
    [SerializeField]
    private GameObject radiobuttonTemplate;

    //The title of the Question object holds the actual question itself, e.g., 'how was your day?'
    [SerializeField]
    private GameObject title;

    //index of the answer option that will activate the 'if yes' questions
    public int ifyesTrigger { get; private set; }
    public bool ifyes { get; private set; }
    public List<Question> ifyesQuestions;

    List<GameObject> answerOptions;
    Questionnaire parentQuestionnaire;

    void Awake()
    {
        answerOptions = new List<GameObject>();
        gameObject.AddComponent<ToggleGroup>();
        RadiobuttonGroup = gameObject.GetComponent<ToggleGroup>();
        RadiobuttonGroup.allowSwitchOff = true;
    }

    public void SetTitle(string questionTitle)
    {
        title.GetComponent<TextMeshProUGUI>().text = questionTitle;
    }

    public void SetIfyes(bool ifyes, int ifyesTrigger)
    {
        this.ifyes = ifyes;
        this.ifyesTrigger = ifyesTrigger;
    }

    public void SetParentQuestionnaire(Questionnaire parentQuestionnaire)
    {
        this.parentQuestionnaire = parentQuestionnaire;
    }

    //If the hierarchy of the Template objects changes, the below 'add' methods may also need to change
    public void AddTextinput(string placeholdertext)
    {
        var inputfield = Instantiate(textinputTemplate);
        inputfield.SetActive(true);
        inputfield.transform.SetParent(gameObject.transform);
        answerOptions.Add(inputfield);

        inputfield.transform.Find("Text Area/Placeholder").GetComponent<TextMeshProUGUI>().text = placeholdertext;
    }

    public void AddCheckbox(string labeltext)
    {
        var checkbox = Instantiate(checkboxTemplate);
        checkbox.SetActive(true);
        checkbox.transform.SetParent(gameObject.transform);
        answerOptions.Add(checkbox);

        checkbox.transform.Find("Label").GetComponent<TextMeshProUGUI>().text = labeltext;
    }

    public void AddRadiobutton(string labeltext)
    {
        var button = Instantiate(radiobuttonTemplate);
        button.SetActive(true);
        button.transform.SetParent(gameObject.transform);
        answerOptions.Add(button);

        button.GetComponent<Toggle>().group = RadiobuttonGroup;
        button.transform.Find("Label").GetComponent<TextMeshProUGUI>().text = labeltext;
    }
}