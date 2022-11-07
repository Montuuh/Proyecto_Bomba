using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class Server : MonoBehaviour
{
    public enum Protocol { TCP, UDP }
    public Protocol protocol;

    public int serverPort;
    public string serverIP;
    private IPEndPoint serverIPEP;

    private Socket serverSocket;

    // Thread
    private Thread serverAcceptThread;
    private Thread serverThread;
    private Thread clientThread;

    // Destination EndPoint and IP
    private IPEndPoint clientIPEP;
    private EndPoint clientEP;
    
    private List<EndPoint> clientEPList;
    private List<Socket> clientSocketList;


    void Start()
    {
        StartServer();
    }

    private void OnDisable()
    {
        if (serverSocket != null)
            serverSocket.Close();
        if (serverAcceptThread != null)
            serverAcceptThread.Abort();
        if (serverThread != null)
            serverThread.Abort();
        if (clientThread != null)
            clientThread.Abort();
    }

    private void StartServer(string ip = null, int port = 0)
    {
        if (ip != null)
            serverIP = ip;
        if (port != 0)
            serverPort = port;
        
        clientSocketList = new List<Socket>();
        clientEPList = new List<EndPoint>();

        // Initialize serverSocket
        if (protocol == Protocol.TCP)
        {
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }
        else
        {
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        }

        // Accept any client IP
        clientIPEP = new IPEndPoint(IPAddress.Any, 0);
        clientEP = (EndPoint)clientIPEP;
        
        // Server start
        serverIPEP = new IPEndPoint(IPAddress.Parse(serverIP), serverPort);
        serverSocket.Bind(serverIPEP);
        Debug.Log("[SERVER] Server started on " + serverIP + ":" + serverPort);

        if (protocol == Protocol.TCP)
        {
            serverSocket.Listen(10);
            serverAcceptThread = new Thread(new ThreadStart(ServerAcceptThread));
            serverAcceptThread.IsBackground = true;
            serverAcceptThread.Start();
        }
        else
        {
            serverThread = new Thread(new ThreadStart(UDPThread));
            serverThread.IsBackground = true;
            serverThread.Start();
        }
    }

    private void ServerAcceptThread()
    {
        while (true)
        {
            // Accept new connections
            Socket clientSocket = serverSocket.Accept();
            clientSocketList.Add(clientSocket);
            //clientEPList.Add(clientSocket.RemoteEndPoint);
            Debug.Log("[SERVER] Client connected: " + clientSocket.RemoteEndPoint.ToString());

            clientThread = new Thread(ClientThread);
            clientThread.IsBackground = true;
            clientThread.Start(clientSocket);
        }
    }

    private void ClientThread(object clientSocket)
    {
        Socket client = (Socket)clientSocket;
        while (true)
        {
            // Receive data from client
            byte[] data = new byte[1024];
            int recv = client.Receive(data);
            string stringData = Encoding.ASCII.GetString(data, 0, recv);
            Debug.Log("[SERVER] Received: " + stringData + " from " + client.RemoteEndPoint.ToString());

            // Send reply
            string reply = "OK: " + stringData;
            byte[] replyData = Encoding.ASCII.GetBytes(reply);
            client.Send(replyData);
        }
    }

    private void UDPThread()
    {
        while (true)
        {
            // Receive data from client
            byte[] data = new byte[1024];
            int recv = serverSocket.ReceiveFrom(data, ref clientEP);
            // deserialize data
            ClientData clientData = Serialize.DeserializeClientData(data);

            //string stringData = Encoding.ASCII.GetString(data, 0, recv);
            //Debug.Log("[SERVER] Received: " + stringData + " from " + clientEP.ToString());

            // Send reply
            //string reply = "OK: " + stringData;
            //byte[] replyData = Encoding.ASCII.GetBytes(reply);
            byte[] replyData = Serialize.SerializeClientData(clientData);
            serverSocket.SendTo(replyData, clientEP);

            // Check if endpoint is not in the list
            if (!clientEPList.Contains(clientEP))
            {
                clientEPList.Add(clientEP);
                Debug.Log("[SERVER] Client connected: " + clientEP.ToString());
            }
        }
    }

    // Deserialize clientData
    private int DeserializeClientData(byte[] data)
    {
        int recv = 0;
        int pos = 0;

        // Deserialize data, string username, uint uid
        string username = Encoding.ASCII.GetString(data, pos, 16);
        pos += 16;
        recv += 16;
        uint uid = BitConverter.ToUInt32(data, pos);
        pos += 4;
        recv += 4;

        return recv;
    }
}
