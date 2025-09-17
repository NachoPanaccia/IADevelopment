using UnityEngine;


/// Estado de huida: corre en dirección contraria al jugador y evita obstáculos
/// Corre cuando lo ve y cando lo olvida, corre un extra y vuelve a Patrulla
public class HuirEnemigoState : EstadoEnemigo<EnemyStates>
{
    private readonly EnemigoModel model;
    private float tiempoExtra; // tiempo acumulado luego de que el sensor olvida al jugador

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
        //Control de memoria + tiempo extra 
        if (model.Sensor == null)
        {
            
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
                // Mientras lo vea, no sumamos extra 
                tiempoExtra = 0f;
            }
            else
            {
                // Ya no lo ve escapamos un poco mas 
                tiempoExtra += Time.deltaTime;
                if (tiempoExtra >= model.TiempoHuidaExtra)
                {
                    fsm.SetState(EnemyStates.Patrulla);
                    return;
                }
            }
        }

        // Flee Corremos en direccion contraria
        Vector3 dir = Vector3.zero;
        if (model.Sensor != null && model.Sensor.ObjetivoActual != null)
        {
            // vector desde el jugador al enemy 
            dir = (model.transform.position - model.Sensor.ObjetivoActual.position);
            dir.y = 0f; // bloqueamos eje Y 
            if (dir.sqrMagnitude > 0.0001f) dir.Normalize();
        }

        // Evitamos los obstaculos 
        Vector3 deseada = dir * model.VelocidadHuida;       // velocidad Flee
        Vector3 evit = model.CalcularEvitacion(deseada);    // corrección de obstáculos
        Vector3 final = deseada + evit;                     // mezcla ambas
        if (final.sqrMagnitude > 0.0001f) final.Normalize();

        // Aplicamos movimiento y rotamos para q vea para el lado correcto
        model.MoverXZ(final, model.VelocidadHuida); // mueve en XZ con velocidad de huida
        model.MirarHacia(final);                    // rota hacia donde corre
    }

    public override void Exit()
    {
        if (model.HabilitarLogs) Debug.Log("[Enemigo] Exit Huir → Patrulla");
        model.MoverXZ(Vector3.zero, 0f); // frenamos 
    }
}
