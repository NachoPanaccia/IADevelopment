using UnityEngine;

public class OldSpinPlayerState : OldIState
{
    ISpin _spin;
    public OldSpinPlayerState(ISpin spin)
    {
        _spin = spin;
    }

    public void Enter()
    {
        _spin.Spin();   
    }

    public void Execute()
    {
    }

    public void Exit()
    {
        _spin.Spin();

    }
}
