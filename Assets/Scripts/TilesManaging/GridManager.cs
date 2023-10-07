using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class Tile
{
    public enum TileTerrain
    {
        NOT_WALKABLE,
        WALKABLE,
        HAS_A_UNIT_ON
    }
    public int x;
    public float y;
    public int z;
    public Vector3 realPos;
    public GameObject currentTile;
    public TileTerrain state;
    public int G;
    public int H;
    public int F { get { return G + H; } }
    public Tile previous;
    public Tile(TileTerrain state, int xPos, float yPos, int zPos, GameObject go, Vector3 position)
    {
        this.state = state;
        x = xPos;
        y = yPos;
        z = zPos;
        realPos = position;
        if (go != null) currentTile = go;
    }
}

public static class Grid
{
    public static List<Tile> tiles = new List<Tile>();
    public static Dictionary<Vector2Int, Tile> tilesGrid = new Dictionary<Vector2Int, Tile>();
}
public class GridManager : MonoBehaviour
{
    private static GridManager _instance;
    public static GridManager Instance { get { return _instance; } }

    public GameObject gridElementPrefab;
    public int gridSizeX = 5;
    public int gridSizeZ = 5;
    public float spacing = 1.0f;
    public Vector3 target;
    public GameObject centerAround;
    public List<Tile> tiles;
    public LayerMask layer;

    public delegate void SimpleEvent();
    public static event SimpleEvent gridGenerated;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }


    private void OnEnable()
    {
        Grid.tiles.Clear();
        Grid.tilesGrid.Clear();
        tiles = new List<Tile>();
        target = centerAround.transform.position;
        GenerateGrid();
        Grid.tiles = tiles;
        gridGenerated?.Invoke();
    }

    void GenerateGrid()
    {
        for (int x = -gridSizeX / 2; x <= gridSizeX / 2; x++)
        {
            for (int z = -gridSizeZ / 2; z <= gridSizeZ / 2; z++)
            {
                Vector3 position = target + new Vector3(x * spacing, 0, z * spacing);
                RaycastHit rayHit;
                NavMeshHit hit;
                // Needed to get it higher to avoid being INSIDE the object and thus not triggering object collider
                if (Physics.Raycast(position + 10 * Vector3.up, Vector3.down, out rayHit, 100f, layer))
                {
                    position.y = rayHit.point.y;
                }

                if (NavMesh.SamplePosition(position, out hit, spacing / 2, NavMesh.AllAreas))
                {
                    //add the hit position up vector
                    // position.y = hit.position.y;
                    // The position is on a navigable area,  create a cube.
                    var currentGo = Instantiate(gridElementPrefab, position, Quaternion.identity, transform);
                    Tile tile = new Tile(Tile.TileTerrain.WALKABLE, x, position.y, z, gridElementPrefab, position);
                    tiles.Add(tile);
                    var currentTile = currentGo.AddComponent<CurrentTile>();
                    currentTile.tile = tile;
                    Grid.tilesGrid.Add(new Vector2Int(x, z), tile);
                }
                else
                {
                    var currentGo = Instantiate(gridElementPrefab, position, Quaternion.identity, transform);
                    currentGo.layer = 7;
                    Tile tile = new Tile(Tile.TileTerrain.NOT_WALKABLE, x, position.y, z, null, position);
                    tiles.Add(tile);
                    var currentTile = currentGo.AddComponent<CurrentTile>();
                    currentTile.tile = tile;
                    Grid.tilesGrid.Add(new Vector2Int(x, z), tile);
                }

            }
        }
    }
    public List<Tile> GetNeightbourTiles(Vector2Int currentTile)
    {
        var map = Grid.tilesGrid;

        List<Tile> neighbours = new List<Tile>();
        //right
        Vector2Int locationToCheck = new Vector2Int(
            currentTile.x + 1,
            currentTile.y
        );

        if (map.ContainsKey(locationToCheck))
        {
            if (Mathf.Abs(Grid.tilesGrid[locationToCheck].y - Grid.tilesGrid[currentTile].y) <= 1)
            {
                neighbours.Add(map[locationToCheck]);
            }
        }

        //left
        locationToCheck = new Vector2Int(
            currentTile.x - 1,
            currentTile.y
        );

        if (map.ContainsKey(locationToCheck))
        {
            if (Mathf.Abs(Grid.tilesGrid[locationToCheck].y - Grid.tilesGrid[currentTile].y) <= 1)
            {
                neighbours.Add(map[locationToCheck]);
            }
        }

        //top
        locationToCheck = new Vector2Int(
            currentTile.x,
            currentTile.y + 1
        );

        if (map.ContainsKey(locationToCheck))
        {
            if (Mathf.Abs(Grid.tilesGrid[locationToCheck].y - Grid.tilesGrid[currentTile].y) <= 1)
            {
                neighbours.Add(map[locationToCheck]);
            }
        }

        //bottom
        locationToCheck = new Vector2Int(
            currentTile.x,
            currentTile.y - 1
        );

        if (map.ContainsKey(locationToCheck))
        {
            if (Mathf.Abs(Grid.tilesGrid[locationToCheck].y - Grid.tilesGrid[currentTile].y) <= 1)
            {
                neighbours.Add(map[locationToCheck]);
            }
        }

        return neighbours;
    }

}