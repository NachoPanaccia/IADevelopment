using UnityEngine;

public class RunToStopState : State
{
    public RunToStopState(PlayerController player, FSM fsm, PlayerModel model, PlayerView view)
        : base(player, fsm, model, view) { }

    public override void Enter()
    {
        // Arranca la animaci�n de frenado y vamos bajando la �inercia� suavemente
        model.StepFactor(false);
        player.Move(Vector3.zero, 0f);
        view.PlayRunToStop();
    }

    public override void HandleInput()
    {
        // Pre-leemos input para decidir a d�nde ir cuando termine el clip
        model.InputVector = player.ReadMovementInput();
    }

    public override void LogicUpdate()
    {
        // Sale SOLO cuando la animaci�n termin� por completo (no loopeada)
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
        // Seguir frenando hasta 0 mientras dura el clip
        model.StepFactor(false);
        view.SetSpeedParam(model.speedFactor * 3f); // si ten�s el par�metro "Speed"
        player.Move(Vector3.zero, 0f);
    }
}
