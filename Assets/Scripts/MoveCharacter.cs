using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    [SerializeField] private float interactableHoverRadius = 3.0f;

    public delegate void OnFocusChanged(Interactable newFocus);
    public OnFocusChanged onFocusChangedCallback;

    private Interactable focus;  // Our current focus: Item, Enemy etc.
    private Dictionary<string, Interactable> previousHoverObjects;
    private Dictionary<string, Interactable> currentHoverObjects;


    public LayerMask movementMask;      // The ground
    public LayerMask interactionMask;   // Everything we can interact with

    private int currentFocus;


    private void OnEnable()
    {
        currentCamera = Camera.main;
        agent = GetComponent<NavMeshAgent>();

        GridManager.onGridGenerated += MoveCharacterToTheClosestTile;
        ActionManager.onMove += MoveCharacterWithInputs;
        GeneralStateMachine.onStateChanged += StateChanged;
        ActionManager.onUse += MoveWithMouse;
        ActionManager.onSelectInteractable += FocusInteractable;
    }

    private void OnDisable()
    {
        GridManager.onGridGenerated -= MoveCharacterToTheClosestTile;
        ActionManager.onMove -= MoveCharacterWithInputs;
        GeneralStateMachine.onStateChanged -= StateChanged;
        ActionManager.onUse -= MoveWithMouse;
        ActionManager.onSelectInteractable -= FocusInteractable;
    }

    private void Start()
    {
        previousHoverObjects = new Dictionary<string, Interactable>();
        currentHoverObjects = new Dictionary<string, Interactable>();
    }

    private void Update()
    {
        Collider[] hitColliders = Physics.OverlapSphere(Player.Instance.transform.position, interactableHoverRadius, interactionMask);
        foreach (var hitCollider in hitColliders)
        {
            if (previousHoverObjects.ContainsKey(hitCollider.name))
            {
                currentHoverObjects.Add(hitCollider.name, previousHoverObjects[hitCollider.name]);
                previousHoverObjects.Remove(hitCollider.name);
            }
            else
            {
                var interactable = hitCollider.GetComponent<Interactable>();
                currentHoverObjects.Add(hitCollider.name, interactable);
                interactable.IsHover = true;
                if (!interactable.IsFocus)
                {
                    interactable.OnHover();
                }
            }
        }
        foreach (var interactable in previousHoverObjects)
        {
            if (interactable.Value != null)
            {
                interactable.Value.IsHover = false;
                interactable.Value.OnDefocused();
            }
        }
        previousHoverObjects = new Dictionary<string, Interactable>(currentHoverObjects);
        currentHoverObjects.Clear();


    }

    private void FocusInteractable()
    {
        if (previousHoverObjects.Count == 0) return;
        if (currentFocus >= previousHoverObjects.Count) currentFocus = 0;
        SetFocus(previousHoverObjects.ElementAt(currentFocus).Value);
        currentFocus++;
    }

    private void StateChanged(GameState gs)
    {
        currentGameState = gs;
        if (currentGameState != GameState.Exploring)
        {
            agent.isStopped = true;
        }
        else
        {
            agent.isStopped = false;
        }
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
            if (Physics.Raycast(ray, out hit, 100.0f, movementMask))
            {
                if (NavMesh.SamplePosition(hit.point, out NavMeshHit navHit, 2.0f, NavMesh.AllAreas))
                {
                    agent.destination = navHit.position;
                    SetFocus(null);
                }
            }
            if (Physics.Raycast(ray, out hit, 100f, interactionMask))
            {
                if (NavMesh.SamplePosition(hit.point, out NavMeshHit navHit, 2.0f, NavMesh.AllAreas))
                {
                    agent.destination = navHit.position;
                }
                SetFocus(hit.collider.GetComponent<Interactable>());
            }
        }
    }

    void SetFocus(Interactable newFocus)
    {
        if (onFocusChangedCallback != null)
            onFocusChangedCallback.Invoke(newFocus);

        // If our focus has changed
        if (focus != newFocus && focus != null)
        {
            // Let our previous focus know that it's no longer being focused
            focus.OnDefocused();
        }

        // Set our focus to what we hit
        // If it's not an interactable, simply set it to null
        focus = newFocus;

        if (focus != null)
        {
            // Let our focus know that it's being focused
            focus.OnFocused(transform);
        }

    }
}
