using SimpleEventBus.SimpleEventBus.Runtime;

namespace NarrativeGame.Interactions.Core
{
    public class InteractorInZoneEvent : IEvent
    {
        public InteractorInZoneEvent(IInteractor interactor)
        {
            Interactor = interactor;
        }

        public IInteractor Interactor { get; }
    }
}