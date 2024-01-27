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
    /// are the preferred way to send a Request.
    ///
    /// <example>
    /// For example:
    /// <code>
    /// Client.Init("localhost:8000");
    /// Client.GetInstance().Login(new User {..});
    /// string search = "{ 'input': 'orange' }";
    /// Client.GetInstance().Post("/ingredients/search", search)
    ///     .Then(res => Debug.Log(res.Get("list")))
    ///     .Catch(err => Debug.LogError(err)) 
    ///     .Send();
    /// </code>
    /// </example>
    ///
    /// For a more detailed explanation on how the server works, please read the server documentation.
    /// </summary>
    public class Client
    {
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
        /// Register an account on the server.
        /// </summary>
        /// <returns>
        /// Returns true if registration succeeded.
        /// </returns>
        public Task<bool> Register(AccountDetails account)
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


        /// <summary>
        /// Login on the server.
        /// </summary>
        /// <returns>
        /// Returns true if login succeeded.
        /// </returns>
        public Task<bool> Login(User user)
        {
            this.userEmail = user.email;

            string url = adress + "/auth/login";

            return AwaitRSGPromise<bool>(ret =>
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
        public Task<bool> Refresh()
        {
            string url = adress + "/auth/refresh";

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
        /// A helper method to await RSGPromises that are used to send html requests.
        /// The library only supports using a callback mechanism, 
        /// but that makes sending request in a specific order very complicated.
        /// </summary>
        /// <return>
        /// Returns T.
        /// </jreturn>
        private static Task<T> AwaitRSGPromise<T>(Action<TaskCompletionSource<T>> action)
        {
            // !!!
            // Volgens mij is dit heel omslachtig en niet efficient, maar weet niet hoe ik anders RestClient.Post(..) await.
            //
            // Om een of andere rede kun je het alleen d.m.v. callbacks gebruiken...
            var tcs = new TaskCompletionSource<T>();
            action(tcs);
            return tcs.Task;
        }

        /// <summary>
        /// Send a Post request.
        /// </summary>
        public Task<R> SendPostRequest<B, R>(string url, B body, Func<string, R> on_then, Action<RequestException> on_catch)
        {
            return AwaitRSGPromise<R>(ret =>
            {
                var rh = new RequestHelper
                {
                    Uri = Client.GetInstance().adress + "/" + url,
                    Body = body,
                };
                Debug.Log(rh.Uri);
                RestClient.Post(Client.GetInstance().Authorize(rh)).Then(response =>
                {
                    var res = on_then(response.Text);

                    ret.SetResult(res);
                }).Catch(err =>
                {
                    on_catch(err as RequestException);

                    ret.SetResult(default);
                });
            });
        }

        /// <summary>
        /// Send a Get request.
        /// </summary>
        public Task<R> SendGetRequest<B, R>(string url, B body, Func<string, R> on_then, Action<RequestException> on_catch)
        {
            return AwaitRSGPromise<R>(ret =>
            {
                var rh = new RequestHelper
                {
                    Uri = Client.GetInstance().adress + "/" + url,
                    Body = body,
                };
                RestClient.Get(Client.GetInstance().Authorize(rh)).Then(response =>
                {
                    R res = on_then(response.Text);

                    ret.SetResult(res);
                }).Catch(err =>
                {
                    on_catch(err as RequestException);

                    ret.SetResult(default);
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
        public Request<B, R> Post<B, R>(string url, B body)
        {
            return new Request<B, R>(RequestType.POST, url, body);
        }

        /// <summary>
        /// Send a Get request using the Request helper class.
        /// </summary>
        /// <returns>
        /// Returns a Request helper class.
        /// </returns>
        public Request<B, R> Get<B, R>(string url, B body)
        {
            return new Request<B, R>(RequestType.GET, url, body);
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
    public class Request<B, R>
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
        private Func<string, R> on_then;

        /// <value> 
        /// The action to run if the request was not successfull. 
        /// </value> 
        private Action<RequestException> on_catch = delegate { };

        public Request(RequestType type, string url, B body)
        {
            this.type = type;
            this.url = url;
            this.body = body;
        }

        /// <summary>
        /// Set the on_then action. This is optionial.
        /// </summary>
        public Request<B, R> Then(Func<string, R> on_then)
        {
            this.on_then = on_then;
            return this;
        }

        /// <summary>
        /// Set the on_catch action. This is optional.
        /// </summary>
        public Request<B, R> Catch(Action<RequestException> on_catch)
        {
            this.on_catch = on_catch;
            return this;
        }

        /// <summary>
        /// Send the request. Should be called at the end.
        /// </summary>
        public Task<R> Send()
        {
            switch (this.type)
            {
                case RequestType.GET:
                    return Client.GetInstance().SendGetRequest(this.url, this.body, this.on_then, this.on_catch);
                case RequestType.POST:
                    return Client.GetInstance().SendPostRequest(this.url, this.body, this.on_then, this.on_catch);
                default:
                    return default;
            }
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

