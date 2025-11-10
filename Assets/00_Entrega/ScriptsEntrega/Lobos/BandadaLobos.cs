// Archivo: BandadaLobos.cs
using System.Collections.Generic;
using UnityEngine;

public class BandadaLobos : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] Transform lider;

    [Header("Estado de alerta")]
    [SerializeField] bool alertaActiva = false;
    [SerializeField] Vector3 objetivoAlerta;
    [SerializeField] float tiempoApagadoAlerta = 2.0f;
    float temporizador;

    [Header("Seguidores (solo lectura)")]
    [SerializeField] bool habilitarLogs = false;

    // Registro de seguidores para coordinación/depuración
    readonly HashSet<LoboSeguidorFlocking> seguidores = new HashSet<LoboSeguidorFlocking>();
    public IReadOnlyCollection<LoboSeguidorFlocking> Seguidores => seguidores;

    public Transform Lider => lider;
    public bool AlertaActiva => alertaActiva;
    public Vector3 ObjetivoAlerta => objetivoAlerta;

    void Awake()
    {
        if (lider == null)
        {
            var posible = GetComponentInChildren<LoboLider>();
            if (posible != null) lider = posible.transform;
        }
    }

    void Update()
    {
        if (!alertaActiva) return;

        temporizador -= Time.deltaTime;
        if (temporizador <= 0f)
        {
            DesactivarAlerta();
        }
    }

    // === API de bandada ===
    public void RegistrarSeguidor(LoboSeguidorFlocking seguidor)
    {
        if (seguidor == null) return;
        if (seguidores.Add(seguidor) && habilitarLogs)
            Debug.Log($"[BandadaLobos] Seguidor registrado: {seguidor.name} (total: {seguidores.Count})");
    }

    public void DesregistrarSeguidor(LoboSeguidorFlocking seguidor)
    {
        if (seguidor == null) return;
        if (seguidores.Remove(seguidor) && habilitarLogs)
            Debug.Log($"[BandadaLobos] Seguidor desregistrado: {seguidor.name} (total: {seguidores.Count})");
    }

    public void ActivarAlerta(Vector3 objetivo, float refresco = 0.3f)
    {
        objetivoAlerta = objetivo;
        alertaActiva = true;
        temporizador = Mathf.Max(refresco, 0.05f);
        if (habilitarLogs) Debug.Log($"[BandadaLobos] ALERTA ON → {objetivoAlerta}");
    }

    public void DesactivarAlerta()
    {
        alertaActiva = false;
        if (habilitarLogs) Debug.Log("[BandadaLobos] ALERTA OFF");
    }

    void OnDrawGizmos()
    {
        if (!alertaActiva) return;

        Gizmos.color = new Color(1f, 0.2f, 0.2f, 0.6f);
        Gizmos.DrawSphere(objetivoAlerta + Vector3.up * 0.1f, 0.25f);

        if (lider != null)
        {
            Gizmos.DrawLine(lider.position + Vector3.up * 0.2f, objetivoAlerta + Vector3.up * 0.2f);
        }
    }
}

