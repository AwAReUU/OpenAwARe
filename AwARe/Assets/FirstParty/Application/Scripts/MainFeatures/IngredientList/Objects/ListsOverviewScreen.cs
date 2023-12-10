// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

using System;
using System.Collections.Generic;
using AwARe.InterScenes.Objects;
using UnityEditorInternal.VersionControl;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using IL = AwARe.IngredientList.Logic;

namespace AwARe.IngredientList.Objects
{
    public class ListsOverviewScreen : MonoBehaviour
    {
        // (assigned within unity)
        [FormerlySerializedAs("ingredientListManager")][SerializeField] private IngredientListManager manager;
        [SerializeField] private GameObject listItemObject; //list item 'prefab'

        // the objects drawn on screen to display the Lists
        List<ListItem> listItems = new();

        private void OnEnable()
        {
            DisplayLists();
        }

        private void OnDisable()
        {
            RemoveListObjects();
        }

        /// <summary>
        /// Creates GameObjects with buttons to select or destroy every ingredient list.
        /// </summary>
        public void DisplayLists()
        {
            RemoveListObjects();

            foreach (Logic.IngredientList ingredientList in manager.Lists)
            {
                // create a new list item to display this list
                GameObject itemObject = Instantiate(listItemObject, listItemObject.transform.parent);
                itemObject.SetActive(true);

                // Set the ingredients of the item and keep track of it.
                ListItem item = itemObject.GetComponent<ListItem>();
                item.SetList(listItems.Count, ingredientList);
                listItems.Add(item);
            }

            int checkIndex = manager.IndexList;
            if(checkIndex >= 0 && checkIndex < listItems.Count)
                listItems[checkIndex].Check(true);

        }

        /// <summary>
        /// Destroys all currently displayed GameObjects in the ScrollView.
        /// </summary>
        private void RemoveListObjects()
        {
            foreach (ListItem o in listItems)
                Destroy(o.gameObject);
            listItems = new();
        }

        /// <summary>
        /// Calls an instance of manager to create a new ingredient list, then displays the new list of ingredient lists.
        /// </summary>
        public void OnAddListButtonClick()
        {
            manager.CreateList();
            DisplayLists();
        }

        /// <summary>
        /// Calls an instance of manager to close this screen and open the IngredientListScreen of the given ingredient list.
        /// </summary>
        /// <param name="list"> The ingredient list that was selected </param>
        public void OnListButtonClick(Logic.IngredientList list)
        {
            manager.ChangeToIngredientListScreen(list, this.gameObject);
        }

        public void OnBackButtonClick()
        {
            SceneSwitcher.Get().LoadScene("Home");
        }

        /// <summary>
        /// Toggles the color and visibility of two buttons, representing a check button and its corresponding border button.
        /// Also checking if there are buttons that have been previously checked and activating them
        /// </summary>
        /// <param name="btn1"> The check button to be toggled. </param>
        /// <param name="btn2"> The corresponding border button to be toggled.
        public void OnCheckButtonClick(int index, Logic.IngredientList list)
        {
            // Check if button was not already checked.
            int previousIndex = manager.IndexList;
            if (index == previousIndex)
                return;

            // Switch active buttons
            listItems[previousIndex].Check(false);
            listItems[index].Check(true);

            // Change active list
            manager.CheckList(index, list);
        }
    }
}
    