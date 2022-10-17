using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class UDPServer : MonoBehaviour
{
    private string serverIP;
    private int serverPort;
    
    private int recv;
    private byte[] data = new byte[1024];

    private Thread serverThread;
    private Socket udpSocket;
    
    // Destination EndPoint and IP
    private IPEndPoint clientIPEP;
    private EndPoint clientEP;

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
        Debug.Log("[SERVER] Closing UDP socket & thread...");
        udpSocket.Close();
        serverThread.Abort();
    }

    private void InitializeUDPSocket()
    {
        Debug.Log("[SERVER] Initializing UDP socket...");
        udpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
    }

    private void InitializeThread()
    {
        Debug.Log("[SERVER] Initializing UDP thread...");
        serverThread = new Thread(ReceiveData);
        serverThread.IsBackground = true;
        serverThread.Start();
    }

    private void ReceiveData()
    {
        // Client IP EndPoint
        IPAddress ipAddress = IPAddress.Parse(serverIP);
        clientIPEP = new IPEndPoint(IPAddress.Any, serverPort);
        clientEP = (EndPoint)clientIPEP;

        // Creating Socket and binding it to the address
        try
        {
            udpSocket.Bind(clientIPEP);
            Debug.Log("[SERVER] UDP socket bound to " + clientIPEP.ToString());
        }
        catch (Exception e)
        {
            Debug.Log("[ERROR SERVER] Failed to bind socket: " + e.ToString());
        }

        Debug.Log("[SERVER] Waiting for client...");

        // Receive Data From Client
        try
        {
            recv = udpSocket.ReceiveFrom(data, ref clientEP);
            Debug.Log("[SERVER] Message received from " + clientEP.ToString() + ": " + Encoding.ASCII.GetString(data, 0, recv));
        }
        catch (Exception e)
        {
            Debug.Log("[ERROR SERVER] Failed to receive data: " + e.ToString());
        }

        // ToDo: Send data to clients
        // SendString("HOLAAAAAAA CLIENTE ASQUEROSO");
    }

    private void SendString(string message)
    {
        // Send Data to Client
        data = Encoding.ASCII.GetBytes(message);
        udpSocket.SendTo(data, data.Length, SocketFlags.None, clientEP);
    }
}
