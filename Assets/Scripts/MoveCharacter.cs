using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class MoveCharacter : MonoBehaviour
{
    [SerializeField] private float speedX = 1.0f;
    [SerializeField] private float speedY = 1.0f;
    [SerializeField] private Camera currentCamera;
    private NavMeshAgent agent;
    private GameState currentGameState;

    private void OnEnable()
    {
        currentCamera = Camera.main;
        agent = GetComponent<NavMeshAgent>();
    
        GridManager.onGridGenerated += MoveCharacterToTheClosestTile;
        ActionManager.onMove += MoveCharacterWithInputs;
        GeneralStateMachine.onStateChanged += StateChanged;
        ActionManager.onUse += MoveWithMouse;
    }

    private void OnDisable()
    {
        GridManager.onGridGenerated -= MoveCharacterToTheClosestTile;
        ActionManager.onMove -= MoveCharacterWithInputs;
        GeneralStateMachine.onStateChanged -= StateChanged;
        ActionManager.onUse -= MoveWithMouse;
    }

    private void StateChanged(GameState gs)
    {
        currentGameState = gs;
    }

    private void MoveCharacterWithInputs(Vector2 moveVector)
    {
        if (currentGameState != GameState.Exploring || moveVector == Vector2.zero) return;
        Vector3 directionForward = currentCamera.transform.forward;
        directionForward.y = 0;

        Vector3 directionRight = currentCamera.transform.right;
        directionRight.y = 0;

        Vector3 newPosition = transform.position + directionForward.normalized * moveVector.y * speedY + directionRight.normalized * moveVector.x * speedX;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(newPosition, out hit, 2.0f, NavMesh.AllAreas))
        {
            agent.destination = hit.position;
        }
    }
    private void Update()
    {
        if (currentGameState != GameState.Exploring)
        {
            agent.destination = transform.position;
        };

    }

    private void MoveCharacterToTheClosestTile()
    {
        float distanceToTheClosestTile = Mathf.Infinity;
        Tile closestTile = null;
        Vector3 playerPos = Player.Instance.transform.position;
        foreach (var tile in Grid.tilesGrid)
        {
            if (tile.Value.IsThisTileWalkable())
            {
                float currentDistance = Vector3.Distance(tile.Value.realPos, playerPos);
                if (currentDistance < distanceToTheClosestTile)
                {
                    closestTile = tile.Value;
                    distanceToTheClosestTile = currentDistance;
                }
            }
        }
        transform.position = closestTile.realPos + Vector3.up * 0.1f;
        Player.Instance.currentTile = closestTile;
        Player.Instance.previousTileState = Player.Instance.currentTile.state;
        Player.Instance.currentTile.state = Tile.TileTerrain.HAS_A_UNIT_ON;
    }

    private void MoveWithMouse()
    {
        if (currentGameState == GameState.Exploring)
        {
            Ray ray = MouseInput.Instance.cameraRay;
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100.0f))
            {
                if (NavMesh.SamplePosition(hit.point, out NavMeshHit navHit, 2.0f, NavMesh.AllAreas))
                {
                    agent.destination = navHit.position;
                }

            }
        }
    }
}
