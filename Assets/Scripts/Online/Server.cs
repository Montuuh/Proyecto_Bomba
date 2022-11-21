using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public enum Protocol { TCP, UDP }

public class Server : MonoBehaviour
{
    public Protocol protocol;

    public string serverIP;
    private IPEndPoint serverIPEP;

    public Socket serverSocket;

    // Thread
    private Thread serverAcceptThread;
    private Thread serverThread;
    private Thread clientThread;

    // Destination EndPoint and IP
    private IPEndPoint clientIPEP;
    private EndPoint clientEP;

    private List<EndPoint> clientEPList = new List<EndPoint>();
    private Dictionary<EndPoint, ClientData> clientsUDP = new Dictionary<EndPoint, ClientData>();
    private List<Socket> clientSocketList = new List<Socket>();
    private Dictionary<Socket, ClientData> clientsTCP = new Dictionary<Socket, ClientData>();


    private List<ColorPlayer> colorPlayerList = new List<ColorPlayer>();
    private List<ColorPlayer> colorPlayerAvailable = new List<ColorPlayer>();
    private List<ClientData> pendingNewPlayer = new List<ClientData>();
    private bool toSendBoard = false;

    void Start()
    {
        // Add all colors to the pool
        colorPlayerAvailable.Add(ColorPlayer.RED);
        colorPlayerAvailable.Add(ColorPlayer.BLUE);
        colorPlayerAvailable.Add(ColorPlayer.GREEN);
        colorPlayerAvailable.Add(ColorPlayer.YELLOW);
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

    private void Update()
    {
        if (pendingNewPlayer.Count > 0)
        {
            ColorPlayer colorPlayer = GetRandomColorPlayer();
            Sender sender = new Sender(SenderType.CLIENTCONNECT) { clientData = pendingNewPlayer[0] };
            sender.clientData.colorPlayer = colorPlayer;
            SendClientConnectedToAll(sender);

            pendingNewPlayer.RemoveAt(0);
        }

        if (toSendBoard)
        {
            Sender sender = new Sender(SenderType.SENDBOARD) { cells = GenerateFullMap() };
            SendBoardToAll(sender);
            toSendBoard = false;
        }
    }

    public void StartServer(bool isTcp = false)
    {
        if (isTcp)
            protocol = Protocol.TCP;
        else
            protocol = Protocol.UDP;

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
        serverIPEP = new IPEndPoint(IPAddress.Any, 9500);
        serverSocket.Bind(serverIPEP);
        Debug.Log("[SERVER] Server started on " + serverIPEP.ToString());

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

    // ToDo: Not yet adapted to serialization
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

    // ToDo: Not yet adapted to serialization
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

            if (recv == 0)
                continue;

            Sender sender = Serialize.DeserializeSender(data);

            if (!clientEPList.Contains(clientEP))
            {
                clientEPList.Add(clientEP);
                clientsUDP.Add(clientEP, sender.clientData);
                Debug.Log("[SERVER] Client connected: " + clientEP.ToString() + " with username: " + sender.clientData.userName + " | " + sender.clientData.userID.ToString());

                pendingNewPlayer.Add(sender.clientData);
            }
            else
            {
                DecodeSender(sender);              
            }
        }
    }

    #region SENDERS
    // Send client data to all clients except the endpoints passed
    private void SendClientDataToAll(Sender sender, EndPoint senderEP = null)
    {
        if (senderEP != null)
            Debug.Log("[SERVER] Sending sender type client data: " + sender.clientData.userName + " | " + sender.clientData.userID.ToString() + " to all clients except: " + senderEP.ToString());
        else
            Debug.Log("[SERVER] Sending sender type client data: " + sender.clientData.userName + " | " + sender.clientData.userID.ToString() + " to all clients");
        byte[] data = Serialize.SerializeSender(sender);
        SendToAll(data, senderEP);
    }
    private void SendClientDataToAll(Sender sender, Socket senderSocket)
    {
        if (senderSocket != null)
            Debug.Log("[SERVER] Sending sender type client data: " + sender.clientData.userName + " | " + sender.clientData.userID.ToString() + " to all clients except: " + senderSocket.RemoteEndPoint.ToString());
        else
            Debug.Log("[SERVER] Sending sender type client data: " + sender.clientData.userName + " | " + sender.clientData.userID.ToString() + " to all clients");
        byte[] data = Serialize.SerializeSender(sender);
        SendToAll(data, senderSocket.RemoteEndPoint);
    }

    // Send string to all clients except the endpoints passed
    private void SendStringToAll(Sender sender, EndPoint senderEP = null)
    {
        if (senderEP != null)
            Debug.Log("[SERVER] Sending sender type string: " + sender.message + " to all clients except: " + senderEP.ToString());
        else
            Debug.Log("[SERVER] Sending string: " + sender.message + " to all clients");
        byte[] data = Serialize.SerializeSender(sender);
        SendToAll(data, senderEP);
    }
    private void SendStringToAll(Sender sender, Socket senderSocket)
    {
        if (senderSocket != null)
            Debug.Log("[SERVER] Sending sender type string: " + sender.message + " to all clients except: " + senderSocket.RemoteEndPoint.ToString());
        else
            Debug.Log("[SERVER] Sending string: " + sender.message + " to all clients");
        byte[] data = Serialize.SerializeSender(sender);
        SendToAll(data, senderSocket.RemoteEndPoint);
    }

    // Send client data string to all clients except the endpoints passed
    private void SendClientChatToAll(Sender sender, EndPoint senderEP = null)
    {
        if (senderEP != null)
            Debug.Log("[SERVER] Sending sender type client string: " + sender.clientChat + " to all clients except: " + senderEP.ToString());
        else
            Debug.Log("[SERVER] Sending sender type client string: " + sender.clientChat + " to all clients");
        byte[] data = Serialize.SerializeSender(sender);
        SendToAll(data, senderEP);
    }
    private void SendClientChatToAll(Sender sender, Socket senderSocket)
    {
        if (senderSocket != null)
            Debug.Log("[SERVER] Sending sender type client string: " + sender.clientChat + " to all clients except: " + senderSocket.RemoteEndPoint.ToString());
        else
            Debug.Log("[SERVER] Sending sender type client string: " + sender.clientChat + " to all clients");
        byte[] data = Serialize.SerializeSender(sender);
        SendToAll(data, senderSocket.RemoteEndPoint);
    }

    // Send client data and cell position to all clients except the endpoints passed
    private void SendClientCellToAll(Sender sender, EndPoint senderEP = null)
    {
        if (senderEP != null)
            Debug.Log("[SERVER] Sending sender type client cell: " + sender.cellPosX.ToString() + " " + sender.cellPosY.ToString() + " to all clients except: " + senderEP.ToString());
        else
            Debug.Log("[SERVER] Sending sender type client cell: " + sender.cellPosX.ToString() + " " + sender.cellPosY.ToString() + " to all clients");
        byte[] data = Serialize.SerializeSender(sender);
        SendToAll(data, senderEP);
    }
    private void SendClientCellToAll(Sender sender, Socket senderSocket)
    {
        if (senderSocket != null)
            Debug.Log("[SERVER] Sending sender type client cell: " + sender.cellPosX.ToString() + " " + sender.cellPosY.ToString() + " to all clients except: " + senderSocket.RemoteEndPoint.ToString());
        else
            Debug.Log("[SERVER] Sending sender type client cell: " + sender.cellPosX.ToString() + " " + sender.cellPosY.ToString() + " to all clients");
        byte[] data = Serialize.SerializeSender(sender);
        SendToAll(data, senderSocket.RemoteEndPoint);
    }

    // Send start game to all clients except the endpoints passed
    private void SendStartGameToAll(Sender sender, EndPoint senderEP = null)
    {
        if (senderEP != null)
            Debug.Log("[SERVER] Sending sender type start game to all clients except: " + senderEP.ToString());
        else
            Debug.Log("[SERVER] Sending sender type start game to all clients");
        byte[] data = Serialize.SerializeSender(sender);
        SendToAll(data, senderEP);
    }
    private void SendStartGameToAll(Sender sender, Socket senderSocket)
    {
        if (senderSocket != null)
            Debug.Log("[SERVER] Sending sender type start game to all clients except: " + senderSocket.RemoteEndPoint.ToString());
        else
            Debug.Log("[SERVER] Sending sender type start game to all clients");
        byte[] data = Serialize.SerializeSender(sender);
        SendToAll(data, senderSocket.RemoteEndPoint);
    }

    // Send client disconnected to all clients except the endpoints passed
    private void SendClientDisconnectedToAll(Sender sender, EndPoint senderEP = null)
    {
        if (senderEP != null)
            Debug.Log("[SERVER] Sending sender type client disconnected to all clients except: " + senderEP.ToString());
        else
            Debug.Log("[SERVER] Sending sender type client disconnected to all clients");
        byte[] data = Serialize.SerializeSender(sender);
        SendToAll(data, senderEP);
    }
    private void SendClientDisconnectedToAll(Sender sender, Socket senderSocket)
    {
        if (senderSocket != null)
            Debug.Log("[SERVER] Sending sender type client disconnected to all clients except: " + senderSocket.RemoteEndPoint.ToString());
        else
            Debug.Log("[SERVER] Sending sender type client disconnected to all clients");
        byte[] data = Serialize.SerializeSender(sender);
        SendToAll(data, senderSocket.RemoteEndPoint);
    }

    // Send client connected to all client except the endpoints passed
    private void SendClientConnectedToAll(Sender sender, EndPoint senderEP = null)
    {
        if (senderEP != null)
            Debug.Log("[SERVER] Sending sender type client connected to all clients except: " + senderEP.ToString());
        else
            Debug.Log("[SERVER] Sending sender type client connected to all clients");
        byte[] data = Serialize.SerializeSender(sender);
        SendToAll(data, senderEP);
    }
    private void SendClientConnectedToAll(Sender sender, Socket senderSocket)
    {
        if (senderSocket != null)
            Debug.Log("[SERVER] Sending sender type client connected to all clients except: " + senderSocket.RemoteEndPoint.ToString());
        else
            Debug.Log("[SERVER] Sending sender type client connected to all clients");
        byte[] data = Serialize.SerializeSender(sender);
        SendToAll(data, senderSocket.RemoteEndPoint);
    }

    // Senc color player to all clients except the endpoints passed
    private void SendColorPlayerToAll(Sender sender, EndPoint senderEP = null)
    {
        if (senderEP != null)
            Debug.Log("[SERVER] Sending sender type color player to all clients except: " + senderEP.ToString());
        else
            Debug.Log("[SERVER] Sending sender type color player to all clients");
        byte[] data = Serialize.SerializeSender(sender);
        SendToAll(data, senderEP);
    }
    private void SendColorPlayerToAll(Sender sender, Socket senderSocket)
    {
        if (senderSocket != null)
            Debug.Log("[SERVER] Sending sender type color player to all clients except: " + senderSocket.RemoteEndPoint.ToString());
        else
            Debug.Log("[SERVER] Sending sender type color player to all clients");
        byte[] data = Serialize.SerializeSender(sender);
        SendToAll(data, senderSocket.RemoteEndPoint);
    }

    // Send board to all clients except the endpoints passed
    private void SendBoardToAll(Sender sender, EndPoint senderEP = null)
    {
        if (senderEP != null)
            Debug.Log("[SERVER] Sending sender type board to all clients except: " + senderEP.ToString());
        else
            Debug.Log("[SERVER] Sending sender type board to all clients");
        byte[] data = Serialize.SerializeSender(sender);
        SendToAll(data, senderEP);
    }
    private void SendBoardToAll(Sender sender, Socket senderSocket)
    {
        if (senderSocket != null)
            Debug.Log("[SERVER] Sending sender type board to all clients except: " + senderSocket.RemoteEndPoint.ToString());
        else
            Debug.Log("[SERVER] Sending sender type board to all clients");
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
                if (except != null && clientSocket.RemoteEndPoint.ToString() == except.ToString())
                    continue;

                clientSocket.Send(data);
            }
        }
        else
        {
            foreach (EndPoint clientEP in clientEPList)
            {
                if (except != null && clientEP.ToString() == except.ToString())
                    continue;

                serverSocket.SendTo(data, clientEP);
            }
        }
    }
    #endregion SENDERS

    #region DECODERS
    // Decode sender data
    private void DecodeSender(Sender sender)
    {
        switch (sender.senderType)
        {
            case SenderType.STRING: // Used to pass a string to all clients, being sent by [SERVER]
                Debug.Log("[SERVER] Received Event = STRING: " + sender.message);
                break;
            case SenderType.CLIENTDATA: // Used to pass client data to all clients (username, uid...)
                Debug.Log("[SERVER] Received Event = CLIENTDATA from client: " + sender.clientData.userName + " | " + sender.clientData.userID.ToString() + " from " + clientEP.ToString());
                SendClientDataToAll(sender);
                break;
            case SenderType.CLIENTCHAT: // Used for chat only, for now
                Debug.Log("[SERVER] Received Event = CLIENTCHAT: " + sender.clientChat + " || from client: " + sender.clientData.userName + " | " + sender.clientData.userID.ToString() + " from " + clientEP.ToString());
                SendClientChatToAll(sender);
                break;
            case SenderType.CLIENTCELL: // Used to pass client data and pressed cell info
                Debug.Log("[SERVER] Received Event = CLIENTCELL position: " + sender.cellPosX.ToString() + " " + sender.cellPosY.ToString() + " || from client: " + sender.clientData.userName + " | " + sender.clientData.userID.ToString() + " from " + clientEP.ToString());
                SendClientCellToAll(sender);
                break;
            case SenderType.STARTGAME: // Used when the host starts the game
                Debug.Log("[SERVER] Received Event = STARTGAME game sender type");
                SendStartGameToAll(sender);
                toSendBoard = true;
                break;
            case SenderType.CLIENTDISCONNECT: // Used when a client disconnects
                Debug.Log("[SERVER] Received Event = CLIENTDISCONNECT message from client: " + sender.clientData.userName + " | " + sender.clientData.userID.ToString() + " from " + clientEP.ToString());
                SendClientDisconnectedToAll(sender);
                DisconnectClient(sender.clientData);
                break;
            case SenderType.CLIENTCONNECT:
                Debug.Log("[SERVER] I don't know why, but I received Event = CLIENTCONNECT client: " + sender.clientData.userName + " | " + sender.clientData.userID.ToString() + " from " + clientEP.ToString());
                break;
            case SenderType.SENDBOARD:
                Debug.Log("[SERVER] I don't know why, but I received Event = SENDBOARD from " + clientEP.ToString());
                break;
            default:
                Debug.Log("[SERVER] Trying to decode UNKNOWN sender type...");
                break;
        }
    }
    #endregion DECODERS

    #region HELPERS
    private void DisconnectClient(ClientData clientData)
    {
        Debug.Log("[SERVER] Disconnecting client... " + clientData.userName + " | " + clientData.userID.ToString());
        if (protocol == Protocol.TCP)
        {
            // ToDo: TCP NOT YET TESTED, IM NOT SURE IF THIS WILL WORK
            foreach (Socket clientSocket in clientSocketList)
            {
                if (clientSocket.RemoteEndPoint.ToString() == clientEP.ToString())
                {
                    clientSocketList.Remove(clientSocket);
                    clientSocket.Close();
                    break;
                }
            }
        }
        else
        {
            clientEPList.Remove(clientEP);
            clientsUDP.Remove(clientEP);
        }

        // Remove color form list and add color to available colors
        colorPlayerAvailable.Add(clientData.colorPlayer);
        colorPlayerList.Remove(clientData.colorPlayer);
    }

    private ColorPlayer GetRandomColorPlayer()
    {
        ColorPlayer colorPlayer = new ColorPlayer();
        
        int randomIndex = UnityEngine.Random.Range(0, colorPlayerAvailable.Count);
        colorPlayer = colorPlayerAvailable[randomIndex];
        colorPlayerAvailable.Remove(colorPlayer);
        colorPlayerList.Add(colorPlayer);

        return colorPlayer;
    }
    #endregion HELPERS

    #region MAPGEN
    private Cell[,] GenerateFullMap()
    {
        DifficultyNew difficulty = DifficultyNew.Extreme;

        int[] widthHeightMines = GameGenerator.GetWidthHeightMines((DifficultyNew)(int)difficulty);
        int width = widthHeightMines[0];
        int height = widthHeightMines[1];
        int mines = widthHeightMines[2];
        Cell[,] cells = GameGenerator.GenerateCells(width, height);
        int[] bounds = GameGenerator.GetMultiplayerBigCenterBounds(width / 2, height / 2);
        cells = GameGenerator.GenerateMines(cells, mines, bounds);
        cells = GameGenerator.GenerateNumbers(cells);
        cells = GameResolver.ResolveCells(cells, width / 2, height / 2);
        return cells;
    }
    #endregion MAPGEN
}
