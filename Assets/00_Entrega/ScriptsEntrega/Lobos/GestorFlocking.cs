
using System.Collections.Generic;
using UnityEngine;

public class GestorFlocking : MonoBehaviour
{
    public static GestorFlocking Instance { get; private set; }

    [Header("Radios (vecindad)")]
    [SerializeField] public float radioSeparacion = 2.8f;
    [SerializeField] public float radioCohesion = 5.5f;

    [Header("Pesos de fuerzas")]
    [SerializeField, Range(0f, 3f)] public float pesoSeparacion = 2.0f;
    [SerializeField, Range(0f, 3f)] public float pesoCohesion = 1.0f;
    [SerializeField, Range(0f, 3f)] public float pesoAlineacion = 1.1f;
    [SerializeField, Range(0f, 3f)] public float pesoObjetivo = 1.8f;

   
    public enum ModoPatrulla { Circular, PingPong, Aleatorio }

    [Header("Objetivo global (mover como grupo)")]
    [SerializeField] public bool usarObjetivoGlobal = true;
    [SerializeField] public Transform[] puntosPatrulla;       // waypoints 
    [SerializeField] public ModoPatrulla modoPatrulla = ModoPatrulla.PingPong;
    [SerializeField, Min(0.2f)] public float distanciaCambioPunto = 1.4f;

    [Tooltip("Porcentaje de boids dentro del radio para avanzar al siguiente punto (0.6 = 60%)")]
    [SerializeField, Range(0.1f, 1f)] public float porcentajeParaCambiar = 0.7f;

   
    [SerializeField] int indicePuntoActual = 0;
    bool pingPongHaciaAdelante = true;

    // Registro de boids
    readonly List<BoidLobo> boids = new();
    public List<BoidLobo> Todos => boids;

    // Objetivo actual (posición mundial)
    public Vector3 ObjetivoActual
    {
        get
        {
            if (puntosPatrulla != null && puntosPatrulla.Length > 0)
            {
                var t = puntosPatrulla[indicePuntoActual];
                if (t != null) return t.position;
            }
            return transform.position;
        }
    }

    void Awake() => Instance = this;

    void Update()
    {
        if (!usarObjetivoGlobal || puntosPatrulla == null || puntosPatrulla.Length == 0) return;

        // Avanza al próximo cuando llega la mayoría
        int total = boids.Count;
        if (total == 0) return;

        Vector3 objetivo = ObjetivoActual;
        float r = Mathf.Max(0.2f, distanciaCambioPunto);
        int dentro = 0;

        for (int i = 0; i < total; i++)
        {
            var b = boids[i];
            if (b == null) continue;
            if (Vector3.Distance(Planar(b.transform.position), Planar(objetivo)) <= r)
                dentro++;
        }

        if (total > 0 && (float)dentro / total >= porcentajeParaCambiar)
            avanzarIndicePatrulla();
    }

    void avanzarIndicePatrulla()
    {
        int n = puntosPatrulla?.Length ?? 0;
        if (n <= 1) return;

        switch (modoPatrulla)
        {
            case ModoPatrulla.Circular:
                indicePuntoActual = (indicePuntoActual + 1) % n;
                break;

            case ModoPatrulla.PingPong:
                if (pingPongHaciaAdelante)
                {
                    indicePuntoActual++;
                    if (indicePuntoActual >= n - 1) { indicePuntoActual = n - 1; pingPongHaciaAdelante = false; }
                }
                else
                {
                    indicePuntoActual--;
                    if (indicePuntoActual <= 0) { indicePuntoActual = 0; pingPongHaciaAdelante = true; }
                }
                break;

            case ModoPatrulla.Aleatorio:
                int nuevo;
                do { nuevo = Random.Range(0, n); } while (nuevo == indicePuntoActual && n > 1);
                indicePuntoActual = nuevo;
                break;
        }
    }

    public void AgregarBoid(BoidLobo b)
    {
        if (b != null && !boids.Contains(b)) boids.Add(b);
    }

    public void QuitarBoid(BoidLobo b)
    {
        if (b != null) boids.Remove(b);
    }

    static Vector3 Planar(Vector3 v) { v.y = 0f; return v; }

    void OnDrawGizmosSelected()
    {
        if (puntosPatrulla == null) return;

        Gizmos.color = new Color(0.2f, 1f, 0.6f, 0.6f);
        foreach (var p in puntosPatrulla)
        {
            if (p == null) continue;
            Gizmos.DrawSphere(p.position + Vector3.up * 0.05f, 0.15f);
        }

        if (Application.isPlaying)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(ObjetivoActual + Vector3.up * 0.05f, distanciaCambioPunto);
        }
    }
}
