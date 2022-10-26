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
    private uint userID;
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

    public uint GetUID()
    {
        return userID;
    }

    public void SetUID(uint _userID)
    {
        userID = _userID;
    }

    private void SetRandomGuest()
    {
        int random = UnityEngine.Random.Range(0, 100000);
        userName = "Guest" + random.ToString();
        userID = (uint)random;
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
    
    private Thread clientThread;
    private Socket socket;
    private IPEndPoint serverIPEP;
    private EndPoint serverEP;

    // public GameObject chat;
    // List<Message> pendingMessages = new List<Message>();

    void Start()
    {
        clientData = new ClientData();
    }

    private void Update()
    {
        //if (pendingMessages.Count > 0)
        //{
        //    chat.GetComponent<Chat>().pendingMessages.Add(pendingMessages[0]);
        //    pendingMessages.Remove(pendingMessages[0]);
        //}
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

        // Socket initialization
        if (protocol == Protocol.TCP)
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        else
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        // Thread initialization
        clientThread = new Thread(ClientThread);
        clientThread.IsBackground = true;
        clientThread.Start();
    }

    private void ClientThread()
    {
        // Client IP EndPoint
        Debug.Log("[CLIENT] Trying to connect to server --> " + serverIP + ":" + serverPort);
        IPAddress ipAddress = IPAddress.Parse(serverIP);
        serverIPEP = new IPEndPoint(ipAddress, serverPort);
        serverEP = (EndPoint)serverIPEP;

        if (protocol == Protocol.TCP)
        {
            socket.Connect(serverIPEP);

            if (socket.Connected)
            {
                Debug.Log("[CLIENT] Connected to server --> " + serverIP + ":" + serverPort);
                // Send welcome message
                SendString("Username: " + clientData.GetUserName() + " | UID: " + clientData.GetUID() + " | Connected to server");

                while (true)
                {
                    // Receive data from server
                    byte[] data = new byte[1024];
                    int receivedDataLength = 0;
                    receivedDataLength = socket.Receive(data);
                    if (receivedDataLength == 0)
                        break;
                    Debug.Log("[CLIENT] Received: " + Encoding.ASCII.GetString(data, 0, receivedDataLength));
                }
            }
        }
        else
        {
            // Send username to server
            SendString("Username: " + clientData.GetUserName() + " | UID: " + clientData.GetUID() + " | Connected to server");

            while (true)
            {
                // Receive data from server
                byte[] data = new byte[1024];
                int receivedDataLength = 0;
                receivedDataLength = socket.ReceiveFrom(data, ref serverEP);
                if (receivedDataLength == 0)
                    break;
                Debug.Log("[CLIENT] Received: " + Encoding.ASCII.GetString(data, 0, receivedDataLength));
            }
        }
    }

    public void SendString(string message)
    {
        byte[] data = new byte[1024];
        data = Encoding.ASCII.GetBytes(message);

        if (protocol == Protocol.TCP)
        {
            Debug.Log("[CLIENT] Sending to server: " + socket.RemoteEndPoint.ToString() + " Message: " + message);
            data = Encoding.ASCII.GetBytes(message);
            socket.Send(data, data.Length, SocketFlags.None);
        }
        else
        {
            Debug.Log("[CLIENT] Sending to server: " + serverIPEP.ToString() + " Message: " + message);
            socket.SendTo(data, data.Length, SocketFlags.None, serverEP);
        }
    }
}
