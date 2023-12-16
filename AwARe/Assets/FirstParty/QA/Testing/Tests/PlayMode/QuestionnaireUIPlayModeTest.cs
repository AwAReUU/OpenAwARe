using AwARe.Questionnaire.Data;
using AwARe.Questionnaire.Objects;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class QuestionnaireTests
{
    private QuestionnaireConstructor questionnaireConstructor;

    [OneTimeSetUp, Description("Load the test scene once.")]
    public void OneTimeSetup() => SceneManager.LoadScene("FirstParty/Application/Scenes/AppScenes/Questionnaire");

    [UnitySetUp, Description("Reset the scene before each test. Obtain the questionnaireConstructor")]
    public IEnumerator Setup()
    {
        yield return null; //skip one frame to ensure the scene has been loaded.
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        yield return null; //skip one frame to ensure the scene has been reloaded.
        questionnaireConstructor = GameObject.Find("QuestionnaireConstructor").GetComponent<QuestionnaireConstructor>();
        //TextAsset testFormat = Resources.Load<TextAsset>("Data/Questionnaire/Testformat");
    }


    [Test, Description("Test whether the creation of an empty question inside of a questionnaire works.")]
    public void Test_QuestionCreation()
    {
        //Arrange: Setup a questionnaire.
        GameObject emptyQuestionnaire = Object.Instantiate(
            Resources.Load<GameObject>("Prefabs/MainFeatures/Questionnaire/Questionnaire"));
        QuestionData data = new QuestionData
        {
            questionTitle = "Test title",
            answerOptions = new List<AnswerOptionData>()
        };
        Questionnaire questionnaire = emptyQuestionnaire.GetComponent<Questionnaire>();

        //Act: Add the question.
        GameObject question = questionnaire.AddQuestion(data);

        //Assert: Is the first question equal to the created question?
        Assert.IsTrue(emptyQuestionnaire.transform.Find("ScrollWindow/Content").GetChild(0).gameObject == question);
    }
}

/// <summary>
/// This class allows us to insert a custom json text file to use before constructing a questionnaire.
/// </summary>
public class QuestionnaireMockJsonTests
{
    private MockQuestionnaireConstructor mockQuestionnaireConstructor;
    private TextAsset testFormat;
    private GameObject questionnairePrefab;

    [OneTimeSetUp, Description("Load the test scene once.")]
    public void OneTimeSetup()
    {
        //Use anything but Questionnaire scene, since that scene already contains a questionnaire constructor.
        SceneManager.LoadScene("FirstParty/Application/Scenes/AppScenes/AR");
        testFormat = Resources.Load<TextAsset>("Data/Questionnaire/Testformat");
        questionnairePrefab = Resources.Load<GameObject>("Prefabs/MainFeatures/Questionnaire/Questionnaire");
    }

    [UnitySetUp, Description("Reset the scene before each test. Obtain the questionnaireConstructor")]
    public IEnumerator Setup()
    {
        yield return null; //skip one frame to ensure the scene has been loaded.
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        yield return null; //skip one frame to ensure the scene has been reloaded.
        GameObject gameObject = new GameObject("MockQuestionnaireConstructor");
        gameObject.AddComponent<MockQuestionnaireConstructor>();
        mockQuestionnaireConstructor = gameObject.GetComponent<MockQuestionnaireConstructor>();
    }

    [UnityTest, Description("Check whether an empty json object crashes the program.")]
    public IEnumerator Test_EmptyJsonObject_NoCrash()
    {
        TextAsset newText = new("{}");
        mockQuestionnaireConstructor.InitializeFields(newText, questionnairePrefab);
        GameObject questionnaire = mockQuestionnaireConstructor.QuestionnaireFromJsonString();
        Assert.IsNotNull(questionnaire);
        yield return null;
    }

    //TODO: Improve our error handling so this does not fail.
    //[UnityTest, Description("Check whether a completely empty file crashes the program.")]
    //public IEnumerator Test_EmptyStringJson_NoCrash()
    //{
    //    TextAsset newText = new("");
    //    mockQuestionnaireConstructor.InitializeFields(newText, questionnairePrefab);
    //    GameObject questionnaire = mockQuestionnaireConstructor.QuestionnaireFromJsonString();
    //    Assert.IsNotNull(questionnaire);
    //    yield return null;
    //}

    [UnityTest, Description("Test whether the questions are stored in the right order.")]
    public IEnumerator Test_ValidQuestionOrder()
    {
        //Arrange: Create a questionnaire with questions from the testFormat.
        GameObject[] questions = new GameObject[8];
        mockQuestionnaireConstructor.InitializeFields(testFormat, questionnairePrefab);
        GameObject questionnaire = mockQuestionnaireConstructor.QuestionnaireFromJsonString();
        string[] titles =
        {
            "How you doing?",
            "Which of the following is true?",
            "Type something in the box",
            "How you doing?",
            "Are you human?",
            "What kind of human?",
            "What is your favorite color?",
            "Are you sure you are a human?"
        };

        //Act: Store the titles in order in an array.
        for (int i = 0; i < questions.Length; i++)
            questions[i] = questionnaire.transform.Find("ScrollWindow/Content").GetChild(i).gameObject;

        //Assert: Check if all questions are in the same order as in the json.
        for (int i = 0; i < questions.Length; i++)
            Assert.IsTrue(questions[i].GetComponent<Question>().GetTitle() == titles[i]);

        return null;

        //TODO: this, but inside seperate test.
        ////check if each question's answer options are in order
        //for (int i = 0; i < questions.Length; i++)
        //{
        //    GameObject question = questions[i];

        //    for (int j = 1; j < question.transform.childCount; j++)
        //    {
        //        //Assert.IsTrue(question.transform.GetChild(j).gameObject.GetComponent<OptionInfoHolder>().GetOptionNumber() == j - 1);
        //    }
        //}
    }

    [UnityTest, Description("Test whether all checkboxes are created. (And not too many!)")]
    public IEnumerator Test_CheckBoxCreation()
    {
        //Arrange: Create constructor
        mockQuestionnaireConstructor.InitializeFields(testFormat, questionnairePrefab);
        //Act: Initialize the questions.
        GameObject _ = mockQuestionnaireConstructor.QuestionnaireFromJsonString();
        yield return null;
        //Act: Count the checkBoxes.
        GameObject[] checkboxes = GameObject.FindGameObjectsWithTag("CheckBox");
        //Assert: Ensure the right amount of checkBoxes have been generated.
        Assert.True(checkboxes.Length == 3);
    }

    [UnityTest, Description("Test whether all radioButtons are created. (And not too many!)")]
    public IEnumerator Test_RadioButtonCreation()
    {
        //Arrange: Create constructor
        mockQuestionnaireConstructor.InitializeFields(testFormat, questionnairePrefab);
        //Act: Initialize the questions.
        GameObject _ = mockQuestionnaireConstructor.QuestionnaireFromJsonString();
        yield return null;
        //Act: Count the radioButtons.
        GameObject[] radioButtons = GameObject.FindGameObjectsWithTag("RadioButton");
        //Assert: Ensure the right amount of radioButtons have been generated.
        Assert.True(radioButtons.Length == 10);
    }

    [UnityTest, Description("Test whether all inputFields are created. (And not too many!)")]
    public IEnumerator Test_InputFieldCreation()
    {
        //Arrange: Create constructor
        mockQuestionnaireConstructor.InitializeFields(testFormat, questionnairePrefab);
        //Act: Initialize the questions.
        GameObject _ = mockQuestionnaireConstructor.QuestionnaireFromJsonString();
        yield return null;
        //Act: Count the inputFields.
        GameObject[] inputFields = GameObject.FindGameObjectsWithTag("InputField");
        //Assert: Ensure the right amount of inputFields have been generated.
        Assert.True(inputFields.Length == 1);
    }

    [UnityTest, Description("Tests whether IfYes questions can be revealed using the TriggerIndex")]
    public IEnumerator Test_ToggleIfYes()
    {
        //Arrange: Create constructor
        mockQuestionnaireConstructor.InitializeFields(testFormat, questionnairePrefab);
        GameObject questionnaireGameObject = mockQuestionnaireConstructor.QuestionnaireFromJsonString();
        yield return null;
        //Act: activate the IfYes questions.
        Questionnaire questionnaire = questionnaireGameObject.GetComponent<Questionnaire>();
        List<GameObject> questions = questionnaire.GetQuestions();
        questions[4].GetComponent<Question>().ChangeIfYesState(0, true);
        GameObject[] radioButtons = GameObject.FindGameObjectsWithTag("RadioButton");
        //Assert: Ensure that the radioButtons of the IfYes questions have been generated by counting them.
        Assert.True(radioButtons.Length == 17);
    }
}