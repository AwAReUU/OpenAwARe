using AwARe.Questionnaire.Data;

using UnityEngine;
using UnityEngine.Serialization;

namespace AwARe.Questionnaire.Objects
{
    //This file contains the QuestionnaireConstructor, as well as several data classes
    //these data classes are used to store the data read from the json file
    //unity's json reader is a bit limited, so the data holders are used as intermediary
    public class QuestionnaireConstructor : MonoBehaviour
    {
        private QuestionnaireData data;

        [SerializeField] private GameObject questionnairePrefab;
        [SerializeField] private Transform subcanvas;
        [SerializeField] TextAsset jsonfile;
        private GameObject questionnaire;

        //currently, start only loads the test questionnaire from a json file
        //in the future, this obviously has to change
        void Start()
        {
            QuestionnaireFromFile("Testformat");
        }

        public QuestionnaireData QuestionnaireFromFile(string filename)
        {
            data = JsonUtility.FromJson<QuestionnaireData>(jsonfile.text);
            if (data != null)
                return data;

            Debug.Log("Data file is null. Is the file 'Questionnaires/" + filename + "' correct?");
            return null;
        }

        //makes a questionnaire object and returns it
        private GameObject MakeQuestionnaire(QuestionnaireData questionnaireData)
        {
            GameObject questionnaireObject = Instantiate(questionnaireTemplate, gameObject.transform, false);
            questionnaireObject.SetActive(true);

            Questionnaire questionnaireScript = questionnaireObject.gameObject.GetComponent<Questionnaire>();
            questionnaireScript.SetTitle(questionnaireData.questionnaireTitle);
            questionnaireScript.SetDescription(questionnaireData.questionnaireDescription);

            foreach(QuestionData question in questionnaireData.questions)
                questionnaireScript.AddQuestion(question);

            return questionnaireObject;
        }
    }
}


//Notes to self:
//todo: fix layout with diff aspect ratio's
//figure out how to dynamically alter the size of shit maybe?
//layoutelement, layoutgroup, content size fitter, etc.