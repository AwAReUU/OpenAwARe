// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

using System;
using AwARe.InterScenes.Objects;
using AwARe.Server.Logic;
using System.Collections.Generic;
using System.IO;
using AwARe.Questionnaire.Data;
using UnityEngine;

namespace AwARe.Questionnaire.Objects
{
    [Serializable]
    public struct QuestionnaireRequestBody
    {
        public string questionnaire;
    }

    /// <summary>
    /// Class <c>SubmitButton</c> contains logic for the submit button's behaviour.
    /// </summary>
    public class SubmitButton : MonoBehaviour
    {
        /// <summary>
        /// The questionnaire object that this submitbutton will submit/save.
        /// </summary>
        public GameObject questionnaireObject;
        private string folderpath;

        private void Start()
        {
            folderpath = Path.Combine(Application.persistentDataPath, "Data/Questionnaireresults");

            //create the folderpath if it doesn't exist already on the device
            if (!Directory.Exists(folderpath))
            {
                Directory.CreateDirectory(folderpath);
            }
        }

        /// <summary>
        /// Switches the scene back to the home screen.
        /// </summary>
        public void Submit()
        {
            var data = GetData();
            if (data == null)
            {
                Debug.LogError("Questionnaire data is null.");
                return;
            }

            string jsonData = JsonUtility.ToJson(data, true);

            // Warning: Send() is an async method that may run after "LoadScene(...) because it is not awaited!
            // Await is not needed here, because we dont need any return value.
            Client.GetInstance().Post<QuestionnaireRequestBody, object>("quest/save", new QuestionnaireRequestBody
            {
                questionnaire = jsonData
            }).Then((_) =>
            {
                // Do nothing, we dont expect any return values.
                return null;
            }).Catch((err) =>
            {

                if (err.StatusCode == 403)
                {
                    // Unauthorized. User must login.
                    Debug.LogError("Failed to send Questionnaire. You're not logged in.");
                }
                else
                {
                    Debug.LogError("Failed to send Questionnaire: " + err.ServerMessage);
                }
            }).Send();

            SceneSwitcher.Get().LoadScene("Home");
            SaveQuestionnaire(data);
        }

        /// <summary>
        /// Get the questionnare data.
        /// </summary>
        private AnsweredQuestionnaireData GetData()
        {
            if (questionnaireObject != null)
            {
                Questionnaire questionnaire = questionnaireObject.GetComponent<Questionnaire>();

                if (questionnaire != null)
                {
                    // Create a AnsweredQuestionnaireData to store the responses in
                    AnsweredQuestionnaireData questionnaireData = new AnsweredQuestionnaireData
                    {
                        Questionnairetitle = questionnaire.GetTitle(),
                        //replace with real user id once accounts are implemented
                        UserID = "fake user id",
                        SubmissionDate = DateTime.Now.ToString(),
                        AnsweredQuestions = new List<AnsweredQuestionData>()
                    };

                    // Iterate through each question in the questionnaire
                    foreach (var questionObject in questionnaire.Questions)
                    {
                        //skip inactive questions (they have no answer given)
                        if (!questionObject.activeSelf) continue;

                        Question question = questionObject.GetComponent<Question>();

                        AnsweredQuestionData answeredQuestionData = new AnsweredQuestionData
                        {
                            QuestionTitle = question.GetTitle(),
                            Answers = new List<string>()
                        };

                        foreach (var entry in question.AnswerOptions)
                        {
                            AnswerOption answerOption = entry.Item1;
                            GameObject answerOptionObject = entry.Item2;
                            string answeroptiontext = answerOption.GetOptionText(answerOptionObject);
                            if (answeroptiontext != null) answeredQuestionData.Answers.Add(answeroptiontext);
                        }

                        questionnaireData.AnsweredQuestions.Add(answeredQuestionData);
                    }

                    return questionnaireData;

                }
                else
                {
                    Debug.LogError("Questionnaire component not found on the questionnaireObject.");
                    return null;
                }
            }
            else { return null; }
        }

        /// <summary>
        /// Saves the questionnare to a file.
        /// </summary>
        private void SaveQuestionnaire(AnsweredQuestionnaireData data)
        {
            // Save collected data
            int number = Directory.GetFiles(folderpath).Length;
            string path = Path.Combine(folderpath, "submission" + (number + 1));
            Save(data, path);
            Debug.Log("Saved questionnaire results to a file");
        }

        /// <summary>
        /// Saves an answeredQuestionnaireData instance to a file.
        /// </summary>
        /// <param name="data">The data instance to save.</param>
        /// <param name="filepath">The full path to the file (including name to give it).</param>
        public void Save(AnsweredQuestionnaireData data, string filepath)
        {
            // get the data path of this save data
            string dataPath = filepath;

            string jsonData = JsonUtility.ToJson(data, true);

            // create the file in the path if it doesn't exist
            // if the file path or name does not exist, return the default SO
            if (!Directory.Exists(Path.GetDirectoryName(dataPath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(dataPath));
            }

            // attempt to save here data
            try
            {
                // save datahere
                File.WriteAllText(dataPath, jsonData);
                Debug.Log("Save data to: " + dataPath);
            }
            catch (Exception e)
            {
                // write out error here
                Debug.LogError("Failed to save data to: " + dataPath);
                Debug.LogError("Error " + e.Message);
            }
        }
    }
}
