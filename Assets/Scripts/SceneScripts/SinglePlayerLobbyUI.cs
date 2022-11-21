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
        Play(DifficultyNew.Beginner);
    }

    public void IntermediateButton()
    {
        // Set difficulty to medium
        Play(DifficultyNew.Intermediate);
    }

    public void ExtremeButton()
    {
        // Set difficulty to hard
        Play(DifficultyNew.Extreme);
    }

    public void LegendButton()
    {
        // Set difficulty to hard
        Play(DifficultyNew.Legend);
    }

    public void Play(DifficultyNew difficulty)
    {
        switch (difficulty)
        {
            case DifficultyNew.Beginner:
                Debug.Log("Beginner");
                SceneManager.LoadScene(SceneManager.Scene.SinglePlayerGame);
                break;
            case DifficultyNew.Intermediate:
                Debug.Log("Intermediate");
                SceneManager.LoadScene(SceneManager.Scene.SinglePlayerGame);
                break;
            case DifficultyNew.Extreme:
                Debug.Log("Extreme");
                SceneManager.LoadScene(SceneManager.Scene.SinglePlayerGame);
                break;
            case DifficultyNew.Legend:
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
