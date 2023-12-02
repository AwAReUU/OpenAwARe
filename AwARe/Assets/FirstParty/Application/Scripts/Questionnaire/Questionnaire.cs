using System.Collections.Generic;
using TMPro;
using UnityEngine;

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

    //create a new question gameobject from its data class and returns it
    public GameObject AddQuestion(QuestionData data)
    {
        //instantiate the template
        var question = Instantiate(questionTemplate);
        question.transform.SetParent(gameObject.transform.Find("Question Scroller/Content"));
        question.SetActive(true);
        questions.Add(question.gameObject);

        //set the title, questionnaire it belongs to, and if its an 'if yes' question
        //'if yes' questions show more questions when 'yes' is answer to them
        var questionscript = question.gameObject.GetComponent<Question>();
        questionscript.SetTitle(data.questiontitle);
        questionscript.SetIfyes(data.ifyes, data.ifyestrigger);
        questionscript.SetParentQuestionnaire(this);

        //add each answer option to the question
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

        //add the questions to be shown if yes is answered to the questionnaire, and hides them
        if(data.ifyes)
        {
            foreach (QuestionData ifyesQuestionData in data.ifyesquestions)
            {
                var ifyesQuestion = AddQuestion(ifyesQuestionData);
                ifyesQuestion.SetActive(false);
                questionscript.ifyesQuestions.Add(ifyesQuestion);
            }
        }

        return question;
    }
}
