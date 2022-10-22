using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using UnityEngine;
using System.Text;
using System;

public class Client : MonoBehaviour
{
    public enum Protocol { TCP, UDP }
    public Protocol protocol;

    private int serverPort;
    private string serverIP;

    private string userName;
    private bool hasSetUsername = false;

    private int recv;
    private byte[] data = new byte[1024];

    private Thread clientThread;
    private Socket socket;

    // Destination EndPoint and IP
    private IPEndPoint serverIPEP;
    private EndPoint serverEP;

 
    void Start()
    {
        // Get IP and port
        serverIP = GameObject.Find("ServerManager").GetComponent<Server>().serverIP;
        serverPort = GameObject.Find("ServerManager").GetComponent<Server>().serverPort;

        SetRandomGuest();
        ConnectToServer();
    }

    private void OnDisable()
    {
        //Debug.Log("[CLIENT] Closing socket & thread...");
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
        serverEP = (EndPoint)serverIPEP;

        // Send data to server
        SendData("This is a message from the client: " + userName);

        // Receive data from server
        try
        {
            recv = socket.Receive(data);
            Debug.Log("[CLIENT] Received: " + Encoding.Default.GetString(data, 0, recv));
        }
        catch (Exception e)
        {
            Debug.Log("[CLIENT] Failed to receive message. Error: " + e.ToString());
        }
    }

    private void TCPThread()
    {
        socket.Connect(serverIPEP);

        // Debug.Log("[CLIENT] Connected to server!");
        SendData("Hello from client: " + userName);

        // Debug.Log("[CLIENT] Receiving data from server...");
        data = new byte[1024];
        recv = socket.Receive(data);
        Debug.Log("[CLIENT] Data received from server: " + Encoding.Default.GetString(data, 0, recv));

        // Debug.Log("[CLIENT] Closing TCP socket...");
        socket.Close();
    }

    // SendData to server
    private void SendData(string message)
    {
        try
        {
            Debug.Log("[CLIENT] Sending to server: " + serverIPEP.ToString() + " Message: " + message);
            data = Encoding.Default.GetBytes(message);

            switch (protocol)
            {
                case Protocol.TCP:
                    socket.Send(data, data.Length, SocketFlags.None);
                    break;
                case Protocol.UDP:
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
    private void SetRandomGuest()
    {
        int random = UnityEngine.Random.Range(0, 10000);
        userName = "Guest" + random.ToString();
    }
    public string GetUsername()
    {
        return userName;
    }

    public void SetUsername(string name)
    {
        userName = name;
        hasSetUsername = true;
    }
}
