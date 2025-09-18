using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] Transform target;
    [SerializeField] Vector3 targetOffset = new Vector3(0f, 1.6f, 0f);

    /* distancia */
    [SerializeField] float distance = 6.5f;
    [SerializeField] float minDistance = 3f;
    [SerializeField] float maxDistance = 10f;

    /* Orbita */
    [SerializeField] float mouseSensitivity = 120f;
    [SerializeField] float pitchMin = -20f;
    [SerializeField] float pitchMax = 70f;

    /* Suavizado */
    [SerializeField] float followLerp = 12f;

    /* Colisiones */
    [SerializeField] float collisionRadius = 0.2f;
    [SerializeField] LayerMask collisionMask = ~0;

    private bool invertX = false;
    private bool invertY = false;
    private bool lockCursor = true;

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
        if (target) _yaw = target.eulerAngles.y;
        _pitch = Mathf.Clamp(_pitch, pitchMin, pitchMax);
        distance = Mathf.Clamp(distance, minDistance, maxDistance);
    }

    void LateUpdate()
    {
        if (!target) return;

        float dt = Time.deltaTime;

        float mx = Input.GetAxis("Mouse X");
        float my = Input.GetAxis("Mouse Y");

        float signX = invertX ? -1f : 1f;
        float signY = invertY ? 1f : -1f;

        _yaw += mx * mouseSensitivity * dt * signX;
        _pitch += my * mouseSensitivity * dt * signY;
        _pitch = Mathf.Clamp(_pitch, pitchMin, pitchMax);

        float wheel = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(wheel) > 0.0001f) distance = Mathf.Clamp(distance - wheel * 2f, minDistance, maxDistance);

        Quaternion rot = Quaternion.Euler(_pitch, _yaw, 0f);

        Vector3 pivot = target.position + targetOffset;
        Vector3 idealCamPos = pivot - (rot * Vector3.forward) * distance;

        float adjustedDist = distance;
        Vector3 toCam = idealCamPos - pivot;
        float len = toCam.magnitude;
        if (len > 0.001f)
        {
            Vector3 dir = toCam / len;
            if (Physics.SphereCast(pivot, collisionRadius, dir, out RaycastHit hit, distance, collisionMask, QueryTriggerInteraction.Ignore))
                adjustedDist = Mathf.Max(0.1f, hit.distance - 0.05f);
        }

        Vector3 targetCamPos = pivot - (rot * Vector3.forward) * adjustedDist;

        float t = 1f - Mathf.Exp(-followLerp * dt);
        transform.position = Vector3.Lerp(transform.position, targetCamPos, t);
        transform.rotation = Quaternion.LookRotation(pivot - transform.position, Vector3.up);
    }

    public void AlignBehindTarget()
    {
        if (!target) return;
        _yaw = target.eulerAngles.y;
    }
}