using UnityEngine;

public class PunchState : State
{
    public PunchState(PlayerController c, FSM f, PlayerModel m, PlayerView v) : base(c, f, m, v) { }

    public override void Enter() { view?.PlayPunch(); }

    public override void Execute()
    {
        if (view != null && view.IsFinished("Punch"))
        {
            var input = controller.ReadMovementInput();
            if (input.sqrMagnitude < 0.001f)
            { 
                fsm.ChangeState(controller.Idle);
                return; 
            }

            bool run = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
            fsm.ChangeState(run ? controller.Run : controller.Walk);
        }
    }

    public override void FixedExecute()
    {
        controller.Move(Vector3.zero, 0f);
    }
}
