using NarrativeGame.Interactions.Core.Interfaces;

namespace NarrativeGame.Interactions.Core.Samples.Interactables
{
    public abstract class InstantInteractable : Interactable
    {
        public override void Interact(IInteractor interactor)
        {
            ExecuteInteraction(interactor);
            CancelInteract(interactor);
        }

        public override void CancelInteract(IInteractor interactor)
        {
            interactor.ResetInteract();
        }

        protected abstract void ExecuteInteraction(IInteractor interactor);
    }
}