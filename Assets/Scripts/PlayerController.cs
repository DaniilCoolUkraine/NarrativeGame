using System;
using NarrativeGame.Interactions.Extendables.Events;
using SimpleEventBus.SimpleEventBus.Runtime;
using Sirenix.OdinInspector;
using UnityEngine;

namespace NarrativeGame
{
    public class PlayerController : SerializedMonoBehaviour
    {
        [SerializeField, Required] private IEnablable[] _enablables;

        private CursorController _cursorController;
        private bool _isInDialogue;
        
        private void OnEnable()
        {
            _cursorController = new CursorController();

            GlobalEvents.AddListener<DialogueStartEvent>(OnDialogueStart);
            GlobalEvents.AddListener<DialogueEndEvent>(OnDialogueEnd);
        }

        private void OnDisable()
        {
            GlobalEvents.RemoveListener<DialogueStartEvent>(OnDialogueStart);
            GlobalEvents.RemoveListener<DialogueEndEvent>(OnDialogueEnd);
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            if (_isInDialogue)
                return;

            _cursorController.LockCursor();
        }

        private void OnDialogueStart(DialogueStartEvent ev)
        {
            foreach (var enablable in _enablables) 
                enablable.Disable();

            _isInDialogue = true;
            _cursorController.UnlockCursor();
        }
        
        private void OnDialogueEnd(DialogueEndEvent ev)
        {
            foreach (var enablable in _enablables) 
                enablable.Enable();
            
            _isInDialogue = false;
            _cursorController.LockCursor();
        }

        private void SetCursorState(bool newState)
        {
            Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
        }
    }
}