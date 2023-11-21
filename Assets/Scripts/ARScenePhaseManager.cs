using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ARScenePhaseManager : MonoBehaviour
{
    [SerializeField] private GameObject phasePolyScan;
    [SerializeField] private GameObject phaseActiveRoom;
    [SerializeField] private GameObject phaseIngredientList;
    [SerializeField] private GameObject phaseQuestionnaire;
    [SerializeField] private GameObject phaseHomeScreen;

    bool fromHome = true;

    private void Awake()
    {
        Debug.Log("Awake");
        //SetPhasePolyScan();
    }

    // ? Completely resets the scene, not sure if needed
    // ? https://forum.unity.com/threads/reset-ar-session.537261/#post-6581095
    // private void SceneReset() 
    // {
    //     var xrManagerSettings = UnityEngine.XR.Management.XRGeneralSettings.Instance.Manager;
    //     xrManagerSettings.DeinitializeLoader();
    //     UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex); // reload current scene
    //     xrManagerSettings.InitializeLoaderSync();
    //     SetPhasePolyScan();
    // }

    public void SetPhase(string s)
    {
        phasePolyScan.SetActive(false);
        phaseActiveRoom.SetActive(false);
        phaseIngredientList.SetActive(false);
        phaseQuestionnaire.SetActive(false);
        phaseHomeScreen.SetActive(false);

        if (s == "poly")
        {

            phasePolyScan.SetActive(true);
        }
        else if (s == "ar")
        {
            fromHome = false;
            phaseActiveRoom.SetActive(true);
        }
        else if (s == "ing")
        {

            phaseIngredientList.SetActive(true);
        }
        else if (s == "que")
        {

            phaseQuestionnaire.SetActive(true);
        }
        else if (s == "home")
        {
            fromHome = true;
            phaseHomeScreen.SetActive(true);
        }
    }
    
    public void FromHome(bool b)
    {
        fromHome = b;
    }

    public void GoBack()
    {
        if (fromHome)
            SetPhase("home");
        else
            SetPhase("ar");
    }
}
