using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class Game : MonoBehaviour
{
    #region Variables
    public enum Difficulty
    {
        Begginer, // Begginer = 9x9, 10 mines, 0.12% chance of mine
        Intermediate, // Intermediate = 16x16, 40 mines, 0.16% chance of mine
        Extreme, // Expert = 20x20, 85 mines, 0.21% chance of mine
        Legend // Legend = 25x25, 160 mines, 0.25% chance of mine
    }
    public Difficulty difficulty = Difficulty.Begginer;
    private int width;
    private int height;
    private int mines;

    private bool isGodMode = false;
    private bool isRevealedExceptMines = false;
    private bool letIngameInput = true;

    private Board board;
    private Cell[,] cells;
    #endregion Variables

    #region Unity Methods
    private void Awake()
    {
        cells = new Cell[width, height];
        board = GetComponentInChildren<Board>();
    }

    private void Start()
    {
        // ToDo: Add some kind of menu to select difficulty
        // Loads the game at the start of the engine project.
        CreateGame();
    }

    private void Update()
    {
        DebugKeys();

        // Besides this line, if the flag is false, won't run the code
        if (!letIngameInput) return;

        IngameInput();
    }
    #endregion Unity Methods

    #region Input Logic
    private void IngameInput()
    {
        // Left click
        if (Input.GetMouseButtonDown(0))
        {
            Reveal();
        }
        // Right click
        if (Input.GetMouseButtonDown(1))
        {
            Flag();
        }
    }

    // Some debug keys
    private void DebugKeys()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            CreateGame();
        }
        if (Input.GetKeyDown(KeyCode.F1))
        {
            isGodMode = !isGodMode;
            isRevealedExceptMines = false;
            ReloadBoard();
        }
        if (Input.GetKeyDown(KeyCode.F2))
        {
            isRevealedExceptMines = !isRevealedExceptMines;
            isGodMode = false;
            ReloadBoard();
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            QuitGame();
        }
    }

    // On right click flag the cell
    private void Flag()
    {
        var position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var x = Mathf.FloorToInt(position.x);
        var y = Mathf.FloorToInt(position.y);
        if (x >= 0 && x < width && y >= 0 && y < height)
        {
            var cell = cells[x, y];
            if (!cell.isRevealed)
            {
                if (CheckFlagsAvailable())
                {
                    cell.isFlagged = !cell.isFlagged;
                }
                else
                {
                    if (cell.isFlagged)
                    {
                        cell.isFlagged = false;
                    }
                }
                cells[x, y] = cell;
                if (CheckWin())
                {
                    Win();
                }
                ReloadBoard();
            }
        }
    }

    // Counts the total of flags used. Returns false if there are no flags available
    private bool CheckFlagsAvailable()
    {
        var flagsUsed = 0;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var cell = cells[x, y];
                if (cell.isFlagged)
                {
                    flagsUsed++;
                }
            }
        }
        return flagsUsed < mines ? true : false;
    }

    // On left click reveal the cell
    private void Reveal()
    {
        var position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var x = Mathf.FloorToInt(position.x);
        var y = Mathf.FloorToInt(position.y);
        if (x >= 0 && x < width && y >= 0 && y < height)
        {
            var cell = cells[x, y];
            if (cell.isFlagged) return;
            
            if (cell.isRevealed)
            {
                RevealAdjacentAvailableCells(cell);
            }
            
            if (cell.cellType == Cell.CellType.Empty)
            {
                RevealEmptyCells(cell);
            }
            else if (cell.cellType == Cell.CellType.Mine)
            {
                cell.isExploded = true;
                cells[x, y] = cell;
                GameOver();
            }
            else
            {
                cell.isRevealed = true;
                cells[x, y] = cell;
                if (CheckWin())
                {
                    Win();
                }
            }
            ReloadBoard();
        }
    }

    // Recursive function to reveal all empty cells that are close
    private void RevealEmptyCells(Cell cell)
    {
        if (cell.isRevealed) return;
        if (cell.cellType == Cell.CellType.Mine) return;

        cell.isRevealed = true;
        cells[cell.position.x, cell.position.y] = cell;

        if (cell.cellType == Cell.CellType.Empty)
        {
            var nearCells = GetNearCells(cell);
            foreach (var nearCell in nearCells)
            {
                RevealEmptyCells(nearCell);
            }
        }
    }

    // This functions count the mines around the cell and returns the number
    private List<Cell> GetNearCells(Cell cell)
    {
        var nearCells = new List<Cell>();
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0) continue;
                var nearX = cell.position.x + x;
                var nearY = cell.position.y + y;
                if (nearX >= 0 && nearX < width && nearY >= 0 && nearY < height)
                {
                    nearCells.Add(cells[nearX, nearY]);
                }
            }
        }
        return nearCells;
    }

    // This function reveal all the surrounding cells if the cell has the same number of flags as mines around it
    private void RevealAdjacentAvailableCells(Cell cell)
    {
        List<Cell> nearCells = GetNearCells(cell);

        // Gets bomb and flags count of the surrounding cells
        int bombCount = 0;
        int bombsFlagged = 0;
        for (int i = 0; i < nearCells.Count; i++)
        {
            Cell cellI = nearCells[i];
            cellI.isRevealed = true;
            if (cellI.cellType == Cell.CellType.Mine)
            {
                bombCount++;
                if (cellI.isFlagged)
                {
                    bombsFlagged++;
                }
            }
        }

        // If the bomb count is equal to the number of flags around the cell, reveal the rest of the cells
        if (bombCount == bombsFlagged)
        {
            for (int i = 0; i < nearCells.Count; i++)
            {
                Cell cellI = nearCells[i];
                switch(cellI.cellType)
                {
                    case Cell.CellType.Empty:
                        RevealEmptyCells(cellI);
                        break;
                    case Cell.CellType.Mine:
                        //cellI.isExploded = true;
                        //cells[cellI.position.x, cellI.position.y] = cellI;
                        //GameOver();
                        break;
                    default:
                        cellI.isRevealed = true;
                        cells[cellI.position.x, cellI.position.y] = cellI;
                        break;
                }
            }
        }
    }
    #endregion Input Logic

    #region Game Generator
    private void CreateGame()
    {
        SetBoardSize();
        SetCamera();

        letIngameInput = true;
        cells = new Cell[width, height];

        // ToDo:
        // First empty map, and when the player clicks the first cell, do generate mines and numbers, but with a condition
        // Mines and numbers are generated with an offset from the clicked cell. For example 1 cell away from the clicked cell and some random cells around it.
        // This way the player will never click on a mine on the first click.

        SetupCells();
        CreateMines();
        SetupNumbers();

        ReloadBoard();
    }

    private void SetBoardSize()
    {
        switch(difficulty)
        {
            case Difficulty.Begginer:
                width = 9;
                height = 9;
                mines = 10;
                break;
            case Difficulty.Intermediate:
                width = 16;
                height = 16;
                mines = 40;
                break;
            case Difficulty.Extreme:
                width = 20;
                height = 20;
                mines = 85;
                break;
            case Difficulty.Legend:
                width = 25;
                height = 25;
                mines = 160;
                break;
            default:
                width = 2;
                height = 2;
                mines = 1;
                break;
        }
    }
    
    private void SetupCells()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                cells[x, y] = new Cell
                {
                    position = new Vector3Int(x, y, 0),
                    cellType = Cell.CellType.Empty,
                    isRevealed = false,
                    isFlagged = false,
                    isExploded = false,
                    number = 0
                };
            }
        }
    }

    // Mine generation
    private void CreateMines()
    {
        var minesLeft = mines;
        while (minesLeft > 0)
        {
            var x = Random.Range(0, width);
            var y = Random.Range(0, height);
            var cell = cells[x, y];
            if (cell.cellType != Cell.CellType.Mine)
            {
                cell.cellType = Cell.CellType.Mine;
                cells[x, y] = cell;
                minesLeft--;
            }
        }
    }

    private void SetupNumbers()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var cell = cells[x, y];
                if (cell.cellType != Cell.CellType.Mine)
                {
                    var number = 0;
                    for (int i = -1; i <= 1; i++)
                    {
                        for (int j = -1; j <= 1; j++)
                        {
                            if (i == 0 && j == 0)
                            {
                                continue;
                            }
                            var x1 = x + i;
                            var y1 = y + j;
                            if (x1 >= 0 && x1 < width && y1 >= 0 && y1 < height)
                            {
                                var cell1 = cells[x1, y1];
                                if (cell1.cellType == Cell.CellType.Mine)
                                {
                                    number++;
                                }
                            }
                        }
                    }
                    cell.number = number;
                    if (number > 0)
                    {
                        cell.cellType = Cell.CellType.Number;
                    }
                    cells[x, y] = cell;
                }
            }
        }
        ReloadBoard();
    }
    #endregion Game Generator

    #region Win&Lose
    private bool CheckWin()
    {
        bool ret = true;

        // All cells revealed except mines
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var cell = cells[x, y];
                if (cell.cellType != Cell.CellType.Mine && !cell.isRevealed)
                {
                    ret = false;
                }
            }
        }
        if (ret) return ret;

        // All mines flagged
        ret = true;
        var minesLeft = mines;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var cell = cells[x, y];
                if (cell.isFlagged)
                {
                    if (cell.cellType == Cell.CellType.Mine)
                    {
                        minesLeft--;
                    }
                    else
                    {
                        // If there is a bad flag
                        ret = false;
                    }
                }
            }
        }
        if (minesLeft != 0)
            ret = false;

        return ret;
    }

    private void Win()
    {
        Debug.Log("Win");
        letIngameInput = false;
    }

    private void GameOver()
    {
        Debug.Log("GameOver");
        letIngameInput = false;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var cell = cells[x, y];
                if (cell.cellType == Cell.CellType.Mine)
                {
                    cell.isRevealed = true;
                    cells[x, y] = cell;
                }
            }
        }
        ReloadBoard();
    }
    #endregion Win&Lose

    #region Helpers
    private void QuitGame()
    {
        UnityEditor.EditorApplication.isPlaying = false;
        Application.Quit();
    }
    
    private void ReloadBoard()
    {
        board.Draw(cells, isGodMode, isRevealedExceptMines);
    }

    private void MoveCamera()
    {
        var camera = Camera.main;
        var position = camera.transform.position;
        position.x = width / 2f;
        position.y = height / 2f;
        camera.transform.position = position;
    }

    private void SetCamera()
    {
        var camera = Camera.main;

        // Position on the middle
        var position = camera.transform.position;
        position.x = width / 2f;
        position.y = height / 2f;
        position.z = -10;
        camera.transform.position = position;

        // Size
        switch (difficulty)
        {
            case Difficulty.Begginer:
                camera.orthographicSize = 5;
                break;
            case Difficulty.Intermediate:
                camera.orthographicSize = 9;
                break;
            case Difficulty.Extreme:
                camera.orthographicSize = 11;
                break;
            case Difficulty.Legend:
                camera.orthographicSize = 13;
                break;
            default:
                camera.orthographicSize = 5;
                break;
        }
    }
    #endregion Helpers
}
