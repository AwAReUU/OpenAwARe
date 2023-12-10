using System.Collections.Generic;

using AwARe.IngredientList.Logic;

using TMPro;

using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using static AwARe.IngredientList.Logic.IngredientList;

namespace AwARe.IngredientList.Objects
{
    public class IngredientListItem : MonoBehaviour
    {
        [SerializeField] private IngredientListScreen screen;

        private Entree entree;

        // (assigned within unity)
        [SerializeField] private TextMeshProUGUI ingredientLabel;
        [SerializeField] private TextMeshProUGUI quantityLabel;
        [SerializeField] private TextMeshProUGUI quantityTypeLabel;

        /// <summary>
        /// Changes the name of the list to the name that is put into the inputfield in unity
        /// </summary>
        public void SetItem(Entree entree)
        {
            this.entree = entree;
            DisplayItem();
        }
        
        /// <summary>
        /// Creates GameObjects with an "edit" and "delete" button for all the ingredients in this is List and adds them to the ScrollView 
        /// and adds an "Add Ingredient" button to the end of the ScrollView.
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
        /// <param name="ingredient"> The Ingredient that will be deleted from the list </param>
        public void OnItemClick() =>
            screen.OnItemClick(entree);

        /// <summary>
        /// Calls an instance of IngredientListManager to delete the given ingredient from the ingredient list, then displays the updated list.
        /// </summary>
        /// <param name="ingredient"> The Ingredient that will be deleted from the list </param>
        public void OnDeleteButtonClick() =>
            screen.OnDeleteItemButtonClick(entree.ingredient);
    }
}