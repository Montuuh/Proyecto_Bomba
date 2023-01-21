using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class CursorManager : MonoBehaviour
{

    public List<GameObject> cursorList = new List<GameObject>();
    public List<ClientData> currentPlayingPlayers = new List<ClientData>();

    private float lerpTime = 1.0f;

    private float lerpTimePassedRed = 0.0f;
    private float lerpTimePassedGreen = 0.0f;
    private float lerpTimePassedBlue = 0.0f;
    private float lerpTimePassedYellow = 0.0f;

    private Vector2 lerpInitPosRed;
    private Vector2 lerpInitPosGreen;
    private Vector2 lerpInitPosBlue;
    private Vector2 lerpInitPosYellow;

    private Vector2 lerpDestinationPosRed;
    private Vector2 lerpDestinationPosGreen;
    private Vector2 lerpDestinationPosBlue;
    private Vector2 lerpDestinationPosYellow;

    //private void Start()
    //{
    //    for (int i = 0; i < cursorList.Count; i++)
    //    {
    //        cursorList[i] = GameObject.Instantiate(cursorList[i]);
    //        cursorList[i].transform.SetParent(GameObject.Find("CursorManager").transform, false);

    //        Vector2 spawnPos = Camera.main.ViewportToWorldPoint(new Vector2(0.5f, 0.5f));

    //        cursorList[i].transform.position = spawnPos;
    //    }
    //}

    private void Update()
    {
        foreach (var player in currentPlayingPlayers)
        {
            switch (player.colorPlayer)
            {
                case ColorPlayer.NONE:
                    break;
                case ColorPlayer.RED:

                    lerpTimePassedRed += Time.deltaTime;
                    cursorList[0].transform.position = Vector2.Lerp(lerpInitPosRed, lerpDestinationPosRed, lerpTimePassedRed);

                    break;

                case ColorPlayer.GREEN:

                    lerpTimePassedGreen += Time.deltaTime;
                    cursorList[1].transform.position = Vector2.Lerp(lerpInitPosGreen, lerpDestinationPosGreen, lerpTimePassedGreen);

                    break;

                case ColorPlayer.BLUE:

                    lerpTimePassedBlue += Time.deltaTime;
                    cursorList[2].transform.position = Vector2.Lerp(lerpInitPosBlue, lerpDestinationPosBlue, lerpTimePassedBlue);

                    break;

                case ColorPlayer.YELLOW:

                    lerpTimePassedYellow += Time.deltaTime;
                    cursorList[3].transform.position = Vector2.Lerp(lerpInitPosYellow, lerpDestinationPosYellow, lerpTimePassedYellow);

                    break;

                default:
                    break;
            }
        }
    }

    public void UpdateCursor(MouseData mouseData)
    {
        Vector2 destinationPos = new Vector2(mouseData.x, mouseData.y);

        switch (mouseData.clientData.colorPlayer)
        {
            case ColorPlayer.NONE:
                break;
            case ColorPlayer.RED:
                if (destinationPos.x != lerpDestinationPosRed.x || destinationPos.y != lerpDestinationPosRed.y)
                {
                    lerpInitPosRed = cursorList[0].transform.position;
                    lerpDestinationPosRed = destinationPos;
                    lerpTimePassedRed = 0.0f;
                }
                break;
            case ColorPlayer.GREEN:
                if (destinationPos.x != lerpDestinationPosGreen.x || destinationPos.y != lerpDestinationPosGreen.y)
                {
                    lerpInitPosGreen = cursorList[1].transform.position;
                    lerpDestinationPosGreen = destinationPos;
                    lerpTimePassedGreen = 0.0f;
                }
                break;
            case ColorPlayer.BLUE:
                if (destinationPos.x != lerpDestinationPosBlue.x || destinationPos.y != lerpDestinationPosBlue.y)
                {
                    lerpInitPosBlue = cursorList[2].transform.position;
                    lerpDestinationPosBlue = destinationPos;
                    lerpTimePassedBlue = 0.0f;
                }
                break;
            case ColorPlayer.YELLOW:
                if (destinationPos.x != lerpDestinationPosYellow.x || destinationPos.y != lerpDestinationPosYellow.y)
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

}
