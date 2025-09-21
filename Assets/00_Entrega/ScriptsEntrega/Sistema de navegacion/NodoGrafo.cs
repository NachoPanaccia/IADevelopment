using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Nodo básico para pathfinding sobre grilla en mundo.
/// </summary>
public class NodoGrafo
{
    public Vector3 posicion;
    public bool caminable;
    public List<NodoGrafo> vecinos = new List<NodoGrafo>();

    // Campos para Dijkstra/A*
    public float distancia = float.MaxValue;
    public NodoGrafo previo = null;
}
