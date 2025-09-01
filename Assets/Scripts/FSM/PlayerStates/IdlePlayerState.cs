using UnityEngine;

public class IdlePlayerState : IState
{
    IMove _move;

    public IdlePlayerState(IMove move)
    {
        _move = move;
    }

    public void Enter()
    {
        _move.Move(Vector3.zero);
    }

    public void Execute()
    {

    }

    public void Exit()
    {

    }
}