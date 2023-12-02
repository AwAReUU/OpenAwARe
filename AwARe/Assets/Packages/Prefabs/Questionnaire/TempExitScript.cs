using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempExitScript : MonoBehaviour
{
    // ! TEMP SCRIPT VOOR QUICK FIX IN TUSSENTIJDSE OVERDRACHT
    ARScenePhaseManager aRScenePhaseManager;
    // Start is called before the first frame update
    void Start()
    {
        aRScenePhaseManager = FindObjectOfType<ARScenePhaseManager>();
    }

    public void BackToHome()
    {
        aRScenePhaseManager.SetPhase("home");
    }
}
