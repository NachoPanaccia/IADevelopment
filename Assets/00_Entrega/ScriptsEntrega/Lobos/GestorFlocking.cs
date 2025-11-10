// Archivo: GestorFlocking.cs
using System.Collections.Generic;
using UnityEngine;

public class GestorFlocking : MonoBehaviour
{
    // Singleton simple para que BoidLobo lo encuentre
    public static GestorFlocking Instance { get; private set; }

    [Header("Radios")]
    [SerializeField] public float radioSeparacion = 2.5f;
    [SerializeField] public float radioCohesion = 5.0f;

    [Header("Pesos")]
    [SerializeField, Range(0f, 3f)] public float pesoSeparacion = 1.8f;
    [SerializeField, Range(0f, 3f)] public float pesoCohesion = 1.0f;
    [SerializeField, Range(0f, 3f)] public float pesoAlineacion = 1.0f;
    [SerializeField, Range(0f, 3f)] public float pesoObjetivo = 1.3f; // líder / objetivo común

    [Header("Integración con bandada (opcional)")]
    [SerializeField] BandadaLobos bandada;          // arrastrá tu objeto "Bandada" si querés seguir líder/alerta
    [SerializeField] bool usarObjetivoBandada = true;

    // 👉 Campo que te faltaba (expuesto en el Inspector)
    [SerializeField, Min(0f)] float distanciaComodaAlLider = 3.2f;

    // Colección de boids registrados (estilo del profe)
    readonly List<BoidLobo> boids = new();

    // ===== API pública =====
    public List<BoidLobo> Todos => boids;
    public BandadaLobos Bandada => bandada;
    public bool UsarObjetivoBandada => usarObjetivoBandada;
    public float DistanciaComodaAlLider => distanciaComodaAlLider;

    void Awake()
    {
        Instance = this;
        if (bandada == null) bandada = FindAnyObjectByType<BandadaLobos>();
    }

    public void AgregarBoid(BoidLobo b)
    {
        if (b != null && !boids.Contains(b)) boids.Add(b);
    }

    public void QuitarBoid(BoidLobo b)
    {
        if (b != null) boids.Remove(b);
    }

    // (Opcional) Gizmos de ayuda
    void OnDrawGizmosSelected()
    {
        // Dibuja un círculo aproximado del "anillo" alrededor del líder
        if (bandada != null && bandada.Lider != null && distanciaComodaAlLider > 0f)
        {
            Gizmos.color = new Color(0.2f, 1f, 0.6f, 0.4f);
            Vector3 c = bandada.Lider.position;
            const int pasos = 32;
            Vector3 prev = c + new Vector3(distanciaComodaAlLider, 0f, 0f);
            for (int i = 1; i <= pasos; i++)
            {
                float ang = (i / (float)pasos) * Mathf.PI * 2f;
                Vector3 p = c + new Vector3(Mathf.Cos(ang) * distanciaComodaAlLider, 0f, Mathf.Sin(ang) * distanciaComodaAlLider);
                Gizmos.DrawLine(prev, p);
                prev = p;
            }
        }
    }
}
