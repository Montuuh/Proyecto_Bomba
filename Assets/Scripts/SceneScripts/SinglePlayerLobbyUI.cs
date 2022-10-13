using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SinglePlayerLobbyUI : MonoBehaviour
{
    // Awake is called before Start
    private void Awake()
    {
        // Set cursor to visible
        Cursor.visible = true;
    }

    public void BeginnerButton()
    {
        // Set difficulty to easy
        SceneManager.difficulty = Game.Difficulty.Beginner;
        Play();
    }

    public void IntermediateButton()
    {
        // Set difficulty to medium
        SceneManager.difficulty = Game.Difficulty.Intermediate;
        Play();
    }

    public void ExtremeButton()
    {
        // Set difficulty to hard
        SceneManager.difficulty = Game.Difficulty.Extreme;
        Play();
    }

    public void LegendButton()
    {
        // Set difficulty to hard
        SceneManager.difficulty = Game.Difficulty.Legend;
        Play();
    }

    public void Play()
    {
        switch (SceneManager.difficulty)
        {
            case Game.Difficulty.Beginner:
                Debug.Log("Beginner");
                SceneManager.LoadScene(SceneManager.Scene.SinglePlayerGame);
                break;
            case Game.Difficulty.Intermediate:
                Debug.Log("Intermediate");
                SceneManager.LoadScene(SceneManager.Scene.SinglePlayerGame);
                break;
            case Game.Difficulty.Extreme:
                Debug.Log("Extreme");
                SceneManager.LoadScene(SceneManager.Scene.SinglePlayerGame);
                break;
            case Game.Difficulty.Legend:
                Debug.Log("Legend");
                SceneManager.LoadScene(SceneManager.Scene.SinglePlayerGame);
                break;
        }
    }

    public void MainMenuButton()
    {
        // Load MainMenu scene
        SceneManager.LoadScene(SceneManager.Scene.MainMenu);
    }
}
