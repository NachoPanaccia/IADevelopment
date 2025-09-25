using UnityEngine;

public class PlayerView : MonoBehaviour
{
    [SerializeField] Animator animator;     // Animator del prefab visual
    [SerializeField] Transform modelRoot;   // Raíz visual (para rotar el modelo)
    public Transform ModelRoot => modelRoot ? modelRoot : transform;

    // evita preguntar por nombres enb cada frame
    static readonly int IdleHash = Animator.StringToHash("Idle");
    static readonly int WalkHash = Animator.StringToHash("Walk");
    static readonly int RunHash = Animator.StringToHash("Run");
    static readonly int RunToStopHash = Animator.StringToHash("RunToStop");
    static readonly int PunchHash = Animator.StringToHash("Punch");

    public void PlayIdle() => Play(IdleHash);
    public void PlayWalk() => Play(WalkHash);
    public void PlayRun() => Play(RunHash);
    public void PlayRunToStop() => Play(RunToStopHash);
    public void PlayPunch() => Play(PunchHash);

    void Play(int stateHash, float fade = 0.1f, int layer = 0)
    {
        if (!animator) return;
        animator.CrossFade(stateHash, fade, layer);
    }

    public bool IsFinished(string stateName, int layer = 0) => IsFinished(Animator.StringToHash(stateName), layer);

    public bool IsFinished(int stateHash, int layer = 0)
    {
        if (!animator) return true;
        var st = animator.GetCurrentAnimatorStateInfo(layer);
        if (st.shortNameHash != stateHash) return false; // aún no estamos en ese state
        if (animator.IsInTransition(layer)) return false; // no contar si está transicionando
        return st.normalizedTime >= 0.99f;                 // 1.0 = fin; 0.99 evita bordes
    }
}
