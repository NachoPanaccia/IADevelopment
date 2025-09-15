using UnityEngine;

/// <summary>
/// Estado de ataque: persigue al jugador con steering behaviour (Seek/Pursuit).
/// Si llega a distancia de ataque, ejecuta la acción de “matar jugador” (termina el juego).
/// Requiere: EnemigoModel con Sensor configurado y métodos MoverXZ/MirarHacia/CalcularEvitacion/VelocidadAtaque.
/// </summary>
public class AttackEnemigoState : EstadoEnemigo<EnemyStates>
{
    private readonly EnemigoModel modelo;
    [SerializeField] private float distanciaAtaque = 2f;   // configurable si lo exponés
    [SerializeField] private float prediccionTiempo = 0.5f; // tiempo de look-ahead para Pursuit

    public AttackEnemigoState(EnemigoModel modelo, float distanciaAtaque = 2f, float prediccionTiempo = 0.5f)
    {
        this.modelo = modelo;
        this.distanciaAtaque = distanciaAtaque;
        this.prediccionTiempo = prediccionTiempo;
    }

    public override void Enter()
    {
        if (modelo.HabilitarLogs) Debug.Log("[Enemigo] Enter Attack");
    }

    public override void Execute()
    {
        if (modelo.Sensor == null || modelo.Sensor.ObjetivoActual == null)
        {
            // Si no hay objetivo, o se perdió, volvemos a patrulla
            fsm.SetState(EnemyStates.Patrulla);
            return;
        }

        Transform objetivo = modelo.Sensor.ObjetivoActual;

        // --- Pursuit: predecir la posición futura del jugador usando su velocidad ---
        Vector3 posPrevista = objetivo.position;
        Rigidbody rbJugador = objetivo.GetComponent<Rigidbody>();
        if (rbJugador != null)
        {
            posPrevista += rbJugador.linearVelocity * prediccionTiempo;
        }

        // Dirección deseada (Seek hacia la posición prevista)
        Vector3 dir = (posPrevista - modelo.transform.position);
        dir.y = 0f;
        if (dir.sqrMagnitude > 0.0001f) dir.Normalize();

        Vector3 deseada = dir * modelo.VelocidadAtaque;
        Vector3 evit = modelo.CalcularEvitacion(deseada); // si está desactivada, devuelve Vector3.zero
        Vector3 final = deseada + evit;
        if (final.sqrMagnitude > 0.0001f) final.Normalize();

        // Aplicar movimiento
        modelo.MoverXZ(final, modelo.VelocidadAtaque);
        modelo.MirarHacia(final);

        // ¿Está a distancia de ataque?
        float dist = Vector3.Distance(
            new Vector3(modelo.transform.position.x, 0f, modelo.transform.position.z),
            new Vector3(posPrevista.x, 0f, posPrevista.z));

        if (dist <= distanciaAtaque)
        {
            if (modelo.HabilitarLogs) Debug.Log("[Enemigo] Atacó al jugador. GAME OVER.");

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false; // detener play en editor
#else
            Application.Quit(); // cerrar app en build
#endif
        }
    }

    public override void Exit()
    {
        if (modelo.HabilitarLogs) Debug.Log("[Enemigo] Exit Attack");
        modelo.MoverXZ(Vector3.zero, 0f);
    }
}
