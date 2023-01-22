using System.Collections;
using System.Collections.Generic;
using System.Net;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;

public class MultiPlayerGame : MonoBehaviour
{
    #region Variables

    public int health = 3;

    public DifficultyNew difficulty = DifficultyNew.Beginner;
    private int width;
    private int height;
    private int mines;

    private int emptyCellScore = 1;

    private float mousePositionTime = 0.5f; // Time between mouse lerps
    private float mousePositionTimer = 0.5f;

    private bool letIngameInput = true; // This boolean locks the input of the player. If false player can not play
    private bool isGodMode = false; // Debug key boolean to reveal all tiles
    private bool isRevealedExceptMines = false; // Debug key boolean to reveal all tiles except mines

    private float countdownTimePerNumber = 3.0f; // Starting counter before the game
    private int countdownNumber = 0; // Starting counter before the game
    private bool once = true;
    private StartingCountdown countdown;

    private MultiplayerBoard board;
    private Highlight highlight; // Highlights on top of board
    private TileClear tileClear; // Tiles in background script

    private Cell[,] cells; // 2D array of current cells
    private SinglePlayerGameUI buttonMainMenuUI; // SinglePlayerGameUI object

    private ScoreManager scoreManager;

    public Client localPlayer;

    private CameraShaker cameraShaker;
    
    public float normalShakerDuration ;
    public float normalShakerIntensity;
    public float bombShakerDuration;
    public float bombShakerIntensity;

    public GameObject winnerText;

    [HideInInspector]
    public bool win = false;

    private EventHandler eventHandler;
    #endregion Variables

    #region Unity Methods
    private void Awake()
    {
        board = GetComponentInChildren<MultiplayerBoard>();

        highlight = GetComponentInChildren<Highlight>();
        highlight.width = width;
        highlight.height = height;

        tileClear = GetComponentInChildren<TileClear>();
        countdown = GetComponent<StartingCountdown>();

        buttonMainMenuUI = GetComponent<SinglePlayerGameUI>();
        buttonMainMenuUI.mainMenuGo.SetActive(false);
        
        localPlayer = GameObject.Find("ClientManager").GetComponent<Client>();
        localPlayer.game = this;
        eventHandler = GameObject.Find("ClientManager").GetComponent<EventHandler>();

        cameraShaker = Camera.main.GetComponent<CameraShaker>();

        scoreManager = localPlayer.scoreManager;
        scoreManager.localPlayer = localPlayer.clientData;

        TMP_Text scoreText = GameObject.Find("Score").GetComponent<TMP_Text>();
        scoreText.alignment = TextAlignmentOptions.Left;
    }

    private void Update()
    {
        // Debug keys handler
        DebugKeys();

        // Send Mouse Position periodically
        if (mousePositionTimer > 0)
        {
            mousePositionTimer -= Time.deltaTime;
        }
        else
        {
            mousePositionTimer = mousePositionTime;

            // Get mouse position and convert it to world position (2D)
            Vector3 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            eventHandler.SendMousePosition(position.x, position.y);
        }

        // Starting Countdown before game starts
        if (StartingCountdown()) return;

        // Besides this line, if the flag is false, won't run the code, so in game input is not allowed
        if (!letIngameInput) return;

        ReloadHighlight();

        // Left click
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            int x = Mathf.FloorToInt(position.x);
            int y = Mathf.FloorToInt(position.y);

            if (x >= 0 && x < width && y >= 0 && y < height)
            {
                Cell cell = cells[x, y];
                if (cell.isFlagged || (cell.isRevealed)) return;

                eventHandler.SendRevealCell(x, y);
            }
        }
        // Right click
        if (Input.GetMouseButtonDown(1))
        {
            // Don't let flags to be placed at the start of the game
            Flag();
        }

    }
    #endregion Unity Methods

    public void StartGame(Cell[,] cells)
    {
        // Set difficulty, map size and generate board
        difficulty = DifficultyNew.Extreme;
        width = cells.GetLength(0);
        height = cells.GetLength(1);
        mines = GameGenerator.GetWidthHeightMines(difficulty)[2];
        this.cells = new Cell[width, height];
        this.cells = cells;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                this.cells[x, y].position = new Vector3Int(x, y, 0);
            }
        }
        this.cells = GameGenerator.GenerateNumbers(cells);
        SetCamera();
        ReloadBoard();
    }

    private bool StartingCountdown()
    {
        bool ret = false;

        if (countdownNumber == 0)
        {
            countdownTimePerNumber = 1f;
            countdown.SendNumber(countdownNumber, width / 2, height / 2);
            countdownNumber++;
            return true;
        }
        
        if (countdownTimePerNumber > 0f)
        {
            countdownTimePerNumber -= Time.deltaTime;
            return true;
        }

        if (countdownNumber < 3)
        {
            countdownTimePerNumber = 1f;
            countdown.SendNumber(countdownNumber, width / 2, height / 2);
            countdownNumber++;
            return true;

        }
        else if (countdownNumber == 4)
        {
            countdownTimePerNumber = 1f;
            countdownNumber++;
            return true;

        }

        if (once)
        {
            eventHandler.SendFirstCell(width / 2, height / 2);
            once = false;
        }

        return ret;
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

    #region Input Logic
    // When received Reveal cell from server
    public void Reveal(int x, int y, ClientData clientData)
    {
        Cell cell = cells[x, y];
        if (cell.isFlagged || cell.isRevealed) return;

        // If cell is an empty cell, reveal numbers and empty cells
        if (cell.cellType == Cell.CellType.Empty)
        {
            RevealEmptyCells(cell, clientData);

            if (CheckWin())
            {
                Win();
            }
        }
        // If cell is a mine, reveal all mines and end the game
        else if (cell.cellType == Cell.CellType.Mine)
        {
            tileClear.SendTileToNarnia(cell.position, clientData.colorPlayer);

            cell.isExploded = true;
            cells[x, y] = cell;

            if (localPlayer.clientData.userID == clientData.userID)
            {
                cameraShaker.TriggerShake(bombShakerDuration, bombShakerIntensity);
            }

            health--;

            if(health == 0)
                GameOver();
        }
        // Number cell, reveal it
        else
        {
            tileClear.SendTileToNarnia(cell.position, clientData.colorPlayer);

            if (localPlayer.clientData.userID == clientData.userID)
                cameraShaker.TriggerShake(normalShakerDuration, normalShakerIntensity);

            AddScore(clientData, emptyCellScore);
            cell.isRevealed = true;
            cell.color = clientData.colorPlayer;
            
            cells[x, y] = cell;
            if (CheckWin())
            {
                Win();
            }
        }

        ReloadBoard();
    }

    // Recursive function to reveal all empty cells that are close to each other
    private void RevealEmptyCells(Cell cell, ClientData clientData)
    {
        if (cell.isRevealed || cell.color != ColorPlayer.NONE) return;

        tileClear.SendTileToNarnia(cell.position, clientData.colorPlayer);

        if (localPlayer.clientData.userID == clientData.userID)
            cameraShaker.TriggerShake(normalShakerDuration, bombShakerIntensity);
        
        clientData.score += emptyCellScore;
        AddScore(clientData, 0);

        cell.isRevealed = true;
        cell.color = clientData.colorPlayer;
        cells[cell.position.x, cell.position.y] = cell;

        if (cell.cellType == Cell.CellType.Empty)
        {
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    int x = cell.position.x + i;
                    int y = cell.position.y + j;
                    if (x >= 0 && x < width && y >= 0 && y < height)
                    {
                        Cell cell1 = cells[x, y];
                        if (!cell1.isRevealed)
                        {
                            RevealEmptyCells(cell1, clientData);
                        }
                    }
                }
            }
        }
    }

    //Paneo
    private void AddScore(ClientData clientData, int ammount)
    {
        if (clientData.colorPlayer == ColorPlayer.NONE) return;

        if (localPlayer.clientData.userID == clientData.userID)
        {
            localPlayer.clientData.score = clientData.score + ammount;
            scoreManager.UpdateScores(localPlayer.clientData);
        }
        else
        {
            clientData.score += ammount;
            scoreManager.UpdateScores(clientData);
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
                    cell.color = localPlayer.clientData.colorPlayer;
                }
                else
                {
                    // If there are no flags available, unflagging the cell
                    if (cell.isFlagged)
                    {
                        cell.isFlagged = false;
                        cell.color = ColorPlayer.NONE;
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

    // Some debug keys
    private void DebugKeys()
    {
  
        if (Input.GetKeyDown(KeyCode.F1))
        {
            isGodMode = !isGodMode;
            isRevealedExceptMines = false;
            ReloadBoard();
        }
        if (Input.GetKeyDown(KeyCode.F2))
        {
            //isRevealedExceptMines = !isRevealedExceptMines;
            //isGodMode = false;
            //ReloadBoard();
        }

        if (Input.GetKeyDown(KeyCode.F3))
        {
            Win();
        }
    }
    #endregion Input Logic

    #region Win&Lose
    // Check if all mines are flagged or all cells are revealed except mines
    private bool CheckWin()
    {
        int flagsUsedWell = 0;
        foreach (Cell cell in cells)
        {
            if (cell.isFlagged && cell.cellType == Cell.CellType.Mine)
                flagsUsedWell++;
        }
        return flagsUsedWell == mines ? true : false;
    }

    // Ends game and shows final score
    private void Win()
    {
        Debug.Log("Win");
        letIngameInput = false;

        GameObject.Find("Tilemap").SetActive(false);
        GameObject.Find("HighlightMap").SetActive(false);

        Vector3 newPos = new Vector3(0, -100, 0);
        RectTransform rTransform = GameObject.Find("Score").GetComponent<RectTransform>();
        rTransform.anchoredPosition = newPos;

        TMP_Text scoreText = GameObject.Find("Score").GetComponent<TMP_Text>();
        scoreText.alignment = TextAlignmentOptions.Center;

        scoreManager.currentPlayingPlayers.Sort(SortByScore);
        winnerText.SetActive(true);
        winnerText.GetComponent<TMP_Text>().text = scoreManager.currentPlayingPlayers[0].userName.ToUpper() + " HAS WON!";

        buttonMainMenuUI.mainMenuGo.SetActive(true);

        win = true;
    }

    // Sorts the score list from highest to lowest
    private int SortByScore(ClientData c1, ClientData c2)
    {
        return c2.score.CompareTo(c1.score);
    }

    // Reveals all bomb tiles and displays menu button
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
                        tileClear.SendTileToNarnia(cell.position, ColorPlayer.NONE);

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
        buttonMainMenuUI.mainMenuGo.SetActive(true);
        foreach (Transform child in buttonMainMenuUI.mainMenuGo.transform)
            child.gameObject.SetActive(true);
    }
    #endregion Win&Lose

    #region Helpers
    private void QuitGame()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
        Application.Quit();
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
            case DifficultyNew.Beginner:
                camera.orthographicSize = 5;
                break;
            case DifficultyNew.Intermediate:
                camera.orthographicSize = 9;
                break;
            case DifficultyNew.Extreme:
                camera.orthographicSize = 11;
                break;
            case DifficultyNew.Legend:
                camera.orthographicSize = 13;
                break;
            default:
                camera.orthographicSize = 5;
                break;
        }
    }
    #endregion Helpers
}
