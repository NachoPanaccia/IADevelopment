using UnityEngine;

public class RunState : State
{
    public RunState(PlayerController player, FSM fsm, PlayerModel model, PlayerView view) : base(player, fsm, model, view) { }

    public override void Enter() => view.PlayRun();

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
        bool runKey = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

        if (!moving)
        {
            fsm.ChangeState(player.RunToStop);
            return;
        }

        if (moving && !runKey)
        {
            fsm.ChangeState(player.Walk);
            return;
        }
    }

    public override void PhysicsUpdate()
    {
        bool moving = model.InputVector.sqrMagnitude > 0.01f;
        model.StepFactor(moving);

        Vector3 worldDir = player.ToCameraSpace(model.InputVector);
        float speed = model.GetSpeed(true);
        player.Move(worldDir, speed);

        view.SetSpeedParam(model.speedFactor * 3f);
    }
}
