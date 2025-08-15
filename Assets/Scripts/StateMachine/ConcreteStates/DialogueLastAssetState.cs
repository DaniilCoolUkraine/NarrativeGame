using System.Collections.Generic;
using NarrativeGame.Dialogue.Strategies;
using NarrativeGame.Interactions.Extendables.Events;
using NarrativeGame.Interactions.Extendables.Interactables;
using SimpleEventBus.SimpleEventBus.Runtime;

namespace NarrativeGame.StateMachine.ConcreteStates
{
    public class DialogueLastAssetState : IState
    {
        private readonly DialogueInteractable              _interactable;
        private readonly IEnumerable<IDialogueEndStrategy> _dialogueEndStrategies;

        public DialogueLastAssetState(DialogueInteractable interactable, IEnumerable<IDialogueEndStrategy> dialogueEndStrategies)
        {
            _interactable = interactable;
            _dialogueEndStrategies = dialogueEndStrategies;
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
            foreach (var strategy in _dialogueEndStrategies) 
                strategy.Execute();
        }
    }
}