// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

using System.Collections.Generic;
using UnityEngine;

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
        // The parent element
        [SerializeField] private IngredientListManager manager;
        
        // UI elements to control/copy
        [SerializeField] public GameObject listItemObject; //list item 'prefab'
        
        // Tracked UI elements
        readonly List<ListItem> lists = new();

        private void OnEnable()
        {
            DisplayLists();
        }

        private void OnDisable()
        {
            RemoveLists();
        }

        /// <summary>
        /// Creates GameObjects with buttons to select or destroy every ingredient list.
        /// </summary>
        public void DisplayLists()
        {
            RemoveLists();

            foreach (Logic.IngredientList ingredientList in manager.Lists)
            {
                // create a new list item to display this list
                GameObject itemObject = Instantiate(listItemObject, listItemObject.transform.parent);
                itemObject.SetActive(true);

                // Set the ingredients of the item and keep track of it.
                ListItem item = itemObject.GetComponent<ListItem>();
                item.SetItem(lists.Count, ingredientList);
                lists.Add(item);
            }

            int checkIndex = manager.IndexList;
            if(checkIndex >= 0 && checkIndex < lists.Count)
                lists[checkIndex].Check(true);

        }


        /// <summary>
        /// Destroys all currently displayed GameObjects in the ScrollView.
        /// </summary>
        private void RemoveLists()
        {
            foreach (ListItem o in lists)
                Destroy(o.gameObject);
            lists.Clear();
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
        /// <param name="list"> The ingredient list that was selected. </param>
        public void OnItemClick(Logic.IngredientList list)
        {
            manager.ChangeToIngredientListScreen(list, this.gameObject);
        }

        /// <summary>
        /// Unchecks the previous item and checks the given.
        /// </summary>
        /// <param name="index"> The index of the item to check/select. </param>
        /// <param name="list"> The list of the item to check/select. </param>
        public void OnCheckButtonClick(int index, Logic.IngredientList list)
        {
            // Check if button was not already checked.
            int previousIndex = manager.IndexList;
            if (index == previousIndex)
                return;

            // Switch active buttons
            lists[previousIndex].Check(false);
            lists[index].Check(true);

            // Change active list
            manager.CheckList(index, list);
        }
    }
}
    