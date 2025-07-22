using SimpleEventBus.SimpleEventBus.Runtime;

namespace NarrativeGame.Interactions.Core
{
    public class InteractableChangedEvent : IEvent
    {
        public InteractableChangedEvent(IInteractable interactable)
        {
            Interactable = interactable;
        }

        public IInteractable Interactable { get; }
    }
}