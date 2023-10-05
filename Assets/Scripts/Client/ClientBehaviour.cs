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

        Debug.Log("test" + await Client.CheckLogin());


        // Client.Post("/ingredients/get").Then(res =>
        // {
        //     res.Text // <--json
        // }).Catch(err =>
        // {

        // }).Send();
    }
}
