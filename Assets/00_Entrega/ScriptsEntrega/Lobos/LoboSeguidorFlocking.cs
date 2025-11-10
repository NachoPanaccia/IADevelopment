// Archivo: LoboSeguidorFlocking.cs
using System;
using System.Reflection;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class LoboSeguidorFlocking : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] BandadaLobos bandada;
    [SerializeField] ObstacleAvoidance evitacion; // opcional

    [Header("Vecindad (Boids)")]
    [SerializeField] float radioVecindad = 4f;
    [SerializeField] LayerMask mascaraLobos; // layer de los lobos

    [Header("Pesos Boids")]
    [SerializeField] float pesoSeparacion = 1.5f;
    [SerializeField] float pesoAlineacion = 1.0f;
    [SerializeField] float pesoCohesion = 1.0f;

    [Header("Objetivo")]
    [SerializeField] float pesoHaciaObjetivo = 1.2f; // hacia líder o objetivo de alerta
    [SerializeField] float distanciaDeseadaAlLider = 3.5f; // confort al merodear

    [Header("Evitación (mezcla general)")]
    [SerializeField] bool usarEvitacion = true;
    [SerializeField] float pesoEvitacion = 1.0f;

    [Header("Evitación interna (fallback si ObstacleAvoidance no expone método)")]
    [SerializeField] float rangoPrediccion = 3.0f;
    [SerializeField] float radioEvitar = 0.5f;
    [SerializeField] LayerMask mascaraObstaculos = ~0; // por defecto, todo

    [Header("Movimiento")]
    [SerializeField] float velocidadMaxima = 6.0f;
    [SerializeField] float fuerzaMaxima = 10.0f;

    [Header("Debug")]
    [SerializeField] bool habilitarLogs = false;

    Rigidbody rb;

    // Cache de método de evitación por reflexión (si existe en ObstacleAvoidance)
    MethodInfo metodoEvitacionReflex;
    bool intentoReflexHecho = false;

    void Reset()
    {
        bandada = FindAnyObjectByType<BandadaLobos>();
        evitacion = GetComponent<ObstacleAvoidance>();
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (bandada == null) bandada = FindAnyObjectByType<BandadaLobos>();
        bandada?.RegistrarSeguidor(this);
        if (usarEvitacion && evitacion == null) evitacion = GetComponent<ObstacleAvoidance>();
    }

    void OnDestroy()
    {
        bandada?.DesregistrarSeguidor(this);
    }

    void FixedUpdate()
    {
        if (bandada == null)
        {
            rb.linearVelocity = Vector3.zero;
            return;
        }

        Vector3 vFlocking = CalcularFlocking();
        Vector3 vObjetivo = CalcularHaciaObjetivo(); // líder u objetivo de alerta
        Vector3 vEvitacion = Vector3.zero;

        if (usarEvitacion)
        {
            // 1) Intentar usar ObstacleAvoidance si ofrece un método compatible
            vEvitacion = IntentarEvitacionConComponente(rb.linearVelocity);
            // 2) Si no hay método, fallback interno
            if (vEvitacion == Vector3.zero)
                vEvitacion = CalcularEvitacionInterna(rb.linearVelocity);
        }

        // Mezcla
        Vector3 aceleracion =
            vFlocking
            + vObjetivo * pesoHaciaObjetivo
            + vEvitacion * pesoEvitacion;

        // Limitar fuerza y aplicar
        aceleracion = Vector3.ClampMagnitude(aceleracion, fuerzaMaxima);
        Vector3 nuevaVel = rb.linearVelocity + aceleracion * Time.fixedDeltaTime;
        nuevaVel = Vector3.ClampMagnitude(nuevaVel, velocidadMaxima);
        nuevaVel.y = 0f;

        rb.linearVelocity = nuevaVel;
        if (nuevaVel.sqrMagnitude > 0.0001f)
            transform.forward = nuevaVel.normalized;
    }

    Vector3 CalcularFlocking()
    {
        Collider[] vecinos = Physics.OverlapSphere(transform.position, radioVecindad, mascaraLobos);

        Vector3 separacion = Vector3.zero;
        Vector3 alineacion = Vector3.zero;
        Vector3 cohesion = Vector3.zero;
        int countSep = 0, countAli = 0, countCoh = 0;

        foreach (var c in vecinos)
        {
            if (c.attachedRigidbody == rb) continue;

            // Separación
            Vector3 diff = transform.position - c.transform.position;
            diff.y = 0f;
            float dist = diff.magnitude;
            if (dist > 0.0001f)
            {
                separacion += diff.normalized / dist;
                countSep++;
            }

            // Alineación
            var rbOtro = c.attachedRigidbody;
            if (rbOtro != null)
            {
                Vector3 v = rbOtro.linearVelocity;
                v.y = 0f;
                alineacion += v;
                countAli++;
            }

            // Cohesión
            Vector3 p = c.transform.position;
            p.y = 0f;
            cohesion += p;
            countCoh++;
        }

        if (countSep > 0) separacion = (separacion / countSep).normalized * pesoSeparacion;
        if (countAli > 0) alineacion = (alineacion / countAli).normalized * pesoAlineacion;
        if (countCoh > 0)
        {
            cohesion /= countCoh;
            Vector3 haciaCentro = (cohesion - transform.position);
            haciaCentro.y = 0f;
            cohesion = haciaCentro.normalized * pesoCohesion;
        }

        return separacion + alineacion + cohesion;
    }

    Vector3 CalcularHaciaObjetivo()
    {
        Vector3 objetivo;
        if (bandada.AlertaActiva)
        {
            objetivo = bandada.ObjetivoAlerta;
        }
        else
        {
            if (bandada.Lider == null) return Vector3.zero;
            // Mantenerse cerca del líder (círculo de confort)
            Vector3 haciaLider = bandada.Lider.position - transform.position;
            haciaLider.y = 0f;

            float dist = haciaLider.magnitude;
            if (dist > distanciaDeseadaAlLider * 1.2f)
            {
                // si me alejé mucho, volver
                return haciaLider.normalized;
            }
            if (dist < distanciaDeseadaAlLider * 0.8f)
            {
                // si estoy muy pegado, me alejo un toque
                return -haciaLider.normalized * 0.5f;
            }

            // Zona neutra: un leve “wander” para que no queden estáticos
            return WanderSuave(0.6f, 1.2f);
        }

        Vector3 dir = (objetivo - transform.position);
        dir.y = 0f;
        return dir.sqrMagnitude > 0.0001f ? dir.normalized : Vector3.zero;
    }

    // Wander muy simple: ruido en el plano XZ
    Vector3 WanderSuave(float fuerzaMin, float fuerzaMax)
    {
        float ang = UnityEngine.Random.Range(0f, Mathf.PI * 2f);
        float mag = UnityEngine.Random.Range(fuerzaMin, fuerzaMax);
        Vector3 r = new Vector3(Mathf.Cos(ang), 0f, Mathf.Sin(ang)) * mag;
        return r;
    }

    // ==========================
    // Evitación integrada (robusta)
    // ==========================

    // 1) Intentar usar un método del componente ObstacleAvoidance si existe
    Vector3 IntentarEvitacionConComponente(Vector3 velocidadActual)
    {
        if (evitacion == null) return Vector3.zero;

        // Buscar el método una sola vez
        if (!intentoReflexHecho)
        {
            intentoReflexHecho = true;
            var tipo = evitacion.GetType();

            // Prioridades de nombres comunes
            metodoEvitacionReflex = tipo.GetMethod("Avoid", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                                    ?? tipo.GetMethod("CalcularEvitacion", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                                    ?? tipo.GetMethod("GetAvoidance", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (habilitarLogs)
            {
                Debug.Log(metodoEvitacionReflex != null
                    ? $"[LoboSeguidorFlocking] Encontré método de evitación en {tipo.Name}: {metodoEvitacionReflex.Name}"
                    : $"[LoboSeguidorFlocking] No encontré método público de evitación en {tipo.Name}. Uso fallback interno.");
            }
        }

        if (metodoEvitacionReflex == null) return Vector3.zero;

        try
        {
            // Intentar invocar con firma (Vector3) → Vector3
            var pars = metodoEvitacionReflex.GetParameters();
            object resultado;

            if (pars.Length == 1 && pars[0].ParameterType == typeof(Vector3))
            {
                resultado = metodoEvitacionReflex.Invoke(evitacion, new object[] { velocidadActual });
            }
            else if (pars.Length == 0)
            {
                resultado = metodoEvitacionReflex.Invoke(evitacion, null);
            }
            else
            {
                // Firma no compatible
                return Vector3.zero;
            }

            if (resultado is Vector3 v) return NoY(v);
        }
        catch { /* ignorar y fallback */ }

        return Vector3.zero;
    }

    // 2) Fallback interno si no hay método en el componente
    Vector3 CalcularEvitacionInterna(Vector3 velocidadActual)
    {
        Vector3 dir = velocidadActual;
        if (dir.sqrMagnitude < 0.0001f)
        {
            // si estoy casi quieto, usar forward actual
            dir = transform.forward * 0.01f;
        }

        dir.y = 0f;
        dir.Normalize();

        // Predictivo + SphereCast
        Vector3 origen = transform.position + Vector3.up * 0.2f;
        float distancia = Mathf.Max(rangoPrediccion, 0.1f);

        if (Physics.SphereCast(origen, Mathf.Max(radioEvitar, 0.01f), dir, out RaycastHit hit, distancia, mascaraObstaculos))
        {
            // Vector para alejarse del obstáculo (desde su centro aproximado)
            Vector3 alejamiento = (transform.position - hit.collider.bounds.center);
            alejamiento.y = 0f;
            if (alejamiento.sqrMagnitude < 0.0001f)
            {
                // Si estamos casi en el centro, empujamos perpendicular a la dirección
                alejamiento = Vector3.Cross(dir, Vector3.up);
            }
            return alejamiento.normalized;
        }

        return Vector3.zero;
    }

    static Vector3 NoY(Vector3 v)
    {
        v.y = 0f;
        return v;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0.2f, 0.6f, 1f, 0.25f);
        Gizmos.DrawWireSphere(transform.position, radioVecindad);

        // Debug del fallback de evitación
        Gizmos.color = Color.yellow;
        Vector3 dir = transform.forward;
        dir.y = 0f;
        dir.Normalize();
        Gizmos.DrawWireSphere(transform.position + dir * rangoPrediccion, radioEvitar);
    }
}
