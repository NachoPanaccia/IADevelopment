using UnityEngine;

public class SpinPlayerState : IState
{
    ISpin _spin;

    public SpinPlayerState(ISpin spin)
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
