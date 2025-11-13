
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Todos los boids hacen seek al MISMO objetivo global.
/// </summary>
[RequireComponent(typeof(BoidLobo))]
public class ComportamientoObjetivoComun : MonoBehaviour, IFlockingComportamiento
{
    BoidLobo boid;
    GestorFlocking gf;

    [SerializeField, Range(0f, 2f)] float fuerzaExtraObjetivo = 1.0f;

    void Awake()
    {
        boid = GetComponent<BoidLobo>();
        gf = GestorFlocking.Instance;
    }

    public Vector3 ObtenerDireccion(List<BoidLobo> _)
    {
        if (gf == null) gf = GestorFlocking.Instance;
        if (gf == null || !gf.usarObjetivoGlobal) return Vector3.zero;

        Vector3 objetivo = gf.ObjetivoActual;

        // Seek base
        Vector3 v = boid.Seek(objetivo);

        // Pequeño tirón extra para que alcancen los rezagados
        if (fuerzaExtraObjetivo > 0.001f)
        {
            Vector3 hacia = objetivo - transform.position;
            hacia.y = 0f;
            if (hacia.sqrMagnitude > 0.0001f)
            {
                Vector3 deseado = hacia.normalized * boid.VelocidadMaxima;
                v += boid.Steer(deseado) * fuerzaExtraObjetivo;
            }
        }

        return v;
    }
}
