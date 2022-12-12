using UnityEngine;

public class CustomCursor : ScriptableObject
{
    private Texture2D cursorTexture { get; set; }
    private Vector2 position { get; set; }

    public void Init(ColorPlayer colorPlayer, bool isPlayer = false)
    {
        cursorTexture = Resources.Load<Texture2D>(GetCursorTextureName(colorPlayer));
        if (isPlayer)
        {
            Cursor.SetCursor(cursorTexture, new Vector2(0, 0), CursorMode.Auto);
        }
        else
        {
            this.position = new Vector2(30, 30);
        }
    }

    private string GetCursorTextureName(ColorPlayer colorPlayer)
    {
        string cursorTextureName = "Cursors/";
        switch (colorPlayer)
        {
            case ColorPlayer.NONE:
                return cursorTextureName += "Arrow_white";
            case ColorPlayer.RED:
                return cursorTextureName += "Arrow_red";
            case ColorPlayer.BLUE:
                return cursorTextureName += "Arrow_blue";
            case ColorPlayer.GREEN:
                return cursorTextureName += "Arrow_green";
            case ColorPlayer.YELLOW:
                return cursorTextureName += "Arrow_yellow";
            default:
                Debug.Log("Trying to load texure: " + cursorTextureName + "NONE");
                return "NONE";
        }
    }
}
