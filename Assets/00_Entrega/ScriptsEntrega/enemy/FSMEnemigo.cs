using UnityEngine;
using System.Collections.Generic;

public class FSMEnemigo<T>
{
    private IEstadoEnemigo<T> estadoActual;

    // Debug opcional
    public string EstadoActualNombre => estadoActual != null ? estadoActual.GetType().Name : "NULL";

    public void SetInitialState(IEstadoEnemigo<T> inicial)
    {
        estadoActual = inicial;
        if (estadoActual != null) estadoActual.Enter();
        else Debug.LogError("[FSMEnemigo] SetInitialState recibió NULL.");
    }

    public void OnUpdate()
    {
        // ✅ Evita NullReference si aún no hay estado inicial
        if (estadoActual == null) return;
        estadoActual.Execute();
    }

    public void SetState(T input)
    {
        if (estadoActual == null)
        {
            Debug.LogWarning("[FSMEnemigo] SetState llamado sin estadoActual. Ignorando.");
            return;
        }

        if (estadoActual.GetState(input, out IEstadoEnemigo<T> siguiente) && siguiente != null)
        {
            estadoActual.Exit();
            estadoActual = siguiente;
            estadoActual.Enter();
        }
        // Si no existe transición para ese input, simplemente no cambia.
    }
}

public interface IEstadoEnemigo<T>
{
    void Enter();
    void Execute();
    void Exit();

    void AddTransition(T input, IEstadoEnemigo<T> state);
    bool GetState(T input, out IEstadoEnemigo<T> state);
}
