using System.Collections;
using UnityEngine;

[RequireComponent(typeof(ChestView))]
public class ChestController : MonoBehaviour, IInteractable
{
    public static event System.Action<ChestController> OnAnyChestOpened;

    [Header("Referencias")]
    [SerializeField] private ChestView view;
    [SerializeField] private ChestPressedLogic pressedLogic;
    [SerializeField] private ChestPromptView promptView;

    [Header("Estado")]
    [SerializeField] private bool opened = false;  // ya quedó en Chest_Press
    bool opening = false;                          // anim de apertura en curso

    public Vector3 Position => transform.position;

    void Reset()
    {
        view = GetComponent<ChestView>();
        pressedLogic = GetComponent<ChestPressedLogic>();
        promptView = GetComponentInChildren<ChestPromptView>();
    }

    void Awake()
    {
        if (!view) view = GetComponent<ChestView>();
        if (!promptView) promptView = GetComponentInChildren<ChestPromptView>();

        if (!opened) { view.PlayIdle(); promptView?.SetPromptVisible(false); }
        else { view.PlayPress(); promptView?.SetPromptVisible(false); }
    }

    public bool CanInteract(Transform interactor) => !opened && !opening;

    public void Interact(Transform interactor)
    {
        if (!CanInteract(interactor)) return;
        promptView?.SetPromptVisible(false);
        StartCoroutine(OpenRoutine());
    }

    IEnumerator OpenRoutine()
    {
        opening = true;

        view.PlayOpen();
        yield return new WaitUntil(() => view.IsFinished("Chest_Open"));

        view.PlayPress();
        opened = true;
        opening = false;
        
        promptView?.SetPromptVisible(false);

        if (pressedLogic) pressedLogic.OnChestPressed();

        OnAnyChestOpened?.Invoke(this);
    }
}
