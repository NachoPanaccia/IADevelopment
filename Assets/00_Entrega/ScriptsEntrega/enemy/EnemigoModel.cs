using UnityEngine;

/// <summary>
/// Modelo y helpers del enemigo. SIN ObstacleAvoidance.
/// Mantiene la API CalcularEvitacion() para no romper estados, pero devuelve Vector3.zero.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class EnemigoModel : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float velocidadPatrulla = 3.5f;
    [SerializeField] private float velocidadHuida = 6f;
    [SerializeField] private float velocidadGiro = 10f;
    [SerializeField] private float distanciaLlegada = 0.5f;

    [Header("Patrulla")]
    [SerializeField] private Transform[] puntosPatrulla;
    [SerializeField] private bool hacerPingPong = true;
    [SerializeField] private float tiempoEsperaEnPunto = 0.25f;

    [Header("Detección / Huida")]
    [SerializeField] private DeteccionLineaDeVista sensor;     // tu componente de visión
    [SerializeField] private float tiempoHuidaExtra = 1.25f;   // tiempo extra tras olvidar al jugador

    [Header("Depuración")]
    [SerializeField] private bool habilitarLogs = false;

    // Estado interno de patrulla
    public int indicePunto { get; set; }
    public int direccion { get; set; } = 1; // 1 → adelante, -1 → atrás (ping-pong)
    public float tiempoEsperaRestante { get; set; }

    // Refs
    private Rigidbody rb;
    public Rigidbody Rb => rb;

    // Expuestos para estados
    public float VelocidadPatrulla => velocidadPatrulla;
    public float VelocidadHuida => velocidadHuida;
    public float VelocidadGiro => velocidadGiro;
    public float DistanciaLlegada => distanciaLlegada;
    public Transform[] PuntosPatrulla => puntosPatrulla;
    public bool HacerPingPong => hacerPingPong;
    public float TiempoEsperaEnPunto => tiempoEsperaEnPunto;
    public float TiempoHuidaExtra => tiempoHuidaExtra;
    public DeteccionLineaDeVista Sensor => sensor;
    public bool HabilitarLogs => habilitarLogs;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (sensor == null) sensor = GetComponent<DeteccionLineaDeVista>();
        if (puntosPatrulla == null || puntosPatrulla.Length == 0)
            Debug.LogWarning("[EnemigoModel] No hay puntos de patrulla asignados.");
    }

    /// <summary> Aplica una velocidad deseada (en XZ) conservando la componente Y de la física. </summary>
    public void MoverXZ(Vector3 direccionXZ, float velocidad)
    {
        direccionXZ.y = 0f;
        direccionXZ = direccionXZ.normalized * velocidad;
        direccionXZ.y = rb.linearVelocity.y; // coherente con PlayerModel
        rb.linearVelocity = direccionXZ;
    }

    /// <summary> Rota suavemente hacia la dirección indicada (en XZ). </summary>
    public void MirarHacia(Vector3 direccion)
    {
        direccion.y = 0f;
        if (direccion.sqrMagnitude <= 0.0001f) return;
        var rotObjetivo = Quaternion.LookRotation(direccion.normalized, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotObjetivo, Time.deltaTime * velocidadGiro);
    }

    /// <summary>
    /// SIN evitación: se mantiene para no romper estados; devuelve Vector3.zero.
    /// </summary>
    public Vector3 CalcularEvitacion(Vector3 velocidadDeseada)
    {
        return Vector3.zero;
    }
}
