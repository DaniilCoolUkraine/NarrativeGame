using System;

namespace NarrativeGame.Door
{
    public interface IUnlockStrategy
    {
        void Initialize(Action onUnlocked);
        void Dispose();
    }
}