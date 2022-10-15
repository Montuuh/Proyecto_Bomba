using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Board : MonoBehaviour
{
    public Tilemap tilemap { get; private set; }

    public Tile tileUnknown;
    public Tile tileEmpty;
    public Tile tileMine;
    public Tile tileExploded;
    public Tile tileFlagged;
    public Tile tileBadFlagged;

    public Tile tileNumber1;
    public Tile tileNumber2;
    public Tile tileNumber3;
    public Tile tileNumber4;
    public Tile tileNumber5;
    public Tile tileNumber6;
    public Tile tileNumber7;
    public Tile tileNumber8;

    private void Awake()
    {
        tilemap = GetComponent<Tilemap>();
    }

    public void Draw(Cell[,] cells, bool isGodMode, bool isRevealedExceptMines)
    {
        tilemap.ClearAllTiles();
        for (int x = 0; x < cells.GetLength(0); x++)
        {
            for (int y = 0; y < cells.GetLength(1); y++)
            {
                var cell = cells[x, y];
                var position = new Vector3Int(x, y, 0);
                var tile = ScriptableObject.CreateInstance<Tile>();
                if (isGodMode)
                    tile = GetTileGodMode(cell);
                else if (isRevealedExceptMines)
                    tile = GetTileRevealAllExceptMines(cell);
                else
                    tile = GetTileNormal(cell);
                tilemap.SetTile(position, tile);
            }
        }

    }

    #region GetTile
    private Tile GetTileNormal(Cell cell)
    {
        if (cell.isExploded)
            return tileExploded;
        
        if (cell.isRevealed)
        {
            switch (cell.cellType)
            {
                case Cell.CellType.Empty:
                    return tileEmpty;
                case Cell.CellType.Mine:
                    return tileMine;
                case Cell.CellType.Number:
                    return GetTileNumber(cell);
                default:
                    return tileEmpty;
            }
        }
        else
        {
            if (cell.isFlagged)
            {
                if (cell.isBadFlagged)
                    return tileBadFlagged;
                else
                    return tileFlagged;
            }
            else
            {
                return tileUnknown;
            }
        }
    }
    
    private Tile GetTileGodMode(Cell cell)
    {
        if (cell.isExploded)
            return tileExploded;
        
        if (cell.isRevealed)
        {
            switch (cell.cellType)
            {
                case Cell.CellType.Empty:
                    return tileEmpty;
                case Cell.CellType.Mine:
                    return tileMine;
                case Cell.CellType.Number:
                    return GetTileNumber(cell);
                default:
                    return tileEmpty;
            }
        }
        else
        {
            if (cell.isFlagged)
            {
                if (cell.isBadFlagged)
                    return tileBadFlagged;
                else
                    return tileFlagged;
            }
            else
            {
                switch (cell.cellType)
                {
                    case Cell.CellType.Empty:
                        return tileEmpty;
                    case Cell.CellType.Mine:
                        return tileMine;
                    case Cell.CellType.Number:
                        return GetTileNumber(cell);
                    default:
                        return tileEmpty;
                }
            }
        }
    }

    private Tile GetTileRevealAllExceptMines(Cell cell)
    {
        if (cell.isExploded)
            return tileExploded;
        
        if (cell.isRevealed)
        {
            switch (cell.cellType)
            {
                case Cell.CellType.Empty:
                    return tileEmpty;
                case Cell.CellType.Mine:
                    return tileUnknown;
                case Cell.CellType.Number:
                    return GetTileNumber(cell);
                default:
                    return tileEmpty;
            }
        }
        else
        {
            if (cell.isFlagged)
            {
                if (cell.isBadFlagged)
                    return tileBadFlagged;
                else
                    return tileFlagged;
            }
            else
            {
                switch (cell.cellType)
                {
                    case Cell.CellType.Empty:
                        return tileEmpty;
                    case Cell.CellType.Mine:
                        return tileUnknown;
                    case Cell.CellType.Number:
                        return GetTileNumber(cell);
                    default:
                        return tileEmpty;
                }
            }
        }
    }

    private Tile GetTileNumber(Cell cell)
    {
        switch (cell.number)
        {
            case 1:
                return tileNumber1;
            case 2:
                return tileNumber2;
            case 3:
                return tileNumber3;
            case 4:
                return tileNumber4;
            case 5:
                return tileNumber5;
            case 6:
                return tileNumber6;
            case 7:
                return tileNumber7;
            case 8:
                return tileNumber8;
            default:
                return tileEmpty;
        }
    }
    #endregion GetTile
}
