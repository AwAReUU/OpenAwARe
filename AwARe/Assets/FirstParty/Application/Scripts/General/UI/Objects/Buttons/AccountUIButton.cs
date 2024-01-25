// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \* 

using System.Collections;
using System.Collections.Generic;

using AwARe.InterScenes.Objects;
using AwARe.UI.Objects;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

namespace AwARe
{
    public class AccountUIButton : MonoBehaviour
    {

        // input fields
        [SerializeField] private TMP_InputField loginEmailInputField;
        [SerializeField] private TMP_InputField loginPasswordInputField;
        [SerializeField] private TMP_InputField registerEmailInputField;
        [SerializeField] private TMP_InputField registerPasswordInputField;
        [SerializeField] private TMP_InputField FirstNameInputField;
        [SerializeField] private TMP_InputField LastNameInputField;
        [SerializeField] private TMP_InputField passwordConfirmInputField;

        // buttons and pop up gameobject
        [SerializeField] private Button stayButton;
        [SerializeField] private Button leaveButton;
        [SerializeField] private GameObject unsavedPopup;

        // Start is called before the first frame update
        void Start()
        {
            
        }

        void Awake()
        {
            stayButton.onClick.AddListener(delegate () { this.OnStayButtonClick(); });
            leaveButton.onClick.AddListener(delegate () { this.OnLeaveButtonClick(); });
        }

        /// <summary>
        /// When AccountUIBackButton is clicked, checks if anything was written in the inputfields.
        /// If not go back to home else activate popup.
        /// </summary>
        public void OnAccountUIBackButtonClick()
        {
            string loginpassword = loginPasswordInputField.text;
            string loginemail = loginEmailInputField.text;
            string registeremail = registerEmailInputField.text;
            string registerpassword = registerPasswordInputField.text;
            string registerfirstname = FirstNameInputField.text;
            string registerlastname = LastNameInputField.text;
            string registerconfirmpassword = passwordConfirmInputField.text;

            if (!string.IsNullOrEmpty(registeremail) || !string.IsNullOrEmpty(registerpassword) ||
                !string.IsNullOrEmpty(registerfirstname) || !string.IsNullOrEmpty(registerlastname) ||
                !string.IsNullOrEmpty(registerconfirmpassword) || !string.IsNullOrEmpty(loginemail) || !string.IsNullOrEmpty(loginpassword))
            {
                unsavedPopup.SetActive(true);
            }
            else SceneSwitcher.Get().LoadScene("Home");

        }

        /// <summary>
        /// Deactivate popup if user clicks on the stay button.
        /// </summary>
        public void OnStayButtonClick()
        {
            unsavedPopup.SetActive(false);
        }
        /// <summary>
        /// Go to home page if user clicks on the leave button.
        /// </summary>
        public void OnLeaveButtonClick()
        {
            SceneSwitcher.Get().LoadScene("Home"); 
        }

    }
}
