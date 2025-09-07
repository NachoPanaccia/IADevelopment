using UnityEngine;

public class OldIdlePlayerState : OldIState
{
    IMove _move;
    public OldIdlePlayerState(IMove move)
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
