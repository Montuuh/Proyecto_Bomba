using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReadClientInput : MonoBehaviour
{
    public string clientInput;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ReadInputIP(string input)
    {
        clientInput = input;
        Debug.Log("ReadStringInput: " + input);
        
        if (clientInput.Contains("."))
        {
            // Get client script and connect to server
            switch (GameObject.Find("ClientManager").GetComponent<ServerManager>().protocol)
            {
                case ServerManager.Protocol.TCP:
                    GameObject.Find("ClientManager").GetComponent<TCPClient>().ConnectToServer();
                    break;
                case ServerManager.Protocol.UDP:
                    GameObject.Find("ClientManager").GetComponent<UDPClient>().ConnectToServer(clientInput);
                    break;
                default:
                    Debug.Log("Invalid protocol");
                    break;
            }
        }
    }
}
