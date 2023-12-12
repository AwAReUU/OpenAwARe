// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AwARe.IngredientList.Objects
{
    /// <summary>
    /// An UI Element displaying a single list.
    /// </summary>
    public class ListItem : MonoBehaviour
    {
        // The parent element
        [SerializeField] private ListsOverviewScreen screen;
        
        // UI elements to control
        [SerializeField] private TextMeshProUGUI nameLabel;
        [SerializeField] private TextMeshProUGUI quantityLabel;
        [SerializeField] private Image checkButton;
        [SerializeField] private GameObject checkHighlight;
        
        // Options parameters
        [SerializeField] private Color checkOnColor;
        
        // The data represented.
        private Logic.IngredientList list;
        private int index;                  // TODO: Overhaul IngredientList and find listItems by some IngredientList.ID instead

        // Other parameters and data.
        private Color checkOffColor;
        
        private void Awake()
        {
            checkOffColor = checkButton.color;
        }
        
        /// <summary>
        /// Sets this item to represent the given result.
        /// </summary>
        /// <param name="index">The index in the item list.</param>
        /// <param name="list">The new ingredient list represented.</param>
        public void SetItem(int index, Logic.IngredientList list)
        {
            this.index = index;
            this.list = list;
            DisplayItem();
        }
        
        /// <summary>
        /// Corrects this UI element to represent its data.
        /// </summary>
        private void DisplayItem()
        {
            gameObject.name =  list.ListName;
            nameLabel.text = list.ListName;
            quantityLabel.text = list.NumberOfIngredients().ToString();
        }
        
        /// <summary>
        /// Corrects this UI element to represent its selection.
        /// </summary>
        /// <param name="check">True to check, false to uncheck.</param>
        public void Check(bool check)
        {
            if (check)
            {
                checkButton.color = checkOnColor;
                checkHighlight.SetActive(true);
            }
            else
            {
                checkButton.color = checkOffColor;
                checkHighlight.SetActive(false);
            }
        }
        
        /// <summary>
        /// Bound to the UI element. <br/>
        /// Passes the button press and its data to the parent.
        /// </summary>
        public void OnCheckButtonClick() =>
            screen.OnCheckButtonClick(index, list);
        
        /// <summary>
        /// Bound to the UI element. <br/>
        /// Passes the button press and its data to the parent.
        /// </summary>
        public void OnItemClick() =>
            screen.OnItemClick(list);
    }
}