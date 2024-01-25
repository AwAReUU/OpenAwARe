// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*  

using System.Collections.Generic;
using AwARe.Data.Logic;
using TMPro;
using UnityEngine;

namespace AwARe
{
    public class RoomListItem : MonoBehaviour
    {
        [SerializeField] private RoomListOverviewScreen screen;
        [SerializeField] private TextMeshProUGUI nameLabel;
        private int index;

        private string name;
        // The data represented.
        private List<string> list;
        // Start is called before the first frame update
        void Start()
        {
            name = nameLabel.text;
        }

        /// <summary>
        /// Sets this item to represent the given result.
        /// </summary>
        /// <param name="index">The index in the item list.</param>
        /// <param name="list">The new ingredient list represented.</param>
        public void SetRoomItem(int index, List<string> list, string name)
        {
            this.index = index;
            this.list = list;
            DisplayRoomItem(name);
        }
        /// <summary>
        /// Corrects this UI element to represent its data.
        /// </summary>
        private void DisplayRoomItem(string name)
        {
            gameObject.name = name;
            nameLabel.text = name;
            
        }
        /// <summary>
        /// Loads the room of which the name is on the roomlistitem
        /// </summary>
        public void OnRoomItemClick() =>
            screen.OnRoomItemClick(name);

        /// <summary>
        /// Delete the roomlistitem
        /// </summary>
        public void DeleteList() =>
            screen.DeleteList(name);

    }
}
