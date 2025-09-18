using NUnit.Framework.Interfaces;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private PlayerView view;

    [Header("Modelo (datos)")]
    public PlayerModel model = new PlayerModel();

    // Estrategia de movimiento (se puede reemplazar por CharacterController, Rigidbody, etc.)
    private IMove moveStrategy;

    // FSM + estados
    private FSM fsm;
    private IdleState idleState;
    private WalkState walkState;
    private RunState runState;
    private RunToStopState runToStopState;
    private PunchState punchState;

    private void Awake()
    {
        if (!view) view = GetComponentInChildren<PlayerView>();

        var rb = GetComponent<Rigidbody>();
        moveStrategy = new RigidbodyMove(rb, model, view ? view.ModelRoot : null);

        fsm = new FSM();
        idleState = new IdleState(this, fsm, model, view);
        walkState = new WalkState(this, fsm, model, view);
        runState = new RunState(this, fsm, model, view);        // nuevo
        runToStopState = new RunToStopState(this, fsm, model, view);  // nuevo
        punchState = new PunchState(this, fsm, model, view);      // nuevo
    }

    private void Start()
    {
        fsm.Initialize(idleState);
    }

    private void Update()
    {
        fsm.CurrentState.HandleInput();
        fsm.CurrentState.LogicUpdate();
    }

    private void FixedUpdate()
    {
        fsm.CurrentState.PhysicsUpdate();
    }

    // ======= Helpers que usan los estados =======

    // Lee WASD/Arrows con Input clásico
    public Vector3 ReadMovementInput()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        return new Vector3(h, 0f, v).normalized;
    }

    // Convierte el input a espacio de cámara (WASD relativo a cámara)
    public Vector3 ToCameraSpace(Vector3 inputDir)
    {
        if (inputDir.sqrMagnitude < 0.0001f) return Vector3.zero;

        var cam = Camera.main;
        if (!cam) return inputDir;

        Vector3 camFwd = Vector3.Scale(cam.transform.forward, new Vector3(1, 0, 1)).normalized;
        Vector3 camRight = Vector3.Scale(cam.transform.right, new Vector3(1, 0, 1)).normalized;

        return (camFwd * inputDir.z + camRight * inputDir.x).normalized;
    }

    public void Move(Vector3 worldDir)  // compat: usa velocidad de caminar por defecto
    {
        moveStrategy.Move(worldDir, model.GetSpeed(false));
    }

    public void Move(Vector3 worldDir, float speed) // para estados que quieran fijar velocidad
    {
        moveStrategy.Move(worldDir, speed);
    }

    // Accesos que usan los estados para cambiar
    public IdleState Idle => idleState;
    public WalkState Walk => walkState;
    public RunState Run => runState;
    public RunToStopState RunToStop => runToStopState;
    public PunchState Punch => punchState;
}
