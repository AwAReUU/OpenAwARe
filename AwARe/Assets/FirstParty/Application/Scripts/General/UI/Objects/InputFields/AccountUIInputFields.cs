using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AwARe
{
    public class AccountUIInputFields : MonoBehaviour
    {
        // input fields
        public InputField loginEmailInputField;
        public InputField loginPasswordInputField;
        public InputField registerEmailInputField;
        public InputField registerPasswordInputField;
        public InputField FirstNameInputField;
        public InputField LastNameInputField;
        public InputField passwordConfirmInputField;

        // 


        // Start is called before the first frame update
        void Start()
        {
            string registeremail = registerEmailInputField.text;
            string registerpassword = registerPasswordInputField.text;
            string registerfirstname = FirstNameInputField.text;
            string registerlastname = LastNameInputField.text;
            string registerconfirmpassword = passwordConfirmInputField.text;

        }

        // Update is called once per frame
        void Update()
        {
        
        }
        public void OnLoginButtonClick()
        {
  
        }

        public void OnRegisterButtonClick()
        {

        }


    }
}
