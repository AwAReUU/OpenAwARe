using System.Collections.Generic;

using AwARe.Questionnaire.Data;

using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace AwARe.Questionnaire.Objects
{
    /// <summary>
    /// Class <c>Questionnaire</c> is responsible for managing and displaying a questionnaire in the UI.
    /// </summary>
    public class Questionnaire : MonoBehaviour
    {

        [SerializeField] private TextMeshProUGUI title;
        [SerializeField] private TextMeshProUGUI description;
        [SerializeField] private Transform questionsWindow;
        [SerializeField] private GameObject questionPrefab;

        [SerializeField] private GameObject submitButton;
        [SerializeField] private GameObject questionCreatorPrefab;
        private List<GameObject> questions;

        void Awake()
        {
            questions = new List<GameObject>();
        }

        /// <summary>
        /// Set the title of the questionnaire in the UI.
        /// </summary>
        /// <param name="questionnaireTitle">Title to set to the UI.</param>
        public void SetTitle(string title) => this.title.text = title;
        /// <summary>
        /// Set the description of the questionnaire in the UI.
        /// </summary>
        /// <param name="questionnaireDescription">Description to set in the UI.</param>
        public void SetDescription(string description) => this.description.text = description;


        /// <summary>
        /// Instantiate a questionCreator and use it to create a new question.
        /// </summary>
        /// <param name="data">Data containing the information about the question to be added.</param>
        /// <returns>The instantiated questionCreatorObject.</returns>
        public GameObject AddQuestion(QuestionData data)
        {
            // Instantiate the QuestionCreator prefab
            GameObject questionCreatorObject = InstantiateQuestionCreator();
            QuestionCreator questionCreator = questionCreatorObject.GetComponent<QuestionCreator>();

            // Set the title and if-yes information using the instantiated QuestionCreator
            ConfigureQuestionCreator(questionCreator, data);

            // Instantiate the template and set its parent
            questionCreatorObject.transform.SetParent(gameObject.transform.Find("Question Scroller/Content"));
            questionCreatorObject.SetActive(true);
            questions.Add(questionCreatorObject.gameObject);

            // Add each answer option to the question using the instantiated QuestionCreator
            AddAnswerOptions(questionCreator, data);

            // Add the questions to be shown if yes is answered to the questionnaire, and hide them
            HandleIfYesQuestions(questionCreator, data);

            return questionCreatorObject;
        }

        private GameObject InstantiateQuestionCreator() => Instantiate(questionCreatorPrefab);

        private void ConfigureQuestionCreator(QuestionCreator questionCreator, QuestionData data)
        {
            questionCreator.SetTitle(data.questionTitle);
            questionCreator.SetIfyes(data.ifYes, data.ifYesTrigger);
        }

        private void AddAnswerOptions(QuestionCreator questionCreator, QuestionData data)
        {
            foreach (AnswerOptionData answer in data.answerOptions)
                questionCreator.AddAnswerOption(answer);
        }

        private void HandleIfYesQuestions(QuestionCreator questionCreator, QuestionData data)
        {
            if (!data.ifYes) return;

            foreach (QuestionData ifyesQuestionData in data.ifYesQuestions)
            {
                var ifyesQuestion = AddQuestion(ifyesQuestionData);
                ifyesQuestion.SetActive(false);
                questionCreator.ifYesQuestions.Add(ifyesQuestion);
            }
        }
    }
}
