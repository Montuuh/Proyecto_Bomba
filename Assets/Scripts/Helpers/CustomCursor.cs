using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomCursor : MonoBehaviour
{
    private Texture2D cursorTexture;
    private CursorMode cursorMode = CursorMode.Auto;
    private Vector2 hotSpot = Vector2.zero;

    void Start()
    {
        SetWhiteCursor();
    }
    
    void SetColorCursor(ColorPlayer color)
    {
        switch (color)
        {
            case ColorPlayer.RED:
                cursorTexture = Resources.Load("Cursors/Arrow_red") as Texture2D;
                break;
            case ColorPlayer.BLUE:
                cursorTexture = Resources.Load("Cursors/Arrow_blue") as Texture2D;
                break;
            case ColorPlayer.GREEN:
                cursorTexture = Resources.Load("Cursors/Arrow_green") as Texture2D;
                break;
            case ColorPlayer.YELLOW:
                cursorTexture = Resources.Load("Cursors/Arrow_yellow") as Texture2D;
                break;
            default:
                break;
        }
        Cursor.SetCursor(cursorTexture, hotSpot, cursorMode);
    }

    void SetWhiteCursor()
    {
        // load the texture
        cursorTexture = Resources.Load<Texture2D>("Cursors/Arrow_white");
        Cursor.SetCursor(cursorTexture, hotSpot, cursorMode);
    }
}
