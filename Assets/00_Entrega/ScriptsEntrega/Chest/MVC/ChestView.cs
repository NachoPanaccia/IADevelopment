using UnityEngine;

public class ChestView : MonoBehaviour
{
    [SerializeField] private Animator animator;

    public void PlayIdle() => animator.CrossFade("Chest_Idle", 0.05f);
    public void PlayOpen() => animator.CrossFade("Chest_Open", 0.05f);
    public void PlayPress() => animator.CrossFade("Chest_Press", 0.05f);

    public bool IsFinished(string stateName)
    {
        var info = animator.GetCurrentAnimatorStateInfo(0);
        return info.IsName(stateName) && info.normalizedTime >= 1f && !animator.IsInTransition(0);
    }
}
