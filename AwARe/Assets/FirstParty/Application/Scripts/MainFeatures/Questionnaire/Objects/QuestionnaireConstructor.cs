using System.Reflection;

using AwARe.Questionnaire.Data;

using UnityEngine;
using UnityEngine.Serialization;

namespace AwARe.Questionnaire.Objects
{
    /// <summary>
    /// Class <c>QuestionnaireConstructor</c> is used for creating a <see cref="Questionnaire"/> from a json string.
    /// </summary>
    public class QuestionnaireConstructor : MonoBehaviour
    {
        /// <value>
        /// Reference to "Subcanvas" from questionnaire prefab.
        /// </value>
        [SerializeField] private Transform subcanvas;
        /// <value>
        /// Reference to "Questionnaire" Prefab.
        /// </value>
        [SerializeField] private GameObject questionnairePrefab;
        /// <value>
        /// Reference to an input jsonFile to be used for constructing the <see cref="Questionnaire"/>.
        /// </value>
        [SerializeField] private TextAsset jsonFile;
        /// <value>
        /// Deserialized JSON data of which a <see cref="Questionnaire"/> can be created.
        /// </value>
        private QuestionnaireData Data { get; set; }

        /// <summary>
        /// Create a <see cref="Questionnaire"/> from json string.
        /// </summary>
        private void Start()
        {
            QuestionnaireFromJsonString(jsonFile.text);
        }

        /// <summary>
        /// Convert <paramref name="jsonText"/> to data object and creates a <see cref="Questionnaire"/> out of it.
        /// (Deserialization)
        /// </summary>
        /// <param name="jsonText">The json text to be deserialized.</param>
        /// <returns>A questionnaire GameObject.</returns>
        public GameObject QuestionnaireFromJsonString(string jsonText)
        {
            Data = JsonUtility.FromJson<QuestionnaireData>(jsonText);
            return Data == null ? null : MakeQuestionnaire(Data);
        }
        /// <summary>
        /// Convert Json string from the SerializeField TextAsset to data object
        /// and create a questionnaire out of it.
        /// </summary>
        /// <returns>A questionnaire GameObject.</returns>
        public GameObject QuestionnaireFromJsonString() => QuestionnaireFromJsonString(jsonFile.text);


        /// <summary>
        /// Makes a questionnaire object and returns it.
        /// </summary>
        /// <param name="questionnaireData">deserialized questionnaire data from a json.</param>
        /// <returns>A questionnaire GameObject.</returns>
        private GameObject MakeQuestionnaire(QuestionnaireData questionnaireData)
        {
            GameObject questionnaireObject = Instantiate(questionnairePrefab, subcanvas);
            questionnaireObject.SetActive(true);

            Questionnaire questionnaireScript = questionnaireObject.gameObject.GetComponent<Questionnaire>();
            questionnaireScript.SetTitle(questionnaireData.questionnaireTitle);
            questionnaireScript.SetDescription(questionnaireData.questionnaireDescription);

            foreach(QuestionData question in questionnaireData.questions)
                questionnaireScript.AddQuestion(question);

            return questionnaireObject;
        }

        /// <summary>
        /// Obtain the JsonFile from the serialize field.
        /// </summary>
        /// <returns>JsonFile obtained from the serialize field.</returns>
        public TextAsset GetJsonFile() => jsonFile;

        /// <summary>
        /// Obtain the QuestionnairePrefab from the serialize field.
        /// </summary>
        /// <returns>Questionnaire prefab GameObject obtained from the serialize field.</returns>
        public GameObject GetQuestionnairePrefab() => questionnairePrefab;
    }

    /// <summary>
    /// Class <c>MockQuestionnaireConstructor</c> is used for testing purposes. It has an empty
    /// Start, so the behaviour defined in Start() of <see cref="QuestionnaireConstructor"/> is not called.
    /// </summary>
    public class MockQuestionnaireConstructor : QuestionnaireConstructor
    {
        /// <summary>
        /// Empty Start method, so no starting code is executed.
        /// </summary>
        private void Start() { }

        /// <summary>
        /// Initializes private serialize fields from inside the QuestionnaireConstructor using reflection. 
        /// If a value is provided for a field, it is set directly. 
        /// If no value is provided, the value from the QuestionnaireConstructor instance is used.
        /// </summary>
        /// <param name="jsonTextAsset">Optional: TextAsset containing JSON data for the questionnaire.</param>
        /// <param name="questionnairePrefab">Optional: GameObject template for the questionnaire.</param>
        public void InitializeFields(TextAsset jsonTextAsset = null, GameObject questionnairePrefab = null)
        {
            FieldInfo jsonFileField = typeof(QuestionnaireConstructor).
                GetField("jsonFile", BindingFlags.Instance | BindingFlags.NonPublic);
            if (jsonFileField != null)
                jsonFileField.SetValue(this, jsonTextAsset != null ? jsonTextAsset : GetJsonFile());

            FieldInfo prefabField = typeof(QuestionnaireConstructor).
                GetField("questionnairePrefab", BindingFlags.Instance | BindingFlags.NonPublic);
            if (prefabField == null) return;
            prefabField.SetValue(this, questionnairePrefab != null ? questionnairePrefab : GetQuestionnairePrefab());
        }

       
    }
}