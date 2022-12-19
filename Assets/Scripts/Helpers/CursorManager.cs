using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorManager : MonoBehaviour
{
    private CustomCursor cursorPlayer;
    private CustomCursor cursorPlayer2;
    private CustomCursor cursorPlayer3;
    private CustomCursor cursorPlayer4;

    private void Start()
    {
        // Find game object in children
        cursorPlayer = GameObject.Find("CursorPlayer1").GetComponent<CustomCursor>();
        cursorPlayer2 = GameObject.Find("CursorPlayer2").GetComponent<CustomCursor>();
        cursorPlayer3 = GameObject.Find("CursorPlayer3").GetComponent<CustomCursor>();
        cursorPlayer4 = GameObject.Find("CursorPlayer4").GetComponent<CustomCursor>();

        // Init cursors
        cursorPlayer.Init(ColorPlayer.RED, true);
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
        switch (playerNumber)
        {
            case 1:
                if (cursorPlayer.color != color)
                    cursorPlayer.UpdateTexture(color);
                cursorPlayer.UpdatePosition(x, y);
                break;
            case 2:
                if (cursorPlayer2.color != color)
                    cursorPlayer2.UpdateTexture(color);
                cursorPlayer2.UpdatePosition(x, y);
                break;
            case 3:
                if (cursorPlayer3.color != color)
                    cursorPlayer3.UpdateTexture(color);
                cursorPlayer3.UpdatePosition(x, y);
                break;
            case 4:
                if (cursorPlayer4.color != color)
                    cursorPlayer4.UpdateTexture(color);
                cursorPlayer4.UpdatePosition(x, y);
                break;
            default:
                break;
        }
    }

    public void ChangeCursorTexture(int playerNumber, ColorPlayer color)
    {
        Debug.Log("ChangeCursorTexture: player=" + playerNumber + " color=" + color);
        switch (playerNumber)
        {
            case 1:
                cursorPlayer.Init(color, true);
                break;
            case 2:
                cursorPlayer2.Init(color);
                break;
            case 3:
                cursorPlayer3.Init(color);
                break;
            case 4:
                cursorPlayer4.Init(color);
                break;
            default:
                Debug.Log("Trying to change cursor texture for player: " + playerNumber);
                break;
        }
    }
}
