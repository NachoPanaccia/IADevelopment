using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

public class FSM
{
    private Dictionary<PlayerStates, IState> states = new();

    private IState current;

    public void SetStartState(PlayerStates startingInput)
    {
        if (states.TryGetValue(startingInput, out IState newState))
        {
            current = newState;
            current.Enter();
        }
    }

    public void SetStartState(IState startingState)
    {
        current = startingState;
        current.Enter();
    }

    public void OnUpdate()
    {
        current.Execute();
    }

    public void AddState(PlayerStates input, IState state)
    {
        states.TryAdd(input, state);
    }

    public void SetState(PlayerStates input)
    {
        if (states.TryGetValue(input, out IState newState))
        {
            if (newState != current)
            {
                current.Exit();
                Debug.Log("Exiting:" + current);
                current = newState;
                current.Enter();
                Debug.Log("Entering:" + current);
            }
        }
    }
}
