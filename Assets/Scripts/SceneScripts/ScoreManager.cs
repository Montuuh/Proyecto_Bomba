using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public List<ClientData> currentPlayingPlayers = new List<ClientData>();
    

    public void UpdateScores(ClientData clientData)
    {
        foreach (var player in currentPlayingPlayers)
        {
            if (player.userID == clientData.userID)
                player.score = clientData.score;
        }
    }
}
