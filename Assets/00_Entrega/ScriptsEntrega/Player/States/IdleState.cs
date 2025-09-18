using UnityEngine;

public class IdleState : State
{
    public IdleState(PlayerController player, FSM fsm, PlayerModel model, PlayerView view) : base(player, fsm, model, view) { }

    public override void Enter()
    {
        view.PlayIdle();
        view.SetSpeedParam(0f);
        model.InputVector = Vector3.zero;
        model.speedFactor = 0f;
    }

    public override void HandleInput()
    {
        model.InputVector = player.ReadMovementInput();
    }

    public override void LogicUpdate()
    {
        if (Input.GetMouseButtonDown(0))
        {
            fsm.ChangeState(player.Punch);
            return;
        }

        bool moving = model.InputVector.sqrMagnitude > 0.01f;
        if (moving)
        {
            bool runKey = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
            fsm.ChangeState(runKey ? player.Run : player.Walk);
        }
    }
}
