using UnityEngine;

public class RunState : State
{
    Vector3 moveDir;

    public RunState(PlayerController c, FSM f, PlayerModel m, PlayerView v) : base(c, f, m, v) { }

    public override void Enter() { view?.PlayRun(); }

    public override void Execute()
    {
        if (Input.GetMouseButtonDown(0))
        { 
            fsm.ChangeState(controller.Punch); 
            return; 
        }

        var input = controller.ReadMovementInput();
        bool shift = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

        if (input.sqrMagnitude < 0.001f)
        { 
            fsm.ChangeState(controller.RunToStop);
            return; 
        }

        if (!shift)
        { 
            fsm.ChangeState(controller.Walk); 
            return; 
        }

        moveDir = controller.ToCameraSpace(input);
    }

    public override void FixedExecute()
    {
        controller.Move(moveDir, model.runSpeed);
    }
}
