// Archivo: BoidLobo.cs
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EntidadSteering))]
public class BoidLobo : MonoBehaviour
{
    EntidadSteering steering;
    GestorFlocking gf;

    // Exposición pública simple (sin reflección)
    public Vector3 VelocidadActual => steering != null ? steering.Velocidad : Vector3.zero;
    public float VelocidadMaxima => steering != null ? steering.VelocidadMaxima : 6f;

    // Proxies para que los comportamientos puedan usar Seek/Steer sin tocar EntidadSteering
    public Vector3 Seek(Vector3 posicion) => steering != null ? steering.Seek(posicion) : Vector3.zero;
    public Vector3 Steer(Vector3 deseado) => steering != null ? steering.Steer(deseado) : Vector3.zero;
    public void AddFuerza(Vector3 fuerza) { if (steering != null) steering.AddFuerza(fuerza); }
    public void Mover() { if (steering != null) steering.Mover(); }

    void Start()
    {
        steering = GetComponent<EntidadSteering>();
        gf = GestorFlocking.Instance;

        gf?.AgregarBoid(this);

        // Empuje inicial aleatorio suave
        var rnd = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f)).normalized * (VelocidadMaxima * 0.5f);
        AddFuerza(rnd);
    }

    void OnDestroy()
    {
        gf?.QuitarBoid(this);
    }

    void Update()
    {
        if (gf == null) gf = GestorFlocking.Instance;
        if (gf == null) return;

        var todos = gf.Todos;

        Vector3 fuerzaTotal = Vector3.zero;

        // 1) Separación / Cohesión / Alineación (si existen)
        var compSep = GetComponent<ComportamientoSeparacion>();
        if (compSep) fuerzaTotal += compSep.ObtenerDireccion(todos) * gf.pesoSeparacion;

        var compCoh = GetComponent<ComportamientoCohesion>();
        if (compCoh) fuerzaTotal += compCoh.ObtenerDireccion(todos) * gf.pesoCohesion;

        var compAli = GetComponent<ComportamientoAlineacion>();
        if (compAli) fuerzaTotal += compAli.ObtenerDireccion(todos) * gf.pesoAlineacion;

        // 2) Seguir Líder / Objetivo (nuevo comportamiento)
        if (gf.UsarObjetivoBandada)
        {
            var compSeguir = GetComponent<ComportamientoSeguirLider>();
            if (compSeguir) fuerzaTotal += compSeguir.ObtenerDireccion(todos) * gf.pesoObjetivo;
        }

        // 3) Aplicar y mover (estilo profe)
        AddFuerza(fuerzaTotal);
        Mover();
    }
}
