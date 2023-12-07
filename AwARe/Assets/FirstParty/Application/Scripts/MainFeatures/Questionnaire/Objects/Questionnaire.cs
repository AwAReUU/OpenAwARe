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

        //create a new question gameobject from its data class and returns it
        public GameObject AddQuestion(QuestionData data)
        {
            //instantiate the template
            var questionObject = Instantiate(questionPrefab, questionsWindow);
            questionObject.SetActive(true);
            questions.Add(questionObject);

            // Set the title, questionnaire it belongs to, and if its an 'if yes' question
            //'if yes' questions show more questions when 'yes' is answer to them
            var questionscript = question.gameObject.GetComponent<QuestionCreator>();
            questionscript.SetTitle(data.questionTitle);
            questionscript.SetIfyes(data.ifYes, data.ifYesTrigger);

            //add each answer option to the question
            foreach (AnswerOptionData answer in data.answerOptions)
            {
                questionscript.AddAnswerOption(answer);
            }

            //add the questions to be shown if yes is answered to the questionnaire, and hides them
            if(data.ifYes)
            {
                foreach (QuestionData ifyesQuestionData in data.ifYesQuestions)
                {
                    var ifyesQuestion = AddQuestion(ifyesQuestionData);
                    ifyesQuestion.SetActive(false);
                    questionscript.ifYesQuestions.Add(ifyesQuestion);
                }
            }

            return questionObject;
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
