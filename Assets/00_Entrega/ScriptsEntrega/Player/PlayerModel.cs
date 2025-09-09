using UnityEngine;

[System.Serializable]
public class PlayerModel
{
    [Header("Movimiento")]
    public float walkSpeed = 2.5f;
    public float rotationLerp = 12f;

    // buffer del último input (x,z)
    public Vector3 InputVector { get; set; }
}