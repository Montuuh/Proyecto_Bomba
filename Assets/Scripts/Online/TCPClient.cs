using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using UnityEngine;
using System.Text;
using System;

public class TCPClient : MonoBehaviour
{
    public string userName;
    public bool hasSetUsername = false;
    private string serverIP;
    private int serverPort;

    private int recv;
    private byte[] data = new byte[1024];

    private Thread clientThread;
    private Socket tcpSocket;

    // Destination EndPoint and IP
    private IPEndPoint serverIPEP;
    
    // Start is called before the first frame update
    void Start()
    {
        // Get IP and port
        serverIP = GameObject.Find("ServerManager").GetComponent<Server>().serverIP;
        serverPort = GameObject.Find("ServerManager").GetComponent<Server>().serverPort;
        
        SetRandomGuest();
    }

    private void OnDisable()
    {
        //Debug.Log("[CLIENT] Closing TCP socket & thread...");
        if (tcpSocket != null)
            tcpSocket.Close();
        if (clientThread != null)
            clientThread.Abort();
    }

    public void ConnectToServer(string ip = null, int port = 0)
    {
        if (ip != null)
            serverIP = ip;
        if (port != 0)
            serverPort = port;

        InitializeTCPSocket();
        InitializeThread();
    }

    private void InitializeTCPSocket()
    {
        //Debug.Log("[CLIENT] Initializing TCP socket...");
        tcpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    }

    private void InitializeThread()
    {
        //Debug.Log("[CLIENT] Initializing TCP thread...");
        clientThread = new Thread(new ThreadStart(ClientThread));
        clientThread.IsBackground = true;
        clientThread.Start();
    }

    private void ClientThread()
    {
        Debug.Log("[CLIENT] Trying to connect to TCP server --> " + serverIP + ":" + serverPort);
        IPAddress ipAddress = IPAddress.Parse(serverIP);
        serverIPEP = new IPEndPoint(ipAddress, serverPort);
        tcpSocket.Connect(serverIPEP);

        // Debug.Log("[CLIENT] Connected to server!");
        SendString("Hello from client: " + userName);

        // Debug.Log("[CLIENT] Receiving data from server...");
        data = new byte[1024];
        recv = tcpSocket.Receive(data);
        Debug.Log("[CLIENT] Data received from server: " + Encoding.Default.GetString(data, 0, recv));

        // Debug.Log("[CLIENT] Closing TCP socket...");
        tcpSocket.Close();
    }

    private void SendString(string message)
    {
        try
        {
            Debug.Log("[CLIENT] Sending to server: " + serverIPEP.ToString() + " Message: " + message);
            data = Encoding.Default.GetBytes(message);
            tcpSocket.Send(data, data.Length, SocketFlags.None);
        }
        catch (Exception e)
        {
            Debug.Log("[CLIENT] Error sending string: " + e.ToString());
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
