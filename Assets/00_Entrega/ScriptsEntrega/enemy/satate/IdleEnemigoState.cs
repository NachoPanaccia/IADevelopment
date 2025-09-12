using UnityEngine;

public class IdleEnemigoState : EstadoEnemigo<EnemyStates>
{
    private readonly EnemigoModel modelo;
    private readonly float tiempoIdle;
    private float temporizador;

    public IdleEnemigoState(EnemigoModel modelo, float tiempoIdle = 2f)
    {
        this.modelo = modelo;
        this.tiempoIdle = tiempoIdle;
    }

    public override void Enter()
    {
        temporizador = 0f;
        // detener completamente en XZ, manteniendo Y física
        modelo.MoverXZ(Vector3.zero, 0f);
        if (modelo.HabilitarLogs) Debug.Log("[Enemigo] Enter Idle");
        Debug.Log("llegamos aa idle");
    }

    public override void Execute()
    {

        // Si detecta al jugador, pasa a Huir inmediatamente
        if (modelo.Sensor != null && modelo.Sensor.ObjetivoVisible)
        {
            fsm.SetState(EnemyStates.Huir);
            return;
        }

        temporizador += Time.deltaTime;
        if (temporizador >= tiempoIdle)
        {
            fsm.SetState(EnemyStates.Patrulla);
        }
    }

    public override void Exit()
    {
        if (modelo.HabilitarLogs) Debug.Log("[Enemigo] Exit Idle");
    }
}
