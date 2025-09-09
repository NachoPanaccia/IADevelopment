using UnityEngine;

public class PlayerView : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [Tooltip("Ra�z visual (mesh/rig). Si se deja vac�o se usa el Transform del Animator.")]
    [SerializeField] private Transform modelRoot;

    public Animator Animator => animator;
    public Transform ModelRoot => modelRoot != null ? modelRoot : animator.transform;

    public void PlayIdle() => animator.CrossFade("Idle", 0.1f);
    public void PlayWalk() => animator.CrossFade("Walk", 0.1f);

    // �til si despu�s quer�s blend tree por velocidad
    public void SetSpeedParam(float value) => animator.SetFloat("Speed", value);
}
