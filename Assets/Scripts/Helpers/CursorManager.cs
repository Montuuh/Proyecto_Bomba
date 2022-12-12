using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorManager : MonoBehaviour
{
    public CustomCursor cursorPlayer;
    public CustomCursor cursorPlayer2;
    public CustomCursor cursorPlayer3;
    public CustomCursor cursorPlayer4;

    private void Start()
    {
        cursorPlayer = ScriptableObject.CreateInstance<CustomCursor>();
        cursorPlayer.Init(ColorPlayer.NONE, true);
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

    public void ChangeCursorTexture(int playerNumber, ColorPlayer color)
    {
        switch (playerNumber)
        {
            case 1:
                cursorPlayer = ScriptableObject.CreateInstance<CustomCursor>();
                cursorPlayer.Init(color, true);
                break;
            case 2:
                cursorPlayer2 = ScriptableObject.CreateInstance<CustomCursor>();
                cursorPlayer2.Init(color);
                break;
            case 3:
                cursorPlayer3 = ScriptableObject.CreateInstance<CustomCursor>();
                cursorPlayer3.Init(color);
                break;
            case 4:
                cursorPlayer4 = ScriptableObject.CreateInstance<CustomCursor>();
                cursorPlayer4.Init(color);
                break;
            default:
                Debug.Log("Trying to change cursor texture for player: " + playerNumber);
                break;
        }
    }
}
