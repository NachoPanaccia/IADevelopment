using UnityEngine;

/// <summary>
/// Estado de patrulla: recorre puntos en ping-pong o loop.
/// Si el sensor ve al jugador, transiciona a Huir.
/// Pasa a Idle luego de alcanzar 'puntosAntesDeIdle' puntos de patrulla (eventos reales, NO frames).
/// Requiere: EnemigoModel con puntosPatrulla, Sensor y helpers de movimiento.
/// </summary>
public class PatrullaEnemigoState : EstadoEnemigo<EnemyStates>
{
    private readonly EnemigoModel modelo;

    // ✅ Nuevo criterio: cuenta puntos alcanzados, no frames
    private int puntosContados;
    private readonly int puntosAntesDeIdle;

    public PatrullaEnemigoState(EnemigoModel modelo, int puntosAntesDeIdle = 3)
    {
        this.modelo = modelo;
        this.puntosAntesDeIdle = Mathf.Max(1, puntosAntesDeIdle);
    }

    public override void Enter()
    {
        if (modelo.HabilitarLogs) Debug.Log("[Enemigo] Enter Patrulla");

        // Normalizar estado interno
        if (modelo.PuntosPatrulla != null && modelo.PuntosPatrulla.Length > 0)
        {
            modelo.indicePunto = Mathf.Clamp(modelo.indicePunto, 0, modelo.PuntosPatrulla.Length - 1);
        }
        modelo.tiempoEsperaRestante = Mathf.Max(0f, modelo.tiempoEsperaRestante);

        // Reiniciar el contador de "hechos de patrulla"
        puntosContados = 0;

        // Si no hay puntos, quedate en lugar
        if (modelo.PuntosPatrulla == null || modelo.PuntosPatrulla.Length == 0)
        {
            modelo.MoverXZ(Vector3.zero, 0f);
        }
    }

    public override void Execute()
    {
        // 1) Si ve al jugador -> Huir
        if (modelo.Sensor != null && modelo.Sensor.ObjetivoVisible)
        {
            fsm.SetState(EnemyStates.Huir);
            return;
        }

        // 2) Si no hay puntos de patrulla, quedate quieto (no contemos nada)
        if (modelo.PuntosPatrulla == null || modelo.PuntosPatrulla.Length == 0)
        {
            modelo.MoverXZ(Vector3.zero, 0f);
            return;
        }

        // 3) Espera en punto
        if (modelo.tiempoEsperaRestante > 0f)
        {
            modelo.tiempoEsperaRestante -= Time.deltaTime;
            modelo.MoverXZ(Vector3.zero, 0f);
            return;
        }

        // 4) Moverse al punto actual
        Transform punto = modelo.PuntosPatrulla[modelo.indicePunto];
        Vector3 haciaPunto = punto.position - modelo.transform.position;
        Vector3 dir = new Vector3(haciaPunto.x, 0f, haciaPunto.z);

        if (dir.sqrMagnitude > 0.0001f)
            dir.Normalize();

        Vector3 deseada = dir * modelo.VelocidadPatrulla;
        Vector3 evit = modelo.CalcularEvitacion(deseada); // hoy devuelve Vector3.zero
        Vector3 final = deseada + evit;
        if (final.sqrMagnitude > 0.0001f) final.Normalize();

        modelo.MoverXZ(final, modelo.VelocidadPatrulla);
        modelo.MirarHacia(final);

        // 5) ¿Llegamos al punto? (distancia plana XZ)
        float distXZ = Vector3.Distance(
            new Vector3(modelo.transform.position.x, 0, modelo.transform.position.z),
            new Vector3(punto.position.x, 0, punto.position.z));

        if (distXZ <= modelo.DistanciaLlegada)
        {
            // ✅ Contamos un “hecho de patrulla”: punto alcanzado
            puntosContados++;

            // Siguiente punto + espera
            AvanzarIndice();
            modelo.tiempoEsperaRestante = modelo.TiempoEsperaEnPunto;

            // ¿Ya alcanzamos suficientes puntos para “descansar”?
            if (puntosContados >= puntosAntesDeIdle)
            {
                if (modelo.HabilitarLogs) Debug.Log("[Enemigo] Patrulla → Idle (por puntos alcanzados)");
                fsm.SetState(EnemyStates.Idle);
                return;
            }
        }
    }

    // Ping-pong: 0→1→2→1→0…, o loop
    private void AvanzarIndice()
    {
        if (modelo.PuntosPatrulla == null || modelo.PuntosPatrulla.Length <= 1) return;

        if (modelo.HacerPingPong)
        {
            modelo.indicePunto += modelo.direccion;
            if (modelo.indicePunto >= modelo.PuntosPatrulla.Length)
            {
                modelo.indicePunto = modelo.PuntosPatrulla.Length - 2;
                modelo.direccion = -1;
            }
            else if (modelo.indicePunto < 0)
            {
                modelo.indicePunto = 1;
                modelo.direccion = 1;
            }
        }
        else
        {
            modelo.indicePunto = (modelo.indicePunto + 1) % modelo.PuntosPatrulla.Length;
        }
    }
}
