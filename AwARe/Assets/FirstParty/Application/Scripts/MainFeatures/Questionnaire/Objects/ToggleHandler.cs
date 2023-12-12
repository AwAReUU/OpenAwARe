using UnityEngine;
using UnityEngine.UI;

namespace AwARe.Questionnaire.Objects
{
    /// <summary>
    /// Class <c>ToggleHandler</c> is used to handle toggling of IfYes questions.
    /// This class can be used for radiobuttons, as well as checkboxes.
    /// </summary>
    public class ToggleHandler : MonoBehaviour
    {
        private GameObject question { get; set; }
        private int optionIndex { get; set; }

        /// <summary>
        /// notifies the question that this answer option is now yes or no, true or false.
        /// </summary>
        public void NotifyChange() =>
            question.GetComponent<Question>().ChangeIfYesState(optionIndex, gameObject.GetComponent<Toggle>().isOn);

        /// <summary>
        /// Set the question of this Togglehandler.
        /// </summary>
        /// <param name="question"></param>
        public void SetQuestion(GameObject question) => 
            this.question = question;

        /// <summary>
        /// Assign the index <paramref name="optionIndex"/> to this option.
        /// </summary>
        /// <param name="optionIndex"></param>
        public void AssignIndex(int optionIndex) =>
            this.optionIndex = optionIndex;
    }
}