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
        private Entree entree;
        
        /// <summary>
        /// Sets this item to represent the given entree.
        /// </summary>
        /// <param name="entree">The entree represented.</param>
        public void SetItem(Entree entree)
        {
            this.entree = entree;
            DisplayItem();
        }
        
        /// <summary>
        /// Corrects this UI element to represent its data.
        /// </summary>
        private void DisplayItem()
        {
            // change the text to match the ingredient info
            gameObject.name =  entree.ingredient.Name;
            ingredientLabel.text = entree.ingredient.Name;
            quantityLabel.text = entree.quantity.ToString();
            quantityTypeLabel.text = entree.type.ToString();
        }

        /// <summary>
        /// Calls an instance of IngredientListManager to delete the given ingredient from the ingredient list, then displays the updated list.
        /// </summary>
        /// <param name="ingredient"> The Ingredient that will be deleted from the list. </param>
        public void OnItemClick() =>
            screen.OnItemClick(entree);

        /// <summary>
        /// Calls an instance of IngredientListManager to delete the given ingredient from the ingredient list, then displays the updated list.
        /// </summary>
        /// <param name="ingredient"> The Ingredient that will be deleted from the list. </param>
        public void OnDeleteButtonClick() =>
            screen.OnDeleteItemButtonClick(entree.ingredient);
    }
}