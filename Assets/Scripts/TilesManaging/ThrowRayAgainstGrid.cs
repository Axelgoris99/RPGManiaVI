using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.TextCore.Text;

public class ThrowRayAgainstGrid : MonoBehaviour
{
    [SerializeField]
    private int range;

    [SerializeField]
    private LayerMask layer;

    [SerializeField]
    private GameObject decal;

    [SerializeField]
    private float speed = 1.0f;

    private Player character;

    // Path finder
    private PathFinder pathFinder;
    private List<Tile> path;

    // Range finder
    private RangeFinder rangeFinder;
    private List<Tile> rangeFinderTiles;

    // For ray casting caching
    private Transform previousGOTouched;
    private Tile previousTile;
    private DecalProjector previousDecal;

    // Start is called before the first frame update
    void Start()
    {
        pathFinder = new PathFinder();
        path = new List<Tile>();
        character = Player.Instance;

        rangeFinder = new RangeFinder();
        rangeFinderTiles = new List<Tile>();

    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (rangeFinderTiles.Count <= 0)
        {
            GetInRangeTile();
            DisplayPossibleTile();
        }
        Camera cam = Camera.main;
        // Throw a ray from the camera towards the mouse in the scene
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        // if the ray hits the plane...
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, layer))
        {

            Tile tile = previousTile;
            if (hit.transform != previousGOTouched)
            {
                tile = hit.transform.GetComponent<CurrentTile>().tile;
                if(previousDecal) previousDecal.enabled = true;
                previousDecal = hit.transform.GetChild(0).GetComponent<DecalProjector>();
                previousDecal.enabled = false;
                previousTile = tile;
            }
            if (rangeFinderTiles.Contains(tile))
            {
                if (!decal.activeInHierarchy) decal.SetActive(true);
                decal.transform.position = hit.transform.position + Vector3.up * 1f;
                if (Input.GetMouseButtonDown(0))
                {
                    path = pathFinder.FindPath(Player.Instance.currentTile, hit.transform.GetComponent<CurrentTile>().tile, rangeFinderTiles);
                }
            }
        }
        else
        {
            if (decal.activeInHierarchy) decal.SetActive(false);
        }
        if (path.Count > 0)
        {
            MoveAlongPath();
            foreach (var tile in rangeFinderTiles)
            {
                tile.currentTile.GetComponent<Collider>().enabled = false;
                tile.currentTile.transform.GetChild(0).gameObject.SetActive(false);
            }
            rangeFinderTiles.Clear();
        }

    }

    private void MoveAlongPath()
    {
        var step = speed * Time.deltaTime;

        float yIndex = path[0].y;
        character.transform.position = Vector3.MoveTowards(character.transform.position, path[0].realPos, step);
        character.transform.position = new Vector3(character.transform.position.x, yIndex, character.transform.position.z);
        if (Vector3.Distance(character.transform.position, path[0].realPos) < 0.1f)
        {
            PositionCharacterOnLine(path[0]);
            path.RemoveAt(0);
        }
    }
    private void PositionCharacterOnLine(Tile tile)
    {
        character.currentTile.state = character.previousTileState;
        character.currentTile = tile;
        character.previousTileState = tile.state;
        character.currentTile.state = Tile.TileTerrain.HAS_A_UNIT_ON;
        character.transform.position = tile.realPos;
    }

    private void GetInRangeTile()
    {
        rangeFinderTiles = rangeFinder.GetTilesInRange(new Vector2Int(Player.Instance.currentTile.x, Player.Instance.currentTile.z), range);
    }

    private void DisplayPossibleTile()
    {
        {
            foreach (var tile in rangeFinderTiles)
            {
                tile.currentTile.GetComponent<Collider>().enabled = true;
                tile.currentTile.transform.GetChild(0).gameObject.SetActive(true);
            }
        }
    }
}
