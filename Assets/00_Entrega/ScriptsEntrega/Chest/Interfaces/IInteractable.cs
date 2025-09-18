using UnityEngine;

public interface IInteractable
{
    bool CanInteract(Transform interactor);
    void Interact(Transform interactor);
    Vector3 Position { get; }
}