using UnityEngine;
using Proyecto26;
using System;
using System.Threading.Tasks;

// ----------------------------------------------------------------------------

public class Client
{

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

    public static Client getInstance()
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
    private string password;

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
                Debug.Log("[client]: Logged in");

                ret.SetResult(true);
            }).Catch(err =>
            {
                Debug.LogError("[client]: Failed to login: " + err.Message);

                ret.SetResult(false);
            });
        });

    }

    // ----------------------------------------------------------------------------

    private static async Task<T> AwaitRSGPromise<T>(Action<TaskCompletionSource<T>> action)
    {
        // !!!
        // Volgens mij is dit heel omslachtig en niet efficient, maar weet niet hoe ik anders RestClient.Post(..) await.
        //
        // Om een of andere rede kun je RSG.IPromise alleen d.m.v. callbacks gebruiken...
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
struct TokenResponse
{
    public string accessToken;
    public string refreshToken;
}

public enum Response
{
    OK,
    ERR
}
