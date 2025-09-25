using UnityEngine;

public class WalkState : State
{
    Vector3 moveDir;

    public WalkState(PlayerController c, FSM f, PlayerModel m, PlayerView v) : base(c, f, m, v) { }

    public override void Enter() { view?.PlayWalk(); }

    public override void Execute()
    {
        if (Input.GetMouseButtonDown(0)) { fsm.ChangeState(controller.Punch); return; }

        var input = controller.ReadMovementInput();
        if (input.sqrMagnitude < 0.001f) { fsm.ChangeState(controller.Idle); return; }

        bool run = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        if (run) { fsm.ChangeState(controller.Run); return; }

        moveDir = controller.ToCameraSpace(input);
    }

    public override void FixedExecute()
    {
        controller.Move(moveDir, model.walkSpeed);
    }
}
