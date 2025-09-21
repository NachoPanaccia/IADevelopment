using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Algoritmos de pathfinding. Implementación robusta de Dijkstra.
/// </summary>
public static class Pathfinder
{
    /// <summary>
    /// Calcula el camino más corto entre inicio y destino (si existe).
    /// Devuelve lista vacía si el destino no es alcanzable.
    /// </summary>
    public static List<NodoGrafo> dijkstra(NodoGrafo inicio, NodoGrafo destino)
    {
        var resultadoVacio = new List<NodoGrafo>();
        if (inicio == null || destino == null) return resultadoVacio;

        // Recolecto el subgrafo alcanzable desde "inicio"
        var todos = obtenerTodos(inicio);

        // Inicializo costos
        foreach (var n in todos)
        {
            n.distancia = float.MaxValue;
            n.previo = null;
        }
        inicio.distancia = 0f;

        var abiertos = new List<NodoGrafo>(todos);
        var cerrados = new HashSet<NodoGrafo>();

        while (abiertos.Count > 0)
        {
            // Tomo el de menor distancia
            int idxMin = 0;
            float min = abiertos[0].distancia;
            for (int i = 1; i < abiertos.Count; i++)
            {
                if (abiertos[i].distancia < min)
                {
                    min = abiertos[i].distancia;
                    idxMin = i;
                }
            }

            var actual = abiertos[idxMin];
            abiertos.RemoveAt(idxMin);
            cerrados.Add(actual);

            if (actual == destino) break; // llegamos: corto

            // Relajo vecinos
            foreach (var v in actual.vecinos)
            {
                if (v == null || cerrados.Contains(v)) continue;

                float nuevo = actual.distancia + Vector3.Distance(actual.posicion, v.posicion);
                if (nuevo < v.distancia)
                {
                    v.distancia = nuevo;
                    v.previo = actual;
                }
            }
        }

        // Si destino no tiene "previo" (y no es igual a inicio), no hay camino
        if (destino != inicio && destino.previo == null)
            return resultadoVacio;

        // Reconstruyo
        var camino = new List<NodoGrafo>();
        var nodo = destino;
        while (nodo != null)
        {
            camino.Insert(0, nodo);
            nodo = nodo.previo;
        }
        return camino;
    }

    /// <summary>
    /// BFS para recolectar todos los nodos alcanzables desde "inicio".
    /// </summary>
    static HashSet<NodoGrafo> obtenerTodos(NodoGrafo inicio)
    {
        var visitados = new HashSet<NodoGrafo>();
        var cola = new Queue<NodoGrafo>();
        cola.Enqueue(inicio);

        while (cola.Count > 0)
        {
            var actual = cola.Dequeue();
            if (actual == null || visitados.Contains(actual)) continue;

            visitados.Add(actual);
            foreach (var v in actual.vecinos)
                if (v != null) cola.Enqueue(v);
        }

        return visitados;
    }
}
