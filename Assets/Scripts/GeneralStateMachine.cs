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

    private GameState currentState;
    private void Start()
    {
        // Set the initial state (e.g., Exploring)
        ChangeToFighting();
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
        // Update the current state
        currentState = newState;

        onStateChanged?.Invoke(currentState);
    }
}