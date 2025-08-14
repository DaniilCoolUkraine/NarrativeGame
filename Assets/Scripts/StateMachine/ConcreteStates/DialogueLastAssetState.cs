using NarrativeGame.Interactions.Extendables.Events;
using NarrativeGame.Interactions.Extendables.Interactables;
using SimpleEventBus.SimpleEventBus.Runtime;

namespace NarrativeGame.StateMachine.ConcreteStates
{
    public class DialogueLastAssetState : IState
    {
        private DialogueInteractable _interactable;

        public DialogueLastAssetState(DialogueInteractable interactable)
        {
            _interactable = interactable;
        }
        
        public void Enter()
        {
            GlobalEvents.AddListener<DialogueEndEvent>(OnDialogueEnd);
        }

        public void Exit()
        {
            GlobalEvents.RemoveListener<DialogueEndEvent>(OnDialogueEnd);
        }

        public void Update()
        {
        }

        private void OnDialogueEnd(DialogueEndEvent ev)
        {
            _interactable.gameObject.SetActive(false);
        }
    }
}