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

        InitializeUDPSocket();
        InitializeThread();
    }

    private void OnDisable()
    {
        Debug.Log("[CLIENT] Closing UDP socket & thread...");
        udpSocket.Close();
        clientThread.Abort();
    }

    private void InitializeUDPSocket()
    {
        Debug.Log("[CLIENT] Initializing UDP socket...");
        udpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
    }

    private void InitializeThread()
    {
        Debug.Log("[CLIENT] Initializing UDP thread...");
        clientThread = new Thread(ServerConnection);
        clientThread.IsBackground = true;
        clientThread.Start();
    }

    private void ServerConnection()
    {
        // Client IP EndPoint
        IPAddress ipAddress = IPAddress.Parse(serverIP);
        serverIPEP = new IPEndPoint(ipAddress, serverPort);
        serverEP = (EndPoint)serverIPEP;

        SendData("Hello this is a message from the client to the server, maracatone");
    }

    // SendData to server
    private void SendData(string message)
    {
        try
        {
            Debug.Log("[CLIENT] Sending: " + message + " to server: " + serverIPEP.ToString());
            data = Encoding.ASCII.GetBytes(message);
            recv = udpSocket.SendTo(data, data.Length, SocketFlags.None, serverEP);
        }
        catch (Exception e)
        {
            Debug.Log("[CLIENT] Failed to send message. Error: " + e.ToString());
        }
    }
}
