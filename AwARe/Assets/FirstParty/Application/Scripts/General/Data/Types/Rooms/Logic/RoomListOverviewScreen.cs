using AwARe.IngredientList.Objects;
using AwARe.InterScenes.Objects;
using PlasticGui.Configuration.CloudEdition.Welcome;
using System.Collections;
using System.Collections.Generic;

using AwARe.AwARe.Data.Objects;

using NSubstitute.ReturnsExtensions;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

namespace AwARe.Data.Logic
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
    public class RoomListOverviewScreen : MonoBehaviour
    {
        // The parent element
        [SerializeField] private RoomListManager manager;

        // UI elements to control/copy
        [SerializeField] private GameObject listItemObject; //list item 'prefab'
        [SerializeField] private GameObject nameSaveRoom;

        // Tracked UI elements
        readonly List<ListItem> lists = new();

        private void OnEnable() { DisplayLists(); }

        private void OnDisable() { RemoveLists(); }

        /// <summary>
        /// Creates GameObjects with buttons to select or destroy every ingredient list.
        /// </summary>
        public void DisplayLists()
        {
            RemoveLists();

                foreach (string roomsave in manager.roomlist)
                {
                    // create a new list item to display this list
                    GameObject itemObject = Instantiate(listItemObject, listItemObject.transform.parent);
                    TMP_Text buttontext = itemObject.GetComponentInChildren<TMP_Text>();
                    buttontext.text = roomsave;
                    itemObject.gameObject.SetActive(true);
                }

            int checkIndex = manager.IndexList;
            if (checkIndex >= 0 && checkIndex < lists.Count)
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

        public void OnConfirmNameButton()
        {
            nameSaveRoom.SetActive(false);
            // Check if RoomListManager is initialized
            if (manager != null)
            {
                // Check if roomlist is not null before accessing it
                if (manager.roomlist != null)
                {
                    OnAddListButtonClick();
                }
                else
                {
                    Debug.LogError("roomlist in RoomListManager is null.");
                }
            }
            else
            {
                Debug.LogError("RoomListManager is null.");
            }
        }

        public void OnSaveNewRoomClick()
        {
            nameSaveRoom.SetActive(true);
        }

        /// <summary>
        /// Loads the Home scene.
        /// </summary>
        public void OnBackButtonClick() { SceneSwitcher.Get().LoadScene("Home"); }

        /// <summary>
        /// Unchecks the previous item and checks the given.
        /// </summary>
        /// <param name="index"> The index of the item to check/select. </param>
        /// <param name="list"> The list of the item to check/select. </param>
        /*public void OnCheckButtonClick(int index, string save)
        {
            // Check if button was not already checked.
            int previousIndex = manager.IndexList;
            if (index == previousIndex)
                return;

            // Switch active buttons
            lists[previousIndex].Check(false);
            lists[index].Check(true);

            // Change active list
            manager.CheckList(index, save);
        }*/
    }
}
