using UnityEngine;

public class WalkState : State
{
    public WalkState(PlayerController player, FSM fsm, PlayerModel model, PlayerView view) : base(player, fsm, model, view) { }

    public override void Enter() => view.PlayWalk();

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
        if (!moving)
        {
            fsm.ChangeState(player.Idle);
            return;
        }

        bool runKey = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        if (runKey) fsm.ChangeState(player.Run);
    }

    public override void PhysicsUpdate()
    {
        bool moving = model.InputVector.sqrMagnitude > 0.01f;
        model.StepFactor(moving);

        Vector3 worldDir = player.ToCameraSpace(model.InputVector);
        float speed = model.GetSpeed(false);
        player.Move(worldDir, speed);

        view.SetSpeedParam(model.speedFactor * 3f);
    }
}
