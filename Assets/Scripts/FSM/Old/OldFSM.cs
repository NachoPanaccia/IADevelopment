using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

public class OldFSM
{
    private Dictionary<PlayerStates, OldIState> states = new();

    private OldIState current;

    public void SetStartState(PlayerStates startingInput)
    {
        if (states.TryGetValue(startingInput, out OldIState newState))
        {
            current = newState;
            current.Enter();
        }
    }
    public void SetStartState(OldIState startingState)
    {
        current = startingState;
        current.Enter();
    }

    public void OnUpdate()
    {
        current.Execute();
    }

    public void AddState(PlayerStates input, OldIState state)
    {
        states.TryAdd(input, state);
    }

    public void SetState(PlayerStates input)
    {
        if (states.TryGetValue(input, out OldIState newState))
        {
            if (newState != current)
            {
                current.Exit();
                Debug.Log("Exiting: " + current);
                current = newState;
                current.Enter();
                Debug.Log("Entering: " + current);
            }
        }
    }
}
