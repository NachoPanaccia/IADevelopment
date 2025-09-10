using UnityEngine;

[System.Serializable]
public class PlayerModel
{
    [Header("Movimiento")]
    public float walkSpeed = 2.5f;   // velocidad tope al caminar
    public float runSpeed = 5.0f;   // velocidad tope al correr (más alta)
    public float rotationLerp = 12f;

    // último input leído (x,z)
    public Vector3 InputVector { get; set; }

    [Header("Rampa 0..1 (suavizado)")]
    [Range(0f, 1f)] public float speedFactor = 0f;  // 0 reposo, 1 tope
    public float accel = 4f;
    public float decel = 6f;

    // Suaviza aceleración / frenado (no decide estados)
    public void StepFactor(bool isMoving)
    {
        float target = isMoving ? 1f : 0f;
        float rate = isMoving ? accel : decel;
        speedFactor = Mathf.MoveTowards(speedFactor, target, rate * Time.deltaTime);
    }

    // Velocidad efectiva según estado
    public float GetSpeed(bool running)
    {
        return (running ? runSpeed : walkSpeed) * speedFactor;
    }
}
