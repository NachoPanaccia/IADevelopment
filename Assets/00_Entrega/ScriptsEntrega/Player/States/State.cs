using UnityEngine;

public abstract class State : IState
{
    protected readonly PlayerController player;
    protected readonly FSM fsm;
    protected readonly PlayerModel model;
    protected readonly PlayerView view;

    protected State(PlayerController player, FSM fsm, PlayerModel model, PlayerView view)
    {
        this.player = player;
        this.fsm = fsm;
        this.model = model;
        this.view = view;
    }

    public virtual void Enter() { }
    public virtual void Exit() { }
    public virtual void HandleInput() { }
    public virtual void LogicUpdate() { }
    public virtual void PhysicsUpdate() { }
}