// Archivo: LoboLider.cs
using UnityEngine;

[RequireComponent(typeof(EntidadSteering))]
public class LoboLider : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] BandadaLobos bandada;
    [SerializeField] DeteccionLineaDeVista deteccion; // tu script

    [Header("Parámetros")]
    [SerializeField] float refrescoObjetivo = 0.25f;
    [SerializeField] bool usarLecturaDirectaDeteccion = true;

    [Header("Debug")]
    [SerializeField] bool habilitarLogs = false;

    float temporizador;

    void Reset()
    {
        bandada = FindAnyObjectByType<BandadaLobos>();
        deteccion = GetComponent<DeteccionLineaDeVista>();
    }

    void Awake()
    {
        if (bandada == null) bandada = FindAnyObjectByType<BandadaLobos>();
    }

    void Update()
    {
        if (!usarLecturaDirectaDeteccion || deteccion == null) return;

        temporizador -= Time.deltaTime;

        bool enVista = false;
        Vector3 posObjetivo = Vector3.zero;

        // Intentar leer propiedades típicas (ajustá nombres si difiere tu detector)
        try
        {
            var tipo = deteccion.GetType();
            var propVista = tipo.GetProperty("EnVistaJugador");
            var propUlt = tipo.GetProperty("UltimaPosicionVista");
            if (propVista != null) enVista = (bool)propVista.GetValue(deteccion);
            if (propUlt != null) posObjetivo = (Vector3)propUlt.GetValue(deteccion);
        }
        catch { }

        if (enVista && temporizador <= 0f)
        {
            temporizador = refrescoObjetivo;
            bandada?.ActivarAlerta(posObjetivo, refrescoObjetivo);
            if (habilitarLogs) Debug.Log($"[LoboLider] Ve jugador → {posObjetivo}");
        }
    }

    // Llamables por eventos desde tu FSM/detector si preferís
    public void ActivarAlerta(Vector3 posicionObjetivo)
    {
        bandada?.ActivarAlerta(posicionObjetivo, refrescoObjetivo);
        if (habilitarLogs) Debug.Log($"[LoboLider] ActivarAlerta({posicionObjetivo})");
    }

    public void DesactivarAlerta()
    {
        bandada?.DesactivarAlerta();
        if (habilitarLogs) Debug.Log("[LoboLider] DesactivarAlerta()");
    }
}
