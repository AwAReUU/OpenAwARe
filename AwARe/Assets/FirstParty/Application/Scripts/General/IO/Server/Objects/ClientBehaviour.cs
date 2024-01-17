using Unity.VisualScripting;
using UnityEngine;

using AwARe.Server.Logic;

namespace AwARe.Server.Objects
{
    public class ClientBehaviour : MonoBehaviour
    {
        /// <value>
        /// The server adress.
        /// </value>
        public string adress;

        /// <value>
        /// The user email.
        /// </value>
        public string email;
        /// <value>
        /// The user password.
        /// </value>
        public string password;

        // Start is called before the first frame update
        void Start()
        {
            this.Login();
        }

        /// <summary>
        /// Login to the server.
        /// </summary>
        private async void Login()
        {
            Client.Init(this.adress, new User { email = this.email, password = this.password });

            string search = "orange";
            Client.Get("/ingr/search", search).Then(res =>
            {
                Debug.Log(res);
            }).Catch(err =>
            {
                Debug.Log(err);
            }).Send();
        }
    }
}
