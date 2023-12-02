using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace AwARe.MonoBehaviours
{
    public class NotImplemented : MonoBehaviour
    {
        public void ShowPopUp() =>
            NotImplementedHandler.Get().ShowPopUp();

        public void HidePopUp() =>
            NotImplementedHandler.Get().HidePopUp();
    }
}
