using NarrativeGame.Interactions.Core.Interfaces;
using NarrativeGame.Interactions.Core.Samples.Events;
using NarrativeGame.Interactions.Extendables.Events;
using SimpleEventBus.SimpleEventBus.Runtime;
using Sirenix.OdinInspector;
using UnityEngine;

namespace NarrativeGame.Ui
{
    public class DialoguePanel : MonoBehaviour
    {
        [SerializeField, Required] private CanvasGroup _dialogueGroup;

        private IInteractable _currentInteractable;

        private void OnEnable()
        {
            GlobalEvents.AddListener<DialogueStartEvent>(OnDialogueStart);
        }

        private void OnDisable()
        {
            GlobalEvents.RemoveListener<DialogueStartEvent>(OnDialogueStart);
        }

        private void OnDialogueStart(DialogueStartEvent ev)
        {
            GlobalEvents.AddListener<InteractableChangedEvent>(OnInteractableChanged);
            
            _dialogueGroup.gameObject.SetActive(true);
            _currentInteractable = ev.Interactable;
        }

        private void OnInteractableChanged(InteractableChangedEvent ev)
        {
            if (_currentInteractable == ev.Interactable) return;

            _dialogueGroup.gameObject.SetActive(false);
            GlobalEvents.RemoveListener<InteractableChangedEvent>(OnInteractableChanged);
        }
    }
}