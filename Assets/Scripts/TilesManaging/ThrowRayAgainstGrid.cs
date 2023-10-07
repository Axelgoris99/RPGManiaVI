using finished3;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

    private PathFinder pathFinder;
    private List<Tile> path;

    private RangeFinder rangeFinder;
    private List<Tile> rangeFinderTiles;
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
            Tile tile = hit.transform.GetComponent<CurrentTile>().tile;
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
            if(decal.activeInHierarchy) decal.SetActive(false);
        }
        if (path.Count > 0)
        {
            MoveAlongPath();
            rangeFinderTiles.Clear();
        }

    }

    private void MoveAlongPath()
    {
        var step = speed * Time.deltaTime;

        float yIndex = path[0].y;
        character.transform.position = Vector3.MoveTowards(character.transform.position, path[0].realPos, step);
        character.transform.position = new Vector3(character.transform.position.x, yIndex, character.transform.position.z);
        if (Vector3.Distance(character.transform.position, path[0].realPos) < 0.00001f)
        {
            PositionCharacterOnLine(path[0]);
            path.RemoveAt(0);
        }
    }
    private void PositionCharacterOnLine(Tile tile)
    {
        character.transform.position = tile.realPos;
        character.currentTile = tile;
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
                tile.currentTile.GetComponent<MeshRenderer>().material.color = Color.red;
                tile.currentTile.GetComponent<MeshRenderer>().enabled = true;
            }
        }
    }
}
