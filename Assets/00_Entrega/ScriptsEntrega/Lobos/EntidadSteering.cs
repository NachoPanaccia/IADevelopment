// Archivo: EntidadSteering.cs
using UnityEngine;

/// <summary>
/// Integra fuerzas de steering y mueve el transform en el plano XZ.
/// </summary>
public class EntidadSteering : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] float velocidadMaxima = 7f;
    [SerializeField] float fuerzaMaxima = 12f;
    [SerializeField] float friccion = 0.0f; // 0 = sin desaceleración artificial

    Vector3 velocidad;
    Vector3 acumuladorFuerza;

    public Vector3 Velocidad => velocidad;
    public float VelocidadMaxima => velocidadMaxima;

    public void AddFuerza(Vector3 f)
    {
        f.y = 0f;
        acumuladorFuerza += Vector3.ClampMagnitude(f, fuerzaMaxima);
    }

    public Vector3 Seek(Vector3 objetivoMundo)
    {
        Vector3 deseado = (objetivoMundo - transform.position);
        deseado.y = 0f;
        if (deseado.sqrMagnitude < 0.0001f) return Vector3.zero;
        deseado = deseado.normalized * velocidadMaxima;
        return Steer(deseado);
    }

    public Vector3 Steer(Vector3 velocidadDeseada)
    {
        velocidadDeseada.y = 0f;
        Vector3 steer = velocidadDeseada - velocidad;
        return Vector3.ClampMagnitude(steer, fuerzaMaxima);
    }

    public void Mover()
    {
        // Integración simple
        Vector3 aceleracion = acumuladorFuerza;
        velocidad += aceleracion * Time.deltaTime;

        // Fricción opcional
        if (friccion > 0f)
            velocidad = Vector3.Lerp(velocidad, Vector3.zero, friccion * Time.deltaTime);

        // Clamp de velocidad
        velocidad = Vector3.ClampMagnitude(velocidad, velocidadMaxima);

        // Movimiento en plano
        Vector3 delta = velocidad * Time.deltaTime;
        delta.y = 0f;
        transform.position += delta;

        // Orientación (si hay movimiento)
        Vector3 dir = velocidad;
        dir.y = 0f;
        if (dir.sqrMagnitude > 0.0001f)
            transform.rotation = Quaternion.LookRotation(dir.normalized, Vector3.up);

        acumuladorFuerza = Vector3.zero;
    }
}
