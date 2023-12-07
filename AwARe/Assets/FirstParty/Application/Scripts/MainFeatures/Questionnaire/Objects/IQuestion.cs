using System;
using System.Collections.Generic;

using AwARe.Questionnaire.Data;

using TMPro;
using UnityEngine;
using UnityEngine.UI;

using Object = UnityEngine.Object;

namespace AwARe.Questionnaire.Objects
{
    public class QuestionCreator : MonoBehaviour
    {
        [SerializeField] private GameObject titleContainer;
        [SerializeField] private GameObject textInputPrefab;
        [SerializeField] private GameObject checkBoxPrefab;
        [SerializeField] private GameObject radioButtonPrefab;

        private List<GameObject> answerOptions;
        protected List<bool> answerOptionStates;
        protected int answerOptionNumberCounter;

        public int ifYesTrigger { get; private set; }
        public bool ifYes { get; private set; }
        public List<GameObject> ifYesQuestions;

        void Awake()
        {
            answerOptions = new List<GameObject>();
            answerOptionStates = new List<bool>();
        }

        public void SetTitle(string questionTitle) =>
            titleContainer.GetComponent<TextMeshProUGUI>().text = questionTitle;

        public void AddAnswerOption(AnswerOptionData answerOptionData)
        {
            AnswerOption answerOption = CreateAnswerOption((OptionType)Enum.Parse(typeof(OptionType), answerOptionData.optionType));
            GameObject newOption = answerOption.GetAnswerOption(answerOptionData.optionText);
            answerOptions.Add(newOption);
        }

        private AnswerOption CreateAnswerOption(OptionType optionType)
        {
            switch (optionType)
            {
                case OptionType.Radio:
                    return new RadioAnswerOption(gameObject, radioButtonPrefab);
                case OptionType.Checkbox:
                    return new CheckBoxAnswerOption(gameObject, checkBoxPrefab);
                case OptionType.Textbox:
                    return new TextAnswerOption(gameObject, textInputPrefab);
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

    public abstract class AnswerOption
    {
        protected GameObject owner;
        protected GameObject questionPrefab; // Added field

        protected AnswerOption(GameObject owner, GameObject questionPrefab)
        {
            this.owner = owner;
            this.questionPrefab = questionPrefab;
        }

        public abstract GameObject GetAnswerOption(string title);
        protected void InitializeAnswerOption(GameObject option, string labelText)
        {
            option.SetActive(true);
            option.transform.SetParent(owner.transform);

            ToggleHandler toggleHandler = option.GetComponent<ToggleHandler>();
            if (toggleHandler != null)
            {
                toggleHandler.setQuestion(owner);
                option.transform.Find("Label").GetComponent<TextMeshProUGUI>().text = labelText;
            }
        }
    }

    public class TextAnswerOption : AnswerOption
    {
        public TextAnswerOption(GameObject owner, GameObject questionPrefab) : base(owner, questionPrefab) { }

        public override GameObject GetAnswerOption(string placeholderText)
        {
            GameObject inputField = Object.Instantiate(questionPrefab);
            InitializeAnswerOption(inputField, placeholderText);
            inputField.transform.Find("Text Area/Placeholder").GetComponent<TextMeshProUGUI>().text =
                placeholderText;
            return inputField;
        }
    }

    public class RadioAnswerOption : AnswerOption
    {
        private readonly ToggleGroup radiobuttonGroup;

        public RadioAnswerOption(GameObject owner, GameObject questionPrefab) : base(owner, questionPrefab)
        {
            radiobuttonGroup = owner.GetComponent<ToggleGroup>();
            if (radiobuttonGroup == null)
            {
                radiobuttonGroup = owner.AddComponent<ToggleGroup>();
                radiobuttonGroup.allowSwitchOff = true;
            }
        }

        public override GameObject GetAnswerOption(string labelText)
        {
            GameObject radioButton = Object.Instantiate(questionPrefab);
            radioButton.GetComponent<Toggle>().group = radiobuttonGroup;
            InitializeAnswerOption(radioButton, labelText);
            radioButton.transform.Find("Label").GetComponent<TextMeshProUGUI>().text = labelText;
            return radioButton;
        }
    }

    public class CheckBoxAnswerOption : AnswerOption
    {
        public CheckBoxAnswerOption(GameObject owner, GameObject questionPrefab) : base(owner, questionPrefab) { }

        public override GameObject GetAnswerOption(string labelText)
        {
            GameObject checkbox = Object.Instantiate(questionPrefab);
            InitializeAnswerOption(checkbox, labelText);
            checkbox.transform.Find("Label").GetComponent<TextMeshProUGUI>().text = labelText;
            return checkbox;
        }
    }
}
