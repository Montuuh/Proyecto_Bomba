using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ClientLobby : MonoBehaviour
{
    public TMP_InputField inputJoinIP;
    private string _inputJoinIP;
    private string _inputJoinCode;
    public Button buttonJoinToServer;
    
    public Button buttonCreateServer;
    
    public TMP_InputField inputUsername;
    private string _inputUsername;
    
    public Button buttonHostServer;
    public Button buttonJoinServer;

    public Button buttonStartGame;

    public TMP_InputField inputChat;
    public TMP_Text serverCodeIn;
    private string _inputChat;
    public GameObject chat;
    
    public Toggle checkboxIsTcp;
    private bool isTcp = false;

    public Button buttonBack;

    public Client client;

    private void Start()
    {
        DeactivateAll();
        inputUsername.gameObject.SetActive(true);
    }

    #region INPUT EVENTS

    // Show host/join buttons after username
    public void OnInputUsername()
    {
        _inputUsername = inputUsername.text;
        if (_inputUsername != null && _inputUsername != "")
        {
            client.clientData.userName = _inputUsername;
            DeactivateAll();
            buttonJoinServer.gameObject.SetActive(true);
            buttonHostServer.gameObject.SetActive(true);
        }
    }
    
    // Creates server
    public void OnClickCreateServer()
    {
        // check if servermanager doesn't exist

        GameObject serverGo = new GameObject("ServerManager", typeof(Server), typeof(DontDestroyMe));
        Server server = serverGo.GetComponent<Server>();
        server.StartServer(isTcp);
        
        client.ConnectToServer(IPAddressHelper.GetLocalIPAddress(), isTcp);

        DeactivateAll();
        chat.SetActive(true);
        serverCodeIn.text = client.serverCode;
    }

    // Not used for now, we are using join by server code
    public void OnInputJoinIP()
    {
        _inputJoinIP = inputJoinIP.text.ToUpper();
    }
    public void OnInputJoinServerCode()
    {
        _inputJoinCode = inputJoinIP.text.ToUpper();
    }

    // Joins server
    public void OnClickJoinToServer()
    {
        // If cant connect
        if (_inputJoinCode == null || _inputJoinCode == "" || !IPAddressHelper.IsValidServerCode(_inputJoinCode))
        {
            inputJoinIP.text = _inputJoinCode = "";
            inputJoinIP.placeholder.GetComponent<TMP_Text>().text = "Invalid code";
            return;
        }

        string ip = IPAddressHelper.DecodeIPAddress(_inputJoinCode);
        client.ConnectToServer(ip);

        DeactivateAll();
        chat.SetActive(true);
        serverCodeIn.text = client.serverCode;
        buttonStartGame.gameObject.SetActive(false);
    }

    // Send chat 
    public void OnInputChat()
    {
        _inputChat = inputChat.text;
        if (_inputChat != null && _inputChat != "")
        {
            client.SendClientChat(client.clientData, _inputChat);
            inputChat.text = "";
        }
    }

    // On hosting server
    public void OnClickHostServer()
    {
        DeactivateAll();
        checkboxIsTcp.gameObject.SetActive(true);
        buttonCreateServer.gameObject.SetActive(true);
        buttonBack.gameObject.SetActive(true);
        client.clientData.isHost = true;
    }

     // On joining server
    public void OnClickJoinServer()
    {
        DeactivateAll();
        inputJoinIP.gameObject.SetActive(true);
        buttonJoinToServer.gameObject.SetActive(true);
        buttonBack.gameObject.SetActive(true);
        client.serverCode= _inputJoinCode;
    }

    public void OnValueChangeIsTcp()
    {
        isTcp = !isTcp;
    }

    // Return to host/join state
    public void OnClickBack()
    {
        _inputJoinIP = inputJoinIP.text = _inputJoinCode = "";
        DeactivateAll();
        buttonJoinServer.gameObject.SetActive(true);
        buttonHostServer.gameObject.SetActive(true);
    }
    
    // Start game
    public void OnClickStartGame()
    {
        if (client.clientData.isHost)
            client.SendStartGame();
    }
    #endregion INPUT EVENTS

    #region HELPERS
    private void DeactivateAll()
    {
        inputJoinIP.gameObject.SetActive(false);
        inputUsername.gameObject.SetActive(false);
        buttonHostServer.gameObject.SetActive(false);
        buttonJoinServer.gameObject.SetActive(false);
        checkboxIsTcp.gameObject.SetActive(false);
        buttonCreateServer.gameObject.SetActive(false);
        buttonJoinToServer.gameObject.SetActive(false);
        buttonBack.gameObject.SetActive(false);
        chat.SetActive(false);
    }
    #endregion HELPERS
}
