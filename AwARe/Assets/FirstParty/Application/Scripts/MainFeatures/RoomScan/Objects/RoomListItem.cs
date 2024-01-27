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
    public class RoomListItem : MonoBehaviour
    {
        // The parent element
        [SerializeField] private RoomOverviewScreen screen;

        // UI element to control
        [SerializeField] private TextMeshProUGUI nameLabel;

        // The data represented.
        private Data.Logic.Room room;

        /// <summary>
        /// Sets this item to represent the given result.
        /// </summary>
        /// <param name="index">The index in the item list.</param>
        /// <param name="room">The room represented.</param>
        public void SetItem(Data.Logic.Room room)
        {
            this.room = room;
            DisplayItem();
        }
        /// <summary>
        /// Corrects this UI element to represent its data.
        /// </summary>
        private void DisplayItem()
        {
            gameObject.name = room.RoomName;
            nameLabel.text = room.RoomName;
        }

        /// <summary>
        /// Loads the room of corresponding to this listItem.
        /// </summary>
        public void OnItemClick() =>
            screen.OnItemClick(room);

        /// <summary>
        /// Delete the room of corresponding to this listItem.
        /// </summary>
        public void OnDeleteButtonClick() =>
            screen.OnDeleteButtonClick(room);

    }
}
