using UnityEngine;

public class RunToStopState : State
{
    public RunToStopState(PlayerController player, FSM fsm, PlayerModel model, PlayerView view)
        : base(player, fsm, model, view) { }

    public override void Enter()
    {
        // Arranca la animación de frenado y vamos bajando la “inercia” suavemente
        model.StepFactor(false);
        player.Move(Vector3.zero, 0f);
        view.PlayRunToStop();
    }

    public override void HandleInput()
    {
        // Pre-leemos input para decidir a dónde ir cuando termine el clip
        model.InputVector = player.ReadMovementInput();
    }

    public override void LogicUpdate()
    {
        // Sale SOLO cuando la animación terminó por completo (no loopeada)
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
        view.SetSpeedParam(model.speedFactor * 3f); // si tenés el parámetro "Speed"
        player.Move(Vector3.zero, 0f);
    }
}
