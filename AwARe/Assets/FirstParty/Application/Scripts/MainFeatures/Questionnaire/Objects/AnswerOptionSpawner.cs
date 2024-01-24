// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

using UnityEngine;

namespace AwARe.Questionnaire.Objects
{
    /// <summary>
    /// Class <c>AnswerOptionSpawner</c> is responsible for creating different types of answer options. Note that this
    /// class does not add the answerOption to the question, it is only used for creation.
    /// </summary>
    public class AnswerOptionSpawner
    {
        private readonly GameObject question;
        private readonly GameObject textOptionPrefab;
        private readonly GameObject checkBoxOptionPrefab;
        private readonly GameObject radioOptionPrefab;

        /// <summary>
        /// Initializes a new instance of the <see cref="AnswerOptionSpawner"/> class.
        /// </summary>
        /// <param name="question">The question to which the answerOptions should be added.</param>
        /// <param name="textOptionPrefab">Prefab for creating text answerOptions.</param>
        /// <param name="checkBoxOptionPrefab">Prefab for creating checkBox answerOptions.</param>
        /// <param name="radioOptionPrefab">Prefab for creating radioButton answerOptions.</param>
        public AnswerOptionSpawner(
            GameObject question,
            GameObject textOptionPrefab,
            GameObject checkBoxOptionPrefab,
            GameObject radioOptionPrefab)
        {
            this.question = question;
            this.textOptionPrefab = textOptionPrefab;
            this.checkBoxOptionPrefab = checkBoxOptionPrefab;
            this.radioOptionPrefab = radioOptionPrefab;
        }

        /// <summary>
        /// Create a text answerOption for the question.
        /// </summary>
        /// <returns>A <see cref="TextAnswerOption"/> instance.</returns>
        public TextAnswerOption CreateTextAnswerOption() =>
            new(question, textOptionPrefab);

        /// <summary>
        /// Create a radioButton answerOption for the question.
        /// </summary>
        /// <param name="currentAnswerOptionIndex">Index that the answerOption will have.</param>
        /// <returns>A <see cref="RadioAnswerOption"/> instance.</returns>
        public RadioAnswerOption CreateRadioAnswerOption(int currentAnswerOptionIndex) =>
            new(question, radioOptionPrefab, currentAnswerOptionIndex);

        /// <summary>
        /// Create a checkBox answerOption for the question.
        /// </summary>
        /// <param name="currentAnswerOptionIndex">Index that the answerOption will have.</param>
        /// <returns>A <see cref="CheckBoxAnswerOption"/> instance.</returns>
        public CheckBoxAnswerOption CreateCheckBoxAnswerOption(int currentAnswerOptionIndex) =>
            new(question, checkBoxOptionPrefab, currentAnswerOptionIndex);
    }
}
