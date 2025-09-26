using UnityEngine;

public class ChestSpawnPoint : MonoBehaviour
{
    [Header("Peso en la ruleta de aparición")]
    [Min(0f)] public float peso = 1f;

    public Vector3 Position
    {
        get { return transform.position; }
    }
}
