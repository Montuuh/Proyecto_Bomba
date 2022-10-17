using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class TCPClient : MonoBehaviour
{
    private int recv;
    private byte[] data = new byte[1024];

    Thread clientThread;

    // Bounding mechanism
    private Socket tcpSocket;

    // IP and Adress
    IPEndPoint serverIPEP;
    EndPoint serverEP;

    int serverPort = 9500;

    void Start()
    {
        // Initialize Thread
        tcpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        clientThread = new Thread(SendData2Server);
        clientThread.IsBackground = true;
        clientThread.Start();
    }

    private void SendData2Server()
    {
        //Server IP EndPoint
        serverIPEP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), serverPort);
        serverEP = (EndPoint)serverIPEP;

        try
        {
            Debug.Log("Sending to server...");

            //Send Data to Server
            data = Encoding.ASCII.GetBytes("Zupare paneo konektao");
            recv = tcpSocket.SendTo(data, data.Length, SocketFlags.None, serverEP);
        }
        catch (Exception e)
        {
            Debug.Log("Server critical error occurred: " + e.ToString());
        }
    }

    private void OnDisable()
    {
        Debug.Log("Disabling...");

        tcpSocket.Close();
        clientThread.Abort();
    }

}
