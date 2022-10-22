using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class TCPServer : MonoBehaviour
{
    private string serverIP;
    private int serverPort;

    private int recv;
    private byte[] data = new byte[1024];

    private Thread serverThread;
    private Socket tcpSocket;

    // Destination EndPoint and IP
    private IPEndPoint clientIPEP;
    private EndPoint clientEP;

    // Start is called before the first frame update
    void Start()
    {
        // Get IP and port
        serverIP = GameObject.Find("ServerManager").GetComponent<ServerManager>().serverIP;
        serverPort = GameObject.Find("ServerManager").GetComponent<ServerManager>().serverPort;

        StartServer();
    }

    private void OnDisable()
    {
        //Debug.Log("[SERVER] Closing TCP socket & thread...");
        if (tcpSocket != null)
            tcpSocket.Close();
        if (serverThread != null)
            serverThread.Abort();
    }

    public void StartServer(string ip = null, int port = 0)
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
        //Debug.Log("[SERVER] Initializing TCP socket...");
        tcpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    }

    private void InitializeThread()
    {
        //Debug.Log("[SERVER] Initializing TCP thread...");
        serverThread = new Thread(new ThreadStart(ServerThread));
        serverThread.IsBackground = true;
        serverThread.Start();
    }

    private void ServerThread()
    {
        // Client IP EndPoint
        IPAddress ipAddress = IPAddress.Parse(serverIP);
        clientIPEP = new IPEndPoint(ipAddress, serverPort);
        clientEP = (EndPoint)clientIPEP;
        
        try
        {
            tcpSocket.Bind(clientIPEP);
            Debug.Log("[SERVER] TCP socket bound to " + clientIPEP.ToString());
        }
        catch (Exception e)
        {
            Debug.Log("[ERROR SERVER] Failed to bind socket: " + e.Message);
        }
        
        tcpSocket.Listen(10);
        while (true)
        {
            Socket clientSocket = tcpSocket.Accept();
            try
            {
                recv = clientSocket.Receive(data);
                Debug.Log("[SERVER] Message received: " + Encoding.Default.GetString(data, 0, recv));
                //Debug.Log("[SERVER] Message received from " + clientSocket.LocalEndPoint.ToString() + ": " + Encoding.Default.GetString(data, 0, recv));
            }
            catch (Exception e)
            {
                Debug.Log("[ERROR SERVER] Failed to receive data: " + e.Message);
            }

            // Send data to the client
            SendData(clientSocket ,"Heyyy client, I have received your message");

            // Close the connection socket
            clientSocket.Close();
        }
    }
    
    private void SendData(Socket socket, string message)
    {
        try
        {
            Debug.Log("[SERVER] Sending message to client: " + message);
            data = Encoding.Default.GetBytes(message);
            socket.Send(data);
        }
        catch (Exception e)
        {
            Debug.Log("[ERROR SERVER] Failed to send data: " + e.Message);
        }
    }
}
