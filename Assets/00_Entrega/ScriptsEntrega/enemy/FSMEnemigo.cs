using UnityEngine;

// FSMEnemigo.cs
public class FSMEnemigo<T>
{
    IEstadoEnemigo<T> estadoActual;

    public void SetInitialState(IEstadoEnemigo<T> inicial) { }
    public void OnUpdate() { estadoActual.Execute(); }
    public void SetState(T input) { }
}

public interface IEstadoEnemigo<T>
{
    void Enter();
    void Execute();
    void Exit();
    void AddTransition(T input, IEstadoEnemigo<T> state);
    bool GetState(T input, out IEstadoEnemigo<T> state);
}
