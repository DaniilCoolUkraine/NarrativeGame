using SimpleEventBus.SimpleEventBus.Runtime;

namespace NarrativeGame.Animations.Events
{
    public class GroundedEvent : IEvent
    {
        public GroundedEvent(bool grounded)
        {
            Grounded = grounded;
        }

        public bool Grounded { get; }
    }
}