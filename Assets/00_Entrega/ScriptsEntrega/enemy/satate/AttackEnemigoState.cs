using UnityEngine;
using UnityEngine.SceneManagement;


/// Estado de ataque: el enemigo persigue al jugador
/// Si llega a estar lo suficientemente cerca se termina el juego

public class AttackEnemigoState : EstadoEnemigo<EnemyStates>
{
    private readonly EnemigoModel modelo;
    private readonly float distanciaAtaque;
    private readonly float prediccionTiempo;

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
        // si perdió la referencia del jugador, volvemos a patrulla
        if (modelo.Sensor == null || modelo.Sensor.ObjetivoActual == null)
        {
            fsm.SetState(EnemyStates.Patrulla);
            return;
        }

        Transform objetivo = modelo.Sensor.ObjetivoActual;

        // pursuit ratamos de adivinar adónde va a estar el jugador
        Vector3 posPrevista = objetivo.position;
        Rigidbody rbJugador = objetivo.GetComponent<Rigidbody>();
        if (rbJugador != null)
        {
            posPrevista += rbJugador.linearVelocity * prediccionTiempo;
        }

        // calculamos la dirección hacia esa posición prevista
        Vector3 dir = (posPrevista - modelo.transform.position);
        dir.y = 0f;
        if (dir.sqrMagnitude > 0.0001f) dir.Normalize();

        // metemos esa dirección en un seek y le sumamos la evitacion de obstáculos
        Vector3 deseada = dir * modelo.VelocidadAtaque;
        Vector3 evit = modelo.CalcularEvitacion(deseada);
        Vector3 final = deseada + evit;
        if (final.sqrMagnitude > 0.0001f) final.Normalize();

        // mover y mirar hacia la dirección final
        modelo.MoverXZ(final, modelo.VelocidadAtaque);
        modelo.MirarHacia(final);

        // chequeamos si ya está lo bastante cerca como para atacar
        float dist = Vector3.Distance(
            new Vector3(modelo.transform.position.x, 0f, modelo.transform.position.z),
            new Vector3(posPrevista.x, 0f, posPrevista.z));

        if (dist <= distanciaAtaque)
        {
            if (modelo.HabilitarLogs) Debug.Log("[Enemigo] Atacó al jugador. GAME OVER.");

            SceneManager.LoadScene("Pantalla_Derrota");
        }
    }

    public override void Exit()
    {
        if (modelo.HabilitarLogs) Debug.Log("[Enemigo] Exit Attack");
        modelo.MoverXZ(Vector3.zero, 0f); // lo frenamos 
    }
}
