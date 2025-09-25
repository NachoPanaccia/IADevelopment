using UnityEngine;

/// Los estados llaman Move(dir, speed)
public interface IMove
{
    void Move(Vector3 direction, float speed);
}
