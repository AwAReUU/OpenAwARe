// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using AwARe.InterScenes.Objects;
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

        /// <value> If true, a coroutine is already running to refresh the login session. </value>
        private bool refreshing = false;

        /// <summary>
        /// At startup initialise the client-server connection.
        /// </summary>
        public void Awake()
        {
            // Keep the scene alive
            SceneSwitcher.Get().Keepers.Add(gameObject.scene);

            Client.Init(this.Adress());
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

        /// <summary>
        /// Refresh the login session every x seconds.
        /// </summary>
        public void InvokeRefreshLoginSession(int interval)
        {
            if (!refreshing)
            {
                refreshing = true;
                RefreshLoginSession(interval);
            }
        }


        /// <summary>
        /// Refresh the login session every x seconds.
        /// </summary>
        private async void RefreshLoginSession(int interval)
        {
            while (await Client.GetInstance().Refresh())
            {
                await Task.Delay(interval * 1000);
            }
            refreshing = false;
        }
    }
}
