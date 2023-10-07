using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class PathFinder
{
    private Dictionary<Vector2Int, Tile> searchableTiles;
    public List<Tile> FindPath(Tile start, Tile end, List<Tile> inRangeTiles)
    {
        searchableTiles = new Dictionary<Vector2Int, Tile>();

        List<Tile> openList = new List<Tile>();
        List<Tile> closedList = new List<Tile>();

        if (inRangeTiles.Count > 0)
        {
            foreach (var item in inRangeTiles)
            {
                searchableTiles.Add(new Vector2Int(item.x, item.z), item);
            }
        }
        else
        {
            searchableTiles = Grid.tilesGrid;
        }


        openList.Add(start);

        while (openList.Count > 0)
        {
            Tile currentTile = openList.OrderBy(x => x.F).First();

            openList.Remove(currentTile);
            closedList.Add(currentTile);

            if (currentTile == end)
            {
                return GetFinishedList(start, end);
            }

            foreach (var tile in GetNeightbourTiles(currentTile))
            {
                if (!searchableTiles.ContainsKey(new Vector2Int(tile.x, tile.z))) {continue; };
                if (tile.state==Tile.TileTerrain.NOT_WALKABLE || tile.state == Tile.TileTerrain.HAS_A_UNIT_ON || closedList.Contains(tile))
                {
                    continue;
                }

                tile.G = GetManhattenDistance(start, tile);
                tile.H = GetManhattenDistance(end, tile);

                tile.previous = currentTile;


                if (!openList.Contains(tile))
                {
                    openList.Add(tile);
                }
            }
        }

        return new List<Tile>();
    }

    private List<Tile> GetFinishedList(Tile start, Tile end)
    {
        List<Tile> finishedList = new List<Tile>();
        Tile currentTile = end;

        while (currentTile != start)
        {
            finishedList.Add(currentTile);
            currentTile = currentTile.previous;
        }

        finishedList.Reverse();

        return finishedList;
    }

    private int GetManhattenDistance(Tile start, Tile tile)
    {
        return Mathf.Abs(start.x - tile.x) + Mathf.Abs(start.z - tile.z);
    }
    private List<Tile> GetNeightbourTiles(Tile currentTile)
    {
        List<Tile> neighbours = new List<Tile>();

        //right
        Vector2Int locationToCheck = new Vector2Int(
            currentTile.x + 1,
            currentTile.z
        );

        if (searchableTiles.ContainsKey(locationToCheck))
        {
            neighbours.Add(searchableTiles[locationToCheck]);
        }

        //left
        locationToCheck = new Vector2Int(
            currentTile.x - 1,
            currentTile.z
        );

        if (searchableTiles.ContainsKey(locationToCheck))
        {
            neighbours.Add(searchableTiles[locationToCheck]);
        }

        //top
        locationToCheck = new Vector2Int(
            currentTile.x,
            currentTile.z + 1
        );

        if (searchableTiles.ContainsKey(locationToCheck))
        {
            neighbours.Add(searchableTiles[locationToCheck]);
        }

        //bottom
        locationToCheck = new Vector2Int(
            currentTile.x,
            currentTile.z - 1
        );

        if (searchableTiles.ContainsKey(locationToCheck))
        {
            neighbours.Add(searchableTiles[locationToCheck]);
        }

        return neighbours;
    }

}

