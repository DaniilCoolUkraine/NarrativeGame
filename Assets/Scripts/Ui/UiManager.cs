using NarrativeGame.Interactions.Extendables.Events;
using SimpleEventBus.SimpleEventBus.Runtime;
using Sirenix.OdinInspector;
using UnityEngine;

namespace NarrativeGame.Ui
{
    public class UiManager : MonoBehaviour
    {
        [SerializeField, Required] private UiInteractionPanel _interactionPanel;
        [SerializeField, Required] private DialoguePanel _dialoguePanel;

        private bool _previousInteractionPanelState;
        
        private void OnEnable()
        {
            GlobalEvents.AddListener<DialogueStartEvent>(OnDialogueStart);
            GlobalEvents.AddListener<DialogueEndEvent>(OnDialogueEnd);
        }

        private void OnDisable()
        {
            GlobalEvents.RemoveListener<DialogueStartEvent>(OnDialogueStart);
            GlobalEvents.RemoveListener<DialogueEndEvent>(OnDialogueEnd);
        }

        private void OnDialogueStart(DialogueStartEvent ev)
        {
            _previousInteractionPanelState = _interactionPanel.gameObject.activeSelf;
            _interactionPanel.ForceUpdateState(false);
        }
        
        private void OnDialogueEnd(DialogueEndEvent ev)
        {
            _interactionPanel.ForceUpdateState(_previousInteractionPanelState);
        }
    }
}