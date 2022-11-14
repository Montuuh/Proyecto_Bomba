using System.Collections;
using TMPro;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows;

public class ClientLobby : MonoBehaviour
{
    public TMP_InputField inputJoinIP;
    private string _inputJoinIP;
    public TMP_InputField inputJoinPort;
    private string _inputJoinPort;
    public Button buttonJoinToServer;
    
    public TMP_InputField inputHostIP;
    private string _inputHostIP;
    public TMP_InputField inputHostPort;
    private string _inputHostPort;
    public Button buttonCreateServer;
    
    public TMP_InputField inputUsername;
    private string _inputUsername;
    
    public Button buttonHostServer;
    public Button buttonJoinServer;
    
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
    
    public void OnInputHostIP()
    {
        _inputHostIP = inputHostIP.text;
    }
    public void OnInputHostPort()
    {
        _inputHostPort = inputHostPort.text;
    }
    public void OnClickCreateServer()
    {
        if ((_inputHostIP == null || _inputHostIP == "") || (_inputHostPort == null || _inputHostPort == ""))
            return;

        // Start server
        GameObject serverGo = new GameObject("ServerManager", typeof(Server), typeof(DontDestroyMe));
        Server server = serverGo.GetComponent<Server>();
        server.StartServer(_inputHostIP, int.Parse(_inputHostPort), isTcp);
        client.ConnectToServer(_inputHostIP, int.Parse(_inputHostPort));

        DeactivateAll();
        chat.SetActive(true);
    }

    public void OnInputJoinIP()
    {
        _inputJoinIP = inputJoinIP.text;
    }
    public void OnInputJoinPort()
    {
        _inputJoinPort = inputJoinPort.text;
    }
    public void OnClickJoinToServer()
    {
        if ((_inputJoinIP == null || _inputJoinIP == "") || (_inputJoinPort == null || _inputJoinPort == ""))
            return;

        client.ConnectToServer(_inputJoinIP, int.Parse(_inputJoinPort));

        DeactivateAll();
        chat.SetActive(true);
    }

    public void OnInputChat()
    {
        _inputChat = inputChat.text;
        if (_inputChat != null && _inputChat != "")
        {
            client.SendClientString(client.clientData, _inputChat);
            inputChat.text = "";
        }
    }


    public void OnClickHostServer()
    {
        DeactivateAll();
        inputHostIP.gameObject.SetActive(true);
        inputHostPort.gameObject.SetActive(true);
        checkboxIsTcp.gameObject.SetActive(true);
        buttonCreateServer.gameObject.SetActive(true);
        buttonBack.gameObject.SetActive(true);
    }
    public void OnClickJoinServer()
    {
        DeactivateAll();
        inputJoinIP.gameObject.SetActive(true);
        inputJoinPort.gameObject.SetActive(true);
        buttonJoinToServer.gameObject.SetActive(true);
        buttonBack.gameObject.SetActive(true);
    }

    public void OnValueChangeIsTcp()
    {
        isTcp = !isTcp;
    }

    public void OnClickBack()
    {
        _inputJoinIP = _inputJoinPort = _inputHostIP = _inputHostPort =  "";
        inputJoinIP.text = inputJoinPort.text = inputHostIP.text = inputHostPort.text = "";
        DeactivateAll();
        buttonJoinServer.gameObject.SetActive(true);
        buttonHostServer.gameObject.SetActive(true);
    }
    #endregion INPUT EVENTS

    #region HELPERS
    private void DeactivateAll()
    {
        inputJoinIP.gameObject.SetActive(false);
        inputJoinPort.gameObject.SetActive(false);
        inputHostIP.gameObject.SetActive(false);
        inputHostPort.gameObject.SetActive(false);
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
