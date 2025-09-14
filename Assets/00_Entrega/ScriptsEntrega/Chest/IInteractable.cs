using UnityEngine;

public interface IInteractable
{
    /// Puede devolver false si ya fue usado/abierto.
    bool CanInteract(Transform interactor);

    /// Acción principal (se asume que el interactor está en rango).
    void Interact(Transform interactor);

    /// Posición “lógica” para priorizar el más cercano.
    Vector3 Position { get; }
}
