using System.IO;
using UnityEngine;

public enum SenderType
{
    NONE,
    STRING, // This will serve as a message from server
    CLIENTDATA // For the moment clientdata, but in the future it will be like CELL, EMOJI... that will also include client data
}

public class Sender
{
    public ClientData clientData;
    public string message;
    public SenderType senderType;

    public Sender()
    {
        clientData = new ClientData();
        message = "";
        senderType = SenderType.NONE;
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
}

public static class Serialize
{
    public static byte[] SerializeSender(Sender sender)
    {
        Debug.Log("[SERIALIZER] Serializing...");

        uint uid = sender.clientData.userID;
        string username = sender.clientData.userName;

        MemoryStream stream = new MemoryStream();
        BinaryWriter writer = new BinaryWriter(stream);
        writer.Write((int)sender.senderType);
        writer.Write(uid);
        writer.Write(username);

        return stream.ToArray();
    }

    public static Sender DeserializeSender(byte[] data)
    {
        Debug.Log("[SERIALIZER] Deserializing...");

        MemoryStream stream = new MemoryStream(data);
        stream.Write(data, 0, data.Length);

        BinaryReader reader = new BinaryReader(stream);
        stream.Seek(0, SeekOrigin.Begin);

        SenderType type = (SenderType)reader.ReadInt32();
        if (type == SenderType.STRING)
        {
            Sender sender = new Sender(reader.ReadString());
            return sender;
        }
        else if (type == SenderType.CLIENTDATA)
        {
            Sender sender = new Sender(new ClientData());
            sender.clientData.userID = reader.ReadUInt32();
            sender.clientData.userName = reader.ReadString();
            return sender;
        }
        else
        {
            Debug.LogError("[SERIALIZER] Error deserializing Sender");
            return null;
        }
    }
}
