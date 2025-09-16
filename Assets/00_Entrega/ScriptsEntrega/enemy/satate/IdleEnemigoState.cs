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
        modelo.MoverXZ(Vector3.zero, 0f); // detenerse
        if (modelo.HabilitarLogs) Debug.Log("[Enemigo] Enter Idle");
    }

    public override void Execute()
    {
        // Si detecta al jugador, random: par ? Huir, impar ? Attack
        if (modelo.Sensor != null && modelo.Sensor.ObjetivoVisible)
        {
            int tiro = Random.Range(0, 1000000);
            bool esPar = (tiro % 2) == 0;
            if (modelo.HabilitarLogs) Debug.Log($"[Enemigo][Idle] Detectó jugador. Random={tiro} ? {(esPar ? "Huir" : "Attack")}");

            fsm.SetState(esPar ? EnemyStates.Huir : EnemyStates.Attack);
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
