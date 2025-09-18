using UnityEngine;

public class PunchState : State
{
    public PunchState(PlayerController player, FSM fsm, PlayerModel model, PlayerView view) : base(player, fsm, model, view) { }

    public override void Enter()
    {
        model.StepFactor(false);
        player.Move(Vector3.zero);
        view.PlayPunch();
    }

    public override void HandleInput()
    {
        model.InputVector = player.ReadMovementInput();
    }

    public override void LogicUpdate()
    {
        if (view.IsAnimFinished("Punch"))
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
}
