using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBoard : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] Tile tilePrefab;
    [SerializeField] TileHandler[] tileHandler;

    [Header("Settings")]
    [SerializeField] GameManager gameManager;
    [SerializeField] float waitDuration = 0.1f;
    [SerializeField] float fourTileProbability = 0.1f;
    
    TileGrid grid;
    List<Tile> tiles;
    bool isMoving = false;

    void Awake()
    {
        grid = GetComponentInChildren<TileGrid>();
        //memory allocation
        tiles = new List<Tile>(16);
    }

    void Update()
    {
        if (!isMoving)
        {
            HandleInput();
        }
    }

    #region Tile Management

    public void GenerateTile()
    {
        Tile tile = Instantiate(tilePrefab, grid.transform);

        int tileNumber = UnityEngine.Random.value < fourTileProbability ? 4 : 2;
        TileHandler handler = tileNumber == 4 ? tileHandler[1] : tileHandler[0];
        
        tile.InitializeTile(handler, tileNumber);
        tile.Spawn(grid.GetRandomEmptyCell());

        tiles.Add(tile);
    }

    public void ClearBoard()
    {
        foreach (var cell in grid.tileCells)
        {
            cell.tile = null;
        }

        foreach (var tile in tiles)
        {
            Destroy(tile.gameObject);
        }

        tiles.Clear();
    }

    #endregion

    #region Movement

    void HandleInput()
    {
        Vector2Int direction = Vector2Int.zero;
        int startX = 0, incrementX = 1, startY = 0, incrementY = 1;

        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            direction = Vector2Int.up;
        }
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            direction = Vector2Int.down;
            startY = grid.height - 1;
            incrementY = -1;
        }
        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            direction = Vector2Int.left;
            startX = grid.width - 1;
            incrementX = -1;
        }
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            direction = Vector2Int.right;
            startX = 0;
            incrementX = 1;
        }

        if (direction != Vector2Int.zero)
        {
            MoveTiles(direction, startX, incrementX, startY, incrementY);
        }
    }

    void MoveTiles(Vector2Int direction, int startX, int incrementX, int startY, int incrementY)
    {
        bool changed = false;

        for (int x = startX; x >= 0 && x < grid.width; x += incrementX)
        {
            for (int y = startY; y >= 0 && y < grid.height; y += incrementY)
            {
                TileCell cell = grid.GetCell(x, y);
                if (cell.occupied)
                {
                    changed |= MoveTile(cell.tile, direction);
                }
            }
        }

        if (changed)
        {
            StartCoroutine(WaitForChanges());
        }
    }

    bool MoveTile(Tile tile, Vector2Int direction)
    {
        TileCell newCell = null;
        TileCell adjacent = grid.GetAdjacentCell(tile.tileCell, direction);

        while (adjacent != null)
        {
            if (adjacent.occupied)
            {
                if (IsMergeableWith(tile, adjacent.tile))
                {
                    MergeTiles(adjacent.tile, tile);
                    return true;
                }
                break;
            }

            newCell = adjacent;
            adjacent = grid.GetAdjacentCell(adjacent, direction);
        }
        
        if (newCell != null)
        {
            tile.MoveTo(newCell);
            isMoving = false;
            return true;
        }

        return false;
    }

    #endregion

    #region Tile Merging

    void MergeTiles(Tile tile, Tile otherTile)
    {
        tiles.Remove(otherTile);
        otherTile.Merge(tile.tileCell);

        int index = Mathf.Clamp(IndexOf(tile.tileHandler) + 1, 0, this.tileHandler.Length - 1);
        int newNumber = tile.number * 2;

        tile.InitializeTile(tileHandler[index], newNumber);
        gameManager.IncreaseScore(newNumber);
    }

    bool IsMergeableWith(Tile tile, Tile otherTile)
    {
        // check if numbers match amd the other tile isnt locked
        return tile.number == otherTile.number && !otherTile.locked;
    }

    //study
    int IndexOf(TileHandler handler)
    {
        return Array.IndexOf(tileHandler, handler);
    }

    #endregion

    public bool IsGameOver()
    {
        if (tiles.Count != grid.size) 
        {
            return false;
        }

        Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

        foreach (var tile in tiles)
        {
            foreach (var direction in directions)
            {
                TileCell adjacentCell = grid.GetAdjacentCell(tile.tileCell, direction);
                if (adjacentCell != null && IsMergeableWith(tile, adjacentCell.tile))
                {
                    return false;
                }
            }
        }

        return true;
    }

    IEnumerator WaitForChanges()
    {
        isMoving = true;
        yield return new WaitForSeconds(waitDuration);

        isMoving = false;

        foreach (var tile in tiles)
        {
            Debug.Log("Unlocking tile");
            tile.locked = false;
        }

        if (tiles.Count != grid.size)
        {
            GenerateTile();
        }

        if (IsGameOver())
        {
            gameManager.GameOver();
        }
    }
}