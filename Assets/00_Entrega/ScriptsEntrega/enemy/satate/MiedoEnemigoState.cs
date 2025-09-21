using UnityEngine;

/// Estado de miedo: el enemigo se queda quieto y, tras un tiempo, vuelve a Patrulla.
public class MiedoEnemigoState : EstadoEnemigo<EnemyStates>
{
    private readonly EnemigoModel modelo;
    private readonly float duracionMiedo;     // segundos configurables
    private float tiempoRestanteMiedo;

    public MiedoEnemigoState(EnemigoModel modelo, float duracionMiedo = 3f)
    {
        this.modelo = modelo;
        this.duracionMiedo = Mathf.Max(0f, duracionMiedo);
    }

    public override void Enter()
    {
        tiempoRestanteMiedo = duracionMiedo;
        modelo.MoverXZ(Vector3.zero, 0f); // frenamos al entrar
        if (modelo.HabilitarLogs) Debug.Log("[Enemigo] Enter Miedo (quieto)");
    }

    public override void Execute()
    {
        // Mantenerse inmóvil
        modelo.MoverXZ(Vector3.zero, 0f);

        // Temporizador
        if (tiempoRestanteMiedo > 0f)
        {
            tiempoRestanteMiedo -= Time.deltaTime;
            if (tiempoRestanteMiedo <= 0f)
            {
                if (modelo.HabilitarLogs) Debug.Log("[Enemigo] Miedo → Patrulla (fin del timer)");
                fsm.SetState(EnemyStates.Patrulla);
            }
        }
    }

    public override void Exit()
    {
        if (modelo.HabilitarLogs) Debug.Log("[Enemigo] Exit Miedo");
    }
}
