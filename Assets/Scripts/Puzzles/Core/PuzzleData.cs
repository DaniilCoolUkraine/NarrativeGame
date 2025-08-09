using System.Linq;
using NarrativeGame.Puzzles.Core.Interfaces;
using Sirenix.OdinInspector;

namespace NarrativeGame.Puzzles
{
    public class PuzzleData
    {
        [HorizontalGroup("Name")] public string Name;
        [HorizontalGroup("Name")] public int Id;
        [Required] public IPuzzle[] Puzzles;

        public bool Solved => Puzzles.All(p => p.Solved);
    }
}