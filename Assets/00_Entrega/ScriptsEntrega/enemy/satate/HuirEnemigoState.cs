using UnityEngine;

/// <summary>
/// Estado de huida: corre en dirección contraria al jugador y evita obstáculos por raycast.
/// Continúa mientras el sensor lo tenga (visión/memoria). Cuando lo olvida, corre un extra y vuelve a Patrulla.
/// </summary>
public class HuirEnemigoState : EstadoEnemigo<EnemyStates>
{
    private readonly EnemigoModel model;
    private float tiempoExtra; // tiempo acumulado luego de que el sensor "olvida" al jugador

    public HuirEnemigoState(EnemigoModel model)
    {
        this.model = model;
    }

    public override void Enter()
    {
        if (model.HabilitarLogs) Debug.Log("[Enemigo] Enter Huir");
        tiempoExtra = 0f;
    }

    public override void Execute()
    {
        // Control de memoria + extra:
        if (model.Sensor == null)
        {
            // Sin sensor: huir un toque y volver
            tiempoExtra += Time.deltaTime;
            if (tiempoExtra >= model.TiempoHuidaExtra)
            {
                fsm.SetState(EnemyStates.Patrulla);
                return;
            }
        }
        else
        {
            if (model.Sensor.ObjetivoVisible)
            {
                // Mientras lo vea o lo recuerde, resetea el extra
                tiempoExtra = 0f;
            }
            else
            {
                // El sensor ya "olvidó": empezar a contar extra
                tiempoExtra += Time.deltaTime;
                if (tiempoExtra >= model.TiempoHuidaExtra)
                {
                    fsm.SetState(EnemyStates.Patrulla);
                    return;
                }
            }
        }

        // Movimiento de huida
        Vector3 dir = Vector3.zero;
        if (model.Sensor != null && model.Sensor.ObjetivoActual != null)
        {
            dir = (model.transform.position - model.Sensor.ObjetivoActual.position);
            dir.y = 0f;
            if (dir.sqrMagnitude > 0.0001f) dir.Normalize();
        }

        // Evitación simple
        Vector3 deseada = dir * model.VelocidadHuida;
        Vector3 evit = model.CalcularEvitacion(deseada);
        Vector3 final = deseada + evit;
        if (final.sqrMagnitude > 0.0001f) final.Normalize();

        model.MoverXZ(final, model.VelocidadHuida);
        model.MirarHacia(final);
    }

    public override void Exit()
    {
        if (model.HabilitarLogs) Debug.Log("[Enemigo] Exit Huir → Patrulla");
        model.MoverXZ(Vector3.zero, 0f);
    }
}
