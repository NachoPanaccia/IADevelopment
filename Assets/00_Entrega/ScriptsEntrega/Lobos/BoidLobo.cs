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

    // Proxies para comportamientos
    public Vector3 Seek(Vector3 posicion) => steering != null ? steering.Seek(posicion) : Vector3.zero;
    public Vector3 Steer(Vector3 deseado) => steering != null ? steering.Steer(deseado) : Vector3.zero;
    public void AddFuerza(Vector3 f) { if (steering != null) steering.AddFuerza(f); }
    public void Mover() { if (steering != null) steering.Mover(); }

    void Start()
    {
        steering = GetComponent<EntidadSteering>();
        gf = GestorFlocking.Instance;
        gf?.AgregarBoid(this);

        // Empuje inicial leve para romper simetrías
        Vector3 rnd = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized * (VelocidadMaxima * 0.35f);
        AddFuerza(rnd);
    }

    void OnDestroy() => gf?.QuitarBoid(this);

    void Update()
    {
        if (gf == null) gf = GestorFlocking.Instance;
        if (gf == null) return;

        List<BoidLobo> todos = gf.Todos;
        Vector3 fuerza = Vector3.zero;

        var sep = GetComponent<ComportamientoSeparacion>();
        if (sep) fuerza += sep.ObtenerDireccion(todos) * gf.pesoSeparacion;

        var coh = GetComponent<ComportamientoCohesion>();
        if (coh) fuerza += coh.ObtenerDireccion(todos) * gf.pesoCohesion;

        var ali = GetComponent<ComportamientoAlineacion>();
        if (ali) fuerza += ali.ObtenerDireccion(todos) * gf.pesoAlineacion;

        if (gf.usarObjetivoGlobal)
        {
            var obj = GetComponent<ComportamientoObjetivoComun>();
            if (obj) fuerza += obj.ObtenerDireccion(todos) * gf.pesoObjetivo;
        }

        AddFuerza(fuerza);
        Mover();
    }
}
