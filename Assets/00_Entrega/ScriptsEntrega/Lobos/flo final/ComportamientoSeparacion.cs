// Archivo: ComportamientoSeparacion.cs
using System.Collections.Generic;
using UnityEngine;

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

        Vector3 total = Vector3.zero;
        int count = 0;

        for (int i = 0; i < todos.Count; i++)
        {
            var otro = todos[i];
            if (otro == boid) continue;

            Vector3 diff = transform.position - otro.transform.position;
            diff.y = 0f;
            float dist = diff.magnitude;
            if (dist > gf.radioSeparacion) continue;

            // Más cerca => más fuerte
            if (dist > 0.0001f)
            {
                total += (diff / dist) * (1f / dist);
                count++;
            }
        }

        if (count == 0) return Vector3.zero;

        var dir = total.normalized * boid.VelocidadMaxima;
        return boid.Steer(dir);
    }
}
