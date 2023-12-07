//intermediary data holder classes

using System;
using System.Collections.Generic;

namespace AwARe.Questionnaire.Objects
{

    /// <summary>
    /// Contains a title, description and a list of questions.
    /// </summary>
    [Serializable]
    public class QuestionnaireData
    {
        public string questionnaireTitle;
        public string questionnaireDescription;
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
        public string optiontype;
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
        public string questionTitle;
        public bool ifYes;
        public int ifYesTrigger;
        public List<QuestionData> ifYesQuestions;
        public List<AnswerOptionData> answerOptions;
    }

    //public enum  optionType
    //{
    //    radio,
    //    checkBox,
    //    textBox
    //}
}