using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

namespace AwARe
{
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
        [SerializeField] private Button securityButton;
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

        private bool IsStrongPassword(string password)
        {
            // Password should be at least 12 characters long
            if (password.Length < 12)
            {
                return false;
            }

            // Password should have a combination of uppercase letters, lowercase letters, numbers, and symbols
            Regex uppercaseRegex = new Regex(@"[A-Z]");
            Regex lowercaseRegex = new Regex(@"[a-z]");
            Regex digitRegex = new Regex(@"\d");
            Regex symbolRegex = new Regex(@"[!@#$%^&*()_+{}\[\]:;<>,.?~\\-]");

            return uppercaseRegex.IsMatch(password) && lowercaseRegex.IsMatch(password) &&
                digitRegex.IsMatch(password) && symbolRegex.IsMatch(password);
        }

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
        }

        // Update is called once per frame
        void Update()
        {
            

        }

        void Awake()
        {
            securityButton.onClick.AddListener(delegate () { this.OnSecurityButtonClick(); });
            referRegisterButton.onClick.AddListener(delegate () { OnReferRegisterButtonClick(); });
            referLoginButton.onClick.AddListener(delegate () { this.OnReferLoginButtonClick(); });
            registerButton.onClick.AddListener(delegate () { this.OnRegisterButtonClick(); });
        }
        public void OnLoginButtonClick()
        {
           
        }

        public void OnReferRegisterButtonClick()
        {
            registerScreen.SetActive(true);
            loginScreen.SetActive(false);
        }

        public void OnReferLoginButtonClick()
        {
            registerScreen.SetActive(false);
            loginScreen.SetActive(true);
        }

        public void OnSecurityButtonClick()
        {
            registerPasswordInputField.contentType = TMP_InputField.ContentType.Standard;
            securityButton.image.sprite = ;

        }

        public void OnRegisterButtonClick()
        {
            warningAllFields.SetActive(false);
            //warningPWEIncorrect.SetActive(false);
            warningIncorrectEmail.SetActive(false);
            warningWeakPW.SetActive(false);
            warningDissimilarPW.SetActive(false);

            string registeremail = registerEmailInputField.text;
            string registerpassword = registerPasswordInputField.text;
            string registerfirstname = FirstNameInputField.text;
            string registerlastname = LastNameInputField.text;
            string registerconfirmpassword = passwordConfirmInputField.text;

            Regex emailRegex = new Regex(@"@");

            if (string.IsNullOrEmpty(registeremail) || string.IsNullOrEmpty(registerpassword) ||
                string.IsNullOrEmpty(registerfirstname) || string.IsNullOrEmpty(registerlastname) ||
                string.IsNullOrEmpty(registerconfirmpassword))
            {
                Debug.LogError("Please fill in all required fields.");
                warningAllFields.SetActive(true);

                return; // Stop execution if any field is empty
            }

            if (registerpassword != registerconfirmpassword)
            {
                Debug.LogError("Please make sure the passwords are the same");
                warningDissimilarPW.SetActive(true);
                return;
            }

            if (IsStrongPassword(registerpassword)!)
            {
                Debug.LogError("Password should have a combination of uppercase letters, lowercase letters, numbers, and symbols and be atleast 12 characters.");
                warningWeakPW.SetActive(true);
                return;
            }

            /*if ((emailRegex.IsMatch(registeremail)!))
            {
                Debug.LogError("Invalid email address");
                warningIncorrectEmail.SetActive(true);
                return;
            }*/

            

        }


    }
}
