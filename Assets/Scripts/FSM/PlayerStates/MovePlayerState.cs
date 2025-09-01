using UnityEngine;

public class MovePlayerState : IState
{
    IMove _move;

    public MovePlayerState(IMove move)
    {
        _move = move;
    }

    public void Enter()
    {
        
    }

    public void Execute()
    {
        var h = Input.GetAxis("Horizontal");
        var v = Input.GetAxis("Vertical");
        if (h != 0 || v != 0)
        {
            Vector3 dir = new Vector3(h, 0, v);
            _move.Move(dir.normalized);
            _move.Look(dir);
        }
    }

    public void Exit()
    {

    }
}
