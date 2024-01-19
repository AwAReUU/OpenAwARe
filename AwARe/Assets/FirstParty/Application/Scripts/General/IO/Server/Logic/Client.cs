using UnityEngine;
using Proyecto26;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json;

// ----------------------------------------------------------------------------

namespace AwARe.Server.Logic
{
    /// <summary>
    /// A singleton that holds a connection with the server.
    /// This class can be used to login and register on the server.
    /// It can also send requests to the server. The Post() and Get() methods
    /// are the easiest methods to send a Request.
    ///
    /// <example>
    /// For example:
    /// <code>
    /// Client.Init("localhost:8000", new User(..));
    /// string search = "{ 'input': 'orange' }";
    /// Client.Post("/ingredients/search", search)
    ///     .Then(res => Debug.Log(res.Get("list")))
    ///     .Catch(err => Debug.LogError(err)) // Optional
    ///     .Send();
    /// </code>
    /// </example>
    ///
    /// For a more detailed explanation on how the server works, please read the server documentation.
    /// </summary>
    public class Client
    {
        // ----------------------------------------------------------------------------
        // Static methods. No login needed, so it is not part of the singleton instance:

        /// <summary>
        /// Register an account on the server.
        /// </summary>
        /// <returns>
        /// Returns true if registration succeeded.
        /// </returns>
        public static Task<bool> Register(string adress, AccountDetails account)
        {
            string url = adress + "/auth/register";

            return AwaitRSGPromise<bool>(ret =>
            {
                RestClient.Post(url, account).Then(response =>
                {
                    Debug.Log("[client]: Registration is succesfull");

                    ret.SetResult(true);
                }).Catch(err =>
                {
                    Debug.LogError("[client]: Failed to register user: " + err.Message);
                    ret.SetResult(false);
                });
            });
        }

        // ----------------------------------------------------------------------------
        // Singleton:

        /// <value> 
        /// The singleton instance.
        /// </value> 
        private static Client instance;

        /// <summary>
        /// Call this before GetInstance().
        /// This should be called only once during the entire application runtime!!!
        /// </summary>
        public static void Init(string adress)
        {
            Client.instance = new Client(adress);
        }

        /// <summary>
        /// Access to the singleton instance.
        /// Init() must be called first!
        /// </summary>
        public static Client GetInstance()
        {
            if (Client.instance == null)
            {
                Debug.LogAssertion("Client.Init() must be called first");
                // TODO: throw error
            }
            return Client.instance;
        }

        // ----------------------------------------------------------------------------
        // Instance:

        /// <value> 
        /// The server adress
        /// </value> 
        private readonly string adress;

        /// <value> 
        /// The user's email adress
        /// </value> 
        private string userEmail;

        /// <value> 
        /// This token is retrieved from the server. Use this in the 'authorization' header, to access protected routes.
        /// </value> 
        private string accessToken;

        /// <value> 
        /// This token is retrieved from the server. Use this in the body of a refresh login request.
        /// </value> 
        private string refreshToken;


        private Client(string adress)
        {
            this.adress = adress;
        }

        /// <summary>
        /// Login on the server.
        /// </summary>
        /// <returns>
        /// Returns true if login succeeded.
        /// </returns>
        public async Task<bool> Login(User user)
        {
            this.userEmail = user.email;

            string url = adress + "/auth/login";

            return await AwaitRSGPromise<bool>(ret =>
            {
                RestClient.Post<TokenResponse>(url, user).Then(response =>
                {
                    this.accessToken = response.accessToken;
                    this.refreshToken = response.refreshToken;

                    Debug.Log("[client]: Logged in");

                    ret.SetResult(true);
                }).Catch(err =>
                {
                    Debug.LogError("[client]: Failed to login: " + err.Message);

                    ret.SetResult(false);
                });
            });
        }

        /// <summary>
        /// Logout on the server.
        /// </summary>
        /// <returns>
        /// Returns true if logout succeeded.
        /// </returns>
        public Task<bool> Logout()
        {
            string url = adress + "/auth/logout";

            return AwaitRSGPromise<bool>(ret =>
            {
                RestClient.Post(url, new EmptyResponse { }).Then(response =>
                {
                    Debug.Log("[client]: Logged out");

                    ret.SetResult(true);
                }).Catch(err =>
                {
                    Debug.LogError("[client]: Failed to logout: " + err.Message);

                    ret.SetResult(false);
                });
            });
        }

        /// <summary>
        /// Check if a login session is active.
        /// </summary>
        /// <returns>
        /// Returns true if logged in. Returns false if not logged in or if there are connections issues.
        /// </returns>
        public Task<bool> CheckLogin()
        {
            string url = this.adress + "/auth/check";

            return AwaitRSGPromise<bool>(ret =>
            {
                var rh = new RequestHelper
                {
                    Uri = url
                };

                RestClient.Get(this.Authorize(rh)).Then(response =>
                {
                    ret.SetResult(true);
                }).Catch(err =>
                {
                    ret.SetResult(false);
                });
            });

        }

        /// <summary>
        /// Refresh the login session.
        /// </summary>
        /// <returns>
        /// Returns true if refreshing succeeded.
        /// </returns>
        private Task<bool> Refresh()
        {
            string url = adress + "/auth/refreshToken";

            return AwaitRSGPromise<bool>(ret =>
            {
                RestClient.Post<TokenResponse>(url, new RefreshRequest { email = this.userEmail, token = this.refreshToken }).Then(response =>
                {
                    this.accessToken = response.accessToken;
                    this.refreshToken = response.refreshToken;

                    Debug.Log("[client]: Refreshed login session");

                    ret.SetResult(true);
                }).Catch(err =>
                {
                    Debug.LogError("[client]: Failed to refresh login session: " + err.Message);

                    ret.SetResult(false);
                });
            });
        }

        /// <summary>
        /// A helper method to add the authorization header to a request.
        /// </summary>
        /// <returns>
        /// Returns the request with the included authorization header.
        /// </returns>
        private RequestHelper Authorize(RequestHelper rh)
        {
            rh.Headers.Add("authorization", string.Format("{0} {1}", this.refreshToken, this.accessToken));
            return rh;
        }

        /// <summary>
        /// A helper method to await the Promises from the library that is used to send html requests.
        /// The library only supports using a callback mechanism, 
        /// but that makes sending request in a specific order very complicated.
        /// </summary>
        /// <returns>
        /// Returns a Task that can be awaited.
        /// </returns>
        private static async Task<T> AwaitRSGPromise<T>(Action<TaskCompletionSource<T>> action)
        {
            // !!!
            // Volgens mij is dit heel omslachtig en niet efficient, maar weet niet hoe ik anders RestClient.Post(..) await.
            //
            // Om een of andere rede kun je het alleen d.m.v. callbacks gebruiken...
            var tcs = new TaskCompletionSource<T>();
            action(tcs);
            return await tcs.Task;
        }

        /// <summary>
        /// Send a Post request.
        /// </summary>
        public async void SendPostRequest<B>(string url, B body, Action<string> on_then, Action<Exception> on_catch)
        {
            await AwaitRSGPromise<bool>(ret =>
            {
                var rh = new RequestHelper
                {
                    Uri = Client.GetInstance().adress + "/" + url,
                    Body = body,
                };
                RestClient.Post(Client.GetInstance().Authorize(rh)).Then(response =>
                {
                    on_then(response.Text);

                    ret.SetResult(true);
                }).Catch(err =>
                {
                    on_catch(err);

                    ret.SetResult(false);
                });
            });
        }

        /// <summary>
        /// Send a Get request.
        /// </summary>
        public async void SendGetRequest<B>(string url, B body, Action<string> on_then, Action<Exception> on_catch)
        {
            await AwaitRSGPromise<bool>(ret =>
            {
                var rh = new RequestHelper
                {
                    Uri = Client.GetInstance().adress + url,
                    Body = body,
                };
                RestClient.Get(Client.GetInstance().Authorize(rh)).Then(response =>
                {
                    on_then(response.Text);

                    ret.SetResult(true);
                }).Catch(err =>
                {
                    on_catch(err);

                    ret.SetResult(false);
                });
            });
        }

        // ----------------------------------------------------------------------------
        // Helper methods:

        /// <summary>
        /// Send a Post request using the Request helper class.
        /// </summary>
        /// <returns>
        /// Returns a Request helper class.
        /// </returns>
        public Request<B> Post<B>(string url, B body)
        {
            return new Request<B>(RequestType.POST, url, body);
        }

        /// <summary>
        /// Send a Get request using the Request helper class.
        /// </summary>
        /// <returns>
        /// Returns a Request helper class.
        /// </returns>
        public Request<B> Get<B>(string url, B body)
        {
            return new Request<B>(RequestType.GET, url, body);
        }
    }

    // ----------------------------------------------------------------------------

    public enum RequestType
    {
        POST,
        GET
    }

    /// <summary>
    /// A helper class to send html requests. Is uses the Builder pattern.
    /// Instead of Build() at the end, call Send().
    /// </summary>
    public class Request<B>
    {
        /// <value> 
        /// The HTML request type.
        /// </value> 
        private readonly RequestType type;

        /// <value> 
        /// The url containing the server adress + the requested route.
        /// </value> 
        private readonly string url;

        /// <value> 
        /// The HTML request body. 
        /// </value> 
        private readonly B body;

        /// <value> 
        /// The action to run if the request was successfull. 
        /// </value> 
        private Action<string> on_then;

        /// <value> 
        /// The action to run if the request was not successfull. 
        /// </value> 
        private Action<Exception> on_catch = delegate { };

        public Request(RequestType type, string url, B body)
        {
            this.type = type;
            this.url = url;
            this.body = body;
        }

        /// <summary>
        /// Set the on_then action. This is not optionial.
        /// </summary>
        public Request<B> Then(Action<string> on_then)
        {
            this.on_then = on_then;
            return this;
        }

        /// <summary>
        /// Set the on_catch action. This is optional.
        /// </summary>
        public Request<B> Catch(Action<Exception> on_catch)
        {
            this.on_catch = on_catch;
            return this;
        }

        /// <summary>
        /// Send the request. Should be called at the end.
        /// </summary>
        public void Send()
        {
            switch (this.type)
            {
                case RequestType.GET:
                    this.Get();
                    break;
                case RequestType.POST:
                    this.Post();
                    break;
            }
        }

        private void Post()
        {
            Client.GetInstance().SendPostRequest(this.url, this.body, this.on_then, this.on_catch);
        }

        private void Get()
        {
            Client.GetInstance().SendGetRequest(this.url, this.body, this.on_then, this.on_catch);
        }
    }

    // ----------------------------------------------------------------------------
    // Helper classes to serialize into JSON:

    [Serializable]
    public struct AccountDetails
    {
        public string firstName;
        public string lastName;
        public string email;
        public string password;
        public string confirmPassword;
    }

    [Serializable]
    public struct User
    {
        public string email;
        public string password;
    }


    [Serializable]
    public struct RefreshRequest
    {
        public string email;
        public string token;
    }

    [Serializable]
    struct TokenResponse
    {
        public string accessToken;
        public string refreshToken;
    }

    [Serializable]
    struct EmptyResponse { }
}


// Init with server adress
// (During runtime) Login
// Send req. and catch a 440 for refreshing and try again.
// On_then if successfull
// On_catch if not, supply error code