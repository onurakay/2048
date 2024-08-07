using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileGrid : MonoBehaviour
{
    public TileRow[] tileRows { get; private set; }
    public TileCell[] tileCells { get; private set; }
    public int size => tileCells.Length;
    public int height => tileRows.Length;
    public int width => size / height;
    
    void Awake() {
        tileRows = GetComponentsInChildren<TileRow>();
        tileCells = GetComponentsInChildren<TileCell>();
    }

    void Start() {
        for (int y = 0; y < tileRows.Length; y++)
        {
            for (int x = 0; x < tileRows[y].cells.Length; x++)
            {
                tileRows[y].cells[x].coordinates = new Vector2Int(x, y);
            }
        }
    }

    public TileCell GetRandomEmptyCell()
    {
        List<TileCell> emptyCells = new List<TileCell>();

        foreach (var cell in tileCells)
        {
            if (cell.empty)
            {
                emptyCells.Add(cell);
            }
        }

        if (emptyCells.Count == 0)
        {
            return null;
        }

        return emptyCells[Random.Range(0, emptyCells.Count)];
    }


    public TileCell GetCell(int x, int y)
    {
        if (x >= 0 && x < width && y >= 0 && y < height)
        {
            return tileRows[y].cells[x];
        }
        else
        {
            return null;
        }
    }

    public TileCell GetAdjacentCell(TileCell cell, Vector2Int direction)
    {
        Vector2Int coordinates = cell.coordinates;
        coordinates.x += direction.x;
        coordinates.y -= direction.y;

        return GetCell(coordinates);
    }

    TileCell GetCell(Vector2Int coordinates)
    {
        return GetCell(coordinates.x, coordinates.y);
    }
}
