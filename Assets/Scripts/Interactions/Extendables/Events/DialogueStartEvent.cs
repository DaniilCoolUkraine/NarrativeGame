using NarrativeGame.Interactions.Core.Interfaces;
using NarrativeGame.Interactions.Extendables.Interactables;
using SimpleEventBus.SimpleEventBus.Runtime;

namespace NarrativeGame.Interactions.Extendables.Events
{
    public class DialogueStartEvent : IEvent
    {
        public DialogueInteractable Interactable { get; }
        public IInteractor Interactor { get; }

        public DialogueStartEvent(DialogueInteractable interactable, IInteractor interactor)
        {
            Interactable = interactable;
            Interactor = interactor;
        }
    }
}