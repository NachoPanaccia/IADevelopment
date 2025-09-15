using UnityEngine;

public interface IInteractable
{
    /// Puede devolver false si ya fue usado/abierto.
    bool CanInteract(Transform interactor);

    /// Acci�n principal (se asume que el interactor est� en rango).
    void Interact(Transform interactor);

    /// Posici�n �l�gica� para priorizar el m�s cercano.
    Vector3 Position { get; }
}
