using AwARe.InterScenes.Objects;

using UnityEngine;
using UnityEngine.UI;

namespace AwARe.Questionnaire.Objects
{
    public class SubmitButton : MonoBehaviour
    {
        public void Submit()
        {
            SceneSwitcher.Get().LoadScene("Home");
        }
    }
}