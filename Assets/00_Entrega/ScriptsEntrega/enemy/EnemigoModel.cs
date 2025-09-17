using UnityEngine;

// Modelo y helpers del enemig

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
    [SerializeField] private DeteccionLineaDeVista sensor;     // le pasamos el DeteccionLineaDeVista
    [SerializeField] private float tiempoHuidaExtra = 1.25f;   

    [Header("Ataque")]
    [SerializeField] private float velocidadAtaque = 7f;       

    [Header("Depuración")]
    [SerializeField] private bool habilitarLogs = false;       

    // Estado interno de patrulla
    public int indicePunto { get; set; }                      
    public int direccion { get; set; } = 1;                   
    public float tiempoEsperaRestante { get; set; }            

    
    private Rigidbody rb;
    public Rigidbody Rb => rb;

    
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
    public float VelocidadAtaque => velocidadAtaque;

    private void Awake()
    {
       
        rb = GetComponent<Rigidbody>();
        // si no asignaste sensor en inspector lo busca en el mismo objeto
        if (sensor == null) sensor = GetComponent<DeteccionLineaDeVista>();
        // avisa si no le pasaste puntos de patrulla
        if (puntosPatrulla == null || puntosPatrulla.Length == 0)
            Debug.LogWarning("[EnemigoModel] No hay puntos de patrulla asignados.");
    }

    // movimiento en xz con física, mantiene la velocidad y del rigidbody lo usamos en estados despues
    public void MoverXZ(Vector3 direccionXZ, float velocidad)
    {
        direccionXZ.y = 0f;
        direccionXZ = direccionXZ.normalized * velocidad;
        direccionXZ.y = rb.linearVelocity.y; 
        rb.linearVelocity = direccionXZ;
    }

    // rotación suave hacia una dirección lo usamos en estados despues
    public void MirarHacia(Vector3 direccion)
    {
        direccion.y = 0f;
        if (direccion.sqrMagnitude <= 0.0001f) return;
        var rotObjetivo = Quaternion.LookRotation(direccion.normalized, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotObjetivo, Time.deltaTime * velocidadGiro);
    }

    // calcula la evitación de obstáculos lo usamos en estados despues
    public Vector3 CalcularEvitacion(Vector3 velocidadDeseada)
    {
        var avoider = GetComponent<ObstacleAvoidance>();
        if (avoider != null)
            return avoider.Evitar(velocidadDeseada); // lo usabamos cuando no lo teniamos queda por las dudas
        return Vector3.zero;
    }
}
