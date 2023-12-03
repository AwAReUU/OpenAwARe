using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AwARe.UI.Objects
{
    public class PopUpScript : MonoBehaviour
    {
        [SerializeField] private GameObject popupPanel;

        // popUp is set to active or inactive
        public void PopUpOn() =>
            popupPanel.SetActive(true);

        public void PopUpOff() =>
            popupPanel.SetActive(false);
    }
}
