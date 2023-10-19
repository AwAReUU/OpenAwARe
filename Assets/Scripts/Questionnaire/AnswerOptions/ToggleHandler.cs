using UnityEngine;
using UnityEngine.UI;

public class ToggleHandler : MonoBehaviour
{
    private GameObject question;
    private int optionNumber;

    //notifies the question that this answer option is now yes or no, true or false
    public void NotifyChange()
    {
        question.GetComponent<Question>().ChangeIfyesState(optionNumber, gameObject.GetComponent<Toggle>().isOn);
    }

    public void setQuestion(GameObject question)
    {
        this.question = question;
    }

    //should be called exactly once per answer option object, when it is created by te question
    public void setNumber(ref int optionNumber)
    {
        this.optionNumber = optionNumber;
        optionNumber++;
    }
}