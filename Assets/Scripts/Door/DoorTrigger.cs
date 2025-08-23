using NarrativeGame.Door.Events;
using NarrativeGame.Interactions.Extendables.Interactables;
using SimpleEventBus.SimpleEventBus.Runtime;
using UnityEngine;

namespace NarrativeGame.Door
{
    public class DoorTrigger : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<CarryInteractable>(out var carryInteractable)) 
                GlobalEvents.Publish(new CarryInteractableInTriggerEvent(carryInteractable.Id));
        }
    }
}