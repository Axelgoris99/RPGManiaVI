using System.Collections;
using UnityEngine;
public enum GameState
{
    MainMenu,
    Exploring,
    Fighting,
    Paused,
    GameOver
}

public class GeneralStateMachine : MonoBehaviour
{
    #region Singleton
    private static GeneralStateMachine instance;
    public static GeneralStateMachine Instance { get { return instance; } }

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
    public delegate void StateChanged(GameState newState);
    public static event StateChanged onStateChanged;

    public GameState gameState = GameState.MainMenu;

    private GameState currentState;
    public GameState CurrentState
    {
        get { return currentState; }
        set
        {
            currentState = value;
            onStateChanged?.Invoke(currentState);
        }
    }
    private void Start()
    {
        // Set the initial state (e.g., Exploring)
        ChangeToFighting();
     }
    private void Update()
    {
        if(gameState != currentState)
        {
            ChangeState(gameState);
        }
    }

    // Implement methods for state transitions
    public void ChangeToMainMenu()
    {
        ChangeState(GameState.MainMenu);
    }

    public void ChangeToExploring()
    {
        ChangeState(GameState.Exploring);
    }
    public void ChangeToFighting()
    {
        ChangeState(GameState.Fighting);
    }

    public void ChangeToPaused()
    {
        ChangeState(GameState.Paused);
    }

    public void ChangeToGameOver()
    {
        ChangeState(GameState.GameOver);
    }

    private void ChangeState(GameState newState)
    {
        if (currentState != newState)
        {
            // Update the current state
            currentState = newState;
            onStateChanged?.Invoke(currentState);
            gameState = currentState;
        }
    }
}