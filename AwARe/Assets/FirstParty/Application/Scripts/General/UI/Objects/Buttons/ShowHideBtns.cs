// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

/*
 * Temporary/To be corrected
 */

using UnityEngine;

namespace AwARe.UI.Objects
{
    /// <summary>
    /// Controls the visibility of  the foldable button and the buttons related to it
    /// </summary>
    public class ShowHideBtns : MonoBehaviour
    {
        [SerializeField] private GameObject unFoldBtn;
        [SerializeField] private GameObject unFoldBtns;
        /// <summary>
        /// setting the buttons to inactive at the start, making them invisable.
        /// </summary>
        void Start()
        {
            this.unFoldBtn.SetActive(false);
            this.unFoldBtns.SetActive(false);
        }

        /// <summary>
        /// Displays the buttons by setting them to an active state.
        /// </summary>
        public void ShowBtns()
        {
            this.unFoldBtn.SetActive(true);
            this.unFoldBtns.SetActive(true);
        }
    }
}
