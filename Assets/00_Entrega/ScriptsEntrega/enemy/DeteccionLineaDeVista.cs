using UnityEngine;

#if UNITY_EDITOR
using UnityEditor; // esto solo para dibujar el arco en el editor
#endif


/// Sensor de visión: mira en un cono  chequea obstáculos y guarda al jugador.


[DisallowMultipleComponent]
public class DeteccionLineaDeVista : MonoBehaviour
{
    
    [Header("Estado de detección")]
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

    
    [Header("Parámetros de visión")]
    [SerializeField] private Transform objetivo;  
    [SerializeField] private float distanciaVision = 12f;
    [SerializeField, Range(1f, 179f)] private float anguloVision = 90f;
    [SerializeField] private float alturaOjos = 1.6f;
    [SerializeField] private LayerMask mascaraObstaculos;

    [Header("Memoria")]
    [SerializeField] private float tiempoMemoriaVision = 2.0f;

    [Header("Chequeo por performance")]
    [SerializeField] private float intervaloChequeo = 0.1f;

    [Header("Debug")]
    [SerializeField] private bool habilitarLogs = true;
    [SerializeField] private bool dibujarGizmos = true;
    [SerializeField] private bool dibujarSiempre = true;
    [SerializeField] private Color colorCono = new Color(0f, 1f, 0.2f, 0.25f);
    [SerializeField] private Color colorRayoOK = Color.green;
    [SerializeField] private Color colorRayoBloqueado = Color.red;

    [Header("Autoconfiguración")]
    [SerializeField] private bool buscarObjetivoPorTag = true;
    [SerializeField] private string tagObjetivo = "Player";

    private float tiempoAcumulado;

    private void Awake()
    {
        
        if (objetivo == null && buscarObjetivoPorTag)
        {
            var go = GameObject.FindWithTag(tagObjetivo);
            if (go != null) objetivo = go.transform;
            if (habilitarLogs && objetivo == null)
                Debug.LogWarning($"[DeteccionLineaDeVista] No se encontró un objeto con Tag '{tagObjetivo}'");
        }

        // aseguramos que el cono tenga un alfa visible
        if (colorCono.a < 0.1f) colorCono.a = 0.25f;
    }

    private void Update()
    {
        tiempoAcumulado += Time.deltaTime;
        tiempoDesdeUltimaVista += Time.deltaTime;

        // hacemos el chequeo cada intervaloChequeo (no todos los frames)
        if (tiempoAcumulado >= intervaloChequeo)
        {
            tiempoAcumulado = 0f;
            ActualizarDeteccion();
        }

        // dibuja línea en modo Play para depurar
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

        // chequeamos rango y ángulo
        if (dist <= distanciaVision && Vector3.Angle(transform.forward, dir) <= anguloVision * 0.5f)
        {
            // si no hay obstáculo en el medio lo ve
            if (!HayObstaculos(origenVista, objetivo.position))
            {
                bool antesNoVeia = !objetivoVisible;
                objetivoVisible = true;
                objetivoActual = objetivo;
                distanciaAlObjetivo = dist;
                ultimaPosicionVista = objetivo.position;
                tiempoDesdeUltimaVista = 0f;

                if (habilitarLogs && antesNoVeia)
                    Debug.Log($" Objetivo detectado: {objetivo.name} a {distanciaAlObjetivo:0.0} m");
                return;
            }
        }

        // si no lo ve directo, usa memoria
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

    // Gizmos 
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

        DibujarConoWire(origenVista, transform.forward, anguloVision, distanciaVision);

        if (objetivo != null)
        {
            bool bloqueado = HayObstaculos(origenVista, objetivo.position);
            Gizmos.color = bloqueado ? colorRayoBloqueado : colorRayoOK;
            Gizmos.DrawLine(origenVista, objetivo.position);
            Gizmos.DrawSphere(objetivo.position, 0.1f);
        }

#if UNITY_EDITOR
        Handles.color = new Color(colorCono.r, colorCono.g, colorCono.b, Mathf.Clamp01(colorCono.a));
        Vector3 forward = transform.forward;
        float medio = anguloVision * 0.5f;
        Vector3 from = Quaternion.Euler(0, -medio, 0) * forward;
        Handles.DrawSolidArc(origenVista, Vector3.up, from, anguloVision, distanciaVision);
#endif
    }

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

        Gizmos.DrawLine(origen, origen + (Quaternion.AngleAxis(-medio, Vector3.up) * forward) * radio);
        Gizmos.DrawLine(origen, origen + (Quaternion.AngleAxis(medio, Vector3.up) * forward) * radio);
    }
}
