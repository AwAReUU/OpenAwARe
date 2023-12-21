// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

using TMPro;
using UnityEngine;
using static AwARe.IngredientList.Logic.IngredientList;

namespace AwARe.IngredientList.Objects
{
    /// <summary>
    /// An UI Element displaying an ingredient within a list.
    /// </summary>
    public class IngredientListItem : MonoBehaviour
    {
        // The parent element
        [SerializeField] private IngredientListScreen screen;

        // UI elements to control
        [SerializeField] private TextMeshProUGUI ingredientLabel;
        [SerializeField] private TextMeshProUGUI quantityLabel;
        [SerializeField] private TextMeshProUGUI quantityTypeLabel;

        // The data represented.
        private Entry entry;
        
        /// <summary>
        /// Sets this item to represent the given entry.
        /// </summary>
        /// <param name="entry">The entry represented.</param>
        public void SetItem(Entry entry)
        {
            this.entry = entry;
            DisplayItem();
        }
        
        /// <summary>
        /// Corrects this UI element to represent its data.
        /// </summary>
        private void DisplayItem()
        {
            // change the text to match the ingredient info
            gameObject.name =  entry.ingredient.Name;
            ingredientLabel.text = entry.ingredient.Name;
            quantityLabel.text = entry.quantity.ToString();
            quantityTypeLabel.text = entry.type.ToString();
        }

        /// <summary>
        /// Calls an instance of IngredientListManager to delete the given ingredient from the ingredient list, then displays the updated list.
        /// </summary>
        public void OnItemClick() =>
            screen.OnItemClick(entry);

        /// <summary>
        /// Calls an instance of IngredientListManager to delete the given ingredient from the ingredient list, then displays the updated list.
        /// </summary>
        public void OnDeleteButtonClick() =>
            screen.OnDeleteItemButtonClick(entry.ingredient);
    }
}