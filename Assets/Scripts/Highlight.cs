using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Tilemaps;
public class Highlight : MonoBehaviour
{
    public Tilemap tilemap { get; private set; }
    public Tile highlight;
    private Vector3Int lastPosition = new Vector3Int(0,0,0);
    private void Awake()
    {
        tilemap = GetComponent<Tilemap>();
    }

    public void Draw(int x, int y)
    {
        var position = new Vector3Int(x, y, 0);

        if (position != lastPosition)
        {
            tilemap.ClearAllTiles();
            tilemap.SetTile(position, highlight);
        }
    }

}
