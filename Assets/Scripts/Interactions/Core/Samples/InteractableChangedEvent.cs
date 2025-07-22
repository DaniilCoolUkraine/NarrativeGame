using SimpleEventBus.SimpleEventBus.Runtime;

namespace NarrativeGame.Interactions.Core
{
    public class InteractableChangedEvent : IEvent
    {
        public InteractableChangedEvent(IInteractable interactable, IInteractor interactor)
        {
            Interactable = interactable;
            Interactor = interactor;
        }

        public IInteractable Interactable { get; }
        public IInteractor Interactor { get; }
    }
}