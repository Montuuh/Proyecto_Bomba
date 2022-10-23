using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class Server : MonoBehaviour
{
    public enum Protocol { TCP, UDP }
    public Protocol protocol;

    public int serverPort = 9500;
    public string serverIP = "127.0.0.1";
    
    private int recv;
    private byte[] data = new byte[1024];

    private Thread serverThread;
    private Socket socket;

    // Destination EndPoint and IP
    private IPEndPoint clientIPEP;
    private EndPoint clientEP;

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
    }

    private void InitializeThread()
    {
        Debug.Log("[SERVER] Initializing thread...");
        switch (protocol)
        {
            case Protocol.TCP:
                serverThread = new Thread(new ThreadStart(ServerThread));
                serverThread.IsBackground = true;
                serverThread.Start();
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
        // Client IP EndPoint
        IPAddress ipAddress = IPAddress.Parse(serverIP);
        clientIPEP = new IPEndPoint(ipAddress, serverPort);
        clientEP = (EndPoint)clientIPEP;

        // Creating Socket and binding it to the address
        try
        {
            socket.Bind(clientIPEP);
            Debug.Log("[SERVER] Socket bound to " + clientIPEP.ToString());
        }
        catch (Exception e)
        {
            Debug.Log("[ERROR SERVER] Failed to bind socket: " + e.Message);
        }

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

    void UDPThread()
    {
        // Receive Data From Client
        try
        {
            recv = socket.ReceiveFrom(data, ref clientEP);
            Debug.Log("[SERVER] Message received from " + clientEP.ToString() + ": " + Encoding.Default.GetString(data, 0, recv));
        }
        catch (Exception e)
        {
            Debug.Log("[ERROR SERVER] Failed to receive data: " + e.Message);
        }

        // Send data to client
        SendData("Heyyy client, I have received your message");
    }

    void TCPThread()
    {
        socket.Listen(10);
        while (true)
        {
            Socket clientSocket = socket.Accept();
            try
            {
                recv = clientSocket.Receive(data);
                Debug.Log("[SERVER] Message received: " + Encoding.Default.GetString(data, 0, recv));
            }
            catch (Exception e)
            {
                Debug.Log("[ERROR SERVER] Failed to receive data: " + e.Message);
            }

            // Send data to the client
            SendData("Heyyy client, I have received your message", clientSocket);

            // Close the connection socket
            clientSocket.Close();
        }
    }

    private void SendData(string message, Socket clientSocket = null)
    {
        // Send Data to Client
        try
        {
            Debug.Log("[SERVER] Sending message to " + clientEP.ToString() + ": " + message);
            data = Encoding.Default.GetBytes(message);
            switch (protocol)
            {
                case Protocol.TCP:
                    clientSocket.Send(data);
                    break;
                case Protocol.UDP:
                    socket.SendTo(data, data.Length, SocketFlags.None, clientEP);
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

}
