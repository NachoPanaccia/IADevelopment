// Archivo: ComportamientoPatrullaPuntos.cs
using UnityEngine;

/// <summary>
/// Comportamiento de steering que hace SEEK hacia un punto de patrulla (de GestorFlocking).
/// - Avanza al siguiente cuando está a "distanciaCambioPunto".
/// - Modo de avance: Circular / PingPong / Aleatorio (lee del GestorFlocking).
/// - Offset en anillo por boid para que no se amontonen en el punto.
/// No mueve; solo devuelve dirección (estilo profe).
/// </summary>
[RequireComponent(typeof(BoidLobo))]
public class ComportamientoPatrullaPuntos : MonoBehaviour, IFlockingComportamiento
{
    [Header("Ajustes de comportamiento")]
    [SerializeField] bool empezarEnPuntoMasCercano = true;
    [SerializeField] bool usarOffsetAnillo = true;
    [SerializeField, Min(0.1f)] float multiplicadorAnillo = 0.5f;    // <1 = más cerca del centro
    [SerializeField, Range(0f, 2f)] float fuerzaExtraObjetivo = 0.8f;

    [Header("Debug")]
    [SerializeField] bool dibujarGizmos = false;

    BoidLobo boid;
    GestorFlocking gf;
    Transform[] puntos;
    int indice;
    bool pingPongHaciaAdelante = true;     // para el modo PingPong
    Vector3 offsetAnilloLocal;             // offset estable por boid

    void Awake()
    {
        boid = GetComponent<BoidLobo>();
        gf = GestorFlocking.Instance;

        puntos = gf != null ? gf.PuntosPatrulla : null;

        // Semilla estable por instancia
        int seed = GetInstanceID();
        Random.InitState(seed);

        float radioBase = (gf != null) ? Mathf.Max(1f, gf.radioSeparacion * 0.8f) : 2f;
        float r = Mathf.Max(0.1f, radioBase * multiplicadorAnillo);

        float ang = Random.Range(0f, Mathf.PI * 2f);
        offsetAnilloLocal = new Vector3(Mathf.Cos(ang) * r, 0f, Mathf.Sin(ang) * r);

        // Elegir punto inicial
        indice = 0;
        if (empezarEnPuntoMasCercano && puntos != null && puntos.Length > 0)
        {
            indice = EncontrarIndiceMasCercano(transform.position, puntos);
        }
    }

    public Vector3 ObtenerDireccion(System.Collections.Generic.List<BoidLobo> _)
    {
        if (gf == null) gf = GestorFlocking.Instance;
        if (gf == null) return Vector3.zero;

        if (!gf.UsarPatrullaGlobal) return Vector3.zero;

        puntos = gf.PuntosPatrulla;
        if (puntos == null || puntos.Length == 0) return Vector3.zero;
        if (indice < 0 || indice >= puntos.Length) indice = 0;

        Transform p = puntos[indice];
        if (p == null) return Vector3.zero;

        // ¿Llega al punto?
        float distPlano = Vector3.Distance(Planar(transform.position), Planar(p.position));
        if (distPlano <= Mathf.Max(0.2f, gf.distanciaCambioPunto))
        {
            AvanzarIndice(gf.ModoDePatrulla, puntos.Length);
        }

        // Target con offset en anillo para evitar amontonamiento
        Vector3 target = p.position;
        if (usarOffsetAnillo) target += offsetAnilloLocal;

        // 1) seek al target
        Vector3 v = boid.Seek(target);

        // 2) tirón extra directo al punto "puro"
        if (fuerzaExtraObjetivo > 0.001f)
        {
            Vector3 deseado = (Planar(p.position - transform.position)).normalized * boid.VelocidadMaxima;
            v += boid.Steer(deseado) * fuerzaExtraObjetivo;
        }

        // Debug
        if (dibujarGizmos)
        {
            ultimoTarget = target;
            ultimoPuro = p.position;
            tieneGizmo = true;
        }

        return v;
    }

    // ==== helpers ====
    void AvanzarIndice(GestorFlocking.ModoPatrulla modo, int cantidad)
    {
        if (cantidad <= 1) return;

        switch (modo)
        {
            case GestorFlocking.ModoPatrulla.Circular:
                indice = (indice + 1) % cantidad;
                break;

            case GestorFlocking.ModoPatrulla.PingPong:
                if (pingPongHaciaAdelante)
                {
                    indice++;
                    if (indice >= cantidad - 1) { indice = cantidad - 1; pingPongHaciaAdelante = false; }
                }
                else
                {
                    indice--;
                    if (indice <= 0) { indice = 0; pingPongHaciaAdelante = true; }
                }
                break;

            case GestorFlocking.ModoPatrulla.Aleatorio:
                int nuevo;
                do { nuevo = Random.Range(0, cantidad); } while (nuevo == indice && cantidad > 1);
                indice = nuevo;
                break;
        }
    }

    static int EncontrarIndiceMasCercano(Vector3 pos, Transform[] pts)
    {
        int best = 0;
        float bestDist = float.MaxValue;
        for (int i = 0; i < pts.Length; i++)
        {
            if (pts[i] == null) continue;
            float d = Vector3.Distance(Planar(pos), Planar(pts[i].position));
            if (d < bestDist) { bestDist = d; best = i; }
        }
        return best;
    }

    static Vector3 Planar(Vector3 v) { v.y = 0f; return v; }

    // === gizmos ===
    Vector3 ultimoTarget, ultimoPuro;
    bool tieneGizmo;
    void OnDrawGizmos()
    {
        if (!dibujarGizmos || !tieneGizmo) return;
        Gizmos.color = new Color(0.2f, 1f, 0.6f, 0.8f);
        Gizmos.DrawWireSphere(ultimoTarget + Vector3.up * 0.05f, 0.15f);
        Gizmos.color = new Color(1f, 0.9f, 0.2f, 0.8f);
        Gizmos.DrawLine(transform.position + Vector3.up * 0.05f, ultimoPuro + Vector3.up * 0.05f);
    }
}
