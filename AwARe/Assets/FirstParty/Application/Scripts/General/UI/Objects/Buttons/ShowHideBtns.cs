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
    public class ShowHideBtns : MonoBehaviour
    {
        [SerializeField] private GameObject unFoldBtn;
        [SerializeField] private GameObject unFoldBtns;

        // Start is called before the first frame update

        // this is meant for a manager empty object that sets the gameobjects on active or inactive at the start 
        // and has a method for showing the buttons 
        void Start()
        {
            this.unFoldBtn.SetActive(false);
            this.unFoldBtns.SetActive(false);
        }

        public void ShowBtns()
        {
            this.unFoldBtn.SetActive(true);
            this.unFoldBtns.SetActive(true);
        }
    }
}
