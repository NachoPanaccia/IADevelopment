using UnityEngine;

public interface IInteractable
{
    bool CanInteract(Transform interactor);

    void Interact(Transform interactor);

    // Posición en el mundo
    Vector3 Position { get; }
}
