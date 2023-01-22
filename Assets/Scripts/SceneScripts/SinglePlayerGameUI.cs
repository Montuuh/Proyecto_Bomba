using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SinglePlayerGameUI : MonoBehaviour
{
    public GameObject mainMenuGo;
    public GameObject YouLost;

    void Awake()
    {
        if (mainMenuGo == null)
        {
            mainMenuGo = GameObject.Find("MainMenu_button");
        }
    }

    public void MainMenuButton()
    {
        // destroy client gameobject
        Destroy(GameObject.Find("ClientManager"));
        // destroy server gameobject
        Destroy(GameObject.Find("ServerManager"));
        SceneManager.LoadScene(SceneManager.Scene.MainMenu);
    }
}
