// Archivo: Pathfinder.cs
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Algoritmos de pathfinding sobre NodoGrafo.
/// Incluye Dijkstra, A* y Theta* (opcional, requiere línea de vista configurada).
/// </summary>
public static class Pathfinder
{
    // ====== Config para Theta* (opcional) ======
    static LayerMask capaObstaculos = default;
    static float alturaLineaVista = 0.5f;

    /// <summary>
    /// Configura la capa de obstáculos y la altura a la que se raycastea para línea de vista (Theta*).
    /// </summary>
    public static void configurarLineaVista(LayerMask capa, float alturaVista = 0.5f)
    {
        capaObstaculos = capa;
        alturaLineaVista = alturaVista;
    }

    static bool tieneLineaVista(Vector3 a, Vector3 b)
    {
        // Si no hay capa configurada, asumimos que sí hay línea de vista (para no romper)
        if (capaObstaculos == default) return true;
        Vector3 pa = a + Vector3.up * alturaLineaVista;
        Vector3 pb = b + Vector3.up * alturaLineaVista;
        return !Physics.Linecast(pa, pb, capaObstaculos);
    }

    // ====== Utilidades comunes ======
    static float costoEntre(NodoGrafo a, NodoGrafo b)
    {
        return Vector3.Distance(a.posicion, b.posicion);
    }

    static float heuristica(NodoGrafo a, NodoGrafo b)
    {
        // Euclidiana: consistente en grillas con distancias euclidianas
        return Vector3.Distance(a.posicion, b.posicion);
    }

    static List<NodoGrafo> reconstruirCamino(NodoGrafo destino)
    {
        var lista = new List<NodoGrafo>();
        var n = destino;
        while (n != null)
        {
            lista.Insert(0, n);
            n = n.previo;
        }
        return lista;
    }

    static HashSet<NodoGrafo> recolectarDesde(NodoGrafo inicio)
    {
        var visitados = new HashSet<NodoGrafo>();
        var cola = new Queue<NodoGrafo>();
        cola.Enqueue(inicio);
        while (cola.Count > 0)
        {
            var act = cola.Dequeue();
            if (act == null || visitados.Contains(act)) continue;
            visitados.Add(act);
            foreach (var v in act.vecinos)
                if (v != null) cola.Enqueue(v);
        }
        return visitados;
    }

    // ====== DIJKSTRA ======
    public static List<NodoGrafo> dijkstra(NodoGrafo inicio, NodoGrafo destino)
    {
        var vacio = new List<NodoGrafo>();
        if (inicio == null || destino == null) return vacio;

        var todos = recolectarDesde(inicio);

        foreach (var n in todos)
        {
            n.distancia = float.MaxValue; // usamos "distancia" como gCost
            n.previo = null;
        }
        inicio.distancia = 0f;

        var abiertos = new List<NodoGrafo>(todos);
        var cerrados = new HashSet<NodoGrafo>();

        while (abiertos.Count > 0)
        {
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

            if (actual == destino) break;

            foreach (var v in actual.vecinos)
            {
                if (v == null || cerrados.Contains(v)) continue;

                float nuevo = actual.distancia + costoEntre(actual, v);
                if (nuevo < v.distancia)
                {
                    v.distancia = nuevo;
                    v.previo = actual;
                }
            }
        }

        if (destino != inicio && destino.previo == null) return vacio;
        return reconstruirCamino(destino);
    }

    // ====== A* ======
    public static List<NodoGrafo> aStar(NodoGrafo inicio, NodoGrafo destino)
    {
        var vacio = new List<NodoGrafo>();
        if (inicio == null || destino == null) return vacio;

        var todos = recolectarDesde(inicio);

        // g = distancia, f = g + h (lo llevamos aparte)
        var fScore = new Dictionary<NodoGrafo, float>(todos.Count);

        foreach (var n in todos)
        {
            n.distancia = float.MaxValue; // g
            n.previo = null;
            fScore[n] = float.MaxValue;
        }

        inicio.distancia = 0f;
        fScore[inicio] = heuristica(inicio, destino);

        var abiertos = new List<NodoGrafo> { inicio };
        var cerrados = new HashSet<NodoGrafo>();

        while (abiertos.Count > 0)
        {
            // Tomo el de menor f
            int idxMin = 0;
            float minF = fScore[abiertos[0]];
            for (int i = 1; i < abiertos.Count; i++)
            {
                float f = fScore[abiertos[i]];
                if (f < minF)
                {
                    minF = f;
                    idxMin = i;
                }
            }

            var actual = abiertos[idxMin];
            if (actual == destino) return reconstruirCamino(destino);

            abiertos.RemoveAt(idxMin);
            cerrados.Add(actual);

            foreach (var v in actual.vecinos)
            {
                if (v == null || cerrados.Contains(v)) continue;

                float tentativeG = actual.distancia + costoEntre(actual, v);
                if (tentativeG < v.distancia)
                {
                    v.previo = actual;
                    v.distancia = tentativeG;
                    fScore[v] = tentativeG + heuristica(v, destino);

                    if (!abiertos.Contains(v))
                        abiertos.Add(v);
                }
            }
        }

        return vacio;
    }

    // ====== THETA* (opcional, línea de vista) ======
    public static List<NodoGrafo> thetaStar(NodoGrafo inicio, NodoGrafo destino)
    {
        var vacio = new List<NodoGrafo>();
        if (inicio == null || destino == null) return vacio;

        var todos = recolectarDesde(inicio);
        var fScore = new Dictionary<NodoGrafo, float>(todos.Count);

        foreach (var n in todos)
        {
            n.distancia = float.MaxValue; // g
            n.previo = null;
            fScore[n] = float.MaxValue;
        }

        inicio.distancia = 0f;
        fScore[inicio] = heuristica(inicio, destino);
        inicio.previo = inicio; // parent de la raíz es ella misma

        var abiertos = new List<NodoGrafo> { inicio };
        var cerrados = new HashSet<NodoGrafo>();

        while (abiertos.Count > 0)
        {
            int idxMin = 0;
            float minF = fScore[abiertos[0]];
            for (int i = 1; i < abiertos.Count; i++)
            {
                float f = fScore[abiertos[i]];
                if (f < minF) { minF = f; idxMin = i; }
            }

            var actual = abiertos[idxMin];
            if (actual == destino) return reconstruirCamino(destino);

            abiertos.RemoveAt(idxMin);
            cerrados.Add(actual);

            foreach (var v in actual.vecinos)
            {
                if (v == null || cerrados.Contains(v)) continue;

                // Parent candidate = parent(actual) si hay línea de vista
                NodoGrafo padre = actual.previo != null ? actual.previo : actual;

                float nuevoG;
                NodoGrafo nuevoPadre;

                if (padre != null && tieneLineaVista(padre.posicion, v.posicion))
                {
                    nuevoG = padre.distancia + costoEntre(padre, v);
                    nuevoPadre = padre;
                }
                else
                {
                    nuevoG = actual.distancia + costoEntre(actual, v);
                    nuevoPadre = actual;
                }

                if (nuevoG < v.distancia)
                {
                    v.distancia = nuevoG;
                    v.previo = nuevoPadre;
                    fScore[v] = nuevoG + heuristica(v, destino);

                    if (!abiertos.Contains(v))
                        abiertos.Add(v);
                }
            }
        }

        return vacio;
    }
}
