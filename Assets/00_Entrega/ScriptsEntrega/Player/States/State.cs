using UnityEngine;

/// Base común para todos los estados de la FSM.
// da acceso a controller/model/view
public abstract class State : IState
{
    protected readonly PlayerController controller;
    protected readonly FSM fsm;
    protected readonly PlayerModel model;
    protected readonly PlayerView view;

    protected State(PlayerController controller, FSM fsm, PlayerModel model, PlayerView view)
    {
        this.controller = controller;
        this.fsm = fsm;
        this.model = model;
        this.view = view;
    }

    public virtual void Enter() { }
    public abstract void Execute();
    public virtual void FixedExecute() { }
}
