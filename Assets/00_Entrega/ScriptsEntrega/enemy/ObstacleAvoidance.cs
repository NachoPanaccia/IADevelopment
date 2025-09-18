using UnityEngine;

[DisallowMultipleComponent]
public class ObstacleAvoidance : MonoBehaviour
{
    [Header("Detecci�n")]
    [SerializeField] private float rangoPrediccionBase = 2f;   
    [SerializeField] private float factorRangoPorVelocidad = 0.15f; 
    [SerializeField] private float radio = 0.6f;               
    [SerializeField] private LayerMask mascaraObstaculos = ~0; 

    [Header("Respuesta")]
    [SerializeField] private float pesoEvitacion = 2.0f;       
    [SerializeField] private float atenuarFrente = 0.5f;       
    [SerializeField] private bool forzarLateral = true;        
    [Header("Depuraci�n")]
    [SerializeField] private bool habilitarLogs = false;
    [SerializeField] private bool dibujarGizmos = true;

    private Vector3 ultimaVelocidad;
    private RaycastHit ultimoHit;
    private bool huboHit;

    
    /// Devuelve un vector de evitaci�n (en xz). Si no hay obst�culo, Vector3.zero.
    
    
    public Vector3 Evitar(Vector3 velocidadDeseada)
    {
        ultimaVelocidad = velocidadDeseada;
        huboHit = false;

        // Si no nos movemos no evitamos nada
        if (velocidadDeseada.sqrMagnitude < 0.0001f)
            return Vector3.zero;

        // Direcci�n y distancia 
        Vector3 dir = velocidadDeseada.normalized;
        float lookAhead = rangoPrediccionBase + factorRangoPorVelocidad * velocidadDeseada.magnitude;

        
        if (Physics.SphereCast(transform.position, radio, dir, out RaycastHit hit, lookAhead, mascaraObstaculos, QueryTriggerInteraction.Ignore))
        {
            huboHit = true;
            ultimoHit = hit;

            // Normal del obst�culo 
            Vector3 normal = hit.normal;
            normal.y = 0f;
            if (normal.sqrMagnitude < 0.0001f)
                normal = (transform.position - hit.point).normalized; // fallback

            
            if (forzarLateral)
            {
                
                Vector3 lateral = Vector3.Cross(Vector3.up, dir).normalized;
                // Elegimos el lado que m�s se aleja del obst�culo
                float lado = Mathf.Sign(Vector3.Dot(lateral, normal));
                normal = lateral * lado;
            }
            else
            {
                
                float frontal = Mathf.Abs(Vector3.Dot(normal, dir));
                normal *= Mathf.Lerp(1f, atenuarFrente, frontal);
            }

            // Peso de evitaci�n
            Vector3 evitacion = normal * pesoEvitacion;
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
        float lookAhead = rangoPrediccionBase + factorRangoPorVelocidad * ultimaVelocidad.magnitude;

        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, dir * lookAhead);
        Gizmos.DrawWireSphere(transform.position + dir * lookAhead, radio);

        // Punto de impacto
        if (huboHit)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(ultimoHit.point, 0.1f);
            Gizmos.DrawRay(ultimoHit.point, ultimoHit.normal); // normal del obst�culo
        }

        // Radio instant�neo alrededor
        Gizmos.color = new Color(0f, 0.3f, 0f, 0.7f);
        Gizmos.DrawWireSphere(transform.position, radio);
    }
}

