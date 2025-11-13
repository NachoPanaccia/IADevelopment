using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Ajusta la dirección/velocidad del boid hacia la media de sus vecinos.
/// </summary>
[RequireComponent(typeof(BoidLobo))]
public class ComportamientoAlineacion : MonoBehaviour, IFlockingComportamiento
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

        Vector3 velProm = Vector3.zero;
        int count = 0;
        float r = gf.radioCohesion; // mismo radio que cohesión

        foreach (var otro in todos)
        {
            if (otro == null || otro == boid) continue;
            Vector3 delta = otro.transform.position - transform.position;
            delta.y = 0f;
            if (delta.magnitude < r)
            {
                velProm += otro.VelocidadActual;
                count++;
            }
        }

        if (count == 0) return Vector3.zero;

        velProm /= count;
        velProm.y = 0f;
        if (velProm.sqrMagnitude < 0.0001f) return Vector3.zero;

        Vector3 deseado = velProm.normalized * boid.VelocidadMaxima;
        return boid.Steer(deseado);
    }
}
