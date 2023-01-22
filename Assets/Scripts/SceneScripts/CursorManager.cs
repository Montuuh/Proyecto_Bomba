using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class CursorManager : MonoBehaviour
{

    public List<GameObject> cursorList = new List<GameObject>();
    public List<ClientData> currentPlayingPlayers = new List<ClientData>();

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

    private void Update()
    {
        // Applies lerp to specific cursor
        foreach (var player in currentPlayingPlayers)
        {
            switch (player.colorPlayer)
            {
                case ColorPlayer.NONE:
                    break;
                case ColorPlayer.RED:
                    if (lerpTimePassedRed < lerpTime)
                    {
                        lerpTimePassedRed += Time.deltaTime / lerpTime;
                        cursorList[0].transform.position = Vector2.LerpUnclamped(lerpInitPosRed, lerpDestinationPosRed, EaseOut(lerpTimePassedRed));
                    }
                    break;

                case ColorPlayer.GREEN:
                    if (lerpTimePassedGreen < lerpTime)
                    {
                        lerpTimePassedGreen += Time.deltaTime / lerpTime;
                        cursorList[1].transform.position = Vector2.LerpUnclamped(lerpInitPosGreen, lerpDestinationPosGreen, EaseOut(lerpTimePassedGreen));
                    }
                    break;

                case ColorPlayer.BLUE:
                    if (lerpTimePassedBlue < lerpTime)
                    {
                        lerpTimePassedBlue += Time.deltaTime  / lerpTime;
                        cursorList[2].transform.position = Vector2.LerpUnclamped(lerpInitPosBlue, lerpDestinationPosBlue, EaseOut(lerpTimePassedBlue));
                    }
                    break;

                case ColorPlayer.YELLOW:
                    if (lerpTimePassedYellow < lerpTime)
                    {
                        lerpTimePassedYellow += Time.deltaTime  / lerpTime;
                        cursorList[3].transform.position = Vector2.LerpUnclamped(lerpInitPosYellow, lerpDestinationPosYellow, EaseOut(lerpTimePassedYellow));
                    }
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
                if (Mathf.FloorToInt(destinationPos.x) != Mathf.FloorToInt(lerpDestinationPosRed.x) || Mathf.FloorToInt(destinationPos.y) != Mathf.FloorToInt(lerpDestinationPosRed.y))
                {
                    lerpInitPosRed = cursorList[0].transform.position;
                    lerpDestinationPosRed = destinationPos;
                    lerpTimePassedRed = 0.0f;
                }
                break;
            case ColorPlayer.GREEN:
                if (Mathf.FloorToInt(destinationPos.x) != Mathf.FloorToInt(lerpDestinationPosGreen.x) || Mathf.FloorToInt(destinationPos.y) != Mathf.FloorToInt(lerpDestinationPosGreen.y))
                {
                    lerpInitPosGreen = cursorList[1].transform.position;
                    lerpDestinationPosGreen = destinationPos;
                    lerpTimePassedGreen = 0.0f;
                }
                break;
            case ColorPlayer.BLUE:
                if (Mathf.FloorToInt(destinationPos.x) != Mathf.FloorToInt(lerpDestinationPosBlue.x) || Mathf.FloorToInt(destinationPos.y) != Mathf.FloorToInt(lerpDestinationPosBlue.y))
                {
                    lerpInitPosBlue = cursorList[2].transform.position;
                    lerpDestinationPosBlue = destinationPos;
                    lerpTimePassedBlue = 0.0f;
                }
                break;
            case ColorPlayer.YELLOW:
                if (Mathf.FloorToInt(destinationPos.x) != Mathf.FloorToInt(lerpDestinationPosYellow.x) || Mathf.FloorToInt(destinationPos.y) != Mathf.FloorToInt(lerpDestinationPosYellow.y))
                {
                    lerpInitPosYellow = cursorList[3].transform.position;
                    lerpDestinationPosYellow = destinationPos;
                    lerpTimePassedYellow = 0.0f;
                }
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
