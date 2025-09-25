using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private PlayerView view;   //capa visual

    [Header("Datos")]
    public PlayerModel model = new PlayerModel();

    private IMove moveStrategy;

    // FSM + estados
    private FSM fsm;
    private IdleState idleState;
    private WalkState walkState;
    private RunState runState;
    private RunToStopState runToStopState;
    private PunchState punchState;

    void Awake()
    {
        if (!view) view = GetComponentInChildren<PlayerView>();

        var rb = GetComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        moveStrategy = new RigidbodyMove(rb, model, view ? view.ModelRoot : null);

        // FSM + estados
        fsm = new FSM();
        idleState = new IdleState(this, fsm, model, view);
        walkState = new WalkState(this, fsm, model, view);
        runState = new RunState(this, fsm, model, view);
        runToStopState = new RunToStopState(this, fsm, model, view);
        punchState = new PunchState(this, fsm, model, view);
    }

    void Start() => fsm.Initialize(idleState);
    void Update() => fsm.Execute();
    void FixedUpdate() => fsm.FixedExecute();

    public Vector3 ReadMovementInput()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        return new Vector3(h, 0f, v).normalized;
    }

    /// Proyecta el input al espacio de cámara (mover “hacia donde miro”).
    public Vector3 ToCameraSpace(Vector3 inputDir)
    {
        if (inputDir.sqrMagnitude < 0.0001f) return Vector3.zero;
        var cam = Camera.main;
        if (!cam) return inputDir;

        Vector3 camFwd = Vector3.Scale(cam.transform.forward, new Vector3(1, 0, 1)).normalized;
        Vector3 camRight = Vector3.Scale(cam.transform.right, new Vector3(1, 0, 1)).normalized;
        return (camFwd * inputDir.z + camRight * inputDir.x).normalized;
    }

    public void Move(Vector3 worldDir) => moveStrategy.Move(worldDir, model.walkSpeed);

    public void Move(Vector3 worldDir, float speed) => moveStrategy.Move(worldDir, speed);

    // Accesos a los estados
    public IdleState Idle => idleState;
    public WalkState Walk => walkState;
    public RunState Run => runState;
    public RunToStopState RunToStop => runToStopState;
    public PunchState Punch => punchState;
}
