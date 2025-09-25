public sealed class FSM
{
    public State CurrentState { get; private set; }

    public void Initialize(State start)
    {
        CurrentState = start;
        CurrentState.Enter();
    }

    public void ChangeState(State next)
    {
        if (CurrentState == next) return;
        CurrentState = next;
        CurrentState?.Enter();
    }

    // Se llaman desde PlayerController.Update / FixedUpdate
    public void Execute() => CurrentState?.Execute();
    public void FixedExecute() => CurrentState?.FixedExecute();
}