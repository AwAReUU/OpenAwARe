using System.Reflection;

using AwARe.Questionnaire.Data;

using UnityEngine;
using UnityEngine.Serialization;

namespace AwARe.Questionnaire.Objects
{
    public class QuestionnaireConstructor : MonoBehaviour
    {
        [SerializeField] private GameObject questionnairePrefab;
        [SerializeField] private Transform subcanvas;
        [SerializeField] private GameObject questionnaireTemplate;
        [SerializeField] private TextAsset jsonFile;

        /// <value>
        /// JSON data of which a questionnnaire can be created.
        /// </value>
        private QuestionnaireData Data { get; set; }

        //currently, start only loads the test questionnaire from a json file
        //in the future, this obviously has to change
        private void Start()
        {
            QuestionnaireFromJsonString(jsonFile.text);
        }

        /// <summary>
        /// Convert <paramref name="jsonText"/> to data object and creates a questionnaire out of it.
        /// </summary>
        /// <returns>A questionnaire gameobject.</returns>
        public GameObject QuestionnaireFromJsonString(string jsonText)
        {
            Data = JsonUtility.FromJson<QuestionnaireData>(jsonText);
            return Data == null ? null : MakeQuestionnaire(Data);
        }
        public GameObject QuestionnaireFromJsonString()
        {
            Data = JsonUtility.FromJson<QuestionnaireData>(jsonFile.text);
            return Data == null ? null : MakeQuestionnaire(Data);
        }

        /// <summary>
        /// Makes a questionnaire object and returns it.
        /// </summary>
        /// <param name="questionnaireData">deserialized questionnaire data from a json.</param>
        /// <returns>A questionnaire gameobject.</returns>
        private GameObject MakeQuestionnaire(QuestionnaireData questionnaireData)
        {
            GameObject questionnaireObject = Instantiate(questionnaireTemplate, gameObject.transform, false);
            questionnaireObject.SetActive(true);

            Questionnaire questionnaireScript = questionnaireObject.gameObject.GetComponent<Questionnaire>();
            questionnaireScript.SetTitle(questionnaireData.questionnaireTitle);
            questionnaireScript.SetDescription(questionnaireData.questionnaireDescription);

            foreach(QuestionData question in questionnaireData.questions)
                questionnaireScript.AddQuestion(question);

            return questionnaireObject;
        }

        public TextAsset GetJsonFile() => jsonFile;
        public GameObject GetQuestionnaireTemplate() => questionnaireTemplate;
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
        void Start() { }

        /// <summary>
        /// Initializes private fields inside the QuestionnaireConstructor using reflection. 
        /// If a value is provided for a field, it is set directly. 
        /// If no value is provided, the value from the QuestionnaireConstructor instance is used.
        /// </summary>
        /// <param name="jsonTextAsset">Optional: TextAsset containing JSON data for the questionnaire.</param>
        /// <param name="template">Optional: GameObject template for the questionnaire.</param>
        public void InitializeFields(TextAsset jsonTextAsset = null, GameObject template = null)
        {
            FieldInfo jsonFileField = typeof(QuestionnaireConstructor).
                GetField("jsonFile", BindingFlags.Instance | BindingFlags.NonPublic);
            if (jsonFileField != null)
                jsonFileField.SetValue(this, jsonTextAsset != null ? jsonTextAsset : GetJsonFile());
            else
                Debug.LogError("Field 'jsonFile' not found in QuestionnaireConstructor.");

            FieldInfo templateField = typeof(QuestionnaireConstructor).
                GetField("questionnaireTemplate", BindingFlags.Instance | BindingFlags.NonPublic);
            if (templateField != null)
                templateField.SetValue(this, template != null ? template : GetQuestionnaireTemplate());
            else
                Debug.LogError("Field 'questionnaireTemplate' not found in QuestionnaireConstructor.");
        }
    }
}