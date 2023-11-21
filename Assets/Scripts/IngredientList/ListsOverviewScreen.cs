using System.Collections.Generic;
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
        private void DisplayLists()
        {
            RemoveListObjects();

            foreach (IngredientList ingredientList in ingredientListManager.Lists)
            {
                // create a new list item to display this list
                GameObject listItem = Instantiate(listItemObject, scrollViewContent);
                listItem.SetActive(true);

                // change the text to match the list info
                Button delButton = listItem.transform.GetChild(1).GetComponent<Button>();
                Button listButton = listItem.transform.GetChild(0).GetComponent<Button>();
                listButton.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text =
                    ingredientList.ListName;
                listButton.transform.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text =
                    ingredientList.NumberOfIngredients().ToString();

                listObjects.Add(listItem);

                // store i in an int for pass-by value to the lambda expression
                listButton.onClick.AddListener(() => { OnListButtonClick(ingredientList); });
                delButton.onClick.AddListener(() => { OnDeleteButtonClick(ingredientList); });
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
        private void OnAddListButtonClick()
        {
            ingredientListManager.CreateList();
            DisplayLists();
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
    }
}