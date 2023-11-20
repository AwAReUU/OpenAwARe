using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NavigationScriptTO : MonoBehaviour
{

    // Navigation to the scenes from button clicks
    public void OnButtonNewRoomClick()
    {
        
        SceneManager.LoadScene("Polygon_Scan");
    }

    public void OnButtonQuestionnaireClick()
    {

        SceneManager.LoadScene("QuestionnairePage");
    }

    public void OnButtonIngredientListClick()
    {

        SceneManager.LoadScene("IngredientListUI");
    }

    public void OnButtonHomeClick()
    {

        SceneManager.LoadScene("HomeScreen");
    }

}
