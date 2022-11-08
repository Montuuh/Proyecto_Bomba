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

    // Not yet adapted to serialization
    private void ServerAcceptThread()
    {
        while (true)
        {
            // Accept new connections
            Socket clientSocket = serverSocket.Accept();
            clientSocketList.Add(clientSocket);
            Debug.Log("[SERVER] Client connected: " + clientSocket.RemoteEndPoint.ToString());

            clientThread = new Thread(ClientThread);
            clientThread.IsBackground = true;
            clientThread.Start(clientSocket);
        }
    }

    // Not yet adapted to serialization
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
            byte[] data = new byte[1024];
            int recv = serverSocket.ReceiveFrom(data, ref clientEP);
            
            Sender sender = Serialize.DeserializeSender(data);

            if (!clientEPList.Contains(clientEP))
            {
                clientEPList.Add(clientEP);
                Debug.Log("[SERVER] Client connected: " + clientEP.ToString() + " with username: " + sender.clientData.userName + " | " + sender.clientData.userID.ToString());

                // Send welcome message to all clients
                string welcome = sender.clientData.userName + " has joined the server";
                SendStringToAll(welcome, clientEP);
            }
            else
            {
                // ToDo: Redirect received client data type sender to all players
                if (sender.senderType == SenderType.CLIENTDATA)
                {
                    Debug.Log("[SERVER] Received client data sender type from client: " + sender.clientData.userName + " | " + sender.clientData.userID.ToString() + " from " + clientEP.ToString());
                    SendClientDataToAll(sender.clientData, clientEP);
                }
            }
        }
    }

    // Send ClientData struct to all clients except the sender
    public void SendClientDataToAll(ClientData clientData, EndPoint senderEP)
    {
        Debug.Log("[SERVER] Sending sender type client data: " + clientData.userName + " | " + clientData.userID.ToString() + " to all clients except: " + senderEP.ToString());
        Sender sender = new Sender(clientData);
        byte[] data = Serialize.SerializeSender(sender);
        SendToAll(data, senderEP);
    }
    public void SendClientDataToAll(ClientData clientData, Socket senderSocket)
    {
        Debug.Log("[SERVER] Sending sender type client data: " + clientData.userName + " | " + clientData.userID.ToString() + " to all clients except:" + senderSocket.RemoteEndPoint.ToString());
        Sender sender = new Sender(clientData);
        byte[] data = Serialize.SerializeSender(sender);
        SendToAll(data, senderSocket.RemoteEndPoint);
    }

    // Send string to all clients except the sender
    public void SendStringToAll(string message, EndPoint senderEP)
    {
        Debug.Log("[SERVER] Sending sender type string: " + message + " to all clients except: " + senderEP.ToString());
        Sender sender = new Sender(message);
        byte[] data = Serialize.SerializeSender(sender);
        SendToAll(data, senderEP);
    }
    public void SendStringToAll(string message, Socket senderSocket)
    {
        Debug.Log("[SERVER] Sending sender type string: " + message + " to all clients except: " + senderSocket.RemoteEndPoint.ToString());
        Sender sender = new Sender(message);
        byte[] data = Serialize.SerializeSender(sender);
        SendToAll(data, senderSocket.RemoteEndPoint);
    }

    // Send data stream to all clients
    private void SendToAll(byte[] data, EndPoint except = null)
    {
        if (protocol == Protocol.TCP)
        {
            // ToDo: Not yet adapted to serializing
            foreach (Socket clientSocket in clientSocketList)
            {
                if (except != null && clientSocket.RemoteEndPoint == except)
                    continue;

                clientSocket.Send(data);
            }
        }
        else
        {
            foreach (EndPoint clientEP in clientEPList)
            {
                if (except != null && clientEP == except)
                    continue;

                serverSocket.SendTo(data, clientEP);
            }
        }
    }
}
