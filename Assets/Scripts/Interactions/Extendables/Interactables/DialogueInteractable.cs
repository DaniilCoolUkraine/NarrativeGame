using System;
using NarrativeGame.Dialogue;
using NarrativeGame.Interactions.Core.Interfaces;
using NarrativeGame.Interactions.Core.Samples.Interactables;
using NarrativeGame.Interactions.Extendables.Events;
using NarrativeGame.StateMachine;
using NarrativeGame.StateMachine.ConcreteStates;
using SimpleEventBus.SimpleEventBus.Runtime;
using Sirenix.OdinInspector;
using UnityEngine;

namespace NarrativeGame.Interactions.Extendables.Interactables
{
    public class DialogueInteractable : Interactable
    {
        [SerializeField, Required] private DialogueAsset _dialogueAsset;

        private IInteractor   _interactor;
        private IStateMachine _stateMachine;

        private DialogueDefaultState   _defaultState;
        private DialogueLastAssetState _lastAssetState;

        private void Awake()
        {
            _defaultState = new DialogueDefaultState();
            _lastAssetState = new DialogueLastAssetState(this);
        }

        private void OnEnable()
        {
            _stateMachine = new StateMachine.StateMachine();
            _stateMachine.ChangeState(_defaultState);
            
            GlobalEvents.AddListener<DialogueAssetsEnd>(OnDialogueAssetsEnd);
        }

        private void OnDisable()
        {
            GlobalEvents.RemoveListener<DialogueAssetsEnd>(OnDialogueAssetsEnd);
        }

        public override bool CanInteract(IInteractor interactor)
        {
            return interactor.CanInteract(this);
        }

        public override void Interact(IInteractor interactor)
        {
            _interactor = interactor;
            GlobalEvents.Publish(new DialogueStartEvent(this, interactor, _dialogueAsset));

            GlobalEvents.AddListener<DialogueEndEvent>(OnDialogueEnd);
        }

        public override void CancelInteract(IInteractor interactor)
        {
            interactor.ResetInteract();
        }

        public void UpdateAsset(DialogueAsset newAsset)
        {
            _dialogueAsset = newAsset;
        }

        private void OnDialogueEnd(DialogueEndEvent ev)
        {
            CancelInteract(_interactor);
            GlobalEvents.RemoveListener<DialogueEndEvent>(OnDialogueEnd);
        }

        private void OnDialogueAssetsEnd(DialogueAssetsEnd ev)
        {
            if (ev.DialogueInteractable == this)
            {
                GlobalEvents.RemoveListener<DialogueAssetsEnd>(OnDialogueAssetsEnd);
                _stateMachine.ChangeState(_lastAssetState);
            }
        }
    }
}