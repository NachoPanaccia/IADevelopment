using UnityEngine;

/// <summary>
/// Patrulla entre puntos. Si detecta al jugador, tira un random:
/// - Par  → Huir
/// - Impar → Attack
/// </summary>
public class PatrullaEnemigoState : EstadoEnemigo<EnemyStates>
{
    private readonly EnemigoModel modelo;
    private readonly int iteracionesParaIdle;
    private int contadorIteraciones;

    public PatrullaEnemigoState(EnemigoModel modelo, int iteracionesParaIdle = 5)
    {
        this.modelo = modelo;
        this.iteracionesParaIdle = Mathf.Max(0, iteracionesParaIdle);
    }

    public override void Enter()
    {
        if (modelo.HabilitarLogs) Debug.Log("[Enemigo] Enter Patrulla");
        contadorIteraciones = 0;
        if (modelo.PuntosPatrulla == null || modelo.PuntosPatrulla.Length == 0) return;
        modelo.indicePunto = Mathf.Clamp(modelo.indicePunto, 0, modelo.PuntosPatrulla.Length - 1);
        modelo.tiempoEsperaRestante = 0f;
    }

    public override void Execute()
    {
        // 1) Si detecta al jugador, decide por random → Huir o Attack
        if (modelo.Sensor != null && modelo.Sensor.ObjetivoVisible)
        {
            int tiro = Random.Range(0, 1000000);
            bool esPar = (tiro % 2) == 0;
            if (modelo.HabilitarLogs) Debug.Log($"[Enemigo][Patrulla] Detectó jugador. Random={tiro} → {(esPar ? "Huir" : "Attack")}");

            fsm.SetState(esPar ? EnemyStates.Huir : EnemyStates.Attack);
            return;
        }

        // 2) Patrullar normalmente
        if (modelo.PuntosPatrulla == null || modelo.PuntosPatrulla.Length == 0)
            return;

        Transform objetivo = modelo.PuntosPatrulla[modelo.indicePunto];
        Vector3 dir = (objetivo.position - modelo.transform.position);
        dir.y = 0f;

        // Llegada al punto
        if (dir.magnitude <= modelo.DistanciaLlegada)
        {
            // esperar
            if (modelo.tiempoEsperaRestante <= 0f)
            {
                contadorIteraciones++;
                AvanzarIndice();
                modelo.tiempoEsperaRestante = modelo.TiempoEsperaEnPunto;
            }
            else
            {
                modelo.tiempoEsperaRestante -= Time.deltaTime;
                modelo.MoverXZ(Vector3.zero, 0f);
            }
        }
        else
        {
            // Movimiento con suavizado + evitación opcional
            Vector3 deseada = dir.normalized * modelo.VelocidadPatrulla;
            Vector3 evit = modelo.CalcularEvitacion(deseada);
            Vector3 final = deseada + evit;
            if (final.sqrMagnitude > 0.0001f) final.Normalize();

            modelo.MoverXZ(final, modelo.VelocidadPatrulla);
            modelo.MirarHacia(final);
        }

        // 3) Pasar a Idle cada X iteraciones
        if (contadorIteraciones >= iteracionesParaIdle && iteracionesParaIdle > 0)
        {
            fsm.SetState(EnemyStates.Idle);
        }
    }

    private void AvanzarIndice()
    {
        if (modelo.PuntosPatrulla.Length <= 1)
            return;

        int i = modelo.indicePunto + modelo.direccion;
        if (modelo.HacerPingPong)
        {
            if (i >= modelo.PuntosPatrulla.Length)
            {
                modelo.direccion = -1; i = modelo.PuntosPatrulla.Length - 2;
            }
            else if (i < 0)
            {
                modelo.direccion = 1; i = 1;
            }
        }
        else
        {
            if (i >= modelo.PuntosPatrulla.Length) i = 0;
            if (i < 0) i = modelo.PuntosPatrulla.Length - 1;
        }
        modelo.indicePunto = Mathf.Clamp(i, 0, modelo.PuntosPatrulla.Length - 1);
    }

    public override void Exit()
    {
        if (modelo.HabilitarLogs) Debug.Log("[Enemigo] Exit Patrulla");
    }
}
