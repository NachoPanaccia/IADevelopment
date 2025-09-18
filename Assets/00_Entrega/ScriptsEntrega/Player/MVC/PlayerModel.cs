using UnityEngine;

[System.Serializable]
public class PlayerModel
{
    [Header("Movimiento")]
    public float walkSpeed = 7f;
    public float runSpeed = 13f;
    public float rotationLerp = 15f;

    public Vector3 InputVector { get; set; }

    [Header("Rampa 0..1 (suavizado)")]
    [Range(0f, 1f)] public float speedFactor = 0f;
    public float accel = 4f;
    public float decel = 6f;

    public void StepFactor(bool isMoving)
    {
        float target = isMoving ? 1f : 0f;
        float rate = isMoving ? accel : decel;
        speedFactor = Mathf.MoveTowards(speedFactor, target, rate * Time.deltaTime);
    }

    public float GetSpeed(bool running)
    {
        return (running ? runSpeed : walkSpeed) * speedFactor;
    }
}
