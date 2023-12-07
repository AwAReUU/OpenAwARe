using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace AwARe.Questionnaire.Objects
{
    public class Questionnaire : MonoBehaviour
    {

        [SerializeField] private TextMeshProUGUI title;
        [SerializeField] private TextMeshProUGUI description;
        [SerializeField] private Transform questionsWindow;
        [SerializeField] private GameObject questionPrefab;

        [SerializeField] private GameObject submitButton;
        [SerializeField] private GameObject questionCreatorPrefab;
        //[SerializeField] private GameObject questionTemplate;
        private List<GameObject> questions;

        void Awake()
        {
            questions = new List<GameObject>();
        }

        public void SetTitle(string title)
        {
            this.title.text = title;
        }

        public void SetDescription(string description)
        {
            this.description.text = description;
        }

        // Instantiate a QuestionCreator and use it to create a new question
        public GameObject AddQuestion(QuestionData data)
        {
            // Instantiate the QuestionCreator prefab
            GameObject questionCreatorObject = Instantiate(questionCreatorPrefab);
            QuestionCreator questionCreator = questionCreatorObject.GetComponent<QuestionCreator>();

            // Set the title and if-yes information
            questionCreator.SetTitle(data.questionTitle);
            questionCreator.SetIfyes(data.ifYes, data.ifYesTrigger);

            // Instantiate the template
            //GameObject questionTemp = Instantiate(questionTemplate);
            questionCreatorObject.transform.SetParent(gameObject.transform.Find("Question Scroller/Content"));
            questionCreatorObject.SetActive(true);
            questions.Add(questionCreatorObject.gameObject);

            // Set the title and questionnaire information using the instantiated QuestionCreator
            var questionscript = questionCreatorObject.GetComponent<QuestionCreator>();
            questionscript.SetTitle(data.questionTitle);
            questionscript.SetIfyes(data.ifYes, data.ifYesTrigger);

            // Add each answer option to the question using the instantiated QuestionCreator
            foreach (AnswerOptionData answer in data.answerOptions)
            {
                questionCreator.AddAnswerOption(answer);
            }

            // Add the questions to be shown if yes is answered to the questionnaire, and hide them
            if (data.ifYes)
            {
                foreach (QuestionData ifyesQuestionData in data.ifYesQuestions)
                {
                    var ifyesQuestion = AddQuestion(ifyesQuestionData);
                    ifyesQuestion.SetActive(false);
                    questionscript.ifYesQuestions.Add(ifyesQuestion);
                }
            }

            return questionCreatorObject;
        }

        private void AddAnswer(AnswerOptionData data, Question question)
        {
            switch (data.optiontype)
            {
                case "radio":
                    question.AddRadiobutton(data.optiontext);
                    break;
                case "checkbox":
                    question.AddCheckbox(data.optiontext);
                    break;
                case "textbox":
                    question.AddTextinput(data.optiontext);
                    break;
            }
        }

        private void AddIfYesQuestion(QuestionData data, Question question)
        {
            var ifyesQuestion = AddQuestion(data);
            ifyesQuestion.SetActive(false);
            question.ifyesQuestions.Add(ifyesQuestion);
        }

    }
}
