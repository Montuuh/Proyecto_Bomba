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

    public int width, height = 0;

    private void Awake()
    {
        tilemap = GetComponent<Tilemap>();
    }

    public void Draw(int x, int y)
    {
        var position = new Vector3Int(x, y, 0);

        if (position != lastPosition)
        {
            lastPosition = position;
            tilemap.ClearAllTiles();
            tilemap.SetTile(position, highlight);
        }
    }

    public void Clear()
    {
        tilemap.ClearAllTiles();
    }

    private void Update()
    {
        // Get mouse position and convert it to world position (2D)
        Vector3 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        int x = Mathf.FloorToInt(position.x);
        int y = Mathf.FloorToInt(position.y);

        // Bounds check
        if (x >= 0 && x < width && y >= 0 && y < height)
        {
            Draw(x, y);
        }
        else
        {
            Draw(-100, -100);
        }
    }

}
