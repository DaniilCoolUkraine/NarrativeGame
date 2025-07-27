using NarrativeGame.Dialogue;
using NarrativeGame.Interactions.Core.Interfaces;
using NarrativeGame.Interactions.Extendables.Interactables;
using SimpleEventBus.SimpleEventBus.Runtime;

namespace NarrativeGame.Interactions.Extendables.Events
{
    public class DialogueStartEvent : IEvent
    {
        public DialogueInteractable Interactable { get; }
        public IInteractor Interactor { get; }
        public DialogueAsset DialogueAsset { get; }

        public DialogueStartEvent(DialogueInteractable interactable, IInteractor interactor, DialogueAsset dialogueAsset)
        {
            Interactable = interactable;
            Interactor = interactor;
            DialogueAsset = dialogueAsset;
        }
    }
}