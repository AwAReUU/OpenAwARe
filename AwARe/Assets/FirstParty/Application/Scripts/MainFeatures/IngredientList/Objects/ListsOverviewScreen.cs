// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

using System;
using System.Collections.Generic;
using AwARe.InterScenes.Objects;
using UnityEngine;
using UnityEngine.UI;
using IL = AwARe.IngredientList.Logic;

namespace AwARe.IngredientList.Objects
{
    /// <summary>
    /// <para>
    ///     Handles the UI of the ingredient list overview screen.
    /// </para>
    /// <para>
    ///     Shows an overview of all <see cref="IngredientList"/>s that have been created and saved by the user.
    ///     Allows the user to select a list to edit and tick a checkbox of the list they want to be visualized in AR.
    /// </para>
    /// </summary>
    public class ListsOverviewScreen : MonoBehaviour
    {
        // (assigned within unity)
        [SerializeField] private GameObject backButton;
        [SerializeField] private IngredientListManager ingredientListManager;
        [SerializeField] private Transform scrollViewContent;
        [SerializeField] private GameObject listItemObject; //list item 'prefab'
        [SerializeField] private GameObject questionButton;
        [SerializeField] private GameObject sortingButton; 
        [SerializeField] private GameObject popupScreen;
        [SerializeField] private GameObject NottherepopupScreen;
        private Dictionary<GameObject, Button> checkButtonsDictionary = new Dictionary<GameObject, Button>();
        private Button selectedCheckButton;
        private Button selectedBorderButton;
       

        // the objects drawn on screen to display the Lists
        List<GameObject> listObjects = new();

        private void OnEnable()
        {
            Button backB = backButton.GetComponent<Button>();
            Button questionB = questionButton.GetComponent<Button>();
            Button sortingB = sortingButton.GetComponent<Button>();
            backB.onClick.AddListener(OnBackButtonClick);
            questionB.onClick.AddListener(() => ingredientListManager.PopUpOn(NottherepopupScreen));
            sortingB.onClick.AddListener(() => ingredientListManager.PopUpOn(NottherepopupScreen));
            backButton.SetActive(true);

            DisplayLists();
        }

        private void OnDisable()
        {
            Button backB = backButton.GetComponent<Button>();
            Button questionB = questionButton.GetComponent<Button>();
            Button sortingB = sortingButton.GetComponent<Button>();
            backB.onClick.RemoveAllListeners();
            questionB.onClick.RemoveAllListeners();
            sortingB.onClick.RemoveAllListeners();

            RemoveListObjects();
        }

        /// <summary>
        /// Creates GameObjects with buttons to select or destroy every ingredient list.
        /// </summary>
        public void DisplayLists()
        {
            RemoveListObjects();

            foreach (Logic.IngredientList ingredientList in ingredientListManager.Lists)
            {
                // create a new list item to display this list
                GameObject listItem = Instantiate(listItemObject, scrollViewContent);
                listItem.SetActive(true);

                // change the text to match the list info
                Button delButton = listItem.transform.GetChild(4).GetComponent<Button>();
                Button listButton = listItem.transform.GetChild(2).GetComponent<Button>();
                Button checkButton = listItem.transform.GetChild(0).GetComponent<Button>();
                Button editButton = listItem.transform.GetChild(3).GetComponent<Button>();
                Button borderButton = listItem.transform.GetChild(1).GetComponent<Button>();
                // checkbutton is first grey
                checkButton.GetComponent<Image>().color = Color.gray;


                checkButtonsDictionary.Add(listItem, checkButton);

                listButton.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text =
                    ingredientList.ListName;
                listButton.transform.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text =
                    ingredientList.NumberOfIngredients().ToString();
                

                listObjects.Add(listItem);

                // store i in an int for pass-by value to the lambda expression
                editButton.onClick.AddListener(() => OnListButtonClick(ingredientList));
                delButton.onClick.AddListener(() => OnDeleteButtonClick(ingredientList));
                checkButton.onClick.AddListener(() => OnCheckButtonClick(checkButton, borderButton, ingredientList));
            }
        }

        /// <summary>
        /// Destroys all currently displayed GameObjects in the ScrollView.
        /// </summary>
        private void RemoveListObjects()
        {
            foreach (GameObject o in listObjects)
            {
                Destroy(o);
            }
            listObjects = new List<GameObject>();
        }

        /// <summary>
        /// Calls an instance of ingredientListManager to create a new ingredient list, then displays the new list of ingredient lists.
        /// </summary>
        public void OnAddListButtonClick()
        {
            ingredientListManager.CreateList();
            DisplayLists();
        }

        /// <summary>
        /// Calls the image of a button and changes its color.
        /// </summary>
        /// <param name="btn"> the button that changes color. </param>
        /// <param name="colr"> the color that the button changes into. </param>
        public void ChangeColor(Button btn, Color colr)
        {
            btn.GetComponent<Image>().color = colr;
        }

        /// <summary>
        /// Calls an instance of ingredientListManager which is used by the DeletePopUp method which interacts 
        ///  with the popupScreen elements which are set to active in this method.
        /// </summary>
        /// <param name="list"> The ingredient list that is to be deleted. </param>
        private void OnDeleteButtonClick(Logic.IngredientList list)
        {
            popupScreen.SetActive(true);
            DeletePopUp(list);
        }

        /// <summary>
        /// Calls an instance of ingredientListManager, if the user clicks the yes button on the pop-up they confirm the deletion and  deletes the given ingredient list.
        /// The pop-up screen also disappears.
        /// If the user clicks the no button on the pop-up the pop-up screen disappears. 
        /// </summary>
        /// <param name="list"> The ingredient list that is to be deleted. </param>
        private void DeletePopUp(Logic.IngredientList list)
        {
            Button yesButton = popupScreen.transform.GetChild(2).GetComponent<Button>();
            Button noButton = popupScreen.transform.GetChild(1).GetComponent<Button>();
           

            yesButton.onClick.AddListener(() => {
                ingredientListManager.DeleteList(list);
                DisplayLists();
                popupScreen.SetActive(false);   
            });
            noButton.onClick.AddListener(() => {popupScreen.SetActive(false); });
        }

        /// <summary>
        /// Calls an instance of ingredientListManager to close this screen and open the IngredientListScreen of the given ingredient list.
        /// </summary>
        /// <param name="list"> The ingredient list that was selected. </param>
        private void OnListButtonClick(Logic.IngredientList list)
        {
            ingredientListManager.ChangeToIngredientListScreen(list, this.gameObject);
        }

        private void OnBackButtonClick()
        {
            SceneSwitcher.Get().LoadScene("Home");
            Debug.Log(Storage.Get().ActiveIngredientList.ListName);
        }

        /// <summary>
        /// Toggles the color and visibility of two buttons, representing a check button and its corresponding border button.
        /// Also checking if there are buttons that have been previously checked and activating them.
        /// </summary>
        /// <param name="btn1"> The check button to be toggled. </param>
        /// <param name="btn2"> The corresponding border button to be toggled.</param>
        /// <param name="list"> The ingredient list that has been checked.</param>
        public void OnCheckButtonClick(Button btn1, Button btn2, IL.IngredientList list)
        {
            if (selectedCheckButton != null)
            {
                // Turn off previous buttons
                ChangeColor(selectedCheckButton, Color.gray);
                selectedBorderButton.gameObject.SetActive(false);
            }

            if (btn1 == selectedCheckButton)
            {
                // Set active list to empty
                Storage.Get().ActiveIngredientList = new("Empty");
            }
            else
            {
                // Set active list
                Storage.Get().ActiveIngredientList = list;

                // Select new active buttons
                selectedCheckButton = btn1;
                selectedBorderButton = btn2;
                
                // Turn on new buttons
                ChangeColor(selectedCheckButton,  Color.green );
                selectedBorderButton.gameObject.SetActive(true);
            }
        }
    }
}
    