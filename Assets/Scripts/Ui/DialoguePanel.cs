using NarrativeGame.Dialogue;
using NarrativeGame.Interactions.Core.Interfaces;
using NarrativeGame.Interactions.Extendables.Events;
using SimpleEventBus.SimpleEventBus.Runtime;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NarrativeGame.Ui
{
    public class DialoguePanel : MonoBehaviour
    {
        [SerializeField, Required] private CanvasGroup _dialogueGroup;
        [SerializeField, Required] private TextMeshProUGUI _dialogueText;
        [SerializeField, Required] private Image _dialogueImage;

        private DialogueAsset _currentDialogue;
        private bool _isBusy;
        
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

        private void Update()
        {
            if (!_isBusy)
                return;
            
            if (Input.GetMouseButtonDown(0)) 
                UpdateLine();
        }

        private void OnDialogueStart(DialogueStartEvent ev)
        {
            _dialogueGroup.gameObject.SetActive(true);
            _currentDialogue = ev.DialogueAsset;
            _isBusy = true;

            _dialogueImage.sprite = ev.DialogueAsset.Image;

            UpdateLine();
        }
        
        private void OnDialogueEnd(DialogueEndEvent ev)
        {
            _dialogueGroup.gameObject.SetActive(false);
            _currentDialogue = null;

            _isBusy = false;
        }

        private void UpdateLine()
        {
            _dialogueText.text = $"— {_currentDialogue.GetNextLine()}";
        }
    }
}