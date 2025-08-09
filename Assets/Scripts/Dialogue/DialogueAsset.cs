using System;
using NarrativeGame.Interactions.Extendables.Events;
using SimpleEventBus.SimpleEventBus.Runtime;
using Sirenix.OdinInspector;
using UnityEngine;

namespace NarrativeGame.Dialogue
{
    [CreateAssetMenu(fileName = nameof(DialogueAsset), menuName = "Dialogue/" + nameof(DialogueAsset))]
    public class DialogueAsset : ScriptableObject
    {
        [SerializeField, Required, PreviewField] private Sprite _image;
        [SerializeField, TextArea] private string[] _dialogueLines;

        public Sprite Image => _image;
        private int _currentLineIndex;

        private void Awake()
        {
            _currentLineIndex = 0;
        }

        public string GetNextLine()
        {
            if (_currentLineIndex >= _dialogueLines.Length)
            {
                GlobalEvents.Publish(new DialogueEndEvent());
                _currentLineIndex = 0;
                return null;
            }

            string line = _dialogueLines[_currentLineIndex];
            _currentLineIndex++;
            return line;
        }
    }
}