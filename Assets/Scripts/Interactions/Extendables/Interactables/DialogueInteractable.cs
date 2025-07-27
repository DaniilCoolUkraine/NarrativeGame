using System;
using NarrativeGame.Dialogue;
using NarrativeGame.Interactions.Core.Interfaces;
using NarrativeGame.Interactions.Core.Samples.Interactables;
using NarrativeGame.Interactions.Extendables.Events;
using SimpleEventBus.SimpleEventBus.Runtime;
using Sirenix.OdinInspector;
using UnityEngine;

namespace NarrativeGame.Interactions.Extendables.Interactables
{
    public class DialogueInteractable : Interactable
    {
        [SerializeField, Required] private DialogueAsset _dialogueAsset;
        private IInteractor _interactor;

        private void OnEnable()
        {
            GlobalEvents.AddListener<DialogueEndEvent>(OnDialogueEnd);
        }

        private void OnDisable()
        {
            GlobalEvents.RemoveListener<DialogueEndEvent>(OnDialogueEnd);
        }

        public override bool CanInteract(IInteractor interactor)
        {
            return interactor.CanInteract(this);
        }

        public override void Interact(IInteractor interactor)
        {
            _interactor = interactor;
            GlobalEvents.Publish(new DialogueStartEvent(this, interactor, _dialogueAsset));
        }

        public override void CancelInteract(IInteractor interactor)
        {
            interactor.ResetInteract();
        }

        private void OnDialogueEnd(DialogueEndEvent ev)
        {
            CancelInteract(_interactor);
        }
    }
}