using UnityEngine;

public class RunToStopState : State
{
    public RunToStopState(PlayerController player, FSM fsm, PlayerModel model, PlayerView view) : base(player, fsm, model, view) { }

    public override void Enter()
    {
        model.StepFactor(false);
        player.Move(Vector3.zero, 0f);
        view.PlayRunToStop();
    }

    public override void HandleInput()
    {
        model.InputVector = player.ReadMovementInput();
    }

    public override void LogicUpdate()
    {
        if (view.IsAnimFinished("RunToStop"))
        {
            bool moving = model.InputVector.sqrMagnitude > 0.01f;
            if (!moving)
            {
                fsm.ChangeState(player.Idle);
                return;
            }

            bool runKey = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
            fsm.ChangeState(runKey ? player.Run : player.Walk);
        }
    }

    public override void PhysicsUpdate()
    {
        model.StepFactor(false);
        view.SetSpeedParam(model.speedFactor * 3f);
        player.Move(Vector3.zero, 0f);
    }
}
