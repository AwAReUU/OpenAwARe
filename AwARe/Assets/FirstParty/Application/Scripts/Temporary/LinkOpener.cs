using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AwARe
{
    public class LinkOpener : MonoBehaviour
    {
        public void OpenWebPage(string link = null)
        {
            if (link == null)
                Application.OpenURL("https://paypal.me/nickandco");
            else
                Application.OpenURL(link);
        }
    }
}
