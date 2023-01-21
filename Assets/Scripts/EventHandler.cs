using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventHandler : MonoBehaviour
{
    public Client client;
    private ColorPlayer clientColor;

    public void SendRevealCell(int x, int y)
    {
        Debug.Log("[EVENT] SendRevealCell event received, SendRevealCell: " + x + ", " + y + " from " + client.clientData.userName);
        client.SendClientCell(client.clientData, x, y);
    }

    //First Cell does not go to any player
    public void SendFirstCell(int x, int y)
    {
        Debug.Log("[EVENT] SendRevealCell event received, SendRevealCell: " + x + ", " + y + " from " + client.clientData.userName);

        clientColor = client.clientData.colorPlayer;
        client.clientData.colorPlayer = ColorPlayer.NONE;
        client.SendClientCell(client.clientData, x, y);
        client.clientData.colorPlayer = clientColor;
    }

    public void SendFlagCell(int x, int y)
    {
        Debug.Log("[EVENT] SendFlagCell event received, SendFlagCell: " + x + ", " + y + " from " + client.clientData.userName);
        client.SendClientCell(client.clientData, x, y);
    }

    public void SendMousePosition(float mouseX, float mouseY)
    {
        Debug.Log("[EVENT] SendCursorPosition event received, SendCursorPosition: " + mouseX + ", " + mouseY + ". From " + client.clientData.userName);
        client.SendMousePosition(client.clientData, mouseX, mouseY);
    }

}
