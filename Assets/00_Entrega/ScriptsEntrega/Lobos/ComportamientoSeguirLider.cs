// Archivo: ComportamientoSeguirLider.cs
using UnityEngine;

/// <summary>
/// Comportamiento de flocking que hace SEEK hacia el líder (con un offset en anillo)
/// o hacia el objetivo de alerta de la bandada.
/// Mantiene el estilo "profe": solo devuelve un vector de steering; no mueve por sí solo.
/// </summary>
[RequireComponent(typeof(BoidLobo))]
public class ComportamientoSeguirLider : MonoBehaviour, IFlockingComportamiento
{
    BoidLobo boid;
    GestorFlocking gf;
    BandadaLobos bandada;

    // Offset estable por lobo para formarse alrededor del líder (evita amontonamiento)
    [SerializeField] float multiplicadorAnillo = 1.0f; // 1 = usa DistanciaComodaAlLider tal cual
    Vector3 offsetLocalAnillo; // en espacio local del líder (se rota con él)

    void Awake()
    {
        boid = GetComponent<BoidLobo>();
        gf = GestorFlocking.Instance;
        bandada = gf != null ? gf.Bandada : null;

        // Semilla estable por instancia para repartir ángulos
        int seed = GetInstanceID();
        Random.InitState(seed);

        float r = (gf != null ? gf.DistanciaComodaAlLider : 3.5f) * multiplicadorAnillo;
        float ang = Random.Range(0f, Mathf.PI * 2f);
        offsetLocalAnillo = new Vector3(Mathf.Cos(ang) * r, 0f, Mathf.Sin(ang) * r);
    }

    public Vector3 ObtenerDireccion(System.Collections.Generic.List<BoidLobo> _)
    {
        if (gf == null) gf = GestorFlocking.Instance;
        if (gf == null) return Vector3.zero;

        if (bandada == null) bandada = gf.Bandada;
        if (bandada == null) return Vector3.zero;

        // Si hay alerta, todos buscan el objetivo común (última posición vista del jugador)
        if (bandada.AlertaActiva)
        {
            return boid.Seek(bandada.ObjetivoAlerta);
        }

        // Sin alerta: seguir/merodear al líder con offset en anillo
        if (bandada.Lider == null) return Vector3.zero;

        // Transformar el offset local a mundo según la orientación del líder
        var lider = bandada.Lider;
        Vector3 offsetMundo = lider.rotation * offsetLocalAnillo;
        Vector3 target = lider.position + offsetMundo;

        return boid.Seek(target);
    }
}
