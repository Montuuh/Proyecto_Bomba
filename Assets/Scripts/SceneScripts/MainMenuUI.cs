using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
    // Awake is called before Start
    private void Awake()
    {
        // Set cursor to visible
        Cursor.visible = true;
    }

    public void SinglePlayerButton()
    {
        // Load SinglePlayerLobby scene
        SceneManager.LoadScene(SceneManager.Scene.SinglePlayerLobby);
    }

    public void OnlineButton()
    {
        // Load OnlineLobby scene
        SceneManager.LoadScene(SceneManager.Scene.OnlineLobby);
    }

    public void QuitButton()
    {
        // Quit application
        Application.Quit();
        // editor quit
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
