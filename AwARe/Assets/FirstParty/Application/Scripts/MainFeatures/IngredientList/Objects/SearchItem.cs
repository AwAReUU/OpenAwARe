using AwARe.IngredientList.Logic;
using TMPro;
using UnityEngine;

namespace AwARe.IngredientList.Objects
{
    /// <summary>
    /// An UI Element displaying a single search result.
    /// </summary>
    public class SearchItem : MonoBehaviour
    {
        // The parent element
        [SerializeField] private SearchScreen screen;
        
        // UI elements to control
        [SerializeField] private TextMeshProUGUI nameLabel;
        
        // The data represented.
        private Ingredient result;

        /// <summary>
        /// Sets this item to represent the given result.
        /// </summary>
        /// <param name="result">The new search result represented.</param>
        public void SetItem(Ingredient result)
        {
            this.result = result;
            DisplayItem();
        }
        
        /// <summary>
        /// Corrects this UI element to represent its data.
        /// </summary>
        private void DisplayItem()
        {
            gameObject.name =  result.Name;
            nameLabel.text = result.Name;
        }

        /// <summary>
        /// Bound to the UI element. <br/>
        /// Passes the button press and its data to the parent.
        /// </summary>
        public void OnItemClick() =>
            screen.OnItemClick(result);
    }
}