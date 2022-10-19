using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SceneManager
{
    // Variable holder
    public static Game.Difficulty difficulty;
    public static MultiPlayerGame.Difficulty multiDifficulty;

    // Enum with all scenes
    // Has to be named exactly like the scene in Unity
    public enum Scene
    {
        MainMenu,
        OnlineLobby,
        SinglePlayerLobby,
        SinglePlayerGame
    }

    // Load scene by name and by enum
    // To Do: Unload current scene. I don't know if Unity does it by default
    // To Do: Loading scene
    public static void LoadScene(string sceneName)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }
    public static void LoadScene(Scene scene)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(scene.ToString());
    }
}
