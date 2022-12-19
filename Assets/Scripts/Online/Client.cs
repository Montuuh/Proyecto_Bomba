using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Text;
using System.IO;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

// color enum
public enum ColorPlayer
{
    NONE,
    RED,
    BLUE,
    GREEN,
    YELLOW
}

public class MouseData
{
    public int x;
    public int y;
    public ColorPlayer color;
    public int playerNumber;
}

public class ClientData
{
    public string userName;
    public uint userID;
    public ColorPlayer colorPlayer;
    public bool isHost = false;
    public int playerNumber = 0;
    public int score = 0;

    public ClientData()
    {
        SetRandomGuest();
        colorPlayer = ColorPlayer.NONE;
    }
    public ClientData(uint userID, string userName, ColorPlayer colorPlayer)
    {
        this.userName = userName;
        this.userID = userID;
        this.colorPlayer = colorPlayer;
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
    public ClientData clientData;
    
    private Protocol protocol;
    
    private string serverIP;
    
    private Thread clientThread;
    private Socket socket;
    private IPEndPoint serverIPEP;
    private EndPoint serverEP;
    public string serverCode;

    [HideInInspector]
    public MultiPlayerGame game;
    private Dictionary<Vector2Int, ClientData> pendingRevealedCells = new Dictionary<Vector2Int, ClientData>();
    private Cell[,] cellsToUpload;
    
    public Chat chat;
    private List<Message> pendingMessages = new List<Message>();
    private List<ClientData> pendingPlayers = new List<ClientData>();

    private List<SceneManager.Scene> pendingScenes = new List<SceneManager.Scene>();

    public CursorManager cursorManager;
    private List<MouseData> pendingMousePositions = new List<MouseData>();

    [SerializeField]
    public List<ClientData> lobbyPlayers = new List<ClientData>();
    public int a = 12;

    public ScoreManager scoreManager;

    void Start()
    {
        if (clientData == null)
            clientData = new ClientData();
    }

    private void Update()
    {
        if (pendingScenes.Count > 0)
        {
            SceneManager.LoadScene(pendingScenes[0]);
            pendingScenes.Remove(pendingScenes[0]);
        }

        if (game != null)
        {

            if (cellsToUpload != null)
            {
                game.StartGame(cellsToUpload);
                cellsToUpload = null;
            }
            if (pendingRevealedCells.Count > 0)
            {
                foreach (KeyValuePair<Vector2Int, ClientData> revealedCell in pendingRevealedCells)
                {
                    game.Reveal(revealedCell.Key.x, revealedCell.Key.y, revealedCell.Value);
                }
                pendingRevealedCells.Clear();
            }
        }

        if (chat == null)
        {
            chat = FindObjectOfType<Chat>();
        }
        else
        {
            if (pendingMessages.Count > 0)
            {
                chat.SendMessageToChat(pendingMessages[0].clientData, pendingMessages[0].text);
                pendingMessages.RemoveAt(0);
            }
            
            if (pendingPlayers.Count > 0)
            {
                bool existing = false;

                foreach (var player in lobbyPlayers)
                {

                    if(player.userID == pendingPlayers[0].userID)
                        existing = true;
                }
                
                if (!existing)
                {
                    lobbyPlayers.Add(pendingPlayers[0]);
                    scoreManager.currentPlayingPlayers = lobbyPlayers;
                }

                existing = false;

                chat.AddPlayerToHUD(pendingPlayers[0]);
                pendingPlayers.RemoveAt(0);
            }
        }

        if (cursorManager == null)
        {
            cursorManager = FindObjectOfType<CursorManager>();
        }
        else
        {
            if (pendingMousePositions.Count > 0)
            {
                cursorManager.UpdateCursor(pendingMousePositions[0].x, pendingMousePositions[0].y, pendingMousePositions[0].color, pendingMousePositions[0].playerNumber);
                pendingMousePositions.RemoveAt(0);
            }
        }
    }

    private void OnDisable()
    {
        DisconnectFromServer();
    }
    
    public void ConnectToServer(string ip, bool isTcp = false)
    {
        serverIP = ip;
        if (isTcp)
            protocol = Protocol.TCP;
        else
            protocol = Protocol.UDP;

        // Socket initialization
        if (protocol == Protocol.TCP)
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        else
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        // Thread initialization
        clientThread = new Thread(ClientThread);
        clientThread.IsBackground = true;
        clientThread.Start();

        serverCode = IPAddressHelper.EncodeIPAddress(ip);
    }

    public void DisconnectFromServer()
    {
        if (socket != null)
            socket.Close();
        if (clientThread != null)
            clientThread.Abort();
    }
    private void ClientThread()
    {
        // Client IP EndPoint
        IPAddress ipAddress = IPAddress.Parse(serverIP);
        serverIPEP = new IPEndPoint(ipAddress, 9500);
        serverEP = (EndPoint)serverIPEP;
        Debug.Log("[CLIENT] Trying to connect to server --> " + serverIPEP.ToString());

        if (protocol == Protocol.TCP)
        {
            // ToDo: Not yet adapted to serializing
            socket.Connect(serverIPEP);

            if (socket.Connected)
            {
                Debug.Log("[CLIENT] Connected to server --> " + serverIP + ":9500");
                // Send welcome message
                SendClientChat(clientData, "Username: " + clientData.userName + " | UID: " + clientData.userID + " | Connected to server");

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
                byte[] data = new byte[2048];
                int recv = socket.ReceiveFrom(data, ref serverEP);
                Sender sender = Serialize.DeserializeSender(data);
                DecodeSender(sender);
                // !RECEIVE DATA
            }
        }
    }

    #region SENDERS
    public void SendClientChat(ClientData _clientData, string message)
    {
        Sender sender = new Sender(SenderType.CLIENTCHAT) { clientData = _clientData, clientChat = message };

        byte[] data = Serialize.SerializeSender(sender);

        if (protocol == Protocol.TCP)
        {
            // ToDo: Not yet adapted to serializing
            Debug.Log("[CLIENT] Sending to server: " + socket.RemoteEndPoint.ToString() + " Message: " + message);
            socket.Send(data, data.Length, SocketFlags.None);
        }
        else
        {
            Debug.Log("[CLIENT] Sending to server: " + serverIPEP.ToString() + " Message: " + sender.clientChat);
            socket.SendTo(data, data.Length, SocketFlags.None, serverEP);
        }
    }

    public void SendClientData(ClientData _clientData)
    {
        Sender sender = new Sender(SenderType.CLIENTDATA) { clientData = _clientData };

        byte[] data = Serialize.SerializeSender(sender);
        
        if (protocol == Protocol.TCP)
        {
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
        Sender sender = new Sender(SenderType.CLIENTCELL) { clientData = _clientData, cellPosX = _cellPosX, cellPosY = _cellPosY };

        byte[] data = Serialize.SerializeSender(sender);

        if (protocol == Protocol.TCP)
        {
            Debug.Log("[CLIENT] Sending CLIENTCELL to server: " + socket.RemoteEndPoint.ToString() + " || Sender type: " + sender.senderType + " || Sender username and UID: " + sender.clientData.userName + " | " + sender.clientData.userID + " || Cell revealed: " + sender.cellPosX + " | " + sender.cellPosY);
            socket.Send(data, data.Length, SocketFlags.None);
        }
        else
        {
            Debug.Log("[CLIENT] Sending CLIENTCELL to server: " + serverIPEP.ToString() + " || Sender type: " + sender.senderType + " || Sender username and UID: " + sender.clientData.userName + " | " + sender.clientData.userID + " || Cell revealed: " + sender.cellPosX + " | " + sender.cellPosY);
            socket.SendTo(data, data.Length, SocketFlags.None, serverEP);
        }
    }

    public void SendStartGame()
    {
        Sender sender = new Sender(SenderType.STARTGAME);

        byte[] data = Serialize.SerializeSender(sender);

        if (protocol == Protocol.TCP)
        {
            Debug.Log("[CLIENT] Sending STARTGAME to server: " + socket.RemoteEndPoint.ToString() + " || Sender type: " + sender.senderType);
            socket.Send(data, data.Length, SocketFlags.None);
        }
        else
        {
            Debug.Log("[CLIENT] Sending STARTGAME to server: " + serverIPEP.ToString() + " || Sender type: " + sender.senderType);
            socket.SendTo(data, data.Length, SocketFlags.None, serverEP);
        }
    }

    public void GetClientListFromServer()
    {
        Sender sender = new Sender(SenderType.SENDCLIENTLIST);

        byte[] data = Serialize.SerializeSender(sender);

        if (protocol == Protocol.TCP)
        {
            Debug.Log("[CLIENT] Sending SENDCLIENTLIST to server: " + socket.RemoteEndPoint.ToString() + " || Sender type: " + sender.senderType);
            socket.Send(data, data.Length, SocketFlags.None);
        }
        else
        {
            Debug.Log("[CLIENT] Sending SENDCLIENTLIST to server: " + serverIPEP.ToString() + " || Sender type: " + sender.senderType);
            socket.SendTo(data, data.Length, SocketFlags.None, serverEP);
        }
    }

    public void SendMousePosition(int _mousePosX, int _mousePosY)
    {
        Sender sender = new Sender(SenderType.MOUSE) { mousePosX = _mousePosX, mousePosY = _mousePosY, clientData = clientData };

        byte[] data = Serialize.SerializeSender(sender);

        if (protocol == Protocol.TCP)
        {
            Debug.Log("[CLIENT] Sending MOUSEPOSITION to server: " + socket.RemoteEndPoint.ToString() + " || Sender type: " + sender.senderType + " || Mouse position: " + sender.mousePosX + " | " + sender.mousePosY);
            socket.Send(data, data.Length, SocketFlags.None);
        }
        else
        {
            Debug.Log("[CLIENT] Sending MOUSEPOSITION to server: " + serverIPEP.ToString() + " || Sender type: " + sender.senderType + " || Mouse position: " + sender.mousePosX + " | " + sender.mousePosY);
            socket.SendTo(data, data.Length, SocketFlags.None, serverEP);
        }
    }
    #endregion SENDERS

    #region DECODERS
    private void DecodeSender(Sender sender)
    {
        switch (sender.senderType)
        {
            case SenderType.STRING:
                Debug.Log("[CLIENT] Received STRING sender type from server: " + sender.message);
                break;
            case SenderType.CLIENTDATA:
                Debug.Log("[CLIENT] Received CLIENTDATA sender type from server: " + sender.clientData.userName + " | " + sender.clientData.userID);
                break;
            case SenderType.CLIENTCHAT:
                Debug.Log("[CLIENT] Received CLIENTCHAT sender type from server: " + sender.clientData.userName + " | " + sender.clientData.userID + " || " + sender.clientChat);
                pendingMessages.Add(new Message { text = sender.clientChat, clientData = sender.clientData });                
                break;
            case SenderType.CLIENTCELL:
                Debug.Log("[CLIENT] Received CLIENTCELL sender type from server: " + sender.clientData.userName + " | " + sender.clientData.userID + " || " + sender.cellPosX + " | " + sender.cellPosY);
                pendingRevealedCells.Add(new Vector2Int(sender.cellPosX, sender.cellPosY), sender.clientData);
                break;
            case SenderType.STARTGAME:
                Debug.Log("[CLIENT] Received STARTGAME sender type from server");
                pendingScenes.Add(SceneManager.Scene.MultiplayerGame);
                break;
            case SenderType.CLIENTDISCONNECT:
                Debug.Log("[CLIENT] Received CLIENTDISCONNECT sender type from server: " + sender.clientData.userName + " | " + sender.clientData.userID);
                pendingMessages.Add(new Message { text = sender.clientData.userName + " has left the server :(" });
                // ToDo: Check if you have been disconnected
                break;
            case SenderType.CLIENTCONNECT:
                Debug.Log("[CLIENT] Received CLIENTCONNECT sender type from server: " + sender.clientData.userName + " | " + sender.clientData.userID);
                pendingMessages.Add(new Message { text = sender.clientData.userName + " has joined the server :)" });
                //pendingPlayers.Add(sender.clientData);
                if (sender.clientData.userID == clientData.userID)
                    clientData.playerNumber = sender.clientData.playerNumber;
                SetColorIfPlayer(sender.clientData);
                GetClientListFromServer();
                break;
            case SenderType.SENDBOARD:
                Debug.Log("[CLIENT] Received SENDBOARD sender type from server: ");
                cellsToUpload = sender.cells;
                break;
            case SenderType.SENDCLIENTLIST:
                Debug.Log("[CLIENT] Received SENDCLIENTLIST sender type from server: ");
                pendingPlayers = sender.clientList.ToList<ClientData>();
                break;
            case SenderType.MOUSE:
                Debug.Log("[CLIENT] Received MOUSE sender type from server: " + sender.clientData.userName + " | " + sender.clientData.userID + " || " + sender.mousePosX + " | " + sender.mousePosY);
                pendingMousePositions.Add(new MouseData { playerNumber = sender.clientData.playerNumber, x = sender.mousePosX, y = sender.mousePosY, color = sender.clientData.colorPlayer });
                break;
            default:
                Debug.Log("[CLIENT] Trying to decode UNKNOWN sender type...");
                break;
        }
    }
    #endregion DECODERS

    private void SetColorIfPlayer(ClientData clientData)
    {
        if (clientData.userName == this.clientData.userName && clientData.userID == this.clientData.userID)
        {
            this.clientData.colorPlayer = clientData.colorPlayer;
        }
    }
}
