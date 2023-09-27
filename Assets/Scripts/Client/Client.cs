using UnityEngine;
using Proyecto26;
using System;
using System.Threading.Tasks;

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

    public static Client GetInstance()
    {
        if (Client.instance == null)
        {
            instance = new Client();
        }
        return Client.instance;
    }

    // ----------------------------------------------------------------------------
    // Instance:

    private string accessToken;
    private string refreshToken;

    private string email;

    private Client()
    {
    }

    // Returns true if login succeeded
    public Task<bool> Login(string adress, User user)
    {
        string url = adress + "/auth/login";

        return AwaitRSGPromise<bool>(ret =>
        {
            RestClient.Post<TokenResponse>(url, user).Then(response =>
            {
                this.accessToken = response.accessToken;
                this.refreshToken = response.refreshToken;
                this.email = user.email;

                Debug.Log("[client]: Logged in");

                ret.SetResult(true);
            }).Catch(err =>
            {
                Debug.LogError("[client]: Failed to login: " + err.Message);

                ret.SetResult(false);
            });
        });
    }

    public Task<bool> Refresh(string adress)
    {
        string url = adress + "/auth/refreshToken";

        return AwaitRSGPromise<bool>(ret =>
        {
            RestClient.Post<TokenResponse>(url, new RefreshRequest { email = this.email, token = this.refreshToken }).Then(response =>
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


    public Task<bool> Logout(string adress)
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
    public Task<bool> CheckLogin(string adress)
    {
        string url = adress + "/auth/check";

        return AwaitRSGPromise<bool>(ret =>
        {
            var rh = new RequestHelper
            {
                Uri = url
            };

            RestClient.Get(Authorize(rh)).Then(response =>
            {
                ret.SetResult(true);
            }).Catch(err =>
            {
                ret.SetResult(false);
            });
        });

    }

    private RequestHelper Authorize(RequestHelper rh)
    {
        rh.Headers.Add("authorization", string.Format("{0} {1}", this.refreshToken, this.accessToken));
        return rh;
    }

    // ----------------------------------------------------------------------------

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

public enum Response
{
    OK,
    ERR
}
