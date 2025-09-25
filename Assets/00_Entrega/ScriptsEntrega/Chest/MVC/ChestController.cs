using System.Collections;
using UnityEngine;

// tira OnAnyChestOpened para que LevelManager sepa
[RequireComponent(typeof(ChestView))]
public class ChestController : MonoBehaviour, IInteractable
{
    public static event System.Action<ChestController> OnAnyChestOpened;

    [Header("Referencias")]
    [SerializeField] private ChestView view;                 // anim
    [SerializeField] private ChestPressedLogic pressedLogic; // Ruleta y disparo del premio
    [SerializeField] private ChestPromptView promptView;

    [Header("Estado")]
    [SerializeField] private bool opened;
    private bool opening;

    public Vector3 Position => transform.position;

    private void Reset()
    {
        view = GetComponent<ChestView>();
        pressedLogic = GetComponent<ChestPressedLogic>();
        promptView = GetComponentInChildren<ChestPromptView>();
    }

    private void Awake()
    {
        if (!view)
        {
            view = GetComponent<ChestView>();
        }
        if (!promptView)
        {
            promptView = GetComponentInChildren<ChestPromptView>();
        }

        if (!opened)
        {
            view.PlayIdle();
            promptView?.SetPromptVisible(false);
        }
        else
        {
            view.PlayPress();
            promptView?.SetPromptVisible(false);
        }
    }

    public bool CanInteract(Transform interactor)
    {
        return !opened && !opening;
    }

    public void Interact(Transform interactor)
    {
        if (!CanInteract(interactor))
        {
            return;
        }

        promptView?.SetPromptVisible(false);
        StartCoroutine(OpenRoutine());
    }

    private IEnumerator OpenRoutine()
    {
        opening = true;

        view.PlayOpen();
        yield return new WaitUntil(() => view.IsFinished("Chest_Open"));

        view.PlayPress();
        opened = true;
        opening = false;

        promptView?.SetPromptVisible(false);

        if (pressedLogic)
        {
            pressedLogic.OnChestPressed();
        }
        OnAnyChestOpened?.Invoke(this);
    }
}
