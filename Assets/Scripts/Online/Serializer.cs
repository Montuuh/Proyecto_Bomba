using System.IO;
using UnityEngine;

public enum SenderType
{
    NONE,
    STRING, // This will serve as a message from server
    CLIENTDATA, // For the moment clientdata, but in the future it will be like CELL, EMOJI... that will also include client data
    CLIENTSTRING,
    CLIENTCELL,
    STARTGAME,
    CLIENTDISCONNECT,
    CLIENTCONNECT
}

public class Sender
{
    public ClientData clientData;
    public string message;
    public SenderType senderType;
    public string clientChat;
    public int cellPosX;
    public int cellPosY;

    public Sender()
    {
        clientData = new ClientData();
        message = "";
        senderType = SenderType.NONE;
        clientChat = "";
        cellPosX = 0;
        cellPosY = 0;
    }
    public Sender(ClientData clientData)
    {
        this.clientData = clientData;
        senderType = SenderType.CLIENTDATA;
    }
    public Sender(string message)
    {
        this.message = message;
        senderType = SenderType.STRING;
    }
    public Sender(ClientData clientData, string clientString)
    {
        this.clientData = clientData;
        this.clientChat = clientString;
        senderType = SenderType.CLIENTSTRING;
    }
    public Sender(ClientData clientData, int cellPosX, int cellPosY)
    {
        this.clientData = clientData;
        this.cellPosX = cellPosX;
        this.cellPosY = cellPosY;
        senderType = SenderType.CLIENTCELL;
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
                break;
            case SenderType.CLIENTSTRING:
                writer.Write((int)sender.senderType); // 1 -> int
                writer.Write(sender.clientData.userID); // 2 -> uint
                writer.Write(sender.clientData.userName); // 3 -> string
                writer.Write(sender.clientChat); // 4 -> string
                break;
            case SenderType.CLIENTCELL:
                writer.Write((int)sender.senderType); // 1 -> int
                writer.Write(sender.clientData.userID); // 2 -> uint
                writer.Write(sender.clientData.userName); // 3 -> string
                writer.Write(sender.cellPosX); // 4 -> int
                writer.Write(sender.cellPosY); // 5 -> int
                break;
            case SenderType.STARTGAME:
                writer.Write((int)sender.senderType); // 1 -> int
                break;
            case SenderType.CLIENTDISCONNECT:
                writer.Write((int)sender.senderType); // 1 -> int
                writer.Write(sender.clientData.userID); // 2 -> uint
                writer.Write(sender.clientData.userName); // 3 -> string
                break;
            case SenderType.CLIENTCONNECT:
                writer.Write((int)sender.senderType); // 1 -> int
                writer.Write(sender.clientData.userID); // 2 -> uint
                writer.Write(sender.clientData.userName); // 3 -> string
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

        if (senderType == SenderType.STRING)
        {
            string message = reader.ReadString(); // 2 -> string
            Sender sender = new Sender(message);
            return sender;
        }
        else if (senderType == SenderType.CLIENTDATA)
        {
            uint uid = reader.ReadUInt32(); // 2 -> uint
            string username = reader.ReadString(); // 3 -> string
            Sender sender = new Sender(new ClientData(uid, username));
            return sender;
        }
        else if (senderType == SenderType.CLIENTSTRING)
        {
            uint uid = reader.ReadUInt32(); // 2 -> uint
            string username = reader.ReadString(); // 3 -> string
            string clientString = reader.ReadString(); // 4 -> string
            Sender sender = new Sender(new ClientData(uid, username), clientString);
            return sender;
        }
        else if (senderType == SenderType.CLIENTCELL)
        {
            uint uid = reader.ReadUInt32(); // 2 -> uint
            string username = reader.ReadString(); // 3 -> string
            int x = reader.ReadInt32(); // 4 -> int
            int y = reader.ReadInt32(); // 5 -> int
            Sender sender = new Sender(new ClientData(uid, username), x, y);
            return sender;
        }
        else if (senderType == SenderType.STARTGAME)
        {
            Sender sender = new Sender(new ClientData(0, ""), "");
            sender.senderType = SenderType.STARTGAME;
            return sender;
        }
        else if (senderType == SenderType.CLIENTDISCONNECT)
        {
            uint uid = reader.ReadUInt32(); // 2 -> uint
            string username = reader.ReadString(); // 3 -> string
            Sender sender = new Sender(new ClientData(uid, username));
            sender.senderType = SenderType.CLIENTDISCONNECT;
            return sender;
        }
        else if (senderType == SenderType.CLIENTCONNECT)
        {
            uint uid = reader.ReadUInt32(); // 2 -> uint
            string username = reader.ReadString(); // 3 -> string
            Sender sender = new Sender(new ClientData(uid, username));
            sender.senderType = SenderType.CLIENTCONNECT;
            return sender;
        }
        else
        {
            Debug.LogError("[SERIALIZER] Trying to deserialize NONE type...");
            return null;
        }
    }
}
