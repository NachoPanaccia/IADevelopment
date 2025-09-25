public class RunToStopState : State
{
    public RunToStopState(PlayerController c, FSM f, PlayerModel m, PlayerView v) : base(c, f, m, v) { }

    public override void Enter() { view?.PlayRunToStop(); }

    public override void Execute()
    {
        // Cuando termina la animación, pasamos a Idle
        if (view != null && view.IsFinished("RunToStop"))
        {
            fsm.ChangeState(controller.Idle);
        }
    }

    public override void FixedExecute()
    {
        controller.Move(UnityEngine.Vector3.zero, 0f); // quieto durante la frenada
    }
}
