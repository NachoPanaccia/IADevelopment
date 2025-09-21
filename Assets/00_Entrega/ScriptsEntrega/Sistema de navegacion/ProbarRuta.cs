using UnityEngine;

/// <summary>
/// Script de prueba: con la tecla (por defecto Espacio) regenera el grafo,
/// busca nodos más cercanos a jugador/objetivo y traza la ruta con LineRenderer.
/// </summary>
public class ProbarRuta : MonoBehaviour
{
    [SerializeField] Transform jugador;
    [SerializeField] Transform objetivo;
    [SerializeField] GrafoDinamico grafo;
    [SerializeField] PathVisualizer visualizador;
    [SerializeField] KeyCode teclaCalcular = KeyCode.Space;
    [SerializeField] bool loguearEstados = false;

    void Update()
    {
        if (!Input.GetKeyDown(teclaCalcular)) return;

        if (grafo == null || visualizador == null || jugador == null || objetivo == null)
        {
            Debug.LogError("[ProbarRuta] Faltan referencias (grafo/visualizador/jugador/objetivo).");
            return;
        }

        //grafo.generarGrafo();

        var origen = grafo.buscarMasCercano(jugador.position);
        var destino = grafo.buscarMasCercano(objetivo.position);

        if (origen == null || destino == null)
        {
            Debug.LogWarning("[ProbarRuta] No se encontraron nodos caminables cercanos.");
            visualizador.dibujar(null);
            return;
        }

        var camino = Pathfinder.dijkstra(origen, destino);

        if (camino.Count == 0)
        {
            if (loguearEstados) Debug.Log("[ProbarRuta] Destino inalcanzable desde el origen.");
            visualizador.dibujar(null);
        }
        else
        {
            if (loguearEstados) Debug.Log($"[ProbarRuta] Camino calculado. Nodos: {camino.Count}");
            visualizador.dibujar(camino);
        }
    }
}
