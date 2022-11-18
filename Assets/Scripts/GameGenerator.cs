using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public enum DifficultyNew
{
    Beginner, // Beginner = 9x9, 10 mines, 0.12% chance of mine
    Intermediate, // Intermediate = 16x16, 40 mines, 0.16% chance of mine
    Extreme, // Expert = 20x20, 85 mines, 0.21% chance of mine
    Legend // Legend = 25x25, 160 mines, 0.25% chance of mine
}

public static class GameGenerator
{
    public static int[] GetWidthHeightMines(DifficultyNew difficulty)
    {
        int[] widthHeightMines = new int[3];
        switch (difficulty)
        {
            case DifficultyNew.Beginner:
                widthHeightMines[0] = 9;
                widthHeightMines[1] = 9;
                widthHeightMines[2] = 10;
                break;
            case DifficultyNew.Intermediate:
                widthHeightMines[0] = 16;
                widthHeightMines[1] = 16;
                widthHeightMines[2] = 40;
                break;
            case DifficultyNew.Extreme:
                widthHeightMines[0] = 20;
                widthHeightMines[1] = 20;
                widthHeightMines[2] = 85;
                break;
            case DifficultyNew.Legend:
                widthHeightMines[0] = 25;
                widthHeightMines[1] = 25;
                widthHeightMines[2] = 160;
                break;
        }
        return widthHeightMines;
    }

    public static Cell[,] GenerateCells(int width, int height)
    {
        Cell[,] cells = new Cell[width, height];
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
        return cells;
    }
    public static Cell[,] GenerateCells(DifficultyNew difficulty)
    {
        int[] widthHeightMines = GetWidthHeightMines(difficulty);
        return GenerateCells(widthHeightMines[0], widthHeightMines[1]);
    }

    public static Cell[,] GenerateMines(Cell[,] cells, int numMines, int[] bounds = null)
    {
        Cell[,] _cells = cells;

        int width = _cells.GetLength(0);
        int height = _cells.GetLength(1);
        int minesLeft = numMines;
        while (minesLeft > 0)
        {
            int x = Random.Range(0, width);
            int y = Random.Range(0, height);

            if (bounds != null)
            {
                if (x >= bounds[0] && x <= bounds[1] && y >= bounds[2] && y <= bounds[3])
                {
                    continue;
                }
            }

            if (_cells[x, y].cellType != Cell.CellType.Mine)
            {
                _cells[x, y].cellType = Cell.CellType.Mine;
                minesLeft--;
            }
        }

        return _cells;
    }
    public static Cell[,] GenerateMines(Cell[,] cells, int numMines, int[][] bounds = null)
    {
        Cell[,] _cells = cells;

        int width = _cells.GetLength(0);
        int height = _cells.GetLength(1);
        int minesLeft = numMines;
        while (minesLeft > 0)
        {
            int x = Random.Range(0, width);
            int y = Random.Range(0, height);

            bool skip = false;
            if (bounds != null)
            {
                foreach (int[] bound in bounds)
                {
                    int xMin = bound[0];
                    int xMax = bound[1];
                    int yMin = bound[2];
                    int yMax = bound[3];

                    if (x >= xMin && x <= xMax && y >= yMin && y <= yMax)
                    {
                        skip = true;
                        continue;
                    }
                }
            }
            if (skip)
                continue;

            if (_cells[x, y].cellType != Cell.CellType.Mine)
            {
                _cells[x, y].cellType = Cell.CellType.Mine;
                minesLeft--;
            }
        }

        return _cells;
    }

    public static Cell[,] GenerateNumbers(Cell[,] cells)
    {
        Cell[,] _cells = cells;

        int width = _cells.GetLength(0);
        int height = _cells.GetLength(1);
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (_cells[x, y].cellType != Cell.CellType.Mine)
                {
                    int numMines = 0;
                    for (int i = -1; i <= 1; i++)
                    {
                        for (int j = -1; j <= 1; j++)
                        {
                            if (x + i >= 0 && x + i < width && y + j >= 0 && y + j < height)
                            {
                                if (_cells[x + i, y + j].cellType == Cell.CellType.Mine)
                                {
                                    numMines++;
                                }
                            }
                        }
                    }
                    if (numMines > 0)
                        _cells[x, y].cellType = Cell.CellType.Number;
                    _cells[x, y].number = numMines;
                }
            }
        }

        return _cells;
    }

    public static int[] GetBoundsFromFirstCellRevealed(int x, int y)
    {
        // Return bounds with random range of 1-3
        int[] bounds = new int[4];
        bounds[0] = x - Random.Range(1, 3);
        bounds[1] = x + Random.Range(1, 3);
        bounds[2] = y - Random.Range(1, 3);
        bounds[3] = y + Random.Range(1, 3);
        return bounds;
    }

    public static int[] GetMultiplayerBigCenterBounds(int x, int y)
    {
        int[] bounds = new int[4];
        bounds[0] = x - Random.Range(1, 5);
        bounds[1] = x + Random.Range(1, 5);
        bounds[2] = y - Random.Range(1, 5);
        bounds[3] = y + Random.Range(1, 5);
        return bounds;
    }
}

public static class GameResolver
{
    public static Cell[,] ResolveCells(Cell[,] cells, int x, int y, bool isFirstClick = false)
    {
        Cell[,] _cells = cells;
        int width = _cells.GetLength(0);
        int height = _cells.GetLength(1);
        if (x >= 0 && x < width && y >= 0 && y < height)
        {
            if (_cells[x, y].isFlagged)
            {
                return _cells;
            }

            _cells[x, y].isRevealed = true;
            if (_cells[x, y].cellType == Cell.CellType.Mine)
            {
                _cells[x, y].isExploded = true;
                return _cells;
            }
            if (_cells[x, y].cellType == Cell.CellType.Empty)
            {
                for (int i = -1; i <= 1; i++)
                {
                    for (int j = -1; j <= 1; j++)
                    {
                        if (x + i >= 0 && x + i < width && y + j >= 0 && y + j < height)
                        {
                            if (!_cells[x + i, y + j].isRevealed)
                            {
                                _cells = ResolveCells(_cells, x + i, y + j);
                            }
                        }
                    }
                }
            }

            if (_cells[x, y].cellType == Cell.CellType.Number)
            {
                int numFlags = 0;
                for (int i = -1; i <= 1; i++)
                {
                    for (int j = -1; j <= 1; j++)
                    {
                        if (x + i >= 0 && x + i < width && y + j >= 0 && y + j < height)
                        {
                            if (_cells[x + i, y + j].isFlagged)
                            {
                                numFlags++;
                            }
                        }
                    }
                }
                if (numFlags == _cells[x, y].number)
                {
                    for (int i = -1; i <= 1; i++)
                    {
                        for (int j = -1; j <= 1; j++)
                        {
                            if (x + i >= 0 && x + i < width && y + j >= 0 && y + j < height)
                            {
                                if (!_cells[x + i, y + j].isRevealed)
                                {
                                    _cells = ResolveCells(_cells, x + i, y + j);
                                }
                            }
                        }
                    }
                }
            }
        }
        return _cells;
    }
}
