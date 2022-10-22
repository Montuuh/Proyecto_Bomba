using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class UDPClient : MonoBehaviour
{
    public enum Protocol { TCP, UDP }
    public Protocol protocol;

    private string serverIP;
    private int serverPort;

    public string userName;
    private bool hasSetUsername = false;

    private int recv;
    private byte[] data = new byte[1024];

    private Thread clientThread;
    private Socket udpSocket;

    // Destination EndPoint and IP
    private IPEndPoint serverIPEP;
    private EndPoint serverEP;

    // Start is called before the first frame update
    void Start()
    {
        // Get IP and port
        serverIP = GameObject.Find("ServerManager").GetComponent<ServerManager>().serverIP;
        serverPort = GameObject.Find("ServerManager").GetComponent<ServerManager>().serverPort;

        SetRandomGuest();
    }
    
    private void OnDisable()
    {
        //Debug.Log("[CLIENT] Closing UDP socket & thread...");
        if (udpSocket != null)
            udpSocket.Close();
        if (clientThread != null)
            clientThread.Abort();
    }

    public void ConnectToServer(string ip = null, int port = 0)
    {
        if (ip != null)
            serverIP = ip;
        if (port != 0)
            serverPort = port;
        
        InitializeUDPSocket();
        InitializeThread();
    }

    private void InitializeUDPSocket()
    {
        //Debug.Log("[CLIENT] Initializing UDP socket...");
        udpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
    }

    private void InitializeThread()
    {
        //Debug.Log("[CLIENT] Initializing UDP thread...");
        clientThread = new Thread(ClientThread);
        clientThread.IsBackground = true;
        clientThread.Start();
    }

    private void ClientThread()
    {
        // Client IP EndPoint
        Debug.Log("[CLIENT] Trying to connect to UDP server --> " + serverIP + ":" + serverPort);
        IPAddress ipAddress = IPAddress.Parse(serverIP);
        serverIPEP = new IPEndPoint(ipAddress, serverPort);
        serverEP = (EndPoint)serverIPEP;

        // Send data to server
        SendData("This is a message from the client: " + userName);

        // Receive data from server
        try
        {
            recv = udpSocket.Receive(data);
            Debug.Log("[CLIENT] Received: " + Encoding.Default.GetString(data, 0, recv));
        }
        catch (Exception e)
        {
            Debug.Log("[CLIENT] Failed to send message. Error: " + e.ToString());
        }
    }

    // SendData to server
    private void SendData(string message)
    {
        try
        {
            Debug.Log("[CLIENT] Sending to server: " + serverIPEP.ToString() + " Message: " + message);
            data = Encoding.Default.GetBytes(message);
            recv = udpSocket.SendTo(data, data.Length, SocketFlags.None, serverEP);
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
