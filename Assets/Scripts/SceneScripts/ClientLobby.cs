using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientLobby : MonoBehaviour
{
    private GameObject inputServer;
    private GameObject inputUsername;
    
    private GameObject usernameWelcome;

    private GameObject chat;

    private Client client;
    
    // Start is called before the first frame update
    void Start()
    {
        client = GameObject.Find("ClientManager").GetComponent<Client>();

        inputServer = GameObject.Find("InputServer");
        inputUsername = GameObject.Find("InputUsername");
        chat = GameObject.Find("Chat");

        inputServer.SetActive(false);
        inputUsername.SetActive(true);
        chat.SetActive(false);
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

        //ShowUsernameWelcome();
        ShowChat();
        // Load lobby
    }

    private void ShowChat()
    {
        chat.SetActive(true);
    }
}
