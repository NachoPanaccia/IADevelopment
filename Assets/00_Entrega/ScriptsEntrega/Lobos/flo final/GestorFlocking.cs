// Archivo: GestorFlocking.cs
using System.Collections.Generic;
using UnityEngine;

public class GestorFlocking : MonoBehaviour
{
    public static GestorFlocking Instance { get; private set; }

    [Header("Radios")]
    [SerializeField] public float radioSeparacion = 2.8f;
    [SerializeField] public float radioCohesion = 5.5f;

    [Header("Pesos")]
    [SerializeField, Range(0f, 3f)] public float pesoSeparacion = 2.0f;
    [SerializeField, Range(0f, 3f)] public float pesoCohesion = 1.0f;
    [SerializeField, Range(0f, 3f)] public float pesoAlineacion = 1.1f;
    [SerializeField, Range(0f, 3f)] public float pesoObjetivo = 1.8f; // empuje hacia el objetivo común

    // =========================================
    // OBJETIVO GLOBAL (SIN LÍDER) — "mover como grupo"
    // =========================================
    public enum ModoObjetivoGlobal { PatrullaPuntos, ObjetivoManual, PosicionFija }

    [Header("Objetivo Global (mover como grupo)")]
    [SerializeField] public bool usarObjetivoGlobal = true;
    [SerializeField] public ModoObjetivoGlobal modoObjetivoGlobal = ModoObjetivoGlobal.PatrullaPuntos;

    [Header("Patrulla por puntos")]
    public enum ModoPatrulla { Circular, PingPong, Aleatorio }
    [SerializeField] public ModoPatrulla modoPatrulla = ModoPatrulla.Circular;
    [SerializeField] public Transform[] puntosPatrulla;
    [SerializeField, Min(0.2f)] public float distanciaCambioPunto = 1.4f;
    [SerializeField, Range(0.1f, 1f)] public float porcentajeParaCambiar = 0.6f; // % de boids que debe estar dentro del radio para avanzar

    [Header("Objetivo manual o fijo")]
    [SerializeField] public Transform objetivoManual;     // si modo = ObjetivoManual
    [SerializeField] public Vector3 posicionObjetivoFijo; // si modo = PosicionFija

    // Estado interno de patrulla (un único índice para TODOS los boids)
    [SerializeField] int indicePuntoActual = 0;
    bool pingPongHaciaAdelante = true;

    // (compatibilidad con scripts previos: no se usa en este enfoque)
    [Header("Compat (no usar con objetivo global)")]
    [SerializeField] BandadaLobos bandada;
    [SerializeField] bool usarObjetivoBandada = false;
    [SerializeField, Min(0f)] float distanciaComodaAlLider = 3.0f;

    // Lista de boids registrados (estilo profe)
    readonly List<BoidLobo> boids = new();
    public List<BoidLobo> Todos => boids;

    // === API pública: objetivo actual en MUNDO ===
    public Vector3 ObjetivoActual
    {
        get
        {
            switch (modoObjetivoGlobal)
            {
                case ModoObjetivoGlobal.PatrullaPuntos:
                    if (puntosPatrulla != null && puntosPatrulla.Length > 0 && puntosPatrulla[indicePuntoActual] != null)
                        return puntosPatrulla[indicePuntoActual].position;
                    break;
                case ModoObjetivoGlobal.ObjetivoManual:
                    if (objetivoManual != null) return objetivoManual.position;
                    break;
                case ModoObjetivoGlobal.PosicionFija:
                    return posicionObjetivoFijo;
            }
            return transform.position; // fallback inocuo
        }
    }

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if (!usarObjetivoGlobal) return;

        if (modoObjetivoGlobal == ModoObjetivoGlobal.PatrullaPuntos)
        {
            ActualizarPatrullaComoGrupo();
        }
        // En ObjetivoManual / PosicionFija no hay nada que actualizar por frame
    }

    void ActualizarPatrullaComoGrupo()
    {
        if (puntosPatrulla == null || puntosPatrulla.Length == 0) return;

        Vector3 objetivo = ObjetivoActual;
        int total = boids.Count;
        if (total == 0) return;

        // ¿Qué porcentaje de boids ya está dentro del radio?
        int dentro = 0;
        float r = Mathf.Max(0.2f, distanciaCambioPunto);

        for (int i = 0; i < total; i++)
        {
            if (boids[i] == null) continue;
            float d = Vector3.Distance(Planar(boids[i].transform.position), Planar(objetivo));
            if (d <= r) dentro++;
        }

        float pct = (float)dentro / total;

        if (pct >= porcentajeParaCambiar)
        {
            AvanzarIndicePatrulla();
        }
    }

    void AvanzarIndicePatrulla()
    {
        int n = puntosPatrulla != null ? puntosPatrulla.Length : 0;
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
        // dibujar puntos y el objetivo actual
        if (puntosPatrulla != null && puntosPatrulla.Length > 0)
        {
            Gizmos.color = new Color(0.2f, 1f, 0.6f, 0.5f);
            for (int i = 0; i < puntosPatrulla.Length; i++)
            {
                if (puntosPatrulla[i] == null) continue;
                Gizmos.DrawSphere(puntosPatrulla[i].position + Vector3.up * 0.05f, 0.15f);
            }

            if (Application.isPlaying && modoObjetivoGlobal == ModoObjetivoGlobal.PatrullaPuntos)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(ObjetivoActual + Vector3.up * 0.05f, distanciaCambioPunto);
            }
        }
    }
}
