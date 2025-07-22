using SimpleEventBus.SimpleEventBus.Runtime;

namespace NarrativeGame.Animations.Events
{
    public class MovementEvent : IEvent
    {
        public MovementEvent(float speed, float speedChangeRate, float inputMagnitude)
        {
            Speed = speed;
            SpeedChangeRate = speedChangeRate;
            InputMagnitude = inputMagnitude;
        }

        public float Speed { get; }
        public float SpeedChangeRate { get; }
        public float InputMagnitude { get; }
    }
}