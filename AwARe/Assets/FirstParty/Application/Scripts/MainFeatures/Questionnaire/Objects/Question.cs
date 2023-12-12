using System;
using System.Collections.Generic;

using AwARe.Questionnaire.Data;

using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AwARe.Questionnaire.Objects
{
    public class Question : MonoBehaviour
    {
        [SerializeField] private GameObject titleContainer;
        [SerializeField] private GameObject textInputPrefab;
        [SerializeField] private GameObject checkBoxPrefab;
        [SerializeField] private GameObject radioButtonPrefab;

        private List<GameObject> answerOptions { get; set; }
        public List<bool> answerOptionStates { get; set; }
        public int answerOptionNumberCounter = 0;
        public int ifYesTriggerIndex { get; private set; }
        public bool ifYes { get; private set; }
        public List<GameObject> ifYesQuestions { get; set; }

        void Awake()
        {
            answerOptions = new List<GameObject>();
            answerOptionStates = new List<bool>();
            ifYesQuestions = new List<GameObject>();
        }

        /// <summary>
        /// Set the title of this question.
        /// </summary>
        /// <param name="questionTitle">new title to set to this question.</param>
        public void SetTitle(string questionTitle) => title.GetComponent<TextMeshProUGUI>().text = questionTitle;

        /// <summary>
        /// Obtain the title form this question.
        /// </summary>
        /// <returns>The title of this question.</returns>
        public string GetTitle() => title.GetComponent<TextMeshProUGUI>().text;

        /// <summary>
        /// Add an answer option to this question.
        /// </summary>
        /// <param name="answerOptionData">The data representing the new option to be added.</param>
        public void AddAnswerOption(AnswerOptionData answerOptionData)
        {
            AnswerOptionSpawner answerOptionFactory = CreateAnswerOptionSpawner((OptionType)Enum.Parse(typeof(OptionType), answerOptionData.optionType));
            GameObject newOption = answerOptionFactory.CreateAnswerOption(answerOptionData.optionText);
            answerOptions.Add(newOption);
            answerOptionStates.Add(false);
        }

        /// <summary>
        /// Create the factory corresponding to this question's option type.
        /// </summary>
        /// <param name="optionType">The optionType of this question.</param>
        /// <returns>A factory to construct the answer option corresponding to "optionType".</returns>
        private AnswerOptionSpawner CreateAnswerOptionSpawner(OptionType optionType)
        {
            switch (optionType)
            {
                case OptionType.Radio:
                    return new RadioAnswerOption(gameObject, radioButtonPrefab, answerOptionNumberCounter++);
                case OptionType.Checkbox:
                    return new CheckBoxAnswerOption(gameObject, checkBoxPrefab, answerOptionNumberCounter++);
                case OptionType.Textbox:
                    return new TextAnswerOption(gameObject, textInputPrefab);
                case OptionType.Error:
                default:
                    throw new System.ArgumentException("Invalid option type: " + optionType);
            }
        }

        /// <summary>
        /// Sets the index of the answerOption that triggers additional questions.
        /// <paramref name="ifYes"/> should be set to true if this answer contains addition questions.
        /// </summary>
        /// <param name="ifYes">The condition that triggers additional questions.</param>
        /// <param name="ifYesTriggerIndex">The index of the answer option that acts as the trigger for additional questions.</param>
        public void SetIfYes(bool ifYes, int ifYesTriggerIndex)
        {
            this.ifYes = ifYes;
            this.ifYesTriggerIndex = ifYesTriggerIndex;
        }

        /// <summary>
        /// Changes the state of an answer option and updates the visibility
        /// of additional questions if needed.
        /// </summary>
        /// <param name="optionIndex">The index of the answer option whose state is being changed.</param>
        /// <param name="newState">The new state to set for the specified answer option.</param>
        public void ChangeIfYesState(int optionIndex, bool newState)
        {
            answerOptionStates[optionIndex] = newState;

            foreach (GameObject ifYesQuestion in ifYesQuestions)
                ifYesQuestion.SetActive(answerOptionStates[ifYesTriggerIndex]);

            //Fixes a bug where items are stacked on eachother:
            if (answerOptionStates[ifYesTriggerIndex])
                LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)gameObject.transform.parent);
        }
    }
}
