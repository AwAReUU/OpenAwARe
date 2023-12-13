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
        private List<GameObject> questions { get; set; }

        /// <summary>
        /// Initialize a new <see cref="Questionnaire"/>.
        /// </summary>
        private void Awake()
        {
            questions = new List<GameObject>();
        }

        /// <summary>
        /// Obtain the questions in this questionnaire.
        /// </summary>
        /// <returns>The question GameObjects that are inside of this questionnaire.</returns>
        public List<GameObject> GetQuestions() => questions;

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
            GameObject questionCreatorObject = Instantiate(questionCreatorPrefab);
            Question question = questionCreatorObject.GetComponent<Question>();

            question.SetTitle(data.questionTitle);
            question.SetIfYes(data.ifYes, data.ifYesTrigger);

            // Instantiate the template and set its parent
            questionCreatorObject.transform.SetParent(gameObject.transform.Find("Question Scroller/Content"));
            questionCreatorObject.SetActive(true);
            questions.Add(questionCreatorObject.gameObject);

            // Add each answer option to the question using the instantiated QuestionCreator
            AddAnswerOptions(question, data);

            // Add the questions to be shown if yes is answered to the questionnaire, and hide them
            AddIfYesQuestions(question, data);

            return questionCreatorObject;
        }

        private void ConfigureQuestionCreator(Question question, QuestionData data)
        {
            question.SetTitle(data.questionTitle);
            question.SetIfyes(data.ifYes, data.ifYesTrigger);
        }

        /// <summary>
        /// Add a single answer option, using "QuestionData" to the given "Question".
        /// </summary>
        /// <param name="question">The question to add the new answer option to.</param>
        /// <param name="data">The data needed to construct the new answer option.</param>
        private void AddAnswerOptions(Question question, QuestionData data)
        {
            foreach (AnswerOptionData answer in data.answerOptions)
                question.AddAnswerOption(answer);
        }

        /// <summary>
        /// Adds all IfYesQuestions from <paramref name="data"/> to <paramref name="question"/> if they exist.
        /// </summary>
        /// <param name="question">The Question instance triggering the "ifYes" condition.</param>
        /// <param name="data">The QuestionData containing information about the "ifYes" condition and associated questions.</param>
        private void AddIfYesQuestions(Question question, QuestionData data)
        {
            if (!data.ifYes) return;

            foreach (QuestionData ifYesQuestionData in data.ifYesQuestions)
            {
                GameObject ifYesQuestion = AddQuestion(ifYesQuestionData);
                ifYesQuestion.SetActive(false);
                question.IfYesQuestions.Add(ifYesQuestion);
            }
        }
    }
}
