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
    private int recv;
    private byte[] data = new byte[1024];

    Thread serverThread;
    Thread clientThread;

    // Bounding mechanism
    private Socket udpSocket;

    // IP and Adress
    IPEndPoint clientIPEP;
    EndPoint clientEP;

    int serverPort = 9500;

    // Start is called before the first frame update
    void Start()
    {
        // Initialize Thread
        udpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        serverThread = new Thread(ReceiveData);
        serverThread.IsBackground = true;
        serverThread.Start();
    }
    private void ReceiveData()
    {
        //Client IP EndPoint
        clientIPEP = new IPEndPoint(IPAddress.Any, serverPort);
        clientEP = (EndPoint)clientIPEP;

        try
        {
            Debug.Log("Waiting for client...");

            //Creating Socket and binding it to the address
            udpSocket.Bind(clientIPEP);

            //Recieve Data From Client and send Answer
            recv = udpSocket.ReceiveFrom(data, ref clientEP);
            Debug.Log("Data received from client: " + Encoding.ASCII.GetString(data, 0, recv));
            udpSocket.SendTo(data, data.Length, SocketFlags.None, clientEP);

        } catch (Exception e)
        {
            Debug.Log("Error binding socket: " + e.ToString());
        }
    }

    private void OnDisable()
    {
        Debug.Log("Disabling...");

        udpSocket.Close();
        serverThread.Abort();
    }
}
