using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileClear : MonoBehaviour
{

    public GameObject normalTile;

    public void SendTileToNarnia(Vector3 position, ColorPlayer color)
    {
        GameObject tmpObj = Instantiate(normalTile);

        tmpObj.transform.SetParent(GameObject.Find("Grid").transform, false);

        tmpObj.GetComponent<TileClearBehaviour>().color = color;

        tmpObj.transform.position = position;
    }

}
