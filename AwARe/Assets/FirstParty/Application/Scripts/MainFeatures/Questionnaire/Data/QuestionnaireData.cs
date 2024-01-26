// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

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

        /// <summary>
        /// The list of responses a user has given to this question.
        /// </summary>
        public List<AnswerData> UserAnswers;
    }

    /// <summary>
    /// Contains data to save about a single answer.
    /// </summary>
    [Serializable]
    public class AnswerData
    {
        /// <summary>
        /// The title of the question
        /// </summary>
        public string QuestionTitle;

        /// <summary>
        /// The text of the given anwer
        /// </summary>
        public string AnswerText;

        /// <summary>
        /// The index of the selected answer option (-1 if none selected for radio/checkbox)
        /// </summary>
        public int SelectedAnswerIndex;
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