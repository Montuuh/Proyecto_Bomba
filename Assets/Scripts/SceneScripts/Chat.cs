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
    public int maxMessages = 25;
    public GameObject chatPanel, textObject;
 
    private void Update()
    {
        //if (!gameObject.activeSelf)
        //{
        //    pendingMessages.Clear();
        //}

        //if (pendingMessages.Count > 0)
        //{
        //    SendMessageToChat(clientDatapendingMessages[0].text);
        //    pendingMessages.Remove(pendingMessages[0]);
        //}
    }

    public void SendMessageToChat(Client client, string text)
    {
        if (messages.Count >= maxMessages)
        {
            Destroy(messages[0].textObject.gameObject);
            messages.Remove(messages[0]);
        }

        Message newMessage = new Message();

        text = client.clientData.userName + ": " + text;

        newMessage.text = text;
        GameObject newText = Instantiate(textObject, chatPanel.transform);

        newMessage.textObject = newText.GetComponent<TMP_Text>();
        newMessage.textObject.text = newMessage.text;

        messages.Add(newMessage);

        client.SendClientString(client.clientData, text);

    }
}
