using NarrativeGame.Interactions.Core.Interfaces;
using NarrativeGame.Interactions.Core.Samples.Interactables;
using NarrativeGame.Interactions.Extendables.Events;
using SimpleEventBus.SimpleEventBus.Runtime;

namespace NarrativeGame.Interactions.Extendables.Interactables
{
    public class DialogueInteractable : Interactable
    {
        public override bool CanInteract(IInteractor interactor)
        {
            return interactor.CanInteract(this);
        }

        public override void Interact(IInteractor interactor)
        {
            GlobalEvents.Publish(new DialogueStartEvent(this, interactor));
        }

        public override void CancelInteract(IInteractor interactor)
        {
            interactor.ResetInteract();
        }
    }
}