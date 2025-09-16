using UnityEngine;

[DisallowMultipleComponent]
public class ObstacleAvoidance : MonoBehaviour
{
    [Header("Detección")]
    [SerializeField] private float rangoPrediccionBase = 2f;   // metros de look-ahead mínimos
    [SerializeField] private float factorRangoPorVelocidad = 0.15f; // metros extra por cada m/s
    [SerializeField] private float radio = 0.6f;               // radio del SphereCast
    [SerializeField] private LayerMask mascaraObstaculos = ~0; // por defecto: todo

    [Header("Respuesta")]
    [SerializeField] private float pesoEvitacion = 2.0f;       // cuán fuerte empuja al costado
    [SerializeField] private float atenuarFrente = 0.5f;       // reduce empuje si la normal da muy “de frente”
    [SerializeField] private bool forzarLateral = true;        // proyecta la normal al plano XZ y perpendicular a la marcha

    [Header("Depuración")]
    [SerializeField] private bool habilitarLogs = false;
    [SerializeField] private bool dibujarGizmos = true;

    private Vector3 ultimaVelocidad;
    private RaycastHit ultimoHit;
    private bool huboHit;

    /// <summary>
    /// Devuelve un vector de evitación (en XZ). Si no hay obstáculo, Vector3.zero.
    /// Pasar acá la velocidad deseada (m/s * dirección).
    /// </summary>
    public Vector3 Evitar(Vector3 velocidadDeseada)
    {
        ultimaVelocidad = velocidadDeseada;
        huboHit = false;

        // Si no nos estamos moviendo, no hay nada que evitar.
        if (velocidadDeseada.sqrMagnitude < 0.0001f)
            return Vector3.zero;

        // Dirección y distancia de look-ahead.
        Vector3 dir = velocidadDeseada.normalized;
        float lookAhead = rangoPrediccionBase + factorRangoPorVelocidad * velocidadDeseada.magnitude;

        // SphereCast al frente
        if (Physics.SphereCast(transform.position, radio, dir, out RaycastHit hit, lookAhead, mascaraObstaculos, QueryTriggerInteraction.Ignore))
        {
            huboHit = true;
            ultimoHit = hit;

            // Normal del obstáculo (aplanada a XZ)
            Vector3 normal = hit.normal;
            normal.y = 0f;
            if (normal.sqrMagnitude < 0.0001f)
                normal = (transform.position - hit.point).normalized; // fallback

            // Opcional: “forzar” una componente lateral pura si la normal apunta demasiado hacia atrás o adelante
            if (forzarLateral)
            {
                // Eje lateral respecto a la marcha (perpendicular en el plano XZ)
                Vector3 lateral = Vector3.Cross(Vector3.up, dir).normalized;
                // Elegimos el lado que más se aleja del obstáculo
                float lado = Mathf.Sign(Vector3.Dot(lateral, normal));
                normal = lateral * lado;
            }
            else
            {
                // Si la normal es muy frontal, la atenuamos para no frenar en seco.
                float frontal = Mathf.Abs(Vector3.Dot(normal, dir));
                normal *= Mathf.Lerp(1f, atenuarFrente, frontal);
            }

            // Peso de evitación
            Vector3 evitacion = normal * pesoEvitacion;
            evitacion.y = 0f;

            if (habilitarLogs)
                Debug.Log($"[ObstacleAvoidance] Esquivando '{hit.collider.name}' a {hit.distance:0.00}m. Normal={hit.normal}, Evitacion={evitacion}");

            return evitacion;
        }

        return Vector3.zero;
    }

    private void OnDrawGizmos()
    {
        if (!dibujarGizmos) return;

        // Dibujar look-ahead con datos actuales
        Vector3 dir = (ultimaVelocidad.sqrMagnitude > 0.0001f) ? ultimaVelocidad.normalized : transform.forward;
        float lookAhead = rangoPrediccionBase + factorRangoPorVelocidad * ultimaVelocidad.magnitude;

        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, dir * lookAhead);
        Gizmos.DrawWireSphere(transform.position + dir * lookAhead, radio);

        // Punto de impacto
        if (huboHit)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(ultimoHit.point, 0.1f);
            Gizmos.DrawRay(ultimoHit.point, ultimoHit.normal); // normal del obstáculo
        }

        // Radio instantáneo alrededor
        Gizmos.color = new Color(0f, 0.3f, 0f, 0.7f);
        Gizmos.DrawWireSphere(transform.position, radio);
    }
}

