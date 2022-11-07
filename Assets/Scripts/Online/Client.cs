using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text;
using System;
using System.IO;
using Newtonsoft.Json.Bson;
using Newtonsoft.Json;
using System.Runtime.InteropServices.ComTypes;


// Serialize static class
public static class Serialize
{
    public static byte[] SerializeClientData(ClientData clientData)
    {
        Debug.Log("[CLIENT] Serializing...");

        uint uid = clientData.userID;
        string username = clientData.userName;

        MemoryStream stream = new MemoryStream();
        BinaryWriter writer = new BinaryWriter(stream);
        writer.Write(uid);
        writer.Write(username);

        return stream.ToArray();
    }

    public static ClientData DeserializeClientData(byte[] data)
    {
        Debug.Log("[CLIENT] Deserializing...");

        MemoryStream stream = new MemoryStream(data);
        stream.Write(data, 0, data.Length);

        BinaryReader reader = new BinaryReader(stream);
        stream.Seek(0, SeekOrigin.Begin);
        ClientData clientData = new ClientData();
        clientData.userID = reader.ReadUInt32();
        clientData.userName = reader.ReadString();

        return clientData;
    }
}

public class ClientData
{
    public string userName;
    public uint userID;
    
    public ClientData()
    {
        SetRandomGuest();
    }

    private void SetRandomGuest()
    {
        int random = UnityEngine.Random.Range(0, 100000);
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
            // Send username to server
            // SendString("Username: " + clientData.userName + " | UID: " + clientData.userID + " | Connected to server");

            // Send ClientData to server
            SendClientData(clientData);

            while (true)
            {
                // Receive data from server
                byte[] data = new byte[1024];
                int receivedDataLength = 0;
                // Receive clientData from server
                receivedDataLength = ReceiveClientData(data);
                //receivedDataLength = socket.ReceiveFrom(data, ref serverEP);
                if (receivedDataLength == 0)
                    break;
                Debug.Log("[CLIENT] Received: " + Encoding.ASCII.GetString(data, 0, receivedDataLength));
            }
        }
    }

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
        byte[] data = Serialize.SerializeClientData(_clientData);
        if (protocol == Protocol.TCP)
        {
            Debug.Log("[CLIENT] Sending to server: " + socket.RemoteEndPoint.ToString() + " Message: " + data.Length);
            socket.Send(data, data.Length, SocketFlags.None);
        }
        else
        {
            Debug.Log("[CLIENT] Sending to server: " + serverIPEP.ToString() + " Message: " + data.Length);
            socket.SendTo(data, data.Length, SocketFlags.None, serverEP);
        }
    }

    public int ReceiveClientData(byte[] data)
    {
        //byte[] data = new byte[1024];
        int receivedDataLength = 0;
        if (protocol == Protocol.TCP)
            receivedDataLength = socket.Receive(data);
        else
            receivedDataLength = socket.ReceiveFrom(data, ref serverEP);
        if (receivedDataLength == 0)
            return 0;
        clientData = Serialize.DeserializeClientData(data);
        Debug.Log("[CLIENT] Received: " + clientData.userName + " | " + clientData.userID);

        return receivedDataLength;
    }
}
