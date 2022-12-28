using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileClear : MonoBehaviour
{

    public GameObject normalTile;

    public void SendTileToNarnia(Vector3 position)
    {
        GameObject tmpObj = Instantiate(normalTile);

        tmpObj.transform.SetParent(GameObject.Find("Grid").transform, false);

        //position.z = -0.1f;

        tmpObj.transform.position = position;
    }

}
