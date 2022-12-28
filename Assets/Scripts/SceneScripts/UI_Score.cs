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
        scoreManager = GameObject.Find("ClientManager").GetComponent<ScoreManager>();

        scoreObject = gameObject.GetComponent<TMP_Text>();
    }


    // Update is called once per frame
    void Update()
    {
        scoreText = "";

        foreach (var player in scoreManager.currentPlayingPlayers)
        {
            scoreText += player.userName + ": " + player.score + "\n";
        }

        scoreObject.text = scoreText;
    }
}