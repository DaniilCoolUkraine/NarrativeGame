using NarrativeGame.Puzzles.Core.Interfaces;
using SimpleEventBus.SimpleEventBus.Runtime;

namespace NarrativeGame.Puzzles.Core
{
    public class PuzzlePartSolvedEvent : IEvent
    {
        public PuzzlePartSolvedEvent(IPuzzle puzzle)
        {
            Puzzle = puzzle;
        }

        public IPuzzle Puzzle { get; }
    }
}