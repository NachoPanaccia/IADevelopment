using UnityEngine;

/// Datos del jugador sin lógica.
[System.Serializable]
public class PlayerModel
{
    [Header("Movimiento")]
    public float walkSpeed = 2.6f;
    public float runSpeed = 5.2f;
    public float rotationLerp = 12f;
}
