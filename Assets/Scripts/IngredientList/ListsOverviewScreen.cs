using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace IngredientLists
{
    public class ListsOverviewScreen : MonoBehaviour
    {
        // (assigned within unity)
        [SerializeField] private GameObject backButton;
        [SerializeField] private IngredientListManager ingredientListManager;
        [SerializeField] private Transform scrollViewContent;
        [SerializeField] private GameObject listItemObject; //list item 'prefab'
        private int clickCount = 0;
        private Dictionary<GameObject, Button> checkButtonsDictionary = new Dictionary<GameObject, Button>();
        private Button selectedCheckButton;
        private Button selectedBorderButton;

        // the objects drawn on screen to display the Lists
        List<GameObject> listObjects = new();

        private void OnEnable()
        {
            Button backB = backButton.GetComponent<Button>();
            backB.onClick.AddListener(delegate { OnBackButtonClick(); });
            backButton.SetActive(true);

            DisplayLists();
        }

        private void OnDisable()
        {
            Button backB = backButton.GetComponent<Button>();
            backB.onClick.RemoveAllListeners();
            RemoveListObjects();
        }

        /// <summary>
        /// Creates GameObjects with buttons to select or destroy every ingredient list.
        /// </summary>
        public void DisplayLists()
        {
            RemoveListObjects();

            foreach (IngredientList ingredientList in ingredientListManager.Lists)
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
                editButton.onClick.AddListener(() => { OnListButtonClick(ingredientList); });
                delButton.onClick.AddListener(() => { OnDeleteButtonClick(ingredientList); });
                checkButton.onClick.AddListener(() => { OnCheckButtonClick(checkButton, borderButton); });
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

        public void ChangeColor(Button btn,Color colr)
        {
            btn.GetComponent<Image>().color = colr;

        }

        /// <summary>
        /// Calls an instance of ingredientListManager to delete the given ingredient list, then displays the new list of ingredient lists.
        /// </summary>
        /// <param name="list"> The ingredient list that is to be deleted </param>
        private void OnDeleteButtonClick(IngredientList list)
        {
            ingredientListManager.DeleteList(list);
            DisplayLists();
        }

        /// <summary>
        /// Calls an instance of ingredientListManager to close this screen and open the IngredientListScreen of the given ingredient list.
        /// </summary>
        /// <param name="list"> The ingredient list that was selected </param>
        private void OnListButtonClick(IngredientList list)
        {
            ingredientListManager.ChangeToIngredientListScreen(list, this.gameObject);
        }

        private void OnBackButtonClick()
        {
            //TODO: Open main menu
        }

        public void OnCheckButtonClick(Button btn1, Button btn2)
        {
            // Toggle the color of btn1 and the visibility of btn2
            ChangeColor(btn1, btn1.GetComponent<Image>().color == Color.gray ? Color.green : Color.gray);
            btn2.gameObject.SetActive(!btn2.gameObject.activeSelf);

            // Deactivate the previously selected check button and its corresponding border button
            if (selectedCheckButton != null && selectedCheckButton != btn1)
            {
                ChangeColor(selectedCheckButton, Color.gray);
                selectedBorderButton.gameObject.SetActive(false);
            }

            // Update the currently selected list item
            selectedCheckButton = btn1;
            selectedBorderButton = btn2;
        }
    }
}
    