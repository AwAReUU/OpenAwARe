using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.UI;

public class ToggleHandler : MonoBehaviour
{
    private GameObject question;

    int optionNumber;

    public void NotifyChange()
    {
        question.GetComponent<Question>().ChangeIfyesState(optionNumber, gameObject.GetComponent<Toggle>().isOn);
    }

    public void setQuestion(GameObject question)
    {
        this.question = question;
    }

    public void setNumber(ref int optionNumber)
    {
        this.optionNumber = optionNumber;
        optionNumber++;
    }
}
