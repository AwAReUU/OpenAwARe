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
        /// <value>
        /// The question that this <see cref="ToggleHandler"/>'s answerOption belongs to.
        /// </value>
        private GameObject Question { get; set; }
        /// <value>
        /// The index of this <see cref="ToggleHandler"/>'s answerOption.
        /// </value>
        private int OptionIndex { get; set; }

        /// <summary>
        /// Notifies the question that this answer option is now checked or unchecked.
        /// </summary>
        public void NotifyChange()
        {
            if (Question == null)
            {
                Debug.LogError("ToggleHandler's Question property is null");
                return;
            }
            Question questionComponent = Question.GetComponent<Question>();
            if (questionComponent == null)
            {
                Debug.LogError("ToggleHandler's questionComponent is null"); 
                return;
            }
            Toggle toggle = gameObject.GetComponent<Toggle>();
            if (toggle == null)
            {
                Debug.LogError("ToggleHandler's Toggle is null");
                return;
            }
            questionComponent.ChangeIfYesState(OptionIndex, toggle.isOn);
        }

        /// <summary>
        /// Set the question of this <see cref="ToggleHandler"/>.
        /// </summary>
        /// <param name="question">Question that this answerOption's toggleHandler is a child of.</param>
        public void SetQuestion(GameObject question) => 
            Question = question;

        /// <summary>
        /// Assign the index <paramref name="optionIndex"/> to this option.
        /// </summary>
        /// <param name="optionIndex">The index of this option.</param>
        public void AssignIndex(int optionIndex) =>
            OptionIndex = optionIndex;
    }
}