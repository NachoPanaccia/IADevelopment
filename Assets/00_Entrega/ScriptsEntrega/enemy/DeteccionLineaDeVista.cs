using UnityEngine;

#if UNITY_EDITOR
using UnityEditor; // Para Handles.DrawSolidArc (solo en Editor)
#endif

/// <summary>
/// Detección por línea de vista con FOV, raycast a obstáculos y memoria de visión.
/// Versión para UN objetivo. Dibuja Gizmos siempre (opcional) y loguea cuando ve al objetivo.
/// </summary>
[DisallowMultipleComponent]
public class DeteccionLineaDeVista : MonoBehaviour
{
    // ===== Estado (solo lectura) =====
    [Header("Estado de detección (solo lectura)")]
    [SerializeField] private bool objetivoVisible;
    [SerializeField] private Transform objetivoActual;
    [SerializeField] private float distanciaAlObjetivo;
    [SerializeField] private Vector3 ultimaPosicionVista;
    [SerializeField] private float tiempoDesdeUltimaVista;

    public bool ObjetivoVisible => objetivoVisible;
    public Transform ObjetivoActual => objetivoActual;
    public float DistanciaAlObjetivo => distanciaAlObjetivo;
    public Vector3 UltimaPosicionVista => ultimaPosicionVista;
    public float TiempoDesdeUltimaVista => tiempoDesdeUltimaVista;

    // ===== Configuración de visión =====
    [Header("Percepción por visión")]
    [SerializeField] private Transform objetivo;              // Asignar Player acá (o Tag)
    [SerializeField] private float distanciaVision = 12f;
    [SerializeField, Range(1f, 179f)] private float anguloVision = 90f;
    [SerializeField] private float alturaOjos = 1.6f;
    [SerializeField] private LayerMask mascaraObstaculos;

    [Header("Memoria de visión")]
    [SerializeField] private float tiempoMemoriaVision = 2.0f;

    [Header("Barrido / performance")]
    [SerializeField] private float intervaloChequeo = 0.1f;

    [Header("Depuración")]
    [SerializeField] private bool habilitarLogs = true;
    [SerializeField] private bool dibujarGizmos = true;
    [SerializeField] private bool dibujarSiempre = true;      // <- dibuja aunque no esté seleccionado
    [SerializeField] private Color colorCono = new Color(0f, 1f, 0.2f, 0.25f);
    [SerializeField] private Color colorRayoOK = Color.green;
    [SerializeField] private Color colorRayoBloqueado = Color.red;

    [Header("Autoconfiguración")]
    [SerializeField] private bool buscarObjetivoPorTag = true;
    [SerializeField] private string tagObjetivo = "Player";

    private float tiempoAcumulado;

    private void Awake()
    {
        // Si no arrastraste el objetivo, buscar por Tag (una sola vez)
        if (objetivo == null && buscarObjetivoPorTag)
        {
            var go = GameObject.FindWithTag(tagObjetivo);
            if (go != null) objetivo = go.transform;
            if (habilitarLogs && objetivo == null)
                Debug.LogWarning($"[DeteccionLineaDeVista] No se encontró un objeto con Tag '{tagObjetivo}'. Asigná el objetivo en el Inspector.");
        }

        // Asegurar alfa visible del cono
        if (colorCono.a < 0.1f) colorCono.a = 0.25f;
    }

    private void Update()
    {
        tiempoAcumulado += Time.deltaTime;
        tiempoDesdeUltimaVista += Time.deltaTime;

        if (tiempoAcumulado >= intervaloChequeo)
        {
            tiempoAcumulado = 0f;
            ActualizarDeteccion();
        }

        // Línea de depuración en Play
        if (Application.isPlaying && objetivo != null)
        {
            Debug.DrawLine(ObtenerPuntoVista(), objetivo.position,
                objetivoVisible ? colorRayoOK : colorRayoBloqueado, 0f, false);
        }
    }

    private void ActualizarDeteccion()
    {
        if (objetivo == null)
        {
            objetivoVisible = false;
            return;
        }

        Vector3 origenVista = ObtenerPuntoVista();
        Vector3 dir = objetivo.position - origenVista;
        float dist = dir.magnitude;

        // Dentro del ángulo y rango
        if (dist <= distanciaVision && Vector3.Angle(transform.forward, dir) <= anguloVision * 0.5f)
        {
            // ¿Hay obstáculo bloqueando?
            if (!HayObstaculos(origenVista, objetivo.position))
            {
                bool antesNoVeia = !objetivoVisible; // para loguear solo en flanco
                objetivoVisible = true;
                objetivoActual = objetivo;
                distanciaAlObjetivo = dist;
                ultimaPosicionVista = objetivo.position;
                tiempoDesdeUltimaVista = 0f;

                if (habilitarLogs && antesNoVeia)
                    Debug.Log($"👀 Objetivo detectado: {objetivo.name} a {distanciaAlObjetivo:0.0} m");
                return;
            }
        }

        // Si no hay LOS directa, memoria
        if (objetivoActual != null && tiempoDesdeUltimaVista <= tiempoMemoriaVision)
        {
            objetivoVisible = true;
        }
        else
        {
            objetivoVisible = false;
            objetivoActual = null;
           
        }
    }

    // ===== Utilitarios =====
    private Vector3 ObtenerPuntoVista()
    {
        Vector3 pos = transform.position;
        pos.y += alturaOjos;
        return pos;
    }

    private bool HayObstaculos(Vector3 origen, Vector3 destino)
    {
        Vector3 dir = destino - origen;
        float dist = dir.magnitude;
        if (dist <= Mathf.Epsilon) return false;
        return Physics.Raycast(origen, dir.normalized, dist, mascaraObstaculos, QueryTriggerInteraction.Ignore);
    }

    // ===== Gizmos siempre visibles =====
    private void OnDrawGizmos()
    {
        if (!dibujarGizmos || !dibujarSiempre) return;
        DibujarGizmosComun();
    }

    private void OnDrawGizmosSelected()
    {
        if (!dibujarGizmos || dibujarSiempre) return;
        DibujarGizmosComun();
    }

    private void DibujarGizmosComun()
    {
        Vector3 origenVista = Application.isPlaying ? ObtenerPuntoVista() : transform.position + Vector3.up * alturaOjos;

        // Fallback con líneas si no estamos en Editor
        DibujarConoWire(origenVista, transform.forward, anguloVision, distanciaVision);

        // Línea hacia el objetivo si existe
        if (objetivo != null)
        {
            bool bloqueado = HayObstaculos(origenVista, objetivo.position);
            Gizmos.color = bloqueado ? colorRayoBloqueado : colorRayoOK;
            Gizmos.DrawLine(origenVista, objetivo.position);
            Gizmos.DrawSphere(objetivo.position, 0.1f);
        }

        // En Editor, dibujar arco sólido (mucho más visible)
#if UNITY_EDITOR
        Handles.color = new Color(colorCono.r, colorCono.g, colorCono.b, Mathf.Clamp01(colorCono.a));
        Vector3 forward = transform.forward;
        float medio = anguloVision * 0.5f;
        Vector3 from = Quaternion.Euler(0, -medio, 0) * forward;
        // DrawSolidArc dibuja en el plano perpendicular al "up" (Vector3.up)
        Handles.DrawSolidArc(origenVista, Vector3.up, from, anguloVision, distanciaVision);
#endif
    }

    // Cono "alambre" para plataformas fuera del Editor
    private void DibujarConoWire(Vector3 origen, Vector3 forward, float anguloTotal, float radio)
    {
        Gizmos.color = new Color(colorCono.r, colorCono.g, colorCono.b, 0.4f);
        int segmentos = 28;
        float medio = anguloTotal * 0.5f;

        Vector3 prev = origen + Quaternion.AngleAxis(-medio, Vector3.up) * forward * radio;
        for (int i = 1; i <= segmentos; i++)
        {
            float t = Mathf.Lerp(-medio, medio, i / (float)segmentos);
            Vector3 dir = Quaternion.AngleAxis(t, Vector3.up) * forward;
            Vector3 punto = origen + dir * radio;
            Gizmos.DrawLine(prev, punto);
            prev = punto;
        }

        // bordes del cono
        Gizmos.DrawLine(origen, origen + (Quaternion.AngleAxis(-medio, Vector3.up) * forward) * radio);
        Gizmos.DrawLine(origen, origen + (Quaternion.AngleAxis(medio, Vector3.up) * forward) * radio);
    }
}
