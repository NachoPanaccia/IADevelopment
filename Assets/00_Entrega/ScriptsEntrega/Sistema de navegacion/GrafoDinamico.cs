using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Genera un grafo en torno al jugador, marca nodos caminables/no caminables
/// y (opcional) los muestra como esferas en runtime o como Gizmos.
/// </summary>
public class GrafoDinamico : MonoBehaviour
{
    [Header("Parámetros de grilla")]
    [SerializeField] Vector2Int tamañoGrilla = new Vector2Int(20, 20);
    [SerializeField] float tamañoCelda = 1f;
    [SerializeField] float alturaNodos = 0f; // Y donde se "apoya" la grilla
    [SerializeField] LayerMask capaObstaculo;

    [Header("Centro del grafo")]
    [SerializeField] Transform jugador;

    [Header("Conectividad")]
    [SerializeField] bool usarDiagonales = false; // si querés 8 direcciones
    [SerializeField] bool evitarCorteEsquinas = true; // para diagonales, evita atravesar esquinas

    [Header("Actualización dinámica")]
    [SerializeField] bool regenerarAutomaticamente = true;
    [SerializeField] float intervaloRegeneracion = 0.25f;
    [SerializeField] bool habilitarLogs = false;

    [Header("Visualización con Gizmos (Scene/Play)")]
    [SerializeField] bool dibujarConGizmos = true;
    [SerializeField] float radioEsferaGizmos = 0.15f;
    [SerializeField] Color colorCaminable = new Color(0f, 0.8f, 0.25f, 0.95f);
    [SerializeField] Color colorBloqueado = new Color(0.9f, 0.15f, 0.15f, 0.95f);
    [SerializeField] bool dibujarVecinosGizmos = false;
    [SerializeField] Color colorVecino = new Color(1f, 0.95f, 0.2f, 0.95f);

    [Header("Visualización en runtime (esferas)")]
    [SerializeField] bool instanciarEsferasRuntime = false;
    [SerializeField] GameObject prefabEsferaNodo; // opcional, si no hay creo primitivas
    [SerializeField] float escalaEsferaRuntime = 0.2f;
    [SerializeField] Transform contenedorEsferas; // opcional

    NodoGrafo[,] nodos;
    float acumuladorTiempo;
    readonly List<Renderer> esferasInstanciadas = new();
    MaterialPropertyBlock bloqueColor;
    static readonly int idColor = Shader.PropertyToID("_Color");

    void Awake()
    {
        bloqueColor = new MaterialPropertyBlock();
    }

    void Start()
    {
        if (jugador == null)
        {
            Debug.LogWarning("[GrafoDinamico] Falta asignar 'jugador' en el Inspector.");
            return;
        }

        generarGrafo();
        if (instanciarEsferasRuntime) refrescarEsferasRuntime();
    }

    void Update()
    {
        if (!regenerarAutomaticamente || jugador == null) return;

        acumuladorTiempo += Time.deltaTime;
        if (acumuladorTiempo >= intervaloRegeneracion)
        {
            acumuladorTiempo = 0f;
            generarGrafo();
            if (instanciarEsferasRuntime) refrescarEsferasRuntime();
        }
    }

    /// <summary>
    /// Genera/actualiza el grafo alrededor del jugador y conecta vecinos.
    /// </summary>
    public void generarGrafo()
    {
        if (jugador == null)
        {
            Debug.LogError("[GrafoDinamico] No hay jugador asignado, no puedo generar el grafo.");
            return;
        }

        nodos = new NodoGrafo[tamañoGrilla.x, tamañoGrilla.y];
        Vector3 centro = new Vector3(jugador.position.x, alturaNodos, jugador.position.z);
        Vector3 origen = centro - new Vector3(tamañoGrilla.x, 0f, tamañoGrilla.y) * (tamañoCelda * 0.5f);

        // 1) Crear nodos
        for (int x = 0; x < tamañoGrilla.x; x++)
        {
            for (int y = 0; y < tamañoGrilla.y; y++)
            {
                Vector3 pos = origen + new Vector3(x * tamañoCelda, 0f, y * tamañoCelda);

                // Caja de chequeo: XZ del tamaño de la celda, una altura estándar (1m) para detectar obstáculo
                Vector3 semiExt = new Vector3(tamañoCelda * 0.45f, 0.5f, tamañoCelda * 0.45f);
                bool bloqueado = Physics.CheckBox(pos + Vector3.up * semiExt.y, semiExt, Quaternion.identity, capaObstaculo);

                var nodo = new NodoGrafo
                {
                    posicion = pos,
                    caminable = !bloqueado,
                    vecinos = new List<NodoGrafo>(usarDiagonales ? 8 : 4),
                    distancia = float.MaxValue,
                    previo = null
                };
                nodos[x, y] = nodo;
            }
        }

        // 2) Conectar vecinos
        for (int x = 0; x < tamañoGrilla.x; x++)
        {
            for (int y = 0; y < tamañoGrilla.y; y++)
            {
                var n = nodos[x, y];
                if (n == null || !n.caminable) continue;

                agregarVecino(n, x - 1, y);
                agregarVecino(n, x + 1, y);
                agregarVecino(n, x, y - 1);
                agregarVecino(n, x, y + 1);

                if (usarDiagonales)
                {
                    // Diagonales con control de corte de esquina
                    agregarDiagonal(n, x - 1, y - 1, x - 1, y, x, y - 1);
                    agregarDiagonal(n, x - 1, y + 1, x - 1, y, x, y + 1);
                    agregarDiagonal(n, x + 1, y - 1, x + 1, y, x, y - 1);
                    agregarDiagonal(n, x + 1, y + 1, x + 1, y, x, y + 1);
                }
            }
        }

        if (habilitarLogs)
            Debug.Log($"[GrafoDinamico] Grafo regenerado. Nodos: {tamañoGrilla.x * tamañoGrilla.y}");
    }

    void agregarVecino(NodoGrafo nodo, int x, int y)
    {
        if (x < 0 || x >= tamañoGrilla.x || y < 0 || y >= tamañoGrilla.y) return;
        var v = nodos[x, y];
        if (v != null && v.caminable)
            nodo.vecinos.Add(v);
    }

    void agregarDiagonal(NodoGrafo nodo, int xDiag, int yDiag, int xCard1, int yCard1, int xCard2, int yCard2)
    {
        if (xDiag < 0 || xDiag >= tamañoGrilla.x || yDiag < 0 || yDiag >= tamañoGrilla.y) return;

        var vDiag = nodos[xDiag, yDiag];
        if (vDiag == null || !vDiag.caminable) return;

        if (evitarCorteEsquinas)
        {
            // Evito pasar por esquinas bloqueadas (ambos cardinales deben ser caminables)
            var c1 = (xCard1 >= 0 && xCard1 < tamañoGrilla.x && yCard1 >= 0 && yCard1 < tamañoGrilla.y) ? nodos[xCard1, yCard1] : null;
            var c2 = (xCard2 >= 0 && xCard2 < tamañoGrilla.x && yCard2 >= 0 && yCard2 < tamañoGrilla.y) ? nodos[xCard2, yCard2] : null;
            if (c1 == null || c2 == null || !c1.caminable || !c2.caminable) return;
        }

        nodo.vecinos.Add(vDiag);
    }

    /// <summary>
    /// Devuelve el nodo caminable más cercano a una posición. Si no hay, devuelve null.
    /// </summary>
    public NodoGrafo buscarMasCercano(Vector3 posicion)
    {
        if (nodos == null) return null;

        NodoGrafo mejor = null;
        float mejorDist = float.MaxValue;

        foreach (var n in nodos)
        {
            if (n == null || !n.caminable) continue;
            float d = Vector3.Distance(posicion, n.posicion);
            if (d < mejorDist)
            {
                mejorDist = d;
                mejor = n;
            }
        }
        return mejor;
    }

    // =========================
    // Visualización con Gizmos
    // =========================
    void OnDrawGizmos()
    {
        if (!dibujarConGizmos || nodos == null) return;

        for (int x = 0; x < nodos.GetLength(0); x++)
        {
            for (int y = 0; y < nodos.GetLength(1); y++)
            {
                var n = nodos[x, y];
                if (n == null) continue;

                Gizmos.color = n.caminable ? colorCaminable : colorBloqueado;
                Vector3 p = n.posicion + Vector3.up * 0.02f;
                Gizmos.DrawSphere(p, radioEsferaGizmos);

                if (dibujarVecinosGizmos && n.caminable)
                {
                    Gizmos.color = colorVecino;
                    foreach (var v in n.vecinos)
                    {
                        if (v == null) continue;
                        Gizmos.DrawLine(p, v.posicion + Vector3.up * 0.02f);
                    }
                }
            }
        }
    }

    // =====================================
    // Visualización mediante instancias (GO)
    // =====================================
    void refrescarEsferasRuntime()
    {
        if (nodos == null) return;

        int total = nodos.GetLength(0) * nodos.GetLength(1);

        // Crear faltantes
        while (esferasInstanciadas.Count < total)
            esferasInstanciadas.Add(crearEsfera());

        // Desactivar sobrantes
        for (int i = total; i < esferasInstanciadas.Count; i++)
            if (esferasInstanciadas[i] != null)
                esferasInstanciadas[i].gameObject.SetActive(false);

        // Posicionar/colorear activas
        int idx = 0;
        for (int x = 0; x < nodos.GetLength(0); x++)
        {
            for (int y = 0; y < nodos.GetLength(1); y++)
            {
                var n = nodos[x, y];
                var r = esferasInstanciadas[idx++];
                if (r == null) continue;

                r.gameObject.SetActive(true);
                r.transform.position = n.posicion;
                r.transform.localScale = Vector3.one * escalaEsferaRuntime;

                aplicarColor(r, n.caminable ? colorCaminable : colorBloqueado);
            }
        }
    }

    Renderer crearEsfera()
    {
        GameObject go;
        if (prefabEsferaNodo != null)
        {
            go = Instantiate(prefabEsferaNodo, contenedorEsferas ? contenedorEsferas : transform);
        }
        else
        {
            go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            if (contenedorEsferas) go.transform.SetParent(contenedorEsferas, true);
            else go.transform.SetParent(transform, true);

            var col = go.GetComponent<Collider>();
            if (col) Destroy(col);
            if (!go.TryGetComponent<MeshRenderer>(out _)) go.AddComponent<MeshRenderer>();
            if (!go.TryGetComponent<MeshFilter>(out _)) go.AddComponent<MeshFilter>();
        }

        go.name = "EsferaNodo";
        return go.GetComponentInChildren<Renderer>();
    }

    void aplicarColor(Renderer renderer, Color color)
    {
        bloqueColor.Clear();
        bloqueColor.SetColor(idColor, color);
        renderer.SetPropertyBlock(bloqueColor);
    }

    // =================
    // Utilidades Editor
    // =================
    [ContextMenu("Generar grafo (manual)")]
    void generarManual()
    {
        generarGrafo();
        if (instanciarEsferasRuntime) refrescarEsferasRuntime();
    }

    [ContextMenu("Limpiar esferas runtime")]
    void limpiarEsferasRuntime()
    {
        foreach (var r in esferasInstanciadas)
            if (r != null) DestroyImmediate(r.gameObject);
        esferasInstanciadas.Clear();
    }
}


