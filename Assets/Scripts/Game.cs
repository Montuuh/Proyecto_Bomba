using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    private Board board;
    private Cell[,] cells;

    [Range(2, 32)]
    public int width = 16;
    [Range(2, 32)]
    public int height = 16;
    [Range(1, 32)]
    public int numMines = 16;

    private void Start()
    {
        StartGame();
    }

    private void StartGame()
    {
        Debug.Log("Start Game");
        
        cells = new Cell[width, height];
        
        GenerateEmptyBoard();

        board = GetComponentInChildren<Board>();
        board.Draw(cells);
    }

    private void GenerateEmptyBoard()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Cell cell = new Cell();
                cell.position = new Vector3Int(x, y, 0);
                cell.cellType = Cell.CellType.Unknown;
                cells[x, y] = cell;
            }
        }
    }
}
