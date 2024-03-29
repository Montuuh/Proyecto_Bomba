using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[SerializeField]
public class Message
{
    public string text;
    public TMP_Text textObject;
    public ClientData clientData;
}

public class Chat : MonoBehaviour
{
    List<Message> currentMessages = new List<Message>();
    public int maxMessages = 25;
    public GameObject chatPanel, textObject, playerPanel;
    List<Message> currentPlayers = new List<Message>();

    // Send message
    public void SendMessageToChat(ClientData clientData, string text)
    {
        if (currentMessages.Count >= maxMessages)
        {
            Destroy(currentMessages[0].textObject.gameObject);
            currentMessages.Remove(currentMessages[0]);
        }

        Message newMessage = new Message();
        newMessage.clientData = clientData;
        newMessage.text = text;

        if (newMessage.clientData != null)
            newMessage.text = newMessage.clientData.userName + ": " + text;

        GameObject newText = Instantiate(textObject, chatPanel.transform);
        newMessage.textObject = newText.GetComponent<TMP_Text>();
        newMessage.textObject.text = newMessage.text;

        currentMessages.Add(newMessage);
    }

    // Player list UI
    public void AddPlayerToHUD(ClientData clientData)
    {
        // If player is already in the list, skip the instantiate of the gameobject
        foreach (Message message in currentPlayers)
        {
            if (message.clientData.userID == clientData.userID)
            {
                message.textObject.text = clientData.userName;
                return;
            }
        }
        
        GameObject newText = Instantiate(textObject, playerPanel.transform);
        Message newMessage = new Message();
        newMessage.clientData = clientData;
        newMessage.text = clientData.userName;
        newMessage.textObject = newText.GetComponent<TMP_Text>();
        newMessage.textObject.text = newMessage.text;

        switch (clientData.colorPlayer)
        {
            case ColorPlayer.RED:
                newMessage.textObject.color = Color.red;
                break;
            case ColorPlayer.BLUE:
                newMessage.textObject.color = Color.blue;
                break;
            case ColorPlayer.GREEN:
                newMessage.textObject.color = Color.green;
                break;
            case ColorPlayer.YELLOW:
                newMessage.textObject.color = Color.yellow;
                break;
            default:
                break;
        }

        currentPlayers.Add(newMessage);
    }

    public void RemovePlayerFromHUD(ClientData clientData)
    {
        for (int i = 0; i < currentPlayers.Count; i++)
        {
            if (currentPlayers[i].clientData == clientData)
            {
                Destroy(currentPlayers[i].textObject.gameObject);
                currentPlayers.Remove(currentPlayers[i]);
            }
        }
    }

    public void RemoveAllPlayersFromHUD()
    {
        for (int i = 0; i < currentPlayers.Count; i++)
        {
            Destroy(currentPlayers[i].textObject.gameObject);
        }
        currentPlayers.Clear();
    }
}
