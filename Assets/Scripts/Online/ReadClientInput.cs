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

    private string inputChat;
    public TMP_InputField inputFieldChat;

    private Client client;

    // Start is called before the first frame update
    void Start()
    {
        client = GameObject.Find("ClientManager").GetComponent<Client>();
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

        client.ConnectToServer(inputIP, int.Parse(inputPort));

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

        client.clientData.SetUsername(inputUsername);

        GameObject.Find("ClientLobby").GetComponent<ClientLobby>().OnValidUsername();
    }

    public void OnSubmitInputChat()
    {
        inputChat = inputFieldChat.text;
        
        client.SendData(inputChat);
        inputFieldChat.text = "";
    }
}
