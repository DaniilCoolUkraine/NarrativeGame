using System;
using System.Linq;
using NarrativeGame.Puzzles.Core;
using SimpleEventBus.SimpleEventBus.Runtime;
using Sirenix.OdinInspector;
using UnityEngine;

namespace NarrativeGame.Puzzles
{
    public class PuzzleManager : SerializedMonoBehaviour
    {
        [SerializeField, Required] private PuzzleData[] _puzzles;

        private void OnEnable()
        {
            GlobalEvents.AddListener<PuzzlePartSolvedEvent>(OnPuzzlePartSolved);
        }

        private void OnDisable()
        {
            GlobalEvents.RemoveListener<PuzzlePartSolvedEvent>(OnPuzzlePartSolved);
        }
        
        private void OnPuzzlePartSolved(PuzzlePartSolvedEvent ev)
        {
            var puzzleGroup = _puzzles.FirstOrDefault(g => g.Puzzles.Contains(ev.Puzzle));

            if (puzzleGroup == null)
            {
                Debug.LogWarning($"Puzzle part {ev.Puzzle} solved, but no puzzle group found for it.");
                return;
            }

            if (puzzleGroup.Solved)
            {
                GlobalEvents.Publish(new PuzzleGroupSolvedEvent(puzzleGroup.Id));
            }
        }
    }
}