using UnityEngine;

/// Controla y arma la FSM del enemigo.
[RequireComponent(typeof(EnemigoModel))]
public class EnemigoController : MonoBehaviour
{
    [Header("Parámetros de comportamiento")]
    [SerializeField] private int iteracionesParaIdle = 5;   // cuántas veces patrulla antes de quedarse quieto en idle
    [SerializeField] private float tiempoIdle = 2f;         // cuánto dura el idle
    [SerializeField] private float duracionMiedo = 3f;      // NUEVO: cuánto dura Miedo antes de volver a Patrulla

    [Header("Debug (solo lectura)")]
    [SerializeField] private string estadoActual = "Desconocido";

    private FSMEnemigo<EnemyStates> fsm;
    private EnemigoModel modelo;

    // Estados
    private PatrullaEnemigoState estadoPatrulla;
    private HuirEnemigoState estadoHuir;
    private IdleEnemigoState estadoIdle;
    private AttackEnemigoState estadoAttack;
    private MiedoEnemigoState estadoMiedo;

    private void Awake()
    {
        modelo = GetComponent<EnemigoModel>();
        if (modelo == null)
        {
            Debug.LogError("[EnemigoController] Falta EnemigoModel.");
            enabled = false;
            return;
        }

        fsm = new FSMEnemigo<EnemyStates>();

        // Instancias (pasamos por ctor los parámetros que tocan)
        estadoPatrulla = new PatrullaEnemigoState(modelo, iteracionesParaIdle);
        estadoHuir = new HuirEnemigoState(modelo);
        estadoIdle = new IdleEnemigoState(modelo, tiempoIdle);
        estadoAttack = new AttackEnemigoState(modelo);
        estadoMiedo = new MiedoEnemigoState(modelo, duracionMiedo); // ← NUEVO con timer

        // Set FSM en cada estado
        estadoPatrulla.SetFSM(fsm);
        estadoHuir.SetFSM(fsm);
        estadoIdle.SetFSM(fsm);
        estadoAttack.SetFSM(fsm);
        estadoMiedo.SetFSM(fsm);

        
        estadoPatrulla.AddTransition(EnemyStates.Huir, estadoHuir);
        estadoPatrulla.AddTransition(EnemyStates.Idle, estadoIdle);
        estadoPatrulla.AddTransition(EnemyStates.Attack, estadoAttack);
        estadoPatrulla.AddTransition(EnemyStates.Miedo, estadoMiedo);

        estadoIdle.AddTransition(EnemyStates.Huir, estadoHuir);
        estadoIdle.AddTransition(EnemyStates.Patrulla, estadoPatrulla);
        estadoIdle.AddTransition(EnemyStates.Attack, estadoAttack);
        estadoIdle.AddTransition(EnemyStates.Miedo, estadoMiedo);

        estadoHuir.AddTransition(EnemyStates.Patrulla, estadoPatrulla);
        estadoHuir.AddTransition(EnemyStates.Attack, estadoAttack);
        estadoHuir.AddTransition(EnemyStates.Miedo, estadoMiedo);

        estadoAttack.AddTransition(EnemyStates.Patrulla, estadoPatrulla);
        estadoAttack.AddTransition(EnemyStates.Huir, estadoHuir);
        estadoAttack.AddTransition(EnemyStates.Miedo, estadoMiedo);

        // Salidas posibles desde Miedo (por si querés forzar desde afuera)
        estadoMiedo.AddTransition(EnemyStates.Patrulla, estadoPatrulla);
        estadoMiedo.AddTransition(EnemyStates.Huir, estadoHuir);
        estadoMiedo.AddTransition(EnemyStates.Attack, estadoAttack);
        estadoMiedo.AddTransition(EnemyStates.Idle, estadoIdle);

        // Estado inicial
        fsm.SetInitialState(estadoPatrulla);
        estadoActual = "Patrulla";
    }

    private void Update()
    {
        fsm.OnUpdate();
    }
}
