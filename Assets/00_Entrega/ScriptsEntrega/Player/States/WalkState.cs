using UnityEngine;

public class WalkState : State
{
    public WalkState(PlayerController player, FSM fsm, PlayerModel model, PlayerView view) : base(player, fsm, model, view) { }

    public override void Enter()
    {
        view.PlayWalk();
    }

    public override void HandleInput()
    {
        model.InputVector = player.ReadMovementInput();
    }

    public override void LogicUpdate()
    {
        if (model.InputVector.sqrMagnitude < 0.01f)
            fsm.ChangeState(player.Idle);
    }

    public override void PhysicsUpdate()
    {
        // Movimiento en espacio de c�mara (opcional)
        Vector3 worldDir = player.ToCameraSpace(model.InputVector);
        player.Move(worldDir);

        // �til para blend tree si despu�s lo us�s
        view.SetSpeedParam(model.walkSpeed);
    }
}
