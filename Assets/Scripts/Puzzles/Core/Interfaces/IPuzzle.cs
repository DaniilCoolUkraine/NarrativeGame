namespace NarrativeGame.Puzzles.Core.Interfaces
{
    public interface IPuzzle
    {
        public bool Solved { get; }
        public void Solve();
    }
}