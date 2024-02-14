// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

using UnityEngine;

namespace AwARe
{
    public class ObjGenUIHandler : MonoBehaviour
    {

        /// <summary>
        /// The popup that appears if no ingredientlist has been selected yet.
        /// </summary>
        public GameObject NoListSelectedPopup;

        /// <summary>
        /// The popup that appears if multiple rooms are needed to spawn the objects.
        /// </summary>
        public GameObject SplitRoomsPopup;

        /// <summary>
        /// Set the noListSelectedPopup active or inactive.
        /// </summary>
        /// <param name="setActive">Whether the popup should be set to active.</param>
        public void SetNoListSelectedPopup(bool setActive)
        {
            NoListSelectedPopup.SetActive(setActive);
        }

        /// <summary>
        /// Set the noListSelectedPopup active or inactive.
        /// </summary>
        /// <param name="setActive">Whether the popup should be set to active.</param>
        public void SetSplitRoomsPopup(bool setActive)
        {
            SplitRoomsPopup.SetActive(setActive);
        }
    }
}
