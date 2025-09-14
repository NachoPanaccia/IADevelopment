using System.Linq;
using UnityEngine;

public class PlayerInteractor : MonoBehaviour
{
    [Header("Interacción")]
    [Tooltip("Radio para buscar objetos interactuables (ej.: cofres)")]
    public float interactRadius = 1.8f;

    [Tooltip("Filtrar por Layer (opcional). Si lo dejás en 'Everything', igual funciona con IInteractable).")]
    public LayerMask layerMask = ~0; // Everything por defecto

    [Tooltip("Mostrar un gizmo del radio en escena")]
    public bool showGizmo = true;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
            TryInteract();
    }

    void TryInteract()
    {
        // Busco colliders alrededor del player
        var hits = Physics.OverlapSphere(transform.position, interactRadius, layerMask, QueryTriggerInteraction.Collide);

        IInteractable closest = null;
        float bestSqr = float.MaxValue;

        // Me quedo con el interactuable más cercano
        foreach (var h in hits)
        {
            var interactable = h.GetComponentInParent<IInteractable>();
            if (interactable == null) continue;
            if (!interactable.CanInteract(transform)) continue;

            float sqr = (interactable.Position - transform.position).sqrMagnitude;
            if (sqr < bestSqr) { bestSqr = sqr; closest = interactable; }
        }

        if (closest != null)
            closest.Interact(transform);
        // (si querés feedback visual/sonoro cuando no hay nada, lo agregamos)
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
