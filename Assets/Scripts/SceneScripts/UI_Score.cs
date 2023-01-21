using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UI_Score : MonoBehaviour
{
    
    private TMP_Text scoreObject;
    private string scoreText;
    
    private ScoreManager scoreManager;

    void Start()
    {
        scoreObject = gameObject.GetComponent<TMP_Text>();
    }

    // Displays current scores of players
    void Update()
    {
        scoreManager = GameObject.Find("ClientManager").GetComponent<ScoreManager>();

        List<ClientData> scoreUI = scoreManager.currentPlayingPlayers;

        scoreUI.Sort(SortByScore);

        scoreText = "";

        foreach (var player in scoreManager.currentPlayingPlayers)
        {
            if(player.userID == scoreManager.localPlayer.userID)
                scoreText += (player.userName + ": " + player.score + "\n").ToUpper();
            else
                scoreText += (player.userName + ": " + player.score + "\n").ToLower();
        }

        scoreObject.text = scoreText;
    }

    private int SortByScore(ClientData c1, ClientData c2)
    {
        return c2.score.CompareTo(c1.score);
    }

}
