using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeScene()
    {
        //scene id can be seen in the build manager
        //blankar is 0, questionnaire is 1

        if(SceneManager.GetActiveScene().name == "BlankAR")
        {
            SceneManager.LoadScene(1);
        }
        else if (SceneManager.GetActiveScene().name == "QuestionnairePage")
        {
            SceneManager.LoadScene(0);
        }

        // if (SceneManager.GetActiveScene().name == "BlankAR") gameObject.GetComponent<Image>().color = Color.green;
        // else if (SceneManager.GetActiveScene().name == "QuestionnairePage") gameObject.GetComponent<Image>().color = Color.red;
    }
}