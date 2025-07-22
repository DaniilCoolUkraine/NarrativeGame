using UnityEngine;

namespace NarrativeGame.Interactions.Core
{
    public interface IInteractable
    {
        public Transform transform { get; }
        
        public bool CanInteract(IInteractor interactor);
        public void Interact(IInteractor interactor);
        public void CancelInteract(IInteractor interactor);

        public string InteractDescription();
    }
}