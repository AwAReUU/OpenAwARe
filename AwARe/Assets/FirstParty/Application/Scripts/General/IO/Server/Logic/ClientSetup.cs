// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

using UnityEngine;

namespace AwARe.Server.Logic
{
    /// <summary>
    /// This script should be attached to a gameobject at the start of the application.
    /// 
    /// This is used for settting up the client-server connection.
    /// 
    /// Any server adress should point to a server running the "{ROOTDIR}/Server" node package.
    /// </summary>
    public class ClientSetup : MonoBehaviour
    {
        [Header("Remote Server Adress:")]
        [SerializeField]
        /// <value> The remote server adress. This should be an ip adress or a DNS name. </value>
        private string remoteAdress = "131.211.32.146";
        [SerializeField]
        /// <value> The port that is opened on the remote server. Default is 8000. </value>
        private int remotePort = 8000;

        [Header("Debug Server Adress:")]
        [SerializeField]
        /// <value> 
        /// The debug server adress. This should be an ip adress, a DNS name 
        /// or "localhost" (if runnning on your local machine). 
        /// </value>
        private string debugAdress = "localhost";
        [SerializeField]
        /// <value> The port that is opened on the debug server. Default is 8000. </value>
        private int debugPort = 8000;

        [Header("Setttings:")]
        [SerializeField]
        /// <value> If true, use the debug server. </value>
        private bool debug = false;

        /// <summary>
        /// At startup initialise the client-server connection.
        /// </summary>
        public async void Awake()
        {
            Client.Init(this.Adress());

            // TEMP: For now login with fake admin account:
            await Client.GetInstance().Login(new User
            {
                email = "admin@aware.nl",
                password = "1234test"
            });
            Debug.Log("Logged in: " + await Client.GetInstance().CheckLogin());
        }

        /// <summary>
        /// Get the server adress including the port number.
        /// </summary>
        private string Adress()
        {
            if (debug)
            {
                return debugAdress + ":" + debugPort;
            }
            else
            {
                return remoteAdress + ":" + remotePort;
            }
        }
    }
}