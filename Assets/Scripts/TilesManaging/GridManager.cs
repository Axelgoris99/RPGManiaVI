using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.Rendering.Universal;

public class GridManager : MonoBehaviour
{
    #region Singleton
    private static GridManager _instance;
    public static GridManager Instance { get { return _instance; } }

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
    #endregion

    public GameObject gridElementPrefab;
    public int gridSizeX = 20;
    public int gridSizeZ = 20;
    public float spacing = 1.0f;
    public Vector3 target;
    public GameObject centerAround;
    public List<Tile> tiles;
    public LayerMask layer;
    public Material decalMaterial;

    public delegate void SimpleEvent();
    public static event SimpleEvent onGridGenerated;

    private void OnEnable()
    {
        FightManager.onFightStarted += OnBattleStart;

    }
    private void OnDisable()
    {
        FightManager.onFightStarted -= OnBattleStart;
    }

    private void OnBattleStart()
    {
        Grid.tilesGrid.Clear();
        target = centerAround.transform.position;
        GenerateGrid();
        onGridGenerated?.Invoke();
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
                Vector3 normal = Vector3.up;
                // Needed to get it higher to avoid being INSIDE the object and thus not triggering object collider
                if (Physics.Raycast(position + 10 * Vector3.up, Vector3.down, out rayHit, 100f, layer))
                {
                    position.y = rayHit.point.y;
                    normal = rayHit.normal;

                }

                if (NavMesh.SamplePosition(position, out hit, spacing / 2, NavMesh.AllAreas))
                {
                    // The position is on a navigable area,  create a cube.
                    GameObject currentGo = Instantiate(gridElementPrefab, position, Quaternion.identity, transform);

                    // New tile
                    Tile tile = new Tile(Tile.TileTerrain.WALKABLE, x, position.y, z, gridElementPrefab, position);
                    tile.currentTile = currentGo;
                    CurrentTile currentTile = currentGo.AddComponent<CurrentTile>();
                    currentTile.tile = tile;
                    currentGo.transform.rotation = Quaternion.FromToRotation(Vector3.up, normal);
                    currentGo.GetComponent<Collider>().enabled = false;
                    
                    // Adding the new tile to the grid
                    Grid.tilesGrid.Add(new Vector2Int(x, z), tile);
                    
                    // Decal Projector to display with the range finder
                    GameObject decalGo = new GameObject("decalChild");
                    decalGo.transform.position = currentGo.transform.position + Vector3.up / 2;
                    decalGo.transform.localEulerAngles += new Vector3(90f, 0, 0);
                    decalGo.transform.parent = currentGo.transform;
                    DecalProjector decalProjector = decalGo.AddComponent<DecalProjector>();
                    decalProjector.size = new Vector3(1, 1, 10);
                    decalProjector.material = decalMaterial;
                    decalGo.SetActive(false);
                }
                else
                {
                    var currentGo = Instantiate(gridElementPrefab, position, Quaternion.identity, transform);
                    currentGo.layer = 7;
                    Tile tile = new Tile(Tile.TileTerrain.NOT_WALKABLE, x, position.y, z, null, position);
                    tile.currentTile = currentGo;
                    var currentTile = currentGo.AddComponent<CurrentTile>();
                    currentTile.tile = tile;
                    Grid.tilesGrid.Add(new Vector2Int(x, z), tile);
                }

            }
        }
    }
    public List<Tile> GetNeightbourTilesWithHeight(Vector2Int currentTile)
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
                if (Grid.tilesGrid[locationToCheck].IsThisTileWalkable())
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
                if (Grid.tilesGrid[locationToCheck].IsThisTileWalkable())
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
                if (Grid.tilesGrid[locationToCheck].IsThisTileWalkable())
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
                if (Grid.tilesGrid[locationToCheck].IsThisTileWalkable())
                    neighbours.Add(map[locationToCheck]);
            }
        }

        return neighbours;
    }

}