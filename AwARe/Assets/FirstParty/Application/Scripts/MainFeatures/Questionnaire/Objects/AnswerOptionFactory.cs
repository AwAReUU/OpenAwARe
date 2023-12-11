using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace AwARe.Questionnaire.Objects
{
    /// <summary>
    /// Class <c>AnswerOptionFactory</c> is a base class for creation answer option objects.
    /// </summary>
    public abstract class AnswerOptionFactory
    {
        protected GameObject questionGameObject;
        protected GameObject questionPrefab;

        /// <summary>
        /// Constructor. Used to create a new <see cref="AnswerOptionFactory"/>.
        /// </summary>
        /// <param name="owner">Parent container.</param>
        /// <param name="questionPrefab">Prefab containing elements needed for creating a question.</param>
        protected AnswerOptionFactory(GameObject questionGameObject, GameObject questionPrefab)
        {
            this.questionGameObject = questionGameObject;
            this.questionPrefab = questionPrefab;
        }

        /// <summary>
        /// Create an answer option using the given title. The implementation is defined in the subclasses.
        /// </summary>
        /// <param name="title">The title of the question. (The question itself)</param>
        /// <returns>An answer option GameObject.</returns>
        public abstract GameObject GetAnswerOption(string title);

        /// <summary>
        /// Initializes some common properties of an answer option GameObject.
        /// </summary>
        /// <param name="option">The answer option GameObject to initialize.</param>
        /// <param name="labelText">The text to display on the answer option.</param>
        protected void InitializeAnswerOption(GameObject option, string labelText)
        {
            option.SetActive(true);
            option.transform.SetParent(question.transform);
            
            if (option.GetComponent<ToggleHandler>() == null) return;
            option.transform.Find("Label").GetComponent<TextMeshProUGUI>().text = labelText;
        }

        protected void AddToToggleHandler(GameObject option, int answerOptionNumberCounter)
        {
            ToggleHandler toggleHandler = option.GetComponent<ToggleHandler>();
            if (toggleHandler == null)
                return;
            toggleHandler.SetQuestion(question);
            toggleHandler.AssignIndex(answerOptionNumberCounter);
        }
    }

    /// <summary>
    /// Represents an answer option that displays a text input field.
    /// </summary>
    public class TextAnswerOption : AnswerOptionFactory
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TextAnswerOption"/> class.
        /// </summary>
        /// <param name="questionGameObject">The parent questionGameObject that owns the answer option.</param>
        /// <param name="questionPrefab">The prefab used to create answer option GameObjects.</param>
        public TextAnswerOption(GameObject questionGameObject, GameObject questionPrefab)
            : base(questionGameObject, questionPrefab) { }

        /// <summary>
        /// Create a single TextAnswerOption gameObject.
        /// </summary>
        /// <param name="placeholderText">Placeholdertext will be present in the field before the user answers.</param>
        /// <returns></returns>
        public override GameObject GetAnswerOption(string placeholderText)
        {
            GameObject inputField = Object.Instantiate(questionPrefab);
            InitializeAnswerOption(inputField, placeholderText);
            inputField.transform.Find("Text Area/Placeholder").GetComponent<TextMeshProUGUI>().text =
                placeholderText;
            inputField.tag = "InputField";
            return inputField;
        }
    }

    /// <summary>
    /// Represents an answer option that displays RadioButtons.
    /// </summary>
    public class RadioAnswerOption : AnswerOptionFactory
    {
        private readonly ToggleGroup radiobuttonGroup;
        private int _answerOptionNumberCounter;

        /// <summary>
        /// Initializes a new instance of the <see cref="RadioAnswerOption"/> class.
        /// </summary>
        /// <param name="questionGameObject">The parent questionGameObject that owns the answer option.</param>
        /// <param name="questionPrefab">The prefab used to create answer option GameObjects.</param>
        public RadioAnswerOption(GameObject questionGameObject, GameObject questionPrefab, int answerOptionNumberCounter)
            : base(questionGameObject, questionPrefab)
        {
            _answerOptionNumberCounter = answerOptionNumberCounter;

            radiobuttonGroup = questionGameObject.GetComponent<ToggleGroup>();
            if (radiobuttonGroup != null) return;
            radiobuttonGroup = questionGameObject.AddComponent<ToggleGroup>();
            radiobuttonGroup.allowSwitchOff = true;
        }

        /// <summary>
        /// Create a single RadioAnswerOption GameObject.
        /// </summary>
        /// <param name="labelText">Text to put next to the RadioButton.</param>
        /// <returns></returns>
        public override GameObject GetAnswerOption(string labelText)
        {
            GameObject radioButton = Object.Instantiate(questionPrefab);
            radioButton.GetComponent<Toggle>().group = radiobuttonGroup;
            InitializeAnswerOption(radioButton, labelText);
            Debug.Log("Adding answerop " + _answerOptionNumberCounter);
            AddToToggleHandler(radioButton, _answerOptionNumberCounter);
            radioButton.transform.Find("Label").GetComponent<TextMeshProUGUI>().text = labelText;
            radioButton.tag = "RadioButton";
            return radioButton;
        }
    }
    /// <summary>
    /// Represents an answer option that displays CheckBoxes.
    /// </summary>
    public class CheckBoxAnswerOption : AnswerOptionFactory
    {
        private int _answerOptionNumberCounter;

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckBoxAnswerOption"/> class.
        /// </summary>
        /// <param name="questionGameObject">The parent questionGameObject that owns the answer option.</param>
        /// <param name="questionPrefab">The prefab used to create answer option GameObjects.</param>
        public CheckBoxAnswerOption(GameObject questionGameObject, GameObject questionPrefab, int answerOptionNumberCounter)
            : base(questionGameObject, questionPrefab)
        {
            _answerOptionNumberCounter = answerOptionNumberCounter;
        }

        /// <summary>
        /// Create a single CheckBoxAnswerOption GameObject.
        /// </summary>
        /// <param name="labelText">Text to put next to the CheckBox.</param>
        /// <returns></returns>
        public override GameObject GetAnswerOption(string labelText)
        {
            GameObject checkbox = Object.Instantiate(questionPrefab);
            InitializeAnswerOption(checkbox, labelText);
            AddToToggleHandler(checkbox, _answerOptionNumberCounter);
            checkbox.transform.Find("Label").GetComponent<TextMeshProUGUI>().text = labelText;
            checkbox.tag = "CheckBox";
            return checkbox;
        }
    }
}
