using UnityEngine;

namespace NarrativeGame.Interactions.Core.Interfaces
{
    public interface IInteractor
    {
        public Transform transform { get; }

        public bool CanInteract(IInteractable pickupInteractable);
        public void Interact(IInteractable interactable);
        public void ResetInteract();
    }
}