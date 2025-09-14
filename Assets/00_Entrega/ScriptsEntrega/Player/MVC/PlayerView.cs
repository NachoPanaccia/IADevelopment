using System.Linq;
using UnityEngine;

public class PlayerView : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [Tooltip("Ra�z visual (mesh/rig). Si se deja vac�o se usa el Transform del Animator.")]
    [SerializeField] private Transform modelRoot;

    public Animator Animator => animator;
    public Transform ModelRoot => modelRoot != null ? modelRoot : animator.transform;

    int speedHash;
    bool hasSpeedParam;

    void Awake()
    {
        speedHash = Animator.StringToHash("Speed");
        // Evita warnings si el par�metro no existe en tu Animator
        hasSpeedParam = animator != null && animator.parameters.Any(p => p.nameHash == speedHash);
    }

    public void PlayIdle() => animator.CrossFade("Idle", 0.1f);
    public void PlayWalk() => animator.CrossFade("Walk", 0.1f);
    public void PlayRun() => animator.CrossFade("Run", 0.1f);
    public void PlayRunToStop() => animator.CrossFade("RunToStop", 0.05f);
    public void PlayPunch() => animator.CrossFade("Punch", 0.05f);

    // �til si despu�s quer�s blend tree por velocidad (seguro contra par�metros inexistentes)
    public void SetSpeedParam(float value)
    {
        if (hasSpeedParam) animator.SetFloat(speedHash, value);
    }

    public bool IsAnimFinished(string stateName)
    {
        var info = animator.GetCurrentAnimatorStateInfo(0);
        return info.IsName(stateName) && info.normalizedTime >= 1f && !animator.IsInTransition(0);
    }
}
