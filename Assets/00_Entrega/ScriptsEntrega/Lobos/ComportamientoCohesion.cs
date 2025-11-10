// Archivo: ComportamientoCohesion.cs
using System.Collections.Generic;
using UnityEngine;

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

        Vector3 promedio = Vector3.zero;
        int count = 0;

        for (int i = 0; i < todos.Count; i++)
        {
            var otro = todos[i];
            if (otro == boid) continue;

            Vector3 diff = transform.position - otro.transform.position;
            if (diff.sqrMagnitude > gf.radioCohesion * gf.radioCohesion) continue;

            promedio += otro.transform.position;
            count++;
        }

        if (count == 0) return Vector3.zero;

        promedio /= count;
        return boid.Seek(promedio);
    }
}
