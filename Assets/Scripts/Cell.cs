using UnityEngine;

public struct Cell
{
    public enum CellType
    {
        Empty,
        Number,
        Mine
    }
    
    public Vector3Int position;
    public CellType cellType;
    public bool isRevealed;
    public bool isFlagged;
    public bool isExploded;
    public int number;
}
