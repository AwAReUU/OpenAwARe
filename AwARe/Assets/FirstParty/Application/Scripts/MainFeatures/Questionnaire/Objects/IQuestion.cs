
using System.Collections.Generic;
using TMPro;

using UnityEngine;
using UnityEngine.UI;

namespace AwARe.Questionnaire.Objects
{
    public class QuestionCreator : MonoBehaviour
    {
        /// <value>
        /// The title of the Question object holds the actual question itself.
        /// </value>
        [SerializeField] private GameObject titleContainer;

        private List<GameObject> answerOptions;
        //used to decide if the ifyesQuestions should be displayed or not
        protected List<bool> answerOptionStates;
        protected int answerOptionNumberCounter;

        //index of the answer option that will activate the 'if yes' questions
        public int ifYesTrigger { get; private set; }
        public bool ifYes { get; private set; }
        public List<GameObject> ifYesQuestions;

        void Awake()
        {
            answerOptions = new List<GameObject>();
            answerOptionStates = new List<bool>();
        }

        /// <summary>
        /// Set the title of this question to <see cref="questionTitle"/>
        /// </summary>
        /// <param name="questionTitle"></param>
        public void SetTitle(string questionTitle) =>
            titleContainer.GetComponent<TextMeshProUGUI>().text = questionTitle;

        public void AddAnswerOption(AnswerOptionData answerOptionData)
        {
            GameObject newOption = new();
            if (answerOptionData.optiontype == "radio")
            {
                RadioAnswerOption answerOption = new RadioAnswerOption();
                newOption = answerOption.GetAnswerOption(answerOptionData.optionText);
            }
            else if (answerOptionData.optiontype == "checkbox")
            {
                CheckBoxAnswerOption answerOption = new CheckBoxAnswerOption();
                 newOption = answerOption.GetAnswerOption(answerOptionData.optionText);
            }
            else if (answerOptionData.optiontype == "textbox")
            {
                TextAnswerOption answerOption = new TextAnswerOption();
                newOption = answerOption.GetAnswerOption(answerOptionData.optionText);
            }

            answerOptions.Add(newOption);
        }

        public void SetIfyes(bool ifYes, int ifYesTrigger)
        {
            this.ifYes = ifYes;
            this.ifYesTrigger = ifYesTrigger;
        }
        //hide or reveal the 'ifyes' questions linked to this question depending
        //on the state of the answer options linked to this question
        //this method should be called only from answer option handlers
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
                //fixes a bug that causes all the questions to become stacked the first time they are shown
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

    public abstract class AnswerOption : MonoBehaviour
    {
        public abstract GameObject GetAnswerOption(string title);

        protected void InitializeAnswerOption(GameObject option, string labelText)
        {
            option.SetActive(true);
            option.transform.SetParent(gameObject.transform);
            //answerOptions.Add(option);

            //'if yes' related
            option.GetComponent<ToggleHandler>().setQuestion(gameObject);
            //option.GetComponent<ToggleHandler>().setNumber(ref answerOptionNumberCounter);
            //answerOptionStates.Add(false);

            option.transform.Find("Label").GetComponent<TextMeshProUGUI>().text = labelText;
        }
    }

    public class TextAnswerOption : AnswerOption
    {
        [SerializeField] private GameObject textInputPrefab;

        public override GameObject GetAnswerOption(string placeholderText)
        {
            GameObject inputField = Instantiate(textInputPrefab);
            InitializeAnswerOption(inputField, placeholderText);
            inputField.transform.Find("Text Area/Placeholder").GetComponent<TextMeshProUGUI>().text =
                placeholderText;
            return inputField;
        }
    }

    public class RadioAnswerOption : AnswerOption
    {
        [SerializeField] private GameObject radioButtonPrefab;
        private ToggleGroup radiobuttonGroup;

        void Awake()
        {
            gameObject.AddComponent<ToggleGroup>();
            radiobuttonGroup = gameObject.GetComponent<ToggleGroup>();
            radiobuttonGroup.allowSwitchOff = true;
        }

        public override GameObject GetAnswerOption(string labelText)
        {
            GameObject radioButton = Instantiate(radioButtonPrefab);
            radioButton.GetComponent<Toggle>().group = radiobuttonGroup;
            InitializeAnswerOption(radioButton, labelText);
            radioButton.transform.Find("Label").GetComponent<TextMeshProUGUI>().text = labelText;
            return radioButton;
       
        }
    }

    public class CheckBoxAnswerOption : AnswerOption
    {
        [SerializeField] private GameObject checkBoxPrefab;

        public override GameObject GetAnswerOption(string labelText)
        {
            GameObject checkbox = Instantiate(checkBoxPrefab);
            InitializeAnswerOption(checkbox, labelText);
            checkbox.transform.Find("Label").GetComponent<TextMeshProUGUI>().text = labelText;
            return checkbox;
        }
    }
}