using UnityEngine;

public class PlayerView : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [Tooltip("Raíz visual (mesh/rig). Si se deja vacío se usa el Transform del Animator.")]
    [SerializeField] private Transform modelRoot;

    public Animator Animator => animator;
    public Transform ModelRoot => modelRoot != null ? modelRoot : animator.transform;

    public void PlayIdle() => animator.CrossFade("Idle", 0.1f);
    public void PlayWalk() => animator.CrossFade("Walk", 0.1f);

    // Útil si después querés blend tree por velocidad
    public void SetSpeedParam(float value) => animator.SetFloat("Speed", value);
}
