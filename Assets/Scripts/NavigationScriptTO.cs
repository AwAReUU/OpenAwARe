using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NavigationScriptTO : MonoBehaviour
{
 
    public void OnButtonNewRoomClick()
    {
        // Logic for Button 1
        Debug.Log("Button 1 clicked");
        SceneManager.LoadScene("Polygon_Scan");
    }

    public void OnButtonLoadRoomClick()
    {
        // Logic for Button 2
        Debug.Log("Button 2 clicked");
        ShowPopup("Button 2 Popup");
    }

    public void OnButtonQuestionnaireClick()
    {
        // Logic for Button 3
        Debug.Log("Button 3 clicked");
        SceneManager.LoadScene("QuestionnairePage");
    }

    public void OnButtonSettingsClick()
    {
        // Logic for Button 4
        Debug.Log("Button 4 clicked");
        ShowPopup("Button 4 Popup");
    }

    private void ShowPopup(string message)
    {
        // Logic to show a popup with the given message
        Debug.Log("Showing popup: " + message);
        // Add your popup implementation here
    }
}
