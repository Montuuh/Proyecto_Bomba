using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class CursorManager : MonoBehaviour
{

    public List<GameObject> cursorList = new List<GameObject>();
    public List<ClientData> currentPlayingPlayers = new List<ClientData>();

    public List<Texture2D> cursorTextureList;

    private float lerpTime = 1.0f; // How long from A to B

    private float lerpTimePassedRed = 0.0f;
    private float lerpTimePassedGreen = 0.0f;
    private float lerpTimePassedBlue = 0.0f;
    private float lerpTimePassedYellow = 0.0f;

    // Init pos to perform lerp
    private Vector2 lerpInitPosRed;
    private Vector2 lerpInitPosGreen;
    private Vector2 lerpInitPosBlue;
    private Vector2 lerpInitPosYellow;

    // Destination pos to perform lerp
    private Vector2 lerpDestinationPosRed;
    private Vector2 lerpDestinationPosGreen;
    private Vector2 lerpDestinationPosBlue;
    private Vector2 lerpDestinationPosYellow;

    private MultiPlayerGame game;

    private Vector2 hotSpot = Vector2.zero;
    private CursorMode cursorMode = CursorMode.Auto;


    private void Update()
    {
        game = GameObject.Find("Grid").GetComponent<MultiPlayerGame>();

        foreach (var cursor in cursorList)
        {
            cursor.SetActive(false);
        }


        // Applies lerp to specific cursor
        foreach (var player in currentPlayingPlayers)
        {
            int index = 0;

            switch (player.colorPlayer)
            {
                case ColorPlayer.NONE:
                    break;
                case ColorPlayer.RED:
                    index = 0;

                    // If Local set new cursor and continue
                    if (player.userID == game.localPlayer.clientData.userID)
                    {
                        Cursor.SetCursor(cursorTextureList[index], hotSpot, cursorMode);
                        continue;
                    }

                    // Else perform lerp
                    cursorList[index].SetActive(true);
                    
                    lerpTimePassedRed += Time.deltaTime / lerpTime;
                    cursorList[index].transform.position = Vector2.LerpUnclamped(lerpInitPosRed, lerpDestinationPosRed, EaseOut(lerpTimePassedRed));
                    break;

                case ColorPlayer.GREEN:
                    index = 1;

                    if (player.userID == game.localPlayer.clientData.userID)
                    {
                        Cursor.SetCursor(cursorTextureList[index], hotSpot, cursorMode);
                        continue;
                    }

                    cursorList[index].SetActive(true);
                    
                    lerpTimePassedGreen += Time.deltaTime / lerpTime;
                    cursorList[index].transform.position = Vector2.LerpUnclamped(lerpInitPosGreen, lerpDestinationPosGreen, EaseOut(lerpTimePassedGreen));
                    break;

                case ColorPlayer.BLUE:
                    index = 2;

                    if (player.userID == game.localPlayer.clientData.userID)
                    {
                        Cursor.SetCursor(cursorTextureList[index], hotSpot, cursorMode);
                        continue;
                    }

                    cursorList[index].SetActive(true);
                    
                    lerpTimePassedBlue += Time.deltaTime  / lerpTime;
                    cursorList[index].transform.position = Vector2.LerpUnclamped(lerpInitPosBlue, lerpDestinationPosBlue, EaseOut(lerpTimePassedBlue));
                    break;

                case ColorPlayer.YELLOW:
                    index = 3;

                    if (player.userID == game.localPlayer.clientData.userID)
                    {
                        Cursor.SetCursor(cursorTextureList[index], hotSpot, cursorMode);
                        continue;
                    }

                    cursorList[index].SetActive(true);
                    
                    lerpTimePassedYellow += Time.deltaTime  / lerpTime;
                    cursorList[index].transform.position = Vector2.LerpUnclamped(lerpInitPosYellow, lerpDestinationPosYellow, EaseOut(lerpTimePassedYellow));
                    break;

                default:
                    break;
            }
        }
    }
    public void UpdateCursor(MouseData mouseData)
    {

        Vector2 destinationPos = new Vector2(mouseData.x, mouseData.y);

        // Redirect specific lerp and restart it if destination is different
        switch (mouseData.clientData.colorPlayer)
        {
            case ColorPlayer.NONE:
                break;
            case ColorPlayer.RED:
                    lerpInitPosRed = cursorList[0].transform.position;
                    lerpDestinationPosRed = destinationPos;
                    lerpTimePassedRed = 0.0f;
                break;
            case ColorPlayer.GREEN:
                    lerpInitPosGreen = cursorList[1].transform.position;
                    lerpDestinationPosGreen = destinationPos;
                    lerpTimePassedGreen = 0.0f;
                break;
            case ColorPlayer.BLUE:
                    lerpInitPosBlue = cursorList[2].transform.position;
                    lerpDestinationPosBlue = destinationPos;
                    lerpTimePassedBlue = 0.0f;
                break;
            case ColorPlayer.YELLOW:
                    lerpInitPosYellow = cursorList[3].transform.position;
                    lerpDestinationPosYellow = destinationPos;
                    lerpTimePassedYellow = 0.0f;
                break;
            default:
                break;
        }
    }

    #region HELPERS
    public static float EaseIn(float t)
    {
        return t * t;
    }

    static float Flip(float x)
    {
        return 1 - x;
    }

    public static float EaseOut(float t)
    {
        return Flip(Flip(t) * Flip(t) * Flip(t));
    }
    #endregion

}
