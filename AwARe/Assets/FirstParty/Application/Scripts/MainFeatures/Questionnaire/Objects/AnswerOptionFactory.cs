using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace AwARe.Questionnaire.Objects
{
    /// <summary>
    /// Class <c>AnswerOptionFactory</c> is a base class for creation answer option objects.
    /// </summary>
    public abstract class AnswerOptionSpawner
    {
        protected GameObject question;
        protected GameObject answerOptionPrefab;

        /// <summary>
        /// Constructor. Used to create a new <see cref="AnswerOptionSpawner"/>.
        /// </summary>
        /// <param name="question">Parent container.</param>
        /// <param name="anwerOptionPrefab">Prefab for the AnwerOption to be generated.</param>
        protected AnswerOptionSpawner(GameObject question, GameObject answerOptionPrefab)
        {
            this.question = question;
            this.answerOptionPrefab = answerOptionPrefab;
        }

        /// <summary>
        /// Create an answer option using the given title. The implementation is defined in the subclasses.
        /// </summary>
        /// <param name="title">The title of the question. (The question itself)</param>
        /// <returns>An answer option GameObject.</returns>
        public abstract GameObject CreateAnswerOption(string title);

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

        protected void AddToToggleHandler(GameObject option, int answerOptionIndex)
        {
            ToggleHandler toggleHandler = option.GetComponent<ToggleHandler>();
            if (toggleHandler == null) return;
            toggleHandler.SetQuestion(question);
            toggleHandler.AssignIndex(answerOptionIndex);
        }
    }

    /// <summary>
    /// Represents an answer option that displays a text input field.
    /// </summary>
    public class TextAnswerOption : AnswerOptionSpawner
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
        public override GameObject CreateAnswerOption(string placeholderText)
        {
            GameObject inputField = Object.Instantiate(answerOptionPrefab);
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
    public class RadioAnswerOption : AnswerOptionSpawner
    {
        private readonly ToggleGroup radiobuttonGroup;
        private readonly int thisAnswerOptionIndex;

        /// <summary>
        /// Initializes a new instance of the <see cref="RadioAnswerOption"/> class.
        /// </summary>
        /// <param name="questionGameObject">The parent questionGameObject that owns the answer option.</param>
        /// <param name="questionPrefab">The prefab used to create answer option GameObjects.</param>
        /// <param name="answerOptionIndex">The index that this specific answer option will have.</param>
        public RadioAnswerOption(GameObject questionGameObject, GameObject questionPrefab, int answerOptionIndex)
            : base(questionGameObject, questionPrefab)
        {
            thisAnswerOptionIndex = answerOptionIndex;

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
        public override GameObject CreateAnswerOption(string labelText)
        {
            GameObject radioButton = Object.Instantiate(answerOptionPrefab);
            radioButton.GetComponent<Toggle>().group = radiobuttonGroup;
            InitializeAnswerOption(radioButton, labelText);
            AddToToggleHandler(radioButton, thisAnswerOptionIndex);
            radioButton.transform.Find("Label").GetComponent<TextMeshProUGUI>().text = labelText;
            radioButton.tag = "RadioButton";
            return radioButton;
        }
    }
    /// <summary>
    /// Represents an answer option that displays CheckBoxes.
    /// </summary>
    public class CheckBoxAnswerOption : AnswerOptionSpawner
    {
        private readonly int thisAnswerOptionIndex;

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckBoxAnswerOption"/> class.
        /// </summary>
        /// <param name="questionGameObject">The parent questionGameObject that owns the answer option.</param>
        /// <param name="questionPrefab">The prefab used to create answer option GameObjects.</param>
        /// <param name="answerOptionIndex">The index that this specific answer option will have.</param>
        public CheckBoxAnswerOption(GameObject questionGameObject, GameObject questionPrefab, int answerOptionIndex)
            : base(questionGameObject, questionPrefab)
        {
            thisAnswerOptionIndex = answerOptionIndex;
        }

        /// <summary>
        /// Create a single CheckBoxAnswerOption GameObject.
        /// </summary>
        /// <param name="labelText">Text to put next to the CheckBox.</param>
        /// <returns></returns>
        public override GameObject CreateAnswerOption(string labelText)
        {
            GameObject checkbox = Object.Instantiate(answerOptionPrefab);
            InitializeAnswerOption(checkbox, labelText);
            AddToToggleHandler(checkbox, thisAnswerOptionIndex);
            checkbox.transform.Find("Label").GetComponent<TextMeshProUGUI>().text = labelText;
            checkbox.tag = "CheckBox";
            return checkbox;
        }
    }
}
