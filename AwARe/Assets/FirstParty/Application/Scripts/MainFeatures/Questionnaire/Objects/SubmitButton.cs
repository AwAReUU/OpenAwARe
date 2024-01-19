using System;
using AwARe.InterScenes.Objects;
using AwARe.Server.Logic;
using UnityEngine;
using UnityEngine.UI;

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
            Client.GetInstance().Post("quest/save", new QuestionnaireRequestBody
            {
                questionnaire = "Dit is een test" // TODO: Fill in the actual questionnaire.
            }).Then((_) =>
            {
                // Do nothing, we dont expect any return values.
            }).Catch((err) =>
            {
                // TODO: Handle errors.
                // Warning: This is an async method that may run after "LoadScene(...)"!

                if (err.StatusCode == 403)
                {
                    // Unauthorized. User must login.
                    // TODO: Show alert box
                }
            }).Send();

            SceneSwitcher.Get().LoadScene("Home");
        }
    }
}