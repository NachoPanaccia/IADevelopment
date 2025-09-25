using UnityEngine;

public class PlayerView : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] Transform modelRoot;
    public Transform ModelRoot => modelRoot ? modelRoot : transform;

    // hashes (más robusto que comparar strings cada frame)
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

    /// Devuelve true si la anim 'stateName' (no looping) ya terminó, sin transición.
    public bool IsFinished(string stateName, int layer = 0)
        => IsFinished(Animator.StringToHash(stateName), layer);

    public bool IsFinished(int stateHash, int layer = 0)
    {
        if (!animator) return true;
        var st = animator.GetCurrentAnimatorStateInfo(layer);
        if (st.shortNameHash != stateHash) return false;     // aún no estamos en ese state
        if (animator.IsInTransition(layer)) return false;    // si está transicionando, no contar
        return st.normalizedTime >= 0.99f;                   // 0.99 para evitar bordes
    }
}
