using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

//This file contains the QuestionnaireConstructor, as well as several data classes
//these data classes are used to store the data read from the json file
//unity's json reader is a bit limited, so the data holders are used as intermediary
public class QuestionnaireConstructor : MonoBehaviour
{
    QuestionnaireData data;
    TextAsset jsonfile;

    [SerializeField]
    private GameObject questionnaireTemplate;

    void Start()
    {
        jsonfile = Resources.Load<TextAsset>("Questionnaires/Testformat");
        data = JsonUtility.FromJson<QuestionnaireData>(jsonfile.text);

        if (data == null){ Debug.Log("data file is null"); }

        MakeQuestionnaire(data);


        //TODO:
        //if yes questions
        //submit button functionality
        //code cleanup
        //documentation
        //possibly: add 'required' bool to questions, json format, etc.

        //working on: if yes questions
        //implemented: if yes questions are added to the questionnaire and hidden when a question is made
        //todo: if yes questions are un- and re-hidden when the answer option that triggers them changes

        //ideas for hiding/revealing ifyes questions:
        //give the answer option templates a script that knows which question it belongs to,
        //and have the option tell the question when it changes state. question then decides to hide or reveal.
        //for checkbox & radio: on value changed, share same method
        //for text inut: not applicable

        //bug: if yes questions stack on eachother on first display
        //solution: put them in one container, put container in questionnaire
        //container gets toggled instead, questions in container are always active
        //container has vertical layout group, padding 20, upper center alignment

        
        //json debug shows that if a data point doesn't need to be there, you can leave it out of the file and it will
        //default to the standard for its datatype (null, 0, false, etc.)
    }

    GameObject MakeQuestionnaire(QuestionnaireData data)
    {
        var questionnaire = Instantiate(questionnaireTemplate, gameObject.transform, false);
        questionnaire.SetActive(true);

        var questionnairescript = questionnaire.gameObject.GetComponent<Questionnaire>();
        questionnairescript.SetTitle(data.questionnairetitle);
        questionnairescript.SetDescription(data.questionnairedescription);

        foreach(QuestionData question in data.questions)
        {
            questionnairescript.addQuestion(question);
        }

        return questionnaire;
    }
}

[Serializable]
public class QuestionnaireData
{
    public string questionnairetitle;
    public string questionnairedescription;
    public List<QuestionData> questions;
}

[Serializable]
public class AnswerOptionData
{
    public string optiontype;
    public string optiontext;
}

[Serializable]
public class QuestionData
{
    public string questiontitle;
    public bool ifyes;
    public int ifyestrigger;
    public List<QuestionData> ifyesquestions;
    public List<AnswerOptionData> answeroptions;
}