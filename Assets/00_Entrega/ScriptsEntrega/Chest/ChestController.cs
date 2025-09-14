using System.Collections;
using UnityEngine;

[RequireComponent(typeof(ChestView))]
public class ChestController : MonoBehaviour, IInteractable
{
    [Header("Referencias")]
    [SerializeField] private ChestView view;
    [SerializeField] private ChestPressedLogic pressedLogic; // opcional

    [Header("Estado")]
    [SerializeField] private bool opened = false;  // ya quedó en Chest_Press
    bool opening = false;                          // anim de apertura en curso

    public Vector3 Position => transform.position;

    void Reset()
    {
        view = GetComponent<ChestView>();
        pressedLogic = GetComponent<ChestPressedLogic>();
    }

    void Awake()
    {
        if (!view) view = GetComponent<ChestView>();
        // Aseguramos estado inicial:
        if (!opened) view.PlayIdle();
        else view.PlayPress();
    }

    public bool CanInteract(Transform interactor) => !opened && !opening;

    public void Interact(Transform interactor)
    {
        if (!CanInteract(interactor)) return;
        StartCoroutine(OpenRoutine());
    }

    IEnumerator OpenRoutine()
    {
        opening = true;

        // 1) Pasamos a "Open" y esperamos a que termine
        view.PlayOpen();
        yield return new WaitUntil(() => view.IsFinished("Chest_Open"));

        // 2) Pasamos a "Press" y quedamos ahí para siempre
        view.PlayPress();
        opened = true;
        opening = false;

        // 3) Dispará la lógica de “cofre resuelto”
        if (pressedLogic) pressedLogic.OnChestPressed();
    }
}
