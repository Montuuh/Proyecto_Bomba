using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using System.Collections.Concurrent;

public class Server : MonoBehaviour
{
    public enum Protocol { TCP, UDP }
    public Protocol protocol;

    public int serverPort;
    public string serverIP;
    
    private int recv;
    private byte[] data = new byte[1024];

    private Thread serverThread;
    private Thread serverAcceptThread;
    private Socket socket;
    private List<Socket> clientSocketList;

    // Destination EndPoint and IP
    private IPEndPoint clientIPEP;
    private EndPoint clientEP;
    private List<EndPoint> clientEPList;

    private IPEndPoint serverIPEP;

    void Start()
    {
        StartServer();
    }

    private void OnDisable()
    {
        //Debug.Log("[SERVER] Closing TCP socket & thread...");
        if (socket != null)
            socket.Close();
        if (serverThread != null)
            serverThread.Abort();
    }

    private void StartServer(string ip = null, int port = 0)
    {
        clientSocketList = new List<Socket>();
        clientEPList = new List<EndPoint>();

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
                Debug.Log("[SERVER] Initializing TCP socket...");
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                break;
            case Protocol.UDP:
                Debug.Log("[SERVER] Initializing UDP socket...");
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                break;
            default:
                break;
        }

        // ServerIP EndPoint
        serverIPEP = new IPEndPoint(IPAddress.Parse(serverIP), serverPort);

        // Client IP EndPoint
        clientIPEP = new IPEndPoint(IPAddress.Any, 0);
        clientEP = (EndPoint)clientIPEP;

        // Creating Socket and binding it to the address
        try
        {
            socket.Bind(serverIPEP);
            Debug.Log("[SERVER] Socket bound to " + serverIPEP.ToString());
        }
        catch (Exception e)
        {
            Debug.Log("[ERROR SERVER] Failed to bind socket: " + e.Message);
        }
    }

    private void InitializeThread()
    {
        Debug.Log("[SERVER] Initializing thread...");
        switch (protocol)
        {
            case Protocol.TCP:
                serverThread = new Thread(ServerThread);
                serverThread.IsBackground = true;
                serverThread.Start();

                //serverAcceptThread = new Thread(ServerThreadAccept);
                //serverAcceptThread.IsBackground = true;
                //serverAcceptThread.Start();
                break;
            case Protocol.UDP:
                serverThread = new Thread(ServerThread);
                serverThread.IsBackground = true;
                serverThread.Start();
                break;
            default:
                break;
        }
    }

    private void ServerThread()
    {
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

    private void ServerThreadAccept()
    {
        socket.Listen(10);
        while (clientSocketList.Count <= 10)
        {
            
            Socket clientSocket = socket.Accept();

            data = new byte[1024];
            recv = clientSocket.Receive(data);

            clientSocketList.Add(clientSocket);
        }
    }

    void UDPThread()
    {
        while(true)
        {
            data = new byte[1024];
            recv = socket.ReceiveFrom(data, ref clientEP);
            
            if (recv > 0)
            {
                if(!clientEPList.Contains(clientEP))
                    clientEPList.Add(clientEP);

                string message = Encoding.Default.GetString(data, 0, recv);

                foreach (EndPoint endPoint in clientEPList)
                {

                    //Debug.Log("[SERVER] Received message from " + endPoint.ToString() + ", named: " + message[0] + ": " + message[1]);
                    SendData(message, socket, endPoint);
                }
            }
        }
    }


    void TCPThread()
    {
        socket.Listen(10);
        Socket clientSocket = socket.Accept();
        while (true)
        {
           
            data = new byte[1024];
            recv = clientSocket.Receive(data);

            if (recv > 0)
            {
                string message = Encoding.ASCII.GetString(data, 0, recv);

                Debug.Log("[SERVER] Message received from " + clientSocket.RemoteEndPoint + " = " + message);
                 
                    SendData(message, clientSocket);
            }
           
        }
    }

    private void SendData(string message, Socket clientSocket = null, EndPoint endPoint = null)
    {
        try
        {
            data = Encoding.Default.GetBytes(message);
            switch (protocol)
            {
                case Protocol.TCP:
                    Debug.Log("[SERVER] Sending message to " + clientSocket.RemoteEndPoint + " = " + message);
                    data = Encoding.ASCII.GetBytes(message);
                    clientSocket.Send(data);
                    break;
                case Protocol.UDP:
                    Debug.Log("[SERVER] Sending message to " + clientEP.ToString() + ": " + message);
                    if (endPoint == null)
                        socket.SendTo(data, data.Length, SocketFlags.None, clientEP);
                    else
                        socket.SendTo(data, data.Length, SocketFlags.None, endPoint);
                    break;
                default:
                    break;
            }
        }
        catch (Exception e)
        {
            Debug.Log("[ERROR SERVER] Failed to send data. Error: " + e.Message);
        }
    }

    private string[] DecodeString(string message)
    {
        string[] messageParts = message.Split('|');
        string username = messageParts[0];
        string rest = messageParts[1];
        return new string[] { username, rest };
    }
}
