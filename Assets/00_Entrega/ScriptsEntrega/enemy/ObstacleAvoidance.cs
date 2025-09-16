using UnityEngine;

/// <summary>
/// Evitación de obstáculos simple via SphereCast (steering behaviour).
/// - Proyecta una esfera hacia la velocidad deseada y, si choca, devuelve
///   un vector de empuje lateral para esquivar.
/// - Si no detecta obstáculos, devuelve Vector3.zero.
/// </summary>
public class ObstacleAvoidance : MonoBehaviour
{
    [Header("Parámetros")]
    [SerializeField] private float rangoPrediccion = 2.5f;     // cuánto "mira" hacia adelante
    [SerializeField] private float radio = 0.5f;               // radio de la esfera de prueba
    [SerializeField] private LayerMask mascaraObstaculos;      // capas a considerar como obstáculos
    [SerializeField] private float fuerzaEmpuje = 1.0f;        // peso del vector de evitación

    // cache local para debug
    private Vector3 ultimaVelocidadDeseada;
    private RaycastHit ultimoHit;

    /// <summary>
    /// Calcula el vector de evitación para una velocidad deseada (XZ).
    /// Devuelve Vector3.zero si no hay nada por esquivar.
    /// </summary>
    public Vector3 Evitar(Vector3 velocidadDeseada)
    {
        ultimaVelocidadDeseada = velocidadDeseada;

        // Sin dirección => no evitamos nada
        if (velocidadDeseada.sqrMagnitude < 0.0001f)
            return Vector3.zero;

        Vector3 dir = velocidadDeseada.normalized;
        float distanciaChequeo = rangoPrediccion * velocidadDeseada.magnitude;

        // SphereCast hacia adelante
        if (!Physics.SphereCast(transform.position, radio, dir, out ultimoHit, distanciaChequeo, mascaraObstaculos, QueryTriggerInteraction.Ignore))
            return Vector3.zero;

        // Vector de empuje: apuntamos a alejarnos del obstáculo, solo en XZ
        Vector3 empuje = (transform.position - ultimoHit.point);
        empuje.y = 0f;

        if (empuje.sqrMagnitude < 0.0001f)
        {
            // fallback: empuje lateral ortogonal a la marcha
            empuje = Vector3.Cross(dir, Vector3.up);
        }

        empuje = empuje.normalized * fuerzaEmpuje;
        return empuje;
    }

    // Alias para compatibilidad si alguien llama Avoid(...) en inglés.
    public Vector3 Avoid(Vector3 velocidadDeseada) => Evitar(velocidadDeseada);

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        // Gizmos de debug (cuando está seleccionado)
        Gizmos.color = Color.green;
        Vector3 dir = ultimaVelocidadDeseada.sqrMagnitude > 0.0001f ? ultimaVelocidadDeseada.normalized : transform.forward;
        float distanciaChequeo = rangoPrediccion * Mathf.Max(0.5f, ultimaVelocidadDeseada.magnitude);

        // línea de proyección
        Gizmos.DrawLine(transform.position, transform.position + dir * distanciaChequeo);

        // esfera de origen
        Gizmos.DrawWireSphere(transform.position, radio);

        // punto de impacto
        if (ultimoHit.collider != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(ultimoHit.point, 0.15f);
        }
    }
#endif
}
