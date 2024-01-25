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
        /// <value>
        /// Reference to "Title" inside of "QuestionnairePrefab".
        /// </value>
        [SerializeField] private TextMeshProUGUI title;
        /// <value>
        /// Reference to "Description" inside of "Questionnaire" prefab.
        /// </value>
        [SerializeField] private TextMeshProUGUI description;
        /// <value>
        /// TODO: add comment.
        /// </value>
        [SerializeField] private Transform questionsWindow;
        /// <value>
        /// Reference to "SubmitButton" inside of "Questionnaire" prefab.
        /// </value>
        [SerializeField] private GameObject submitButton;
        /// <value>
        /// Reference to "Question" prefab.
        /// </value>
        [SerializeField] private GameObject questionPrefab;
        /// <value>
        /// List of questions that this questionnaire is currently holding.
        /// </value>
        public List<GameObject> questions { get; set; }
        public List<QuestionnaireData> collectedData;

        /// <summary>
        /// Initialize a new <see cref="Questionnaire"/>.
        /// </summary>
        private void Awake()
        {
            questions = new List<GameObject>();
            collectedData = new List<QuestionnaireData>();
        }
        public List<QuestionnaireData> GetCollectedData() => collectedData;

        /// <summary>
        /// Obtain the questions in this questionnaire.
        /// </summary>
        /// <returns>The question GameObjects that are inside of this questionnaire.</returns>
        public List<GameObject> GetQuestions() => questions;

        /// <summary>
        /// Set the title of the questionnaire in the UI.
        /// </summary>
        /// <param name="questionnaireTitle">Title to set to the UI.</param>
        public void SetTitle(string questionnaireTitle) => title.text = questionnaireTitle;
        /// <summary>
        /// Set the description of the questionnaire in the UI.
        /// </summary>
        /// <param name="questionnaireDescription">Description to set in the UI.</param>
        public void SetDescription(string questionnaireDescription) => description.text = questionnaireDescription;


        /// <summary>
        /// Instantiate a questionCreator and use it to create a new question.
        /// </summary>
        /// <param name="data">Data containing the information about the question to be added.</param>
        /// <returns>The instantiated questionCreatorObject.</returns>
        public GameObject AddQuestion(QuestionData data)
        {
            GameObject questionObject = Instantiate(questionPrefab, questionsWindow);
            Question question = questionObject.GetComponent<Question>();
            questionObject.SetActive(true);
            questions.Add(questionObject.gameObject);

            question.SetTitle(data.questionTitle);
            question.SetIfYes(data.ifYes, data.ifYesTrigger);

            // Instantiate the template and set its parent
            question.SetParentQuestionnaire(this);

            AddAnswerOptions(question, data);
            AddIfYesQuestions(question, data);

            // Add user responses to the QuestionData
            data.UserAnswers = new List<AnswerData>();

            // Add the question data to the collected data
            collectedData.Add(new QuestionnaireData
            {
                questionnaireTitle = "Modify this accordingly", // Modify this accordingly
                questionnaireDescription = "Modify this accordingly", // Modify this accordingly
                questions = new List<QuestionData> { data }
            });

            return questionObject;
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
        /// It sets them as inactive, so they still need to be activated in order to be displayed.
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
