using NarrativeGame.Dialogue;
using NarrativeGame.Interactions.Extendables.Events;
using NarrativeGame.Puzzles.Core;
using SimpleEventBus.SimpleEventBus.Runtime;
using UnityEditor;
using UnityEngine;

namespace NarrativeGame.Interactions.Extendables.Interactables
{
    [RequireComponent(typeof(DialogueInteractable))]
    public class DialogueAssetSwitcher : MonoBehaviour
    {
        [SerializeField] private int[]                _puzzleGroupIds;
        [SerializeField] private DialogueAsset[]      _dialogueAssets;
        [SerializeField] private DialogueInteractable _dialogueInteractable;

        private int _currentDialogueAssetIndex;

        #if UNITY_EDITOR
        private void Reset()
        {
            _dialogueInteractable = GetComponent<DialogueInteractable>();
            EditorUtility.SetDirty(this);
        }
        #endif

        private void OnEnable()
        {
            GlobalEvents.AddListener<PuzzleGroupSolvedEvent>(OnPuzzleGroupSolved);
        }

        private void OnDisable()
        {
            GlobalEvents.RemoveListener<PuzzleGroupSolvedEvent>(OnPuzzleGroupSolved);
        }

        private void OnPuzzleGroupSolved(PuzzleGroupSolvedEvent ev)
        {
            if (_puzzleGroupIds[_currentDialogueAssetIndex] != ev.PuzzleGroupId) 
                return;

            if (_currentDialogueAssetIndex < _dialogueAssets.Length)
            {
                _dialogueInteractable.UpdateAsset(_dialogueAssets[_currentDialogueAssetIndex]);
                _currentDialogueAssetIndex++;
            }

            if (_currentDialogueAssetIndex >= _dialogueAssets.Length)
            {
                GlobalEvents.RemoveListener<PuzzleGroupSolvedEvent>(OnPuzzleGroupSolved);
                GlobalEvents.Publish(new DialogueAssetsEnd(_dialogueInteractable));
            }
        }
    }
}