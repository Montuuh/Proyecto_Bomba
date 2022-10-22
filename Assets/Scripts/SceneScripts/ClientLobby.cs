using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientLobby : MonoBehaviour
{
    private GameObject inputServer;
    private GameObject inputUsername;
    
    private GameObject usernameWelcome;
    
    // Start is called before the first frame update
    void Start()
    {
        inputServer = GameObject.Find("InputServer");
        inputUsername = GameObject.Find("InputUsername");
        usernameWelcome = GameObject.Find("UsernameWelcome");

        inputServer.SetActive(false);
        inputUsername.SetActive(true);
        usernameWelcome.SetActive(false);
    }

    public void OnValidUsername()
    {
        inputServer.SetActive(true);
        inputUsername.SetActive(false);
    }

    public void OnValidServer()
    {
        inputServer.SetActive(false);
        inputUsername.SetActive(false);

        ShowUsernameWelcome();
        // Load lobby
    }

    private void ShowUsernameWelcome()
    {
        usernameWelcome.SetActive(true);
        switch (GameObject.Find("ClientManager").GetComponent<ServerManager>().protocol)
        {
            case ServerManager.Protocol.TCP:
                usernameWelcome.GetComponent<TMPro.TextMeshProUGUI>().text = "Welcome " + GameObject.Find("ClientManager").GetComponent<TCPClient>().userName;
                break;
            case ServerManager.Protocol.UDP:
                usernameWelcome.GetComponent<TMPro.TextMeshProUGUI>().text = "Welcome " + GameObject.Find("ClientManager").GetComponent<UDPClient>().userName;
                break;
        }
    }
}
