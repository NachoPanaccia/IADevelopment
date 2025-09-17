using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] Transform target;
    [SerializeField] Vector3 targetOffset = new Vector3(0f, 1.6f, 0f);

    [Header("Distancia")]
    [SerializeField] float distance = 4f;
    [SerializeField] float minDistance = 2f;
    [SerializeField] float maxDistance = 6f;

    [Header("Órbita (Mouse)")]
    [SerializeField] float mouseSensitivity = 120f; // grados/segundo
    [SerializeField] float pitchMin = -20f;
    [SerializeField] float pitchMax = 70f;

    [Header("Suavizado")]
    [SerializeField] float followLerp = 12f; // mayor = más rígido

    [Header("Colisiones")]
    [SerializeField] float collisionRadius = 0.2f;
    [SerializeField] LayerMask collisionMask = ~0; // Everything por defecto

    [Header("Invertir ejes")]
    [SerializeField] bool invertX = false; // gira a la inversa en horizontal
    [SerializeField] bool invertY = false;

    [SerializeField] private bool lockCursor = true;

    // estado interno
    float _yaw;
    float _pitch;

    void OnEnable()
    {
        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void OnDisable()
    {
        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    void Start()
    {
        if (target)
        {
            // alineamos yaw al inicio con la orientación del target
            _yaw = target.eulerAngles.y;
        }
        _pitch = Mathf.Clamp(_pitch, pitchMin, pitchMax);
        distance = Mathf.Clamp(distance, minDistance, maxDistance);
    }

    void LateUpdate()
    {
        if (!target) return;

        float dt = Time.deltaTime;

        // Entrada de mouse
        float mx = Input.GetAxis("Mouse X");
        float my = Input.GetAxis("Mouse Y");

        // signos (original: X = +, Y = -)
        float signX = invertX ? -1f : 1f;
        float signY = invertY ? 1f : -1f;

        _yaw += mx * mouseSensitivity * dt * signX;
        _pitch += my * mouseSensitivity * dt * signY;
        _pitch = Mathf.Clamp(_pitch, pitchMin, pitchMax);

        // Zoom con rueda
        float wheel = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(wheel) > 0.0001f)
        {
            distance = Mathf.Clamp(distance - wheel * 2f, minDistance, maxDistance);
        }

        // Rotación deseada
        Quaternion rot = Quaternion.Euler(_pitch, _yaw, 0f);

        // Pivot (punto a mirar) y posición ideal
        Vector3 pivot = target.position + targetOffset;
        Vector3 idealCamPos = pivot - (rot * Vector3.forward) * distance;

        // Colisión: acortamos si hay obstáculo
        float adjustedDist = distance;
        Vector3 toCam = idealCamPos - pivot;
        float len = toCam.magnitude;
        if (len > 0.001f)
        {
            Vector3 dir = toCam / len;
            if (Physics.SphereCast(pivot, collisionRadius, dir, out RaycastHit hit, distance, collisionMask, QueryTriggerInteraction.Ignore))
            {
                adjustedDist = Mathf.Max(0.1f, hit.distance - 0.05f);
            }
        }

        Vector3 targetCamPos = pivot - (rot * Vector3.forward) * adjustedDist;

        // Suavizado de posición
        float t = 1f - Mathf.Exp(-followLerp * dt);
        transform.position = Vector3.Lerp(transform.position, targetCamPos, t);

        // Mirar al pivot
        transform.rotation = Quaternion.LookRotation(pivot - transform.position, Vector3.up);
    }

    /// Llamalo si querés reposicionar la cámara detrás del player instantáneamente.
    public void AlignBehindTarget()
    {
        if (!target) return;
        _yaw = target.eulerAngles.y;
    }

#if UNITY_EDITOR
    // Gizmo del radio de colisión (solo editor, no afecta runtime)
    void OnDrawGizmosSelected()
    {
        if (!target) return;
        Gizmos.color = new Color(0.2f, 0.8f, 1f, 0.3f);
        Gizmos.DrawWireSphere(target.position + targetOffset, 0.1f);
    }
#endif
}
