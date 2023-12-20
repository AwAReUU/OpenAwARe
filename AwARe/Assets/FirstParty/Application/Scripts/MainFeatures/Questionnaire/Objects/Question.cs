using System;
using System.Collections.Generic;
using System.Linq;

using AwARe.Questionnaire.Data;

using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AwARe.Questionnaire.Objects
{
    public class Question : MonoBehaviour
    {
        /// <value>
        /// Reference to "Title" inside of "QuestionPrefab".
        /// </value>
        [SerializeField] private GameObject title;
        /// <value>
        /// Reference to "textInputPrefab".
        /// </value>
        [SerializeField] private GameObject textInputPrefab;
        /// <value>
        /// Reference to "checkBoxPrefab".
        /// </value>
        [SerializeField] private GameObject checkBoxPrefab;
        /// <value>
        /// Reference to "radioButtonPrefab".
        /// </value>
        [SerializeField] private GameObject radioButtonPrefab;

        /// <value>
        /// All AnswerOptions that this question has.
        /// </value>
        private List<GameObject> AnswerOptions { get; set; }
        /// <value>
        /// Indicates whether an AnswerOption at an index is checked or not.
        /// </value>
        public List<bool> AnswerOptionStates { get; set; }
        /// <value>
        /// The index of the answer option that toggles the IfYes questions.
        /// </value>
        public int IfYesTriggerIndex { get; private set; }
        /// <value>
        /// Indicates whether this question has ifYes questions.
        /// </value>
        public bool IfYes { get; private set; }
        /// <value>
        /// All IfYes questions that this question has.
        /// </value>
        public List<GameObject> IfYesQuestions { get; set; }
        /// <value>
        /// Counter which is used for assigning a unique index to an AnswerOption upon adding a new one.
        /// </value>
        private int CurrentAnswerOptionIndex = 0;

        private Questionnaire parentQuestionnaire;

        private void Awake()
        {
            AnswerOptions = new List<GameObject>();
            AnswerOptionStates = new List<bool>();
            IfYesQuestions = new List<GameObject>();
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
            AnswerOptions.Add(newOption);
            AnswerOptionStates.Add(false);
        }

        /// <summary>
        /// Create the factory corresponding to this question's option type.
        /// </summary>
        /// <param name="optionType">The optionType of this question.</param>
        /// <returns>A factory to construct the answer option corresponding to "optionType".</returns>
        private AnswerOptionSpawner CreateAnswerOptionSpawner(OptionType optionType)
        {
            return optionType switch
            {
                OptionType.Radio    => new RadioAnswerOption(gameObject, radioButtonPrefab, CurrentAnswerOptionIndex++),
                OptionType.Checkbox => new CheckBoxAnswerOption(gameObject, checkBoxPrefab, CurrentAnswerOptionIndex++),
                OptionType.Textbox  => new TextAnswerOption(gameObject, textInputPrefab),
                OptionType.Error    => throw new System.ArgumentException("Invalid option type: " + optionType),
                _                   => throw new System.ArgumentException("Invalid option type: " + optionType)
            };
        }

        /// <summary>
        /// Sets the index of the answerOption that triggers additional questions.
        /// <paramref name="ifYes"/> should be set to true if this answer contains addition questions.
        /// </summary>
        /// <param name="ifYes">The condition that triggers additional questions.</param>
        /// <param name="ifYesTriggerIndex">The index of the answer option that acts as the trigger for additional questions.</param>
        public void SetIfYes(bool ifYes, int ifYesTriggerIndex)
        {
            IfYes = ifYes;
            IfYesTriggerIndex = ifYesTriggerIndex;
        }

        /// <summary>
        /// Changes the state of an answer option and updates the visibility
        /// of the IfYes-questions this question if needed.
        /// </summary>
        /// <param name="optionIndex">The index of the answer option whose state is being changed.</param>
        /// <param name="newState">The new state to set for the specified answer option.</param>
        public void ChangeIfYesState(int optionIndex, bool newState)
        {
            if (optionIndex < 0 || optionIndex >= AnswerOptionStates.Count)
            {
                Debug.LogError($"Index {optionIndex} does not exist in AnswerOptionStates");
                return;
            }

            AnswerOptionStates[optionIndex] = newState;

            if (IfYesTriggerIndex < 0 || IfYesTriggerIndex >= AnswerOptionStates.Count)
            {
                Debug.LogError($"Index {IfYesTriggerIndex} does not exist in AnswerOptionStates");
                return;
            }

            foreach (GameObject ifYesQuestion in IfYesQuestions)
                ifYesQuestion.SetActive(AnswerOptionStates[IfYesTriggerIndex]);

            //Fixes a bug where items are stacked on eachother:
            if (AnswerOptionStates[IfYesTriggerIndex])
                LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)gameObject.transform.parent);
        }

        /// <summary>
        /// Sets the parentQuestionnaire object where this question lives in.
        /// </summary>
        /// <param name="parentQuestionnaire"></param>
        public void SetParentQuestionnaire(Questionnaire parentQuestionnaire) => this.parentQuestionnaire = parentQuestionnaire;
    }
}
