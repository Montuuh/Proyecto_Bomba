using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Windows;

public class ReadClientInput : MonoBehaviour
{
    private string inputIP;
    public TMP_InputField inputFieldIP;

    private string inputPort;
    public TMP_InputField inputFieldPort;

    private string inputUsername;
    public TMP_InputField inputFieldUsername;

    private ServerManager serverManager;
    private TCPClient tcpClient;
    private UDPClient udpClient;

    // Start is called before the first frame update
    void Start()
    {
        serverManager = GameObject.Find("ClientManager").GetComponent<ServerManager>();
        switch (serverManager.protocol)
        {
            case ServerManager.Protocol.TCP:
                tcpClient = GameObject.Find("ClientManager").GetComponent<TCPClient>();
                break;
            case ServerManager.Protocol.UDP:
                udpClient = GameObject.Find("ClientManager").GetComponent<UDPClient>();
                break;
        }
    }

    public void OnSubmitInputIP()
    {
        inputIP = inputFieldIP.text;
        Debug.Log("Input IP: " + inputIP);

        IsValidDirection();
    }

    public void OnSubmitInputPort()
    {
        inputPort = inputFieldPort.text;
        Debug.Log("Input Port: " + inputPort);

        IsValidDirection();
    }

    private bool IsValidDirection()
    {
        if ((inputIP != null || inputIP != "") && (inputPort == null || inputPort == ""))
            return false;
        if ((inputIP == null || inputIP == "") && (inputPort != null || inputPort != ""))
            return false;

        if (serverManager.protocol == ServerManager.Protocol.TCP) 
            tcpClient.ConnectToServer(inputIP, int.Parse(inputPort));
        else if (serverManager.protocol == ServerManager.Protocol.UDP)
            udpClient.ConnectToServer(inputIP, int.Parse(inputPort));

        GameObject.Find("ClientLobby").GetComponent<ClientLobby>().OnValidServer();

        return true;
    }

    public void OnSubmitInputUsername()
    {
        inputUsername = inputFieldUsername.text;
        Debug.Log("Input Username: " + inputUsername);

        IsValidUsername();
    }

    private void IsValidUsername()
    {
        if (inputUsername == null || inputUsername == "")
            return;

        switch (serverManager.protocol)
        {
            case ServerManager.Protocol.TCP:
                tcpClient.SetUsername(inputUsername);
                break;
            case ServerManager.Protocol.UDP:
                udpClient.SetUsername(inputUsername);
                break;
        }

        GameObject.Find("ClientLobby").GetComponent<ClientLobby>().OnValidUsername();
    }
}
