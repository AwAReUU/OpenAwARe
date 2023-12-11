using UnityEngine;
using UnityEngine.UI;

namespace AwARe.Questionnaire.Objects
{
    public class ToggleHandler : MonoBehaviour
    {
        private GameObject question;
        private int optionNumber;

        /// <summary>
        /// notifies the question that this answer option is now yes or no, true or false.
        /// </summary>
        public void NotifyChange() =>
            question.GetComponent<Question>().ChangeIfyesState(optionNumber, gameObject.GetComponent<Toggle>().isOn);

        public void SetQuestion(GameObject question) => this.question = question;

        //should be called exactly once per answer option object, when it is created by te question
        public void AssignIndex(int optionIndex)
        {
            this.optionIndex = optionIndex;
        }
    }
}