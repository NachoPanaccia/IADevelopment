using UnityEngine;


/// <summary>
/// Estado de patrulla: recorre puntos en ping-pong. 
/// Si el sensor ve al jugador, transiciona a Huir.
/// Requiere: EnemigoModel con puntosPatrulla, Sensor y helpers de movimiento.
/// </summary>
public class PatrullaEnemigoState : EstadoEnemigo<EnemyStates>
{
    private readonly EnemigoModel model;

    public PatrullaEnemigoState(EnemigoModel model)
    {
        this.model = model;
    }

    public override void Enter()
    {
        if (model.HabilitarLogs) Debug.Log("[Enemigo] Enter Patrulla");
        if (model.PuntosPatrulla == null || model.PuntosPatrulla.Length == 0) return;

        // Normalizar estado interno
        model.indicePunto = Mathf.Clamp(model.indicePunto, 0, model.PuntosPatrulla.Length - 1);
        model.tiempoEsperaRestante = Mathf.Max(0f, model.tiempoEsperaRestante);
    }

    public override void Execute()
    {
        // 1) Si ve/recuerda al jugador -> Huir
        if (model.Sensor != null && model.Sensor.ObjetivoVisible)
        {
            fsm.SetState(EnemyStates.Huir);
            return;
        }

        // 2) Patrullaje básico
        if (model.PuntosPatrulla == null || model.PuntosPatrulla.Length == 0)
        {
            model.MoverXZ(Vector3.zero, 0f);
            return;
        }

        // Espera opcional en cada punto
        if (model.tiempoEsperaRestante > 0f)
        {
            model.tiempoEsperaRestante -= Time.deltaTime;
            model.MoverXZ(Vector3.zero, 0f);
            return;
        }

        Transform punto = model.PuntosPatrulla[model.indicePunto];
        Vector3 haciaPunto = punto.position - model.transform.position;
        Vector3 dir = new Vector3(haciaPunto.x, 0f, haciaPunto.z).normalized;

        // Evitación simple por raycast (definida en EnemigoModel)
        Vector3 deseada = dir * model.VelocidadPatrulla;
        Vector3 evit = model.CalcularEvitacion(deseada);
        Vector3 final = deseada + evit;
        if (final.sqrMagnitude > 0.0001f) final.Normalize();

        model.MoverXZ(final, model.VelocidadPatrulla);
        model.MirarHacia(final);

        // Llegada al punto (en XZ)
        float distXZ = Vector3.Distance(
            new Vector3(model.transform.position.x, 0, model.transform.position.z),
            new Vector3(punto.position.x, 0, punto.position.z));

        if (distXZ <= model.DistanciaLlegada)
        {
            AvanzarIndice();
            model.tiempoEsperaRestante = model.TiempoEsperaEnPunto;
        }
    }

    // Ping-pong: 0→1→2→1→0…
    private void AvanzarIndice()
    {
        if (model.PuntosPatrulla.Length <= 1) return;

        if (model.HacerPingPong)
        {
            model.indicePunto += model.direccion;
            if (model.indicePunto >= model.PuntosPatrulla.Length)
            {
                model.indicePunto = model.PuntosPatrulla.Length - 2;
                model.direccion = -1;
            }
            else if (model.indicePunto < 0)
            {
                model.indicePunto = 1;
                model.direccion = 1;
            }
        }
        else
        {
            model.indicePunto = (model.indicePunto + 1) % model.PuntosPatrulla.Length;
        }
    }
}
