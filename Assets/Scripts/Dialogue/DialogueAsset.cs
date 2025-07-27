using NarrativeGame.Interactions.Extendables.Events;
using SimpleEventBus.SimpleEventBus.Runtime;
using UnityEngine;

namespace NarrativeGame.Dialogue
{
    [CreateAssetMenu(fileName = nameof(DialogueAsset), menuName = "Dialogue/" + nameof(DialogueAsset))]
    public class DialogueAsset : ScriptableObject
    {
        [SerializeField, TextArea] private string[] _dialogueLines;
        
        private int _currentLineIndex;
        
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