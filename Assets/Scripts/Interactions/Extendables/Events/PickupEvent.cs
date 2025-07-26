using NarrativeGame.Interactions.Core;
using NarrativeGame.Interactions.Core.Interfaces;
using SimpleEventBus.SimpleEventBus.Runtime;

namespace NarrativeGame.Interactions.Extendables.Events
{
    public class PickupEvent : IEvent
    {
        public IInteractor Interactor { get; }

        public PickupEvent(IInteractor interactor)
        {
            Interactor = interactor;
        }
    }
}