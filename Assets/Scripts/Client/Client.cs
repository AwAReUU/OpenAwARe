using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;
using Proyecto26;

// ----------------------------------------------------------------------------

public struct AccountDetails
{
    string firstName;
    string lastName;
    string email;
    string password;
    string confirmPassword;
}

// ----------------------------------------------------------------------------

public class Client
{
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

    public static void Register(string adress, AccountDetails account)
    {
        // TODO
    }

    // ----------------------------------------------------------------------------
    // Instance:

    private string accesToken;
    private string refreshToken;

    private string email;
    private string password;

    private Client()
    {
        this.Login();
    }

    private void Login()
    {

    }

    private void RefreshToken()
    {

    }

    private void SendPostRequest()
    {
        string url = "";
        RestClient.Post(url, newPost).Then(response =>
        {
            EditorUtility.DisplayDialog("Status", response.StatusCode.ToString(), "Ok");
        });
    }
}
