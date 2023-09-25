using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class QuestionnaireConstructor : MonoBehaviour
{
    //The idea of this class:
    //This class reads json files and converts them into a questionnaire
    //The basic json reader functionality is rather limited, so this will read the data from the json file,
    //then convert it into a proper quesionnaire (which follows the structure as seen in the UML)

    // Start is called before the first frame update

    public QuestionnaireData data;
    public TextAsset jsonfile;

    void Start()
    {
        jsonfile = Resources.Load<TextAsset>("Questionnaires/Testformat");
        data = JsonUtility.FromJson<QuestionnaireData>(jsonfile.text);

        if (data == null){ Debug.Log("data file is null"); }

        Debug.Log(data.questionnairetitle);
        Debug.Log(data.questionnairedescription);
        foreach (QuestionData question in data.questions)
        {
            Debug.Log(question.questiontitle);
            Debug.Log(question.questiontype);
            Debug.Log(question.ifyes);
            Debug.Log(question.ifyestrigger);

            foreach (AnswerOption option in question.answeroptions)
            {
                Debug.Log(option.optiontype);
                Debug.Log(option.optiontext);
            }

            if(question.ifyes)
            {
                Debug.Log("in");
                Debug.Log(question.ifyesquestion.questiontitle);
                Debug.Log(question.ifyesquestion.questiontype);
                //Debug.Log(question.ifyesquestion.ifyes);
                //Debug.Log(question.ifyesquestion.ifyestrigger);

                foreach (AnswerOption option in question.ifyesquestion.answeroptions)
                {
                    Debug.Log(option.optiontype);
                    Debug.Log(option.optiontext);
                }

                if(question.ifyesquestion.ifyes){
                    Debug.Log("success");
                    Debug.Log(question.ifyesquestion.ifyesquestion.questiontitle);
                }
            }
        }

        //json debug shows that if a data point doesn't need to be there, you can leave it out of the file and it will
        //default to the standard for its datatype (null, 0, false, etc.)



        // ████████╗░█████╗░██████╗░░█████╗░
        // ╚══██╔══╝██╔══██╗██╔══██╗██╔══██╗
        // ░░░██║░░░██║░░██║██║░░██║██║░░██║
        // ░░░██║░░░██║░░██║██║░░██║██║░░██║
        // ░░░██║░░░╚█████╔╝██████╔╝╚█████╔╝
        // ░░░╚═╝░░░░╚════╝░╚═════╝░░╚════╝░
        
        //make question factory and give it to questionnaire (see prefabs in bird game tutorial)
        //make question (visual) prefab (obviously)
        //make questionnaire (visual)
        //make questionnaire factory (this file)
        //other:
        //(test) ios notifications
        //documentatie
        //(in general voor groep) documenten deadlines (architectuur enzo)

        //notes:
        //make 'template' question gameobjects for the different question types?
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

[Serializable]
public class QuestionnaireData
{
    public QuestionnaireData()
    {

    }

    public string questionnairetitle;
    public string questionnairedescription;
    public List<QuestionData> questions;
}

//for some reason, unity's json parser cannot parse a class instance into another one of the same class
//because of this, Questiondata holds an IfyesQuestionData and vice verca, but they are functionally identical
//in the json files and the resulting questionnaire, this makes no difference. It is only 
//important in the questionnaireconstructor
[Serializable]
public class QuestionData
{
    public QuestionData()
    {

    }

    public string questiontitle;
    public string questiontype;
    public bool ifyes;
    public int ifyestrigger;
    public IfyesQuestionData ifyesquestion;
    public List<AnswerOption> answeroptions;
}

[Serializable]
public class AnswerOption
{
    public AnswerOption()
    {

    }

    public string optiontype;
    public string optiontext;
}

[Serializable]
public class IfyesQuestionData
{
    public IfyesQuestionData()
    {

    }

    public string questiontitle;
    public string questiontype;
    public bool ifyes;
    public int ifyestrigger;
    public QuestionData ifyesquestion;
    public List<AnswerOption> answeroptions;

}
