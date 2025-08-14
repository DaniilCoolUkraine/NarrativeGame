using NarrativeGame.Interactions.Extendables.Interactables;
using SimpleEventBus.SimpleEventBus.Runtime;

namespace NarrativeGame.Interactions.Extendables.Events
{
    public class DialogueAssetsEnd : IEvent
    {
        public DialogueAssetsEnd(DialogueInteractable dialogueInteractable)
        {
            DialogueInteractable = dialogueInteractable;
        }
        public DialogueInteractable DialogueInteractable { get; }
    }
}