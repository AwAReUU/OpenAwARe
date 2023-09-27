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

    // Update is called once per frame
    void Update()
    {

    }

    private async void Login()
    {
        await Client.GetInstance().Login(adress, new User { email = this.email, password = this.password });

        Debug.Log(await Client.GetInstance().CheckLogin(adress));
    }
}
