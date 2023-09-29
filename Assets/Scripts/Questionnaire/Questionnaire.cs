using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Questionnaire : MonoBehaviour
{

    [SerializeField]
    private GameObject title;
    [SerializeField]
    private GameObject description;
    [SerializeField]
    private GameObject submitButton;
    [SerializeField]
    private GameObject questionTemplate;
    private List<GameObject> questions;
  
    void Awake()
    {
        questions = new List<GameObject>();
    }

    public void SetTitle(string questionnaireTitle)
    {
        this.title.GetComponent<TextMeshProUGUI>().text = questionnaireTitle;
    }

    public void SetDescription(string description)
    {
        this.description.GetComponent<TextMeshProUGUI>().text = description;
    }

    public GameObject addQuestion(QuestionData data)
    {
        var question = Instantiate(questionTemplate);
        question.transform.SetParent(gameObject.transform.Find("Question Scroller/Content"));
        question.SetActive(true);
        questions.Add(question.gameObject);

        var questionscript = question.gameObject.GetComponent<Question>();
        questionscript.SetTitle(data.questiontitle);
        questionscript.SetIfyes(data.ifyes, data.ifyestrigger);
        questionscript.SetParentQuestionnaire(this);

        foreach (AnswerOptionData answer in data.answeroptions)
        {
            if (answer.optiontype == "radio")
            {
                questionscript.AddRadiobutton(answer.optiontext);
            }
            else if (answer.optiontype == "checkbox")
            {
                questionscript.AddCheckbox(answer.optiontext);
            }
            else if (answer.optiontype == "textbox")
            {
                questionscript.AddTextinput(answer.optiontext);
            }
        }

        if(data.ifyes)
        {
            foreach (QuestionData ifyesQuestionData in data.ifyesquestions)
            {
                var ifyesQuestion = addQuestion(ifyesQuestionData);
                ifyesQuestion.SetActive(false);
                questionscript.ifyesQuestions.Add(ifyesQuestion);
            }
        }
        return question;
    }
}
