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

           
        }

        // Update is called once per frame
        void Update()
        {
            

        }

        void Awake()
        {
            securityButton.onClick.AddListener(delegate () { this.OnSecurityButtonClick(); });
        }
        public void OnLoginButtonClick()
        {
  
        }

        public void OnSecurityButtonClick()
        {
            registerPasswordInputField.contentType = TMP_InputField.ContentType.Standard;

        }

        public void OnRegisterButtonClick()
        {

            string registeremail = registerEmailInputField.text;
            string registerpassword = registerPasswordInputField.text;
            string registerfirstname = FirstNameInputField.text;
            string registerlastname = LastNameInputField.text;
            string registerconfirmpassword = passwordConfirmInputField.text;

            if (string.IsNullOrEmpty(registeremail) || string.IsNullOrEmpty(registerpassword) ||
                string.IsNullOrEmpty(registerfirstname) || string.IsNullOrEmpty(registerlastname) ||
                string.IsNullOrEmpty(registerconfirmpassword))
            {
                Debug.LogError("Please fill in all required fields.");
                return; // Stop execution if any field is empty
            }

        }


    }
}
