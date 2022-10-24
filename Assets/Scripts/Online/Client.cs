using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text;
using System;
using System.IO;
using Newtonsoft.Json.Bson;

public class ClientData
{
    private BinaryWriter writer;
    private string userName;
    private bool hasSetUsername = false;

    public ClientData()
    {
        
        SetRandomGuest();
    }

    public string GetUserName()
    {
        return userName;
    }
    
    public void SetUsername(string _userName)
    {
        if (!hasSetUsername)
        {
            hasSetUsername = true;
            userName = _userName;
        }
    }

    private void SetRandomGuest()
    {
        int random = UnityEngine.Random.Range(0, 10000);
        userName = "Guest" + random.ToString();
    }
    
    public void Serialize()
    {
        Debug.Log("[CLIENT] Serializing...");
    }

    public void Deserialize()
    {
        Debug.Log("[CLIENT] Deserializing...");
    }
}

public class Client : MonoBehaviour
{
    public enum Protocol { TCP, UDP }
    public Protocol protocol;

    public ClientData clientData;

    private int serverPort;
    private string serverIP;

    private int recv;
    private byte[] data = new byte[1024];

    private Thread clientThread;
    private Socket socket;

    // Destination EndPoint and IP
    private IPEndPoint serverIPEP;
    private EndPoint serverEP;

    public GameObject chat;
    List<Message> pendingMessages = new List<Message>();

    void Start()
    {
        // Creating the client data
        clientData = new ClientData();
    }

    private void Update()
    {
        if (pendingMessages.Count > 0)
        {
            chat.GetComponent<Chat>().pendingMessages.Add(pendingMessages[0]);
            pendingMessages.Remove(pendingMessages[0]);
        }
    }

    private void OnDisable()
    {
        if (socket != null)
            socket.Close();
        if (clientThread != null)
            clientThread.Abort();
    }
    public void ConnectToServer(string ip = null, int port = 0)
    {
        if (ip != null)
            serverIP = ip;
        if (port != 0)
            serverPort = port;

        InitializeSocket();
        InitializeThread();
    }


    private void InitializeSocket()
    {
        switch (protocol)
        {
            case Protocol.TCP:
                Debug.Log("[CLIENT] Initializing TCP socket...");
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                break;
            case Protocol.UDP:
                Debug.Log("[CLIENT] Initializing UDP socket...");
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                break;
            default:
                break;
        }
    }
    private void InitializeThread()
    {
        switch (protocol)
        {
            case Protocol.TCP:
                Debug.Log("[CLIENT] Initializing TCP thread...");
                clientThread = new Thread(new ThreadStart(ClientThread));
                clientThread.IsBackground = true;
                clientThread.Start();
                break;
            case Protocol.UDP:
                Debug.Log("[CLIENT] Initializing UDP thread...");
                clientThread = new Thread(ClientThread);
                clientThread.IsBackground = true;
                clientThread.Start();
                break;
            default:
                break;
        }
    }

    private void ClientThread()
    {
        // Client IP EndPoint
        Debug.Log("[CLIENT] Trying to connect to server --> " + serverIP + ":" + serverPort);
        IPAddress ipAddress = IPAddress.Parse(serverIP);
        serverIPEP = new IPEndPoint(ipAddress, serverPort);

        switch (protocol)
        {
            case Protocol.TCP:
                TCPThread();
                break;
            case Protocol.UDP:
                UDPThread();
                break;
            default:
                break;
        }
    }

    private void UDPThread()
    {
        SendData(" Hello! I have joined the server");

        ReceiveData();
    }

    private void TCPThread()
    {
        socket.Connect(serverIPEP);

        SendData(" Hello! I have joined the server");

        ReceiveData();
    }

    // SendData to server
    public void SendData(string message)
    {

        string separator = ": ";
        try
        {
            message = clientData.GetUserName() + separator + message;
            Debug.Log("[CLIENT] Sending to server: " + serverIPEP.ToString() + " Message: " + message);
            data = Encoding.Default.GetBytes(message);

            switch (protocol)
            {
                case Protocol.TCP:
                    data = Encoding.ASCII.GetBytes(message);
                    recv = socket.Send(data, data.Length, SocketFlags.None);
                    break;
                case Protocol.UDP:
                    serverEP = (EndPoint)serverIPEP;
                    recv = socket.SendTo(data, data.Length, SocketFlags.None, serverEP);
                    break;
                default:
                    break;
            }
        }
        catch (Exception e)
        {
            Debug.Log("[CLIENT] Failed to send message. Error: " + e.ToString());
        }
    }

    private void ReceiveData()
    {
        while(true)
        {
            try
            {
                data = new byte[1024];
                recv = socket.Receive(data);

                
                Message messa = new Message();
                switch(protocol)
                {
                    case Protocol.TCP:
                        messa.text = Encoding.ASCII.GetString(data, 0, recv);
                        break;
                    case Protocol.UDP:
                        messa.text = Encoding.Default.GetString(data, 0, recv);
                        break;
                }
                pendingMessages.Add(messa);
                

                Debug.Log("[CLIENT] Received: " + Encoding.Default.GetString(data, 0, recv));
            }
            catch (Exception e)
            {
                Debug.Log("[CLIENT] Failed to receive message. Error: " + e.ToString());
            }
        }
    }
}
