using System;
using System.Collections.Generic;

using UnityEngine;

namespace AwARe.Questionnaire.Objects
{
    //This file contains the QuestionnaireConstructor, as well as several data classes
    //these data classes are used to store the data read from the json file
    //unity's json reader is a bit limited, so the data holders are used as intermediary
    public class QuestionnaireConstructor : MonoBehaviour
    {
        private QuestionnaireData data;

        [SerializeField] private GameObject questionnaireTemplate;
        [SerializeField] TextAsset jsonfile;

        //currently, start only loads the test questionnaire from a json file
        //in the future, this obviously has to change
        void Start()
        {
            QuestionnaireFromFile("Exampleformat");
        }

        public GameObject QuestionnaireFromFile(string filename)
        {
            data = JsonUtility.FromJson<QuestionnaireData>(jsonfile.text);
            if(data == null)
            {
                Debug.Log("Data file is null. Is the file 'Questionnaires/" + filename + "' correct?");
                return new GameObject(); //maybe return null?
            }

            return MakeQuestionnaire(data);
        }

        //makes a questionnaire object and returns it
        private GameObject MakeQuestionnaire(QuestionnaireData data)
        {
            var questionnaire = Instantiate(questionnaireTemplate, gameObject.transform, false);
            questionnaire.SetActive(true);

            var questionnairescript = questionnaire.gameObject.GetComponent<Questionnaire>();
            questionnairescript.SetTitle(data.questionnairetitle);
            questionnairescript.SetDescription(data.questionnairedescription);

            foreach(QuestionData question in data.questions)
            {
                questionnairescript.AddQuestion(question);
            }

            return questionnaire;
        }
    }

    //intermediary data holder classes
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
}


//Notes to self:
//todo: fix layout with diff aspect ratio's
//figure out how to dynamically alter the size of shit maybe?
//layoutelement, layoutgroup, content size fitter, etc.