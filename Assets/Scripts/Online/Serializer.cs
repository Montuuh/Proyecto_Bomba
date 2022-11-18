using System.IO;
using Unity.VisualScripting;
using UnityEngine;

public enum SenderType
{
    NONE,
    STRING, // This will serve as a message from server
    CLIENTDATA, // For the moment clientdata, but in the future it will be like CELL, EMOJI... that will also include client data
    CLIENTCHAT,
    CLIENTCELL,
    STARTGAME,
    CLIENTDISCONNECT,
    CLIENTCONNECT,
    SENDBOARD
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
                break;
            case SenderType.CLIENTCHAT:
                writer.Write((int)sender.senderType); // 1 -> int
                writer.Write(sender.clientData.userID); // 2 -> uint
                writer.Write(sender.clientData.userName); // 3 -> string
                writer.Write((int)sender.clientData.colorPlayer); // 4 -> int
                writer.Write(sender.clientChat); // 5 -> string
                break;
            case SenderType.CLIENTCELL:
                writer.Write((int)sender.senderType); // 1 -> int
                writer.Write(sender.clientData.userID); // 2 -> uint
                writer.Write(sender.clientData.userName); // 3 -> string
                writer.Write((int)sender.clientData.colorPlayer); // 4 -> int
                writer.Write(sender.cellPosX); // 5 -> int
                writer.Write(sender.cellPosY); // 6 -> int
                break;
            case SenderType.STARTGAME:
                writer.Write((int)sender.senderType); // 1 -> int
                break;
            case SenderType.CLIENTDISCONNECT:
                writer.Write((int)sender.senderType); // 1 -> int
                writer.Write(sender.clientData.userID); // 2 -> uint
                writer.Write(sender.clientData.userName); // 3 -> string
                writer.Write((int)sender.clientData.colorPlayer); // 4 -> int
                break;
            case SenderType.CLIENTCONNECT:
                writer.Write((int)sender.senderType); // 1 -> int
                writer.Write(sender.clientData.userID); // 2 -> uint
                writer.Write(sender.clientData.userName); // 3 -> string
                writer.Write((int)sender.clientData.colorPlayer); // 4 -> int
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
        int cellPosX, cellPosY = 0;
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
                sender.clientData = new ClientData(userID, userName, colorPlayer);
                break;
            case SenderType.CLIENTCHAT:
                userID = reader.ReadUInt32(); // 2 -> uint
                userName = reader.ReadString(); // 3 -> string
                colorPlayer = (ColorPlayer)reader.ReadInt32(); // 4 -> int
                clientChat = reader.ReadString(); // 5 -> string
                sender.clientChat = clientChat; // 5 -> string
                sender.clientData = new ClientData(userID, userName, colorPlayer);
                break;
            case SenderType.CLIENTCELL:
                userID = reader.ReadUInt32(); // 2 -> uint
                userName = reader.ReadString(); // 3 -> string
                colorPlayer = (ColorPlayer)reader.ReadInt32(); // 4 -> int
                cellPosX = reader.ReadInt32(); // 5 -> int
                cellPosY = reader.ReadInt32(); // 6 -> int
                sender.clientData = new ClientData(userID, userName, colorPlayer);
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
                sender.clientData = new ClientData(userID, userName, colorPlayer);
                break;
            case SenderType.CLIENTCONNECT:
                userID = reader.ReadUInt32(); // 2 -> uint
                userName = reader.ReadString(); // 3 -> string
                colorPlayer = (ColorPlayer)reader.ReadInt32(); // 4 -> int
                sender.clientData = new ClientData(userID, userName, colorPlayer);
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
            default:
                Debug.Log("[SERIALIZER] Trying to deserialize NONE type...");
                break;
        }
        return sender;
    }
}
