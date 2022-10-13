using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SinglePlayerGameUI : MonoBehaviour
{
    public GameObject mainMenuGo;
    
    void Awake()
    {
        if (mainMenuGo == null)
        {
            mainMenuGo = GameObject.Find("MainMenu_button");
        }
    }

    public void MainMenuButton()
    {
        SceneManager.LoadScene(SceneManager.Scene.MainMenu);
    }
}
