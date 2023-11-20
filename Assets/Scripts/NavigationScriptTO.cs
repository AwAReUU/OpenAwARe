using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NavigationScriptTO : MonoBehaviour
{
 
    public void OnButtonNewRoomClick()
    {
        
        SceneManager.LoadScene("Polygon_Scan");
    }

    public void OnButtonLoadRoomClick()
    {

        ShowPopup("Button 2 Popup");
    }

    public void OnButtonQuestionnaireClick()
    {

        SceneManager.LoadScene("QuestionnairePage");
    }

    public void OnButtonSettingsClick()
    {
 
        ShowPopup("Button 4 Popup");
    }

    private void ShowPopup(string message)
    {
        Debug.Log("Showing popup: " + message);

    }
}
