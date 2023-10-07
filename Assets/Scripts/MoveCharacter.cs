using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MoveCharacter : MonoBehaviour
{
    [SerializeField] private float speed = 10.0f;
    [SerializeField] private Camera currentCamera;
    [SerializeField] private Vector2Int startingCase;

    private void OnEnable()
    {
        GridManager.gridGenerated += MoveCharacterToTheClosestCase;
        
    }
    private void OnDisable()
    {
        GridManager.gridGenerated -= MoveCharacterToTheClosestCase;

    }

    private void MoveCharacterToTheClosestCase()
    {
        Vector2Int tile = startingCase;
        while (Grid.tilesGrid[tile].state == Tile.TileTerrain.NOT_WALKABLE)
        {
            tile = tile + new Vector2Int(1, 0);
        }
        transform.position = Grid.tilesGrid[tile].realPos + Vector3.up *0.5f;
        Player.Instance.currentTile = Grid.tilesGrid[tile];
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Z))
        {
            Vector3 direction = currentCamera.transform.forward;
            direction.y = 0;
            direction.Normalize();
            transform.position += direction * speed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.S))
        {
            Vector3 direction = currentCamera.transform.forward;
            direction.y = 0;
            direction.Normalize();
            transform.position += -direction * speed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.Q))
        {
            Vector3 direction = currentCamera.transform.right;
            direction.y = 0;
            direction.Normalize();
            transform.position += -direction * speed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.D))
        {
            Vector3 direction = currentCamera.transform.right;
            direction.y = 0;
            direction.Normalize();
            transform.position += direction * speed * Time.deltaTime;
        }
    }
}