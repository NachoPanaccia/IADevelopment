using UnityEngine;

public class IdleState : State
{
    public IdleState(PlayerController c, FSM f, PlayerModel m, PlayerView v) : base(c, f, m, v) { }

    public override void Enter() { view?.PlayIdle(); }

    public override void Execute()
    {
        if (Input.GetMouseButtonDown(0)) { fsm.ChangeState(controller.Punch); return; }

        var input = controller.ReadMovementInput();
        if (input.sqrMagnitude > 0.001f)
        {
            bool run = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
            fsm.ChangeState(run ? controller.Run : controller.Walk);
        }
    }

    public override void FixedExecute()
    {
        // frenar drift
        controller.Move(Vector3.zero, 0f);
    }
}
