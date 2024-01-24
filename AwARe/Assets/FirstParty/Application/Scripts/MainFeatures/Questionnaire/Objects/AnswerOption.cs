// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace AwARe.Questionnaire.Objects
{
    /// <summary>
    /// Class <c>AnswerOption</c> is a base class for creation and initializing answer option objects.
    /// </summary>
    public abstract class AnswerOption
    {
        /// <summary>
        /// Gets or sets the Question to add the answer option to.
        /// </summary>
        /// <value>
        /// The Question to add the answer option to.
        /// </value>
        protected GameObject Question { get; set; }

        /// <summary>
        /// The Answeroption prefab to be spawned. (Radio/Checkbox/Input/...)
        /// </summary>
        protected readonly GameObject answerOptionPrefab;

        /// <summary>
        /// Initializes a new instance of the <see cref="AnswerOption"/> class.
        /// </summary>
        /// <param name="question">Parent container.</param>
        /// <param name="answerOptionPrefab">Prefab for the AnswerOption to be generated.</param>
        protected AnswerOption(GameObject question, GameObject answerOptionPrefab)
        {
            Question = question;
            this.answerOptionPrefab = answerOptionPrefab;
        }

        /// <summary>
        /// Create an answer option using the given title. The implementation is defined in the subclasses.
        /// </summary>
        /// <param name="title">The title of the question (The question itself).</param>
        /// <returns>An answer option GameObject.</returns>
        public abstract GameObject InstantiateAnswerOption(string title);

        /// <summary>
        /// Initializes some common properties of an answer option GameObject.
        /// </summary>
        /// <param name="option">The answer option GameObject to initialize.</param>
        /// <param name="labelText">The text to display on the answer option.</param>
        protected void InitializeAnswerOption(GameObject option, string labelText)
        {
            option.SetActive(true);
            option.transform.SetParent(Question.transform, false);

            if (option.GetComponent<ToggleHandler>() == null)
                return;
            option.transform.Find("Label").GetComponent<TextMeshProUGUI>().text = labelText;
        }

        /// <summary>
        /// Initializes the <see cref="ToggleHandler"/> for the given answerOption. 
        /// </summary>
        /// <param name="answerOption">The answerOption to add the togglehandler to.</param>
        /// <param name="answerOptionIndex">The index that the new answerOption will get.</param>
        protected void InitializeToggleHandler(GameObject answerOption, int answerOptionIndex)
        {
            if (!answerOption.TryGetComponent(out ToggleHandler toggleHandler))
                return;
            toggleHandler.SetQuestion(Question);
            toggleHandler.AssignIndex(answerOptionIndex);
        }
    }

    /// <summary>
    /// Represents an answer option that displays a text input field.
    /// </summary>
    public class TextAnswerOption : AnswerOption
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
        /// <param name="placeholderText">Placeholder text will be present in the field before the user answers.</param>
        /// <returns>An answerOption GameObject.</returns>
        public override GameObject InstantiateAnswerOption(string placeholderText)
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
    public class RadioAnswerOption : AnswerOption
    {
        private readonly ToggleGroup radiobuttonGroup;
        private readonly int answerOptionIndex;

        /// <summary>
        /// Initializes a new instance of the <see cref="RadioAnswerOption"/> class.
        /// </summary>
        /// <param name="questionGameObject">The parent questionGameObject that owns the answer option.</param>
        /// <param name="questionPrefab">The prefab used to create answer option GameObjects.</param>
        /// <param name="currentAnswerOptionIndex">The index that this specific answer option will have.</param>
        public RadioAnswerOption(GameObject questionGameObject, GameObject questionPrefab, int currentAnswerOptionIndex)
            : base(questionGameObject, questionPrefab)
        {
            answerOptionIndex = currentAnswerOptionIndex;

            if (questionGameObject.TryGetComponent(out radiobuttonGroup))
                return;
            radiobuttonGroup = questionGameObject.AddComponent<ToggleGroup>();
            radiobuttonGroup.allowSwitchOff = true;
        }

        /// <summary>
        /// Create a single RadioAnswerOption GameObject.
        /// </summary>
        /// <param name="labelText">Text to put next to the RadioButton.</param>
        /// <returns>An answerOption GameObject.</returns>
        public override GameObject InstantiateAnswerOption(string labelText)
        {
            GameObject radioButton = Object.Instantiate(answerOptionPrefab);
            radioButton.GetComponent<Toggle>().group = radiobuttonGroup;
            InitializeAnswerOption(radioButton, labelText);
            InitializeToggleHandler(radioButton, answerOptionIndex);
            radioButton.tag = "RadioButton";
            return radioButton;
        }
    }
    /// <summary>
    /// Represents an answer option that displays CheckBoxes.
    /// </summary>
    public class CheckBoxAnswerOption : AnswerOption
    {
        private readonly int answerOptionIndex;

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckBoxAnswerOption"/> class.
        /// </summary>
        /// <param name="questionGameObject">The parent questionGameObject that owns the answer option.</param>
        /// <param name="questionPrefab">The prefab used to create answer option GameObjects.</param>
        /// <param name="currentAnswerOptionIndex">The index that this specific answer option will have.</param>
        public CheckBoxAnswerOption(GameObject questionGameObject, GameObject questionPrefab, int currentAnswerOptionIndex)
            : base(questionGameObject, questionPrefab)
        {
            answerOptionIndex = currentAnswerOptionIndex;
        }

        /// <summary>
        /// Create a single CheckBoxAnswerOption GameObject.
        /// </summary>
        /// <param name="labelText">Text to put next to the CheckBox.</param>
        /// <returns>An AnswerOption GameObject.</returns>
        public override GameObject InstantiateAnswerOption(string labelText)
        {
            GameObject checkbox = Object.Instantiate(answerOptionPrefab);
            InitializeAnswerOption(checkbox, labelText);
            InitializeToggleHandler(checkbox, answerOptionIndex);
            checkbox.tag = "CheckBox";
            return checkbox;
        }
    }
}
