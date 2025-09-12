using UnityEngine;

/// <summary>
/// Controlador principal del Enemigo.
/// - Arma la FSMEnemigo<EnemyStates> y registra los estados/transiciones.
/// - Corre el ciclo de la FSM en Update().
/// Requiere: EnemigoModel, FSMEnemigo<T>, EstadoEnemigo<T>, PatrullaEnemigoState, HuirEnemigoState, IdleEnemigoState.
/// </summary>
[RequireComponent(typeof(EnemigoModel))]
public class EnemigoController : MonoBehaviour
{
    [Header("Parámetros de comportamiento")]
    [SerializeField] private int iteracionesParaIdle = 5;
    [SerializeField] private float tiempoIdle = 2f;

    [Header("Debug (solo lectura)")]
    [SerializeField] private string estadoActual = "Desconocido"; // visible en el Inspector

    // Core
    private FSMEnemigo<EnemyStates> fsm;
    private EnemigoModel modelo;

    // Estados
    private PatrullaEnemigoState estadoPatrulla;
    private HuirEnemigoState estadoHuir;
    private IdleEnemigoState estadoIdle;

    private void Awake()
    {
        // Refs obligatorias
        modelo = GetComponent<EnemigoModel>();
        if (modelo == null)
        {
            Debug.LogError("[EnemigoController] Falta EnemigoModel en el GameObject.");
            enabled = false;
            return;
        }

        // Instanciar FSM y estados
        fsm = new FSMEnemigo<EnemyStates>();

        estadoPatrulla = new PatrullaEnemigoState(modelo, iteracionesParaIdle);
        estadoHuir = new HuirEnemigoState(modelo);
        estadoIdle = new IdleEnemigoState(modelo, tiempoIdle);

        // Inyectar FSM en los estados
        estadoPatrulla.SetFSM(fsm);
        estadoHuir.SetFSM(fsm);
        estadoIdle.SetFSM(fsm);

        // Transiciones
        estadoPatrulla.AddTransition(EnemyStates.Huir, estadoHuir);
        estadoPatrulla.AddTransition(EnemyStates.Idle, estadoIdle);

        estadoIdle.AddTransition(EnemyStates.Huir, estadoHuir);
        estadoIdle.AddTransition(EnemyStates.Patrulla, estadoPatrulla);

        estadoHuir.AddTransition(EnemyStates.Patrulla, estadoPatrulla);

        // Estado inicial
        fsm.SetInitialState(estadoPatrulla);
        estadoActual = "Patrulla";
    }

    private void Update()
    {
        fsm.OnUpdate();

        // Debug simple del estado actual (heurística por sensor)
        if (modelo.Sensor != null && modelo.Sensor.ObjetivoVisible)
            estadoActual = "Huir";
        else
            estadoActual = "Patrulla/Idle";
    }
}


