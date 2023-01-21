using System.Collections;
using System.Collections.Generic;
using System.Net;
using Unity.VisualScripting;
using UnityEngine;

public class Game : MonoBehaviour
{
    #region Variables
    public enum Difficulty
    {
        Beginner, // Beginner = 9x9, 10 mines, 0.12% chance of mine
        Intermediate, // Intermediate = 16x16, 40 mines, 0.16% chance of mine
        Extreme, // Expert = 20x20, 85 mines, 0.21% chance of mine
        Legend // Legend = 25x25, 160 mines, 0.25% chance of mine
    }
    public Difficulty difficulty;
    private int width;
    private int height;
    private int mines;

    private bool letIngameInput = true; // This boolean locks the input of the player. If false player can not play
    private bool firstGameClick = false; // Flag used for the first click of the player. If true, the first click will be ignored
    private bool isGodMode = false; // Debug key boolean to reveal all tiles
    private bool isRevealedExceptMines = false; // Debug key boolean to reveal all tiles except mines

    private Board board; // Board object
    private Highlight highlight; //Highlights on top of board
    private Cell[,] cells; // 2D array of current cells
    private Cell lastRevealedCell; // Last revealed cell
    private SinglePlayerGameUI singlePlayerGameUI; // SinglePlayerGameUI object
    #endregion Variables

    #region Unity Methods
    private void Awake()
    {
        // Setting up the object variables
        cells = new Cell[width, height];
        board = GetComponentInChildren<Board>();
        highlight = GetComponentInChildren<Highlight>();

        // Set active a game object from the same parent
        singlePlayerGameUI = GetComponent<SinglePlayerGameUI>();
        singlePlayerGameUI.mainMenuGo.SetActive(false);
    }

    private void Start()
    {
        // For now, the game is generated when the game starts. A menu will be added later
        StartGame(difficulty);
    }

    private void Update()
    {
        // Debug keys handler. F1 -> GodMode. F2 -> RevealedExceptMines. R -> Restart game. Esc -> Quit game
        DebugKeys();

        // Besides this line, if the flag is false, won't run the code, so in game input is not allowed
        if (!letIngameInput) return;

        ReloadHighlight();
        IngameInput();
    }

    private void ReloadHighlight()
    {
        // Get mouse position and convert it to world position (2D)
        Vector3 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        int x = Mathf.FloorToInt(position.x);
        int y = Mathf.FloorToInt(position.y);

        // Bounds check
        if (x >= 0 && x < width && y >= 0 && y < height)
        {
            highlight.Draw(x, y);
        }
        else
        {
            highlight.Draw(-100, -100);
        }

    }

    #endregion Unity Methods

    #region Input Logic
    private void IngameInput()
    {
        // Left click
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            int x = Mathf.FloorToInt(position.x);
            int y = Mathf.FloorToInt(position.y);
            if (x >= 0 && x < width && y >= 0 && y < height)
            {
                if (!firstGameClick)
                {
                    GameResolver.ResolveCells(cells, x, y);
                    firstGameClick = true;
                }
                else
                {
                    cells = GameResolver.ResolveCells(cells, x, y);
                }
            }
            
            ReloadBoard();
        }
        // Right click
        if (Input.GetMouseButtonDown(1))
        {
            // Don't let flags to be placed at the start of the game
            if (firstGameClick)
                Flag();
        }
    }

    // Some debug keys
    private void DebugKeys()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            isGodMode = false;
            isRevealedExceptMines = false;
            StartGame(difficulty);
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
        // Get mouse position and convert it to world position (2D)
        Vector3 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        int x = Mathf.FloorToInt(position.x);
        int y = Mathf.FloorToInt(position.y);
        // Bounds check
        if (x >= 0 && x < width && y >= 0 && y < height)
        {
            Cell cell = cells[x, y];
            // Only tiles that were not revealed can be flagged
            if (!cell.isRevealed)
            {
                // If there are flags available
                if (CheckFlagsAvailable())
                {
                    cell.isFlagged = !cell.isFlagged;
                }
                else
                {
                    // If there are no flags available, unflagging the cell
                    if (cell.isFlagged)
                    {
                        cell.isFlagged = false;
                    }
                }
                cells[x, y] = cell;
                // Check if all bombs are flagged
                if (CheckWin())
                {
                    Win();
                }
                // Update the board
                ReloadBoard();

            }
        }
    }

    // Counts the total of flags used. Returns false if there are no flags available
    private bool CheckFlagsAvailable()
    {
        int flagsUsed = 0;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Cell cell = cells[x, y];
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
        Vector3 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        int x = Mathf.FloorToInt(position.x);
        int y = Mathf.FloorToInt(position.y);
        if (x >= 0 && x < width && y >= 0 && y < height)
        {
            Cell cell = cells[x, y];
            if (cell.isFlagged) return;

            // First click logic
            if (!firstGameClick)
            {
                firstGameClick = true;
                lastRevealedCell = cell;
                cell.isRevealed = true;
                cells[x, y] = cell;
                CreateFullMap();
            }

            // If cell is an empty cell, reveal numbers and empty cells
            if (cell.cellType == Cell.CellType.Empty)
            {
                RevealEmptyCells(cell);
                if (CheckWin())
                {
                    Win();
                }
            }
            // If cell is a mine, reveal all mines and end the game
            else if (cell.cellType == Cell.CellType.Mine)
            {
                cell.isExploded = true;
                cells[x, y] = cell;
                GameOver();
            }
            // Number cell, reveal it
            else
            {
                lastRevealedCell = cell;
                cell.isRevealed = true;
                cells[x, y] = cell;
                if (CheckWin())
                {
                    Win();
                }
            }

            // Checks if the flags and mines are the same number, and reveals the near cells
            if (cell.isRevealed && !cell.isExploded)
            {
                RevealAdjacentAvailableCells(cell);
            }

            ReloadBoard();

        }
    }

    // Recursive function to reveal all empty cells that are close to each other
    private void RevealEmptyCells(Cell cell)
    {
        if (cell.isRevealed) return;
        if (cell.cellType == Cell.CellType.Mine) return;

        lastRevealedCell = cell;
        cell.isRevealed = true;
        cells[cell.position.x, cell.position.y] = cell;

        if (cell.cellType == Cell.CellType.Empty)
        {
            List<Cell> nearCells = GetNearCells(cell);
            foreach (Cell nearCell in nearCells)
            {
                RevealEmptyCells(nearCell);
            }
        }
    }

    // This functions return all the cells that are near the cell passed as parameter
    private List<Cell> GetNearCells(Cell cell)
    {
        List<Cell> nearCells = new List<Cell>();
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0) continue;
                int nearX = cell.position.x + x;
                int nearY = cell.position.y + y;
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
        bool willDie = false;

        // Gets bomb count of the surrounding cells
        int bombCount = 0;
        
        for (int i = 0; i < nearCells.Count; i++)
        {
            Cell cellI = nearCells[i];
            if (cellI.cellType == Cell.CellType.Mine)
            {
                bombCount++;
                if (!cellI.isFlagged)
                {
                    willDie = true;
                }
            }
        }
        
        // Gets flag count from the surrounding cells
        int flagCount = 0;
        for (int i = 0; i < nearCells.Count; i++)
        {
            Cell cellI = nearCells[i];
            if (cellI.isFlagged)
            {
                flagCount++;
            }
        }

        // If the bomb count is equal to the number of flags around the cell, reveal the rest of the cells
        if (bombCount == flagCount)
        {
            for (int i = 0; i < nearCells.Count; i++)
            {
                Cell cellI = nearCells[i];
                switch(cellI.cellType)
                {
                    case Cell.CellType.Empty:
                        if (!willDie && !cellI.isRevealed)
                        {
                            lastRevealedCell = cellI;
                            RevealEmptyCells(cellI);
                        }
                        break;
                    case Cell.CellType.Mine:
                        if (!cellI.isFlagged)
                        {
                            cellI.isExploded = true;
                            cells[cellI.position.x, cellI.position.y] = cellI;
                            GameOver();
                        }
                        break;
                    default:
                        if (willDie)
                        {
                            if (!cellI.isRevealed) 
                                cellI.isRevealed = false;
                        }
                        else
                        {
                            lastRevealedCell = cellI;
                            cellI.isRevealed = true;
                        }
                        cells[cellI.position.x, cellI.position.y] = cellI;
                        break;
                }
            }
        }
    }
    #endregion Input Logic

    #region Game Generator
    public void StartGame(Difficulty difficulty)
    {
        Debug.Log("Creating " + difficulty.ToString() + " map");

        // Get width, height and mines from difficulty
        int[] widthHeightMines = GameGenerator.GetWidthHeightMines((DifficultyNew)(int)difficulty);
        width = widthHeightMines[0];
        height = widthHeightMines[1];
        mines = widthHeightMines[2];
        cells = GameGenerator.GenerateCells(width, height);

        letIngameInput = true;
        firstGameClick = false;

        SetCamera();

        ReloadBoard();
    }
    private void CreateFullMap(int[] bounds = null)
    {
        Debug.Log("Creating full " + difficulty.ToString() + " map");

        cells = GameGenerator.GenerateMines(cells, mines, bounds);
        cells = GameGenerator.GenerateNumbers(cells);

        ReloadBoard();
    }

    // Mine generation
    private void CreateMines()
    {
        int minesLeft = mines;
        while (minesLeft > 0)
        {
            int x = Random.Range(0, width);
            int y = Random.Range(0, height);

            if (x >= lastRevealedCell.position.x  - 1 && x <= lastRevealedCell.position.x + 1 && y >= lastRevealedCell.position.y - 1 && y <= lastRevealedCell.position.y + 1) continue;

            Cell cell = cells[x, y];
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
                Cell cell = cells[x, y];
                if (cell.cellType != Cell.CellType.Mine)
                {
                    int number = 0;
                    for (int i = -1; i <= 1; i++)
                    {
                        for (int j = -1; j <= 1; j++)
                        {
                            if (i == 0 && j == 0)
                            {
                                continue;
                            }
                            int x1 = x + i;
                            int y1 = y + j;
                            if (x1 >= 0 && x1 < width && y1 >= 0 && y1 < height)
                            {
                                Cell cell1 = cells[x1, y1];
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
                Cell cell = cells[x, y];
                if (cell.cellType != Cell.CellType.Mine && !cell.isRevealed)
                {
                    ret = false;
                }
            }
        }
        if (ret) return ret;

        // All mines flagged
        ret = true;
        int minesLeft = mines;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Cell cell = cells[x, y];
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

        singlePlayerGameUI.mainMenuGo.SetActive(true);

    }

    private void GameOver()
    {
        Debug.Log("GameOver");
        letIngameInput = false;
        
        // Reveal all mines
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Cell cell = cells[x, y];
                if (cell.cellType == Cell.CellType.Mine)
                {
                    if (!cell.isFlagged)
                    {
                        lastRevealedCell = cell;
                        cell.isRevealed = true;
                        cells[x, y] = cell;
                    }
                }
                else
                {
                    if (cell.isFlagged)
                    {
                        cell.isBadFlagged = true;
                        cell.isRevealed = false;
                        cells[x, y] = cell;
                    }
                }
            }
        }
        highlight.Clear();
        ReloadBoard();

        // Make the main menu button appear
        singlePlayerGameUI.mainMenuGo.SetActive(true);

    }
    #endregion Win&Lose

    #region Helpers
    private void QuitGame()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }

    // This function reloads the drawing of the board.
    // Called whenever a cell changes the state
    private void ReloadBoard()
    {
        board.Draw(cells, isGodMode, isRevealedExceptMines);
    }

    private void SetCamera()
    {
        Camera camera = Camera.main;
        
        // Position the camera on the middle of the board
        Vector3 position = camera.transform.position;
        position.x = width / 2f;
        position.y = height / 2f;
        position.z = -10;
        camera.transform.position = position;

        // Resize the camera depending on the board size
        switch (difficulty)
        {
            case Difficulty.Beginner:
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
