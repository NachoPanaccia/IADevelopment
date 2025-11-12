// Archivo: ComportamientoCohesion.cs
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Mueve al boid hacia el centro de masa de sus vecinos (mantiene el grupo unido).
/// </summary>
[RequireComponent(typeof(BoidLobo))]
public class ComportamientoCohesion : MonoBehaviour, IFlockingComportamiento
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

        Vector3 centro = Vector3.zero;
        int count = 0;
        float r = gf.radioCohesion;

        foreach (var otro in todos)
        {
            if (otro == null || otro == boid) continue;
            Vector3 delta = otro.transform.position - transform.position;
            delta.y = 0f;
            float d = delta.magnitude;
            if (d > 0f && d < r)
            {
                centro += otro.transform.position;
                count++;
            }
        }

        if (count == 0) return Vector3.zero;

        centro /= count;
        return boid.Seek(centro);
    }
}

