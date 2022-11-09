using System.IO;
using UnityEngine;

public enum SenderType
{
    NONE,
    STRING, // This will serve as a message from server
    CLIENTDATA, // For the moment clientdata, but in the future it will be like CELL, EMOJI... that will also include client data
    CLIENTSTRING,
    CLIENTCELL
}

public class Sender
{
    public ClientData clientData;
    public string message;
    public SenderType senderType;
    public string clientString;
    public int cellPosX;
    public int cellPosY;

    public Sender()
    {
        clientData = new ClientData();
        message = "";
        senderType = SenderType.NONE;
        clientString = "";
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
        this.clientString = clientString;
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
        // Debug.Log("[SERIALIZER] Serializing...");

        MemoryStream stream = new MemoryStream();
        BinaryWriter writer = new BinaryWriter(stream);
        
        if (sender.senderType == SenderType.STRING)
        {
            writer.Write((int)sender.senderType); // 1 -> int
            writer.Write(sender.message); // 2 -> string
        }
        else if (sender.senderType == SenderType.CLIENTDATA)
        {
            writer.Write((int)sender.senderType); // 1 -> int
            writer.Write(sender.clientData.userID); // 2 -> uint
            writer.Write(sender.clientData.userName); // 3 -> string
        }
        else if (sender.senderType == SenderType.CLIENTSTRING)
        {
            writer.Write((int)sender.senderType); // 1 -> int
            writer.Write(sender.clientData.userID); // 2 -> uint
            writer.Write(sender.clientData.userName); // 3 -> string
            writer.Write(sender.clientString); // 4 -> string
        }
        else if (sender.senderType == SenderType.CLIENTCELL)
        {
            writer.Write((int)sender.senderType); // 1 -> int
            writer.Write(sender.clientData.userID); // 2 -> uint
            writer.Write(sender.clientData.userName); // 3 -> string
            writer.Write(sender.cellPosX); // 4 -> int
            writer.Write(sender.cellPosY); // 5 -> int
        }
        else
        {
            Debug.Log("[SERIALIZER] Error: Sender type not recognized");
        }
        return stream.ToArray();
    }

    public static Sender DeserializeSender(byte[] data)
    {
        // Debug.Log("[SERIALIZER] Deserializing...");

        MemoryStream stream = new MemoryStream(data);
        stream.Write(data, 0, data.Length);

        BinaryReader reader = new BinaryReader(stream);
        stream.Seek(0, SeekOrigin.Begin);

        SenderType type = (SenderType)reader.ReadInt32(); // 1 -> int
        if (type == SenderType.STRING)
        {
            string message = reader.ReadString(); // 2 -> string
            Sender sender = new Sender(message);
            return sender;
        }
        else if (type == SenderType.CLIENTDATA)
        {
            uint uid = reader.ReadUInt32(); // 2 -> uint
            string username = reader.ReadString(); // 3 -> string
            Sender sender = new Sender(new ClientData(uid, username));
            return sender;
        }
        else if (type == SenderType.CLIENTSTRING)
        {
            uint uid = reader.ReadUInt32(); // 2 -> uint
            string username = reader.ReadString(); // 3 -> string
            string clientString = reader.ReadString(); // 4 -> string
            Sender sender = new Sender(new ClientData(uid, username), clientString);
            return sender;
        }
        else if (type == SenderType.CLIENTCELL)
        {
            uint uid = reader.ReadUInt32(); // 2 -> uint
            string username = reader.ReadString(); // 3 -> string
            int x = reader.ReadInt32(); // 4 -> int
            int y = reader.ReadInt32(); // 5 -> int
            Sender sender = new Sender(new ClientData(uid, username), x, y);
            return sender;
        }
        else
        {
            Debug.LogError("[SERIALIZER] Error deserializing Sender");
            return null;
        }
    }
}
