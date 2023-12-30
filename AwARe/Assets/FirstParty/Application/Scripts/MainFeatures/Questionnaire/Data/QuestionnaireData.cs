using System;
using System.Collections.Generic;

namespace AwARe.Questionnaire.Data
{
    /// <summary>
    /// Represents a questionnaire with a title, description, and a list of questions.
    /// </summary>
    [Serializable]
    public class QuestionnaireData
    {
        /// <value>
        /// The title of the questionnaire.
        /// </value>
        public string questionnaireTitle;
        /// <value>
        /// The description of the questionnaire.
        /// </value>
        public string questionnaireDescription;
        /// <value>
        /// The list of questions in the questionnaire.
        /// </value>
        public List<QuestionData> questions;
    }

    /// <summary>
    /// Contains information about the answer
    /// </summary>
    [Serializable]
    public class AnswerOptionData
    {
        /// <value>
        /// The type of the question (radio, checkbox, textbox).
        /// </value>
        public string optionType;
        /// <value>
        /// The text next to the answer option.
        /// </value>
        public string optionText;
    }

    /// <summary>
    /// Contains all data about a single question.
    /// </summary>
    [Serializable]
    public class QuestionData
    {
        /// <value>
        /// The title of the question. (Contains the question itself).
        /// </value>
        public string questionTitle;
        /// <value>
        /// Indicates whether this question triggers additional questions.
        /// </value>
        public bool ifYes;
        /// <value>
        /// The index of the answer option that triggers additional questions when selected.
        /// </value>
        public int ifYesTrigger;
        /// <value>
        /// The list of questions to be shown if "Yes" is selected.
        /// </value>
        public List<QuestionData> ifYesQuestions;
        /// <value>
        /// The list of answer options for the question.
        /// </value>
        public List<AnswerOptionData> answerOptions;
    }

    /// <summary>
    /// Enumerates all available question types.
    /// </summary>
    public enum OptionType
    {
        Radio,
        Checkbox,
        Textbox,
        Error
    }
}