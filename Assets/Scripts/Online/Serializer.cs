using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using System.Collections.Generic;

public enum SenderType
{
    NONE,
    STRING, // This will serve as a message from server
    CLIENTDATA,
    CLIENTCHAT,
    CLIENTCELL,
    STARTGAME,
    CLIENTDISCONNECT,
    CLIENTCONNECT,
    SENDBOARD,
    SENDCLIENTLIST,
    CLOSESERVER,
    MOUSE
}

public class Sender
{
    public ClientData clientData;
    public string message;
    public SenderType senderType;
    public string clientChat;
    public int cellPosX;
    public int cellPosY;
    public Cell[,] cells;
    public ClientData[] clientList;
    public int mousePosX;
    public int mousePosY;

    public Sender(SenderType senderType)
    {
        this.senderType = senderType;
    }
}

public static class Serialize
{
    public static byte[] SerializeSender(Sender sender)
    {
        MemoryStream stream = new MemoryStream();
        BinaryWriter writer = new BinaryWriter(stream);
        
        switch (sender.senderType)
        {
            case SenderType.STRING:
                writer.Write((int)sender.senderType); // 1 -> int
                writer.Write(sender.message); // 2 -> string
                break;
            case SenderType.CLIENTDATA:
                writer.Write((int)sender.senderType); // 1 -> int
                writer.Write(sender.clientData.userID); // 2 -> uint
                writer.Write(sender.clientData.userName); // 3 -> string
                writer.Write((int)sender.clientData.colorPlayer); // 4 -> int
                writer.Write(sender.clientData.score); // 5 --> int
                break;
            case SenderType.CLIENTCHAT:
                writer.Write((int)sender.senderType); // 1 -> int
                writer.Write(sender.clientData.userID); // 2 -> uint
                writer.Write(sender.clientData.userName); // 3 -> string
                writer.Write((int)sender.clientData.colorPlayer); // 4 -> int
                writer.Write(sender.clientData.score); // 5 --> int
                writer.Write(sender.clientChat); // 6 -> string
                break;
            case SenderType.CLIENTCELL:
                writer.Write((int)sender.senderType); // 1 -> int
                writer.Write(sender.clientData.userID); // 2 -> uint
                writer.Write(sender.clientData.userName); // 3 -> string
                writer.Write((int)sender.clientData.colorPlayer); // 4 -> int
                writer.Write(sender.clientData.score); // 5 --> int
                writer.Write(sender.cellPosX); // 6 -> int
                writer.Write(sender.cellPosY); // 7 -> int
                break;
            case SenderType.STARTGAME:
                writer.Write((int)sender.senderType); // 1 -> int
                break;
            case SenderType.CLIENTDISCONNECT:
                writer.Write((int)sender.senderType); // 1 -> int
                writer.Write(sender.clientData.userID); // 2 -> uint
                writer.Write(sender.clientData.userName); // 3 -> string
                writer.Write((int)sender.clientData.colorPlayer); // 4 -> int
                writer.Write(sender.clientData.score); // 5 --> int
                break;
            case SenderType.CLIENTCONNECT:
                writer.Write((int)sender.senderType); // 1 -> int
                writer.Write(sender.clientData.userID); // 2 -> uint
                writer.Write(sender.clientData.userName); // 3 -> string
                writer.Write((int)sender.clientData.colorPlayer); // 4 -> int
                writer.Write(sender.clientData.score); // 5 --> int
                writer.Write(sender.clientData.playerNumber); // 6 -> int

                break;
            case SenderType.SENDBOARD:
                writer.Write((int)sender.senderType); // 1 -> int
                writer.Write(sender.cells.GetLength(0)); // 2 -> int
                writer.Write(sender.cells.GetLength(1)); // 3 -> int
                for (int x = 0; x < sender.cells.GetLength(0); x++)
                {
                    for (int y = 0; y < sender.cells.GetLength(1); y++)
                    {
                        writer.Write((int)sender.cells[x, y].cellType); // 4 -> int
                        writer.Write(sender.cells[x, y].isRevealed); // 5 -> bool
                    }
                }
                break;
            case SenderType.SENDCLIENTLIST:
                writer.Write((int)sender.senderType); // 1 -> int
                if (sender.clientList != null)
                {
                    writer.Write(sender.clientList.Length); // 2 -> int
                    for (int i = 0; i < sender.clientList.Length; i++)
                    {
                        writer.Write(sender.clientList[i].userID); // 3 -> uint
                        writer.Write(sender.clientList[i].userName); // 4 -> string
                        writer.Write((int)sender.clientList[i].colorPlayer); // 5 -> int
                        writer.Write(sender.clientList[i].score); // 6 --> int
                    }
                }
                else
                {
                    writer.Write(0); // 2 -> int
                }
                break;
            case SenderType.CLOSESERVER:
                writer.Write((int)sender.senderType); // 1 -> int
                break;
            case SenderType.MOUSE:
                writer.Write((int)sender.senderType); // 1 -> int
                writer.Write(sender.clientData.userID); // 2 -> uint
                writer.Write(sender.clientData.userName); // 3 -> string
                writer.Write((int)sender.clientData.colorPlayer); // 4 -> int
                writer.Write(sender.clientData.score); // 5 --> int

                writer.Write(sender.mousePosX); // 6 -> int
                writer.Write(sender.mousePosY); // 7 -> int
                writer.Write(sender.clientData.playerNumber); // 8 -> int
                break;
            default:
                Debug.Log("[SERIALIZER] Trying to serialize NONE type...");
                break;
        }

        return stream.ToArray();
    }

    public static Sender DeserializeSender(byte[] data)
    {
        MemoryStream stream = new MemoryStream(data);
        stream.Write(data, 0, data.Length);

        BinaryReader reader = new BinaryReader(stream);
        stream.Seek(0, SeekOrigin.Begin);

        SenderType senderType = (SenderType)reader.ReadInt32(); // 1 -> int

        Sender sender = new Sender(senderType);
        uint userID = 0;
        int cellPosX, cellPosY, playerNumber, score = 0;
        string userName, clientChat, message = "";
        ColorPlayer colorPlayer = ColorPlayer.NONE;
        switch (sender.senderType)
        {
            case SenderType.STRING:
                message = reader.ReadString(); // 2 -> string
                sender.message = message; // 2 -> string
                break;
            case SenderType.CLIENTDATA:
                userID = reader.ReadUInt32(); // 2 -> uint
                userName = reader.ReadString(); // 3 -> string
                colorPlayer = (ColorPlayer)reader.ReadInt32(); // 4 -> int
                score = reader.ReadInt32(); // 5 --> int
                sender.clientData = new ClientData(userID, userName, colorPlayer) { score = score };
                break;
            case SenderType.CLIENTCHAT:
                userID = reader.ReadUInt32(); // 2 -> uint
                userName = reader.ReadString(); // 3 -> string
                colorPlayer = (ColorPlayer)reader.ReadInt32(); // 4 -> int
                score = reader.ReadInt32(); // 5 --> int
                clientChat = reader.ReadString(); // 6 -> string
                sender.clientChat = clientChat; // 7 -> string
                sender.clientData = new ClientData(userID, userName, colorPlayer) { score = score };
                break;
            case SenderType.CLIENTCELL:
                userID = reader.ReadUInt32(); // 2 -> uint
                userName = reader.ReadString(); // 3 -> string
                colorPlayer = (ColorPlayer)reader.ReadInt32(); // 4 -> int
                score = reader.ReadInt32(); // 5 --> int
                cellPosX = reader.ReadInt32(); // 6 -> int
                cellPosY = reader.ReadInt32(); // 7 -> int
                sender.clientData = new ClientData(userID, userName, colorPlayer) { score = score };
                sender.cellPosX = cellPosX;
                sender.cellPosY = cellPosY;
                break;
            case SenderType.STARTGAME:
                // Does not need anything, just the event. Maybe in the future will need the clientdata of the host who started
                break;
            case SenderType.CLIENTDISCONNECT:
                userID = reader.ReadUInt32(); // 2 -> uint
                userName = reader.ReadString(); // 3 -> string
                colorPlayer = (ColorPlayer)reader.ReadInt32(); // 4 -> int
                score = reader.ReadInt32(); // 5 --> int
                sender.clientData = new ClientData(userID, userName, colorPlayer) { score = score };
                break;
            case SenderType.CLIENTCONNECT:
                userID = reader.ReadUInt32(); // 2 -> uint
                userName = reader.ReadString(); // 3 -> string
                colorPlayer = (ColorPlayer)reader.ReadInt32(); // 4 -> int
                playerNumber = reader.ReadInt32(); // 5 -> int
                score = reader.ReadInt32(); // 6 --> int

                sender.clientData = new ClientData(userID, userName, colorPlayer) { playerNumber = playerNumber, score = score };
                break;
            case SenderType.SENDBOARD:
                int width = reader.ReadInt32(); // 2 -> int
                int height = reader.ReadInt32(); // 3 -> int
                Cell[,] cells = new Cell[width, height];
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        Cell.CellType cellType = (Cell.CellType)reader.ReadInt32(); // 4 -> int
                        bool isRevealed = reader.ReadBoolean(); // 5 -> bool
                        cells[x, y] = new Cell { cellType = cellType, isRevealed = isRevealed };
                    }
                }
                sender.cells = cells;
                break;
            case SenderType.SENDCLIENTLIST:
                int clientListCount = reader.ReadInt32(); // 2 -> int
                if (clientListCount > 0)
                {
                    ClientData[] clientList = new ClientData[clientListCount];
                    for (int i = 0; i < clientListCount; i++)
                    {
                        userID = reader.ReadUInt32(); // 3 -> uint
                        userName = reader.ReadString(); // 4 -> string
                        colorPlayer = (ColorPlayer)reader.ReadInt32(); // 5 -> int
                        score = reader.ReadInt32(); // 6 --> int

                        clientList[i] = new ClientData(userID, userName, colorPlayer) { score = score };
                    }
                    sender.clientList = clientList;
                }
                break;
            case SenderType.CLOSESERVER:
                // Does not need anything, just the event. Maybe in the future will need the clientdata of the host who started
                break;
            case SenderType.MOUSE:
                userID = reader.ReadUInt32(); // 2 -> uint
                userName = reader.ReadString(); // 3 -> string
                colorPlayer = (ColorPlayer)reader.ReadInt32(); // 4 -> int
                score = reader.ReadInt32(); // 5 --> int
                int mousePosX = reader.ReadInt32(); // 6 -> int
                int mousePosY = reader.ReadInt32(); // 7 -> int
                playerNumber = reader.ReadInt32(); // 8 -> int

                sender.clientData = new ClientData(userID, userName, colorPlayer) { playerNumber = playerNumber, score = score };
                sender.mousePosX = mousePosX;
                sender.mousePosY = mousePosY;
                break;
            default:
                Debug.Log("[SERIALIZER] Trying to deserialize NONE type...");
                break;
        }
        return sender;
    }
}
