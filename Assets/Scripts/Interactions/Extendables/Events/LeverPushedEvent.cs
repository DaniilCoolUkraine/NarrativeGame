using SimpleEventBus.SimpleEventBus.Runtime;

namespace NarrativeGame.Interactions.Extendables.Events
{
    public class LeverPushedEvent : IEvent
    {
        public LeverPushedEvent(int leverId)
        {
            LeverId = leverId;
        }

        public int LeverId { get; }
    }
}