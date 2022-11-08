using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Text;
using System.IO;
using UnityEngine;
using System.Collections.Generic;

public class ClientData
{
    public string userName;
    public uint userID;
    public List<Cell> lastRevealedCells;        

    public ClientData()
    {
        SetRandomGuest();
    }

    private void SetRandomGuest()
    {
        int random = Random.Range(0, 100000);
        userName = "Guest" + random.ToString();
        userID = (uint)random;
    }
}

public class Client : MonoBehaviour
{
    public enum Protocol { TCP, UDP }
    public Protocol protocol;

    public ClientData clientData;

    private int serverPort;
    private string serverIP;
    
    private Thread clientThread;
    private Socket socket;
    private IPEndPoint serverIPEP;
    private EndPoint serverEP;


    void Start()
    {
        clientData = new ClientData();
    }

    private void Update()
    {
        
    }

    private void OnDisable()
    {
        if (socket != null)
            socket.Close();
        if (clientThread != null)
            clientThread.Abort();
    }
    
    public void ConnectToServer(string ip = null, int port = 0)
    {
        if (ip != null)
            serverIP = ip;
        if (port != 0)
            serverPort = port;

        // Socket initialization
        if (protocol == Protocol.TCP)
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        else
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        // Thread initialization
        clientThread = new Thread(ClientThread);
        clientThread.IsBackground = true;
        clientThread.Start();
    }

    private void ClientThread()
    {
        // Client IP EndPoint
        Debug.Log("[CLIENT] Trying to connect to server --> " + serverIP + ":" + serverPort);
        IPAddress ipAddress = IPAddress.Parse(serverIP);
        serverIPEP = new IPEndPoint(ipAddress, serverPort);
        serverEP = (EndPoint)serverIPEP;

        if (protocol == Protocol.TCP)
        {
            // ToDo: Not yet adapted to serializing
            socket.Connect(serverIPEP);

            if (socket.Connected)
            {
                Debug.Log("[CLIENT] Connected to server --> " + serverIP + ":" + serverPort);
                // Send welcome message
                SendString("Username: " + clientData.userName + " | UID: " + clientData.userID + " | Connected to server");

                while (true)
                {
                    // Receive data from server
                    byte[] data = new byte[1024];
                    int receivedDataLength = 0;
                    receivedDataLength = socket.Receive(data);
                    if (receivedDataLength == 0)
                        break;
                    Debug.Log("[CLIENT] Received: " + Encoding.ASCII.GetString(data, 0, receivedDataLength));
                }
            }
        }
        else
        {
            // First connexion sender
            SendClientData(clientData);

            while (true)
            {
                // RECEIVE DATA
                byte[] data = new byte[1024];
                int recv = socket.ReceiveFrom(data, ref serverEP);
                Sender sender = Serialize.DeserializeSender(data);
                if (sender.senderType == SenderType.CLIENTDATA)
                {
                    Debug.Log("[CLIENT] Received client data sender type from server: " + sender.clientData.userName + " | " + sender.clientData.userID);
                }
                else if (sender.senderType == SenderType.STRING)
                {
                    Debug.Log("[CLIENT] Received string sender type from server: " + sender.message);
                }
                // !RECEIVE DATA
            }
        }
    }

    // ToDo: Not yet adapted to serializing
    public void SendString(string message)
    {
        byte[] data = new byte[1024];
        data = Encoding.ASCII.GetBytes(message);

        if (protocol == Protocol.TCP)
        {
            Debug.Log("[CLIENT] Sending to server: " + socket.RemoteEndPoint.ToString() + " Message: " + message);
            socket.Send(data, data.Length, SocketFlags.None);
        }
        else
        {
            Debug.Log("[CLIENT] Sending to server: " + serverIPEP.ToString() + " Message: " + message);
            socket.SendTo(data, data.Length, SocketFlags.None, serverEP);
        }
    }

    public void SendClientData(ClientData _clientData)
    {
        Sender sender = new Sender(_clientData);
        
        byte[] data = Serialize.SerializeSender(sender);
        
        if (protocol == Protocol.TCP)
        {
            // ToDo: Not yet adapted to serializing
            Debug.Log("[CLIENT] Sending to server: " + socket.RemoteEndPoint.ToString() + " Message: " + data.Length);
            socket.Send(data, data.Length, SocketFlags.None);
        }
        else
        {
            Debug.Log("[CLIENT] Sending to server: " + serverIPEP.ToString() + " || Sender type: " + sender.senderType + " || Sender username and UID: " + sender.clientData.userName + " | " + sender.clientData.userID);
            socket.SendTo(data, data.Length, SocketFlags.None, serverEP);
        }
    }
}
