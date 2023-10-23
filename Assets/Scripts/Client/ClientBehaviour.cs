using Unity.VisualScripting;
using UnityEngine;

public class ClientBehaviour : MonoBehaviour
{
    public string adress;

    public string email;
    public string password;

    // Start is called before the first frame update
    void Start()
    {
        this.Login();
    }

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
