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

    public class SubmitButton : MonoBehaviour
    {
        public void Submit()
        {
            Client.GetInstance().Post("quest/save", new QuestionnaireRequestBody
            {
                questionnaire = "Dit is een test"
            }).Then((_) =>
            {
                // Do nothing, we dont expect any return values.
            }).Catch((_) =>
            {
                // TODO: Handle errors.
                // Warning: This is an async method that may run after "LoadScene(...)"!
            }).Send();

            SceneSwitcher.Get().LoadScene("Home");
        }
    }
}