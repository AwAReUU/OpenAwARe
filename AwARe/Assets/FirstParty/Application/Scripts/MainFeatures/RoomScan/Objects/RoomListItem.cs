// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

using AwARe.Data.Logic;
using TMPro;
using UnityEngine;

namespace AwARe
{
    /// <summary>
    /// An object for representaing a room in a list.
    /// </summary>
    public class RoomListItem : MonoBehaviour
    {
        // The parent element
        [SerializeField] private RoomOverviewScreen screen;

        // UI element to control
        [SerializeField] private TextMeshProUGUI nameLabel;

        // The data represented.
        private int roomIndex;
        private string roomName;

        /// <summary>
        /// Sets this item to represent the given result.
        /// </summary>
        /// <param name="roomIndex">The index in the item list.</param>
        /// <param name="roomName">The name of the room represented.</param>
        public void SetItem(int roomIndex, string roomName)
        {
            this.roomIndex = roomIndex;
            this.roomName = roomName;
            DisplayItem();
        }
        /// <summary>
        /// Corrects this UI element to represent its data.
        /// </summary>
        private void DisplayItem()
        {
            gameObject.name = roomName;
            nameLabel.text = roomName;
        }

        /// <summary>
        /// Loads the room of corresponding to this listItem.
        /// </summary>
        public void OnItemClick() =>
            screen.OnItemClick(roomIndex);

        /// <summary>
        /// Delete the room of corresponding to this listItem.
        /// </summary>
        public void OnDeleteButtonClick() =>
            screen.OnDeleteButtonClick(roomIndex);

    }
}
