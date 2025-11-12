// Archivo: BoidLobo.cs
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EntidadSteering))]
public class BoidLobo : MonoBehaviour
{
    EntidadSteering steering;
    GestorFlocking gf;

    public Vector3 VelocidadActual => steering != null ? steering.Velocidad : Vector3.zero;
    public float VelocidadMaxima => steering != null ? steering.VelocidadMaxima : 6f;

    // Proxies
    public Vector3 Seek(Vector3 posicion) => steering != null ? steering.Seek(posicion) : Vector3.zero;
    public Vector3 Steer(Vector3 deseado) => steering != null ? steering.Steer(deseado) : Vector3.zero;
    public void AddFuerza(Vector3 fuerza) { if (steering != null) steering.AddFuerza(fuerza); }
    public void Mover() { if (steering != null) steering.Mover(); }

    void Start()
    {
        steering = GetComponent<EntidadSteering>();
        gf = GestorFlocking.Instance;
        gf?.AgregarBoid(this);

        // arranque suave
        var rnd = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f)).normalized * (VelocidadMaxima * 0.4f);
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

        // Flocking base (estilo profe)
        var compSep = GetComponent<ComportamientoSeparacion>();
        if (compSep) fuerzaTotal += compSep.ObtenerDireccion(todos) * gf.pesoSeparacion;

        var compCoh = GetComponent<ComportamientoCohesion>();
        if (compCoh) fuerzaTotal += compCoh.ObtenerDireccion(todos) * gf.pesoCohesion;

        var compAli = GetComponent<ComportamientoAlineacion>();
        if (compAli) fuerzaTotal += compAli.ObtenerDireccion(todos) * gf.pesoAlineacion;

        // Objetivo común (sin offsets)
        if (gf.usarObjetivoGlobal)
        {
            var compObj = GetComponent<ComportamientoObjetivoComun>();
            if (compObj) fuerzaTotal += compObj.ObtenerDireccion(todos) * gf.pesoObjetivo;
        }

        AddFuerza(fuerzaTotal);
        Mover();
    }
}
