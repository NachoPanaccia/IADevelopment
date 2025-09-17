using UnityEngine;


/// Controla y arma la FSM del enemigo.

[RequireComponent(typeof(EnemigoModel))]
public class EnemigoController : MonoBehaviour
{
    [Header("Parámetros de comportamiento")]
    [SerializeField] private int iteracionesParaIdle = 5;   // cuántas veces patrulla antes de quedarse quieto en idle
    [SerializeField] private float tiempoIdle = 2f;         // cuánto tiempo dura el idle

    [Header("Debug (solo lectura)")]
    [SerializeField] private string estadoActual = "Desconocido"; // solo para ver en el inspector qué estado tiene ahora

    private FSMEnemigo<EnemyStates> fsm;   // la máquina de estados
    private EnemigoModel modelo;           // el modelo del enemigo, con todos los datos

    // Estados que vamos a usar
    private PatrullaEnemigoState estadoPatrulla;
    private HuirEnemigoState estadoHuir;
    private IdleEnemigoState estadoIdle;
    private AttackEnemigoState estadoAttack;

    private void Awake()
    {
        // agarramos el componente del modelo, si no está explota con error
        modelo = GetComponent<EnemigoModel>();
        if (modelo == null)
        {
            Debug.LogError("[EnemigoController] Falta EnemigoModel.");
            enabled = false;
            return;
        }

        // creamos la FSM
        fsm = new FSMEnemigo<EnemyStates>();

        // instanciamos cada estado con sus parámetros
        estadoPatrulla = new PatrullaEnemigoState(modelo, iteracionesParaIdle);
        estadoHuir = new HuirEnemigoState(modelo);
        estadoIdle = new IdleEnemigoState(modelo, tiempoIdle);
        estadoAttack = new AttackEnemigoState(modelo);

        // le decimos a cada estado a qué fsm pertenece
        estadoPatrulla.SetFSM(fsm);
        estadoHuir.SetFSM(fsm);
        estadoIdle.SetFSM(fsm);
        estadoAttack.SetFSM(fsm);

        // definimos transiciones entre estados
        estadoPatrulla.AddTransition(EnemyStates.Huir, estadoHuir);
        estadoPatrulla.AddTransition(EnemyStates.Idle, estadoIdle);
        estadoPatrulla.AddTransition(EnemyStates.Attack, estadoAttack);

        estadoIdle.AddTransition(EnemyStates.Huir, estadoHuir);
        estadoIdle.AddTransition(EnemyStates.Patrulla, estadoPatrulla);
        estadoIdle.AddTransition(EnemyStates.Attack, estadoAttack);

        estadoHuir.AddTransition(EnemyStates.Patrulla, estadoPatrulla);
        estadoHuir.AddTransition(EnemyStates.Attack, estadoAttack); // esto es opcional, de huir a atacar

        estadoAttack.AddTransition(EnemyStates.Patrulla, estadoPatrulla);
        estadoAttack.AddTransition(EnemyStates.Huir, estadoHuir);

        // arrancamos siempre en patrulla
        fsm.SetInitialState(estadoPatrulla);
        estadoActual = "Patrulla";
    }

    private void Update()
    {
        // cada frame le pedimos a la fsm que ejecute el estado actual
        fsm.OnUpdate();
        
    }
}
