using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerManager : MonoBehaviour
{
    public enum Protocol
    {
        TCP,
        UDP
    }

    public enum ServerType
    {
        Server,
        Client
    }
    
    public ServerType serverType = ServerType.Server;
    public Protocol protocol = Protocol.UDP;
    
    public string serverIP { get; set; } = "127.0.0.1";
    public int serverPort { get; set; } = 9500;

    void Start()
    {
        switch (serverType)
        {
            case ServerType.Server:
                switch (protocol)
                {
                    case Protocol.TCP:
                        // not yet implemented
                        
                        //gameObject.AddComponent<TCPServer>();
                        break;
                    case Protocol.UDP:
                        gameObject.AddComponent<UDPServer>();
                        break;
                }
                break;
            case ServerType.Client:
                switch (protocol)
                {
                    case Protocol.TCP:
                        // not yet implemented
                        
                        //gameObject.AddComponent<TCPClient>();
                        break;
                    case Protocol.UDP:
                        gameObject.AddComponent<UDPClient>();
                        break;
                }
                break;
        }
    }
}
