using SimpleEventBus.SimpleEventBus.Runtime;

namespace NarrativeGame.Door.Events
{
    public class CarryInteractableInTriggerEvent : IEvent
    {
        public CarryInteractableInTriggerEvent(int id)
        {
            Id = id;
        }

        public int Id { get; }
    }
}