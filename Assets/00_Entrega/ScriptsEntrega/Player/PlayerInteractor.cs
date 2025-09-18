using UnityEngine;

public class PlayerInteractor : MonoBehaviour
{
    [Header("Interacción")]
    public float interactRadius = 1.8f;
    public LayerMask layerMask = ~0;
    public bool showGizmo = true;

    IInteractable _current;
    IShowPrompt _currentPrompt;

    void Update()
    {
        UpdateFocus();
        if (Input.GetKeyDown(KeyCode.E) && _current != null) _current.Interact(transform);
    }

    void UpdateFocus()
    {
        var hits = Physics.OverlapSphere(transform.position, interactRadius, layerMask, QueryTriggerInteraction.Collide);

        IInteractable closest = null;
        IShowPrompt closestPrompt = null;
        float bestSqr = float.MaxValue;

        foreach (var h in hits)
        {
            var interactable = h.GetComponentInParent<IInteractable>();
            if (interactable == null) continue;
            if (!interactable.CanInteract(transform)) continue;

            float sqr = (interactable.Position - transform.position).sqrMagnitude;
            if (sqr < bestSqr)
            {
                bestSqr = sqr;
                closest = interactable;
                closestPrompt = h.GetComponentInParent<IShowPrompt>();
            }
        }

        if (!ReferenceEquals(_currentPrompt, closestPrompt))
            _currentPrompt?.SetPromptVisible(false);

        _current = closest;
        _currentPrompt = closestPrompt;

        if (_current != null) _currentPrompt?.SetPromptVisible(true);
        else _currentPrompt?.SetPromptVisible(false);
    }

    void OnDrawGizmosSelected()
    {
        if (!showGizmo) return;
        Gizmos.color = new Color(0.2f, 0.8f, 1f, 0.35f);
        Gizmos.DrawSphere(transform.position, interactRadius);
        Gizmos.color = new Color(0.1f, 0.5f, 1f, 0.9f);
        Gizmos.DrawWireSphere(transform.position, interactRadius);
    }
}