using UnityEngine;

public interface IState
{
    void Enter();
    void Exit();
    void HandleInput();   // leer input / señales
    void LogicUpdate();   // decisiones, cambios de estado
    void PhysicsUpdate(); // movimiento/rotación
}
