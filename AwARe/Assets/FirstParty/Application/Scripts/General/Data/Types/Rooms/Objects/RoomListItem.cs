using AwARe.IngredientList.Objects;
using System.Collections;
using System.Collections.Generic;

using TMPro;

using UnityEngine;

namespace AwARe
{
    public class RoomListItem : MonoBehaviour
    {
        [SerializeField] private ListsOverviewScreen screen;
        // UI elements to control
        [SerializeField] private TextMeshProUGUI nameLabel;
        private int index;
        // The data represented.
        private List<string> list;
        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
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

    }
}
