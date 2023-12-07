//using System.Collections.Generic;

//using TMPro;

//using UnityEngine;
//using UnityEngine.UI;

//namespace AwARe.Questionnaire.Objects
//{
//    public class Question : MonoBehaviour
//    {
//        private ToggleGroup radiobuttonGroup;

//        //'Template' objects are instantiated when a new answer option is created for the question
//        //if more answer options are added in the future, they should have their own 'Template' object
//        [SerializeField]
//        private GameObject textinputTemplate;
//        [SerializeField]
//        private GameObject checkboxTemplate;
//        [SerializeField]
//        private GameObject radiobuttonTemplate;

//        //The title of the Question object holds the actual question itself, e.g., 'how was your day?'
//        [SerializeField]
//        private GameObject title;

//        //index of the answer option that will activate the 'if yes' questions
//        public int ifyesTrigger { get; private set; }

//        public bool ifyes { get; private set; }
//        public List<GameObject> ifyesQuestions;

//        public List<GameObject> answerOptions;
//        //used to decide if the ifyesQuestions should be displayed or not
//        private List<bool> answerOptionStates;
//        private int answerOptionNumberCounter;

//        void Awake()
//        {
//            answerOptions = new List<GameObject>();
//            answerOptionStates = new List<bool>();
//            gameObject.AddComponent<ToggleGroup>();
//            radiobuttonGroup = gameObject.GetComponent<ToggleGroup>();
//            radiobuttonGroup.allowSwitchOff = true;
//        }

//        //hide or reveal the 'ifyes' questions linked to this question depending
//        //on the state of the answer options linked to this question
//        //this method should be called only from answer option handlers
//        public void ChangeIfyesState(int optionNumber, bool value)
//        {
//            Debug.Log("option number:" + optionNumber);
//            answerOptionStates[optionNumber] = value;
//            if (answerOptionStates[ifyesTrigger])
//            {
//                foreach(GameObject ifyesQuestion in ifyesQuestions)
//                {
//                    ifyesQuestion.SetActive(true);
//                }
//                //fixes a bug that causes all the questions to become stacked the first time they are shown
//                LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)gameObject.transform.parent);
//            }
//            else
//            {
//                foreach (GameObject ifyesQuestion in ifyesQuestions)
//                {
//                    ifyesQuestion.SetActive(false);
//                }
//            }
//        }

//        public void SetTitle(string questionTitle)
//        {
//            title.GetComponent<TextMeshProUGUI>().text = questionTitle;
//        }

//        public void SetIfyes(bool ifyes, int ifyesTrigger)
//        {
//            this.ifyes = ifyes;
//            this.ifyesTrigger = ifyesTrigger;
//        }

//        //the below 'Add' methods add an answer option of a specific type.
//        //If more answer options are added in the future, they should have their own 'Add' method
//        public void AddTextinput(string placeholdertext)
//        {
//            var inputfield = Instantiate(textinputTemplate);
//            inputfield.SetActive(true);
//            inputfield.transform.SetParent(gameObject.transform);
//            answerOptions.Add(inputfield);

//            //'if yes' related
//            answerOptionStates.Add(false);

//            inputfield.transform.Find("Text Area/Placeholder").GetComponent<TextMeshProUGUI>().text = placeholdertext;
//        }

//        public void AddCheckbox(string labeltext)
//        {
//            var checkbox = Instantiate(checkboxTemplate);
//            checkbox.SetActive(true);
//            checkbox.transform.SetParent(gameObject.transform);
//            answerOptions.Add(checkbox);

//            //'if yes' related
//            checkbox.GetComponent<ToggleHandler>().setQuestion(gameObject);
//            checkbox.GetComponent<ToggleHandler>().setNumber(ref answerOptionNumberCounter);
//            answerOptionStates.Add(false);

//            checkbox.transform.Find("Label").GetComponent<TextMeshProUGUI>().text = labeltext;
//        }

//        public void AddRadiobutton(string labeltext)
//        {
//            var button = Instantiate(radiobuttonTemplate);
//            button.SetActive(true);
//            button.transform.SetParent(gameObject.transform);
//            answerOptions.Add(button);

//            //'if yes' related
//            button.GetComponent<ToggleHandler>().setQuestion(gameObject);
//            button.GetComponent<ToggleHandler>().setNumber(ref answerOptionNumberCounter);
//            answerOptionStates.Add(false);

//            button.GetComponent<Toggle>().group = radiobuttonGroup;
//            button.transform.Find("Label").GetComponent<TextMeshProUGUI>().text = labeltext;
//        }
//    }
//}
