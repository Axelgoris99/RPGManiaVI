using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BattleState { START, PLAYER_TURN, ENNEMY_TURN, WON, LOST }
public class FightManager : MonoBehaviour
{
    public BattleState state;

    public delegate void SimpleEvent();
    public static event SimpleEvent onFightStarted;
    public static event SimpleEvent onPlayerTurnBegin;
    public static event SimpleEvent onEnemyTurnBegin;
    public static event SimpleEvent onFightOver;

    

    #region Singleton
    private static FightManager instance;
    public static FightManager Instance { get { return instance; } }

    private void Awake()
    {
        // Ensure there's only one instance of the controller
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion
    private void OnEnable()
    {
        GeneralStateMachine.onStateChanged += HandleState;
        GridManager.onGridGenerated += StartBattle;
        ActionManager.onEndTurn += EndTurn;
    }
    private void OnDisable()
    {
        GeneralStateMachine.onStateChanged -= HandleState;
        GridManager.onGridGenerated -= StartBattle;
        ActionManager.onEndTurn -= EndTurn;
    }
    private void HandleState(GameState newState)
    {
        if (newState == GameState.Fighting)
        {
            state = BattleState.START;
            onFightStarted?.Invoke();
        }
    }

    private void EndTurn()
    {
        if (state == BattleState.PLAYER_TURN)
        {
            state = BattleState.ENNEMY_TURN;
            onEnemyTurnBegin?.Invoke();
        }
        else if (state == BattleState.ENNEMY_TURN)
        {
            state = BattleState.PLAYER_TURN;
            onPlayerTurnBegin?.Invoke();
        }
    }

    private void StartBattle()
    {
        if (state == BattleState.START)
        {
            state = BattleState.PLAYER_TURN;
            onPlayerTurnBegin?.Invoke();
        }
    }   

}


