using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthManager : MonoBehaviour
{
    
    public List<ClientData> currentPlayingPlayers = new List<ClientData>();
    public ClientData localPlayer;

    // Updates health of specific player
    public void UpdateLifes(ClientData clientData)
    {
        foreach (var player in currentPlayingPlayers)
        {
            if (player.userID == clientData.userID)
                player.health = clientData.health;
        }
    }
}
