// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \* 

using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using TMPro;

using Unity.VisualScripting.YamlDotNet.Core.Events;

using UnityEngine;
using UnityEngine.UI;

namespace AwARe
{
    /// <summary>
    /// Manages the UI input fields, warnings, and buttons for the account-related functionalities.
    /// </summary>
    public class AccountUIInputFields : MonoBehaviour
    {
        // input fields
        [SerializeField] private TMP_InputField loginEmailInputField;
        [SerializeField] private TMP_InputField loginPasswordInputField;
        [SerializeField] private TMP_InputField registerEmailInputField;
        [SerializeField] private TMP_InputField registerPasswordInputField;
        [SerializeField] private TMP_InputField FirstNameInputField;
        [SerializeField] private TMP_InputField LastNameInputField;
        [SerializeField] private TMP_InputField passwordConfirmInputField;
        [SerializeField] private Button securityButtonRegister;
        [SerializeField] private Button securityButtonLogin;
        [SerializeField] private Button referRegisterButton;
        [SerializeField] private Button referLoginButton;
        [SerializeField] private Button registerButton;
        [SerializeField] private Button loginButton;
        [SerializeField] private GameObject registerScreen;
        [SerializeField] private GameObject loginScreen;

        // warnings
        [SerializeField] private GameObject warningAllFields;
        [SerializeField] private GameObject warningPWEIncorrect;
        [SerializeField] private GameObject warningIncorrectEmail;
        [SerializeField] private GameObject warningWeakPW;
        [SerializeField] private GameObject warningDissimilarPW;

        // images 
        [SerializeField] private Sprite seen;
        [SerializeField] private Sprite hidden;
        private bool visibilityregsec;
        private bool visibilitylogsec;


        // Start is called before the first frame update
        void Start()
        {
            warningAllFields.SetActive(false);
            warningPWEIncorrect.SetActive(false);
            warningIncorrectEmail.SetActive(false);
            warningWeakPW.SetActive(false);
            warningDissimilarPW.SetActive(false);

            warningDissimilarPW.SetActive(false);
            registerScreen.SetActive(false);
            loginScreen.SetActive(true);
            visibilityregsec = false;
            visibilitylogsec = false;
        }

        // Update is called once per frame
        void Update()
        {
            

        }

        void Awake()
        {
            securityButtonRegister.onClick.AddListener(delegate () { this.OnRSecurityButtonClick(); });
            securityButtonLogin.onClick.AddListener(delegate () { this.OnLSecurityButtonClick(); });
            referRegisterButton.onClick.AddListener(delegate () { OnReferRegisterButtonClick(); });
            referLoginButton.onClick.AddListener(delegate () { this.OnReferLoginButtonClick(); });
            registerButton.onClick.AddListener(delegate () { this.OnRegisterButtonClick(); });
            loginButton.onClick.AddListener(delegate () { this.OnLoginButtonClick(); });
        }
        /// <summary>
        /// Handles the logic when the login button is clicked.
        /// </summary>
        public void OnLoginButtonClick()
        {
           // TODO for checking if password and email correspond with credentials saved on the server
        }

        /// <summary>
        /// Switches to the register screen.
        /// </summary>
        public void OnReferRegisterButtonClick()
        {
            registerScreen.SetActive(true);
            loginScreen.SetActive(false);
        }
        /// <summary>
        /// Switches to the login screen.
        /// </summary>
        public void OnReferLoginButtonClick()
        {
            registerScreen.SetActive(false);
            loginScreen.SetActive(true);
        }
        /// <summary>
        /// Handles the logic when the security button for registration is clicked.
        /// The register security button changes image and the password becomes visible
        /// </summary>
        public void OnRSecurityButtonClick()
        {
            Image securityimage1=securityButtonRegister.transform.GetChild(0).GetComponent<Image>();
            

            if (visibilityregsec==false)
            {
                registerPasswordInputField.contentType = TMP_InputField.ContentType.Standard;
                securityimage1.sprite = seen;
                visibilityregsec = true;

            }
            else
            {
                registerPasswordInputField.contentType = TMP_InputField.ContentType.Password;
                securityimage1.sprite = hidden;
                visibilityregsec = false;
            }

        }

        /// <summary>
        /// Handles the logic when the security button for login is clicked.
        /// The login security button changes image and the password becomes visible
        /// </summary>
        public void OnLSecurityButtonClick()
        {
            Image securityimage2 = securityButtonLogin.transform.GetChild(0).GetComponent<Image>();

            if (visibilitylogsec==false)
            {
                loginPasswordInputField.contentType = TMP_InputField.ContentType.Standard;
                securityimage2.sprite = seen;
                visibilitylogsec = true;

            }
            else
            {
                loginPasswordInputField.contentType = TMP_InputField.ContentType.Password;
                securityimage2.sprite = hidden;
                visibilitylogsec = false;
            }

        }

        /// <summary>
        /// Handles the logic when the register button is clicked.
        /// </summary>
        public void OnRegisterButtonClick()
        {
            warningAllFields.SetActive(false);
            //TODO : warningPWEIncorrect.SetActive(false); and something with it
            warningIncorrectEmail.SetActive(false);
            warningWeakPW.SetActive(false);
            warningDissimilarPW.SetActive(false);

            // email and password regular expressions 
            Regex passwordRegex = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*()_+{}\[\]:;<>,.?~\\-]).{12,}$");
            Regex emailRegex = new Regex(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$");


            string registeremail = registerEmailInputField.text;
            string registerpassword = registerPasswordInputField.text;
            string registerfirstname = FirstNameInputField.text;
            string registerlastname = LastNameInputField.text;
            string registerconfirmpassword = passwordConfirmInputField.text;

            // checks if all fields are filled in 
            if (string.IsNullOrEmpty(registeremail) || string.IsNullOrEmpty(registerpassword) ||
                string.IsNullOrEmpty(registerfirstname) || string.IsNullOrEmpty(registerlastname) ||
                string.IsNullOrEmpty(registerconfirmpassword))
            {
                warningAllFields.SetActive(true);

                return; // Stop execution if any field is empty
            }

            // checks if password and confirm password are the same 
            if (registerpassword != registerconfirmpassword)
            {
                warningDissimilarPW.SetActive(true);
                return;
            }

            // checks if password is atleast 12 characters and 
            // A combination of uppercase letters, lowercase letters, numbers, and symbols.
            if (!passwordRegex.IsMatch(registerpassword) || registerpassword.Length < 12)
            {
                warningWeakPW.SetActive(true);
                return;

            }

            // checks if the email address is in an 'email-format'
            if (!emailRegex.IsMatch(registeremail))
            {
                warningIncorrectEmail.SetActive(true);
                return;
            }

        }

    }
}
