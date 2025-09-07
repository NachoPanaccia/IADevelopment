using UnityEngine;

public class SpinPlayerState : State<PlayerStates>
{
    private ISpin _spin;
    public SpinPlayerState(FSM<PlayerStates> fsm, ISpin spin)
    {
        _fsm = fsm;
        _spin = spin;
    }

    public override void Enter()
    {
        base.Enter();
        _spin.Spin();
    }

    public override void Execute()
    {
        base.Execute();

        if (Input.GetKeyDown(KeyCode.Space))
            _fsm.SetState(PlayerStates.Idle);
    }

    public override void Exit()
    {
        base.Exit();
        _spin.Spin();
    }
}
