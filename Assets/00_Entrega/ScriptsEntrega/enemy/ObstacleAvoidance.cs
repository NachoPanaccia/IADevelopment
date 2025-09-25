using UnityEngine;

[DisallowMultipleComponent]
public class ObstacleAvoidance : MonoBehaviour
{
    [Header("Detección")]
    [SerializeField] private float rangoPrediccionBase = 2f;
    //[SerializeField] private float factorRangoPorVelocidad = 0.15f;
    [SerializeField] private float radio = 0.6f;
    [SerializeField] private LayerMask mascaraObstaculos = ~0;

    [Header("Respuesta")]
    [SerializeField] private float pesoEvitacion = 2.0f; 
    [SerializeField] private bool forzarLateral = true;
    [Header("Depuración")]
    [SerializeField] private bool habilitarLogs = false;
    [SerializeField] private bool dibujarGizmos = true;

    private Vector3 ultimaVelocidad;
    private RaycastHit ultimoHit;
    private bool huboHit;

    public Vector3 Evitar(Vector3 velocidadDeseada)
    {
        ultimaVelocidad = velocidadDeseada;
        huboHit = false;

        // si no me estoy moviendo no evito nada
        if (velocidadDeseada.sqrMagnitude < 0.0001f)
            return Vector3.zero;

        // Dirección y distancia 
        Vector3 dir = velocidadDeseada.normalized; // adelante
        float lookAhead = rangoPrediccionBase; // que tan adelante

        // spherecast adelante para ver si hay algo con lo que me voy a chocar
        if (Physics.SphereCast(transform.position, radio, dir, out RaycastHit hit, lookAhead, mascaraObstaculos, QueryTriggerInteraction.Ignore))
        {
            huboHit = true;
            ultimoHit = hit;

            // saco la normal del obstaculo y la uso para empujarme
            Vector3 normal = hit.normal;
            normal.y = 0f; // solo plano XZ

            // si la normal es rara, uso el vector desde el obstaculo hacia mi
            if (normal.sqrMagnitude < 0.0001f)
                normal = (transform.position - hit.point).normalized; // fallback

            // si quiero forzar siempre lateral (izq/der) lo hago por cross es decir salgo perpendicular a un borde
            if (forzarLateral)
            {
                Vector3 lateral = Vector3.Cross(Vector3.up, dir).normalized;
                // elijo lado segun que tan alineado está con la normal
                float lado = Mathf.Sign(Vector3.Dot(lateral, normal));
                normal = lateral * lado;
            }
            

            // fuerzo en XZ y aplico el peso
            Vector3 evitacion = normal * pesoEvitacion; // el peso es cuanto me quiero mover
            evitacion.y = 0f;
            return evitacion;
        }

        return Vector3.zero;
    }

    private void OnDrawGizmos()
    {
        if (!dibujarGizmos) return;

        // Dibujar 
        Vector3 dir = (ultimaVelocidad.sqrMagnitude > 0.0001f) ? ultimaVelocidad.normalized : transform.forward;
        float lookAhead = rangoPrediccionBase;

        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, dir * lookAhead);

        // esfera al final para ver el rango
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


