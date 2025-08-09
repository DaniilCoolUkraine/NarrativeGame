using SimpleEventBus.SimpleEventBus.Runtime;

namespace NarrativeGame.Puzzles.Core
{
    public class PuzzleGroupSolvedEvent : IEvent
    {
        public PuzzleGroupSolvedEvent(int puzzleGroupId)
        {
            PuzzleGroupId = puzzleGroupId;
        }

        public int PuzzleGroupId { get; }
    }
}