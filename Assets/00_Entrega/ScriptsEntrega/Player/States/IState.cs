using UnityEngine;

public interface IState
{
    void Enter();
    void Exit();
    void HandleInput();   // leer input / se�ales
    void LogicUpdate();   // decisiones, cambios de estado
    void PhysicsUpdate(); // movimiento/rotaci�n
}
