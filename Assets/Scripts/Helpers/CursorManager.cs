using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorManager : MonoBehaviour
{

    public List<GameObject> cursorList = new List<GameObject>();
    public List<ClientData> currentPlayingPlayers = new List<ClientData>();


    private void Start()
    {
   

        // Init cursors
        
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F6))
        {
            ChangeCursorTexture(1, ColorPlayer.RED);
        }
        else if (Input.GetKeyDown(KeyCode.F7))
        {
            ChangeCursorTexture(2, ColorPlayer.BLUE);
        }
        else if (Input.GetKeyDown(KeyCode.F8))
        {
            ChangeCursorTexture(3, ColorPlayer.GREEN);
        }
        else if (Input.GetKeyDown(KeyCode.F9))
        {
            ChangeCursorTexture(4, ColorPlayer.YELLOW);
        }
    }

    public void UpdateCursor(int x, int y, ColorPlayer color, int playerNumber)
    {
        
    }

    public void ChangeCursorTexture(int playerNumber, ColorPlayer color)
    {

    }
}
