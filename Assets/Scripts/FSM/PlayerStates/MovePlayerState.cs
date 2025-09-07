using UnityEngine;

public class MovePlayerState : State<PlayerStates>
{
    private IMove _move;

    public MovePlayerState(FSM<PlayerStates> fsm, IMove move)
    {
        _fsm = fsm;
        _move = move;
    }

    public override void Execute()
    {
        base.Execute();

        var h = Input.GetAxis("Horizontal");
        var v = Input.GetAxis("Vertical");
        if (h != 0 || v != 0)
        {
            Vector3 dir = new Vector3(h, 0, v);
            _move.Move(dir.normalized);
            _move.Look(dir);
        }
        else
            _fsm.SetState(PlayerStates.Idle);

        if (Input.GetKeyDown(KeyCode.Space))
            _fsm.SetState(PlayerStates.Spining);
    }
}
