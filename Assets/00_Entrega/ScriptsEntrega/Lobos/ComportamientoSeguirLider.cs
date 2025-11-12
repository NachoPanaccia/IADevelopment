// Archivo: ComportamientoSeguirLider.cs
using UnityEngine;

/// <summary>
/// Seek hacia el líder (con offset en anillo) o hacia el objetivo de alerta.
/// Con la opción "seguirSiempreAlLider", ignora la alerta y lo sigue SIEMPRE.
/// </summary>
[RequireComponent(typeof(BoidLobo))]
public class ComportamientoSeguirLider : MonoBehaviour, IFlockingComportamiento
{
    BoidLobo boid;
    GestorFlocking gf;
    BandadaLobos bandada;

    [Header("Seguir siempre al líder (ignorar alerta)")]
    [SerializeField] bool seguirSiempreAlLider = true;

    [Header("Anillo alrededor del líder")]
    [Tooltip("Factor para encoger/agrandar el anillo relativo a GestorFlocking.DistanciaComodaAlLider (1 = igual, <1 más cerca).")]
    [SerializeField] float multiplicadorDistancia = 0.6f;

    [Header("Tirón extra al líder")]
    [SerializeField, Range(0f, 3f)] float fuerzaExtraCercania = 0.9f;

    [Header("Seguir detrás del líder (opcional)")]
    [SerializeField] float seguirDetrasDistancia = 0.8f;

    Vector3 offsetLocalAnillo;

    void Awake()
    {
        boid = GetComponent<BoidLobo>();
        gf = GestorFlocking.Instance;
        bandada = gf != null ? gf.Bandada : null;

        // Semilla estable por instancia para repartir ángulos
        int seed = GetInstanceID();
        Random.InitState(seed);

        float rBase = (gf != null ? gf.DistanciaComodaAlLider : 3.5f);
        float r = Mathf.Max(0.1f, rBase * Mathf.Max(0.1f, multiplicadorDistancia));

        float ang = Random.Range(0f, Mathf.PI * 2f);
        offsetLocalAnillo = new Vector3(Mathf.Cos(ang) * r, 0f, Mathf.Sin(ang) * r);
    }

    public Vector3 ObtenerDireccion(System.Collections.Generic.List<BoidLobo> _)
    {
        if (gf == null) gf = GestorFlocking.Instance;
        if (gf == null) return Vector3.zero;

        if (bandada == null) bandada = gf.Bandada;
        if (bandada == null || bandada.Lider == null) return Vector3.zero;

        var lider = bandada.Lider;

        // Si NO queremos ignorar alerta y está activa, ir al objetivo común
        if (!seguirSiempreAlLider && bandada.AlertaActiva)
        {
            return boid.Seek(bandada.ObjetivoAlerta);
        }

        // Recalcular radio del anillo si se modificó en runtime
        float rBase = Mathf.Max(0.1f, gf.DistanciaComodaAlLider);
        float r = Mathf.Max(0.1f, rBase * Mathf.Max(0.1f, multiplicadorDistancia));
        offsetLocalAnillo = offsetLocalAnillo.sqrMagnitude > 0.0001f ? offsetLocalAnillo.normalized * r : new Vector3(r, 0f, 0f);

        // Offset girado con el líder + un pequeño desplazamiento hacia atrás
        Vector3 offsetMundo = lider.rotation * offsetLocalAnillo;
        Vector3 detras = Vector3.zero;
        if (seguirDetrasDistancia > 0.001f)
        {
            Vector3 back = -(new Vector3(lider.forward.x, 0f, lider.forward.z)).normalized;
            detras = back * seguirDetrasDistancia;
        }

        Vector3 target = lider.position + offsetMundo + detras;

        // 1) Seek al target del anillo
        Vector3 v = boid.Seek(target);

        // 2) Tirón extra directo al líder (pega más)
        if (fuerzaExtraCercania > 0.001f)
        {
            Vector3 haciaLider = (lider.position - transform.position);
            haciaLider.y = 0f;
            if (haciaLider.sqrMagnitude > 0.0001f)
            {
                Vector3 deseado = haciaLider.normalized * boid.VelocidadMaxima;
                v += boid.Steer(deseado) * fuerzaExtraCercania;
            }
        }

        return v;
    }
}

