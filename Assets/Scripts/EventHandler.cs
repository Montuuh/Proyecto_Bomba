using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventHandler : MonoBehaviour
{
    public Client client;

    void Start()
    {
        client = GameObject.Find("ClientManager").GetComponent<Client>();
    }

    public void SendRevealCell(int x, int y)
    {
        Debug.Log("[EVENT] SendRevealCell event received, SendRevealCell: " + x + ", " + y + " from " + client.clientData.userName);
        client.SendClientCell(client.clientData, x, y);
    }

    public void SendFlagCell(int x, int y)
    {
        Debug.Log("[EVENT] SendFlagCell event received, SendFlagCell: " + x + ", " + y + " from " + client.clientData.userName);
        client.SendClientCell(client.clientData, x, y);
    }
}
