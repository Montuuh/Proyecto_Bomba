using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;


[SerializeField]
public class Message
{
    public string text;
    public TMP_Text textObject;
}

public class Chat : MonoBehaviour
{
    List<Message> messages = new List<Message>();
    public List<Message> pendingMessages = new List<Message>();
    public List<ClientData> pendingClients = new List<ClientData>();
    public List<ClientData> pendingPlayers = new List<ClientData>();
    public int maxMessages = 25;
    public GameObject chatPanel, textObject;
    public GameObject playerPanel;
    Transform chatPosition;

    private void Update()
    {
        if (!gameObject.activeSelf)
        {
            pendingMessages.Clear();
        }

        if (pendingMessages.Count > 0)
        {
            SendMessageToChat(pendingClients[0], pendingMessages[0].text);
            pendingMessages.Remove(pendingMessages[0]);
            pendingClients.Remove(pendingClients[0]);
        }

        if (pendingPlayers.Count > 0)
        {
            AddPlayerToHUD(pendingPlayers[0]);
            pendingPlayers.Remove(pendingPlayers[0]);
        }
    }

    public void SetPendingMessage(ClientData clientData, string text)
    {
        Message newMessage = new Message();

        newMessage.text = text;

        pendingMessages.Add(newMessage);
        pendingClients.Add(clientData);
    }

    public void SendMessageToChat(ClientData clientData, string text)
    {
        if (messages.Count >= maxMessages)
        {
            Destroy(messages[0].textObject.gameObject);
            messages.Remove(messages[0]);
        }

        Message newMessage = new Message();
        newMessage.text = text;
        
        if (clientData != null)
            newMessage.text = clientData.userName + ": " + text;

        GameObject newText = Instantiate(textObject, chatPanel.transform);

        newMessage.textObject = newText.GetComponent<TMP_Text>();
        newMessage.textObject.text = newMessage.text;

        messages.Add(newMessage);
    }

    public void AddPlayer(ClientData clientData)
    {
        pendingPlayers.Add(clientData);
    }

    public void AddPlayerToHUD(ClientData clientData)
    {
        GameObject newText = Instantiate(textObject, playerPanel.transform);
        Message newMessage = new Message();
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
    }
}
