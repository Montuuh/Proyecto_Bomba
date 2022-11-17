using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MultiplayerBoard : MonoBehaviour
{
    public Tilemap tilemap { get; private set; }

    private TileColors tileSprite;

    private Tile tileUnknown;
    private Tile tileEmpty;
    private Tile tileMine;
    private Tile tileExploded;
    private Tile tileFlagged;
    private Tile tileBadFlagged;

    private Tile tileNumber1;
    private Tile tileNumber2;
    private Tile tileNumber3;
    private Tile tileNumber4;
    private Tile tileNumber5;
    private Tile tileNumber6;
    private Tile tileNumber7;
    private Tile tileNumber8;

    private void Awake()
    {
        tilemap = GetComponent<Tilemap>();
    }
    private void Start()
    {
        tileSprite = GetComponent<TileColors>();
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
        switch (cell.color)
        {
            case ColorPlayer.RED:
                tileEmpty = tileSprite.tileEmpty_red;
                tileExploded = tileSprite.tileExploded_red;
                tileFlagged = tileSprite.tileFlagged_red;

                tileNumber1 = tileSprite.tileNumber1_red;
                tileNumber2 = tileSprite.tileNumber2_red;
                tileNumber3 = tileSprite.tileNumber3_red;
                tileNumber4 = tileSprite.tileNumber4_red;
                tileNumber5 = tileSprite.tileNumber5_red;
                tileNumber6 = tileSprite.tileNumber6_red;
                tileNumber7 = tileSprite.tileNumber7_red;
                tileNumber8 = tileSprite.tileNumber8_red;
                break;
            case ColorPlayer.BLUE:
                tileEmpty = tileSprite.tileEmpty_blue;
                tileExploded = tileSprite.tileExploded_blue;
                tileFlagged = tileSprite.tileFlagged_blue;

                tileNumber1 = tileSprite.tileNumber1_blue;
                tileNumber2 = tileSprite.tileNumber2_blue;
                tileNumber3 = tileSprite.tileNumber3_blue;
                tileNumber4 = tileSprite.tileNumber4_blue;
                tileNumber5 = tileSprite.tileNumber5_blue;
                tileNumber6 = tileSprite.tileNumber6_blue;
                tileNumber7 = tileSprite.tileNumber7_blue;
                tileNumber8 = tileSprite.tileNumber8_blue;
                break;
            case ColorPlayer.GREEN:
                tileEmpty = tileSprite.tileEmpty_green;
                tileExploded = tileSprite.tileExploded_green;
                tileFlagged = tileSprite.tileFlagged_green;

                tileNumber1 = tileSprite.tileNumber1_green;
                tileNumber2 = tileSprite.tileNumber2_green;
                tileNumber3 = tileSprite.tileNumber3_green;
                tileNumber4 = tileSprite.tileNumber4_green;
                tileNumber5 = tileSprite.tileNumber5_green;
                tileNumber6 = tileSprite.tileNumber6_green;
                tileNumber7 = tileSprite.tileNumber7_green;
                tileNumber8 = tileSprite.tileNumber8_green;
                break;
            case ColorPlayer.YELLOW:
                tileEmpty = tileSprite.tileEmpty_yellow;
                tileExploded = tileSprite.tileExploded_yellow;
                tileFlagged = tileSprite.tileFlagged_yellow;

                tileNumber1 = tileSprite.tileNumber1_yellow;
                tileNumber2 = tileSprite.tileNumber2_yellow;
                tileNumber3 = tileSprite.tileNumber3_yellow;
                tileNumber4 = tileSprite.tileNumber4_yellow;
                tileNumber5 = tileSprite.tileNumber5_yellow;
                tileNumber6 = tileSprite.tileNumber6_yellow;
                tileNumber7 = tileSprite.tileNumber7_yellow;
                tileNumber8 = tileSprite.tileNumber8_yellow;
                break;
            case ColorPlayer.NONE:
                tileUnknown = tileSprite.tileUnknown_grey;
                tileEmpty = tileSprite.tileEmpty_grey;
                tileMine = tileSprite.tileMine_grey;
                tileExploded = tileSprite.tileExploded_grey;
                tileFlagged = tileSprite.tileFlagged_grey;
                tileBadFlagged = tileSprite.tileBadFlagged_grey;

                tileNumber1 = tileSprite.tileNumber1_grey;
                tileNumber2 = tileSprite.tileNumber2_grey;
                tileNumber3 = tileSprite.tileNumber3_grey;
                tileNumber4 = tileSprite.tileNumber4_grey;
                tileNumber5 = tileSprite.tileNumber5_grey;
                tileNumber6 = tileSprite.tileNumber6_grey;
                tileNumber7 = tileSprite.tileNumber7_grey;
                tileNumber8 = tileSprite.tileNumber8_grey;
                break;
            default:
                break;
        }

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
                return FlagChecking(cell);
            }
            else
            {
                return tileUnknown;
            }
        }
    }
    
    // Debug Key: God Mode
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
                return FlagChecking(cell);
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

    // Debug Key: RevealAllExceptMines
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
                return FlagChecking(cell);
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

    private Tile FlagChecking(Cell cell)
    {
        if (cell.isBadFlagged)
            return tileBadFlagged;
        else
            return tileFlagged;
    }
    #endregion GetTile
}
