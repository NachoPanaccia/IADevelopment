using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class PathVisualizer : MonoBehaviour
{
    [SerializeField] float anchoLinea = 0.05f;
    [SerializeField] bool usarEspacioMundo = true;

    LineRenderer line;

    void Awake()
    {
        line = GetComponent<LineRenderer>();
        line.useWorldSpace = usarEspacioMundo;
        line.widthMultiplier = anchoLinea;
    }

    /// <summary>
    /// Dibuja el camino. Si la lista es nula o vacía, limpia la línea.
    /// </summary>
    public void dibujar(List<NodoGrafo> camino)
    {
        if (camino == null || camino.Count == 0)
        {
            line.positionCount = 0;
            return;
        }

        line.positionCount = camino.Count;
        for (int i = 0; i < camino.Count; i++)
            line.SetPosition(i, camino[i].posicion + Vector3.up * 0.1f);
    }
}
