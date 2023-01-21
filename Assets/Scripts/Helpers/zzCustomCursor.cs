using UnityEngine;

public class CustomCursor : MonoBehaviour
{
    private Texture2D cursorTexture { get; set; }
    public ColorPlayer color { get; set; }
    private Vector2 position { get; set; }
    private bool isPlayer { get; set; }
    private bool isActive { get; set; }

    public void Init(ColorPlayer colorPlayer, bool isPlayer = false)
    {
        this.isPlayer = isPlayer;
        this.isActive = true;
        this.color = colorPlayer;
        cursorTexture = Resources.Load<Texture2D>(GetCursorTextureName(this.color));
        if (isPlayer)
        {
            Cursor.SetCursor(cursorTexture, new Vector2(0, 0), CursorMode.Auto);
        }
        else
        {
            this.position = new Vector2(30, 30);
        }
    }

    private void Update()
    {
        if (!isActive)
        {
            if (cursorTexture != null)
                cursorTexture = null;
            return;
        }
    }

    public void UpdatePosition(int x, int y)
    {
        if (isPlayer)
        {
            //Cursor.SetCursor(cursorTexture, new Vector2(x, y), CursorMode.Auto);
        }
        else
        {
            this.position = new Vector2(x, y);
        }
    }

    public void UpdateTexture(ColorPlayer colorPlayer)
    {
        this.color = colorPlayer;
        cursorTexture = Resources.Load<Texture2D>(GetCursorTextureName(this.color));
        if (isPlayer)
            Cursor.SetCursor(cursorTexture, new Vector2(0, 0), CursorMode.Auto);
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
