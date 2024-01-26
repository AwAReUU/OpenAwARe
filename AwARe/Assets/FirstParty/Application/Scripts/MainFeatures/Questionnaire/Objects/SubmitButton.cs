// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

using System.Collections.Generic;
using AwARe.InterScenes.Objects;
using AwARe.Questionnaire.Data;
using AwARe.Data.Logic;
using AwARe.RoomScan.Polygons.Objects;
using AwARe.Data.Objects;
using AwARe.Objects;
using UnityEngine;
using UnityEngine.UI;

namespace AwARe.Questionnaire.Objects
{
    /// <summary>
    /// Class <c>SubmitButton</c> contains logic for the submit button's behaviour.
    /// </summary>
    public class SubmitButton : MonoBehaviour
    {
        private GameObject questionnaireObject;
        [SerializeField] public QuestionnaireConstructor questionnaireConstructor;
        [SerializeField] public SaveLoadManager filehandler;

        private void Start()
        {
            filehandler = GetComponent<SaveLoadManager>(); // Assuming SaveLoadManager is attached to the same GameObject
            questionnaireObject = questionnaireConstructor.QuestionnaireFromJsonString();
            Debug.Log($"filehandler: {filehandler}");
            Debug.Log($"questionnaireObject: {questionnaireObject}");
        }

        /// <summary>
        /// Switches the scene back to the home screen.
        /// TODO: Format and Send answers to the server.
        /// </summary>
        public void Submit()
        {
            SceneSwitcher.Get().LoadScene("Home");
            SaveQuestionnaire();
        }

        public void SaveQuestionnaire()
        {
            if (filehandler != null && questionnaireObject != null)
            {
                Questionnaire questionnaire = questionnaireObject.GetComponent<Questionnaire>();

                if (questionnaire != null)
                {
                    // Create a list to store the questionnaire data
                    List<QuestionData> questionnaireDataList = new List<QuestionData>();

                    // Iterate through each question in the questionnaire
                    foreach (var questionObject in questionnaire.Questions)
                    {
                        // Get the Question component from the current questionObject
                        Question question = questionObject.GetComponent<Question>();

                        // Create a new QuestionData object to store question information
                        QuestionData questionData = new QuestionData
                        {
                            questionTitle = question.GetTitle(),
                            ifYes = question.IfYes,
                            ifYesTrigger = question.IfYesTriggerIndex,
                            ifYesQuestions = new List<QuestionData>(),
                            answerOptions = new List<AnswerOptionData>(),
                            UserAnswers = question.UserAnswers
                        };

                        // Add the current question data to the list
                        questionnaireDataList.Add(questionData);
                    }

                    // Save collected data
                    Debug.Log($"Number of collected data items: {questionnaireDataList.Count}");
                    filehandler.SaveDataToJson("collectedData.json", questionnaireDataList);
                }
                else
                {
                    Debug.LogError("Questionnaire component not found on the questionnaireObject.");
                }
            }
            else
            {
                Debug.LogError("filehandler or questionnaireObject is null.");
            }
        }


        public void LoadQuestionnaire()
        {
            // Load collected data
            List<QuestionnaireData> loadedData = filehandler.LoadDataFromJson<List<QuestionnaireData>>("collectedData.json");
            // Handle loadedData as needed
        }
    }
}
