using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Health : MonoBehaviour
{

    public List<GameObject> hearts;

    private MultiPlayerGame game;

    void Update()
    {
        game = GameObject.Find("Grid").GetComponent<MultiPlayerGame>();

        for (int i = 0; i < hearts.Count; i++)
        {
            hearts[i].SetActive(false);
        }

        for (int i = 0; i < game.health; i++)
        {
            hearts[i].SetActive(true);
        }
    }
}
