using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightManager : MonoBehaviour
{
    public delegate void SimpleEvent();
    public static event SimpleEvent onFightStarted;
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
        GeneralStateMachine.onStateChanged += handleState;
    }
    private void OnDisable()
    {
        GeneralStateMachine.onStateChanged -= handleState;
    }
    private void handleState(GameState newState)
    {
        if(newState == GameState.Fighting)
        {
            onFightStarted?.Invoke();
        }
    }

}
