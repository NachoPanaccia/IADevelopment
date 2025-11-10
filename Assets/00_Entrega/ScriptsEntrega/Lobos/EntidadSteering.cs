// Archivo: EntidadSteering.cs
using UnityEngine;

public class EntidadSteering : MonoBehaviour
{
    [SerializeField] protected float velocidadMaxima = 6f;
    [SerializeField] protected float fuerzaMaxima = 10f;
    protected Vector3 velocidad;

    // === Propiedades públicas para otros componentes ===
    public float VelocidadMaxima => velocidadMaxima;
    public Vector3 Velocidad => velocidad;

    // Ir hacia una posición (desired = dir * velocidadMaxima)
    public Vector3 Seek(Vector3 posicion)
    {
        var dir = posicion - transform.position;
        dir.y = 0f;
        return Steer(dir.normalized * velocidadMaxima);
    }

    // Convertir desired en steering limitado
    public Vector3 Steer(Vector3 deseado)
    {
        var steering = deseado - velocidad;
        steering = Vector3.ClampMagnitude(steering, fuerzaMaxima * Time.deltaTime);
        return steering;
    }

    // Acumular fuerza (aceleración integrada)
    public void AddFuerza(Vector3 fuerza)
    {
        velocidad = Vector3.ClampMagnitude(velocidad + fuerza, velocidadMaxima);
    }

    // Aplicar movimiento “a lo profe” (sin Rigidbody)
    public void Mover()
    {
        if (velocidad == Vector3.zero) return;
        var dir = velocidad;
        dir.y = 0f;
        if (dir.sqrMagnitude > 0.0001f) transform.forward = dir.normalized;
        transform.position += velocidad * Time.deltaTime;
    }
}
