using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class ActionManager : MonoBehaviour
{
    // assign the actions asset to this field in the inspector:
    public InputActionAsset actions;

    // private field to store move action reference
    private InputAction moveAction;
    private InputAction useAction;
    private InputAction turnCameraRight;
    private InputAction turnCameraLeft;

    // Delegate and event
    public delegate void SimpleTrigger();
    public static event SimpleTrigger onRotateLeft;
    public static event SimpleTrigger onRotateRight;
    public static event SimpleTrigger onUse;
    public static event SimpleTrigger onSelectInteractable;
    public static event SimpleTrigger onEndTurn;

    public delegate void MoveEvent(Vector2 moveVector);
    public static event MoveEvent onMove;


    void Awake()
    {
        // find the "move" action, and keep the reference to it, for use in Update
        moveAction = actions.FindActionMap("gameplay").FindAction("Move");

        // for the "jump" action, we add a callback method for when it is performed
        actions.FindActionMap("gameplay").FindAction("TurnCameraLeft").performed += OnTurnCameraLeft;
        actions.FindActionMap("gameplay").FindAction("TurnCameraRight").performed += OnTurnCameraRight;

        // for the use action
        actions.FindActionMap("gameplay").FindAction("Use").performed += OnUse;
        actions.FindActionMap("gameplay").FindAction("ChangeObject").performed += OnSelectInteractable;
        actions.FindActionMap("gameplay").FindAction("EndTurn").performed += OnEndTurn;
    }

    private void OnTurnCameraRight(InputAction.CallbackContext obj)
    {
        onRotateRight?.Invoke();
    }
    private void OnTurnCameraLeft(InputAction.CallbackContext obj)
    {
        onRotateLeft?.Invoke();
    }

    void Update()
    {
        // our update loop polls the "move" action value each frame
        onMove?.Invoke(moveAction.ReadValue<Vector2>());
    }

    private void OnUse(InputAction.CallbackContext obj)
    {
        onUse?.Invoke();
    }

    private void OnSelectInteractable(InputAction.CallbackContext obj)
    {
        onSelectInteractable?.Invoke();
    }

    private void OnEndTurn(InputAction.CallbackContext obj)
    {
        onEndTurn?.Invoke();
    }

    void OnEnable()
    {
        actions.FindActionMap("gameplay").Enable();
    }
    void OnDisable()
    {
        actions.FindActionMap("gameplay").Disable();
    }
}

