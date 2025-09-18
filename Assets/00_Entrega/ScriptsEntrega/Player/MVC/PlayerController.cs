using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private PlayerView view;

    [Header("Modelo (datos)")]
    public PlayerModel model = new PlayerModel();

    IMove moveStrategy;

    FSM fsm;
    IdleState idleState;
    WalkState walkState;
    RunState runState;
    RunToStopState runToStopState;
    PunchState punchState;

    void Awake()
    {
        if (!view) view = GetComponentInChildren<PlayerView>();

        var rb = GetComponent<Rigidbody>();
        moveStrategy = new RigidbodyMove(rb, model, view ? view.ModelRoot : null);

        fsm = new FSM();
        idleState = new IdleState(this, fsm, model, view);
        walkState = new WalkState(this, fsm, model, view);
        runState = new RunState(this, fsm, model, view);
        runToStopState = new RunToStopState(this, fsm, model, view);
        punchState = new PunchState(this, fsm, model, view);
    }

    void Start()
    {
        fsm.Initialize(idleState);
    }

    void Update()
    {
        fsm.CurrentState.HandleInput();
        fsm.CurrentState.LogicUpdate();
    }

    void FixedUpdate()
    {
        fsm.CurrentState.PhysicsUpdate();
    }

    public Vector3 ReadMovementInput()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        return new Vector3(h, 0f, v).normalized;
    }

    public Vector3 ToCameraSpace(Vector3 inputDir)
    {
        if (inputDir.sqrMagnitude < 0.0001f) return Vector3.zero;

        var cam = Camera.main;
        if (!cam) return inputDir;

        Vector3 camFwd = Vector3.Scale(cam.transform.forward, new Vector3(1, 0, 1)).normalized;
        Vector3 camRight = Vector3.Scale(cam.transform.right, new Vector3(1, 0, 1)).normalized;

        return (camFwd * inputDir.z + camRight * inputDir.x).normalized;
    }

    public void Move(Vector3 worldDir)
    {
        moveStrategy.Move(worldDir, model.GetSpeed(false));
    }

    public void Move(Vector3 worldDir, float speed)
    {
        moveStrategy.Move(worldDir, speed);
    }

    public IdleState Idle => idleState;
    public WalkState Walk => walkState;
    public RunState Run => runState;
    public RunToStopState RunToStop => runToStopState;
    public PunchState Punch => punchState;
}
