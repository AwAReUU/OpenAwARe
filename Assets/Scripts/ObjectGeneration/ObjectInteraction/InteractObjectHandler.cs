using UnityEngine;

/// <summary>
/// TODO: implement pop-up screen with information when clicking
/// on GameObject. Currently this class only colors clicked objects. 
/// </summary>
public class InteractObjectHandler : MonoBehaviour
{
    [SerializeField] private Material black;
    [SerializeField] private Material normal;

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
    
    public void ToggleDataWindow(GameObject target)
    {
        GameObject dataWindow = target.transform.GetChild(0).gameObject;
        if (dataWindow.activeSelf)
            dataWindow.SetActive(false);
        else
            dataWindow.SetActive(true);
    }
}
