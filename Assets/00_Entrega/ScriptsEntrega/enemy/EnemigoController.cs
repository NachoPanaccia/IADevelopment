using UnityEngine;

/// <summary>
/// Controla y arma la FSM del enemigo.
/// </summary>
[RequireComponent(typeof(EnemigoModel))]
public class EnemigoController : MonoBehaviour
{
    [Header("Parámetros de comportamiento")]
    [SerializeField] private int iteracionesParaIdle = 5;
    [SerializeField] private float tiempoIdle = 2f;

    [Header("Debug (solo lectura)")]
    [SerializeField] private string estadoActual = "Desconocido";

    private FSMEnemigo<EnemyStates> fsm;
    private EnemigoModel modelo;

    // Estados
    private PatrullaEnemigoState estadoPatrulla;
    private HuirEnemigoState estadoHuir;
    private IdleEnemigoState estadoIdle;
    private AttackEnemigoState estadoAttack;

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

        estadoPatrulla = new PatrullaEnemigoState(modelo, iteracionesParaIdle);
        estadoHuir = new HuirEnemigoState(modelo);
        estadoIdle = new IdleEnemigoState(modelo, tiempoIdle);
        estadoAttack = new AttackEnemigoState(modelo);

        estadoPatrulla.SetFSM(fsm);
        estadoHuir.SetFSM(fsm);
        estadoIdle.SetFSM(fsm);
        estadoAttack.SetFSM(fsm);

        // Transiciones
        estadoPatrulla.AddTransition(EnemyStates.Huir, estadoHuir);
        estadoPatrulla.AddTransition(EnemyStates.Idle, estadoIdle);
        estadoPatrulla.AddTransition(EnemyStates.Attack, estadoAttack);

        estadoIdle.AddTransition(EnemyStates.Huir, estadoHuir);
        estadoIdle.AddTransition(EnemyStates.Patrulla, estadoPatrulla);
        estadoIdle.AddTransition(EnemyStates.Attack, estadoAttack);

        estadoHuir.AddTransition(EnemyStates.Patrulla, estadoPatrulla);
        estadoHuir.AddTransition(EnemyStates.Attack, estadoAttack); // opcional: de huir a atacar

        estadoAttack.AddTransition(EnemyStates.Patrulla, estadoPatrulla);
        estadoAttack.AddTransition(EnemyStates.Huir, estadoHuir);

        fsm.SetInitialState(estadoPatrulla);
        estadoActual = "Patrulla";
    }

    private void Update()
    {
        fsm.OnUpdate();
        // si tu FSM expone nombre actual, podés actualizarlo acá
       
    }
}

