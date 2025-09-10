using UnityEngine;

/// <summary>
/// Controlador principal del Enemigo.
/// - Arma la FSMEnemigo<EnemyStates> y registra los estados/transiciones.
/// - Corre el ciclo de la FSM en Update().
/// Requiere: EnemigoModel, FSMEnemigo<T>, EstadoEnemigo<T>, PatrullaEnemigoState, HuirEnemigoState.
/// </summary>
[RequireComponent(typeof(EnemigoModel))]
public class EnemigoController : MonoBehaviour
{
    [Header("Debug (solo lectura)")]
    [SerializeField] private string estadoActual = "Desconocido"; // para ver en el Inspector

    // Core
    private FSMEnemigo<EnemyStates> fsm;   // tu FSM genérica para Enemigo
    private EnemigoModel modelo;           // datos y helpers (velocidades, puntos, sensor, etc.)

    // Estados
    private PatrullaEnemigoState patrulla;
    private HuirEnemigoState huir;

    // ===== Ciclo de vida =====
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

        patrulla = new PatrullaEnemigoState(modelo);
        huir = new HuirEnemigoState(modelo);

        // Inyectar FSM a los estados
        patrulla.SetFSM(fsm);
        huir.SetFSM(fsm);

        // Transiciones
        patrulla.AddTransition(EnemyStates.Huir, huir);
        huir.AddTransition(EnemyStates.Patrulla, patrulla);

        // Estado inicial
        fsm.SetInitialState(patrulla);
        estadoActual = "Patrulla";
    }

    private void Update()
    {
        // Avanzar la FSM
        fsm.OnUpdate();

        // Nota: nuestra FSMEnemigo<T> no expone el estado actual.
        // Para debug simple, inferimos según el sensor (suele coincidir con el comportamiento).
        if (modelo.Sensor != null && modelo.Sensor.ObjetivoVisible)
            estadoActual = "Huir";
        else
            estadoActual = "Patrulla";
    }
}

