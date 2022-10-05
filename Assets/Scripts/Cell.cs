using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Cell
{
    public enum CellType
    {
        Unknown,
        Empty,
        Mine,
        Number
    }
    public Vector3Int position;
    public CellType cellType;
    public bool isRevealed;
    public bool isMine;
    public bool isFlagged;
    public bool exploded;
    public int adjacentMines;
}
