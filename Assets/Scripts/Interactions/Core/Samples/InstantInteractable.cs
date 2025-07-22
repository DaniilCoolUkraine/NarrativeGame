namespace NarrativeGame.Interactions.Core
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