using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace AwARe.ObjectInteraction.Objects
{
    
    /// <summary>
    /// TODO: implement pop-up screen with information when clicking on GameObject.
    /// Currently this class is just placeholder code that colors clicked objects. 
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class InteractObjectHandler : MonoBehaviour
    {
        [SerializeField] private Material black;
        [SerializeField] private Material normal;

        /// <summary>
        /// Toggle color of clicked object.
        /// </summary>
        /// <param name="target">Clicked gameobject</param>
        public void ColorObject(GameObject target)
        {
            if (target.GetComponent<MeshRenderer>().material == black)
            {
                target.GetComponent<MeshRenderer>().material = normal;
            }
            else
            {
                target.GetComponent<MeshRenderer>().material = black;
            }
        }
    
        /// <summary>
        /// Enables/disables dataWindow based on its current visibility.
        /// </summary>
        /// <param name="target">Clicked gameobject</param>
        public void ToggleDataWindow(GameObject target)
        {
            GameObject dataWindow = target.transform.GetChild(0).gameObject;
            //flip active state
            dataWindow.SetActive(!dataWindow.activeSelf);
        }
    }
}
