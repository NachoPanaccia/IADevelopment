using System.Collections.Generic;
using UnityEngine;


// EstadoEnemigo.cs
public abstract class EstadoEnemigo<T> : IEstadoEnemigo<T>
{
    protected FSMEnemigo<T> fsm;
    private Dictionary<T, IEstadoEnemigo<T>> transiciones = new();

    public virtual void Enter() { }
    public virtual void Execute() { }
    public virtual void Exit() { }

    public void SetFSM(FSMEnemigo<T> f) => fsm = f;
    public void AddTransition(T input, IEstadoEnemigo<T> state) => transiciones.TryAdd(input, state);
    public bool GetState(T input, out IEstadoEnemigo<T> state) => transiciones.TryGetValue(input, out state);
}

