// Archivo: ComportamientoSeparacion.cs
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Aleja al boid de sus vecinos más cercanos (evita choques).
/// </summary>
[RequireComponent(typeof(BoidLobo))]
public class ComportamientoSeparacion : MonoBehaviour, IFlockingComportamiento
{
    BoidLobo boid;
    GestorFlocking gf;

    void Awake()
    {
        boid = GetComponent<BoidLobo>();
        gf = GestorFlocking.Instance;
    }

    public Vector3 ObtenerDireccion(List<BoidLobo> todos)
    {
        if (gf == null) gf = GestorFlocking.Instance;
        if (gf == null) return Vector3.zero;

        Vector3 suma = Vector3.zero;
        int count = 0;
        float r = gf.radioSeparacion;

        foreach (var otro in todos)
        {
            if (otro == null || otro == boid) continue;
            Vector3 delta = transform.position - otro.transform.position;
            delta.y = 0f;
            float d = delta.magnitude;
            if (d > 0f && d < r)
            {
                // más empuje cuanto más cerca (inversa de la distancia)
                suma += delta.normalized * (1f - (d / r));
                count++;
            }
        }

        if (count == 0) return Vector3.zero;

        Vector3 deseado = suma.normalized * boid.VelocidadMaxima;
        return boid.Steer(deseado);
    }
}
