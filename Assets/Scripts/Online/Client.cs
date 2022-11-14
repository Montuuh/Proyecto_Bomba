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
    public ClientData(uint userID, string userName)
    {
        this.userName = userName;
        this.userID = userID;
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
    
    public Chat chat;


    void Start()
    {
        clientData = new ClientData();
    }

    private void Update()
    {
        // if pressed space, send a clientcell sender to server
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    SendClientCell(clientData, 10, 10);
        //}
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            SceneManager.LoadScene("MultiplayerGame");
        }
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
                SendClientString(clientData, "Username: " + clientData.userName + " | UID: " + clientData.userID + " | Connected to server");

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
                DecodeSender(sender);
                // !RECEIVE DATA
            }
        }
    }

    public void SendClientString(ClientData _clientData, string message)
    {
        Sender sender = new Sender(_clientData, message);

        byte[] data = Serialize.SerializeSender(sender);

        if (protocol == Protocol.TCP)
        {
            // ToDo: Not yet adapted to serializing
            Debug.Log("[CLIENT] Sending to server: " + socket.RemoteEndPoint.ToString() + " Message: " + message);
            socket.Send(data, data.Length, SocketFlags.None);
        }
        else
        {
            Debug.Log("[CLIENT] Sending to server: " + serverIPEP.ToString() + " Message: " + sender.clientString);
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
            Debug.Log("[CLIENT] Sending CLIENTDATA to server: " + socket.RemoteEndPoint.ToString() + " Message: " + data.Length);
            socket.Send(data, data.Length, SocketFlags.None);
        }
        else
        {
            Debug.Log("[CLIENT] Sending CLIENTDATA to server: " + serverIPEP.ToString() + " || Sender type: " + sender.senderType + " || Sender username and UID: " + sender.clientData.userName + " | " + sender.clientData.userID);
            socket.SendTo(data, data.Length, SocketFlags.None, serverEP);
        }
    }

    public void SendClientCell(ClientData _clientData, int _cellPosX, int _cellPosY)
    {
        Sender sender = new Sender(_clientData, _cellPosX, _cellPosY);

        byte[] data = Serialize.SerializeSender(sender);

        if (protocol == Protocol.TCP)
        {
            // ToDo: Not yet adapted to serializing
            Debug.Log("[CLIENT] Sending CLIENTCELL to server: " + socket.RemoteEndPoint.ToString() + " || Sender type: " + sender.senderType + " || Sender username and UID: " + sender.clientData.userName + " | " + sender.clientData.userID + " || Cell revealed: " + sender.cellPosX + " | " + sender.cellPosY);
            socket.Send(data, data.Length, SocketFlags.None);
        }
        else
        {
            Debug.Log("[CLIENT] Sending CLIENTCELL to server: " + serverIPEP.ToString() + " || Sender type: " + sender.senderType + " || Sender username and UID: " + sender.clientData.userName + " | " + sender.clientData.userID + " || Cell revealed: " + sender.cellPosX + " | " + sender.cellPosY);
            socket.SendTo(data, data.Length, SocketFlags.None, serverEP);
        }
    }

    private void DecodeSender(Sender sender)
    {
        if (sender.senderType == SenderType.STRING)
        {
            Debug.Log("[CLIENT] Received STRING sender type from server: " + sender.message);
            if (chat != null) chat.SetPendingMessage(sender.clientData, sender.message);
        }
        else if (sender.senderType == SenderType.CLIENTDATA)
        {
            Debug.Log("[CLIENT] Received CLIENTDATA sender type from server: " + sender.clientData.userName + " | " + sender.clientData.userID);
        }
        else if (sender.senderType == SenderType.CLIENTSTRING)
        {
            Debug.Log("[CLIENT] Received CLIENTSTRING sender type from server: " + sender.clientData.userName + " | " + sender.clientData.userID + " || " + sender.clientString);
            if (chat != null) chat.SetPendingMessage(sender.clientData, sender.clientString);
        }
        else if (sender.senderType == SenderType.CLIENTCELL)
        {
            Debug.Log("[CLIENT] Received CLIENTCELL sender type from server: " + sender.clientData.userName + " | " + sender.clientData.userID + " || " + sender.cellPosX + " | " + sender.cellPosY);
        }
    }
}
