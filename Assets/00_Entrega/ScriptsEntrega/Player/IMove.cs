using UnityEngine;

public interface IMove
{
    // direction debe venir normalizado en XZ
    void Move(Vector3 direction, float speed);
}
