using SimpleEventBus.SimpleEventBus.Runtime;
using UnityEngine;

namespace NarrativeGame.Interactions.Core
{
    public abstract class Interactable : MonoBehaviour, IInteractable
    {
        public abstract bool CanInteract(IInteractor interactor);
        public abstract void Interact(IInteractor interactor);
        public abstract void CancelInteract(IInteractor interactor);
        public abstract string InteractDescription();

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<IInteractor>(out var interactor)) 
                GlobalEvents.Publish(new InteractorInZoneEvent(interactor));
        }
    }
}