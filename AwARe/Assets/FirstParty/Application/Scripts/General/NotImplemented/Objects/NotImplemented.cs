using UnityEngine;

namespace AwARe.NotImplemented.Objects
{
    public class NotImplemented : MonoBehaviour
    {
        public void ShowPopUp() =>
            NotImplementedHandler.Get().ShowPopUp();

        public void HidePopUp() =>
            NotImplementedHandler.Get().HidePopUp();
    }
}
