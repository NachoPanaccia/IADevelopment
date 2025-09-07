using UnityEngine;

public class IdlePlayerState : State<PlayerStates>
{
    private IMove _move;

    public IdlePlayerState(FSM<PlayerStates> fsm, IMove move)
    {
        _fsm = fsm;
        _move = move;
    }

    public override void Enter()
    {
        base.Enter();
        _move.Move(Vector3.zero);
    }

    public override void Execute()
    {
        base.Execute();
        var h = Input.GetAxis("Horizontal");
        var v = Input.GetAxis("Vertical");
        if (h != 0 || v != 0)
        {
            _fsm.SetState(PlayerStates.Moving);
        }
        else if (Input.GetKeyDown(KeyCode.Space))
            _fsm.SetState(PlayerStates.Spining);
    }
}
