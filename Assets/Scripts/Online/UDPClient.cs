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
    private string serverIP;
    private int serverPort;

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
        IPAddress ipAddress = IPAddress.Parse(serverIP);
        serverIPEP = new IPEndPoint(ipAddress, serverPort);
        serverEP = (EndPoint)serverIPEP;

        // Send data to server
        SendData("This is a message from the client");

        // Receive data from server
        recv = udpSocket.Receive(data);
        Debug.Log("[CLIENT] Received: " + Encoding.Default.GetString(data, 0, recv));
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
}
