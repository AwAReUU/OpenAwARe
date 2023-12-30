using AwARe.InterScenes.Objects;

using UnityEngine;
using UnityEngine.UI;

namespace AwARe.Questionnaire.Objects
{
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
            SceneSwitcher.Get().LoadScene("Home");
        }
    }
}