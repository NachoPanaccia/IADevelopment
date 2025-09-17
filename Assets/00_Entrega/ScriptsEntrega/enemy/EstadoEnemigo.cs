using System.Collections.Generic;
using UnityEngine;

// Clase que heredan los States
public abstract class EstadoEnemigo<T> : IEstadoEnemigo<T>
{
   
    protected FSMEnemigo<T> fsm;

    
    private Dictionary<T, IEstadoEnemigo<T>> transiciones = new();

    // estos son los tres métodos base de cualquier estado
    public virtual void Enter() { }   // lo que pasa cuando entra al estado
    public virtual void Execute() { } // lo que hace cada frame estando en el estado
    public virtual void Exit() { }    // lo que pasa cuando sale del estado
    public void SetFSM(FSMEnemigo<T> f) => fsm = f;  
    public void AddTransition(T input, IEstadoEnemigo<T> state) => transiciones.TryAdd(input, state);
    public bool GetState(T input, out IEstadoEnemigo<T> state) => transiciones.TryGetValue(input, out state);
}

