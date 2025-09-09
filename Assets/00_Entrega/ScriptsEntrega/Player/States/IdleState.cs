using UnityEngine;

public class IdleState : State
{
    public IdleState(PlayerController player, FSM fsm, PlayerModel model, PlayerView view) : base(player, fsm, model, view) { }

    public override void Enter()
    {
        view.PlayIdle();
        view.SetSpeedParam(0f);
        model.InputVector = Vector3.zero;
    }

    public override void HandleInput()
    {
        model.InputVector = player.ReadMovementInput();
    }

    public override void LogicUpdate()
    {
        if (model.InputVector.sqrMagnitude > 0.01f)
            fsm.ChangeState(player.Walk);
    }
}
