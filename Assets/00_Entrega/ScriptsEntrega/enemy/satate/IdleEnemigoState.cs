using UnityEngine;

// Estado de idle: frenamos donde estamos pas un tiempo y volvemos a movermos
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
        temporizador = 0f; // arrancamos a contar desde cero
        modelo.MoverXZ(Vector3.zero, 0f); // se queda quieto
        if (modelo.HabilitarLogs) Debug.Log("[Enemigo] Enter Idle");
    }

    public override void Execute()
    {
        // si el sensor ve al jugador, tiramos un random y elegimos
        // hacemos esto para decidir si vamos a atacar o vamos a huir
        if (modelo.Sensor != null && modelo.Sensor.ObjetivoVisible)
        {
            int tiro = Random.Range(0, 1000000);
            bool esPar = (tiro % 2) == 0;
            if (modelo.HabilitarLogs) Debug.Log($"[Enemigo][Idle] Detectó jugador. Random={tiro} ? {(esPar ? "Huir" : "Attack")}");

            fsm.SetState(esPar ? EnemyStates.Huir : EnemyStates.Attack);
            return;
        }

        // si no hay jugador, sumamos tiempo
        temporizador += Time.deltaTime;
        if (temporizador >= tiempoIdle)
        {
            // cuando se cumple el tiempo, vuelve a patrulla
            fsm.SetState(EnemyStates.Patrulla);
        }
    }

    public override void Exit()
    {
        if (modelo.HabilitarLogs) Debug.Log("[Enemigo] Exit Idle");
    }
}
