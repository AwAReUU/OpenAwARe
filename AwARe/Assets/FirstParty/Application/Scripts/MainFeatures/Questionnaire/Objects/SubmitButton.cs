// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

using System;
using AwARe.InterScenes.Objects;
using AwARe.Server.Logic;
using Codice.CM.SEIDInfo;
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
        /// Switches the scene back to the homescreen.
        /// TODO: Format and Send answers to server.
        /// </summary>
        public void Submit()
        {
            Client.GetInstance().Post<QuestionnaireRequestBody, object>("quest/save", new QuestionnaireRequestBody
            {
                questionnaire = "Dit is een test" // TODO: Fill in the actual questionnaire.
            }).Then((_) =>
            {
                // Do nothing, we dont expect any return values.
                return null;
            }).Catch((err) =>
            {
                // Warning: This is an async method that may run after "LoadScene(...)"!

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
        }
    }
}