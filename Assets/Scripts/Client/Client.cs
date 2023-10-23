using UnityEngine;
using Proyecto26;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json;

// ----------------------------------------------------------------------------

public class Client
{
    // ----------------------------------------------------------------------------
    // No login needed:

    // Returns true if registration succeeded
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

    private static Client instance;

    public static void Init(string adress, User user)
    {
        Client.instance = new Client(adress, user);
    }

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

    private readonly string adress;
    private User user;

    private string accessToken;
    private string refreshToken;


    private Client(string adress, User user)
    {
        this.user = user;
        this.adress = adress;

        this.Login();
    }

    // Returns true if login succeeded
    private async Task<bool> Login()
    {
        string url = adress + "/auth/login";

        return await AwaitRSGPromise<bool>(ret =>
        {
            RestClient.Post<TokenResponse>(url, this.user).Then(response =>
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

    private Task<bool> Logout()
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

    // Returns true if logged in. Returns false if not logged in or if there are connections issues.
    public static Task<bool> CheckLogin()
    {
        var client = Client.GetInstance();
        string url = client.adress + "/auth/check";

        return AwaitRSGPromise<bool>(ret =>
        {
            var rh = new RequestHelper
            {
                Uri = url
            };

            RestClient.Get(client.Authorize(rh)).Then(response =>
            {
                ret.SetResult(true);
            }).Catch(err =>
            {
                ret.SetResult(false);
            });
        });

    }

    private Task<bool> Refresh()
    {
        string url = adress + "/auth/refreshToken";

        return AwaitRSGPromise<bool>(ret =>
        {
            RestClient.Post<TokenResponse>(url, new RefreshRequest { email = this.user.email, token = this.refreshToken }).Then(response =>
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

    private async Task<bool> RestoreSession()
    {
        if (!await this.Refresh())
        {
            return await this.Login();
        }
        return false;
    }

    private RequestHelper Authorize(RequestHelper rh)
    {
        rh.Headers.Add("authorization", string.Format("{0} {1}", this.refreshToken, this.accessToken));
        return rh;
    }

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

    public async void SendPostRequest<B>(string url, B body, Action<Dictionary<string, string>> on_then, Action<Exception> on_catch)
    {
        await AwaitRSGPromise<bool>(ret =>
        {
            var rh = new RequestHelper
            {
                Uri = Client.GetInstance().adress + url,
                Body = body,
            };
            RestClient.Post(Client.GetInstance().Authorize(rh)).Then(response =>
            {
                var json = JsonConvert.DeserializeObject<Dictionary<string, string>>(response.Text);
                on_then(json);

                ret.SetResult(true);
            }).Catch(err =>
            {
                on_catch(err);

                ret.SetResult(false);
            });
        });
    }

    public async void SendGetRequest<B>(string url, B body, Action<Dictionary<string, string>> on_then, Action<Exception> on_catch)
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
                var json = JsonConvert.DeserializeObject<Dictionary<string, string>>(response.Text);
                on_then(json);

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

    public static Request<B> Post<B>(string url, B body)
    {
        return new Request<B>(RequestType.POST, url, body);
    }

    public static Request<B> Get<B>(string url, B body)
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

public class Request<B>
{
    private readonly RequestType type;

    private readonly string url;

    private readonly B body;

    private Action<Dictionary<string, string>> on_then;
    private Action<Exception> on_catch = delegate { };

    public Request(RequestType type, string url, B body)
    {
        this.type = type;
        this.url = url;
        this.body = body;
    }

    public Request<B> Then(Action<Dictionary<string, string>> on_then)
    {
        this.on_then = on_then;
        return this;
    }

    public Request<B> Catch(Action<Exception> on_catch)
    {
        this.on_catch = on_catch;
        return this;
    }

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

