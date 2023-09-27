using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Text;
using System.Linq;
// using SimpleJSON;


public class client : MonoBehaviour
{
    public string adress;

    public string email;
    public string password;
    


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    private void Login() {
        UnityWebRequest req = new UnityWebRequest();
        req.downloadHandler = new DownloadHandlerBuffer();

        req.url = adress + "/auth/login";
    }
}
