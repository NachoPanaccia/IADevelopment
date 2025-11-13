// Archivo: ProbarRuta.cs
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Script de prueba: al presionar una tecla (por defecto Espacio) regenera el grafo,
/// resuelve ruta (Dijkstra / A* ) y la muestra con LineRenderer.
/// Si el objetivo (cofre) se instancia en runtime, lo busca por tag cada cierto intervalo.
/// </summary>
public class ProbarRuta : MonoBehaviour
{
    public enum Algoritmo { Dijkstra, AStar, ThetaStar }

    [Header("Referencias")]
    [SerializeField] Transform jugador;
    [SerializeField] Transform objetivo; // puede llegar nulo si se instancia después
    [SerializeField] GrafoDinamico grafo;
    [SerializeField] PathVisualizer visualizador;

    [Header("Algoritmo")]
    [SerializeField] Algoritmo algoritmo = Algoritmo.AStar;

    [Header("Entrada")]
    [SerializeField] KeyCode teclaCalcular = KeyCode.Space;

    [Header("Busqueda objetivo en runtime")]
    [SerializeField] bool habilitarBusquedaObjetivo = true;
    [SerializeField] string tagObjetivo = "Cofre";
    [SerializeField, Min(0.1f)] float intervaloReintentoObjetivo = 0.5f;

    [Header("Logs")]
    [SerializeField] bool habilitarLogs = false;

    float acumuladorBusqueda;

    void Update()
    {
        // 1) Localizar objetivo si aparece en runtime ( intento para 
        if (habilitarBusquedaObjetivo && objetivo == null)
        {
            acumuladorBusqueda += Time.deltaTime;
            if (acumuladorBusqueda >= intervaloReintentoObjetivo)
            {
                acumuladorBusqueda = 0f;
                var go = GameObject.FindWithTag(tagObjetivo);
                if (go != null)
                {
                    objetivo = go.transform;
                    if (habilitarLogs) Debug.Log($"[ProbarRuta] Objetivo encontrado por tag '{tagObjetivo}': {objetivo.name}");
                }
            }
        }

        if (!Input.GetKeyDown(teclaCalcular)) return;

        if (grafo == null || visualizador == null || jugador == null)
        {
            Debug.LogError("[ProbarRuta] Faltan referencias (grafo/visualizador/jugador).");
            return;
        }
        if (objetivo == null)
        {
            Debug.LogWarning("[ProbarRuta] No hay objetivo asignado ni encontrado por tag. Cancelo.");
            visualizador.dibujar(null);
            return;
        }

        
        grafo.generarGrafo();

        // 3) Buscar nodos más cercanos
        var origen = grafo.buscarMasCercano(jugador.position);
        var destino = grafo.buscarMasCercano(objetivo.position);

        if (origen == null || destino == null)
        {
            Debug.LogWarning("[ProbarRuta] No se encontraron nodos caminables cercanos.");
            visualizador.dibujar(null);
            return;
        }

        // 4) Resolver ruta según algoritmo
        List<NodoGrafo> camino = null;

        switch (algoritmo)
        {
            case Algoritmo.Dijkstra:
                camino = Pathfinder.dijkstra(origen, destino);
                break;

            case Algoritmo.AStar:
                camino = Pathfinder.aStar(origen, destino);
                break;

           
        }

        // 5) Dibujar
        if (camino == null || camino.Count == 0)
        {
            if (habilitarLogs) Debug.Log("[ProbarRuta] Destino inalcanzable.");
            visualizador.dibujar(null);
        }
        else
        {
            if (habilitarLogs) Debug.Log($"[ProbarRuta] Camino {algoritmo} calculado. Nodos: {camino.Count}");
            visualizador.dibujar(camino);
        }
    }
}
