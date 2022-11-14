using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


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
    public int maxMessages = 25;
    public GameObject chatPanel, textObject;
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

        if (clientData == null)
        {
            newMessage.text = "[SERVER]:" + text;
        }
        else
        {
            newMessage.text = clientData.userName + ": " + text;
        }
        GameObject newText = Instantiate(textObject, chatPanel.transform);

        newMessage.textObject = newText.GetComponent<TMP_Text>();
        newMessage.textObject.text = newMessage.text;

        messages.Add(newMessage);
    }
}
