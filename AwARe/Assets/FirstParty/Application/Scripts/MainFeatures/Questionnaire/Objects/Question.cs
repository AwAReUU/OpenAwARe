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
        public int answerOptionNumberCounter { get; set; }
        public int ifYesTrigger { get; private set; }
        public bool ifYes { get; private set; }
        public List<GameObject> ifYesQuestions { get; set; }

        void Awake()
        {
            answerOptions = new List<GameObject>();
            answerOptionStates = new List<bool>();
            ifYesQuestions = new List<GameObject>();
        }

        public void SetTitle(string questionTitle) =>
            titleContainer.GetComponent<TextMeshProUGUI>().text = questionTitle;

        public void AddAnswerOption(AnswerOptionData answerOptionData)
        {
            AnswerOptionFactory answerOptionFactory = CreateAnswerOptionFactory((OptionType)Enum.Parse(typeof(OptionType), answerOptionData.optionType));
            GameObject newOption = answerOptionFactory.GetAnswerOption(answerOptionData.optionText);
            answerOptions.Add(newOption);
            answerOptionStates.Add(false);
        }

        /// <summary>
        /// Create the factory corresponding to this question's option type.
        /// </summary>
        /// <param name="optionType">The optionType of this question.</param>
        /// <returns>A factory to construct the answer option corresponding to "optionType".</returns>
        private AnswerOptionFactory CreateAnswerOptionFactory(OptionType optionType)
        {
            switch (optionType)
            {
                case OptionType.Radio:
                    return new RadioAnswerOption(gameObject, radioButtonPrefab);
                case OptionType.Checkbox:
                    return new CheckBoxAnswerOption(gameObject, checkBoxPrefab);
                case OptionType.Textbox:
                    return new TextAnswerOption(gameObject, textInputPrefab);
                case OptionType.Error:
                default:
                    throw new System.ArgumentException("Invalid option type: " + optionType);
            }
        }

        public void SetIfyes(bool ifYes, int ifYesTrigger)
        {
            this.ifYes = ifYes;
            this.ifYesTrigger = ifYesTrigger;
        }

        public void ChangeIfyesState(int optionNumber, bool value)
        {
            Debug.Log("option number:" + optionNumber);
            answerOptionStates[optionNumber] = value;
            if (answerOptionStates[ifYesTrigger])
            {
                foreach (GameObject ifyesQuestion in ifYesQuestions)
                {
                    ifyesQuestion.SetActive(true);
                }
                LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)gameObject.transform.parent);
            }
            else
            {
                foreach (GameObject ifYesQuestion in ifYesQuestions)
                {
                    ifYesQuestion.SetActive(false);
                }
            }
        }
    }
}
