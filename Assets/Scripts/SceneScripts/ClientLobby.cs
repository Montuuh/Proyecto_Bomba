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
    public Button buttonJoinToServer;
    
    public Button buttonCreateServer;
    
    public TMP_InputField inputUsername;
    private string _inputUsername;
    
    public Button buttonHostServer;
    public Button buttonJoinServer;

    public Button buttonStartGame;

    public TMP_InputField inputChat;
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
    
    public void OnClickCreateServer()
    {
        // Start server
        GameObject serverGo = new GameObject("ServerManager", typeof(Server), typeof(DontDestroyMe));
        Server server = serverGo.GetComponent<Server>();
        server.StartServer(isTcp);

        string A = IPAddressHelper.GetLocalIPAddress();
        string b = IPAddressHelper.EncodeIPAddress(A);
        string c = IPAddressHelper.DecodeIPAddress(b);
        client.ConnectToServer(c, isTcp);

        DeactivateAll();
        chat.SetActive(true);
    }

    public void OnInputJoinIP()
    {
        _inputJoinIP = inputJoinIP.text;
    }
    public void OnClickJoinToServer()
    {
        if ((_inputJoinIP == null || _inputJoinIP == ""))
            return;

        client.ConnectToServer(_inputJoinIP);

        DeactivateAll();
        chat.SetActive(true);
        buttonStartGame.gameObject.SetActive(false);
    }

    public void OnInputChat()
    {
        _inputChat = inputChat.text;
        if (_inputChat != null && _inputChat != "")
        {
            client.SendClientChat(client.clientData, _inputChat);
            inputChat.text = "";
        }
    }


    public void OnClickHostServer()
    {
        DeactivateAll();
        checkboxIsTcp.gameObject.SetActive(true);
        buttonCreateServer.gameObject.SetActive(true);
        buttonBack.gameObject.SetActive(true);
        client.clientData.isHost = true;
    }
    public void OnClickJoinServer()
    {
        DeactivateAll();
        inputJoinIP.gameObject.SetActive(true);
        buttonJoinToServer.gameObject.SetActive(true);
        buttonBack.gameObject.SetActive(true);
    }

    public void OnValueChangeIsTcp()
    {
        isTcp = !isTcp;
    }

    public void OnClickBack()
    {
        _inputJoinIP = inputJoinIP.text = "";
        DeactivateAll();
        buttonJoinServer.gameObject.SetActive(true);
        buttonHostServer.gameObject.SetActive(true);
    }
    
    public void OnClickStartGame()
    {
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
